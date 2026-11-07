# STS2 美术资源抽取索引

- 来源：`D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck`（v0.111.0，1.9 GB）
- 抽取时间：2026-11-07 ｜ 脚本：`extract_sts2_art.py`（根目录，可重跑）
- 结果：**2,310 个 PNG / 415 MB**，按资源原始路径分类

## 目录

| 目录 | 文件 | 大小 | 内容 | 覆盖 |
|---|---:|---:|---|---|
| `cards/` | 910 | 263 MB | **卡图**，1000×760 RGB，按角色分子目录 | 910/910 ✅ |
| `spine_sheets/` | 173 | 67 MB | Spine 图集大图（怪物 120、角色 13、商人 10…） | 173/531 ⚠️ |
| `epoch/` | 57 | 18.5 MB | **编年史插画** 810×500（角色+场景整张） | 57/57 ✅ |
| `ancients/` | 6 | 18 MB | 先古之民大图 | 6/7 |
| `vfx/` | 296 | 8.9 MB | 特效贴图 | 296/372 |
| `packed_misc/` | 519 | 8.5 MB | 打包杂项（意图图标、通用 UI 等） | 520/577 |
| `monsters/` | 16 | 8 MB | 怪物图 | 16/17 |
| `ui/` | 107 | 6.6 MB | 界面元素 | 107/254 |
| `misc/` | 53 | 5.5 MB | 其它 | 53/81 |
| `rooms/` | 62 | 4 MB | 场景背景（overgrowth / hive / glory） | 62/243 |
| `potions/` | 68 | 3 MB | 药水图 | 68/68 ✅ |
| `other/` | 13 | 2 MB | 未归类的根级资源 | — |
| `atlases/` | 3 | 0.7 MB | 图集 | 3/16 |
| `relics/` | 9 | 0.4 MB | 遗物图标 | 9/376 ❌ |
| `powers/` | 14 | 0.3 MB | 能力图标 | 14/272 ❌ |
| `spine_atlas/` | 171 | 0.3 MB | **分件清单**（纯文本 atlas，含每个 region 的 bounds） | 170/170 ✅ |
| `events/` | 4 | <0.1 MB | 事件图 | 4/80 ❌ |

## 缺口：1,659 个 VRAM 压缩纹理没抽出来

游戏把纹理分两套存法：大图用无损 WebP（全部已抽出），小图标/图集用 BC 压缩（`.s3tc` / `.bptc`），这些在文件里是裸 BC 数据块，Python 的 Pillow 解不了。

重灾区：`relics/`（367）、`powers/`（258）、`spine_sheets/`（358，主要是**角色选择界面**那 5 张 3713×2427 大图集）、`rooms/`（181）、`ui/`（147）。

补齐的办法（任选）：

1. **GDRE Tools / gdsdecomp** —— 直接解 pck，自带 ctex→PNG 和 BC 解码。注意这个 pck 的目录表在文件尾部、header 是 version 3，工具不一定认，以实测为准。
2. **Godot 4.5.1 编辑器**导入后重新导出 PNG。
3. **texconv**（DirectXTex）：需要先把裸 BC 块包一层 DDS 头（宽度/高度在 ctex 头部 offset 8/12，格式按 `.s3tc`/`.bptc` 后缀判断）。

## 训 LoRA 用哪几个目录

**完整可用的风格数据集 = `cards/` + `epoch/` + `potions/` = 1,035 张**，这部分的画风、分辨率、笔触完全一致，够训风格 LoRA 了。

不要放进训练集：

- `spine_sheets/` —— 图集里是透明空隙 + `rotate:90` 斜排的头发丝 + 零件并排，模型会学到"拼贴感"而不是画风。当分件参考看可以。
- `ui/`、`packed_misc/`、`vfx/` —— 图标和特效，尺寸小、大量纯色，会把画风拉低。
- `spine_atlas/` —— 是文本，不是图。

## 备注

- 卡图实际尺寸 **1000×760**，mod 卡图对齐这个值才不会在卡池里显小。
- `spine_atlas/_index.tsv` 有每个图集的 sheet 尺寸和 region 数，可以按"分件最少"（静默猎手 2 件）挑模板。
- `refs/`、`tools/` 两个目录 ACL 异常，写入被拒，所以脚本和产物都放在根目录。