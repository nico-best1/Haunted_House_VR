"""
spatial_heatmaps.py
===================
Parte 4 – Visualización espacial y mapas de calor
Responsable: Javi

Desarrolla mapas de calor espaciales (M4) de los eventos de movimiento rápido.
Agrega datos de múltiples sesiones para generar heatmaps globales.

Dependencias:
    pip install pandas numpy matplotlib seaborn pillow
"""

import argparse
from pathlib import Path
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from PIL import Image

# Formato: [x_min, x_max, z_min, z_max]
ROOM_BOUNDS = {
    "Room0": [6, 16.6, 0.6, 11],
    "Room1": [25.9, 36, -3.8, 9.7],
    "Room2": [25.7, 36.5, -17.3, -3.9],
    "Room3": [22, 33.6, -22.9, -12.7],
    "Pasillo1": [0, 22, -5, 0.6],
    "Pasillo2": [18, 25.6, -22.8, 11.7],
    "Pasillo3": [30, 37.5, -21.3, 11.7]
}

# Carga de datos

def load_telemetry(csv_path: str) -> pd.DataFrame:
    """Carga los datos de telemetría manejando el formato del TrackerManager."""
    try:
        df = pd.read_csv(
            csv_path,
            sep=",",
            header=None,
            names=["event_name", "session_id", "event_id", "timestamp_ms", "pos_x", "pos_y", "pos_z"],
            engine="python",
            on_bad_lines="warn"
        )
    except Exception as e:
        print(f"[ERROR] No se pudo leer {csv_path}: {e}")
        return pd.DataFrame()
        
    df["timestamp_ms"] = pd.to_numeric(df["timestamp_ms"], errors="coerce")
    df.dropna(subset=["event_name", "timestamp_ms"], inplace=True)
    
    # Limpiar columnas pos_x, pos_y, pos_z de los ';' finales
    for col in ["pos_x", "pos_y", "pos_z"]:
        df[col] = df[col].astype(str).str.replace(';', '', regex=False)
        df[col] = pd.to_numeric(df[col], errors="coerce")
        
    df["event_name"] = df["event_name"].str.strip()
    df.sort_values("timestamp_ms", inplace=True)
    df.reset_index(drop=True, inplace=True)
    return df

def assign_rooms(df: pd.DataFrame) -> pd.DataFrame:
    """Asigna la sala activa basada en los eventos Enter_<room>."""
    rooms = []
    current_room = "Unknown"
    for _, row in df.iterrows():
        name = row["event_name"]
        if name.startswith("Enter_"):
            current_room = name[6:]
        rooms.append(current_room)
    df["room"] = rooms
    return df

def process_all_sessions(folder_path: str) -> pd.DataFrame:
    """Busca y procesa todos los CSVs de telemetría en la carpeta indicada."""
    base_dir = Path(folder_path)
    all_files = list(base_dir.glob("telemetry_*.csv"))
    
    if not all_files:
        print(f"[ERROR] No se encontraron archivos 'telemetry_*.csv' en el directorio: {folder_path}")
        return pd.DataFrame()
        
    print(f"[INFO] Encontrados {len(all_files)} archivos de telemetría. Combinando datos...")
    
    df_list = []
    for file_path in all_files:
        df_session = load_telemetry(str(file_path))
        if not df_session.empty:
            # Es vital asignar las salas por cada sesión ANTES de combinarlas
            df_session = assign_rooms(df_session)
            df_list.append(df_session)
            
    if not df_list:
        return pd.DataFrame()
        
    # Juntar todas las sesiones en un único DataFrame masivo
    combined_df = pd.concat(df_list, ignore_index=True)
    return combined_df

