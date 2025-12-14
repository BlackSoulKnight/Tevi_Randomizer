
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeviRandomizer
{
    class LocationTracker
    {
        public static bool active = true;
        static List<string> collectedLocationList = new List<string>();
        static public Dictionary<string,string> APLocationName = loadLocationNameList();
        static public Dictionary<string,string> APResoucreLocationame = loadUpgradeResourceLocationList();

        static private Dictionary<string, string> loadLocationNameList() {
            string path = RandomizerPlugin.pluginPath + "/resource/";
            Dictionary<string,string> dict = new Dictionary<string,string>();
            JArray locations = JArray.Parse(File.ReadAllText(path+ "Location.json"));
            foreach(var loc in locations)
            {
                dict.Add($"{loc["Itemname"]} #{loc["slotId"]}", loc["LocationName"].ToString());
            }
            return dict;
        }

        static private Dictionary<string, string> loadUpgradeResourceLocationList()
        {
            Dictionary<string,string> keyValuePairs = new Dictionary<string,string>();
            string path = RandomizerPlugin.pluginPath + "/resource/";
            JArray locations = JArray.Parse(File.ReadAllText(path + "UpgradeResourceLocation.json"));
            foreach(var loc in locations)
            {
                keyValuePairs.Add($"{loc["area"]} #{(int)loc["blockId"]}", loc["LocationName"].ToString());
            }
            return keyValuePairs;
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
        public static bool hasResource(byte area,int blockId)
        {
            if (APResoucreLocationame.ContainsKey($"{area} #{blockId}"))
                return collectedLocationList.Contains(APResoucreLocationame[$"{area} #{blockId}"]);
            else
                return collectedLocationList.Contains($"{area} #{blockId}");
        }


        public static bool checkLocation(string location)
        {
            return collectedLocationList.Contains(location);
        }

        public static void clearItemList() => collectedLocationList.Clear();
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            if (APLocationName.ContainsKey($"{item} #{slot}"))
                addLocationToList(APLocationName[$"{item} #{slot}"]);
            else
            {
                addLocationToList($"{item} #{slot}");
                return;
            }
        }
        public static void addResourceToList(byte area, int blockId)
        {
            if (APResoucreLocationame.ContainsKey($"{area} #{blockId}"))
                addLocationToList(APResoucreLocationame[$"{area} #{blockId}"]);
            else
            {
                addLocationToList($"{area} #{blockId}");
                return;
            }
        }
        

        public static void addLocationToList(string location)
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