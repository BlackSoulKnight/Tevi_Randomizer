using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
                }

                if (eS3File.KeyExists("CustomDifficulty"))
                {
                    RandomizerPlugin.customDiff = eS3File.Load<int>("CustomDifficulty");
                }
                if (eS3File.KeyExists("Seed"))
                {
                    RandomizerPlugin.seed = eS3File.Load<string>("Seed");
                }
                if (eS3File.KeyExists("GoMode"))
                {
                    RandomizerPlugin.GoMode = eS3File.Load<int>("GoMode");
                }
                if (eS3File.KeyExists("HintList"))
                {
                    HintSystem.hintList = eS3File.Load<(string, string, byte)[]>("HintList");
                }
                if (eS3File.KeyExists("LocationList"))
                {
                    LocationTracker.setCollectedLocationList(eS3File.Load<string[]>("LocationList"));
                }
                else
                {
                    LocationTracker.setCollectedLocationList([]);
                }
                if ( ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected && eS3File.KeyExists("Archipelago_currItem"))
                {
                    ArchipelagoInterface.Instance.currentItemNR = eS3File.Load<int>("Archipelago_currItem");
                    ArchipelagoInterface.Instance.refreshRecievedItems();
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
            eS3File.Save("CustomDifficulty", RandomizerPlugin.customDiff);
            eS3File.Save("Seed", RandomizerPlugin.seed);
            eS3File.Save("HintList", HintSystem.hintList);
            eS3File.Save("GoMode", RandomizerPlugin.GoMode);
            eS3File.Save("LocationList", LocationTracker.getCollectedLocationList());
          

            if (ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected)
            {
                eS3File.Save("Archipelago_currItem", ArchipelagoInterface.Instance.currentItemNR);
            }

            eS3File.Sync();
            //Randomizer.saveSpoilerLog($"rando.SpoilerSave{saveslot}.txt", s);
        }

        [HarmonyPatch(typeof(SaveManager), "GetSaveFileName")]
        [HarmonyPrefix]
        static bool customSaveFileNames(ref string __result, ref byte saveslot)
        {
            __result = "randomizer/rando.tevisave" + saveslot + ".sav";
            return false;

        }

    }

}
