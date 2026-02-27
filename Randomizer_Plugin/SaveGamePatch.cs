using EventMode;
using HarmonyLib;
using Map;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TeviRandomizer.TeviRandomizerSettings;

namespace TeviRandomizer
{
    class SaveGamePatch()
    {
        static void decreaseBackupSaveSlot()
        {
            MainVar.instance._backupsaveslot--;
            if (MainVar.instance._backupsaveslot < SettingManager.Instance.GetBackupSaveSlotStart())
            {
                MainVar.instance._backupsaveslot = 99;
            }
            Debug.Log("[SystemManager] Backup save slot is now (Decreased) : " + MainVar.instance._backupsaveslot);
        }

        [HarmonyPatch(typeof(SaveManager), "LoadGame")]
        [HarmonyPostfix]
        static void loadRandomData(ref SaveManager __instance)
        {

            byte saveslot = MainVar.instance._saveslot;
            Dictionary<string, string> data = new Dictionary<string, string>();

            string result = "";
            customSaveFileNames(ref result, ref saveslot);
            if (MainVar.instance.BagID == 0) MainVar.instance.BagID = 1;
            if (ES3.FileExists(result))
            {
                ES3File eS3File = new ES3File(result);

                if (ArchipelagoInterface.Instance.isConnected)
                {
                    ArchipelagoInterface.Instance.storeData();
                }
                else
                {
                    try
                    {
                        string[] keyLocation = eS3File.Load<string[]>("RandoLocation");
                        string[] valItem = eS3File.Load<string[]>("RandoValItem");

                        for (int i = 0; i < keyLocation.Length; i++)
                        {
                            data.Add(keyLocation[i], valItem[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    RandomizerPlugin.__itemData = data;
                    Dictionary<int,int> transitionData = new Dictionary<int,int>();
                    if (eS3File.KeyExists("transitionDataFrom") && eS3File.KeyExists("transitionDataTo"))
                    {
                        int[] from = eS3File.Load<int[]>("transitionDataFrom");
                        int[] to = eS3File.Load<int[]>("transitionDataTo");
                        for(int i = 0;i < from.Length; i++)
                        {
                            transitionData[from[i]] = to[i];
                        }
                        TeviSettings.transitionData = transitionData;
                    }
                }

                if (eS3File.KeyExists("Seed"))
                {
                    RandomizerPlugin.seed = eS3File.Load<string>("Seed");
                }
                if (eS3File.KeyExists("GoMode"))
                {
                    TeviSettings.GoMode = eS3File.Load<int>("GoMode");
                }
                if (eS3File.KeyExists("GoalType"))
                {
                    TeviSettings.goalType = (GoalType)eS3File.Load<int>("GoalType");
                }
                if (eS3File.KeyExists("SuperBosses"))
                {
                    TeviSettings.customFlags[CustomFlags.SuperBosses] = eS3File.Load<bool>("SuperBosses");
                }
                if (eS3File.KeyExists("HintList"))
                {
                    ChatSystemPatch.hintList = eS3File.Load<(string, string, byte)[]>("HintList");
                }
                if (eS3File.KeyExists("EventReplace"))
                {
                    EnemyPatch.eventReplace = eS3File.Load<short[]>("EventReplace");
                }
                if (eS3File.KeyExists("EnemyReplace"))
                {
                    EnemyPatch.enemyReplace = eS3File.Load<short[]>("EnemyReplace");
                }

                if (eS3File.KeyExists("transitionVisited"))
                {
                    RandomizerPlugin.transitionVisited = eS3File.Load<List<int>>("transitionVisited");
                    if(RandomizerPlugin.transitionVisited == null)
                    {
                        RandomizerPlugin.transitionVisited = new List<int>();
                    }
                }

                if (eS3File.KeyExists("UniqueEnemies"))
                {
                    RandomizerPlugin.UniqueEnemiesKilled = eS3File.Load<List<int>>("UniqueEnemies");
                    if(RandomizerPlugin.UniqueEnemiesKilled == null)
                    {
                        RandomizerPlugin.UniqueEnemiesKilled = new List<int>();
                    }
                }
                if (eS3File.KeyExists("randomizedMusic"))
                {
                    Extras.RandomizeExtra.randomizedMusic = eS3File.Load<byte[]>("randomizedMusic");
                }
                if (eS3File.KeyExists("randomizedBG"))
                {
                    Extras.RandomizeExtra.randomizedBG = eS3File.Load<byte[]>("randomizedBG");
                }
                if (eS3File.KeyExists("LocationList"))
                {
                    LocationTracker.setCollectedLocationList(eS3File.Load<string[]>("LocationList"));
                }
                else
                {
                    LocationTracker.setCollectedLocationList([]);
                }
                if(eS3File.KeyExists("Archipelago_currItem"))
                    ArchipelagoInterface.Instance.currentItemNR = eS3File.Load<int>("Archipelago_currItem");

                ItemDistributionSystem.loadFromSlot(eS3File);
                if ( ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected && eS3File.KeyExists("Archipelago_currItem"))
                {
                    ArchipelagoInterface.Instance.storeData();
                }

            }
        }

        [HarmonyPatch(typeof(SaveManager), "SaveGame")]
        [HarmonyPostfix]
        static void saveRandomData(ref bool backup)
        {

            byte saveslot = MainVar.instance._saveslot;
            if (MainVar.instance.CHAPTERRESET_Event > 0)
            {
                saveslot = 100;
            }
            else if (backup && (bool)WorldManager.Instance)
            {
                decreaseBackupSaveSlot();
                saveslot = MainVar.instance._backupsaveslot;
                SettingManager.Instance.IncreaseBackupSaveSlot();
            }
            else if (MainVar.instance._isAutoSave)
            {
                saveslot = 0;
            }

            string result = "";
            customSaveFileNames(ref result, ref saveslot);
            ES3File eS3File = new ES3File(result);
            Dictionary<string, string> s = RandomizerPlugin.__itemData;
            Dictionary<int, int> transitionData = TeviSettings.transitionData;

            if (s != null)
            {
                string[] keyItem = new string[s.Count];
                string[] valItem = new string[s.Count];
                for (int i = 0; i < s.Count; i++)
                {
                    KeyValuePair<string, string> pair = s.ElementAt(i);
                    keyItem[i] = pair.Key;
                    valItem[i] = pair.Value;
                }
                eS3File.Save("RandoLocation", keyItem);
                eS3File.Save("RandoValItem", valItem);
            }
            if (transitionData != null)
            {
                int[] transitionDataFrom = new int[transitionData.Count];
                int[] transitionDataTo = new int[transitionData.Count];
                for (int i = 0; i < transitionData.Count; i++)
                {
                    KeyValuePair<int, int> pair = transitionData.ElementAt(i);
                    transitionDataFrom[i] = pair.Key;
                    transitionDataTo[i] = pair.Value;
                }
                eS3File.Save("transitionDataFrom", transitionDataFrom);
                eS3File.Save("transitionDataTo", transitionDataTo);
            }
            eS3File.Save("transitionVisited", RandomizerPlugin.transitionVisited);
            eS3File.Save("EventReplace", EnemyPatch.eventReplace);
            eS3File.Save("EnemyReplace", EnemyPatch.enemyReplace);
            eS3File.Save("randomizedMusic", Extras.RandomizeExtra.randomizedMusic);
            eS3File.Save("randomizedBG", Extras.RandomizeExtra.randomizedBG);
            eS3File.Save("SuperBosses", TeviSettings.customFlags[CustomFlags.SuperBosses]);
            eS3File.Save("GoalType", (int)TeviSettings.goalType);
            eS3File.Save("Seed", RandomizerPlugin.seed);
            eS3File.Save("HintList", ChatSystemPatch.hintList);
            eS3File.Save("GoMode", TeviSettings.GoMode);
            eS3File.Save("UniqueEnemies",RandomizerPlugin.UniqueEnemiesKilled);
            if (MainVar.instance._isAutoSave)
            {
                if (ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected)
                {
                    eS3File.Save("Archipelago_currItem", autoSaveAPIndex);
                }
                eS3File.Save("LocationList", autoSaveLocations);
                ItemDistributionSystem.saveToTmpSlot(eS3File);

            }
            else
            {
                if (ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected)
                {
                    eS3File.Save("Archipelago_currItem", ArchipelagoInterface.Instance.currentItemNR);
                }
                eS3File.Save("LocationList", LocationTracker.getCollectedLocationList());
                ItemDistributionSystem.saveToSlot(eS3File);
            }


            eS3File.Sync();
        }

        //very cursed Auto save things
        static int autoSaveAPIndex= 0;
        static string[] autoSaveLocations;
        [HarmonyPatch(typeof(SaveManager), "AutoSave")]
        [HarmonyPrefix]
        static bool saveAPTemp(ref bool ___AutoSaved,ref bool forced)
        {
            if (GemaBossRushMode.Instance.isBossRush() ||
                (WorldManager.Instance.CurrentRoomArea == AreaType.ZENITH || WorldManager.Instance.CurrentRoomArea == AreaType.SEAL || EventManager.Instance.mainCharacter.t.position.x <= 0f || EventManager.Instance.mainCharacter.t.position.y >= 0f)||
                (GemaMissionMode.Instance.isInMission())||
                (EventManager.Instance.getMode() != 0 && !forced)||
                (SaveManager.Instance.GetMiniFlag(Mini.NoMoreAutoSaveInLastMaps) > 0 && (WorldManager.Instance.CurrentRoomArea == AreaType.FINALPALACE || WorldManager.Instance.CurrentRoomArea == AreaType.ILLUSIONPALACE))||
                (Time.timeSinceLevelLoad <= 0.5f)
             
                )
            { return true; 
            }
            

            if (forced || (EventManager.Instance.boss == Boss.OFF && !___AutoSaved))
            {
                autoSaveAPIndex = ArchipelagoInterface.Instance.currentItemNR;
                autoSaveLocations = LocationTracker.getCollectedLocationList();
                ItemDistributionSystem.saveToTmp();

            }
            return true;
        }


        [HarmonyPatch(typeof(SaveManager), "GetSaveFileName")]
        [HarmonyPrefix]
        static bool customSaveFileNames(ref string __result, ref byte saveslot)
        {
            __result = "randomizer/rando.tevisave" + saveslot + ".sav";
            return false;

        }

        [HarmonyPatch(typeof(SaveManager), "RenewStackableCount")]
        [HarmonyPrefix]
        static bool RenewStackableCOunt(ref ItemList.Type item)
        {
            switch (item)
            {
                case ItemList.Type.STACKABLE_MP:
                case ItemList.Type.STACKABLE_HP:
                case ItemList.Type.STACKABLE_EP:
                case ItemList.Type.STACKABLE_RATK:
                case ItemList.Type.STACKABLE_MATK:
                case ItemList.Type.STACKABLE_SHARD:
                    return false;
                default:
                    return true;
            }
        }

        [HarmonyPatch(typeof(SaveManager),"_Awake")]
        [HarmonyPostfix]
        static void increaseSaveDataArray(ref SaveManager.SaveData ___savedata,ref SaveManager __instance)
        {
            ___savedata.todoA = new byte[2000];
            ___savedata.todoX = new byte[2000];
            ___savedata.todoY = new byte[2000];
            ___savedata.todoID = new byte[2000];
            ___savedata.oldtodoID = new byte[2000];
            Traverse.Create(__instance).Field("MAXTODO").SetValue(2000);
            Traverse.Create(__instance).Field("MAXOLDTODO").SetValue(2000);
        }
    }

}
