using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Classes;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Extensions;
using ExpandedHotbars.Windows;
using KamiToolKit;

namespace ExpandedHotbars;

public sealed class ExpandedHotbars : IAsyncDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public async Task LoadAsync(CancellationToken cancellationToken) {
        System.Config = await SystemConfiguration.Load();

        await KamiToolKitLibrary.InitializeAsync(PluginInterface, "ExpandedHotbars");

        await IFramework.Get().Run(() => System.HotbarController = new HotbarController(), cancellationToken);

        System.WindowSystem = new WindowSystem("ExpandedHotbars");
        System.WindowSystem.AddWindow(System.ConfigWindow = new ConfigWindow());

        ICommandManager.Get().AddHandler("/expandedhotbars", new CommandInfo(OnCommand) {
            AllowedInMacros = true,
            ShowInHelp = true,
            HelpMessage = "Show/Hide/Toggle hotbars or the config window",
        });

        PluginInterface.UiBuilder.Draw += System.WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi += System.ConfigWindow.Toggle;
        PluginInterface.UiBuilder.OpenConfigUi += System.ConfigWindow.Toggle;

        System.ConfigWindow.DebugToggle();
    }

    public async ValueTask DisposeAsync() {
        PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi -= System.ConfigWindow.Toggle;
        PluginInterface.UiBuilder.Draw -= System.WindowSystem.Draw;

        ICommandManager.Get().RemoveHandler("/expandedhotbars");

        System.WindowSystem.RemoveAllWindows();

        await System.HotbarController.DisposeAsync();
        await KamiToolKitLibrary.DisposeAsync();
    }

    private static void OnCommand(string command, string arguments) {
        if (command is not "/expandedhotbars") return;

        switch (arguments.Split(' ', 2)) {
            case null or [] or [ "" ] or [ "", "" ]:
                System.ConfigWindow.Toggle();
                break;

            case [ var subCommand, var hotbarName ]:
                if (string.IsNullOrEmpty(hotbarName)) {
                    IChatGui.Get().PrintError("Hotbar name was empty or missing. Usage: /expandedhotbars (show|hide|toggle) (hotbarname)", "ExpandedHotbars");
                    return;
                }

                var hotbarEntry = System.Config.Hotbars
                    .FirstOrDefault(hotbar => string.Equals(hotbarName, hotbar.HotbarName, StringComparison.InvariantCultureIgnoreCase));

                if (hotbarEntry is null) {
                    IChatGui.Get().PrintError($"Unable to find {hotbarName} in config file.", "ExpandedHotbars");
                    return;
                }

                hotbarEntry.IsEnabled = subCommand.ToLowerInvariant() switch {
                    "show" => true,
                    "hide" => false,
                    "toggle" => !hotbarEntry.IsEnabled,
                    _ => hotbarEntry.IsEnabled,
                };

                System.Config.Save();

                break;

            default:
                IChatGui.Get().PrintError($"Unable to parse arguments {arguments}. Usage: /expandedhotbars (show|hide|toggle) (hotbarname)", "ExpandedHotbars");
                return;
        }
    }
}
