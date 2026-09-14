using System;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using KamiToolKit.UiOverlay;

namespace ExpandedHotbars.Classes;

public sealed class HotbarController : IAsyncDisposable {
    private readonly OverlayController overlayController;

    public HotbarController() {
        ThreadSafety.AssertMainThread();

        overlayController = new OverlayController();
    }

    public async ValueTask DisposeAsync() {
        await IFramework.Get().Run(overlayController.Dispose);
    }
}
