using System;
using System.IO;
using System.Collections.Generic;


using ItemList;
using UnityEngine;
using static TeviRandomizer.Randomizer;
using UnityEngine.UIElements;

namespace TeviRandomizer
{
    public class Randomizer
    {

        public class Item
        {
            public int Id;
            public string Name;
            public string Type;

            public Item(string _name, string _type)
            {
                this.Name = _name;
                this.Type = _type;
            }
        }
        public class Requirement
        {
            public string Method;
            public int Difficulty;
            public Requirement(string method)
            {
                string[] info = method.Split(',');

                Method = info[0];
                if (info.Length>1)
                    Difficulty = int.Parse(info[1]);
            }
            public bool check(List<string> itemList)
            {
                bool flag = true;
                if (Method == "") return true;
                string[] ors = Method.Split(new string[] { "||" }, StringSplitOptions.None);
                foreach(string or in ors)
                {
                    bool flagCheck = true;
                    string[] ands = Method.Split(new string[] { "&&" }, StringSplitOptions.None);
                    foreach(string item in ands)
                    {
                        if (itemList.Contains(item)||item == "") continue;
                        // some check i choose to ignore
                        if (item.Contains("Coins")) continue;
                        if (item.Contains("Boss"))
                        {
                            bossCount++;
                            continue;
                        }
                        if (item.Contains("Chapter"))
                        {
                            switch(item.Split(' ')[2])
                            {
                                case "1":
                                    if (bossCount >= 1) continue;
                                    break;
                                case "2":
                                    if (bossCount >= 3) continue;
                                    break;
                                case "3":
                                    if (bossCount >= 5) continue;
                                    break;
                                case "4":
                                    if (bossCount >= 7) continue;
                                    break;
                                case "5":
                                    if (bossCount >= 10) continue;
                                    break;
                                case "6":
                                    if (bossCount >= 13) continue;
                                    break;
                                case "7":
                                    if (bossCount >= 16) continue;
                                    break;
                                case "8":
                                    if (bossCount >= 20) continue;
                                    break;
                            }
                        }

                        flag = false;
                        flagCheck = false;
                        break;
                    }
                    flag |= flagCheck;
                }
                return flag;
            }
        }
        public static Randomizer Instance = new Randomizer();
        public class Location
        {


            public string Itemname;
            public string Loaction;
            public int itemId;
            public int slotId;
            public int newItem;
            public int newSlotId;
            public List<Requirement> Requirement;
            public Location(int itemId, string loaction, int slotId,string requirements,string itemname = "")
            {
                Itemname = itemname;
                Loaction = loaction;
                this.itemId = itemId;
                this.slotId = slotId;
                this.Requirement = new List<Requirement>();
                this.newItem = 0;
                this.newSlotId = 0;
                foreach(string method in requirements.Split(';'))
                {
                    Requirement.Add(new Requirement(method));
                }
            }
            public bool isReachAble(List<string> itemList)
            {
                foreach (Requirement req in Requirement)
                {
                    if (req.check(itemList)) return true;
                }
                return false;
            }
            public void setNewItem(ItemList.Type item, int slot)
            {
                newItem = (int)item;
                newSlotId = slot;
            } 
            public void setNewItem(int item, int slot)
            {
                newItem = item;
                newSlotId = slot;
            } 
        }

        public class Area
        {
            public string Name;
            //public List<Money> Money;
            public List<Entrance> Connections;
            public List<Location> Locations;
            public Area(string name) {
                this.Name = name; 
                this.Connections = new List<Entrance>();
                this.Locations = new List<Location>();
            }
            public void addConnection(Area to,string method)
            {
                this.Connections.Add(new Entrance(this, to, method));
            }
        }

        public class Money
        {
            public string Method;
            public int Amount;
        }

