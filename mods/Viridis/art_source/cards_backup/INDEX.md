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

| `fern_leaf.png` / `.ctex` | 蕨叶 / Fern Leaf | 普通 / 攻击 | 小松绿蹲在溪边，手持一片蕨叶探入紫黑色水面，旁边有小鸟与石块。横版 500×380。 | 蕨类、水边、采集、轻盈攻击 | 2026-11-25 |
| `nettle.png` / `.ctex` | 荨麻 / Nettle | 普通 / 攻击 | 小松绿在荨麻丛中向前冲，背后有巨兽剪影，叶片带刺，小鸟在下方。横版 500×380。 | 野外、危险植物、双段攻击 | 2026-11-25 |
| `tree_ring.png` / `.ctex` | 年轮 / Tree Ring | 普通 / 攻击 | 小松绿用铲子削切树桩截面，暖橙色年轮清晰，周围是森林与落叶。横版 500×380。 | 年轮、伐木、时间、古树 | 2026-11-25 |
| `aphid.png` / `.ctex` | 蚜虫 / Aphid | 普通 / 攻击 | 一只绿色蚜虫停在叶茎上吸汁，阳光穿过叶片，背景是深绿植物。横版 500×380。 | 虫害、自然、削弱型攻击 | 2026-11-25 |
| `resin_trap.png` / `.ctex` | 树脂陷阱 / Resin Trap | 普通 / 攻击 | 小松绿在森林里用金色树脂绳困住一头野猪，背景是暮色山峦。横版 500×380。 | 陷阱、狩猎、控制 | 2026-11-25 |
| `earthworm.png` / `.ctex` | 蚯蚓松土 / Earthworm | 普通 / 技能 | 土壤剖面里一条粉色蚯蚓穿过松软泥土，右侧幼苗根须伸展，上方有白花。横版 500×380。 | 地底、土壤、生长、抽牌 | 2026-11-25 |
| `moss_cover.png` / `.ctex` | 苔藓覆盖 / Moss Cover | 普通 / 技能 | 林间岩石与厚苔藓，阳光斜照，前景有蕨芽与落叶，静谧绿色。横版 500×380。 | 苔藓、森林地面、格挡、隐蔽 | 2026-11-25 |
| `melt_to_water.png` / `.ctex` | 忙碌 / Busy（原杀植物 / Melt to Water） | 罕见 / 技能 | 小松绿蹲在树根旁，用金色树脂/黏液把一头野猪缠在树桩之间，背景是暮色山林与蘑菇。横版 500×380。 | 树脂、陷阱、控制、野猪、暮色森林 | 2026-11-26 |

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

蕨叶 / Fern Leaf
1girl, solo, viridis_(character), green_hair, fern, fern_leaf, quick_slash, agile_pose,
motion_lines, card_art, fantasy, painterly, dark_background

荨麻 / Nettle
1girl, solo, viridis_(character), green_hair, stinging_nettle, two_hits, dual_wield, sharp_leaves,
angry_expression, card_art, fantasy, painterly, dark_background

年轮 / Tree Ring
1girl, solo, viridis_(character), green_hair, tree_rings, cross-section, touching_tree_trunk,
concentric_circles, ancient_tree, card_art, fantasy, painterly, dark_background

蚜虫 / Aphid
1girl, solo, viridis_(character), green_hair, pointing, swarm_of_insects, aphids, sickly_green,
disgusted_expression, card_art, fantasy, painterly, dark_background

树脂陷阱 / Resin Trap
1girl, solo, viridis_(character), green_hair, trap, resin_pool, sticky_amber, ensnared, smirking,
card_art, fantasy, painterly, dark_background

蚯蚓松土 / Earthworm
1girl, solo, viridis_(character), green_hair, earthworm, loose_soil, tilling, garden, dirt_on_hands,
card_art, fantasy, painterly, dark_background

苔藓覆盖 / Moss Cover
1girl, solo, viridis_(character), green_hair, moss, mossy_ground, lying_down, camouflage,
soft_green_blanket, card_art, fantasy, painterly, dark_background

忙碌 / Busy（原杀植物 / Melt to Water）
1girl, solo, viridis_(character), green_hair, gardener_outfit, golden_resin, trapping_a_boar, tree_stump, twilight_forest, mushroom, card_art, fantasy, painterly


## 2026-11-27 批量移除（14 张普通/罕见/稀有牌）

| 文件 | 原卡名 | 内容 | 可复用方向 |
|---|---|---|---|
| photosynthetic_pulse | 光合脉冲 | 角色与植物光合脉冲 | 能量/生命回复 |
| pinecone_burst | 松塔爆裂 | 松果爆裂 | 爆发/随机伤害 |
| vernalization | 春化 | 低温催花 | 生长/延缓 |
| germinate | 催芽 | 种子发芽 | 栽培/生长 |
| sap | 树液 | 树液采集 | 能量/粘稠 |
| compost | 腐熟 | 堆肥腐熟 | 成长/收获 |
| fern_spread | 蕨类蔓延 | 蕨叶铺展 | 多段/蔓延 |
| cork_layer | 木栓层 | 树皮木栓层 | 格挡/护甲 |
| peat | 泥炭 | 泥炭沼泽 | 格挡/拖延 |
| borax | 硼砂 | 硼砂结晶 | 生长/强化 |
| seedling_transplant | 幼苗移栽 | 移栽幼苗 | 栽培/移栽 |
| resin_seal | 黑屏版聊 | 树脂封存/聊天画面 | 控制/封存 |
| tree_ring_mark | 年轮刻印 | 年轮刻印 | 格挡/成长 |
| tundra | 苔原 | 苔原雪景 | 格挡/寒冷 |

上述卡牌已从卡池与本地化移除，PNG 与 ctex 保留在本目录，复用时按原名放回 `art_source/cards/` 与 `assets/images/cards/`。
