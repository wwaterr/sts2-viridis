# 杀戮尖塔 2 角色动画（Spine）调研

> 记录时间：2026-11-07 ｜ 游戏 v0.111.0 ｜ 引擎 Godot 4.5.1
> 机制/工程结构部分见：[STS2-MOD机制与结构调研.md](STS2-MOD机制与结构调研.md)
> 本文所有结论均来自本机游戏文件与 mod 文件反查，非推测。

---

## A. 技术栈

| 项 | 值 | 证据 |
|---|---|---|
| 动画方案 | Spine 2D（spine-godot / spine-cpp） | 游戏根目录 `libspine_godot.windows.template_release.x86_64.dll` |
| 运行时版本 | **4.2.36** | pck 内 `.skel` 数据旁的字面量 `4.2.36` |
| 引擎 | Godot 4.5.1 | pck 头 `engine=(4,5,1)` |
| 导入器 | `spine.skel` / `spine.atlas` | `.godot/imported/*.import` 中 `importer="spine.skel"` |
| 扩展注册 | `res://addons/spine/spine_godot_extension.gdextension` | 游戏 pck 与 mod pck 的 `godot/extension_list.cfg` |
| 程序封装 | `MegaCrit.Sts2.Core.Bindings.MegaSpine` | `sts2.dll` 元数据（61 个成员有 XML 注释） |

**Spine 编辑器必须 4.2.x**。4.1 / 4.3 导出的 `.skel` 运行时读不了，这是最容易踩的坑。

---

## B. 原版角色资产结构

```
res://animations/characters/<id>/
├── <id>.skel              # 骨骼 + 动画（二进制）
├── <id>.atlas             # 图集描述（文本）
├── <id>.png               # 图集贴图
└── <id>_skel_data.tres    # SpineSkeletonDataResource
                           #   atlas_res + skeleton_file_res + default_mix + animation_mixes
```

实测细节：

- 原版 `necrobinder.atlas` 里 `scale:0.359` → 按约 36% 导出，原始绘制分辨率更高；单张图集 ≤ 2048 边长较稳。
- 原版皮肤名 `default`（也有 `Default` 写法），mod 建议统一小写。
- 部件命名是 `l hand`、`bottom sleeve`、`eye_glow_l` 这类小写（可含空格）。
- 背景/特效也是同一套：`res://animations/backgrounds/<name>/<name>.skel`。

---

## C. 战斗动画最小集

从原版 `ironclad` 的 `SpineSkeletonDataResource` 中读到的**全部**动画名：

| 动画 | 用途 | 循环 |
|---|---|---|
| `idle_loop` | 待机 | 是 |
| `attack` | 普通攻击 | 否 |
| `attack_heavy` | 重击 | 否 |
| `hurt` | 受击 | 否 |
| `die` | 死亡 | 否 |

同文件里的过渡混合（`SpineAnimationMix`）：

```
idle_loop -> attack        0.1
idle_loop -> attack_heavy  0.02
idle_loop -> hurt          0.03
hurt      -> idle_loop     0.1
hurt      -> die           默认
attack    -> attack        默认
```

缺这些 mix 不会崩，但动作切换会硬切/跳帧。可在自己的 `.tres` 里自由增删。

---

## D. 场景与节点

角色战斗场景（mod 侧命名 `<ModId>_character.tscn`），实测原版同级节点结构：

```
Node2D (Root)
├── SpineSprite        skeleton_data_res = <id>_skel_data.tres
│                      preview_skin = "default"
├── Bounds (Control)   点击 / 描边范围
├── IntentPos (Marker2D)   意图图标锚点
├── CenterPos (Marker2D)   血条 / 头顶 UI 锚点
└── TalkPos  (Marker2D)    对话气泡锚点
```

另外五个场景：`_character_select_bg.tscn`、`_energy_counter.tscn`、`_icon.tscn`、`_merchant.tscn`、`_rest_site.tscn`。

商店 / 篝火可以只用静态图或单帧循环，不必做完整骨骼。

---

## E. 程序侧接口（游戏自带，无需自己写绑定）

命名空间 `MegaCrit.Sts2.Core.Bindings.MegaSpine`：

- `MegaSprite` / `MegaSkeleton` / `MegaSkeletonDataResource`
  - `HasAnimation(string)`、`GetAnimationNames()`
- `MegaAnimationState`
  - `SetAnimation(name, loop, track)`、`AddAnimation(name, delay, loop, track)`
  - `GetCurrentAnimationName(track)`、`GetCurrentAnimationDuration(track)`
