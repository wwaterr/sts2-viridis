# Viridis 图片资产规格

普通卡图按 **25:19 横图**构图，建议保留 **1000 × 760** 母图，按现有普通牌导出 **500 × 380、不透明 RGB PNG**。每张牌单独交付一张纯插画，卡框、牌名、能量和效果文字由游戏绘制。现有「小松绿」「山绿篱」两张先古牌保留 **832 × 1248（2:3）竖图**。

以下尺寸均为宽 × 高，单位为像素。2026-11-22 核对本地工作树（基线 `3c07a05`），检查了 18 个运行时图片文件、13 个对应的制作源图、11 张美术参考，以及原版抽取样本的图像头。表内尺寸是当前文件实测值。母图尺寸是制作建议；游戏中的显示大小另行注明。

## 生图与裁切

1. 将[小松绿三视图与面部特写](../mods/Viridis/art_source/references/intake-2026-11-22/character_references/viridis_character_model_sheet.png)作为**图片输入**传给生成器。项目路径可以定位文件，使用对话生成时也可以直接引用已上传的同一张附件。生成器无法取得原图时，先补传附件再生成。
2. A 是小松绿的角色原设。她在卡图中的造型和服装以 [B 多视角设定](../mods/Viridis/art_source/references/intake-2026-11-22/character_references/viridis_turnaround.png)及 [C 动作立绘](../mods/Viridis/art_source/references/intake-2026-11-22/character_references/viridis_action_pose.png)为准。**前发正中固定有一根很细的黄色挑染**，随中央发束走向。正面或斜侧面露出这束头发时，保留单根细黄线，用相邻灰绿发色衬出它。缩到 500 × 380 后复核位置、根数和粗细。人物与场景的用笔参考[项目插画](../mods/Viridis/art_source/references/intake-2026-11-22/illustrations)中的 F 巨型松果、G 与叶绿素相遇、H 柜台场景。将相应造型和用笔参考图一并输入生成器。I、J 的梗图画风可适度用于合适的卡牌。K 是小松绿的粉丝「叶绿素」在这套画风下的形象。
3. 普通卡铺满 25:19 画布，等比裁切后缩放。生成器提供其他画幅时，给主体留出裁切空间；出图后检查面部、手、动作落点和原画中有意保留的文字。现有「养术高超」「老农形态」采用纯裁切，「老农形态」的裁切窗口下移以保住画内文字（`7f03126`、`66980c6`）。双联图可作预览，接入时逐张导出。

### 叶绿素和卡牌构图

- 小松绿的脸以 A 的正面特写和 B 的卡图设定为准，默认是安静、自然、偏无口的萌系表情。粉色眼睛、放松的眉线、小巧的鼻口和柔和的下颌在侧脸与低机位也要看得出来。专注、发力或防御可以通过视线与身体动作表现，脸上保留 G 的简练色块。
- 选人背景、顶栏头像和地图标记也沿用这张「冷脸萌」的脸：上眼睑微垂，嘴短而闭合，嘴角放平。背景里的专注感靠视线朝向手边的事物来表现。头像缩小时优先保留帽花、煎蛋徽章和中央细黄挑染。
- 《切片呢》中的“切片”指 B 站粉丝从小松绿的直播录像中选取精彩片段并剪辑成短视频。构图里让粉丝叶绿素操作剪辑时间线，小松绿出现在屏幕里的直播预览中。

- 叶绿素出镜时，把 [K 叶绿素设定原图](../mods/Viridis/art_source/references/intake-2026-11-22/character_references/leaf_creature_model_sheet.png)单独传给生成器。K 正视、侧视与后视图分别给出了米色腹部、突出的短鸟喙、两侧的小翅和身后下垂的叶尾。体形上窄下宽，高约 10 cm，头顶的几片叶羽向后贴伏，脚是细小的棕色鸟爪。表情和动作可参考 G。
- 批量排构图时，以缩略图并排核对主角、动作和机位。已有小松绿参与的卡图，改构图时保留她在事件中的作用。例如《枼绿素出击》让她领着粉丝冲锋，《蕨叶》让她俯身挥叶，两张牌分别用低机位侧拍和俯视。纯植物题材可围绕植物构图。叶绿素出镜时对照 K，按牌意决定它是主角、观众还是同伴。

