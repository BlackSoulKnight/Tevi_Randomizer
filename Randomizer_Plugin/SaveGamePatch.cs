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
        //how is saveslot 0 be handled?


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
            Dictionary<ItemData, ItemData> data = new Dictionary<ItemData, ItemData>();

            string result = "";
            customSaveFileNames(ref result, ref saveslot);
            if (MainVar.instance.BagID == 0) MainVar.instance.BagID = 1;
            if (ES3.FileExists(result))
            {
                ES3File eS3File = new ES3File(result);
                try
                {
                    int[] keyItem = eS3File.Load<int[]>("RandoKeyItem");
                    int[] keySlot = eS3File.Load<int[]>("RandoKeySlot");
                    int[] valItem = eS3File.Load<int[]>("RandoValItem");
                    int[] valSlot = eS3File.Load<int[]>("RandoValSlot");

                    for (int i = 0; i < keyItem.Length; i++)
                    {
                        data.Add(new ItemData(keyItem[i], keySlot[i]), new ItemData(valItem[i], valSlot[i]));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                RandomizerPlugin.__itemData = data;
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

                if (LocationTracker.active)
                {
                    if (eS3File.KeyExists("LocationList"))
                    {
                        LocationTracker.setCollectedLocationList(eS3File.Load<string[]>("LocationList"));
                    }
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
            Dictionary<ItemData, ItemData> s = RandomizerPlugin.__itemData;

            int[] keyItem = new int[s.Count];
            int[] keySlot = new int[s.Count];
            int[] valSlot = new int[s.Count];
            int[] valItem = new int[s.Count];
            for (int i = 0; i < s.Count; i++)
            {
                KeyValuePair<ItemData, ItemData> pair = s.ElementAt(i);
                keyItem[i] = pair.Key.itemID;
                keySlot[i] = pair.Key.slotID;
                valItem[i] = pair.Value.itemID;
                valSlot[i] = pair.Value.slotID;
            }

            eS3File.Save("RandoKeyItem", keyItem);
            eS3File.Save("RandoKeySlot", keySlot);
            eS3File.Save("RandoValItem", valItem);
            eS3File.Save("RandoValSlot", valSlot);
            eS3File.Save("CustomDifficulty", RandomizerPlugin.customDiff);
            eS3File.Save("Seed", RandomizerPlugin.seed);
            eS3File.Save("HintList", HintSystem.hintList);
            eS3File.Save("GoMode", RandomizerPlugin.GoMode);

            if(LocationTracker.active) {
                eS3File.Save("Archipelago_ItemList", LocationTracker.getCollectedLocationList());
            }

            eS3File.Sync();
            Randomizer.saveSpoilerLog($"rando.SpoilerSave{saveslot}.txt", s);
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
