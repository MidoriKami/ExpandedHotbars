using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Enums;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;
using KamiToolKit.Extensions;

namespace ExpandedHotbars.Nodes;

public sealed class HotbarOverlayNode : OverlayNode {
    public required HotbarConfig Config { get; init; }

    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

    protected override unsafe void OnUpdate() {
        IsVisible = Config.ShouldShowHotbar() && Config.IsEnabled && !IClientState.Get().IsPvP;
        if (!IsVisible) return;

        Scale = new Vector2(Config.Scale, Config.Scale);

        if (Config.UpdateFlags.HasFlag(ConfigChangedKind.NeedsRebuild)) {
            RebuildLayout();
            Config.UpdateFlags &= ~ConfigChangedKind.NeedsRebuild;
        }

        if (Config.UpdateFlags.HasFlag(ConfigChangedKind.NeedsUpdate)) {
            RecalcLayout();
            Config.UpdateFlags &= ~ConfigChangedKind.NeedsUpdate;
        }

        EnableMoving = Config.IsMovingEnabled;

        IGameConfig.Get().UiConfig.TryGetBool("HotbarLock", out var isHotbarLocked);
        IGameConfig.Get().UiControl.TryGetBool("HotbarEmptyVisible", out var isHotbarEmptyVisible);

        padlockNode.IsVisible = Config.ShowPadlockButton;
        padlockNode.IsLocked = isHotbarLocked;

        ref var dragDropManager = ref AtkStage.Instance()->DragDropManager;
        var isDragging = dragDropManager is { IsDragging: true, MouseMoved: true };

        foreach (var node in hotbarNodes) {
            node.IsDraggable = !isHotbarLocked;
            node.IsBackgroundShown = isHotbarEmptyVisible || isDragging || System.ConfigWindow.IsOpen;

            node.Update();
        }
    }

    public unsafe HotbarOverlayNode() {
        padlockNode = new PadlockButtonNode {
            Size = new Vector2(20.0f, 24.0f),
            TextTooltip = "Left Click to lock/unlock hotbar slots.\n" +
                          "Right Click to enable moving hotbar.\n" +
                          "Control + Click to open config.",
        };
        padlockNode.AddEvent(AtkEventType.MouseClick, OnPadlockButtonClick);
        padlockNode.RemoveEvent(AtkEventType.ButtonClick);
        padlockNode.AttachNode(this);

        OnMoveComplete = OnMoveCompleted;
    }

    private void RebuildLayout() {
        foreach (var node in hotbarNodes) {
            node.Dispose();
        }
        hotbarNodes.Clear();

        foreach (var row in Enumerable.Range(0, (int) Config.Size.Y)) {
            foreach (var column in Enumerable.Range(0, (int) Config.Size.X)) {
                var newHotbarNode = new HotbarNode {
                    Position = new Vector2(8.0f, 8.0f) +
                               new Vector2(44.0f * column, 44.0f * row) +
                               new Vector2(Config.Spacing.X * column, Config.Spacing.Y * row),
                    Size = new Vector2(44.0f, 44.0f),
                    IsClickable = true,
                };

                hotbarNodes.Add(newHotbarNode);

                newHotbarNode.OnPayloadAccepted += (_, payload) => OnPayloadAccepted(row, column, payload);
                newHotbarNode.OnDiscard += _ => OnPayloadDiscard(row, column);

                if (Config.Actions.TryGetValue((row, column), out var actionInfo)) {
                    newHotbarNode.SetSlot(actionInfo.DragDropType, actionInfo.ActionId);
                }

                if (Config.Keybinds.TryGetValue((row, column), out var keybindInfo)) {
                    newHotbarNode.KeyBind = keybindInfo;
                }

                newHotbarNode.AttachNode(this);
                newHotbarNode.Update();
            }
        }

        Size = Config.Size * (new Vector2(44.0f, 44.0f) + Config.Spacing) + new Vector2(16.0f, 16.0f);
        padlockNode.Position = Size + new Vector2(8.0f, -42.0f);
    }

    private void RecalcLayout() {
        foreach (var row in Enumerable.Range(0, (int) Config.Size.Y)) {
            foreach (var column in Enumerable.Range(0, (int) Config.Size.X)) {
                hotbarNodes[(int) (column + row * Config.Size.X)].Position =
                    new Vector2(8.0f, 8.0f) +
                    new Vector2(44.0f * column, 44.0f * row) +
                    new Vector2(Config.Spacing.X * column, Config.Spacing.Y * row);

                if (Config.Keybinds.TryGetValue((row, column), out var keybindInfo)) {
                    hotbarNodes[(int) (column + row * Config.Size.X)].KeyBind = keybindInfo;
                }
                else {
                    hotbarNodes[(int)(column + row * Config.Size.X)].KeyBind = new KeySetting {
                        Key = SeVirtualKey.NO_KEY,
                        KeyModifier = KeyModifierFlag.None,
                    };
                }
            }
        }

        Size = Config.Size * (new Vector2(44.0f, 44.0f) + Config.Spacing) + new Vector2(16.0f, 16.0f);
        padlockNode.Position = Size + new Vector2(8.0f, -42.0f);
    }

    private void OnPayloadAccepted(int row, int column, DragDropPayload payload) {
        Config.Actions[(row, column)] = new ActionInfo {
            DragDropType = payload.Type,
            ActionId = (uint) payload.Int2,
        };

        System.Config.Save();
    }

    private void OnPayloadDiscard(int row, int column) {
        Config.Actions.Remove((row, column));
        Update();

        System.Config.Save();
    }

    private void OnMoveCompleted(NodeBase _) {
        Config.Position = Position;
        Update();

        System.Config.Save();
    }

    private unsafe void OnPadlockButtonClick(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (atkEventData->IsControlHeld) {
            System.ConfigWindow.Toggle();
            return;
        }

        if (atkEventData->IsLeftClick) {
            if (!IGameConfig.Get().UiConfig.TryGetBool("HotbarLock", out var isHotbarLocked)) return;
            IGameConfig.Get().UiConfig.Set("HotbarLock", !isHotbarLocked);
        }
        else if (atkEventData->IsRightClick) {
            Config.IsMovingEnabled = !Config.IsMovingEnabled;
        }
    }

    private readonly List<HotbarNode> hotbarNodes = [];
    private readonly PadlockButtonNode padlockNode;
}
