using System;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ExpandedHotbars.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UiOverlay;

namespace ExpandedHotbars.Classes;

public sealed class HotbarController : IAsyncDisposable {
    private readonly OverlayController overlayController;

    public unsafe HotbarController() {
        ThreadSafety.AssertMainThread();

        overlayController = new OverlayController();

        foreach (var hotbarConfig in System.Config.Hotbars) {

            var position = hotbarConfig.Position;

            // If using default position, set to middle of the screen.
            // Sorry to that one person that wants their hotbar to be at the top left of their screen for whatever reason, you're weird.
            if (hotbarConfig.Position == Vector2.Zero) {
                position = (Vector2)AtkStage.Instance()->ScreenSize / 2.0f;
            }

            var newHotbarNode = new HotbarOverlayNode {
                Position = position,
                Config = hotbarConfig,
            };

            overlayController.AddNode(newHotbarNode);
        }
    }

    public async ValueTask DisposeAsync() {
        await IFramework.Get().Run(overlayController.Dispose);
    }
}
