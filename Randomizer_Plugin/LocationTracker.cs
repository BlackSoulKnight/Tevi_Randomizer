
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ES3;
using static UnityEngine.UIElements.UIR.Allocator2D;

namespace TeviRandomizer
{
    class LocationTracker
    {
        public static bool active = true;
        static List<string> collectedLocationList = new List<string>();
        static public Dictionary<string,string> APLocationName = loadLocationNameList();

        static private Dictionary<string, string> loadLocationNameList() {
            string path = RandomizerPlugin.pluginPath + "/resource/";
            Dictionary<string,string> dict = new Dictionary<string,string>();
            foreach (string line in File.ReadLines(path + "Location.txt"))
            {
                string[] para = line.Split('@');
                dict.Add($"{para[0]} #{para[2]}", para[4]);
            }
            return dict;
        }

        public static void setCollectedLocationList(string[] list)
        {
            collectedLocationList.Clear();
            collectedLocationList.AddRange(list);
        }
        public static string[] getCollectedLocationList() { 

            return collectedLocationList.ToArray(); 
        }
        public static bool hasItem(ItemList.Type item,byte slot)
        {
            if (APLocationName.ContainsKey($"{item} #{slot}"))
                return collectedLocationList.Contains(APLocationName[$"{item} #{slot}"]);
            else
                return collectedLocationList.Contains($"{item} #{slot}");
        }
        public static void clearItemList() => collectedLocationList.Clear();
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            if (APLocationName.ContainsKey($"{item} #{slot}"))
                addItemToList(APLocationName[$"{item} #{slot}"]);
            else
            {
                addItemToList($"{item} #{slot}");
                return;
            }
        }
        public static void addItemToList(string location)
        {
            collectedLocationList.Add(location);
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.checkoutLocation(location);
            }
        }
        public static int getListLenght()
        {
            return collectedLocationList.Count();
        }

        public static void syncArchipelagoLocation()
        {
            foreach (var item in collectedLocationList)
            {
                ArchipelagoInterface.Instance.checkoutLocation(item);
            }
            ArchipelagoInterface.Instance.isSynced = true;
        }
        
    }
}