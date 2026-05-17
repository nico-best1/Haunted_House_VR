"""
stress_metrics.py
=================
Parte 3 – Procesamiento y Cálculo de Métricas de Estrés
Responsable: David

Calcula las métricas de estrés M1 y M2 a partir de los eventos guardados
por el TrackerManager en un fichero CSV, y genera comparativas por sala.

Estructura esperada del CSV (separador coma):
    event_name , timestamp_ms , [pos_x , pos_y , pos_z]

Eventos relevantes:
    Enter_<sala>        → cambio de sala activa
    Quick_Jitter_Move   → movimiento brusco de mando  (contribuye a M1)
    Quick_HMD_Move      → movimiento brusco de cabeza (contribuye a M2)

Métricas:
    M1 – Jitter medio de los mandos:
         Tasa de eventos Quick_Jitter_Move por segundo en cada sala.

    M2 – Frecuencia de Escaneo media del HMD:
         Tasa de eventos Quick_HMD_Move por segundo en cada sala.

Uso:
    python stress_metrics.py --csv sesion.csv [--safe-room SafeRoom] [--output informe.png]

Dependencias:
    pip install pandas matplotlib seaborn numpy
"""

import argparse
import sys
from pathlib import Path
from collections import defaultdict

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec
import seaborn as sns

# ─────────────────────────────────────────────────────────────────────────────
# Constantes
# ─────────────────────────────────────────────────────────────────────────────

JITTER_EVENT  = "Quick_Jitter_Move"
HMD_EVENT     = "Quick_HMD_Move"
ENTER_PREFIX  = "Enter_"

DEFAULT_SAFE_ROOM   = "Room0"   # nombre de la sala de referencia 
DEFAULT_CSV         = "sesion.csv"
DEFAULT_OUTPUT      = "stress_report.png"
OUTPUT_DIR = "graphics"

# ─────────────────────────────────────────────────────────────────────────────
# Carga y limpieza del CSV
# ─────────────────────────────────────────────────────────────────────────────

def load_csv(path: str) -> pd.DataFrame:
    """Carga el CSV generado por el TrackerManager."""
    df = pd.read_csv(
        path,
        header=None,
        names=["event_name", "timestamp_ms", "pos_x", "pos_y", "pos_z"],
        # acepta ficheros con o sin columnas de posición
        engine="python",
        on_bad_lines="warn",
    )
    df["timestamp_ms"] = pd.to_numeric(df["timestamp_ms"], errors="coerce")
    df.dropna(subset=["event_name", "timestamp_ms"], inplace=True)
    df["event_name"] = df["event_name"].str.strip()
    df.sort_values("timestamp_ms", inplace=True)
    df.reset_index(drop=True, inplace=True)
    return df


# ─────────────────────────────────────────────────────────────────────────────
# Asignación de sala a cada evento
# ─────────────────────────────────────────────────────────────────────────────

def assign_rooms(df: pd.DataFrame) -> pd.DataFrame:
    """
    Añade la columna 'room' a cada fila según el último evento Enter_<sala>
    previo. Los eventos anteriores al primer Enter_ quedan como 'Unknown'.
    """
    rooms = []
    current_room = "Unknown"
    for _, row in df.iterrows():
        name = row["event_name"]
        if name.startswith(ENTER_PREFIX):
            current_room = name[len(ENTER_PREFIX):]
        rooms.append(current_room)
    df = df.copy()
    df["room"] = rooms
    return df


# ─────────────────────────────────────────────────────────────────────────────
# Cálculo de duración por sala (en segundos)
# ─────────────────────────────────────────────────────────────────────────────

def room_durations(df: pd.DataFrame) -> dict:
    """
    Calcula cuántos segundos se pasó en cada sala tomando como referencia
    los timestamps de los eventos Enter_ consecutivos.
    El último tramo llega hasta el último evento de la sesión.
    """
    enter_events = df[df["event_name"].str.startswith(ENTER_PREFIX)].copy()
    durations = defaultdict(float)

    last_ts  = df["timestamp_ms"].iloc[-1]
    enter_ts = list(enter_events["timestamp_ms"])
    enter_rooms = [n[len(ENTER_PREFIX):] for n in enter_events["event_name"]]

    for i, (ts, room) in enumerate(zip(enter_ts, enter_rooms)):
        end_ts = enter_ts[i + 1] if i + 1 < len(enter_ts) else last_ts
        durations[room] += (end_ts - ts) / 1000.0   # ms → s

    return dict(durations)


# ─────────────────────────────────────────────────────────────────────────────
# Cálculo de métricas M1 y M2
# ─────────────────────────────────────────────────────────────────────────────

