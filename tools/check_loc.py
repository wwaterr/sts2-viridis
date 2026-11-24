#!/usr/bin/env python3
"""校验 MOD 清单与本地化数据的一致性，CI 和本地都能跑。

检查项：
  1. Viridis.json 必填字段齐全，id 与目录名一致，version 形如 x.y.z
  2. assets/localization/<lang>/*.json 全部能解析
  3. 各语言的键集合一致（漏翻会报错）
  4. 每个 [RegisterCard] / [RegisterCharacter] / [RegisterPotion] 类都有对应的 title / description
  5. 自定义能力的名称和说明能在 powers.json 中找到

用法：
  python tools/check_loc.py

退出码 0 = 通过，1 = 有错误。
"""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
MOD = ROOT / "mods" / "Viridis"
LANGS = ("zhs", "eng")

MANIFEST_REQUIRED = ("id", "name", "version", "has_pck", "has_dll", "dependencies")
CARD_KEYS = ("title", "description")
CHARACTER_KEYS = ("title", "description")
POTION_KEYS = ("title", "description")

errors: list[str] = []
warnings: list[str] = []


def snake(name: str) -> str:
    """CamelCase -> UPPER_SNAKE（BlankCard -> BLANK_CARD）。"""
    return re.sub(r"(?<!^)(?=[A-Z])", "_", name).upper()


def load_json(path: Path) -> dict:
    try:
        with path.open(encoding="utf-8") as f:
            return json.load(f)
    except FileNotFoundError:
        errors.append(f"缺少文件: {path.relative_to(ROOT)}")
    except json.JSONDecodeError as e:
        errors.append(f"JSON 解析失败: {path.relative_to(ROOT)} ({e.msg}, 第 {e.lineno} 行)")
    return {}


# ---------------------------------------------------------------- 1. 清单
def check_manifest() -> None:
    path = MOD / "Viridis.json"
    meta = load_json(path)
    if not meta:
        return

    for key in MANIFEST_REQUIRED:
        if key not in meta:
            errors.append(f"Viridis.json 缺少字段: {key}")

    if meta.get("id") != MOD.name:
        errors.append(f"Viridis.json 的 id 是 {meta.get('id')!r}，与目录名 {MOD.name!r} 不一致")

    version = str(meta.get("version", ""))
    if not re.fullmatch(r"\d+\.\d+\.\d+", version):
        errors.append(f"version 必须是 x.y.z 形式，当前是 {version!r}")

    if not isinstance(meta.get("dependencies", []), list):
        errors.append("dependencies 必须是数组")


# ---------------------------------------------------------- 2/3. 本地化文件
def load_localization() -> dict[str, dict[str, dict[str, str]]]:
    """返回 {lang: {filename: {key: value}}}。"""
    data: dict[str, dict[str, dict[str, str]]] = {}
    for lang in LANGS:
        lang_dir = MOD / "assets" / "localization" / lang
        if not lang_dir.is_dir():
            errors.append(f"缺少语言目录: {lang_dir.relative_to(ROOT)}")
            continue
        data[lang] = {}
        for f in sorted(lang_dir.glob("*.json")):
            parsed = load_json(f)
            if parsed:
                data[lang][f.name] = parsed
    return data


def check_parity(loc: dict[str, dict[str, dict[str, str]]]) -> None:
    if len(loc) < 2:
        return
    names = sorted({name for files in loc.values() for name in files})
    for name in names:
        per_lang = {lang: files.get(name, {}) for lang, files in loc.items()}
        for lang, keys in per_lang.items():
            if not keys:
                errors.append(f"{lang}/{name} 缺失")
        base_lang = next(iter(per_lang))
        base = set(per_lang[base_lang])
        for lang, keys in per_lang.items():
            if lang == base_lang or not keys:
                continue
            missing = sorted(base - set(keys))
            extra = sorted(set(keys) - base)
            if missing:
                errors.append(f"{lang}/{name} 缺少键: {', '.join(missing[:5])}"
                              + (f" 等 {len(missing)} 个" if len(missing) > 5 else ""))
            if extra:
                warnings.append(f"{lang}/{name} 多出键: {', '.join(extra[:5])}"
                                + (f" 等 {len(extra)} 个" if len(extra) > 5 else ""))
            empty = sorted(k for k, v in keys.items() if not str(v).strip())
            if empty:
                warnings.append(f"{lang}/{name} 有空值: {', '.join(empty[:5])}")


