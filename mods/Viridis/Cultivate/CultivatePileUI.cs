using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Viridis.Cultivate;

/// <summary>
/// 栽培牌堆：战斗界面左下角一个和抽牌/弃牌/消耗牌堆并列的按钮，
/// 显示栽培中的卡，并标出剩余回合数。
/// 游戏原生的牌堆容器只认 Draw/Discard/Exhaust 三个按钮，所以这里自建。
/// </summary>
public static class CultivatePileUI
{
    private static Button? _button;
    private static PanelContainer? _panel;
    private static VBoxContainer? _list;

    public static void EnsureAttached(Player? player = null)
    {
        try
        {
            if (_button is not null && GodotObject.IsInstanceValid(_button))
                return;

            var hand = NPlayerHand.Instance;
            if (hand is null)
                return;

            var tree = hand.GetTree();
            if (tree is null)
                return;

            var piles = FindPiles(tree);
            if (piles is null)
            {
                Log.Warn("[Cultivate] CombatPiles container not found (searched by name and type)");
                return;
            }

            var button = new Button
            {
                Text = "栽培 0",
                CustomMinimumSize = new Vector2(96, 44),
                Size = new Vector2(96, 44),
                MouseDefaultCursorShape = Control.CursorShape.PointingHand
            };
            button.AddThemeFontSizeOverride("font_size", 18);

            piles.AddChild(button);

            // 放在弃牌堆上边
            var anchor = (Control?)piles.DiscardPile ?? piles;
            var rect = anchor.GetGlobalRect();
            button.Size = new Vector2(96, 44);
            button.GlobalPosition = new Vector2(rect.Position.X, rect.Position.Y - 58);
            button.Pressed += TogglePanel;

            Log.Info($"[Cultivate] anchor=discard at {rect.Position}, button at {button.GlobalPosition}");

            var panel = new PanelContainer
            {
                Visible = false,
                Size = new Vector2(300, 320)
            };
            piles.AddChild(panel);
            panel.GlobalPosition = new Vector2(rect.Position.X - 80, rect.Position.Y - 400);

            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left", 12);
            margin.AddThemeConstantOverride("margin_top", 10);
            margin.AddThemeConstantOverride("margin_right", 12);
            margin.AddThemeConstantOverride("margin_bottom", 10);
            panel.AddChild(margin);

            // 牌库无上限，列表必须可滚动
            var scroll = new ScrollContainer
            {
                Size = new Vector2(276, 296),
                HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled
            };
            margin.AddChild(scroll);

            var list = new VBoxContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            scroll.AddChild(list);

            _button = button;
            _panel = panel;
            _list = list;

            Log.Info("[Cultivate] cultivate pile button created");
            Refresh(player);
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] create cultivate pile UI failed: {e}");
        }
    }

    public static void Refresh(Player? player)
    {
        try
        {
            EnsureAttached(player);

            if (_button is null || !GodotObject.IsInstanceValid(_button))
                return;

            var power = player?.Creature?.GetPower<CultivateFieldPower>();
            var pending = power?.Pending ?? new List<Planting>();

            _button.Text = $"栽培 {pending.Count}";

            if (_list is null || !GodotObject.IsInstanceValid(_list))
                return;

            foreach (var child in _list.GetChildren())
                child.QueueFree();

            if (pending.Count == 0)
            {
                _list.AddChild(new Label { Text = "（空）" });
                return;
            }

            foreach (var p in pending)
            {
                var label = new Label
                {
                    Text = p.Permanent ? $"{p.Title}  永久" : $"{p.Title}  剩余 {p.TurnsLeft} 回合",
                    AutowrapMode = TextServer.AutowrapMode.WordSmart
                };
                label.AddThemeFontSizeOverride("font_size", 16);
                _list.AddChild(label);
            }
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] refresh cultivate pile failed: {e}");
        }
    }

    private static NCombatPilesContainer? FindPiles(SceneTree tree)
    {
        // 1) 按名字
        if (tree.Root.FindChild("CombatPiles", true, false) is NCombatPilesContainer byName)
            return byName;

        // 2) 按类型遍历整棵树
        foreach (var node in tree.Root.FindChildren("*", "", true, false))
        {
            if (node is NCombatPilesContainer byType)
            {
                Log.Info("[Cultivate] found piles container by type");
                return byType;
            }
        }

        // 3) 兜底：从手牌节点往上找
        var hand = NPlayerHand.Instance;
        Node? cursor = hand;
        while (cursor is not null)
        {
            foreach (var child in cursor.GetChildren())
            {
                if (child is NCombatPilesContainer byWalk)
                    return byWalk;
            }
            cursor = cursor.GetParent();
        }

        return null;
    }

    private static void TogglePanel()
    {
        if (_panel is null || !GodotObject.IsInstanceValid(_panel))
            return;

        _panel.Visible = !_panel.Visible;
    }

    public static void Reset()
    {
        try
        {
            if (_button is not null && GodotObject.IsInstanceValid(_button))
                _button.QueueFree();
            if (_panel is not null && GodotObject.IsInstanceValid(_panel))
                _panel.QueueFree();
        }
        catch
        {
            // 界面可能已销毁
        }
        finally
        {
            _button = null;
            _panel = null;
            _list = null;
        }
    }
}