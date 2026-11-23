"""Export the approved guard rig and the clips used by the combat animator."""
import json
from shutil import copyfile
from pathlib import Path

from png_to_ctex import png_to_ctex

MOD = Path(__file__).resolve().parents[1] / "mods/Viridis"
ART = MOD / "art_source/characters/viridis"
SOURCE = ART / "shovel-fullbody/guard-thrust"
TARGET = MOD / "assets/characters/viridis/combat"


def export():
    name = "viridis-guard-thrust"
    copyfile(SOURCE / f"{name}-long-extension.json", TARGET / "viridis.spjson")
    page, regions = (SOURCE / f"{name}.atlas").read_text().split("\n", 1)
    atlas = {
        "source_path": "res://Viridis/characters/viridis/combat/viridis.atlas",
        "atlas_data": "viridis.png\n" + regions,
        "normal_texture_prefix": "n", "specular_texture_prefix": "s",
    }
    (TARGET / "viridis.spatlas").write_text(json.dumps(atlas, separators=(",", ":")) + "\n")
    png_to_ctex(SOURCE / page, TARGET / "viridis.ctex")


if __name__ == "__main__":
    export()
