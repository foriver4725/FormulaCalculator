#!/usr/bin/env python3
# type: ignore

import argparse
import re
import sys
import xml.etree.ElementTree as ET
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


def indent_xml(elem: ET.Element, level: int = 0) -> None:
    """
    Apply simple indentation to the XML tree for readable output.
    """
    indent = "\n" + "    " * level

    if len(elem) > 0:
        if not elem.text or not elem.text.strip():
            elem.text = indent + "    "

        for child in elem:
            indent_xml(child, level + 1)

        if not elem[-1].tail or not elem[-1].tail.strip():
            elem[-1].tail = indent
    else:
        if level > 0 and (not elem.tail or not elem.tail.strip()):
            elem.tail = indent


def main() -> int:
    """
    CLI entry point.
    Reads a .csproj file, overwrites the Version element, and writes it back.
    """
    parser = argparse.ArgumentParser(
        description="Overwrite the <Version> element in a .csproj file."
    )

    # Version to write
    parser.add_argument("version", help="New version number (example: 2.2.0)")

    # Optional path to .csproj
    parser.add_argument(
        "-f",
        "--file",
        default="FormulaCalculator.csproj",
        help="Path to .csproj file (default: ./FormulaCalculator.csproj)",
    )

    args = parser.parse_args()

    try:
        validate_version(args.version)

        csproj_path = Path(args.file)

        # Ensure the target file exists
        if not csproj_path.is_file():
            raise FileNotFoundError(f".csproj file not found: {csproj_path}")

        # Load XML
        tree = ET.parse(csproj_path)
        root = tree.getroot()

        # Find the first Version element under any PropertyGroup
        version_elem = root.find("./PropertyGroup/Version")
        if version_elem is None:
            raise ValueError("No <Version> element found in the .csproj file.")

        # Store previous version for logging
        old_version = version_elem.text

        # Overwrite version
        version_elem.text = args.version

        # Re-indent XML for stable formatting
        indent_xml(root)

        # Write XML back with declaration and LF newlines
        tree.write(csproj_path, encoding="utf-8", xml_declaration=False)

        print(f"Version updated: {old_version} -> {args.version}")
        print(f"Target file: {csproj_path}")

        return 0

    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
