
using Newtonsoft.Json.Linq;
using Spine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeviRandomizer.TeviRandomizerSettings;
using UnityEngine;

namespace TeviRandomizer
{
    record LocationPosition
    {
        public byte Area,X,Y;
        public LocationPosition(byte area, byte x, byte y)
        {
            Area = area;
            X = x;
            Y = y;
        }
    }
    class LocationTracker
    {
        public static bool active = true;
        static List<string> collectedLocationList = new List<string>();
        static public Dictionary<string, LocationPosition> LocationMapPositions = new();
        static public Dictionary<string,string> APLocationName = loadLocationNameList();
        static public Dictionary<string,string> APResoucreLocationame = loadUpgradeResourceLocationList();

        static private Dictionary<string, string> loadLocationNameList() {
            string path = TeviSettings.pluginPath + "/resource/";
            Dictionary<string,string> dict = new Dictionary<string,string>();
            JArray locations = JArray.Parse(File.ReadAllText(path+ "Location.json"));
            foreach(var loc in locations)
            {
                dict.Add($"{loc["Itemname"]} #{loc["slotId"]}", loc["LocationName"].ToString());
                JObject p = (JObject)(loc["LocationRegion"][0]);
                try
                {
                    LocationMapPositions.Add(loc["LocationName"].ToString(), new((byte)p["Area"], (byte)p["X"], (byte)p["Y"]));
                }
                catch { }
            }
            return dict;
        }

        static private Dictionary<string, string> loadUpgradeResourceLocationList()
        {
            Dictionary<string,string> keyValuePairs = new Dictionary<string,string>();
            string path = TeviSettings.pluginPath + "/resource/";
            JArray locations = JArray.Parse(File.ReadAllText(path + "UpgradeResourceLocation.json"));
            foreach (var loc in locations)
            {
                keyValuePairs.Add($"{loc["area"]} #{(int)loc["blockId"]}", loc["LocationName"].ToString());
            }
            locations = JArray.Parse(File.ReadAllText(path + "MoneyLocations.json"));
            foreach (var loc in locations)
            {
                if (keyValuePairs.ContainsKey($"{loc["area"]} #{(int)loc["blockId"]}"))
                    Debug.LogError($"KEY ALREADY EXISTS: {$"{loc["area"]} #{(int)loc["blockId"]}"}");
                else
                    keyValuePairs.Add($"{loc["area"]} #{(int)loc["blockId"]}", loc["LocationName"].ToString());

                JObject p = (JObject)(loc["LocationRegion"][0]);
                try
                {
                    LocationMapPositions.Add(loc["LocationName"].ToString(), new((byte)p["Area"], (byte)p["X"], (byte)p["Y"]));
                }
                catch { }
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
        public static string getResourceLocationName(byte area,int blockId)
        {
            if (APResoucreLocationame.ContainsKey($"{area} #{blockId}"))
                return APResoucreLocationame[$"{area} #{blockId}"];
            else
                return $"{area} #{blockId}";
        }

        public static bool checkLocation(string location)
        {
            return collectedLocationList.Contains(location);
        }

        public static void clearItemList() => collectedLocationList.Clear();
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            if (APLocationName.ContainsKey($"{item} #{slot}"))
            {
                addLocationToList(APLocationName[$"{item} #{slot}"]);
            }
            else
            {
                addLocationToList($"{item} #{slot}");
            }
        }
        public static void addResourceToList(byte area, int blockId)
        {
            if (APResoucreLocationame.ContainsKey($"{area} #{blockId}"))
            {
                addLocationToList(APResoucreLocationame[$"{area} #{blockId}"]);
            }
            else
            {
                addLocationToList($"{area} #{blockId}");
            }
        }
        

        public static void addLocationToList(string location)
        {
            collectedLocationList.Add(location);
            HintSystemPatch.RemoveCustomTodo(location);
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