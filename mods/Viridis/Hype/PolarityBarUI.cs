using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Viridis.Characters;

namespace Viridis.Hype;

/// <summary>
/// 角色 HP 条上方的「反踩 (-8) ← 黑听 (0) → 入脑 (+8)」双极滑条。
/// 挂在 NCreatureStateDisplay 内并跟随 _healthBar 定位。
/// </summary>
public static class PolarityBarUI
{
    private const float BarWidth = 224f;
    private const float BarHeight = 28f;
    private const float UsableHalf = 104f;
    private const float ThumbW = 24f;
    private const float ThumbH = 28f;
    private const float GapAboveHpBar = 8f;

    private static readonly Color EarwormFill = new(0.56f, 0.78f, 0.34f, 0.88f);
    private static readonly Color BacklashFill = new(0.88f, 0.38f, 0.24f, 0.88f);
    private static readonly Color EarwormText = new("d9f5a8");
    private static readonly Color BacklashText = new("ffb89e");
    private static readonly Color LurkingText = new("dde4d8");

    private sealed class Holder
    {
        public required NCreatureStateDisplay Display;
        public required NHealthBar HealthBar;
        public required Player Player;
        public required Control Root;
        public required ColorRect Fill;
        public required TextureRect Thumb;
        public required Label Label;
    }

    private static readonly Dictionary<ulong, Holder> _holders = new();

