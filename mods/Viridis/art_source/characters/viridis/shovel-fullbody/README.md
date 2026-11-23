# 小松绿全身挥铲

用 Spine 4.2 的 Import Data 打开 `viridis-fullbody.json`，图片路径为 `parts/`。预览 `attack`。同目录的 atlas、图集 PNG 与 JSON 可交给运行时加载。

16 张附件在整个动作中各占一个槽位。空手侧按上臂、前臂、手套拆开，两腿各分为带裤管的大腿、小腿、带绑腿的靴子。领口、腋下、裤腰、膝踝关节的覆盖区已补画。持铲侧四张附件沿用第一版裁片。

动作时长 0.86 秒。前 0.10 秒后坐蓄力，接着后脚蹬地，髋向右送，双肩错开，空手向后反摆。第 0.20 秒出铲到命中位，重心继续落到前腿上。之后后跟着地、回收腰肩和双手，回到起势。

双腿使用双段逆向求解后烘焙为旋转关键帧。`rear_contact` 和 `lead_contact` 标出两处支点。转身采用平面侧转表现，靠髋肩错时、肩点前后移动、胸口投影宽度与头部反向转动完成。

重建分件和骨骼使用 `node tools/build_fullbody_shovel.cjs`。从相同 JSON 生成原速、慢放、分件和关键姿势预览使用 `node tools/preview_fullbody_shovel.cjs`。需要 Node、@napi-rs/canvas 和 FFmpeg。