        public class Entrance
        {
            Area from;
            public Area to;
            public Requirement method;
            public Entrance(Area from, Area to, string method)
            {
                this.from = from;
                this.to = to;
                this.method = new Requirement(method);
            }
            public bool checkEntrance(List<string> itemList)
            {
                if (to == null) return false;
                return method.check(itemList);
            }
        }

        private List<(int,int)> itemPool;
        private List<Location> locations;
        private Dictionary<string,Location> locationString;
        private List<Area> areas;
        public static int bossCount = 0;
        public Randomizer() {
            string path = BepInEx.Paths.PluginPath+ "/tevi_randomizer/resource/" ;

            itemPool = new List<(int,int)>();
            /*
            foreach (string line in File.ReadLines(path + "Item.txt"))
            {
                string[] para = line.Split(':');
                itemPool.Add(new Item(para[0], para[1]));
            }*/
            areas = new List<Area>();

            foreach (string lin in File.ReadLines(path + "Area.txt"))
            {
                areas.Add(new Area(lin));
            }

            foreach(string line in File.ReadLines(path + "Connection.txt"))
            {
                string[] para = line.Split(':');
                areas.Find(x => x.Name == para[0]).addConnection(areas.Find(x=> x.Name == para[1]), para[2]);
            }
            locations = new List<Location>();
            locationString = new Dictionary<string,Location>();
            foreach (string line in File.ReadLines(path + "Location.txt"))
            {
                string[] para = line.Split(':');
                Location newloc = new Location((int)Enum.Parse<ItemList.Type>(para[0]), para[1], int.Parse(para[2]), para[3], para[0]);
                locationString.Add($"{para[0]+para[2]}", newloc);
                locations.Add(newloc);
                itemPool.Add(((int)Enum.Parse(typeof(ItemList.Type), para[0]), int.Parse(para[2])));
                Area se = areas.Find(x => x.Name == para[1]);
                if(se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Debug.LogWarning($"Could not find {para[1]} to place {para[0]}");
                }
            }
            ExtraOptionLocations();
            
            
        }