def compute_metrics(df: pd.DataFrame, durations: dict) -> pd.DataFrame:
    """
    Devuelve un DataFrame con las métricas M1 y M2 por sala.

    M1 = Quick_Jitter_Move por segundo  (jitter medio de mandos)
    M2 = Quick_HMD_Move por segundo     (frecuencia de escaneo media del HMD)
    """
    # Conteo de eventos por sala
    jitter_counts = (
        df[df["event_name"] == JITTER_EVENT]
        .groupby("room")
        .size()
        .rename("jitter_count")
    )
    hmd_counts = (
        df[df["event_name"] == HMD_EVENT]
        .groupby("room")
        .size()
        .rename("hmd_count")
    )

    all_rooms = sorted(set(df["room"]) - {"Unknown"})
    metrics = pd.DataFrame(index=all_rooms)
    metrics.index.name = "room"

    metrics["duration_s"]    = pd.Series(durations)
    metrics["jitter_count"]  = jitter_counts
    metrics["hmd_count"]     = hmd_counts
    metrics.fillna(0, inplace=True)

    # Tasas por segundo (evitar división por 0)
    metrics["M1_jitter_rate"]  = metrics["jitter_count"]  / metrics["duration_s"].replace(0, np.nan)
    metrics["M2_hmd_scan_rate"] = metrics["hmd_count"] / metrics["duration_s"].replace(0, np.nan)
    metrics.fillna(0, inplace=True)

    return metrics


# ─────────────────────────────────────────────────────────────────────────────
# Comparativa con la sala segura
# ─────────────────────────────────────────────────────────────────────────────

def stress_comparison(metrics: pd.DataFrame, safe_room: str) -> pd.DataFrame:
    """
    Calcula el índice de estrés relativo respecto a la sala segura.
    Un valor > 1 indica más estrés que la línea base.
    """
    if safe_room not in metrics.index:
        print(f"[AVISO] Sala segura '{safe_room}' no encontrada en los datos.")
        return metrics

    baseline_m1 = metrics.loc[safe_room, "M1_jitter_rate"]  or 1e-9
    baseline_m2 = metrics.loc[safe_room, "M2_hmd_scan_rate"] or 1e-9

    metrics = metrics.copy()
    metrics["stress_M1"] = metrics["M1_jitter_rate"]   / baseline_m1
    metrics["stress_M2"] = metrics["M2_hmd_scan_rate"] / baseline_m2
    metrics["stress_index"] = (metrics["stress_M1"] + metrics["stress_M2"]) / 2.0
    return metrics


# ─────────────────────────────────────────────────────────────────────────────
# Visualización
# ─────────────────────────────────────────────────────────────────────────────

PALETTE_BASE  = "#1a1a2e"
PALETTE_SAFE  = "#4ade80"   # verde – sala segura
PALETTE_STRESS = "#f87171"  # rojo  – zonas de estrés
PALETTE_MID   = "#60a5fa"   # azul  – valores intermedios

def _room_color(room: str, safe_room: str, stress_index: float) -> str:
    if room == safe_room:
        return PALETTE_SAFE
    if stress_index > 1.5:
        return PALETTE_STRESS
    return PALETTE_MID


