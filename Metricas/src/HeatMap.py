import json
import os
import matplotlib.pyplot as plt
import matplotlib.image as mpimg

# Configuración
base_filename = "telemetria/telemetry_{}.json"
max_files = 100

valid_events = {
    "Quick_HMD_Move",
    "Quick_Jitter_Move"
}

# CONFIGURA TUS MAPAS AQUÍ
MAP_CONFIG = {
    "room_1": {
        "width": 110,
        "height": 36,
        "image": "mapas/Room0.png",
        "offset_x": 21,
        "offset_y": 18
    },
    "room_2": {
        "width": 173,
        "height": 38,
        "image": "mapas/Room1.png",
        "offset_x": 13,
        "offset_y": 19
    },
    "room_3": {
        "width": 123,
        "height": 42,
        "image": "mapas/Room2.png",
        "offset_x": 12,
        "offset_y": 12
    },
    "room_4": {
        "width": 235,
        "height": 59,
        "image": "mapas/Room3.png",
        "offset_x": 20,
        "offset_y": 16
    },
    "room_5": {
        "width": 235,
        "height": 59,
        "image": "mapas/Pasillo1.png",
        "offset_x": 20,
        "offset_y": 16
    },
    "room_6": {
        "width": 235,
        "height": 59,
        "image": "mapas/Pasillo2.png",
        "offset_x": 20,
        "offset_y": 16
    },
    "room_7": {
        "width": 235,
        "height": 59,
        "image": "mapas/Pasillo3.png",
        "offset_x": 20,
        "offset_y": 16
    },
}

# Crear heatmaps por nivel
heatmaps = {
    room: {
        event: [[0 for _ in range(MAP_CONFIG[room]["width"])] for _ in range(MAP_CONFIG[room]["height"])]
        for event in valid_events
    }
    for room in MAP_CONFIG
}

files_read = 0


def parse_concatenated_json(content):
    objects = []
    buffer = ""
    brace_count = 0

    for char in content:
        if char == "{":
            brace_count += 1
        if brace_count > 0:
            buffer += char
        if char == "}":
            brace_count -= 1
            if brace_count == 0:
                try:
                    objects.append(json.loads(buffer))
                except:
                    pass
                buffer = ""

    return objects


# Leer archivos
for i in range(max_files):
    filename = base_filename.format(i)

    if not os.path.exists(filename):
        continue

    files_read += 1

    with open(filename, "r", encoding="utf-8") as f:
        content = f.read()
        events = parse_concatenated_json(content)

        for event in events:
            if not isinstance(event, dict):
                continue

            event_type = event.get("eventType")
            if event_type not in valid_events:
                continue

            room_id = event.get("room_id")
            if room_id not in MAP_CONFIG:
                continue

            pos = event.get("player_position")
            if not pos:
                continue

            x = pos.get("x")
            y = pos.get("y")

            if x is None or y is None:
                continue
            config = MAP_CONFIG[room_id]

            map_width = config["width"]
            map_height = config["height"]

            # Convertir a grid
            offset_x = config["offset_x"]
            offset_y = config["offset_y"]

            # Aplicar offset
            x_adj = x + offset_x
            y_adj = y + offset_y

            # Normalizar
            grid_x = int((x_adj / map_width) * (config["width"] - 1))
            grid_y = int((y_adj / map_height) * (config["height"] - 1))

            grid_x = max(0, min(config["width"] - 1, grid_x))
            grid_y = max(0, min(config["height"] - 1, grid_y))

            heatmaps[room_id][event_type][grid_y][grid_x] += 1


print(f"Archivos leídos: {files_read}")

# Crear carpeta salida
os.makedirs("graficos", exist_ok=True)

# Generar heatmaps por nivel
for room_id, events_dict in heatmaps.items():
    config = MAP_CONFIG[room_id]
    image_path = config["image"]

    if not os.path.exists(image_path):
        print(f"No se encuentra imagen para {room_id}")
        continue

    img = mpimg.imread(image_path)

    for event_type, heatmap in events_dict.items():

        plt.figure()

        # Imagen base
        plt.imshow(img, extent=[0, config["width"], 0, config["height"]])
        if not all(all(cell == 0 for cell in row) for row in heatmap):
            # Heatmap
            plt.imshow(
                heatmap,
                cmap='hot',
                alpha=0.5,
                origin='lower',
                extent=[0, config["width"], 0, config["height"]]
            )

        plt.colorbar(label="Frecuencia")
        plt.title(f"{room_id} - {event_type}")
        plt.axis('off')

        plt.tight_layout()

        filename = f"graficos/heatmap_{room_id}_{event_type}.png"
        plt.savefig(filename)
        plt.close()