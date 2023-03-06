using System;

using HarmonyLib;

using StardewModdingAPI;

using TheFishmongerFishing.HarmonyPatches;

namespace TheFishmongerFishing;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// The logging instance for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        helper.Events.GameLoop.DayEnding += static (_, _) => FishingPatch.Reset();

        try
        {
            Harmony harmony = new(this.Helper.ModRegistry.ModID);
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Failed while applying harmony patches!\n\n{ex}", LogLevel.Error);
        }
    }
}