## v5 卡图接入

审核表中的 96 张插画按卡牌 ID 绑定，运行时文件位于 `assets/images/cards/<卡类名转小写蛇形>.ctex`，对应的 500 × 380 RGB 导出图在 `art_source/cards/`。连同原有 4 张专属插画，共 100 张。原有 4 张沿用各自的显式资源路径。

## 接入前的运行时图片清单

路径相对于 `mods/Viridis/assets/`，对应游戏地址 `res://Viridis/<表内路径>`。RGB 表示所有像素不透明，RGBA 表示实测含透明像素。CTEX 的像素模式以解码后画面为准。“同名 PNG”指下列制作目录中与纹理同名的源文件。

| 用途 | 包内相对路径 | 宽 × 高 | 像素 | 制作源文件 |
|---|---|---:|---|---|
| 卡图占位 | `images/cards/card_placeholder.ctex` | 500 × 380 | RGB | 同名 PNG |
| 养术高超 | `images/cards/cultivation_mastery.ctex` | 500 × 380 | RGB | 同名 PNG |
| 老农形态 | `images/cards/farmer_form_card.ctex` | 500 × 380 | RGB | 同名 PNG |
| 小松绿（先古） | `images/cards/small_pine.ctex` | 832 × 1248 | RGB | 当前仅有 CTEX |
| 山绿篱（先古） | `images/cards/mountain_hedge.ctex` | 832 × 1248 | RGB | 当前仅有 CTEX |
| 考子状态及关键词 | `images/powers/kaozi.ctex` | 256 × 256 | RGBA | 同名 PNG |
| 反踩状态及关键词 | `images/powers/anti_stomp.ctex` | 256 × 256 | RGBA | 同名 PNG |
| 串子状态及关键词 | `images/powers/chuanzi.ctex` | 256 × 256 | RGBA | 同名 PNG |
| 老农形态状态图标 | `images/powers/farmer_form.ctex` | 256 × 256 | RGBA | 同名 PNG |
| 选人肖像 | `characters/viridis/character_select_icon.ctex` | 512 × 768 | RGB | 同名 PNG |
| 锁定肖像 | `characters/viridis/character_select_icon_locked.ctex` | 512 × 768 | RGB | 同名 PNG |
| 选人背景 | `characters/viridis/character_select_bg.ctex` | 2560 × 1200 | RGB | 同名 PNG |
| 顶栏／存档头像 | `characters/viridis/character_icon.ctex` | 256 × 256 | RGB | 同名 PNG |
| 头像描边层 | `characters/viridis/character_icon_outline.ctex` | 256 × 256 | RGBA | 同名 PNG |
| 地图标记 | `characters/viridis/map_marker.ctex` | 98 × 128 | RGBA | 同名 PNG |
| 卡面能量图标 | `images/ui/energy_icon_big.ctex` | 74 × 74 | RGBA | 当前仅有 CTEX |
| 富文本能量图标 | `images/ui/energy_icon_small.ctex` | 24 × 24 | RGBA | 当前仅有 CTEX，代码也指定显示为 24 × 24 |
| 战斗能量球底图 | `images/ui/energy_orb_icon.png` | 128 × 128 | RGBA | 包内即原始 PNG，能量数字由场景叠加 |

卡图的制作目录是 `art_source/cards/`，状态图标是 `art_source/powers/`，角色展示图是 `art_source/characters/viridis/`，均相对于 `mods/Viridis/`。这 13 个 PNG 都是每通道 8 位；逐张转为 RGBA 后，与对应 CTEX 的解码像素一致。角色占位图生成脚本中的尺寸与部分现有成图不同，替换素材请按上表。

## 接入和打包

