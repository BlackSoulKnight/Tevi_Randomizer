using EventMode;
using UnityEngine;
using Bullet;
using Character;
using HarmonyLib;
using Game;

namespace TeviRandomizer
{
    class PlayerCharacterPatch
    {
        // Remove Second Chance for Deathtrigger
        [HarmonyPatch(typeof(SaveManager),"Check1HPBadge")]
        [HarmonyPostfix]
        static void removeSecondChance(ref bool __result)
        {
            if(ArchipelagoInterface.Instance.isConnected && ArchipelagoInterface.Instance.isDeathLinkTriggered())
            {
                __result = false;
            }
        }
    }
}
