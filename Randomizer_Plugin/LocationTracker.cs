
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
                string[] para = line.Split(':');
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
            return collectedLocationList.Contains(APLocationName[$"{item} #{slot}"]);
        }
        public static void clearItemList() => collectedLocationList.Clear();
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            collectedLocationList.Add(APLocationName[$"{item} #{slot}"]);
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.checkoutLocation(APLocationName[$"{item} #{slot}"]);
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

        
    }
}