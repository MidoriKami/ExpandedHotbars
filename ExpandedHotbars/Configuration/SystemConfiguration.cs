using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Utilities;

namespace ExpandedHotbars.Configuration;


public class SystemConfiguration {

    public List<HotbarConfig> Hotbars = [];

    public static async Task<SystemConfiguration> Load() {
        IPluginLog.Get().Debug("Loading system.config.json");
        return await Config.LoadConfig<SystemConfiguration>("system.config.json");
    }

    private Task? saveTask;

    public void Save() {
        IPluginLog.Get().Verbose("Queuing Save for system.config.json");

        if (saveTask is null || saveTask.IsCompleted) {
            saveTask = Task.Run(SaveAsync);
        }
        else {
            IPluginLog.Get().Verbose("Save already in progress for system.config.json, save aborted. File contents may be out of date.");
        }
    }

    private async Task SaveAsync() {
        try {
            IPluginLog.Get().Debug("Saving Config system.config.json");
            await Config.SaveConfig(this, "system.config.json");
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, "Failed to save system.config.json");
        }
    }
}