# -------------------------------------------------------- 4. 类与键的对应
def strip_comments(src: str) -> str:
    """去掉注释，避免文档注释里的示例代码被当成真特性。"""
    src = re.sub(r"/\*.*?\*/", "", src, flags=re.S)
    return re.sub(r"(?m)//.*$", "", src)


def registered_classes(pattern: str) -> list[str]:
    """扫描 MOD 下所有 .cs，找出带指定特性的类名。"""
    attr = re.compile(
        r"\[" + pattern + r"(?:\([^\]]*\))?\][^;{]*?class\s+(\w+)",
        re.S,
    )
    found: list[str] = []
    for cs in MOD.rglob("*.cs"):
        if any(part in {"bin", "obj"} for part in cs.parts):
            continue
        src = strip_comments(cs.read_text(encoding="utf-8", errors="ignore"))
        for name in attr.findall(src):
            if name not in found:
                found.append(name)
    return found


def check_keys(loc: dict[str, dict[str, dict[str, str]]]) -> None:
    families = (
        ("RegisterCard", "cards.json", "VIRIDIS_CARD_", CARD_KEYS),
        ("RegisterCharacter", "characters.json", "VIRIDIS_CHARACTER_", CHARACTER_KEYS),
        ("RegisterPotion", "potions.json", "VIRIDIS_POTION_", POTION_KEYS),
    )
    for attribute, filename, prefix, required in families:
        classes = registered_classes(attribute)
        if not classes:
            warnings.append(f"没有找到任何 [{attribute}] 类")
        for name in classes:
            base = prefix + snake(name)
            for lang, files in loc.items():
                table = files.get(filename, {})
                for suffix in required:
                    if f"{base}.{suffix}" not in table:
                        errors.append(
                            f"{name} ({attribute}) 在 {lang}/{filename} 里缺少键 "
                            f"{base}.{suffix}"
                        )


def check_power_keys(loc: dict[str, dict[str, dict[str, str]]]) -> None:
    sources = (
        path
        for folder in ("Cultivate", "Hype", "Powers")
        for path in (MOD / folder).glob("*.cs")
    )
    required: set[str] = set()
    for path in sources:
        source = strip_comments(path.read_text(encoding="utf-8-sig"))
        children = re.findall(r"\bclass\s+(\w+)\s*:\s*CultivatePowerBase\b", source)
        for name in children:
            required.update(f"VIRIDIS_POWER_{snake(name)}.{suffix}" for suffix in ("title", "description"))
        required.update(re.findall(r'new(?:\s+LocString)?\(\s*"powers"\s*,\s*"(VIRIDIS_POWER_[^"]+)"', source))

    for lang, files in loc.items():
        table = files.get("powers.json", {})
        for key in sorted(required):
            if not table.get(key, "").strip():
                errors.append(f"{lang}/powers.json 缺少能力文本: {key}")


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")

    if not MOD.is_dir():
        print(f"找不到 MOD 目录: {MOD}")
        return 1

    check_manifest()
    loc = load_localization()
    check_parity(loc)
    check_keys(loc)
    check_power_keys(loc)

    for w in warnings:
        print(f"warning: {w}")
    for e in errors:
        print(f"error: {e}")

    total = sum(len(t) for files in loc.values() for t in files.values())
    if errors:
        print(f"\n失败：{len(errors)} 个错误，{len(warnings)} 个警告，共 {total} 条本地化文本")
        return 1
    print(f"通过：{total} 条本地化文本，{len(warnings)} 个警告")
    return 0


if __name__ == "__main__":
    sys.exit(main())
