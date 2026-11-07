using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

namespace CultivatePrototype;

/// <summary>
/// 给"从手牌/牌堆选牌"的界面在左侧加一个红色「退回」按钮。
/// 只在栽培自己的选择流程里启用（由 <see cref="RequestCancelable"/> 打开），
/// 避免影响原版那些不允许跳过的选牌。
/// </summary>
[HarmonyPatch(typeof(NCombatPileCardSelectScreen), "_Ready")]
public static class CancelButtonPatch
{
    private static volatile bool _cancelable;

    /// <summary>在调用 CardSelectCmd 之前置 true，结束后置 false。</summary>
    public static bool RequestCancelable
    {
        get => _cancelable;
        set => _cancelable = value;
    }

    [HarmonyPostfix]
    public static void Postfix(NCombatPileCardSelectScreen __instance)
    {
        if (!_cancelable)
            return;

        try
        {
            AddButton(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] add cancel button failed: {e}");
        }
    }

    private static void AddButton(NCombatPileCardSelectScreen screen)
    {
        var confirm = screen.GetNodeOrNull<Control>("%Confirm");

        var button = new Button
        {
            Text = "退回",
            CustomMinimumSize = new Vector2(180, 56),
            MouseDefaultCursorShape = Control.CursorShape.PointingHand
        };

        // 借用原版按钮的主题，再染成红色
        if (confirm?.Theme is not null)
            button.Theme = confirm.Theme;

        button.AddThemeColorOverride("font_color", new Color(1f, 0.32f, 0.32f));
        button.AddThemeColorOverride("font_hover_color", new Color(1f, 0.5f, 0.5f));
        button.AddThemeFontSizeOverride("font_size", 26);

        screen.AddChild(button);
        button.SetAnchorsPreset(Control.LayoutPreset.CenterLeft);
        button.Position = new Vector2(60, -28);

        button.Pressed += () => Complete(screen);

        Log.Info("[Cultivate] cancel button added to selection screen");
    }

    private static void Complete(NCombatPileCardSelectScreen screen)
    {
        try
        {
            var field = typeof(NCardGridSelectionScreen).GetField(
                "_completionSource",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (field?.GetValue(screen) is TaskCompletionSource<IEnumerable<CardModel>> tcs)
            {
                tcs.TrySetResult(Array.Empty<CardModel>());
                Log.Info("[Cultivate] selection cancelled via button");
            }
            else
            {
                Log.Warn("[Cultivate] completion source not found");
            }
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] cancel failed: {e}");
        }
    }
}