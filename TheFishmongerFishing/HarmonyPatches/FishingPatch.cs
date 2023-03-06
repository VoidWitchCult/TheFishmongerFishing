// Derived from MIT-licensed code by atravita
// Original can be found at: https://github.com/atravita-mods/StardewMods/blob/main/Ginger%20Island%20Mainland%20Adjustments/Ginger%20Island%20Mainland%20Adjustments/ScheduleManager/NPCPatches.cs

using System;
using HarmonyLib;
using StardewModdingAPI;

using StardewValley;
using Microsoft.Xna.Framework;

namespace TheFishmongerFishing.HarmonyPatches;

/// <summary>
/// Handles patches on the NPC class to allow beach fishing.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class FishingPatch
{
    private const string Fishmonger = "TheFishmonger"; // replace this with your NPC's internal name

    /// <summary>
    /// resets the sprites of all people who went fishing.
    /// </summary>
    /// <remarks>Call at DayEnding.</remarks>
    internal static void Reset()
    {
        if (Game1.getCharacterFromName(Fishmonger) is NPC npc)
        {
            npc.Sprite.SpriteHeight = 32;
            npc.Sprite.SpriteWidth = 16;
            npc.Sprite.ignoreSourceRectUpdates = false;
            npc.Sprite.UpdateSourceRect();
            npc.drawOffset.Value = Vector2.Zero;
        }
    }

    /// <summary>
    /// Extends sprite to allow for fishing sprites, which are 64px tall.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation key.</param>
    [HarmonyPostfix]
    [HarmonyPatch("startRouteBehavior")]
    private static void StartFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals($"{Fishmonger}_fish", StringComparison.OrdinalIgnoreCase))
            {
                __instance.extendSourceRect(0, 32);
                __instance.Sprite.tempSpriteHeight = 64;
                __instance.drawOffset.Value = new Vector2(0f, 96f);
                __instance.Sprite.ignoreSourceRectUpdates = false;
                if (Utility.isOnScreen(Utility.Vector2ToPoint(__instance.Position), 64, __instance.currentLocation))
                {
                    __instance.currentLocation.playSoundAt("slosh", __instance.getTileLocation());
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors in postfix for startRouteBehavior\n\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Resets sprite when NPCs are done fishing.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation key.</param>
    /// <remarks>Force the reset no matter which map the NPC is currently on.</remarks>
    [HarmonyPostfix]
    [HarmonyPatch("finishRouteBehavior")]
    private static void EndFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals($"{Fishmonger}_fish", StringComparison.OrdinalIgnoreCase))
            {
                __instance.reloadSprite();
                __instance.Sprite.SpriteWidth = 16;
                __instance.Sprite.SpriteHeight = 32;
                __instance.Sprite.UpdateSourceRect();
                __instance.drawOffset.Value = Vector2.Zero;
                __instance.Halt();
                __instance.movementPause = 1;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors in postfix for endRouteBehavior\n\n{ex}", LogLevel.Error);
        }
    }
}
