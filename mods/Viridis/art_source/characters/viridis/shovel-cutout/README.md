# 小松绿持铲侧拆件试绑

在 Spine 4.2 中使用 Import Data 打开 `viridis-shovel.json`，图像目录指向 `parts/`，播放 `attack`。同目录的 PNG 大图和 atlas 可配合 JSON 提供给 Spine 运行时。

母图是上级 `shovel-strike/frames/guard.png`。可见的袖口、手臂、手套和铲刃从这张图沿轮廓裁出。身体保留原画，通过内置图像工具补绘持铲侧腋下，在最终身体图中只合入肩窝附近的小块。补绘提示为移除画面左侧持铲手臂及白袖，补出马甲侧边与衬衫袖窿，保持服装、姿势及透明背景。肘、腕延长的肤色、手掌底色和拳下木柄由拆件脚本绘制。

五个附件各自固定在一个槽位。铲子是手掌的子骨骼，握点固定。身体在腰部加权，双脚固定；肩、肘、腕和躯干分别做旋转关键帧。0.20 秒挥到前方，0.80 秒回到持铲待机姿势。对侧手臂与身体共用纹理。

原画处理和动作生成见仓库的 `tools/build_shovel_cutout.cjs`，从导出骨骼读取并渲染的预览见 `tools/preview_shovel_cutout.cjs`。两者使用 Node 和 `@napi-rs/canvas`，预览另需 FFmpeg。
