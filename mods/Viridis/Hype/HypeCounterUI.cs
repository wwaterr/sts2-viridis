using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Viridis.Hype;

/// <summary>能量球右侧的“同接”小面板。</summary>
public static class HypeCounterUI
{
    private static PanelContainer? _panel;
    private static Label? _label;
    private static Player? _player;

    public static void Attach(NCombatUi ui, Player player)
    {
        try
        {
            if (_panel is not null && GodotObject.IsInstanceValid(_panel))
            {
                _player = player;
                Refresh(player);
                return;
            }

            var container = ui.EnergyCounterContainer;
            if (container is null)
                return;

            var panel = new PanelContainer
            {
                Visible = false,
                Position = new Vector2(116, 42),
                CustomMinimumSize = new Vector2(104, 40),
                MouseFilter = Control.MouseFilterEnum.Stop
            };

            var style = new StyleBoxFlat
            {
                BgColor = new Color(0.07f, 0.12f, 0.06f, 0.92f),
                BorderColor = new Color("cdeaa2"),
                BorderWidthLeft = 2,
                BorderWidthTop = 2,
                BorderWidthRight = 2,
                BorderWidthBottom = 2,
                CornerRadiusTopLeft = 7,
                CornerRadiusTopRight = 7,
                CornerRadiusBottomLeft = 7,
                CornerRadiusBottomRight = 7
            };
            panel.AddThemeStyleboxOverride("panel", style);

            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left", 8);
            margin.AddThemeConstantOverride("margin_top", 3);
            margin.AddThemeConstantOverride("margin_right", 8);
            margin.AddThemeConstantOverride("margin_bottom", 3);
            panel.AddChild(margin);

            var label = new Label
            {
                Text = $"{Title} 0",
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            label.AddThemeFontSizeOverride("font_size", 20);
            label.AddThemeColorOverride("font_color", new Color("cdeaa2"));
            label.AddThemeColorOverride("font_outline_color", new Color(0.04f, 0.07f, 0.03f));
            label.AddThemeConstantOverride("outline_size", 3);
            margin.AddChild(label);

            container.AddChild(panel);

            panel.MouseEntered += () => ShowTip(panel);
            panel.MouseExited += () => NHoverTipSet.Remove(panel);

            _panel = panel;
            _label = label;
            _player = player;
            Refresh(player);
        }
        catch (Exception e)
        {
            Log.Error($"[Hype] create counter UI failed: {e}");
        }
    }

    public static void Refresh(Player? player)
    {
        if (_panel is null || !GodotObject.IsInstanceValid(_panel))
            return;

        _panel.Visible = HypeSystem.ShouldShow(player);
        if (!_panel.Visible || _label is null)
            return;

        _label.Text = $"{Title} {HypeSystem.GetAmount(player)}";
    }

    private static string Title
        => new LocString("card_keywords", "VIRIDIS_KEYWORD_VIEWER_COUNT.title").GetFormattedText();

    private static void ShowTip(Control owner)
    {
        try
        {
            NHoverTipSet.CreateAndShow(owner, HoverTipFactory.FromKeyword(ViewerCountKeyword.Value));
        }
        catch (Exception e)
        {
            Log.Error($"[Hype] show counter tip failed: {e}");
        }
    }

    public static void Reset()
    {
        try
        {
            if (_panel is not null && GodotObject.IsInstanceValid(_panel))
            {
                NHoverTipSet.Remove(_panel);
                _panel.QueueFree();
            }
        }
        catch
        {
            // 战斗界面可能已经销毁
        }
        finally
        {
            _panel = null;
            _label = null;
            _player = null;
        }
    }
}