- **卡图**通过卡类的 `AssetProfile.PortraitPath` 绑定。通用占位由 `PlaceholderArtCard` 提供。角色的六个展示槽位在 `Characters/ViridisCharacter.cs`，前三个状态的关键词图标在 `Hype/HypeKeywords.cs`。卡框沿用绿色材质，由 `Pools/ViridisPools.cs` 调色，卡池颜色为 `#8eb056`。
- **战斗能量球**单独走 PNG 路径。`Patches/EnergyCounterLeafPatch.cs` 用 `GetFileAsBytes → LoadPngFromBuffer → ImageTexture.CreateFromImage` 加载图片，赋给场景节点 `Layers/LayerLeaf`。场景根节点为 128 × 128，轴心在 (64, 64)。替换时要保留透明底，并给中央能量数字留出辨识空间。
- **其余 17 个纹理**使用 GST2 v1 容器，现有文件均内嵌无损 WebP。仓库[转换器](../tools/png_to_ctex.py)把输入转成 RGBA，按输入尺寸编码，写入 56 字节头和 WebP 数据。它不裁切，也不缩放。其命令行输出目录固定为角色展示目录；转换卡图或状态图标时使用下例显式指定目的地。

从仓库根目录执行（PowerShell，Python 需安装 Pillow）。把示例名字换成要接入的资源名。

```powershell
@'
from pathlib import Path
from tools.png_to_ctex import png_to_ctex

mod = Path("mods/Viridis")
name = "cultivation_mastery"
png_to_ctex(mod / "art_source/cards" / (name + ".png"),
            mod / "assets/images/cards" / (name + ".ctex"))
'@ | python -

.\mods\Viridis\build.ps1
python tools/pck_verify.py mods/Viridis/bin/Viridis.pck
```

[构建脚本](../mods/Viridis/build.ps1)会把整个 `assets/` 传给[打包器](../tools/pck_pack.py)，以 `Viridis/` 为包内前缀。母图和参考图放在 `art_source/`。检查打包结果时确认 `md5_ok=True`；进游戏验收时分别查看手牌、放大预览和实际使用该图标的 UI。此处记录了文件与包内路径，卡框的精确可见边界、引擎可接受的最大纹理尺寸和其他画幅的实机效果仍待测量。

## 参考素材与原版样本

本批 11 张参考图的原文件名、用途和逐张尺寸见[素材索引](../mods/Viridis/art_source/references/intake-2026-11-22/README.md)。它们全部解码为 RGB。以下为**真实编码**，归档时保留原文件字节和扩展名。

| 真实编码 | 参考文件（省略共同扩展名 `.png`） |
|---|---|
| PNG（3 张） | `viridis_character_model_sheet`、`shield_block`、`shovel_strike` |
| JPEG（8 张） | `viridis_turnaround`、`viridis_action_pose`、`leaf_creature_model_sheet`、`counter_scene`、`creature_encounter`、`giant_pinecone`、`cultivation_mastery_mockup`、`farmer_form_mockup` |

生成器按媒体类型接收文件时，按实际编码传入（PNG 为 `image/png`，JPEG 为 `image/jpeg`）。若需要 PNG 交付，解码后另存为真正的 PNG。带框参考图只用于版式和构图核对。

[原版尺寸统计](vanilla-image-sizes.csv)按本地 `art/` 下的类别、宽、高和像素模式汇总了全部 2,310 张已抽取图片。910 张原版卡图中，684 张为 1000 × 760，147 张为 668 × 508，40 张为 500 × 380，其余 39 张使用其他尺寸。原版抽取的覆盖范围及缺失项见[抽取索引](../ART_INDEX.md)。

| 尚无独立成图的用途 | 已核对的依据 |
|---|---|
| 遗物 | 两个园艺遗物的 `IconBaseName` 都是 `gardening_tools`；当前模组包没有独立遗物图片。本地抽取的 9 张原版遗物样本全部为 256 × 256 RGBA，可作制作参照。 |
| 药水 | 当前模组包没有独立药水图片。68 张原版样本中有 64 张为 256 × 256 RGBA。 |
| 战斗角色、动画和特效 | 战斗场景沿用铁甲战士，`CustomCombatSpineSkeletonDataPath` 指向小松绿的独立骨骼。[战斗动画源文件与构建说明](../mods/Viridis/art_source/characters/viridis/spine/README.md)记录了拆件、动作和预览路径。原版场景及 VFX 尺寸可按类别查 CSV。 |
