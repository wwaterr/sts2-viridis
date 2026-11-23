# 小松绿战斗动画

首版使用正面中立立绘、9 个网格部件和 13 根骨骼。相邻网格的接缝共用顶点位置与权重。手腕、肩、头和围裙可以分别运动，眼区通过局部网格缩放眨眼。大幅转身和挥铲需要补绘关节遮挡面和持铲手型。`viridis_layer_manifest.json` 记录后续精拆的 32 层设计。

## 当前战斗动画

游戏使用 `shovel-fullbody/guard-thrust/viridis-guard-thrust-long-extension.json` 和同目录图集。3.6 秒的待机循环带双腕摆动。平刺在第 0.333 秒到达最远处，保持 0.222 秒，整招 0.889 秒。此目录的首版骨骼保留了重击、受击和倒地动作的原始曲线。

在仓库根目录执行以下命令，可从上述源文件更新战斗资源并构建 Mod。

```powershell
python tools/export_viridis_combat.py
./mods/Viridis/build.ps1
```

`assets/characters/viridis/combat/viridis.tscn` 将骨骼挂到战斗角色的 Visuals 节点上。血条与意图图标沿用铁甲战士的定位。角色动画器以 `idle_loop` 开场，绑定原生 `Idle` 和 `Attack` 触发器。普攻播放一次 `attack` 后返回呼吸循环。