# Visualización de mapa de calor
def plot_spatial_analysis(df: pd.DataFrame, room: str, img_path: str, output_path: str):
    """Genera una imagen con 1 subplot. """
    
    df_room = df[df["room"] == room].copy()
    if df_room.empty:
        print(f"[AVISO] No hay datos para la sala {room}.")
        return

    df_stress = df_room[df_room["event_name"].isin(["Quick_Jitter_Move", "Quick_HMD_Move"])].copy()

    # Cargar fondo
    try:
        bg_img = Image.open(img_path)
    except Exception as e:
        print(f"[ERROR] No se pudo cargar el mapa {img_path}: {e}")
        # Crear imagen negra por defecto si falta
        bg_img = Image.new('RGB', (800, 800), color=(30, 30, 30))

    # Determinar límites espaciales
    if room in ROOM_BOUNDS:
        extent = ROOM_BOUNDS[room]
    else:
        print(f"[AVISO] Límites de {room} no definidos en ROOM_BOUNDS. Usando bounding box dinámico (puede deformar el mapa).")
        extent = [
            df_room["pos_x"].min() - 5 if not df_room["pos_x"].isna().all() else 0,
            df_room["pos_x"].max() + 5 if not df_room["pos_x"].isna().all() else 100,
            df_room["pos_z"].min() - 5 if not df_room["pos_z"].isna().all() else 0,
            df_room["pos_z"].max() + 5 if not df_room["pos_z"].isna().all() else 100
        ]

    width_m = extent[1] - extent[0]
    height_m = extent[3] - extent[2]
    # Evitar divisiones por cero
    if width_m == 0: width_m = 1
    
    aspect_ratio = height_m / width_m

    fig_width = 8.0
    fig_height = fig_width * aspect_ratio
    
    # Limitar la altura para evitar que pasillos muy estrechos generen imágenes kilométricas
    if fig_height > 12:
        fig_height = 12.0
        fig_width = fig_height / aspect_ratio

    plt.style.use("dark_background")
    fig, ax = plt.subplots(1, 1, figsize=(fig_width, fig_height), facecolor="#1a1a2e")
    ax.set_aspect('equal', adjustable='box')

    ax.imshow(bg_img, extent=extent, aspect="auto", alpha=0.5)
    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    ax.set_xlabel("Eje X (Mundo)", color="white")
    ax.set_ylabel("Eje Z (Mundo)", color="white")
    ax.tick_params(colors="white")
    for spine in ax.spines.values():
        spine.set_edgecolor("#333355")

    # Tensión espacial (Scatter)
    ax.set_title("M4: Tensión espacial", fontsize=14, color="white", pad=15)
    
    if not df_stress.empty:
        sns.scatterplot(
            x=df_stress["pos_x"], y=df_stress["pos_z"],
            hue=df_stress["event_name"], palette={"Quick_Jitter_Move": "#ffffff", "Quick_HMD_Move": "#d9ff00"},
            ax=ax, s=50, edgecolor="white", alpha=0.9
        )
        leyenda = ax.legend(
            loc="upper right",
            bbox_to_anchor=(1.2, 1),
            fontsize=9,
            facecolor="#1a1a2e", 
            edgecolor="#333355",
            labelcolor="white" 
        )
    else:
        ax.text(0.5, 0.5, "Sin eventos de estrés", ha="center", va="center", color="white", transform=ax.transAxes)

    plt.tight_layout(rect=[0, 0, 1, 0.95])
    plt.savefig(output_path, dpi=150, facecolor=fig.get_facecolor(), bbox_inches="tight")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Genera mapas de calor de tensión espacial agregando todas las sesiones.")
    parser.add_argument("--csv-dir", default="metricas_test", help="Directorio con los archivos telemetry_*.csv")
    parser.add_argument("--map-dir", default="mapas", help="Directorio con las imágenes de fondo")
    parser.add_argument("--out-dir", default="graphics", help="Directorio de salida para los heatmaps")
    args = parser.parse_args()

    df_global = process_all_sessions(args.csv_dir)

    if df_global.empty:
        exit(1)
        
    unique_rooms = [r for r in df_global["room"].unique() if r != "Unknown"]
    if not unique_rooms:
        print("[AVISO] No se encontraron salas válidas en la telemetría global.")
        exit(0)
        
    Path(args.out_dir).mkdir(parents=True, exist_ok=True)
        
    print(f"[INFO] Generando heatmaps agregados para las salas: {', '.join(unique_rooms)}\n")
    
    for room in unique_rooms:
        print(f"Generando mapa de calor para la sala: {room}")
        map_path = Path(args.map_dir) / f"{room}.jpg"
        if not map_path.exists():
            map_path = Path(args.map_dir) / f"{room}.png"
            
        out_path = Path(args.out_dir) / f"spatial_heatmap_{room}.png"
        plot_spatial_analysis(df_global, room, str(map_path), str(out_path))
