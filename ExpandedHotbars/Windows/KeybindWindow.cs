using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Classes;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Extensions;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ExpandedHotbars.Windows;

public class KeybindWindow : Window {

    private KeyListener? keyListener;

    private VirtualKey mainKey = VirtualKey.NO_KEY;
    private VirtualKey modifier = VirtualKey.NO_KEY;

    private readonly List<InputId> conflicts = [];

    public Action<KeybindInfo>? KeybindConfirmed { get; set; }
    public Action? KeybindCleared { get; set; }
    public Action? KeybindCancelled { get; set; }

    public KeybindWindow() : base("Keybind Config") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(350.0f, 250.0f),
            MaximumSize = new Vector2(350.0f, 250.0f),
        };

        Flags |= ImGuiWindowFlags.NoResize;
    }

    public override void Draw() {
        DrawInputDisplay();
        ImGui.ScaledDummy(10.0f);

        DrawConflicts();
        DrawControls();
    }

    private void DrawInputDisplay() {
        ImGui.CenteredText("Input desired key combo");
        ImGui.Separator();
        ImGui.ScaledDummy(5.0f);

        ImGui.CenteredText(modifier is VirtualKey.NO_KEY ? $"{mainKey}" : $"{modifier} + {mainKey}");
    }

    private void DrawConflicts() {
        ImGui.CenteredText("Keybind Conflict(s)");
        ImGui.Separator();
        ImGui.ScaledDummy(5.0f);

        using var child = ImRaii.Child("KeybindConflicts", new Vector2(ImGui.AreaWidth, ImGui.AreaHeight - ImGui.Scaled(24.0f) - ImGui.ItemSpacing.Y));
        if (!child) return;

        if (conflicts.Count is not 0) {
            foreach (var conflict in conflicts) {
                ImGui.Text(conflict.ToString());
            }
        }
        else {
            ImGui.Text("No conflicts found");
        }
    }

    private void DrawControls() {
        var buttonSize = new Vector2(ImGui.TotalWidth / 5.0f, ImGui.Scaled(24.0f));

        if (ImGui.Button("Confirm", buttonSize)) {
            KeybindConfirmed?.Invoke(new KeybindInfo {
                Key = mainKey,
                Modifier = modifier,
            });
            IsOpen = false;
        }

        ImGui.SameLine(ImGui.TotalWidth / 2.0f - buttonSize.X / 2.0f);

        if (ImGui.Button("Clear", buttonSize)) {
            KeybindCleared?.Invoke();
            IsOpen = false;
        }

        ImGui.SameLine(ImGui.TotalWidth - buttonSize.X);

        if (ImGui.Button("Cancel", buttonSize)) {
            KeybindCancelled?.Invoke();
            IsOpen = false;
        }
    }

    public override void OnOpen() {
        base.OnOpen();

        mainKey = VirtualKey.NO_KEY;
        modifier = VirtualKey.NO_KEY;

        conflicts.Clear();

        keyListener = new KeyListener();
        keyListener.OnKeyPressed += KeyPressed;
    }

    public override void OnClose() {
        base.OnClose();

        keyListener?.Dispose();
    }

    private unsafe void KeyPressed(VirtualKey arg1, bool isPressed) {
        if (!IsOpen) return;
        if (!isPressed) return;

        mainKey = VirtualKey.NO_KEY;
        modifier = VirtualKey.NO_KEY;
        foreach (var key in IKeyState.Get().GetValidVirtualKeys()) {
            if (key is VirtualKey.CONTROL or VirtualKey.MENU or VirtualKey.SHIFT) {
                if (IKeyState.Get()[(int)key]) {
                    modifier = key;
                }
            }
            else {
                if (IKeyState.Get()[(int)key]) {
                    mainKey = key;
                }
            }
        }

        conflicts.Clear();
        var keybindSpan = UIInputData.Instance()->GetKeybindSpan();
        foreach (var index in Enumerable.Range(0, keybindSpan.Length)) {
            ref var keybind = ref keybindSpan[index];
            if (keybind.IsKeybindMatch([mainKey, modifier])) {
                conflicts.Add((InputId)index);
            }
        }

        IKeyState.Get().ResetKeyCombo([mainKey, modifier]);
    }
}
