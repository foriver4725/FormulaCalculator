#!/usr/bin/env python3
# type: ignore

import argparse
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.ticker import MaxNLocator
from pathlib import Path

LOG_EPS = 1 + 1e-12


def parse_ns_series(series: pd.Series) -> pd.Series:
    """Convert BenchmarkDotNet time strings like '1,227.47 ns' to float."""
    return (
        series.astype(str)
        .str.replace(",", "", regex=False)
        .str.replace(" ns", "", regex=False)
        .astype(float)
    )


def parse_allocated_series(series: pd.Series) -> pd.Series:
    """
    Convert BenchmarkDotNet allocation strings to float bytes.

    Supported examples:
    - '0 B'
    - '32 B'
    - '1 KB'
    - '1.5 KB'
    - '2 MB'
    """
    s = series.astype(str).str.strip().str.replace(",", "", regex=False)

    values = []
    for raw in s:
        parts = raw.split()
        if len(parts) != 2:
            raise ValueError(f"Unsupported Allocated format: {raw}")

        number_str, unit = parts
        number = float(number_str)

        if unit == "B":
            multiplier = 1.0
        elif unit == "KB":
            multiplier = 1024.0
        elif unit == "MB":
            multiplier = 1024.0 * 1024.0
        elif unit == "GB":
            multiplier = 1024.0 * 1024.0 * 1024.0
        else:
            raise ValueError(f"Unsupported Allocated unit: {unit}")

        values.append(number * multiplier)

    return pd.Series(values, index=series.index, dtype=float)


def build_method_styles(df: pd.DataFrame):
    """Extract Method values in appearance order and assign styles."""
    method_order = list(dict.fromkeys(df["Method"].dropna().tolist()))

    # Assign colors sequentially from matplotlib colormap.
    cmap = plt.get_cmap("tab10")
    colors = {method: cmap(i % 10) for i, method in enumerate(method_order)}

    # Replace underscores with spaces for legend labels.
    labels = {method: method.replace("_", " ") for method in method_order}

    return method_order, colors, labels


def plot_metric(
    df: pd.DataFrame,
    metric_column: str,
    y_label: str,
    output_dir: Path,
    output_name: str,
    use_log_y: bool,
):
    method_order, colors, labels = build_method_styles(df)

    fig, ax = plt.subplots(figsize=(7.2, 4.6), dpi=300)

    # ---------- Draw ----------
    for method in method_order:
        g = df[df["Method"] == method].sort_values("Char Count")

        x = g["Char Count"].to_numpy()
        y = g[metric_column].to_numpy()

        # Filter out invalid values for plotting.
        valid_mask = np.isfinite(x) & np.isfinite(y)
        x = x[valid_mask]
        y = y[valid_mask]

        if len(x) == 0:
            continue

        # For log scale, ensure the fitted line is also plottable by replacing non-positive values.
        if use_log_y:
            y = np.where(y <= 0, LOG_EPS, y)

        # Measured points.
        ax.scatter(
            x,
            y,
            s=28,
            color=colors[method],
            label=labels[method],
            zorder=3,
        )

        # Linear regression is only possible with at least 2 points.
        if len(x) >= 2:
            coef = np.polyfit(x, y, 1)
            poly = np.poly1d(coef)

            x_fit = np.linspace(x.min(), x.max(), 200)
            y_fit = poly(x_fit)

            # For log scale, ensure the fitted line is also plottable by replacing non-positive values.
            if use_log_y:
                y_fit = np.where(y_fit <= 0, LOG_EPS, y_fit)

            if len(x_fit) > 0:
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
    ax.set_ylabel(y_label, fontsize=11)

    if use_log_y:
        ax.set_yscale("log")

    # Reduce the number of ticks.
    ax.xaxis.set_major_locator(MaxNLocator(nbins=4, integer=True))
    if not use_log_y:
        ax.yaxis.set_major_locator(MaxNLocator(nbins=5))

    # Hide grid.
    ax.grid(False)

    # Hide top and right spines.
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)

    # Make tick labels a bit modest.
    ax.tick_params(axis="both", which="major", labelsize=10, length=4)

    # Legend.
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
    plt.close(fig)

    print(f"Saved: {png}")
    print(f"Saved: {pdf}")


def plot(
    csv_path: Path,
    mean_output_dir: Path,
    mean_output_name: str,
    mean_log_y: bool,
    allocated_output_dir: Path | None,
    allocated_output_name: str | None,
    allocated_log_y: bool,
):
    # ---------- Load ----------
    df = pd.read_csv(csv_path)

    # Parse Mean.
    if "Mean" not in df.columns:
        raise ValueError("The CSV does not contain a 'Mean' column.")
    df["Mean"] = parse_ns_series(df["Mean"])

    # Plot Mean.
    plot_metric(
        df=df,
        metric_column="Mean",
        y_label="Time [ns]",
        output_dir=mean_output_dir,
        output_name=mean_output_name,
        use_log_y=mean_log_y,
    )

    # Parse and plot Allocated if present.
    if "Allocated" in df.columns:
        df["AllocatedBytes"] = parse_allocated_series(df["Allocated"])

        actual_allocated_output_dir = (
            allocated_output_dir
            if allocated_output_dir is not None
            else mean_output_dir
        )
        actual_allocated_output_name = (
            allocated_output_name
            if allocated_output_name is not None
            else f"{mean_output_name}_allocated"
        )

        plot_metric(
            df=df,
            metric_column="AllocatedBytes",
            y_label="Allocated [B]",
            output_dir=actual_allocated_output_dir,
            output_name=actual_allocated_output_name,
            use_log_y=allocated_log_y,
        )
    else:
        print("Skipped Allocated plot: 'Allocated' column was not found.")


def main():
    parser = argparse.ArgumentParser(
        description="Plot BenchmarkDotNet results"
    )

    parser.add_argument("csv", type=Path, help="BenchmarkDotNet CSV file")

    # Mean plot options.
    parser.add_argument(
        "--mean-output-dir",
        type=Path,
        default=Path("figures"),
        help="Output directory for Mean plot",
    )
    parser.add_argument(
        "--mean-output-name",
        type=str,
        default="plot",
        help="Output file name for Mean plot (without extension)",
    )
    parser.add_argument(
        "--mean-log-y",
        action="store_true",
        help="Use log scale for Mean plot Y axis",
    )

    # Allocated plot options.
    parser.add_argument(
        "--allocated-output-dir",
        type=Path,
        default=None,
        help="Output directory for Allocated plot (default: same as Mean output dir)",
    )
    parser.add_argument(
        "--allocated-output-name",
        type=str,
        default=None,
        help="Output file name for Allocated plot (without extension)",
    )
    parser.add_argument(
        "--allocated-log-y",
        action="store_true",
        help="Use log scale for Allocated plot Y axis",
    )

    args = parser.parse_args()

    plot(
        csv_path=args.csv,
        mean_output_dir=args.mean_output_dir,
        mean_output_name=args.mean_output_name,
        mean_log_y=args.mean_log_y,
        allocated_output_dir=args.allocated_output_dir,
        allocated_output_name=args.allocated_output_name,
        allocated_log_y=args.allocated_log_y,
    )


if __name__ == "__main__":
    main()