        private void ExtraOptionLocations()
        {
            Location loc;
            loc = new Location(4006, "", 0, "", "Extra Range Potion");
            locations.Add(loc);
            locationString.Add("ExtraRangePotion", loc);
            loc = new Location(4005, "", 0, "", "Extra Melee Potion");
            locations.Add(loc);
            locationString.Add("ExtraMeleePotion", loc);
            loc = new Location(4200, "", 0, "", "Lv3Compass");
            locations.Add(loc);
            locationString.Add("Lv3Compass",loc);

        }
        private void CustomOptions(List<(int, int)> placeditems, List<Location> locationPool)
        {
            foreach (var option in settings)
            {
                string[] info = option.Key.Split(' ');
                switch (info[0])
                {
                    case "Toggle":
                        switch (info[1])
                        {
                            case "Knife":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    locationString["ITEM_KNIFE1"].setNewItem(ItemList.Type.ITEM_KNIFE, 4);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.ITEM_KNIFE && x.slotId == 1));
                                    placeditems.Remove(((int)ItemList.Type.ITEM_KNIFE, 1));
                                }
                                break;
                            case "Lv3Compass":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    locationString["Lv3Compass"].setNewItem(4200, 1);
                                }
                                break;
                            default: break;
                        }
                        break;
                    case "Slider":
                        switch (info[1])
                        {
                            case "RangePot":
                                locationString["ExtraRangePotion"].setNewItem(4006, (int)((UnityEngine.UI.Slider)option.Value).value);
                                break;
                            case "MeleePot":
                                locationString["ExtraMeleePotion"].setNewItem(4005, (int)((UnityEngine.UI.Slider)option.Value).value);
                                break;

                            default: break;

                        }
                        break;
                }
            }
        }


        public void createSeed(System.Random seed)
        {
            int debugVal = 0;
            List<(int, int)> placeditems;
            placeditems = new List<(int, int)>();
            List<Location> locationPool = new List<Location>();

            if (seed == null) seed = new System.Random();
            do
            {
                debugVal++;

                locationPool.Clear();
                locationPool.CopyFrom(locations);
                locationPool.RemoveAll(x => x.itemId > 3000);                 // Remove all Extra Options from Pool (they are above 3000)

                Debug.Log($"Seed creating Try: {debugVal}");
                bossCount = 0;
                placeditems.Clear();
                placeditems.CopyFrom(itemPool);

                CustomOptions(placeditems, locationPool);                 //Extra stuff

                foreach (Location loc in locationPool)
                {
                    (int, int) item = createItem(seed,placeditems);
                    locationString[loc.Itemname + loc.slotId.ToString()].newSlotId = item.Item2;
                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = item.Item1;
                }

                bossCount = 0;
                Debug.Log("Validating");
            } while (!validate());

        }

        private (int,int) createItem(System.Random seed,List<(int,int)> pool)
        {
            (int, int) tmp = pool[seed.Next(pool.Count)];
            pool.Remove(tmp);
            if (Enum.IsDefined(typeof(Upgradable),((ItemList.Type)tmp.Item1).ToString()))
            {
                tmp.Item2 += 3;
            }
            if (((ItemList.Type)tmp.Item1).ToString().Contains("STACKABLE_SHARD"))
            {
                tmp.Item2 += 10;
            }
            return tmp;
        }

        List<string> itemList;
        private bool validate()
        {
            List<Area> areaList = [areas.Find(x=> x.Name=="Start Area")];
            areaList.Add(areas[3]);
            itemList = new List<string>();
            List<(int,int)> tmpList = new List<(int,int)> ();
            
            int lastCount = -1;
            while (itemList.Count != lastCount)
            {
                lastCount = itemList.Count;
                Area[] tmp = new Area[areaList.Count];
                areaList.CopyTo(tmp);
                foreach (Area area in tmp)
                {
                    foreach (Entrance en in area.Connections)
                    {
                        if (!areaList.Contains(en.to)&& en.checkEntrance(itemList))
                        {
                            areaList.Add(en.to);
                        }
                    }
                    foreach (Location loc in area.Locations)
                    {
                        if (tmpList.Contains((loc.newItem, loc.newSlotId)))
                            continue;
                        if (loc.isReachAble(itemList))
                        {
                            if (!tmpList.Contains((loc.newItem, loc.newSlotId)))
                                tmpList.Add((loc.newItem, loc.newSlotId));

                            string item = ((ItemList.Type)loc.newItem).ToString();
                            if (Enum.IsDefined(typeof(Upgradable),item))
                            {

                                if (itemList.Contains(item))
                                {
                                    if (itemList.Contains(item+ "2"))
                                    {
                                        itemList.Add(item + "3");
                                    }
                                    else
                                    {
                                        itemList.Add(item + "2");
                                    }
                                }
                                else
                                {
                                    itemList.Add(item);
                                }
                            }
                            else
                            {
                                itemList.Add(item);
                            }
                        }
                    }
                   
                }


            }
            //Check if its beatable
            return goalCheck(itemList);
        }
        private bool goalCheck(List<string> itemList)
        {
            int gears = itemList.FindAll(x => x == "STACKABLE_COG").Count;
            if ( gears<= 16)
            {
                Debug.Log($"Not Enough Gears in the run. Found {gears}");
                return false;
            }
            if (!itemList.Contains("ITEM_SLIDE")) {
                return false;
                }
            if(!itemList.Contains("ITEM_LINEBOMB")) return false;
            if(!itemList.Contains("ITEM_AirSlide")) return false;
            if(!itemList.Contains("ITEM_Rotater")) return false;

            return true;
        }

        public Dictionary<ItemData,ItemData> GetData() {
            Dictionary<ItemData,ItemData> data = new Dictionary<ItemData, ItemData>(new ItemData.EqualityComparer());
            foreach(Location loc in locations)
            {


                
                ItemData item1 = new ItemData(loc.itemId, loc.slotId);
                ItemData item2 = new ItemData(loc.newItem, loc.newSlotId);
                try
                {
                    data.Add(item1, item2);
                }
                catch
                {
                    Debug.LogWarning($"Already changed {item1.getItemTyp()} slot {item2.getSlotId()}");

                }
            }
            

            return data;
        }



        static public Dictionary<ItemData,ItemData> loadRandomizedItemsFromFile(string path)
        {
            Dictionary<ItemData, ItemData> data = new Dictionary<ItemData, ItemData>(new ItemData.EqualityComparer());

            if (!Directory.Exists(RandomizerPlugin.pluginPath + "Data")) return data;
            Dictionary<ItemData, ItemData> tmp = new Dictionary<ItemData, ItemData>();
            try
            {

                string json = File.ReadAllText(RandomizerPlugin.pluginPath + "Data/" + path);
                string[] blocks = json.Split(';');
                foreach (string block in blocks)
                {
                    ItemData data1, data2;
                    if (block.Length < 5) continue;
                    try
                    {
                        string[] completeItem = block.Split(':');

                        string[] itemDetails1 = completeItem[0].Split(',');
                        string[] itemDetails2 = completeItem[1].Split(',');
                        data1 = new ItemData(int.Parse(itemDetails1[0]), int.Parse(itemDetails1[1]));
                        data2 = new ItemData(int.Parse(itemDetails2[0]), int.Parse(itemDetails2[1]));
                    }
                    catch
                    {
                        Debug.LogError($"Failed to parse {block}");
                        continue;
                    }
                    try
                    {
                        data.Add(data1, data2);
                    }
                    catch
                    {
                        Debug.LogWarning($"Already changed {data1.getItemTyp()} slot {data1.getSlotId()}");

                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return data;
            }
            return data;
        }
        static public Dictionary<string, object> settings = new Dictionary<string, object>();



        static public void saveRandomizedItemsToFile(string path,Dictionary<ItemData,ItemData> itemData)
        {
            if (!Directory.Exists(RandomizerPlugin.pluginPath + "Data")) Directory.CreateDirectory(RandomizerPlugin.pluginPath + "Data");
            StreamWriter saving = File.CreateText(RandomizerPlugin.pluginPath + "Data/" + path);
            StreamWriter spoilerLog = File.CreateText(RandomizerPlugin.pluginPath + "Data/" + path+".spoiler.txt");
            foreach (KeyValuePair<ItemData, ItemData> item in itemData)
            {
                saving.Write($"{item.Key.itemID},{item.Key.slotID}:{item.Value.itemID},{item.Value.slotID};\n");
                string line = "";
                line = $"Randomized Item: {item.Key.itemID} Slot: {item.Key.slotID}";
                for (int x = 0; x < 70 - line.Length; x++) {
                        line += " ";
                    }
                line += $"Original Item: {item.Value.itemID} Slot: {item.Value.slotID}";
                for(int x = 0;x< 140 - line.Length; x++)
                {
                    line += " ";
                }
                Location loc = Instance.locations.Find(x => x.Itemname == item.Key.getItemTyp().ToString() && x.slotId == item.Key.slotID);
                if (loc != null) { line += $"Location: {loc.Loaction}\n"; }
                else line += "\n" ;
                if (item.Key.itemID < 3000)
                    spoilerLog.Write(line);
            }
            spoilerLog .Close();
            saving.Close();
        }
    }
}



/*
 * Reformat Json, this c# without a good conversion looks horrible
 * 
 * Saving Randomized File to specific saveslots
 * Returning to main Menu deloads randmoized data
 * Loading saveslots checks if a saved Randomizer file exists
 * New Game with Randomize option is only temp
 * Need to check what to do with Auto Saves checksum / rando seed?
 */