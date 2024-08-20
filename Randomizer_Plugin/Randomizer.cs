using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using TMPro;
using System.Text.RegularExpressions;
using System.Data;


namespace TeviRandomizer
{
    public class Randomizer
    {
        public static Randomizer Instance = new Randomizer();

        private class Item
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

        static public bool craftingManaShardSwitch = false;
        private class Requirement
        {
            public string Method;
            public int Difficulty;
            public Requirement(string method)
            {
                string[] info = method.Split(',');

                Method = info[0];
                if (info.Length > 1)
                    Difficulty = int.Parse(info[1]);
            }
            public bool check(List<string> itemList)
            {
                if (Method == "") return true;
                Debug.Log(Method);
                Method = Regex.Replace(Method, @"\b(\w|\s)+\b", match => {
                    Debug.Log(match);
                    if (itemList.Contains(match.ToString()))
                    return " true ";
                    else
                        return " false ";
                });
                Method = Method.Replace("&&", "AND").Replace("||", "OR");
                Debug.Log(Method);

                return (bool)new DataTable().Compute(Method,"");
            }


            public bool checkMovementItems(List<string> itemList)
            {

                if (
                    itemList.Contains("ITEM_DOUBLEJUMP") &&
                    itemList.Contains("ITEM_AirDash") &&
                    itemList.Contains("ITEM_WALLJUMP") &&
                    itemList.Contains("ITEM_JETPACK") &&
                    itemList.Contains("ITEM_SLIDE") &&
                    itemList.Contains("ITEM_HIJUMP")&&
                    itemList.Contains("ITEM_WATERMOVEMENT")
                    )
                    return true;

                return false;
            }
        }
        private class Location
        {


            public string Itemname;
            public string Loaction;
            public int itemId;
            public int slotId;
            public int newItem;
            public int newSlotId;
            public List<Requirement> Requirement;
            public Location(int itemId, string loaction, int slotId, string requirements, string itemname = "")
            {
                Itemname = itemname;
                Loaction = loaction;
                this.itemId = itemId;
                this.slotId = slotId;
                this.Requirement = new List<Requirement>();
                this.newItem = 0;
                this.newSlotId = 0;
                foreach (string method in requirements.Split(';'))
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
            public bool debugIsReachAble(List<string> itemList)
            {
                foreach (Requirement req in Requirement)
                {
                    if (!req.check(itemList)) { Debug.Log($"Item {((ItemList.Type)newItem).ToString()} {newSlotId} could not be reached. {req.Method}");
                        return false;
                    }
                }
                return true;
            }
            public bool checkSelfContain(ItemList.Type item)
            {
                foreach(Requirement req in Requirement)
                {
                    if (req.Method.Contains(item.ToString())) return true;
                    if(item == ItemList.Type.I19 && req.Method.Contains("ITEM_OrbTypeC")) return true;
                    if(item == ItemList.Type.I20 && req.Method.Contains("ITEM_OrbTypeS")) return true;  
                }
                return false;
            }
        }

        private class Area
        {
            public string Name;
            //public List<Money> Money;
            public List<Entrance> Connections;
            public List<Location> Locations;
            public Area(string name)
            {
                this.Name = name;
                this.Connections = new List<Entrance>();
                this.Locations = new List<Location>();
            }
            public void addConnection(Area to, string method)
            {
                this.Connections.Add(new Entrance(this, to, method));
            }
        }

        private class Money
        {
            public string Method;
            public int Amount;
        }

        private class Entrance
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
            public void  debugCheckEntrance(List<string> itemList)
            {
                if (to == null) return;
                if (!method.check(itemList)) Debug.Log($"Could not enter {to.Name}. {method.Method}");
            }


        }

        private List<(int, int)> itemPool;
        private List<Location> locations;
        private Dictionary<string, Location> locationString;
        private List<Area> areas;
        private static int bossCount = 0;
        private static int memineCount = 0;


