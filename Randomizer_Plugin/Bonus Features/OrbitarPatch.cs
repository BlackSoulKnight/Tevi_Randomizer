using Character;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TeviRandomizer.TeviRandomizerSettings;
using UnityEngine;

namespace TeviRandomizer
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
        [HarmonyPatch(typeof(EventManager), "ReloadOrbStatus")]
        [HarmonyPrefix]
        static bool orbRealoadStuff(ref CharacterBase ____mainCharacter, ref EventManager __instance)
        {
            (____mainCharacter.phy_perfer as CharacterPhy).canShootFlag = false;
            (____mainCharacter.phy_perfer as CharacterPhy).canChargeFlag = false;
            (____mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag = false;
            (____mainCharacter.phy_perfer as CharacterPhy).canBlackFlag = false;
            WorldManager.Instance.CanShootWall = false;
            if (SaveManager.Instance.GetOrb() <= 0)
            {
                __instance.ShowAllOrbs(t: false);
            }
            else
            {
                (____mainCharacter.phy_perfer as CharacterPhy).canShootFlag = true;
                __instance.ShowAllOrbs(t: true);
                if (SaveManager.Instance.GetOrb() >= 2)
                {
                    WorldManager.Instance.CanShootWall = true;

                    (____mainCharacter.phy_perfer as CharacterPhy).canChargeFlag = true;
                    ReloadBar.Instance.EnableMe();
                    if (SaveManager.Instance.GetOrb() >= 3)
                    {
                        (____mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag = SaveManager.Instance.GetItem(ItemList.Type.I19) > 0;
                        (____mainCharacter.phy_perfer as CharacterPhy).canBlackFlag = SaveManager.Instance.GetItem(ItemList.Type.I20) > 0;
                    }
                    __instance.HidePoweredOrbs(t: false);
                }
                else
                {
                    __instance.HidePoweredOrbs(t: true);
                }
            }
            GemaSpineHUDTopLeft.Instance.UpdateSkin();
            if (Time.timeSinceLevelLoad > 2.5f)
            {
                Debug.Log("[EventManager] Orb Status = " + SaveManager.Instance.GetOrb() + " | CanShoot = " + (____mainCharacter.phy_perfer as CharacterPhy).canShootFlag + " | CanCharge = " + (____mainCharacter.phy_perfer as CharacterPhy).canChargeFlag + " | CanShootWall = " + WorldManager.Instance.CanShootWall + " | canWhite = " + (____mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag + " | canBlack = " + (____mainCharacter.phy_perfer as CharacterPhy).canBlackFlag);
            }


            return false;
        }

        [HarmonyPatch(typeof(CharacterPhy), "PrepareSwitchOrb")]
        [HarmonyPrefix]
        static bool newPrepareSwitchOrb(ref Character.OrbType ot, ref bool forceType, ref CharacterBase ___cb_perfer, ref CharacterPhy __instance)
        {
            if (forceType)
            {
                return true;
            }
            else if (__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0)
            {
                //return false;
            }
            else if (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0)
            {
                //return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(CharacterPhy), "SwitchTypeForced")]
        [HarmonyPrefix]
        static bool forcedTypeSwitch(ref CharacterPhy __instance, ref bool __result)                 // SwitchType MODE AABBCC Not Working
        {
            if ((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
            {
                __instance.PrepareSwitchOrb();
                __result = false;
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(CharacterPhy), "SwitchType")]
        [HarmonyPrefix]
        static bool typeSwitch(ref CharacterPhy __instance, ref bool __result)
        {
            if ((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
            {
                __result = false;
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(HUDPosition), "Update")]
        [HarmonyPrefix]
        static bool gibHUD(ref UIType.HUD ___Type, ref RectTransform ___rt, ref float ___startX, ref float ___startY, ref float ___targetX)
        {
            if (!___rt) return false;

            if (___Type == UIType.HUD.MAIN)
            {
                UnityEngine.Vector3 vector = ___rt.anchoredPosition;
                float num = ___startX;
                float b = ___startY;

                if (Utility.isHideHUD())
                {
                    num = ___targetX;
                }
                vector.x = Mathf.Lerp(vector.x, num, Utility.GetSmooth2(15f));
                vector.y = Mathf.Lerp(vector.y, b, Utility.GetSmooth2(15f));
                ___rt.anchoredPosition = vector;

                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterPhy), "UseBoost")]
        [HarmonyPostfix]
        static void switchOrbBeforeSummon(ref CharacterPhy __instance, ref bool __result)
        {
            if (__result)
            {
                if ((SaveManager.Instance.GetItem(ItemList.Type.I19) < 1 && __instance.orbUsing == OrbType.BLACK) || (SaveManager.Instance.GetItem(ItemList.Type.I20) < 1) && __instance.orbUsing == OrbType.WHITE)
                {
                    __instance.PrepareSwitchOrb(false, true, (Character.OrbType)((int)__instance.orbUsing ^ 1));
                    //__instance.orbUsing = (OrbType)((int)__instance.orbUsing ^ 1);
                }
            }
        }
        [HarmonyPatch(typeof(CharacterPhy), "UseBoost")]
        [HarmonyPrefix]
        static bool test(ref CharacterPhy __instance, ref bool __result)
        {
            if (SaveManager.Instance.GetItem(ItemList.Type.I19) < 1 && SaveManager.Instance.GetItem(ItemList.Type.I20) < 1)
            {
                __result = true; return false;
            }


            return true;
        }

        [HarmonyPatch(typeof(Cyril), "ALWAYS")]
        [HarmonyPrefix]
        static void removeFrocedBoost(ref float ___useboost)
        {
            if (___useboost > 0f)
            {
                ___useboost = 0f;
            }
        }
    }
}
