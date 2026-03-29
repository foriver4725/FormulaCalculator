#!/usr/bin/env python3
# type: ignore

import argparse
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.ticker import MaxNLocator
from pathlib import Path


def plot(csv_path: Path, output_dir: Path, output_name: str):

    # ---------- Load ----------
    df = pd.read_csv(csv_path)

    # "1,227.47 ns" -> 1227.47
    df["Mean"] = (
        df["Mean"]
        .str.replace(",", "", regex=False)
        .str.replace(" ns", "", regex=False)
        .astype(float)
    )

    # ---------- Plot settings ----------
    # Extract Method values in appearance order (no sorting)
    method_order = list(dict.fromkeys(df["Method"].dropna().tolist()))

    # Assign colors sequentially from matplotlib colormap
    cmap = plt.get_cmap("tab10")
    colors = {method: cmap(i % 10) for i, method in enumerate(method_order)}

    # Replace underscores with spaces for legend labels
    labels = {method: method.replace("_", " ") for method in method_order}

    fig, ax = plt.subplots(figsize=(7.2, 4.6), dpi=300)

    # ---------- Draw ----------
    for method in method_order:
        g = df[df["Method"] == method].sort_values("Char Count")

        x = g["Char Count"].to_numpy()
        y = g["Mean"].to_numpy()

        # 実測点
        ax.scatter(
            x,
            y,
            s=28,
            color=colors[method],
            label=labels[method],
            zorder=3,
        )

        # 線形回帰
        coef = np.polyfit(x, y, 1)
        poly = np.poly1d(coef)

        x_fit = np.linspace(x.min(), x.max(), 200)
        y_fit = poly(x_fit)

        ax.plot(
            x_fit,
            y_fit,
            linestyle="-",
            linewidth=2.0,
            color=colors[method],
            alpha=0.95,
            zorder=2,
        )

    # ---------- Style ----------
    ax.set_xlabel("Char Count", fontsize=11)
    ax.set_ylabel("Time [ns]", fontsize=11)
    # ax.set_title("charCount & execTime", fontsize=12, pad=10)

    # 目盛りを少なめに
    ax.xaxis.set_major_locator(MaxNLocator(nbins=4, integer=True))
    ax.yaxis.set_major_locator(MaxNLocator(nbins=5))

    # グリッドは消す
    ax.grid(False)

    # 上と右の枠線を消す
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)

    # 目盛り文字を少し控えめに
    ax.tick_params(axis="both", which="major", labelsize=10, length=4)

    # Legend
    ax.legend(
        frameon=False,
        fontsize=10,
        loc="upper left",
        handlelength=2.2,
    )

    plt.tight_layout()

    # ---------- Save ----------
    output_dir.mkdir(parents=True, exist_ok=True)

    png = output_dir / f"{output_name}.png"
    pdf = output_dir / f"{output_name}.pdf"

    plt.savefig(png, bbox_inches="tight")
    plt.savefig(pdf, bbox_inches="tight")

    print(f"Saved: {png}")
    print(f"Saved: {pdf}")

    # plt.show()


def main():

    parser = argparse.ArgumentParser(
        description="Plot BenchmarkDotNet results"
    )

    parser.add_argument("csv", type=Path, help="BenchmarkDotNet CSV file")

    parser.add_argument(
        "-o",
        "--output",
        type=Path,
        default=Path("figures"),
        help="Output directory",
    )

    parser.add_argument(
        "-n",
        "--name",
        type=str,
        default="plot",
        help="Output file name (without extension)",
    )

    args = parser.parse_args()

    plot(args.csv, args.output, args.name)


if __name__ == "__main__":
    main()