        public Randomizer()
        {
            string path = BepInEx.Paths.PluginPath + "/tevi_randomizer/resource/";

            itemPool = new List<(int, int)>();
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

            foreach (string line in File.ReadLines(path + "Connection.txt"))
            {
                string[] para = line.Split(':');
                areas.Find(x => x.Name == para[0]).addConnection(areas.Find(x => x.Name == para[1]), para[2]);
            }
            locations = new List<Location>();
            locationString = new Dictionary<string, Location>();
            foreach (string line in File.ReadLines(path + "Location.txt"))
            {
                string[] para = line.Split(':');
                Location newloc = new Location((int)Enum.Parse(typeof(ItemList.Type), para[0]), para[1], int.Parse(para[2]), para[3], para[0]);
                locationString.Add($"{para[0] + para[2]}", newloc);
                locations.Add(newloc);
                itemPool.Add(((int)Enum.Parse(typeof(ItemList.Type), para[0]), int.Parse(para[2])));
                Area se = areas.Find(x => x.Name == para[1]);
                if (se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Debug.LogWarning($"Could not find {para[1]} to place {para[0]}");
                }
            }
            //ExtraOptionLocations();


        }

        public string findLocationName(int item,int slot)
        {
            string val = "";
            val = locations.Find(x=> x.itemId == item && x.slotId == slot).Loaction;
            return val;
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
            locationString.Add("Lv3Compass", loc);

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
                            case "Orb":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    locationString["ITEM_ORB1"].setNewItem(ItemList.Type.ITEM_ORB, 4);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.ITEM_ORB && x.slotId == 1));
                                    placeditems.Remove(((int)ItemList.Type.ITEM_ORB, 1));
                                }

                                break;
                            case "Lv3Compass":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.CompassStart] = true;
                                }
                                else
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.CompassStart] = false;
                                }
                                break;
                            case "tmpOption":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.TempOption] = true;
                                }
                                else
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.TempOption] = false;
                                }
                                break;
                            case "NormalItemCraft":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    craftingManaShardSwitch = true;
                                    foreach (var item in Enum.GetValues(typeof(Upgradable)))
                                    {
                                        ItemList.Type t = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item.ToString());
                                        locationString[t + "2"].setNewItem(t, 5);
                                        locationString[t + "3"].setNewItem(t, 6);
                                        locationPool.Remove(locationPool.Find(x => x.itemId == (int)t && x.slotId == 2));
                                        locationPool.Remove(locationPool.Find(x => x.itemId == (int)t && x.slotId == 3));
                                        placeditems.Remove(((int)t, 2));
                                        placeditems.Remove(((int)t, 3));
                                    }
                                }
                                else craftingManaShardSwitch = false;
                                break;
                            case "Ceble":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart] = true;
                                }
                                else
                                {
                                    RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart] = false;
                                }
                                break;
                            default: break;
                        }
                        break;
                    case "Slider":
                        switch (info[1])
                        {
                            case "RangePot":
                                RandomizerPlugin.extraPotions[0] = (int)((UnityEngine.UI.Slider)option.Value).value;
                                //locationString["ExtraRangePotion"].setNewItem(4006, (int)((UnityEngine.UI.Slider)option.Value).value);
                                break;
                            case "MeleePot":
                                RandomizerPlugin.extraPotions[1] = (int)((UnityEngine.UI.Slider)option.Value).value;
                                //locationString["ExtraMeleePotion"].setNewItem(4005, (int)((UnityEngine.UI.Slider)option.Value).value);
                                break;

                            default: break;

                        }
                        break;
                }
            }
        }

        static bool creating = false;
        public async void createSeed(System.Random seed)
        {
            if (creating) { return; }
            creating = true;
            seedCreationLoading();
            await Task.Run(() =>
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

                    bossCount = 0;
                    memineCount = 0;
                    placeditems.Clear();
                    placeditems.CopyFrom(itemPool);

                    CustomOptions(placeditems, locationPool);                 //Extra stuff

                    while (locationPool.Count > 0)
                    {
                        int pos = seed.Next(locationPool.Count);
                        Location loc = locationPool[pos];
                        (int, int) item = createItem(seed, placeditems);
                        locationString[loc.Itemname + loc.slotId.ToString()].newSlotId = item.Item2;
                        locationString[loc.Itemname + loc.slotId.ToString()].newItem = item.Item1;
                        locationPool.Remove(loc);
                    }


                    bossCount = 0;
                } while (!validate());
                creating = false;
                RandomizerPlugin.__itemData = GetData();
            });
        }

        public void synccreateSeed(System.Random seed)
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

                bossCount = 0;
                memineCount = 0;
                placeditems.Clear();
                placeditems.CopyFrom(itemPool);

                CustomOptions(placeditems, locationPool);                 //Extra stuff

                while (locationPool.Count > 0)
                {
                    int pos = seed.Next(locationPool.Count);
                    Location loc = locationPool[pos];
                    (int, int) item = createItem(seed, placeditems);
                    locationString[loc.Itemname + loc.slotId.ToString()].newSlotId = item.Item2;
                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = item.Item1;
                    locationPool.Remove(loc);
                }


                bossCount = 0;
            } while (!validate());
            Debug.Log($"[Randomizer] It took {debugVal} tries to create this Seed.");
            creating = false;
            RandomizerPlugin.__itemData = GetData();
            //saveSpoilerLog("DebugSpoilerLog.txt", GetData(), true);
        }

        public async void seedCreationLoading()
        {
            TextMeshProUGUI text = UI.finishedText.GetComponent<TextMeshProUGUI>();
            string t = "Creating ";
            text.text = "Creating";
            UI.finishedText.SetActive(true);
            await Task.Run(() =>
            {
                int count = 0;
                while (creating)
                {
                    count++;
                    text.text = t + new string('.', count % 4);
                    Task.Delay(300).Wait();
                }
                text.text = "Finished Creating Seed";
            });
        }

        private (int, int) createItem(System.Random seed, List<(int, int)> pool)
        {
            (int, int) tmp = pool[seed.Next(pool.Count)];
            pool.Remove(tmp);
            if (Enum.IsDefined(typeof(Upgradable), ((ItemList.Type)tmp.Item1).ToString()))
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
        List<Area> areaList;
        private bool validate()
        {
            areaList = [areas.Find(x => x.Name == "Thanatara Canyon")];
            if(areaList.Count == 0)
            {
                Debug.LogError("[Randomizer] No Entries in areaList!!!!");
            }
            itemList = new List<string>();
            List<(int, int)> tmpList = new List<(int, int)>();

            List<Entrance> entrances = new List<Entrance>();
            List<Location> locations = new List<Location>();
            entrances.AddRange(areaList[0].Connections);
            locations.AddRange(areaList[0].Locations);

            int lastCount = -1;
            while (itemList.Count != lastCount)
            {
                lastCount = itemList.Count;

                foreach (Entrance en in entrances.ToArray())
                {
                    if (areaList.Contains(en.to)) {
                        entrances.Remove(en); continue;
                    }

                    if (!areaList.Contains(en.to) && en.checkEntrance(itemList))
                    {
                        areaList.Add(en.to);
                        foreach (Entrance e in en.to.Connections)
                        {
                            if (!areaList.Contains(e.to)) entrances.Add(e);
                        }
                        locations.AddRange(en.to.Locations);
                        entrances.Remove(en);
                    }
                }

                foreach (Location loc in locations.ToArray())
                {
                    if (tmpList.Contains((loc.newItem, loc.newSlotId)))
                    {
                        locations.Remove(loc);
                        continue;
                    }

                    if (loc.isReachAble(itemList))
                    {
                        if (loc.Loaction == "Lab Part 2" && !itemList.Contains("Demonfray")) itemList.Add("Demonfray"); // Hard coded boss event
                        if (loc.Loaction == "Gallery of Mirrors 2" && itemList.Contains("ITEM_BombLengthExtend") && !itemList.Contains("Memloch")) itemList.Add("Memloch"); // Hard coded boss event
                        if (loc.Loaction == "Snow City" && !itemList.Contains("Air")) itemList.Add("Air");



                        tmpList.Add((loc.newItem, loc.newSlotId));
                        string item = ((ItemList.Type)loc.newItem).ToString();
                        if (Enum.IsDefined(typeof(Upgradable), item))
                        {

                            if (itemList.Contains(item))
                            {
                                if (itemList.Contains(item + "2"))
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
                        locations.Remove(loc);
                    }
                }
            }
            //Check if its beatable
            Debug.LogWarning("CheckIn");
            if (itemList.FindAll(x => x == "STACKABLE_COG").Count > 11)
            {
                if(areaList.Count != areas.Count)
                {
                    foreach(Area ar in areaList)
                    {
                        bool f = false;
                        foreach (Area a in areas)
                        {
                            if(a.Name == ar.Name) { f= true; break; }
                        }
                        if (!f) { Debug.LogError($"{ar.Name} Not IN LIST"); }
                    }
                    
                }
                bool flag = false;
                foreach (Location loc in locations)
                {
                    flag |= !loc.debugIsReachAble(itemList);
                    
                }
                if (flag)
                    return false;

                foreach (Entrance entrance in entrances)
                {
                    entrance.debugCheckEntrance(itemList);
                }

            }
            return goalCheck(itemList, areaList);

        }
        private bool goalCheck(List<string> itemList, List<Area> areaList)
        {
            int gears = itemList.FindAll(x => x == "STACKABLE_COG").Count;
            if (gears < 16)
            {
                Debug.Log($"Not Enough Gears in the run. Found {gears}");
                return false;
            }
            if (!itemList.Contains("ITEM_SLIDE"))
            {
                Debug.Log($"Slide Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_LINEBOMB"))
            {
                Debug.Log($"LineBomb Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_DOUBLEJUMP"))
            {
                Debug.Log($"Double Jump Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_JETPACK"))
            {
                Debug.Log($"Jet Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_WALLJUMP"))
            {
                Debug.Log($"Walljump Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_AirDash"))
            {
                Debug.Log($"Air Dash Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_AirSlide"))
            {
                Debug.Log($"Fairy Powder Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_Rotater"))
            {
                Debug.Log($"Vortex Glove Not Found");

                return false;
            }
            if (!itemList.Contains("ITEM_HIJUMP"))
            {
                Debug.Log($"Highjump Glove Not Found");

                return false;
            }
            return true;
        }

        public Dictionary<ItemData, ItemData> GetData()
        {
            Dictionary<ItemData, ItemData> data = new Dictionary<ItemData, ItemData>();
            foreach (Location loc in locations)
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


        static public Dictionary<string, object> settings = new Dictionary<string, object>();



        static public void saveSpoilerLog(string FileName, Dictionary<ItemData, ItemData> itemData, bool debugWrite = false)
        {
            if (!Directory.Exists(RandomizerPlugin.pluginPath + "Data")) Directory.CreateDirectory(RandomizerPlugin.pluginPath + "Data");

            StreamWriter spoilerLog = File.CreateText(RandomizerPlugin.pluginPath + "Data/" + FileName);;
            foreach (KeyValuePair<ItemData, ItemData> item in itemData)
            {
                string itemName = "";
                string line = "";
                string line2 = "";
                if (!debugWrite)
                {
                    if (item.Value.itemID < 3000) itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString((ItemList.Type)item.Value.itemID), false);
                    else itemName = ((ItemList.Type)item.Value.itemID).ToString();
                    line = $"Randomized Item: {itemName}"; // Slot: {item.Key.slotID}
                }
                line2 = $"Randomized Item: {item.Value.getItemTyp()} {item.Value.slotID}"; // Slot: 
                for (int x = line.Length; x < 70; x++)
                    line += " ";
                for (int x = line2.Length; x < 70; x++)
                    line2 += " ";
                if (!debugWrite)
                {
                    if (item.Key.itemID < 3000) itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString((ItemList.Type)item.Key.itemID), false);
                    else itemName = ((ItemList.Type)item.Key.itemID).ToString();
                    line += $"Original Item: {itemName}"; // Slot: {item.Value.slotID}
                }
                line2 += $"Original Item: {item.Key.getItemTyp()} {item.Key.slotID}"; // Slot: {item.Value.slotID}
                for (int x = line.Length; x < 140; x++)
                    line += " ";

                for (int x = line2.Length; x < 140; x++)
                    line2 += " ";
                Location loc = Instance.locations.Find(x => x.Itemname == item.Key.getItemTyp().ToString() && x.slotId == item.Key.slotID);
                if (loc != null) { line += $"Location: {loc.Loaction}\n"; }
                else line += "\n";
                if (loc != null) { line2 += $"Location: {loc.Loaction}\n"; }
                else line2 += "\n";
                if (item.Key.itemID < 3000)
                {

                    if (debugWrite)
                    {
                        spoilerLog.Write(line2);
                    }
                    else
                    {
                        spoilerLog.Write(line);
                    }
                }

            }
            spoilerLog.Close();

        }

    }
}




/*
 * Returning to main Menu deloads randmoized data
 */