    public static void Attach(NCreatureStateDisplay display, NHealthBar? healthBar, Creature? creature)
    {
        try
        {
            if (healthBar is null || creature?.IsPlayer != true || creature.Player is not { } player)
                return;

            if (!ShouldShow(player))
                return;

            ulong id = display.GetInstanceId();
            if (_holders.TryGetValue(id, out var existing) && GodotObject.IsInstanceValid(existing.Root))
            {
                existing.Player = player;
                Reposition(display, healthBar);
                UpdateHolder(existing);
                return;
            }

            var root = new Control
            {
                Name = "ViridisPolarityBar",
                CustomMinimumSize = new Vector2(BarWidth, BarHeight),
                Size = new Vector2(BarWidth, BarHeight),
                MouseFilter = Control.MouseFilterEnum.Stop,
                ZIndex = 2
            };

            var trackTex = GD.Load<Texture2D>("res://Viridis/images/ui/polarity_bar_track.ctex");
            var thumbTex = GD.Load<Texture2D>("res://Viridis/images/ui/polarity_bar_thumb.ctex");

            var track = new TextureRect
            {
                Texture = trackTex,
                Position = Vector2.Zero,
                Size = new Vector2(BarWidth, BarHeight),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.Scale,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            root.AddChild(track);

            var fill = new ColorRect
            {
                Position = new Vector2(BarWidth * 0.5f, 6f),
                Size = new Vector2(0f, BarHeight - 12f),
                Color = EarwormFill,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            root.AddChild(fill);

            var thumb = new TextureRect
            {
                Texture = thumbTex,
                Position = new Vector2(BarWidth * 0.5f - ThumbW * 0.5f, 0f),
                Size = new Vector2(ThumbW, ThumbH),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.Scale,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            root.AddChild(thumb);

            var label = new Label
            {
                Position = Vector2.Zero,
                Size = new Vector2(BarWidth, BarHeight),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            label.AddThemeFontSizeOverride("font_size", 15);
            label.AddThemeColorOverride("font_outline_color", new Color(0.05f, 0.07f, 0.05f));
            label.AddThemeConstantOverride("outline_size", 3);
            root.AddChild(label);

            display.AddChild(root);

            var holder = new Holder
            {
                Display = display,
                HealthBar = healthBar,
                Player = player,
                Root = root,
                Fill = fill,
                Thumb = thumb,
                Label = label
            };
            _holders[id] = holder;

            root.MouseEntered += () => ShowTip(holder);
            root.MouseExited += () => NHoverTipSet.Remove(root);

            Reposition(display, healthBar);
            UpdateHolder(holder);
        }
        catch (Exception e)
        {
            Log.Error($"[Hype] PolarityBarUI.Attach failed: {e}");
        }
    }

    public static void Reposition(NCreatureStateDisplay display, NHealthBar? healthBar)
    {
        if (healthBar is null)
            return;

        if (!_holders.TryGetValue(display.GetInstanceId(), out var h) || !GodotObject.IsInstanceValid(h.Root))
            return;

        float hpW = healthBar.Size.X > 10f ? healthBar.Size.X : 200f;
        float x = healthBar.Position.X + (hpW - BarWidth) * 0.5f;
        float y = healthBar.Position.Y - BarHeight - GapAboveHpBar;
        h.Root.Position = new Vector2(x, y);
    }

    public static void Refresh(Player? player)
    {
        if (player is null)
            return;

        foreach (var kv in _holders.ToList())
        {
            if (!GodotObject.IsInstanceValid(kv.Value.Root))
            {
                _holders.Remove(kv.Key);
                continue;
            }

            if (ReferenceEquals(kv.Value.Player, player))
                UpdateHolder(kv.Value);
        }
    }

    public static void Reset()
    {
        foreach (var h in _holders.Values)
        {
            if (GodotObject.IsInstanceValid(h.Root))
            {
                NHoverTipSet.Remove(h.Root);
                h.Root.QueueFree();
            }
        }
        _holders.Clear();
    }

    private static bool ShouldShow(Player player)
        => player.Character is ViridisCharacter
            || HypeSystem.EarwormStacks(player) > 0
            || HypeSystem.BacklashStacks(player) > 0;

    private static void UpdateHolder(Holder h)
    {
        int earworm = HypeSystem.EarwormStacks(h.Player);
        int backlash = HypeSystem.BacklashStacks(h.Player);
        int net = Math.Clamp(earworm - backlash, -HypeSystem.MaxPolarityStacks, HypeSystem.MaxPolarityStacks);

        float cx = BarWidth * 0.5f;
        float ratio = Math.Abs(net) / (float)HypeSystem.MaxPolarityStacks;
        float fillW = ratio * UsableHalf;

        if (net > 0)
        {
            h.Fill.Color = EarwormFill;
            h.Fill.Position = new Vector2(cx, 6f);
            h.Fill.Size = new Vector2(fillW, BarHeight - 12f);
            h.Label.Text = $"{new LocString("card_keywords", "VIRIDIS_KEYWORD_EARWORM.title").GetFormattedText()} +{net}";
            h.Label.AddThemeColorOverride("font_color", EarwormText);
        }
        else if (net < 0)
        {
            h.Fill.Color = BacklashFill;
            h.Fill.Position = new Vector2(cx - fillW, 6f);
            h.Fill.Size = new Vector2(fillW, BarHeight - 12f);
            h.Label.Text = $"{new LocString("card_keywords", "VIRIDIS_KEYWORD_BACKLASH.title").GetFormattedText()} {net}";
            h.Label.AddThemeColorOverride("font_color", BacklashText);
        }
        else
        {
            h.Fill.Size = new Vector2(0f, BarHeight - 12f);
            h.Label.Text = $"{new LocString("card_keywords", "VIRIDIS_KEYWORD_LURKING.title").GetFormattedText()} 0";
            h.Label.AddThemeColorOverride("font_color", LurkingText);
        }

        float thumbX = cx + (net / (float)HypeSystem.MaxPolarityStacks) * UsableHalf - ThumbW * 0.5f;
        h.Thumb.Position = new Vector2(thumbX, 0f);
    }

    private static void ShowTip(Holder h)
    {
        try
        {
            int net = HypeSystem.NetPolarity(h.Player);
            var tip = net > 0
                ? HoverTipFactory.FromKeyword(EarwormKeyword.Value)
                : net < 0
                    ? HoverTipFactory.FromKeyword(BacklashKeyword.Value)
                    : HoverTipFactory.FromKeyword(LurkingKeyword.Value);

            NHoverTipSet.CreateAndShow(h.Root, tip);
        }
        catch (Exception e)
        {
            Log.Error($"[Hype] PolarityBarUI.ShowTip failed: {e.Message}");
        }
    }
}
