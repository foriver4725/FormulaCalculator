#!/usr/bin/env python3
# type: ignore

import argparse
import json
import re
import sys
from pathlib import Path

# Pattern for semantic version (major.minor.patch)
VERSION_PATTERN = re.compile(r"^\d+\.\d+\.\d+$")


def validate_version(version: str) -> None:
    """
    Validate that the version follows the semantic format: X.Y.Z
    """
    if not VERSION_PATTERN.fullmatch(version):
        raise ValueError(
            f"Invalid version format: {version}\n"
            "Expected format: major.minor.patch (example: 2.1.0)"
        )


def main() -> int:
    """
    CLI entry point.
    Reads package.json, overwrites the version field, and writes it back.
    """

    parser = argparse.ArgumentParser(
        description="Overwrite the 'version' field in a Unity package.json file."
    )

    # Version to write
    parser.add_argument("version", help="New version number (example: 2.2.0)")

    # Optional path to package.json
    parser.add_argument(
        "-f",
        "--file",
        default="package.json",
        help="Path to package.json (default: ./package.json)",
    )

    args = parser.parse_args()

    try:
        validate_version(args.version)

        package_path = Path(args.file)

        # Ensure the target file exists
        if not package_path.is_file():
            raise FileNotFoundError(f"package.json not found: {package_path}")

        # Load JSON
        with package_path.open("r", encoding="utf-8") as f:
            data = json.load(f)

        # Store previous version for logging
        old_version = data.get("version")

        # Overwrite version
        data["version"] = args.version

        # Write JSON back with stable formatting
        with package_path.open("w", encoding="utf-8", newline="\n") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
            f.write("\n")

        print(f"Version updated: {old_version} -> {args.version}")
        print(f"Target file: {package_path}")

        return 0

    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
