﻿using Character;
using EventMode;
using Game;
using HarmonyLib;
using Map;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    class EventPatch
    {

        // Free Start Items
        [HarmonyPatch(typeof(Chap0GetKnife), "EVENT")]
        [HarmonyPrefix]
        static bool StartEvent()
        {
            EventManager em = EventManager.Instance;
            if (HUDObtainedItem.Instance.isDisplaying()) return false;

            if (em.EventStage == 1)
            {
                SaveManager.Instance.SetOrb(0);
                SaveManager.Instance.SetItem(ItemList.Type.BADGE_BossPassing, 0);
                SaveManager.Instance.SetItem(ItemList.Type.BADGE_FreeFoodRefill, 0);
                LocationTracker.setCollectedLocationList([]);

                if (ArchipelagoInterface.Instance?.isConnected == true)
                {
                    ArchipelagoInterface.Instance.currentItemNR = 0;
                }

                if (RandomizerPlugin.customFlags[(int)CustomFlags.CompassStart])
                {
                    SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 4);
                    SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 5);
                    SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 6);
                }
                for (int i = RandomizerPlugin.extraPotions[0]; i > 0; i--)
                {
                    SaveManager.Instance.SetItem(ItemList.Type.STACKABLE_RATK, 1, true);
                }
                for (int i = RandomizerPlugin.extraPotions[1]; i > 0; i--)
                {
                    SaveManager.Instance.SetItem(ItemList.Type.STACKABLE_MATK, 1, true);
                }
                if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam)) // Story Mode unstuck Cross Bomb dependancy
                {
                    SaveManager.Instance.AddBreakTile(0, 394, 244);
                    SaveManager.Instance.AddBreakTile(0, 394, 245);
                    SaveManager.Instance.AddBreakTile(0, 367, 246);
                    SaveManager.Instance.AddBreakTile(0, 367, 247);
                    SaveManager.Instance.AddBreakTile(0, 367, 248);
                }

                if (RandomizerPlugin.customStartDiff >= 0)
                    SaveManager.Instance.SetDifficulty(RandomizerPlugin.customStartDiff);
                // Make a Path to Morose
                if (RandomizerPlugin.customFlags[(int)CustomFlags.TempOption])
                {
                    // Blocks to the area between Canyon and morose
                    SaveManager.Instance.AddBreakTile(1, 302, 189);
                    SaveManager.Instance.AddBreakTile(1, 303, 189);
                    SaveManager.Instance.AddBreakTile(1, 304, 189);
                    // Teleporter area
                    SaveManager.Instance.AddBreakTile(3, 456, 189);
                    SaveManager.Instance.AddBreakTile(3, 455, 189);
                    SaveManager.Instance.AddBreakTile(3, 455, 188);
                    SaveManager.Instance.AddBreakTile(3, 456, 188);
                }

            }
            else if (em.EventStage == 20)
            {
                if (em.EventTime > 0.1f && em.EventTime < 100f)
                {
                    SaveManager.Instance.SetOrb(0);
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.ITEM_ORB, 1);
                    em.EventTime = 100f;
                }
                else if (em.EventTime > 100.5f)
                {
                    em.SetStage(21);
                }
                return false;
            }
            else if (em.EventStage == 21)
            {
                if (!SaveManager.Instance.GetCustomGameMainVar(CustomGame.FreeRoam))
                {
                    if (em.EventTime > 0.1f && em.EventTime < 100f)
                    {
                        HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_BossPassing, 1);
                        em.EventTime = 100f;
                    }
                    else if (em.EventTime > 100.5f)
                    {
                        em.SetStage(22);
                    }
                }
                else
                    em.SetStage(30);
                return false;
            }
            else if (em.EventStage == 22)
            {
                if (!SaveManager.Instance.GetCustomGameMainVar(CustomGame.FreeRoam))
                {
                    if (em.EventTime > 0.1f && em.EventTime < 100f)
                    {
                        HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_FreeFoodRefill, 1);
                        em.EventTime = 100f;
                    }
                    else if (em.EventTime > 100.5f)
                    {
                        em.SetStage(30);
                    }
                }
                else
                    em.SetStage(30);
                return false;
            }
            else if (em.EventStage == 40)
            {
                //ShopPatch.alreadyClaimed();
                //VenaItemClaimedCheck();
                if (RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart])
                {
                    SaveManager.Instance.SetItem(ItemList.Type.I19, 1);
                    SaveManager.Instance.SetItem(ItemList.Type.I20, 1);
                }
            }
            return true;
        }

        static void VenaItemClaimedCheck()
        {
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_COG, 23))
            {
                SaveManager.Instance.SetEventFlag(Mode.END_VENA, 1, true);
                SaveManager.Instance.AddResource(ItemList.Resource.COIN, 1000);
                SaveManager.Instance.AddBossBeaten();
            }
        }

        [HarmonyPatch(typeof(AfterMemineChallenge), "EVENT")]
        [HarmonyPrefix]
        static void MemineAllChallangesChecl(ref CharacterBase ___m)
        {
            EventManager em = EventManager.Instance;
            switch (EventManager.Instance.EventStage)
            {
                case 30:
                    em.NextStage();
                    break;
                case 40:
                    {
                        MusicManager.Instance.PlayRoomMusic();
                        GemaMissionMode.Instance.MissionCleared();
                        em.StopEvent();
                        ___m.DoNotDelete = false;
                        ___m.ID = 32;

                        int num = 0;
                        for (int i = 0; i <= 5; i++)
                        {
                            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_SHARD, (byte)i))
                            {
                                num++;
                            }
                        }
                        if (num >= 3 && SaveManager.Instance.GetMiniFlag(Mini.UnlockExplorerUpgrade) <= 0)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockExplorerUpgrade, 1);
                            HUDPopupMessage.Instance.StartCraftAddedMessage();
                        }

                        if (num == 6)
                        {
                            Debug.Log(EventManager.Instance.TryStartEvent(Mode.AllMemineWon, force: true));
                        }
                        break;
                    }

            }
        }

        [HarmonyPatch(typeof(EventManager), "MovePlayerToWarpDevice3")]
        [HarmonyPrefix]
        static bool noWarp()
        {
            return false;
        }

        [HarmonyPatch(typeof(Chap1FreeRoamVena7x7), "REQUIREMENT")]
        [HarmonyPrefix]
        static bool Vena7x7Fix(ref bool __result)
        {
            if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
            {
                __result = false;
                return false;
            }

            __result = (!RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_COG, 23) || SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) == 1);
            return false;
        }

        [HarmonyPatch(typeof(EventDetect), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        static bool VenaEXTRA(ref Collider2D col)
        {
            EventTile component = col.GetComponent<EventTile>();
            if ((bool)component)
            {
                if (EventManager.Instance.GetCurrentEvent() == Mode.OFF && !EventManager.Instance.isBossMode() && component.mode != Mode.LibraryPoint && component.mode != Mode.SnowCaveMazeDisabled && component.mode != Mode.MazeCompleted && !GemaBossRushMode.Instance.isBossRush())
                {
                    if (EventManager.Instance.NoEventStartFromDetect)
                    {
                        Debug.Log("[EventDetect] No Event Start From Detect is ON. This event cannot be triggered : " + component.mode);
                    }
                    else if (EventManager.Instance.CheckEventStartable(component.mode))
                    {
                        if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            bool flag = (!RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_COG, 23) || SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) == 1);
                            if (component.mode == Mode.Chap1FreeRoamVena7x7)
                            {
                                if ((component.mode == Mode.Chap1FreeRoamVena7x7 && SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) == 1) || flag)
                                {
                                    if (WorldManager.Instance.CheckIsWall(component.transform.position, any: false) == 1)
                                    {
                                        Debug.Log("[EventDetect] Event is inside wall, cannot trigger : " + component.mode);
                                        flag = false;
                                    }
                                    if (flag && EventManager.Instance.TryStartEvent(component.mode, force: false))
                                    {
                                        EventManager.Instance.LastHitTrigger = component;
                                    }
                                }
                                return false;
                            }
                            if (component.mode == Mode.BOSS_TAHLIA || component.mode == Mode.BOSS_REVENANCE)
                            {
                                if (WorldManager.Instance.CheckIsWall(component.transform.position, any: false) == 1)
                                {
                                    Debug.Log("[EventDetect] Event is inside wall, cannot trigger : " + component.mode);
                                    flag = false;
                                }
                                if (EventManager.Instance.TryStartEvent(component.mode, force: false))
                                {
                                    EventManager.Instance.LastHitTrigger = component;
                                }
                                return false;
                            }
                        }

                    }
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(Chap0Start), "CheckSR")]
        [HarmonyPrefix]
        static bool StartItemSetup()
        {
            if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
            {
                SaveManager.Instance.SetItem(ItemList.Type.ITEM_KNIFE, 0);
                SaveManager.Instance.SetItem(ItemList.Type.ITEM_ORB, 0);
                SaveManager.Instance.SetEventFlag(Mode.SWITCH_BASEKNIFE, 1, force: true);
                SaveManager.Instance.SetEventFlag(Mode.Chap0CandR, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0SeeBomb, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0SeeEnergyBall, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0SeeGoal, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0Outside, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0Outline7x7, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0CombatTut7x7, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap3ComboRank, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap1PinTool9x9, 1);
                SaveManager.Instance.SetEventFlag(Mode.Chap0UnlockFirstVent, 1, force: true);
                SaveManager.Instance.SetMiniFlag(Mini.UnlockedCraft, 1);
                EventManager.Instance.TryStartEvent(Mode.Chap0GetKnife, force: true);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(EventManager), "CheckAfterMapChange")]
        [HarmonyPrefix]
        static void dontTakeHands(ref (byte, byte) __state)
        {
            __state = (SaveManager.Instance.GetItem(ItemList.Type.QUEST_GHandL), SaveManager.Instance.GetItem(ItemList.Type.QUEST_GHandR));
        }

        [HarmonyPatch(typeof(EventManager), "CheckAfterMapChange")]
        [HarmonyPostfix]
        static void returnHands(ref (byte, byte) __state)
        {
            if (__state.Item1 > 0)
                SaveManager.Instance.SetItem(ItemList.Type.QUEST_GHandL, __state.Item1);
            if (__state.Item2 > 0)
                SaveManager.Instance.SetItem(ItemList.Type.QUEST_GHandR, __state.Item2);
        }

        [HarmonyPatch(typeof(enemyController), "handexhange_delaystart2")]
        [HarmonyPrefix]
        static bool noHandExchange()
        {
            SettingManager.Instance.SetAchievement(Achievements.ACHI_ITEM_GOLDENHANDS);
            return false;
        }


        [HarmonyPatch(typeof(Chap8FreeRoamNoIllusionPalace7x7), "REQUIREMENT")]
        [HarmonyPrefix]
        static bool IllusionReq(ref bool __result)
        {
            __result = false;
            switch (RandomizerPlugin.goalType)
            {
                case RandomizerPlugin.GoalType.BossDefeat:
                    if(SaveManager.Instance.GetCurrentBossBeaten() > 20)
                    {
                        __result = true;
                    }
                    break;
                case RandomizerPlugin.GoalType.AstralGear:
                default:
                    if (SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_COG) < RandomizerPlugin.GoMode)
                    {
                        __result = true;
                        return false;
                    }
                    break;

            }
        
        

            return false;
        }

        [HarmonyPatch(typeof(Chap8FreeRoamNoIllusionPalace7x7), "EVENT")]
        [HarmonyPrefix]
        static bool IllusionText()
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 50)
            {
                if (em.EventTime >= 1f)
                {
                    em.StopEvent();
                    if (!HUDPopupMessage.Instance.gameObject.activeInHierarchy)
                    {
                        string msg = Localize.GetLocalizeTextWithKeyword("FreeRoamNotEnoughCog", contains: false).Replace("16", RandomizerPlugin.GoMode.ToString());
                        HUDPopupMessage.Instance.AddMessage(Localize.GetLocalizeTextWithKeyword("POPUP_INFORMATION", contains: false), msg, null, 0);
                    }
                    em.AllowSkip = true;
                }
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(END_REVENANCE),"EVENT")]
        [HarmonyPrefix]
        static void EndArchipelago()
        {
            if(EventManager.Instance.EventStage == 40 && EventManager.Instance.EventTime >=2.33f)
            {
                ArchipelagoInterface.Instance.sendGOAL();
            }
        }

        // All Events that have Random Badge = true
        [HarmonyPatch(typeof(Chap4CyrilRoom),"EVENT")]
        [HarmonyPrefix]
        static bool CyrilBadge()
        {
            if(EventManager.Instance.EventStage == 10 && EventManager.Instance.EventTime > 0.7f && EventManager.Instance.EventTime < 100f)
            {
                HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_CrystalAbsorberS, 1);
                EventManager.Instance.EventTime = 100f;
            }
            return true;
        }

        [HarmonyPatch(typeof(AllMemineWon),"EVENT")]
        [HarmonyPrefix]
        static bool MemineAllBadge()
        {
            if (EventManager.Instance.EventStage == 20 && EventManager.Instance.EventTime > 0.7f && EventManager.Instance.EventTime < 100f)
            {
                HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_DoubleAirDash, 1);
                EventManager.Instance.EventTime = 100f;
            }
            return true;
        }

        [HarmonyPatch(typeof(AfterMission),"EVENT")]
        [HarmonyPrefix]
        static bool AfterMissionBadge()
        {
            EventManager em = EventManager.Instance;
            if (EventManager.Instance.EventStage == 50 && EventManager.Instance.EventTime > 0.7f && em.EventTime < 100f)
            {
                if (em.getSubMode() == Mode.StartMission3A)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_CrystalGen, 1);
                }
                if (em.getSubMode() == Mode.StartMission3B)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_BoostSizeIncrease, 1);
                }
                if (em.getSubMode() == Mode.StartMission3C)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_BoostCostCut, 1);
                }
                if (em.getSubMode() == Mode.StartMission8A)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_AmuletDouble, 1);
                }
                if (em.getSubMode() == Mode.StartMission15A)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_AmuletQuicken, 1);
                }
                if (em.getSubMode() == Mode.StartMission20A)
                {
                    HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_BoostHitCount, 1);
                }
                em.EventTime = 100f;
            }
            return true;
        }


        // Library Bosses
        // may have to out source GiveItem function to have bosses with higher id than 255 
        static Character.Type boss = Character.Type.NONE;
        [HarmonyPatch(typeof(END_BOOKMARK),"EVENT")]
        [HarmonyPrefix]
        static void CustomBossesReward()
        {
            EventManager em = EventManager.Instance;
            if(em.EventStage == 0)
            {
                for (int num = CharacterManager.Instance.characters.Count - 1; num >= 0; num--)
                {
                    if (CharacterManager.Instance.characters[num] != null && !CharacterManager.Instance.characters[num].isPlayer() && CharacterManager.Instance.characters[num].isBoss != BossType.SUMMON && CharacterManager.Instance.characters[num].isBoss != BossType.NPC)
                    {
                        boss = CharacterManager.Instance.characters[num].type;
                    }
                }
            }
            if(em.EventStage == 100)
            {
                if (boss == Character.Type.Waero_B || boss == Character.Type.EinLee_B || boss == Character.Type.GemaYue_B)
                {
                    em.SetStage(101);
                }
            }
            if(em.EventStage == 101)
            {
                if (!(em.EventTime > 2.5f))
                {
                    return;
                }
                EventManager.Instance.AllowAutoMap = false;
                em.StopEvent();
                WorldManager.Instance.ToggleFlagGate(t: true);
                MusicManager.Instance.PlayRoomMusic();
                CameraScript.Instance.DisableEnemySpawn = true;
                SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 0);
                if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.NoBookmarkUntilMapChange, 1);
                }
                if(boss == Character.Type.Waero_B || boss == Character.Type.EinLee_B || boss == Character.Type.GemaYue_B) {

                    int id = (int)boss % 3 + 200;

                    if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam) && !RandomizerPlugin.checkItemGot(ItemList.Type.STACKABLE_COG, (byte)id))
                    {
                        HUDObtainedItem.Instance.GiveItem(ItemList.Type.STACKABLE_COG, (byte)id);
                    }
                    if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) <= 0)
                    {
                        HUDResourceGotPopup.Instance.AddPopup(ItemList.Resource.COIN, SaveManager.Instance.GetResource(ItemList.Resource.COIN), 1000);
                        SaveManager.Instance.AddResource(ItemList.Resource.COIN, 1000);
                    }
                }
                EventManager.Instance.DontSetMiniMapIcon = 15;
                if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam) && SaveManager.Instance.GetMiniFlag(Mini.GameCleared) <= 0)
                {
                    SaveManager.Instance.AutoSave(forced: true);
                }
                EventManager.Instance.AllowAutoMap = true;
                MusicManager.Instance.PlayRoomMusic();
                SaveManager.Instance.AutoSave(forced: true);
                em.mainCharacter._controller.ResetPhy();
                SaveManager.Instance.ReallyRenewLevel();
            }
        }



        [HarmonyPatch(typeof(Chap4CyrilRoom),"EVENT")]
        [HarmonyPrefix]
        static void NoFreeStuffFromCyril()
        {
            EventManager em = EventManager.Instance;
            switch (em.EventStage)
            {
                case 10:
                    em.SetStage(11);
                    break;
                case 11:
                    if(RandomizerPlugin.checkItemGot(ItemList.Type.BADGE_CrystalAbsorberS, 1))
                    {
                        em.SetStage(20);
                        break;
                    }
                    if (em.EventTime > 0.75f && em.EventTime < 100f)
                    {
                        HUDObtainedItem.Instance.GiveItem(ItemList.Type.BADGE_CrystalAbsorberS, 1, doRandomBadge: true);
                        em.EventTime = 100f;
                    }
                    if (em.EventTime >= 100.5f && !HUDObtainedItem.Instance.isDisplaying())
                    {
                        em.NextStage();
                    }
                    break;
            }
        }
    }



}
