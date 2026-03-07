using Character;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeviRandomizer.Bonus_Features
{
    internal class AreabombPatch
    {
        //test Features
        //Throw Clusterbomb without Crossbombs
        [HarmonyPatch(typeof(ObjectPhy), "UseBomb")]
        [HarmonyPostfix]
        static void useAreaBomb(ref ObjectPhy __instance, ref CharacterBase ___cb_perfer, ref bool __result)
        {
            if (SaveManager.Instance.CanUseItem(ItemList.Type.ITEM_AREABOMB) > 0 && SaveManager.Instance.CanUseItem(ItemList.Type.ITEM_LINEBOMB) == 0)
            {

                ___cb_perfer.playerc_perfer.meter_bomb.EnableMe(0f);
                __instance.SetCounter(0, 1f);
                __instance.SetCounter(1, 0.48f);
                __instance.SetCounter(5, 0f);
                __instance.SetCounter(18, 0f);
                ___cb_perfer.spranim_prefer.ToggleOther(0);
                ___cb_perfer.SetHitboxStarted(on: false);
                ___cb_perfer.ChangeLogicStatus(PlayerLogicState.TEVI_GROUND_ITEM);
                __result = true;
            }
        }
    }
}
