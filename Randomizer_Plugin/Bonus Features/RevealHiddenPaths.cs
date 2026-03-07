using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TeviRandomizer.TeviRandomizerSettings;

namespace TeviRandomizer.Bonus_Features
{
    internal class RevealHiddenPaths
    {

        [HarmonyPatch(typeof(WorldManager), "StartFadeFrontLayer")]
        [HarmonyPrefix]
        static void fade1(ref float target)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])
                target = 0;
        }
        [HarmonyPatch(typeof(WorldManager), "SetFrontLayer")]
        [HarmonyPrefix]
        static void fade2(ref float target)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])
                target = 0;
        }
        [HarmonyPatch(typeof(WorldManager), "Awake")]
        [HarmonyPostfix]
        static void fade0(ref float ___FrontFadeTarget)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])
                ___FrontFadeTarget = 0;
        }

    }
}
