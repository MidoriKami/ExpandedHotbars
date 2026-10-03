using System;
using System.Collections.Generic;
using System.Threading;
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

    private readonly SemaphoreSlim saveLock = new(1, 1);
    private bool pendingSave;

    public void Save() {
        IPluginLog.Get().Verbose("Queuing Save for system.config.json");

        Interlocked.Exchange(ref pendingSave, true);

        Task.Run(SaveAsync);
    }

    private async Task SaveAsync() {

        // If we already have a save task running, abort and return.
        if (!await saveLock.WaitAsync(0)) {
            IPluginLog.Get().Verbose("Save in progress for system.config.json, skipping save.");
            return;
        }

        try {
            // The while is in-case another save request came in while we are awaiting SaveConfig to complete.
            // It'll allow this same task to handle multiple saves.
            while (Interlocked.Exchange(ref pendingSave, false)) {
                IPluginLog.Get().Debug("Saving Config system.config.json");
                await Config.SaveConfig(this, $"system.config.json");
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Failed to save system.config.json");
        }
        finally {
            saveLock.Release();
        }
    }
}
