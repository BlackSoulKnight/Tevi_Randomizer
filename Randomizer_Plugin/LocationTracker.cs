
using Newtonsoft.Json.Linq;
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
        public static bool checkLocation(string location)
        {
            return collectedLocationList.Contains(location);
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