def plot_metrics(metrics: pd.DataFrame, safe_room: str, output_path: str):
    """Genera el informe visual con 4 subplots."""

    has_stress = "stress_index" in metrics.columns
    rooms = metrics.index.tolist()

    colors_m1 = [
        _room_color(r, safe_room, metrics.loc[r, "stress_M1"] if has_stress else 1.0)
        for r in rooms
    ]
    colors_m2 = [
        _room_color(r, safe_room, metrics.loc[r, "stress_M2"] if has_stress else 1.0)
        for r in rooms
    ]
    colors_si = [
        _room_color(r, safe_room, metrics.loc[r, "stress_index"] if has_stress else 1.0)
        for r in rooms
    ]

    plt.style.use("dark_background")
    fig = plt.figure(figsize=(16, 10), facecolor=PALETTE_BASE)
    gs  = gridspec.GridSpec(2, 3, figure=fig, hspace=0.45, wspace=0.35)

    ax1 = fig.add_subplot(gs[0, :2])   # M1 – barras
    ax2 = fig.add_subplot(gs[1, :2])   # M2 – barras
    ax3 = fig.add_subplot(gs[:, 2])    # Índice de estrés – barras horizontales

    for ax in (ax1, ax2, ax3):
        ax.set_facecolor("#0f0f1e")
        ax.tick_params(colors="white")
        ax.xaxis.label.set_color("white")
        ax.yaxis.label.set_color("white")
        ax.title.set_color("white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#333355")

    # ── M1: Jitter medio de mandos ──────────────────────────────────────────
    bars1 = ax1.bar(rooms, metrics["M1_jitter_rate"], color=colors_m1, edgecolor="none", zorder=3)
    ax1.set_title("M₁ – Jitter medio de mandos  (eventos / s)", fontsize=12, pad=10)
    ax1.set_ylabel("Quick_Jitter_Move / s")
    ax1.set_xlabel("Sala")
    ax1.axhline(metrics.loc[safe_room, "M1_jitter_rate"] if safe_room in metrics.index else 0,
                color=PALETTE_SAFE, linestyle="--", linewidth=1.2, alpha=0.7,
                label=f"Baseline ({safe_room})")
    ax1.legend(fontsize=8)
    ax1.grid(axis="y", color="#333355", linewidth=0.5, zorder=0)
    for bar in bars1:
        ax1.text(bar.get_x() + bar.get_width() / 2,
                 bar.get_height() + ax1.get_ylim()[1] * 0.01,
                 f"{bar.get_height():.4f}", ha="center", va="bottom",
                 color="white", fontsize=8)

    # ── M2: Frecuencia de escaneo del HMD ───────────────────────────────────
    bars2 = ax2.bar(rooms, metrics["M2_hmd_scan_rate"], color=colors_m2, edgecolor="none", zorder=3)
    ax2.set_title("M₂ – Frecuencia de escaneo del HMD  (eventos / s)", fontsize=12, pad=10)
    ax2.set_ylabel("Quick_HMD_Move / s")
    ax2.set_xlabel("Sala")
    ax2.axhline(metrics.loc[safe_room, "M2_hmd_scan_rate"] if safe_room in metrics.index else 0,
                color=PALETTE_SAFE, linestyle="--", linewidth=1.2, alpha=0.7,
                label=f"Baseline ({safe_room})")
    ax2.legend(fontsize=8)
    ax2.grid(axis="y", color="#333355", linewidth=0.5, zorder=0)
    for bar in bars2:
        ax2.text(bar.get_x() + bar.get_width() / 2,
                 bar.get_height() + ax2.get_ylim()[1] * 0.01,
                 f"{bar.get_height():.4f}", ha="center", va="bottom",
                 color="white", fontsize=8)

    # ── Índice de estrés combinado ───────────────────────────────────────────
    if has_stress:
        bars3 = ax3.barh(rooms, metrics["stress_index"], color=colors_si, edgecolor="none", zorder=3)
        ax3.axvline(1.0, color=PALETTE_SAFE, linestyle="--", linewidth=1.2,
                    alpha=0.7, label="Baseline = 1.0")
        ax3.set_title("Índice de estrés\n(relativo a sala segura)", fontsize=12, pad=10)
        ax3.set_xlabel("stress_index  (M1+M2) / 2")
        ax3.legend(fontsize=8)
        ax3.grid(axis="x", color="#333355", linewidth=0.5, zorder=0)
        for bar in bars3:
            ax3.text(bar.get_width() + ax3.get_xlim()[1] * 0.01,
                     bar.get_y() + bar.get_height() / 2,
                     f"{bar.get_width():.2f}×", va="center",
                     color="white", fontsize=9)
    else:
        ax3.text(0.5, 0.5, "Sin sala segura\ndefinida",
                 ha="center", va="center", transform=ax3.transAxes,
                 color="white", fontsize=12)

    # Título general
    fig.suptitle("Informe de Métricas de Estrés por Sala  —  Parte 3",
                 fontsize=16, color="white", y=1.01, fontweight="bold")

    # Leyenda de colores
    from matplotlib.patches import Patch
    legend_elements = [
        Patch(facecolor=PALETTE_SAFE,   label="Sala segura (baseline)"),
        Patch(facecolor=PALETTE_MID,    label="Estrés moderado"),
        Patch(facecolor=PALETTE_STRESS, label="Estrés elevado (>1.5×)"),
    ]
    fig.legend(handles=legend_elements, loc="lower center", ncol=3,
               fancybox=True, framealpha=0.2, labelcolor="white", fontsize=9,
               bbox_to_anchor=(0.5, -0.04))

    plt.savefig(output_path, dpi=150, bbox_inches="tight", facecolor=fig.get_facecolor())
    print(f"\n[OK] Gráfico guardado en: {output_path}")


# ─────────────────────────────────────────────────────────────────────────────
# Informe de texto
# ─────────────────────────────────────────────────────────────────────────────

