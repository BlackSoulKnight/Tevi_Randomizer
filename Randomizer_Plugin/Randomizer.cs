using Newtonsoft.Json.Linq;
using Steamworks.Data;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemVar;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace TeviRandomizer
{
    public class Randomizer
    {

        public enum Progression_Items : int
        {
            I13 = RandomizerPlugin.PortalItem,
            I19 = ItemList.Type.I19,
            I20 = ItemList.Type.I20,
            ITEM_KNIFE = ItemList.Type.ITEM_KNIFE,
            ITEM_ORB = ItemList.Type.ITEM_ORB,
            ITEM_LINEBOMB = ItemList.Type.ITEM_LINEBOMB,
            ITEM_AREABOMB = ItemList.Type.ITEM_AREABOMB,
            ITEM_BOMBFUEL = ItemList.Type.ITEM_BOMBFUEL,
            ITEM_HIJUMP = ItemList.Type.ITEM_HIJUMP,
            ITEM_SPEEDUP = ItemList.Type.ITEM_SPEEDUP,
            ITEM_SLIDE = ItemList.Type.ITEM_SLIDE,
            ITEM_WALLJUMP = ItemList.Type.ITEM_WALLJUMP,
            ITEM_DOUBLEJUMP = ItemList.Type.ITEM_DOUBLEJUMP,
            ITEM_JETPACK = ItemList.Type.ITEM_JETPACK,
            ITEM_WATERMOVEMENT = ItemList.Type.ITEM_WATERMOVEMENT,
            ITEM_MASK = ItemList.Type.ITEM_MASK,
            ITEM_OrbTypeS2 = ItemList.Type.ITEM_OrbTypeS2,
            ITEM_OrbTypeS3 = ItemList.Type.ITEM_OrbTypeS3,
            ITEM_OrbTypeC2 = ItemList.Type.ITEM_OrbTypeC2,
            ITEM_OrbTypeC3 = ItemList.Type.ITEM_OrbTypeC3,
            ITEM_AntiDecay = ItemList.Type.ITEM_AntiDecay,
            ITEM_BombLengthExtend = ItemList.Type.ITEM_BombLengthExtend,
            ITEM_AirDash = ItemList.Type.ITEM_AirDash,
            ITEM_RailPass = ItemList.Type.ITEM_RailPass,
            ITEM_AirshipPass = ItemList.Type.ITEM_AirshipPass,
            ITEM_Explorer = ItemList.Type.ITEM_Explorer,
            ITEM_TempRing = ItemList.Type.ITEM_TempRing,
            ITEM_BoostSystem = ItemList.Type.ITEM_BoostSystem,
            ITEM_AttackRange = ItemList.Type.ITEM_AttackRange,
            ITEM_EasyStyle = ItemList.Type.ITEM_EasyStyle,
            ITEM_DodgeShot = ItemList.Type.ITEM_DodgeShot,
            ITEM_RapidShots = ItemList.Type.ITEM_RapidShots,
            ITEM_OrbAmulet = ItemList.Type.ITEM_OrbAmulet,
            ITEM_GoldenGlove = ItemList.Type.ITEM_GoldenGlove,
            ITEM_Rotater = ItemList.Type.ITEM_Rotater,
            ITEM_AirSlide = ItemList.Type.ITEM_AirSlide,
            ITEM_ZCrystal = ItemList.Type.ITEM_ZCrystal,
            QUEST_LibraryKey = ItemList.Type.QUEST_LibraryKey,
            QUEST_RabiPillow = ItemList.Type.QUEST_RabiPillow,
            QUEST_GHandL = ItemList.Type.QUEST_GHandL,
            QUEST_GHandR = ItemList.Type.QUEST_GHandR,
            BADGE_AmuletQuicken = ItemList.Type.BADGE_AmuletQuicken,
            Useable_VenaBombHealBlock = ItemList.Type.Useable_VenaBombHealBlock,
            Useable_VenaBombDispel = ItemList.Type.Useable_VenaBombDispel,
            Useable_VenaBombBunBun = ItemList.Type.Useable_VenaBombBunBun,
            Useable_VenaBombBig = ItemList.Type.Useable_VenaBombBig,
            Useable_VenaBombSmall = ItemList.Type.Useable_VenaBombSmall,
        }


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

        static public bool vanillaItemCraft = false;
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
            public bool check(Dictionary<string, int> itemList)
            {
                if (Method == "") return true;
                string tmp = Method;
                tmp = Regex.Replace(Method, @"\b(\w|\s)+\b( \d+){0,1}", match => {
                    string[] split;
                    string d = match.ToString();

                    split = d.Split(' ');


                    bool flag = false;

                    switch (split[0])
                    {
                        case "Explorer":
                        case "WindSkip":
                        case "EnemyManip":
                        case "BounceKick":
                        case "ADCKick":
                        case "BarrierSkip":
                            break;
                        case "ItemUse":
                        case "NotTeleporter":
                        case "Boss":
                        case "":
                        case "True":
                            flag = true;
                            break;
                        case "Teleporter":
                            flag = itemList.ContainsKey(d);
                            break;
                        case "LibraryExtra":
                            flag = RandomizerPlugin.customFlags[(int)CustomFlags.SuperBosses];
                            break;
                        case "Backflip":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle BackFlip"]).isOn;
                            break;
                        case "RabbitJump":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle RabbitJump"]).isOn;
                            break;
                        case "RabbitWalljump":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle RabbitWalljump"]).isOn;
                            break;
                        case "CKick":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle CKick"]).isOn;
                            break;
                        case "HiddenP":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle HiddenP"]).isOn;
                            break;
                        case "OpenMorose":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle tmpOption"]).isOn;
                            break;
                        case "EarlyDream":
                            flag = ((UnityEngine.UI.Toggle)settings["Toggle EarlyDream"]).isOn;
                            break;
                        case "Chapter":
                            if (split[0] == "Chapter" && itemList.ContainsKey("EVENT_BOSS"))
                                if (has_chapter_reached(int.Parse(split[1]), itemList["EVENT_BOSS"]))
                                    flag = true;
                                else
                                    flag = false;
                            break;
                        case "AllMemine":
                            Debug.LogError("Memine should not be used as Logic");
                            if (itemList.ContainsKey("EVENT_Memine"))
                                flag = itemList["EVENT_Memine"] > 5;
                            break;
                        case "Memine":
                            Debug.LogError("Memine should not be used as Logic");
                            flag = checkMovementItems(itemList);
                            break;
                        case "ChargeShot":
                            if (itemList.ContainsKey("ITEM_ORB"))
                                    flag = itemList["ITEM_ORB"] > 1;
                            break;
                        case "Upgrade":
                            bool option = ((UnityEngine.UI.Toggle)settings["Toggle NormalItemCraft"]).isOn;
                            flag = (option || checkMovementItems(itemList)) && itemList.ContainsKey("ITEM_LINEBOMB");
                            break;
                        case "Core":
                            flag = checkMovementItems(itemList) && itemList.ContainsKey("ITEM_LINEBOMB") && itemList.ContainsKey("ITEM_AREABOMB") && itemList.ContainsKey("ITEM_BombLengthExtend");
                            break;
                        case "Coins":
                            int amount = int.Parse(split[1]);
                            flag = true;
                            if (amount > 250)
                            {
                                flag = can_aquire_Money(itemList);
                            }
                            break;
                        case "RainbowCheck":
                            if (itemList.ContainsKey("EVENT_Memine"))
                                flag = itemList["EVENT_Memine"] > 2;
                            break;
                        case "Goal":
                            switch (RandomizerPlugin.goalType)
                            {
                                case RandomizerPlugin.GoalType.BossDefeat:
                                    if (itemList.ContainsKey("EVENT_BOSS"))
                                        flag = itemList["EVENT_BOSS"] >= 21;
                                    break;
                                case RandomizerPlugin.GoalType.AstralGear:
                                    if (itemList.ContainsKey("STACKABLE_COG"))
                                        flag = itemList["STACKABLE_COG"] >= RandomizerPlugin.GoMode;
                                    break;
                                default:
                                    if (itemList.ContainsKey("STACKABLE_COG"))
                                        flag = itemList["STACKABLE_COG"] >= RandomizerPlugin.GoMode;
                                    break;
                            }
                            break;
                        case "SpinnerBash":
                            if (!itemList.ContainsKey("ITEM_KNIFE"))
                                break;
                            flag = itemList.ContainsKey("ITEM_KNIFE");
                            break;
                        case "VenaBomb":
                            if (itemList.ContainsKey("EVENT_Fire"))
                                flag = itemList.ContainsKey("Useable_VenaBombSmall") || (itemList.ContainsKey("Useable_VenaBombBig") && itemList.ContainsKey("EVENT_Light"));
                            if (itemList.ContainsKey("EVENT_Earth"))
                                flag = (itemList.ContainsKey("Useable_VenaBombDispel") && itemList.ContainsKey("EVENT_Water")) || (itemList.ContainsKey("Useable_VenaBombHealBlock") && itemList.ContainsKey("EVENT_Dark"));
                            break;
                        default:
                            if (itemList.ContainsKey(split[0]))
                            {
                                flag = true;
                                switch (split[0])
                                {
                                    case "ITEM_BOMBFUEL":
                                        flag = itemList.ContainsKey("ITEM_LINEBOMB") | itemList.ContainsKey("ITEM_AREABOMB");
                                        break;
                                    case "ITEM_BombLengthExtend":
                                        flag = itemList.ContainsKey("ITEM_LINEBOMB");
                                        break;
                                    case "ITEM_AirSlide":
                                        flag = itemList.ContainsKey("ITEM_SLIDE");
                                        break;
                                    case "ITEM_Rotater":
                                        flag = itemList.ContainsKey("ITEM_KNIFE");
                                        break;
                                }
                                if (split.Length > 1 && flag)
                                {
                                    if (int.Parse(split[1]) <= itemList[split[0]])
                                        flag = true;
                                    else
                                        flag = false;
                                }
                            }
                            else
                                flag = false;
                            break;
                    }

                    if (flag)
                        return " true ";
                    else
                        return " false ";
                });
                tmp = tmp.Replace("&&", "AND").Replace("||", "OR");
                bool retVal = (bool)new DataTable().Compute(tmp, "");

                return retVal;
            }

            public bool checkMovementItems(Dictionary<string, int> itemList)
            {

                if (
                    itemList.ContainsKey("ITEM_DOUBLEJUMP") &&
                    itemList.ContainsKey("ITEM_AirDash") &&
                    itemList.ContainsKey("ITEM_WALLJUMP") &&
                    itemList.ContainsKey("ITEM_JETPACK") &&
                    itemList.ContainsKey("ITEM_SLIDE") &&
                    itemList.ContainsKey("ITEM_HIJUMP") &&
                    itemList.ContainsKey("ITEM_WATERMOVEMENT")
                    )
                    return true;

                return false;
            }
            private bool has_chapter_reached(int chapter, int deadBoss)
            {
                short counter = 0;
                bossCount = deadBoss;
                if (bossCount >= 1)
                    counter += 1;
                if (bossCount >= 3)
                    counter += 1;
                if (bossCount >= 5)
                    counter += 1;
                if (bossCount >= 7)
                    counter += 1;
                if (bossCount >= 10)
                    counter += 1;
                if (bossCount >= 13)
                    counter += 1;
                if (bossCount >= 16)
                    counter += 1;
                if (bossCount >= 20)
                    counter += 1;
                return counter >= chapter;
            }
            private bool can_aquire_Money(Dictionary<string, int> itemList)
            {
                if (itemList.ContainsKey("ITEM_KNIFE") || itemList.ContainsKey("ITEM_LINEBOMB"))
                    return true;

                return false;
            }
        }
        private class Location
        {


            public string Itemname;
            public string Loaction;
            public string Locationname;
            public int itemId;
            public int slotId;
            public int newItem;
            public int newSlotId;
            public List<Requirement> Requirement;
            public Location(int itemId, string loaction, string locationName, int slotId, string itemname = "")
            {
                Itemname = itemname;
                Loaction = loaction;
                this.Locationname = locationName;
                this.itemId = itemId;
                this.slotId = slotId;
                this.Requirement = new List<Requirement>();
                this.newItem = 0;
                this.newSlotId = 0;
            }
            public bool isReachAble(Dictionary<string, int> itemList)
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
            public void addMethod(string method)
            {
                Requirement.Add(new Requirement(method));
            }
            public bool debugIsReachAble(Dictionary<string, int> itemList)
            {

                foreach (Requirement req in Requirement)
                {
                    if (req.check(itemList)) {
                        return true;
                    }
                }
                Debug.Log($"Item {Locationname} {newSlotId} could not be reached.");
                return false;
            }
            public bool checkSelfContain(ItemList.Type item)
            {
                foreach (Requirement req in Requirement)
                {
                    if (req.Method.Contains(item.ToString())) return true;
                    if (item == ItemList.Type.I19 && req.Method.Contains("ITEM_OrbTypeC")) return true;
                    if (item == ItemList.Type.I20 && req.Method.Contains("ITEM_OrbTypeS")) return true;
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
            public bool checkEntrance(Dictionary<string, int> itemList)
            {
                if (to == null) return false;
                return method.check(itemList);
            }
            public void debugCheckEntrance(Dictionary<string, int> itemList)
            {
                if (to == null) return;
                if (!method.check(itemList)) Debug.Log($"Could not enter {to.Name}. {method.Method}");
            }


        }


        //Item ID, Amount
        private Dictionary<int, int> itemPool;

        private List<Location> locations;
        private Dictionary<string, Location> locationString;
        private List<Area> areas;
        private static int bossCount = 0;
        private List<Area> transitions = new List<Area>();
        private static Dictionary<int, string> transitionIdToName;
        private List<string> ignoreLocationList = new List<string>();
        public Randomizer()
        {
            string path = RandomizerPlugin.pluginPath + "/resource/";

            itemPool = new Dictionary<int, int>();
            areas = new List<Area>();
            JObject areaJson = JObject.Parse(File.ReadAllText(path + "Area.json"));
            foreach (var map in areaJson)
            {
                foreach (var area in (JArray)map.Value)
                {
                    Area newArea = new Area((string)area["Name"]);
                    int _;
                    if (int.TryParse((string)area["Name"], out _))
                    {
                        transitions.Add(newArea);
                    }
                    foreach (var con in area["Connections"])
                    {
                    }

                    areas.Add(newArea);
                }
            }
            foreach (var map in areaJson)
            {
                foreach (var area in (JArray)map.Value)
                {
                    foreach (var con in area["Connections"])
                    {
                        areas.Find(x => x.Name == (string)area["Name"]).addConnection(areas.Find(x => x.Name == (string)con["Exit"]), (string)con["Method"]);
                    }
                }
            }
            transitionIdToName = new Dictionary<int, string>();
            foreach (string line in File.ReadLines(path + "TransitionId.txt"))
            {
                string[] para = line.Split(':');
                if (para.Count() == 2)
                    transitionIdToName.Add(int.Parse(para[0]), para[1]);
            }

            locations = new List<Location>();
            locationString = new Dictionary<string, Location>();

            JArray locationJson = JArray.Parse(File.ReadAllText(path + "Location.json"));


            foreach (var location in locationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["slotId"], location["Itemname"].ToString());
                ItemList.Type item;
                if (Enum.TryParse(location["Itemname"].ToString(), out item)) {
                    if (itemPool.ContainsKey((int)item))
                        itemPool[(int)item] += 1;
                    else
                        itemPool[(int)item] = 1;
                    newloc.itemId = (int)item;
                    newloc.newItem = 0;

                }
                foreach (var method in location["Requirement"])
                {
                    newloc.addMethod(method["Method"].ToString());
                }
                locationString.Add($"{newloc.Itemname + newloc.slotId}", newloc);
                locations.Add(newloc);
                Area se = areas.Find(x => x.Name == newloc.Loaction);
                if (se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Debug.LogWarning($"Could not find {newloc.Loaction} to place {newloc.Itemname}");
                }
            }

        }

        public string findLocationName(int item, int slot)
        {
            string val = "";
            val = locations.Find(x => x.itemId == item && x.slotId == slot).Loaction;
            return val;
        }



        private ItemList.Type getFillerItem(int randomNumber)
        {
            ItemList.Type item = ItemList.Type.STACKABLE_HP + randomNumber%6;

            return item;
        }


        private void CustomOptions(ref Dictionary<int, int> tmpItemPool,ref List<Location> locationPool, System.Random seed)
        {
            ignoreLocationList.Clear();
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
                                    tmpItemPool[(int)ItemList.Type.ITEM_KNIFE]--;
                                }
                                break;
                            case "Orb":
                                if (((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    locationString["ITEM_ORB1"].setNewItem(ItemList.Type.ITEM_ORB, 4);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.ITEM_ORB && x.slotId == 1));
                                    tmpItemPool[(int)ItemList.Type.ITEM_ORB]--;
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
                                    vanillaItemCraft = true;
                                    foreach (var item in Enum.GetValues(typeof(Upgradable)))
                                    {
                                        ItemList.Type t = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item.ToString());
                                        locationString[t + "2"].setNewItem(t, 5);
                                        locationString[t + "3"].setNewItem(t, 6);
                                        locationPool.Remove(locationPool.Find(x => x.itemId == (int)t && x.slotId == 2));
                                        locationPool.Remove(locationPool.Find(x => x.itemId == (int)t && x.slotId == 3));
                                        tmpItemPool[(int)t] -= 2;
                                    }
                                }
                                else vanillaItemCraft = false;
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
                            case "SuperBosses":
                                if (!((UnityEngine.UI.Toggle)option.Value).isOn)
                                {
                                    locationString["STACKABLE_COG200"].setNewItem(ItemList.Type.STACKABLE_COG, 200);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 200));
                                    ignoreLocationList.Add("STACKABLE_COG200");
                                    locationString["STACKABLE_COG201"].setNewItem(ItemList.Type.STACKABLE_COG, 201);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 201));
                                    ignoreLocationList.Add("STACKABLE_COG201");
                                    locationString["STACKABLE_COG202"].setNewItem(ItemList.Type.STACKABLE_COG, 202);
                                    locationPool.Remove(locationPool.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 202));
                                    ignoreLocationList.Add("STACKABLE_COG202");
                                }
                                RandomizerPlugin.customFlags[(int)CustomFlags.SuperBosses] = ((UnityEngine.UI.Toggle)option.Value).isOn;

                                break;
                            default: break;
                        }
                        break;
                    case "Slider":
                        switch (info[1])
                        {
                            case "RangePot":
                                RandomizerPlugin.extraPotions[0] = (int)((UnityEngine.UI.Slider)option.Value).value;
                                break;
                            case "MeleePot":
                                RandomizerPlugin.extraPotions[1] = (int)((UnityEngine.UI.Slider)option.Value).value;
                                break;
                            case "GearReq":
                                RandomizerPlugin.GoMode = (int)((UnityEngine.UI.Slider)option.Value).value;
                                break;
                            case "GearMax":

                                int max = Math.Max((int)((UnityEngine.UI.Slider)option.Value).value, RandomizerPlugin.GoMode);


                                tmpItemPool[(int)ItemList.Type.STACKABLE_COG] = max;
                                for (int i = max; i < 25; i++)
                                {
                                    ItemList.Type newItem = getFillerItem(seed.Next());
                                    if (tmpItemPool.ContainsKey((int)newItem))
                                        tmpItemPool[(int)newItem]++;
                                    else
                                        tmpItemPool[(int)newItem] = 1;
                                }
                                break;
                            default:
                                break;

                        }
                        break;
                    case "Selector":
                        string selectorName = (((string, int, int))UI.UI.settings[option.Key]).Item1;
                        switch (info[1])
                        {
                            case "GoalType":
                                switch (selectorName)
                                {
                                    case "Boss":
                                        RandomizerPlugin.goalType = RandomizerPlugin.GoalType.BossDefeat;
                                        break;
                                    case "Gear":
                                    default:
                                        RandomizerPlugin.goalType = RandomizerPlugin.GoalType.AstralGear;
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        static bool creating = false;
        
        public void synccreateSeed(System.Random seed)
        {

            int debugVal = 0;
            Dictionary<int, int> localItemPool;

            List<Location> locationPool = new List<Location>();
            Area startarea = areas.Find(x => x.Name == "Thanatara Canyon");
            Dictionary<string, int> progressionItems = new Dictionary<string, int>();
            if (seed == null) seed = new System.Random();
            string traverseMode = (((string, int, int))UI.UI.settings["Selector Traverse"]).Item1;
            if (traverseMode == "Transition")
            {
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
                List<Area> toBePlaced = new List<Area>();
                toBePlaced.CopyFrom(transitions);

                while (toBePlaced.Count > 0)
                {
                    List<Area> availabeTransition = recursivAreaSearch(startarea);
                    Debug.Log(availabeTransition.Count);
                    Area nextTarget = availabeTransition[seed.Next(availabeTransition.Count)];
                    Area newEntrance;
                    if (availabeTransition.Count > toBePlaced.Count)
                    {
                        Debug.LogWarning("Something went wrong");
                        foreach (Area area in availabeTransition)
                        {
                            Debug.Log($"{area.Name} Available");
                        }
                        foreach (Area area in toBePlaced)
                        {
                            Debug.Log($"{area.Name} leftover");
                        }
                    }
                    do
                    {
                        newEntrance = toBePlaced[seed.Next(toBePlaced.Count)];

                    } while (availabeTransition.Contains(newEntrance) && !(availabeTransition.All(toBePlaced.Contains) && toBePlaced.All(availabeTransition.Contains)));

                    nextTarget.Connections[0].to = newEntrance;
                    newEntrance.Connections[0].to = nextTarget;
                    toBePlaced.Remove(newEntrance);
                    toBePlaced.Remove(nextTarget);
                }
            }
            if (traverseMode == "Teleporter")
            {
                RandomizerPlugin.customFlags[(int)(CustomFlags.TeleporterRando)] = true;
                startarea.addConnection(areas.Find(x => x.Name == "TeleportHub"), "");
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
            }
            bool failed = false;

            do
            {
                failed = false;
                debugVal++;
                List<int> TeleporterPool = Enumerable.Range(0, 37).ToList();

                locationPool.Clear();
                locationPool.CopyFrom(locations);
                locationPool.RemoveAll(x => x.itemId > 3000);                 // Remove all Extra Options from Pool (they are above 3000)
                locationPool.RemoveAll(x => x.Itemname.Contains("EVENT"));

                foreach (Location loc in locationPool)
                {
                    loc.setNewItem(-1, -1);
                }

                bossCount = 0;
                localItemPool = new Dictionary<int, int>(itemPool);

                CustomOptions(ref localItemPool, ref locationPool, seed);                 //Extra stuff
                foreach (int key in Enum.GetValues(typeof(Progression_Items)))
                {
                    if (localItemPool.ContainsKey(key))
                    {
                        progressionItems[((Progression_Items)key).ToString()] = localItemPool[key];
                    }
                }

                if (traverseMode == "Teleporter")
                {
                    for (int i = 0; i < 37; i++)
                    {
                        progressionItems[$"Teleporter {i}"] = 1;
                    }
                }


                Debug.Log("Prog");
                while (progressionItems.Count > 0)
                {
                    int itemIndex = seed.Next(progressionItems.Count);
                    string item = progressionItems.ElementAt(itemIndex).Key;
                    int itemId;
                    int slotId = 0;

                    if (item.Contains("Teleporter"))
                    {
                        itemId = (int)RandomizerPlugin.PortalItem;

                        slotId = int.Parse(item.Split(' ')[1]);

                    }
                    else
                        itemId = (int)Enum.Parse(typeof(Progression_Items), item);

                    progressionItems[item]--;
                    if (progressionItems[item] <= 0)
                    {
                        progressionItems.Remove(item);
                        if (localItemPool.ContainsKey(itemId))
                            localItemPool.Remove(itemId);
                    }

                    List<Location> spot = scanLocations(progressionItems);
                    if (spot.Count == 0)
                    {
                        Debug.LogError("No locations left, restarting seed gen");
                        failed = true;
                        break;
                    }
                    int pos = seed.Next(spot.Count);
                    Location loc = locationPool.Find(x => x.Locationname == spot[pos].Locationname);

                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = itemId;
                    locationString[loc.Itemname + loc.slotId.ToString()].newSlotId = slotId;

                    locationPool.Remove(loc);
                }
                if (failed)
                    continue;
                Debug.Log("Filler");

                while (locationPool.Count > 0)
                {
                    int pos = seed.Next(locationPool.Count);
                    Location loc = locationPool[pos];
                    int item = createItem(seed, localItemPool);
                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = item;
                    locationPool.Remove(loc);
                }


                bossCount = 0;

            } while (failed || !validate());
            Debug.Log($"[Randomizer] It took {debugVal} tries to create this Seed.");

            creating = false;
            if (!ArchipelagoInterface.Instance.isConnected)
            {
                RandomizerPlugin.__itemData = GetData();
                Dictionary<int, int> transitionData = new Dictionary<int, int>();
                foreach (Area area in transitions)
                {
                    if (area.Connections[0].to == null)
                        transitionData.Add(int.Parse(area.Name), int.Parse(area.Name));
                    else
                        transitionData.Add(int.Parse(area.Name), int.Parse(area.Connections[0].to.Name));
                }
                RandomizerPlugin.transitionData = transitionData;
                saveSpoilerLog(RandomizerPlugin.__itemData);
            }
        
        }

        public async void createSeed(System.Random seed)
        {
            if (creating || ArchipelagoInterface.Instance.isConnected) { return; }
            creating = true;
            seedCreationLoading();
            await Task.Run(() =>
            {
            int debugVal = 0;
            Dictionary<int, int> localItemPool;
            
            List<Location> locationPool = new List<Location>();
            Area startarea = areas.Find(x => x.Name == "Thanatara Canyon");
            Dictionary<string,int> progressionItems = new Dictionary<string, int>();
            if (seed == null) seed = new System.Random();
            string traverseMode = (((string, int, int))UI.UI.settings["Selector Traverse"]).Item1;
            if (traverseMode == "Transition")
            {
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
                List<Area> toBePlaced = new List<Area>();
                toBePlaced.CopyFrom(transitions);
                
                while (toBePlaced.Count > 0)
                {
                    List<Area> availabeTransition = recursivAreaSearch(startarea);
                    Debug.Log(availabeTransition.Count);
                    Area nextTarget = availabeTransition[seed.Next(availabeTransition.Count)];
                    Area newEntrance;
                    if (availabeTransition.Count > toBePlaced.Count)
                    {
                        Debug.LogWarning("Something went wrong");
                        foreach (Area area in availabeTransition)
                        {
                            Debug.Log($"{area.Name} Available");
                        }
                        foreach (Area area in toBePlaced)
                        {
                            Debug.Log($"{area.Name} leftover");
                        }
                    }
                    do
                    {
                        newEntrance = toBePlaced[seed.Next(toBePlaced.Count)];

                    } while (availabeTransition.Contains(newEntrance) && !(availabeTransition.All(toBePlaced.Contains) && toBePlaced.All(availabeTransition.Contains)));

                    nextTarget.Connections[0].to = newEntrance;
                    newEntrance.Connections[0].to = nextTarget;
                    toBePlaced.Remove(newEntrance);
                    toBePlaced.Remove(nextTarget);
                }
            }
            if (traverseMode == "Teleporter")
            {
                    RandomizerPlugin.customFlags[(int)(CustomFlags.TeleporterRando)] = true;
                startarea.addConnection(areas.Find(x => x.Name == "TeleportHub"), "");
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
            }
            bool failed = false;

            do
            {
                failed = false;
                debugVal++;
                List<int> TeleporterPool = Enumerable.Range(0,37).ToList();
                
                locationPool.Clear();
                locationPool.CopyFrom(locations);
                locationPool.RemoveAll(x => x.itemId > 3000);                 // Remove all Extra Options from Pool (they are above 3000)
                locationPool.RemoveAll(x => x.Itemname.Contains("EVENT"));

                foreach(Location loc in locationPool)
                {
                    loc.setNewItem(-1, -1);
                }

                bossCount = 0;
                localItemPool = new Dictionary<int, int>(itemPool);

                CustomOptions(ref localItemPool,ref locationPool, seed);                 //Extra stuff
                foreach (int key in Enum.GetValues(typeof(Progression_Items)))
                {
                    if (localItemPool.ContainsKey(key))
                    {
                        progressionItems[((Progression_Items)key).ToString()] = localItemPool[key];
                    }
                }

                if(traverseMode == "Teleporter")
                {
                    for(int i = 0; i < 37; i++)
                    {
                        progressionItems[$"Teleporter {i}"] = 1;
                    }
                }
                    

                Debug.Log("Prog");
                while(progressionItems.Count > 0)
                {
                    int itemIndex = seed.Next(progressionItems.Count);
                    string item = progressionItems.ElementAt(itemIndex).Key;
                    int itemId;
                    int slotId = 0;

                    if (item.Contains("Teleporter"))
                    {
                        itemId = (int)RandomizerPlugin.PortalItem;
                        
                         slotId = int.Parse(item.Split(' ')[1]);

                    }
                    else
                        itemId = (int)Enum.Parse(typeof(Progression_Items), item);

                    progressionItems[item]--;
                    if (progressionItems[item] <= 0)
                    {
                        progressionItems.Remove(item);
                        if (localItemPool.ContainsKey(itemId))
                            localItemPool.Remove(itemId);
                    }

                    List<Location> spot = scanLocations(progressionItems);
                    if(spot.Count == 0)
                    {
                        Debug.LogError("No locations left, restarting seed gen");
                        failed = true;
                        break;
                    }
                    int pos = seed.Next(spot.Count);
                    Location loc = locationPool.Find(x => x.Locationname == spot[pos].Locationname);

                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = itemId;
                    locationString[loc.Itemname + loc.slotId.ToString()].newSlotId = slotId;

                    locationPool.Remove(loc);
                }
                if (failed)
                    continue;
                Debug.Log("Filler");

                while (locationPool.Count > 0)
                {
                    int pos = seed.Next(locationPool.Count);
                    Location loc = locationPool[pos];
                    int item = createItem(seed,localItemPool);
                    locationString[loc.Itemname + loc.slotId.ToString()].newItem = item;
                    locationPool.Remove(loc);
                }


                bossCount = 0;

            } while (failed || !validate());
            Debug.Log($"[Randomizer] It took {debugVal} tries to create this Seed.");

                creating = false;
                if (!ArchipelagoInterface.Instance.isConnected)
                {
                    RandomizerPlugin.__itemData = GetData();
                    Dictionary<int, int> transitionData = new Dictionary<int, int>();
                    foreach (Area area in transitions)
                    {
                        if (area.Connections[0].to == null)
                            transitionData.Add(int.Parse(area.Name), int.Parse(area.Name));
                        else
                            transitionData.Add(int.Parse(area.Name), int.Parse(area.Connections[0].to.Name));
                    }
                    RandomizerPlugin.transitionData = transitionData;
                    saveSpoilerLog(RandomizerPlugin.__itemData);
                }
            });
        }
        
        private List<Area> recursivAreaSearch(Area startArea, List<Area> visited = null)
        {
            List<Area> area = new List<Area>();
            if (visited == null) visited = new List<Area>();
            if (startArea == null || visited.Contains(startArea)) return area;
            visited.Add(startArea);
            foreach (Entrance con in startArea.Connections)
            {
                area.AddRange(recursivAreaSearch(con.to, visited));
            }
            if (int.TryParse(startArea.Name, out _) && startArea.Connections[0].to == null)
                area.Add(startArea);
            return area;
        }


        public async void seedCreationLoading()
        {
            TextMeshProUGUI text = UI.UI.finishedText.GetComponent<TextMeshProUGUI>();
            string t = "Creating ";
            text.text = "Creating";
            UI.UI.finishedText.SetActive(true);
            await Task.Run(() =>
            {
                int count = 0;
                while (creating)
                {
                    count++;
                    text.text = t + new string('.', count % 4);
                    Task.Delay(300).Wait();
                }
                if (ArchipelagoInterface.Instance.isConnected)
                {
                    text.text = "Connected to AP Server";
                }
                else
                    text.text = "Finished Creating Seed";

            });

        }

        private int createItem(System.Random seed, Dictionary<int, int> pool)
        {
            int item = -1;

            if(pool.Count == 0)
            {
                Debug.LogWarning("Not enough item in the Pool");
                return seed.Next(5) + 1;
            }
            while (item <0)
            {
                int rnd = seed.Next((int)ItemList.Type.MAX);
                if (pool.ContainsKey(rnd))
                {
                    item = rnd;
                    pool[rnd]--;
                    if (pool[rnd] <= 0)
                    {
                        pool.Remove(rnd);
                    }
                }
            }
            return item;
        }

        Dictionary<string, int> itemList = new Dictionary<string, int>();
        List<Area> areaList;

        /*
         * If itemPool is null -> returns all non reachable Locations
         * else return all reachable Locations with no Item placed
         */
        private List<Location> scanLocations(Dictionary<string, int> itemPool = null, Area startArea = null) {

            if (ArchipelagoInterface.Instance.isConnected)
            {
                return null;
            }

            //Check if Return value contains all reachsable locations
            bool leftover = false;
            if (itemPool == null)
            {
                itemPool = new Dictionary<string, int>();
                leftover = true;
            }

            List<Location> locationsFound = new List<Location>();


            int currHint = 0;
            int currBackHint = 1;
            int itemCount;

            if (startArea == null)
                areaList = [areas.Find(x => x.Name == "Thanatara Canyon")];
            else
                areaList = [startArea];
            itemList.Clear();
            if (areaList.Count == 0)
            {
                Debug.LogError("[Randomizer] No Entries in areaList!!!!");
            }

            foreach(var item in itemPool)
            {
                itemList[item.Key] = item.Value;
            }
            List<Location> checkedLocation = new List<Location>();
            List<Entrance> entrances = new List<Entrance>();
            List<Location> localLocations = new List<Location>();
            entrances.AddRange(areaList[0].Connections);
            localLocations.AddRange(areaList[0].Locations);


            itemCount = -1;
            int areaCount = -1;
            while (itemList.Count != itemCount || checkedLocation.Count != areaCount)
            {
                itemCount = itemList.Count;
                areaCount = checkedLocation.Count;

                foreach (Entrance en in entrances.ToArray())
                {
                    if (en.to == null)
                    {
                        entrances.Remove(en);
                        continue;
                    }
                    if (areaList.Contains(en.to))
                    {
                        entrances.Remove(en); continue;
                    }

                    if (!areaList.Contains(en.to) && en.checkEntrance(itemList))
                    {
                        areaList.Add(en.to);
                        foreach (Entrance e in en.to.Connections)
                        {
                            if (!areaList.Contains(e.to)) entrances.Add(e);
                        }
                        localLocations.AddRange(en.to.Locations);
                        entrances.Remove(en);
                    }
                }

                foreach (Location loc in localLocations.ToArray())
                {
                    if (ignoreLocationList.Contains($"{loc.Itemname}{loc.slotId}"))
                    {
                        checkedLocation.Add(loc);
                        localLocations.Remove(loc);
                        continue; 
                    }

                    if (checkedLocation.Contains(loc))
                    {
                        localLocations.Remove(loc);
                        continue;
                    }

                    if (loc.isReachAble(itemList))
                    {

                        //Non standart item
                        if (loc.newItem == 0)
                        {
                            if (!itemList.ContainsKey(loc.Itemname))
                            {
                                itemList[loc.Itemname] = 1;
                            }
                            else
                            {
                                itemList[loc.Itemname] += 1;
                            }
                        }

                        string item;
                        if (loc.newItem > 0)
                        {
                            if (((ItemList.Type)loc.newItem) == RandomizerPlugin.PortalItem)
                            {
                                item = $"Teleporter {loc.newSlotId}";
                            }
                            else
                            {
                                item = ((ItemList.Type)loc.newItem).ToString();
                            }

                            if (!itemList.ContainsKey(item))
                            {
                                itemList[item] = 1;
                            }
                            else
                            {
                                itemList[item] += 1;
                            }



                            //Add major Item to Hint List
                            if (Enum.IsDefined(typeof(MajorItemFlag), item) && currHint < HintSystem.numberOfHints)
                            {
                                if (vanillaItemCraft && loc.Loaction.Contains("Upgrade") && currBackHint + currHint < HintSystem.numberOfHints)
                                {

                                    HintSystem.hintList[HintSystem.hintList.Length - currBackHint] = (loc.Locationname, item, (byte)loc.newSlotId);
                                    currBackHint++;
                                }
                                else
                                {
                                    HintSystem.hintList[currHint] = (loc.Locationname, item, (byte)loc.newSlotId);
                                    currHint++;
                                }
                            }
                        }
                        if(loc.newItem < 0)
                        {
                            locationsFound.Add(loc);
                        }
                        checkedLocation.Add(loc);
                        localLocations.Remove(loc);

                    }
                }
            }



            if (leftover)
            {
                return checkedLocation;
            }
            return locationsFound;

        }

        private bool validate(Area startArea = null)
        {
            if (ArchipelagoInterface.Instance.isConnected)
            {
                return false;
            }

            if (startArea == null)
                areaList = [areas.Find(x => x.Name == "Thanatara Canyon")];
            else
                areaList = [startArea];

            //Check if its beatable

            List<Location> preCheck = scanLocations();
            if (preCheck.Count < locations.Count)
            {
                Debug.LogError("Not all Location have an Item");
                foreach (Location loc in locations)
                {
                    if (!preCheck.Contains(loc))
                        Debug.LogError($"Location {loc.Locationname} is not reachable");
                }
                return false;
            }

            switch (RandomizerPlugin.goalType)
            {
                case RandomizerPlugin.GoalType.AstralGear:
                default:
                    if (itemList.ContainsKey("STACKABLE_COG") && itemList["STACKABLE_COG"] >= RandomizerPlugin.GoMode)
                    {
                        return true;
                    }
                    break;
                case RandomizerPlugin.GoalType.BossDefeat:
                    if (itemList.ContainsKey("EVENT_BOSS") && itemList["EVENT_BOSS"] >= 21)
                        return true;
                    break;
            }

            foreach (var item in itemList)
            {
                if(Enum.IsDefined(typeof(Progression_Items),item.Key))
                    Debug.LogWarning($"{item.Key}:{item.Value}");
            }
            return false;

            /*
            if (!itemList.ContainsKey("STACKABLE_COG"))
                return false;
            if (itemList["STACKABLE_COG"] > Math.Floor((float)RandomizerPlugin.GoMode / 2f))
            {
                //Debug.LogWarning("CheckIn");
                if (areaList.Count != areas.Count)
                {
                    foreach (Area ar in areaList)
                    {
                        bool f = false;
                        foreach (Area a in areas)
                        {
                            if (a.Name == ar.Name) { f = true; break; }
                        }
                        if (!f) { Debug.LogError($"{ar.Name} Not IN LIST"); }
                    }

                }
                bool flag = false;
                foreach (Location loc in locations)
                {
                    if (ignoreLocationList.Contains($"{loc.Itemname}{loc.slotId}"))
                        continue;
                    flag |= !loc.debugIsReachAble(itemList);

                }
                if (flag)
                {
                    return false;
                }

                foreach (Entrance entrance in entrances)
                {
                    entrance.debugCheckEntrance(itemList);
                }
                return goalCheck(itemList, areaList);
            }
            return false;
            */

        }
        private bool goalCheck(Dictionary<string, int> itemList, List<Area> areaList)
        {
            if (itemList["STACKABLE_COG"] < RandomizerPlugin.GoMode)
            {
                Debug.LogWarning($"Not Enough Gears in the run. Found {itemList["STACKABLE_COG"]}");
                return false;
            }
            if (!itemList.ContainsKey("ITEM_SLIDE"))
            {
                Debug.LogWarning($"Slide Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_LINEBOMB"))
            {
                Debug.LogWarning($"LineBomb Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_DOUBLEJUMP"))
            {
                Debug.LogWarning($"Double Jump Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_JETPACK"))
            {
                Debug.LogWarning($"Jet Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_WALLJUMP"))
            {
                Debug.LogWarning($"Walljump Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_AirDash"))
            {
                Debug.LogWarning($"Air Dash Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_AirSlide"))
            {
                Debug.LogWarning($"Fairy Powder Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_Rotater"))
            {
                Debug.LogWarning($"Vortex Glove Not Found");

                return false;
            }
            if (!itemList.ContainsKey("ITEM_HIJUMP"))
            {
                Debug.LogWarning($"Highjump Glove Not Found");

                return false;
            }
            return true;
        }

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Location loc in locations)
            {



                string item1 = loc.Locationname;
                string item2 = ((ItemList.Type)loc.newItem).ToString();
                try
                {
                    if (item2 == RandomizerPlugin.PortalItem.ToString())
                    {
                        data.Add(item1, $"Teleporter {loc.newSlotId}");
                    }
                    else
                    {
                        data.Add(item1, item2);
                    }
                }
                catch
                {
                    Debug.LogWarning($"Already changed {item1}. Dropping {item2}");

                }
            }


            return data;
        }


        static public Dictionary<string, object> settings = new Dictionary<string, object>();


        static public void saveSpoilerLog(Dictionary<string, string> itemData)
        {
            if (!Directory.Exists(RandomizerPlugin.pluginPath + "/Data")) Directory.CreateDirectory(RandomizerPlugin.pluginPath + "/Data");
            DateTime today = DateTime.Now;
            StreamWriter spoilerLog = File.CreateText(RandomizerPlugin.pluginPath + "/Data/" + $"Spoilerlog.{today.Day}.{today.Month}.{today.Year}.{today.Hour}.{today.Minute}.txt");
            ItemList.Type value;
            spoilerLog.WriteLine("Locations:");
            foreach (KeyValuePair<string, string> item in itemData)
            {
                if (ItemList.Type.TryParse(item.Value, out value))
                {
                    spoilerLog.WriteLine($"{item.Key} => {Localize.GetLocalizeTextWithKeyword("ITEMNAME." + (value).ToString(), false)}");
                }
                else
                {
                    spoilerLog.WriteLine($"{item.Key} => {item.Value}");
                }
            }
            spoilerLog.WriteLine("\nTransitions:");
            if (RandomizerPlugin.transitionData != null)
            {
                foreach (var entry in RandomizerPlugin.transitionData)
                {
                    spoilerLog.WriteLine($"{transitionIdToName[entry.Key]} -> {transitionIdToName[entry.Value]}");
                }
            }
            spoilerLog.Close();
        }

        private List<Location> getAvailableLocations(Area startArea)
        {
            List<Location> locations = new List<Location>();

            return locations;
        }

    }
    }
