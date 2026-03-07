using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TeviRandomizer.TeviRandomizerSettings;

namespace TeviRandomizer.Bonus_Features
{
    internal class OrbitarPatch
    {

        //Use Sable and Celia without chargeshots
        [HarmonyPatch(typeof(CharacterPhy), "GetRangedControls")]
        [HarmonyPostfix]
        static void disableChargeShots(ref bool ___mustNormal, ref CharacterPhy __instance)
        {

            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_ORB) < (TeviSettings.customFlags[CustomFlags.CebleStart] ? 1 : 2))
                ___mustNormal = true;

        }

        [HarmonyPatch(typeof(OrbBall), "NormalShot")]
        [HarmonyPostfix]
        static void reduceChargeHeld(ref bool __result, ref CharacterPhy ___owner_phy)
        {
            if (__result)
            {
                float num10 = 9f;
                if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_NormalShotReducerA))
                {
                    num10 -= 3.15f;
                }
                if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_NormalShotReducerB))
                {
                    num10 -= 4.05f;
                }
                if (___owner_phy.charge - num10 <= 0 && ___owner_phy.chargeheld > 0)
                {
                    ___owner_phy.charge += 100;
                    ___owner_phy.chargeheld--;
                }
            }
        }

    }
}
