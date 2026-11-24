from pathlib import Path
import os


def configured_path(name: str) -> Path:
    value = os.environ.get(name)
    if not value:
        raise SystemExit(f"Set {name} to the path you want to inspect.")
    return Path(value).expanduser()


def game_pack() -> Path:
    return configured_path("STS2_GAME_PCK")


def workshop_root() -> Path:
    return configured_path("STS2_WORKSHOP_DIR")


def repo_path(relative: str) -> Path:
    return Path(__file__).resolve().parents[1] / relative
