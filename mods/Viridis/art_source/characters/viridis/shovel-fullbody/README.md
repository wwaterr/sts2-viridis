# 持铲战斗动画制作

游戏使用 `guard-thrust/` 中的定稿。日常导出只需在仓库根目录运行 `python tools/export_viridis_combat.py`。要修改动作本身，再从下面对应的步骤重建。

四张手绘关键帧和配准数据在 `keyframe-study/`，遮挡处的躯干补画在 `keyed-thrust/source/`。起势使用相邻目录 `shovel-strike/frames/guard.png` 和 `knee-on-shin/` 中的关节、分件数据。

### 从关键帧重建

以下命令在仓库根目录的 Windows PowerShell 中运行。制作工具使用 Python 的 Pillow、NumPy、SciPy、Numba，以及 PATH 上的 FFmpeg。预览图使用 Windows 微软雅黑字体。重建会写入骨骼、图集与分件，建议在独立工作树中运行。

```powershell
New-Item -ItemType Directory -Force outputs/viridis-animation | Out-Null
python tools/build_keyed_thrust.py
python tools/retime_keyed_thrust.py
python tools/finish_guard_thrust.py
python tools/retime_guard_thrust.py
python tools/build_guard_breath.py
python tools/build_held_thrust.py
python tools/build_held_thrust.py --longer-extension
python tools/export_viridis_combat.py
```

`finish_guard_thrust.py` 会先生成起势骨骼，再绘制过渡帧。`--longer-extension` 读取上一步的停铲姿势并输出定稿。动作比较视频和中间压缩包位于 `outputs/viridis-animation/`。最终的骨骼、图集描述与贴图位于 `assets/characters/viridis/combat/`（相对 `mods/Viridis/`）。