def print_report(metrics: pd.DataFrame, safe_room: str):
    sep = "─" * 72
    print(f"\n{sep}")
    print("  INFORME DE MÉTRICAS DE ESTRÉS  –  PARTE 3")
    print(sep)

    col_fmt = "{:<20} {:>10} {:>10} {:>14} {:>14} {:>13}"
    print(col_fmt.format(
        "SALA", "Dur.(s)", "Jitter(n)",
        "M1 (j/s)", "M2 (h/s)", "stress_idx"
    ))
    print(sep)

    for room, row in metrics.iterrows():
        si = f"{row['stress_index']:.3f}×" if "stress_index" in metrics.columns else "  –"
        print(col_fmt.format(
            room,
            f"{row['duration_s']:.1f}",
            f"{int(row['jitter_count'])}",
            f"{row['M1_jitter_rate']:.5f}",
            f"{row['M2_hmd_scan_rate']:.5f}",
            si,
        ))

    if "stress_index" in metrics.columns and safe_room in metrics.index:
        print(f"\n  Sala segura (baseline): {safe_room}")
        stressed = metrics[
            (metrics.index != safe_room) & (metrics["stress_index"] > 1.5)
        ]
        if not stressed.empty:
            print(f"  Salas con estrés elevado (>1.5×): {', '.join(stressed.index.tolist())}")
        else:
            print("  No se detectaron salas con estrés significativamente elevado.")

    print(sep + "\n")


# ─────────────────────────────────────────────────────────────────────────────
# Datos de ejemplo (para pruebas sin CSV real)
# ─────────────────────────────────────────────────────────────────────────────

def generate_sample_csv(path: str = "sample_session.csv"):
    """Genera un CSV de ejemplo para probar el script sin datos reales."""
    import random, math

    rng = random.Random(42)
    rows = []
    t = 0

    rooms_config = {
        "SafeRoom":    {"jitter_rate": 0.05, "hmd_rate": 0.10, "duration": 30},
        "Corridor":    {"jitter_rate": 0.20, "hmd_rate": 0.35, "duration": 20},
        "BossRoom":    {"jitter_rate": 0.60, "hmd_rate": 0.80, "duration": 45},
        "Puzzle":      {"jitter_rate": 0.15, "hmd_rate": 0.25, "duration": 25},
    }

    for room, cfg in rooms_config.items():
        rows.append([f"Enter_{room}", int(t * 1000), "", "", ""])
        duration = cfg["duration"]
        dt_ms = 50  # 20 Hz simulation
        steps = int(duration * 1000 / dt_ms)

        for _ in range(steps):
            t += dt_ms / 1000.0
            x = rng.uniform(-5, 5)
            y = rng.uniform(1.5, 2.0)
            z = rng.uniform(-5, 5)

            if rng.random() < cfg["jitter_rate"] * (dt_ms / 1000.0):
                rows.append([JITTER_EVENT, int(t * 1000), f"{x:.3f}", f"{y:.3f}", f"{z:.3f}"])

            if rng.random() < cfg["hmd_rate"] * (dt_ms / 1000.0):
                rows.append([HMD_EVENT, int(t * 1000), f"{x:.3f}", f"{y:.3f}", f"{z:.3f}"])

    df = pd.DataFrame(rows, columns=["event_name", "timestamp_ms", "pos_x", "pos_y", "pos_z"])
    df.to_csv(path, index=False, header=False)
    print(f"[INFO] CSV de ejemplo generado: {path}  ({len(df)} filas)")
    return path


# ─────────────────────────────────────────────────────────────────────────────
# Main
# ─────────────────────────────────────────────────────────────────────────────

def main():

    Path(OUTPUT_DIR).mkdir(exist_ok=True)

    parser = argparse.ArgumentParser(
        description="Calcula métricas de estrés M1/M2 por sala desde el CSV del TrackerManager."
    )
    parser.add_argument("--csv", default=DEFAULT_CSV, help="Ruta al CSV de sesión")
    parser.add_argument("--safe-room", default=DEFAULT_SAFE_ROOM, help="Nombre de la sala segura (sin 'Enter_')")
    parser.add_argument("--output", default=DEFAULT_OUTPUT, help="Ruta de la imagen de salida (.png)")
    parser.add_argument("--sample", action="store_true", help="Genera y usa un CSV de ejemplo")
    args = parser.parse_args()

    csv_path = args.csv

    if args.sample or not Path(csv_path).exists():
        if not args.sample:
            print(f"[AVISO] No se encontró '{csv_path}'. Generando datos de ejemplo...")

        csv_path = str(Path(OUTPUT_DIR) / "sample_session.csv")
        generate_sample_csv(csv_path)

    print(f"[INFO] Cargando: {csv_path}")
    df = load_csv(csv_path)

    print(f"[INFO] Eventos totales cargados: {len(df)}")

    df = assign_rooms(df)
    durations = room_durations(df)
    metrics   = compute_metrics(df, durations)
    metrics   = stress_comparison(metrics, args.safe_room)

    print_report(metrics, args.safe_room)

    plot_metrics(metrics, args.safe_room, str(Path(OUTPUT_DIR) / args.output))

    csv_out = str(Path(OUTPUT_DIR) / args.output.replace(".png", "_tabla.csv"))
    metrics.to_csv(csv_out)

    print(f"[OK] Tabla de métricas guardada en: {csv_out}")


if __name__ == "__main__":
    main()
