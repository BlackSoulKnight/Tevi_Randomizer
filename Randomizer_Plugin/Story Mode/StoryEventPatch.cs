using System;
using System.Collections.Generic;
using System.Text;
using Character;
using EventMode;
using Game;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;


namespace TeviRandomizer.Story_Mode
{
    class StoryEventPatch
    {
        [HarmonyPatch(typeof(Chap0MeetCeliaAndSable), "EVENT")]
        [HarmonyPrefix]
        static void enableChargeShot(ref TrailRenderer[] ___trails, ref Light ___clight, ref Light ___slight)
        {
            EventManager em = EventManager.Instance;
            switch (em.EventStage)
            {
                case 150:
                    if (em.EventTime >= 0.23)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            ___trails[j].gameObject.SetActive(value: false);
                        }
                        em.mainCharacter.spranim_prefer.NoForceAnimation();
                        CameraScript.Instance.transform.position = CameraScript.Instance.targetPosition;
                        FadeManager.Instance.SetFadeColor(1f, 1f, 1f, 1f);
                        FadeManager.Instance.SetFadeSpeed(1f);
                        FadeManager.Instance.SetTargetAlpha(0f);
                        em.ActiveOrbs();
                        em.SetStage(160);
                        UnityEngine.Object.Destroy(___clight.gameObject);
                        UnityEngine.Object.Destroy(___slight.gameObject);
                        SaveManager.Instance.SetOrb(2);
                    }
                    break;
                default:
                    break;
            }
        }
        [HarmonyPatch(typeof(Chap0GoTooFar), "REQUIREMENT")]
        [HarmonyPostfix]
        static void chap0BLocker(ref bool __result)
        {
            __result = false;
        }
        [HarmonyPatch(typeof(Chap1GoShopFirst), "REQUIREMENT")]
        [HarmonyPostfix]
        static void chap1ShopBlocker(ref bool __result)
        {
            __result = false;
        }
        [HarmonyPatch(typeof(Chap1FoundAirCam), "REQUIREMENT")]
        [HarmonyPostfix]
        static void chap1Cam(ref bool __result)
        {
            __result = true;
        }
        [HarmonyPatch(typeof(BOSS_LILY), "REQUIREMENT")]
        [HarmonyPostfix]
        static void BOSSLILY(ref bool __result)
        {
            if (SaveManager.Instance.GetItem(ItemList.Type.QUEST_AirCam) != 0)
            {
                __result = true;
            }
            else
            {
                __result = false;
            }
        }
        [HarmonyPatch(typeof(Chap1S_Mine_Gromp), "REQUIREMENT")]
        [HarmonyPostfix]
        static void BombFuelGuy(ref bool __result)
        {
            if (SaveManager.Instance.GetItem(ItemList.Type.QUEST_Seal) != 0 && WorldManager.Instance.Area == 6 && SaveManager.Instance.GetEventFlag(Mode.Chap1S_Mine_Discover) > 0)
            {
                __result = true;
            }
            else
            {
                __result = false;
            }
        }
        [HarmonyPatch(typeof(BOSS_VENA5x5), "REQUIREMENT")]
        [HarmonyPostfix]
        static void BOSSVENA(ref enemyController ___r)
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 160)
            {
                if (em.EventTime<= 0.6f)
                {
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    CameraScript.Instance.ResetTarget();
                    MusicManager.Instance.PlayMusic(Music.BOSS13);
                    SaveManager.Instance.SetBuffInfo(BuffType.CatDefence_TOP, 2);
                    SaveManager.Instance.SetBuffInfo(BuffType.BunDefence_TOP, 2);
                    ___r.spranim_prefer.ToggleJumpStand(0);
                    if (SaveManager.Instance.GetItemCountInBag(ItemList.Type.OFF) <= 0)
                    {
                        SaveManager.Instance.AddItemToBag(ItemList.Type.Useable_CocoBall);
                    }
                    else if (SaveManager.Instance.GetItemCountInBag(ItemList.Type.OFF) == 1 && SaveManager.Instance.GetItemCountInBag(ItemList.Type.Useable_Biscuit) == 1)
                    {
                        SaveManager.Instance.AddItemToBag(ItemList.Type.Useable_CocoBall);
                    }
                    SaveManager.Instance.SetMiniFlag(Mini.CanOpenBag, 1);
                    if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam) && !SaveManager.Instance.GetCustomGame(CustomGame.RandomBadge))
                    {
                        HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_TauntThink, 1);
                    }
                    em.StopEvent();
                    em.StartBoss();
                    ___r.SetAttackVoiceCoolDownTime(28f);
                }
            }
        }
    }
}
