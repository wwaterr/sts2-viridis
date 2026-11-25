# 卡图备用库

用于存放在卡牌移除后仍可能复用的专属卡图。这里的内容不进入 PCK，也不会被游戏加载。

## 入库规则

1. 只收录原本有专属卡图的卡；使用“图片暂缺”占位图的卡不入库。
2. 同时移动源图与运行时纹理，保持同名：
   - `art_source/cards/<snake_name>.png`
   - `assets/images/cards/<snake_name>.ctex`
3. 在下方表格记录原卡名、英文名、稀有度、类型、卡图内容与可复用方向。
4. 复用卡图时，把 PNG 放回 `art_source/cards/`，ctex 放回 `assets/images/cards/`。卡类名转 snake_case 后与文件名一致时会自动挂载；不一致时在卡类里显式写 `AssetProfile`。
5. 只移除卡牌、不删图。未经确认不要覆盖或删除本目录文件。

## 目录

| 文件 | 原卡 | 稀有度 / 类型 | 卡图内容 | 可复用方向 | 入库日期 |
|---|---|---|---|---|---|
| `millennial_pine.png` / `.ctex` | 千年松 / Millennial Pine | 稀有 / 技能 | 悬崖上的巨大古松，夕阳把天空染成金橙色，风吹动枝叶，背景有远山、岩壁与低矮灌木。横版 500×380。 | 古树、先古、格挡或防御、风景型卡图 | 2026-11-25 |
| `truffle.png` / `.ctex` | 松露 / Truffle | 稀有 / 技能 | 绿发小松绿穿园丁装、戴向日葵帽，蹲在树根旁吃一颗深色松露；土里露出浅色松露和根须，秋日暖光。横版 500×380。 | 采集、蘑菇、下回合资源、角色互动 | 2026-11-25 |

## 原卡面提示词

保留移除时的生成提示词，便于后续按同一风格重绘。

```text
松露 / Truffle
1girl, solo, viridis_(character), green_hair, truffle, treasure, glowing_underground,
delighted_expression, card_art, fantasy, painterly, dark_background

千年松 / Millennial Pine
1girl, solo, viridis_(character), green_hair, millennial_pine, ancient_pine_tree, standing_tall,
long_history, card_art, fantasy, painterly, dark_background
```
