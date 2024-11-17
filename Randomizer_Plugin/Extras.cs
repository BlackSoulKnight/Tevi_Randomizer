using EventMode;
using Game;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    class Extras
    {



        static Harmony WhiteFlashPatch = new Harmony("FlashPatch");

        static public void patchWhiteFlash(bool enable = false)
        {
            if (enable)
            {
                WhiteFlashPatch.PatchAll(typeof(WhiteFlash));
            }
            else
            {
                WhiteFlashPatch.UnpatchSelf();
            }
        }

        class WhiteFlash
        {
            [HarmonyPatch(typeof(CharacterBase), "_Update")]
            [HarmonyPrefix]
            static bool noFlash(ref float ___teleport, ref CharacterBase __instance, ref Transform ___t)
            {
                if (___teleport > 0f)
                {


                    if (___teleport > 199f && ___teleport < 300f)
                    {
                        EventManager.Instance.EFF_CreateWarpEffect(___t.position);
                        __instance.spranim_prefer.NoFlash();
                        __instance.spranim_prefer.Invisible(t: true);
                        EventManager.Instance.HideOrbs(t: true);
                        ___teleport = 300f;
                        return false;
                    }
                    if (___teleport > 299f && ___teleport < 1000f && EventManager.Instance.IsWarping() >= 1000f)
                    {
                        __instance.ChangeDirectionToCenter();
                        __instance.spranim_prefer.NoFlash();
                        __instance.spranim_prefer.Invisible(t: false);
                        EventManager.Instance.EFF_CreateWarpEffect(___t.position);
                        EventManager.Instance.HideOrbs(t: false);
                        ___teleport = 1000f;
                        return false;
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(FadeManager), "SetAll")]
            [HarmonyPrefix]
            static bool ripWhiteFade(ref float r, ref float g, ref float b)
            {
                if (r == 1 && g == 1 && b == 1)
                {
                    return false;
                }
                return true;
            }
            [HarmonyPatch(typeof(END_EIDOLON), "EVENT")]
            [HarmonyPrefix]
            static void eidolonFix(ref enemyController ___boss)
            {
                EventManager em = EventManager.Instance;
                if (em.EventStage == 20)
                {
                    MainVar.instance.RunSpeed = 1f;
                    em.NextStage();
                    if (SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) >= 2)
                    {
                        ___boss.DoNotDelete = true;
                        if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            SaveManager.Instance.SetEventFlag(Mode.END_EIDOLON, 1, force: true);
                        }
                        em.StopEvent();
                        em.TryStartEvent(Mode.END_BOOKMARK, force: true);
                    }
                }
            }


        }
    }
}
