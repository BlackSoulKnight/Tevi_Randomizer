using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Rewired;

using UnityEngine;

namespace TeviRandomizer
{
    class BaseGameFixes
    {
        [HarmonyPatch(typeof(InputButtonManager),"Update")]
        [HarmonyPrefix]
        static void fixControllerNumber(ref InputButtonManager __instance, ref Player ___player)
        {
            if(__instance.currentJoystick > ___player.controllers.joystickCount)
            {
                __instance.currentJoystick = Math.Max(___player.controllers.joystickCount-1,0);
            }
        }
        [HarmonyPatch(typeof(InputButtonManager), "OnControllerDisconnected")]
        [HarmonyPrefix]
        static void fixControllerNumber2(ref InputButtonManager __instance, ref Player ___player)
        {
            if (__instance.currentJoystick > ___player.controllers.joystickCount)
            {
                __instance.currentJoystick = Math.Max(___player.controllers.joystickCount - 1, 0);
            }
        }
    }
}