- `MegaCrit.Sts2.Core.Helpers.SpineNodeExtensions.RunWhenSpineReady(node, sprite, action)` —— 等 Spine 就绪再操作
- RitsuLib 侧：`STS2RitsuLib.Scaffolding.Characters.Visuals`（`CharacterAssetProfile` / `CharacterScenes` / `CharacterImages`）负责把场景与图片路径接到角色上

---

## F. 打包注意

- Godot 工程须装 spine-godot **4.2.36**（Godot 4 版）插件，否则 `.skel/.atlas` 导入不会生成 `SpineSkeletonDataResource`。
- 导出 pck 时 addon 与 `godot/extension_list.cfg` 会一起打进去——mod 的 pck 里确实存在 `res://addons/spine/spine_godot_extension.gdextension`，属正常，不要手工删。
- 清单里 `has_pck` 置 `true`。

---

## G. 低成本替代方案

| 方案 | 适用 | 做法 |
|---|---|---|
| 逐帧 flipbook | 攻击特效、小型角色 | 序列帧 → `*.res` flipbook + `*.gdshader`（ThirtySeven 的 hit 特效即如此） |
| 静态立绘 + Tween | 皮肤 / 换装 | 单张 `_character.png` + `AnimationPlayer` 做呼吸、位移动画 |
| Godot `AnimationPlayer` 拼部件 | 不想买 Spine | 每个部件一个 Sprite2D，用 AnimationPlayer 摆动作（表现力上限低，但零授权成本） |

---

## H. 授权与合规

- Spine 编辑器需正版（Essential / Professional 差异见官网），试用版不能导出可发布资源。
- 严禁拆原版角色的 `.skel/.atlas/.png` 复用或改色分发。

---

## I. 不买 Spine 也能先做的事

1. 定角色设定与技能表 → 推出动作表（是否需要 `cast` / `block` / `victory`）。
2. 拿原版角色当尺寸参照：游戏内截图量身高、头身比、脚底对齐点。
3. 写动作时序表：每个动作的时长、是否循环、位移/缩放、命中帧（哪一帧出特效/伤害数字）。
4. 美术按 Spine 要求分层出图（每个部件独立 PNG，别烘焙阴影）。
5. 用占位方块/纯色贴图把代码与场景跑通，最后再替换真 Spine 资源——避免"美术等程序、程序等美术"。

---

## J. 验收清单（做到这些才算动画接完）

- [ ] `idle_loop` 循环无跳帧，首尾帧衔接平滑
- [ ] `attack` 命中帧与伤害数字/特效时间对齐
- [ ] `hurt` 不打断 `attack` 的收招（用 mix 0.03 过渡）
- [ ] `die` 播完停在最后一帧，不回到 idle
- [ ] 脚底与地面线对齐（`CenterPos`/`Bounds` 位置正确）
- [ ] 选人界面立绘与战斗内角色比例一致
- [ ] 商店 / 篝火场景不穿模、不出画
- [ ] 打包后进游戏无 `SpineSkeletonDataResource` 加载报错（查 `godot.log`）

---

## K. 常见坑

| 现象 | 原因 |
|---|---|
| 角色白块/不显示 | `.skel` 版本不匹配（Spine 4.1/4.3 导出） |
| 报 `SpineSprite` 类不存在 | pck 里缺 `addons/spine` 扩展声明 |
| 动作硬切、抽搐 | `animation_mixes` 缺失或 `default_mix` 为 0 |
| 角色飘在空中 / 陷进地里 | 导出时的原点与 `CenterPos`/`Bounds` 不匹配 |
| 贴图糊 | 导出 scale 过小（原版约 0.36），或图集压缩设置过低 |
| 只显示一部分部件 | 图集超尺寸被截断，或部件未打勾进 atlas |

---

## 附：本目录相关数据

| 文件 | 内容 |
|---|---|
| `raw/vanilla_ironclad_scene.txt` | 原版铁甲战士 Spine 数据资源 + 全部动画名 + mix 表 |
| `raw/vanilla_spine_scan.txt` | 原版 pck 中 Spine 资产路径与导入器配置 |
| `raw/vanilla_character_paths.txt` | 原版角色动画目录命名（`animations/characters/<id>/...`） |
| `raw/spine_version.txt` | 版本字面量 `4.2.36` 的位置与上下文 |