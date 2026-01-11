using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeviRandomizer.TeviRandomizerSettings;



namespace TeviRandomizer
{
    public abstract class OptionObserver
    {
        public object Value
        {
            get
            {
                return _getValue();
            }
        }

        private Func<object> _getValue = () => default!;
        public string Name;
        public string OptionType { get; }
        protected OptionObserver(string type)
        {
            OptionType = type;
            Name = "";
        }
        protected void setValFunction<T>(Func<T> func)
        {
            _getValue = () => func();
        }
    }


    public class ToggleObserver : OptionObserver
    {
        public ToggleObserver(string name, Func<bool> accessor) : base("Toggle")
        {
            Name = name;
            setValFunction(accessor);
        }
        public bool isOn() => Value != null ? (bool)Value : false;
    }

    public class SliderObserver : OptionObserver
    {
        public SliderObserver(string name, Func<int> accessor) : base("Slider")
        {
            Name = name;
            setValFunction(accessor);
        }

    }
    public class SelectorObserver : OptionObserver
    {
        public SelectorObserver(string name, Func<string> accessor) : base("Selector")
        {
            Name = name;
            setValFunction(accessor);
        }
    }



    public class Randomizer
    {


        private void CustomOptions(System.Random seed)
        {
            ignoreLocationList.Clear();
            foreach (var keyValuePair in settings)
            {
                var option = keyValuePair.Value;


                switch (option.OptionType)
                {
                    case "Toggle":
                        bool isOn = (bool)option.Value;
                        switch (option.Name)
                        {
                            case "Knife":
                                if (isOn)
                                {

                                    Location loc = locations.Find(x => x.itemId == (int)ItemList.Type.ITEM_KNIFE && x.slotId == 1);
                                    loc.setNewItem(ItemList.Type.ITEM_KNIFE, 4);
                                    locations.Remove(loc);
                                    itemPool[ItemList.Type.ITEM_KNIFE.ToString()]--;
                                }
                                break;
                            case "Orb":
                                if (isOn)
                                {
                                    Location loc = locations.Find(x => x.itemId == (int)ItemList.Type.ITEM_ORB && x.slotId == 1);
                                    loc.setNewItem(ItemList.Type.ITEM_ORB, 4);
                                    locations.Remove(loc);
                                    itemPool[ItemList.Type.ITEM_ORB.ToString()]--;
                                }

                                break;
                            case "Lv3Compass":
                                TeviSettings.customFlags[CustomFlags.CompassStart] = isOn;
                                break;
                            case "tmpOption":
                                TeviSettings.customFlags[CustomFlags.TempOption] = isOn;
                                break;
                            case "NormalItemCraft":
                                TeviSettings.customFlags[CustomFlags.NormalItemCraft] = isOn;
                                if (isOn)
                                {
                                    foreach (var item in Enum.GetValues(typeof(Upgradable)))
                                    {
                                        ItemList.Type t = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item.ToString());
                                        Location loc = locations.Find(x => x.itemId == (int)t && x.slotId == 2);
                                        loc.setNewItem(t, 5);
                                        locations.Remove(loc);

                                        loc = locations.Find(x => x.itemId == (int)t && x.slotId == 3);
                                        loc.setNewItem(t, 6);
                                        locations.Remove(loc);
                                        itemPool[t.ToString()] -= 2;
                                    }
                                }
                                break;
                            case "Ceble":
                                TeviSettings.customFlags[CustomFlags.CebleStart] = isOn;
                                break;
                            case "SuperBosses":
                                if (!isOn)
                                {
                                    Location loc = locations.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 200);
                                    loc.setNewItem(ItemList.Type.STACKABLE_COG, 200);
                                    locations.Remove(loc);
                                    ignoreLocationList.Add(loc);

                                    loc = locations.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 201);
                                    loc.setNewItem(ItemList.Type.STACKABLE_COG, 201);
                                    locations.Remove(loc);
                                    ignoreLocationList.Add(loc);

                                    loc = locations.Find(x => x.itemId == (int)ItemList.Type.STACKABLE_COG && x.slotId == 202);
                                    loc.setNewItem(ItemList.Type.STACKABLE_COG, 202);
                                    locations.Remove(loc);
                                    ignoreLocationList.Add(loc);
                                }
                                TeviSettings.customFlags[CustomFlags.SuperBosses] = isOn;

                                break;
                            case "BackFlip":
                                TeviSettings.customFlags[CustomFlags.BackFlip] = isOn;
                                break;
                            case "RabbitJump":
                                TeviSettings.customFlags[CustomFlags.RabbitJump] = isOn;
                                break;
                            case "RabbitWalljump":
                                TeviSettings.customFlags[CustomFlags.RabbitWalljump] = isOn;
                                break;
                            case "CKick":
                                TeviSettings.customFlags[CustomFlags.CKick] = isOn;
                                break;
                            case "HiddenP":
                                TeviSettings.customFlags[CustomFlags.HiddenP] = isOn;
                                break;
                            case "RandomMoney":
                                TeviSettings.customFlags[CustomFlags.RandomMoney] = isOn;
                                if (isOn)
                                    locations.AddRange(Money);
                                else
                                    itemPool.Remove(ItemList.Type.I14.ToString());
                                break;
                            case "RandomResource":
                                TeviSettings.customFlags[CustomFlags.RandomResource] = isOn;
                                if (isOn)
                                    locations.AddRange(resources);
                                else
                                {
                                    itemPool.Remove(ItemList.Type.I15.ToString());
                                    itemPool.Remove(ItemList.Type.I16.ToString());
                                }
                                break;
                            default: break;
                        }
                        break;
                    case "Slider":
                        int value = (int)option.Value;
                        switch (option.Name)
                        {
                            case "RangePot":
                                TeviSettings.extraPotions[0] = value;
                                break;
                            case "MeleePot":
                                TeviSettings.extraPotions[1] = value;
                                break;
                            case "GearReq":
                                TeviSettings.GoMode = Math.Max(Math.Min(value, 24), 1);
                                break;
                            case "GearMax":

                                int max = Math.Max(value, TeviSettings.GoMode);


                                itemPool[ItemList.Type.STACKABLE_COG.ToString()] = max;
                                for (int i = max; i < 25; i++)
                                {
                                    ItemList.Type newItem = getFillerItem(seed.Next());
                                    if (itemPool.ContainsKey(newItem.ToString()))
                                        itemPool[newItem.ToString()]++;
                                    else
                                        itemPool[newItem.ToString()] = 1;
                                }
                                break;
                            default:
                                break;

                        }
                        break;
                    case "Selector":
                        string selected = (string)option.Value;
                        switch (option.Name)
                        {
                            case "GoalType":
                                switch (selected)
                                {
                                    case "Boss":
                                        TeviSettings.goalType = GoalType.BossDefeat;
                                        break;
                                    case "Gear":
                                    default:
                                        TeviSettings.goalType = GoalType.AstralGear;
                                        break;
                                }
                                break;
                            case "Traverse":
                                TeviSettings.traverseMode = selected;
                                TeviSettings.customFlags[CustomFlags.TeleporterRando] = selected == "Teleporter";
                                TeviSettings.customFlags[CustomFlags.EntranceRando] = selected == "Entrance";
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            foreach (var item in itemPool.ToArray())
            {
                if (item.Value <= 0)
                    itemPool.Remove(item.Key);
            }
        }

        static Dictionary<string, string> getAPItemNames()
        {
            Dictionary<string, string> retVal = new();
            string path = TeviSettings.pluginPath + "/resource/";
            JToken itemNames = JToken.Parse(File.ReadAllText(path + "ItemToReal.json"));
            foreach (var item in Enum.GetNames(typeof(ItemList.Type)))
            {
                retVal[item] = (string)itemNames[item];
            }
            return retVal;
        }
        static Dictionary<string, string> APItemNames => getAPItemNames();



        static bool LogicHelper(Dictionary<string, int> itemList, string logic)
        {
            string[] split;
            split = logic.Split(' ');
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
                    flag = itemList.ContainsKey(logic);
                    break;
                case "LibraryExtra":
                    flag = TeviSettings.customFlags[CustomFlags.SuperBosses];
                    break;
                case "Backflip":
                    flag = TeviSettings.customFlags[CustomFlags.BackFlip];
                    break;
                case "RabbitJump":
                    flag = TeviSettings.customFlags[CustomFlags.RabbitJump];
                    break;
                case "RabbitWalljump":
                    flag = TeviSettings.customFlags[CustomFlags.RabbitWalljump];
                    break;
                case "CKick":
                    flag = TeviSettings.customFlags[CustomFlags.CKick];
                    break;
                case "HiddenP":
                    flag = TeviSettings.customFlags[CustomFlags.HiddenP];
                    break;
                case "OpenMorose":
                    flag = TeviSettings.customFlags[CustomFlags.tmpOption];
                    break;
                case "EarlyDream":
                    flag = TeviSettings.customFlags[CustomFlags.EarlyDream];
                    break;
                case "Chapter":
                    if (split[0] == "Chapter" && itemList.ContainsKey("EVENT_BOSS"))
                        if (has_chapter_reached(int.Parse(split[1]), itemList["EVENT_BOSS"]))
                            flag = true;
                        else
                            flag = false;
                    break;
                case "AllMemine":
                    Console.WriteLine("Memine should not be used as Logic");
                    if (itemList.ContainsKey("EVENT_Memine"))
                        flag = itemList["EVENT_Memine"] > 5;
                    break;
                case "Memine":
                    Console.WriteLine("Memine should not be used as Logic");
                    flag = checkMovementItems(itemList);
                    break;
                case "ChargeShot":
                    if (itemList.ContainsKey("ITEM_ORB"))
                        flag = itemList["ITEM_ORB"] >= 2;
                    break;
                case "Upgrade":
                    flag = itemList.ContainsKey("I16") && itemList["I16"] >= 90;
                    break;
                case "Core":
                    flag = itemList.ContainsKey("I15") && itemList["I15"] >= 35;
                    break;
                case "Coins":
                    int amount = int.Parse(split[1]);
                    flag = true;
                    if (amount > 250)
                    {
                        return true;
                    }
                    break;
                case "RainbowCheck":
                    if (itemList.ContainsKey("EVENT_Memine"))
                        flag = itemList["EVENT_Memine"] >= 3;
                    break;
                case "Goal":
                    switch (TeviSettings.goalType)
                    {
                        case TeviRandomizerSettings.GoalType.BossDefeat:
                            if (itemList.ContainsKey("EVENT_BOSS"))
                                flag = itemList["EVENT_BOSS"] >= 21;
                            break;
                        case TeviRandomizerSettings.GoalType.AstralGear:
                            if (itemList.ContainsKey("STACKABLE_COG"))
                                flag = itemList["STACKABLE_COG"] >= TeviSettings.GoMode;
                            break;
                        default:
                            if (itemList.ContainsKey("STACKABLE_COG"))
                                flag = itemList["STACKABLE_COG"] >= TeviSettings.GoMode;
                            break;
                    }
                    break;
                case "SpinnerBash":
                    if (itemList.ContainsKey("ITEM_KNIFE"))
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
                    break;
            }
            return flag;
        }


        public static bool checkMovementItems(Dictionary<string, int> itemList)
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
        private static bool has_chapter_reached(int chapter, int deadBoss)
        {
            short counter = 0;
            int bossCount = deadBoss;
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
        private static bool can_aquire_Money(Dictionary<string, int> itemList)
        {
            if (itemList.ContainsKey("ITEM_KNIFE") || itemList.ContainsKey("ITEM_LINEBOMB"))
                return true;

            return false;
        }









        public class Requirement
        {
            public string Method = "";
            public int Difficulty = 0;
            static DataTable validator = new DataTable();
            Operation Logic;
            public Requirement(string method)
            {
                if (method == "" || method == "()")
                    method = "True";
                Method = method;
                string tmp = method;
                method = method.Replace("&&", "&").Replace("||", "|");
                var tokensList = Regex
                    .Split(method, @"([()&|!~])")
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                tokensList.Reverse();
                Stack<object> tokens = new(tokensList);
                Stack<object> stack = new();

                while (tokens.Count > 0)
                {
                    var next = tokens.Pop();
                    if (isExpr(next))
                    {
                        if (stack.Count == 0)
                        {
                            stack.Push(next);
                            continue;
                        }
                        var head = stack.Peek();
                        switch (head)
                        {
                            case "&":
                                stack.Pop();
                                var exp = stack.Pop();
                                Debug.Assert(isExpr(exp));
                                tokens.Push(new OpAnd((Operation)exp, (Operation)next));
                                break;
                            case "|":
                                stack.Pop();
                                exp = stack.Pop();
                                Debug.Assert(isExpr(exp));
                                tokens.Push(new OpOr((Operation)exp, (Operation)next));
                                break;
                            case "!":
                            case "~":
                                stack.Pop();
                                tokens.Push(new OpNot((Operation)next));
                                break;
                            default:
                                stack.Push(next);
                                break;
                        }
                    }
                    else if ("(&|!~".Contains(next.ToString()))
                    {
                        stack.Push(next);
                    }
                    else if (next.ToString() == ")")
                    {
                        var exp = stack.Pop();
                        Debug.Assert(isExpr(exp));
                        var paren = stack.Pop();
                        Debug.Assert(paren.ToString() == "(");
                        tokens.Push(exp);
                    }
                    else
                        tokens.Push(new OpLit(next.ToString()));
                }
                Debug.Assert(stack.Count == 1 && isExpr(stack.Peek()));
                Logic = (Operation)stack.Pop();
            }
            bool isExpr(object token) => token is Operation;
            public bool evaluate(Dictionary<string, int> itemList) => Logic.Evaluate(itemList);


            abstract class Operation
            {

                public abstract bool Evaluate(Dictionary<string, int> itemList);
            }
            class OpLit : Operation
            {
                string name;
                public override bool Evaluate(Dictionary<string, int> itemList) => LogicHelper(itemList, name);
                public OpLit(string name)
                {
                    this.name = name;
                }
            }
            class OpNot : Operation
            {
                Operation op;
                public override bool Evaluate(Dictionary<string, int> itemList)
                {
                    return !op.Evaluate(itemList);
                }

                public OpNot(Operation op)
                {
                    this.op = op;
                }
            }
            class OpOr : Operation
            {
                Operation opL;
                Operation opR;
                public override bool Evaluate(Dictionary<string, int> itemList)
                {
                    return opL.Evaluate(itemList) || opR.Evaluate(itemList);
                }
                public OpOr(Operation op, Operation opR)
                {
                    this.opL = op;
                    this.opR = opR;
                }
            }
            class OpAnd : Operation
            {
                Operation opL;
                Operation opR;
                public override bool Evaluate(Dictionary<string, int> itemList)
                {
                    return opL.Evaluate(itemList) && opR.Evaluate(itemList);
                }
                public OpAnd(Operation op, Operation opR)
                {
                    this.opL = op;
                    this.opR = opR;
                }
            }


        }




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
        public class Location
        {


            public string Itemname;
            public string Loaction;
            public string Locationname;
            public int itemId;
            public int slotId;
            public string newItem;
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
                this.newItem = itemname;
                this.newSlotId = slotId;
            }
            public bool isReachAble(Dictionary<string, int> itemList)
            {
                foreach (Requirement req in Requirement)
                {
                    if (req.evaluate(itemList)) return true;
                }
                return false;
            }
            public void setNewItem(ItemList.Type item, int slot)
            {
                newItem = item.ToString();
                newSlotId = slot;
            }
            public void setNewItem(int item, int slot)
            {
                newItem = ((ItemList.Type)item).ToString();
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
                    if (req.evaluate(itemList))
                    {
                        return true;
                    }
                }
                Console.WriteLine($"Item {Locationname} {newSlotId} could not be reached.");
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
        public class Area
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
            public bool checkEntrance(Dictionary<string, int> itemList)
            {
                if (to == null) return false;
                return method.evaluate(itemList);
            }
            public void debugCheckEntrance(Dictionary<string, int> itemList)
            {
                if (to == null) return;
                if (!method.evaluate(itemList)) Console.WriteLine($"Could not enter {to.Name}. {method.Method}");
            }
        }


        //Item ID, Amount
        private Dictionary<string, int> itemPool;

        public bool finished = false;
        private List<Location> locations;
        private List<Area> areas;
        private List<Area> transitions = new List<Area>();
        private static Dictionary<int, string> transitionIdToName = null;
        private List<Location> resources = new List<Location>();
        private List<Location> Money = new List<Location>();

        private HashSet<Location> ignoreLocationList = new HashSet<Location>();
        public Randomizer()
        {

            string resourcePath = TeviSettings.pluginPath + "/resource/";
            itemPool = new Dictionary<string, int>();
            areas = new List<Area>();
            JObject areaJson = JObject.Parse(File.ReadAllText(resourcePath + "Area.json"));
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
            if (transitionIdToName == null)
            {
                transitionIdToName = new Dictionary<int, string>();
                foreach (string line in File.ReadLines(resourcePath + "TransitionId.txt"))
                {
                    string[] para = line.Split(':');
                    if (para.Count() == 2)
                        transitionIdToName.Add(int.Parse(para[0]), para[1]);
                }
            }
            locations = new List<Location>();

            JArray locationJson = JArray.Parse(File.ReadAllText(resourcePath + "Location.json"));


            foreach (var location in locationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["slotId"], location["Itemname"].ToString());
                ItemList.Type item;
                if (Enum.TryParse(location["Itemname"].ToString(), out item))
                {
                    if (itemPool.ContainsKey(item.ToString()))
                        itemPool[item.ToString()] += 1;
                    else
                        itemPool[item.ToString()] = 1;
                    newloc.itemId = (int)item;

                }
                foreach (var method in location["Requirement"])
                {
                    newloc.addMethod(method["Method"].ToString());
                }
                locations.Add(newloc);
                Area se = areas.Find(x => x.Name == newloc.Loaction);
                if (se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.Loaction} to place {newloc.Itemname}");
                }
            }

            itemPool[ItemList.Type.QUEST_Memory.ToString()] = 1;
            itemPool[ItemList.Type.QUEST_Flute.ToString()] = 1;
            itemPool[ItemList.Type.QUEST_Compass.ToString()] = 1;

            JArray moneyLocationJson = JArray.Parse(File.ReadAllText(resourcePath + "MoneyLocations.json"));
            JArray resourceLocationJson = JArray.Parse(File.ReadAllText(resourcePath + "UpgradeResourceLocation.json"));

            foreach (var location in moneyLocationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["area"] * 100_000_000 + (int)location["blockId"], location["Itemname"].ToString());
                ItemList.Type item;
                if (Enum.TryParse(location["Itemname"].ToString(), out item))
                {
                    if (itemPool.ContainsKey(item.ToString()))
                        itemPool[item.ToString()] += 1;
                    else
                        itemPool[item.ToString()] = 1;
                    newloc.itemId = (int)item;

                }
                foreach (var method in location["Requirement"])
                {
                    newloc.addMethod(method["Method"].ToString());
                }

                Money.Add(newloc);
                Area se = areas.Find(x => x.Name == newloc.Loaction);
                if (se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.Loaction} to place {newloc.Itemname}");
                }
            }
            foreach (var location in resourceLocationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["area"] * 100_000_000 + (int)location["blockId"], location["Itemname"].ToString());
                ItemList.Type item;
                if (Enum.TryParse(location["Itemname"].ToString(), out item))
                {
                    if (itemPool.ContainsKey(item.ToString()))
                        itemPool[item.ToString()] += 1;
                    else
                        itemPool[item.ToString()] = 1;
                    newloc.itemId = (int)item;

                }



                foreach (var method in location["Requirement"])
                {
                    newloc.addMethod(method["Method"].ToString());
                }

                resources.Add(newloc);
                Area se = areas.Find(x => x.Name == newloc.Loaction);
                if (se != null)
                {
                    se.Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.Loaction} to place {newloc.Itemname}");
                }
            }


        }


        private ItemList.Type getFillerItem(int randomNumber)
        {
            ItemList.Type item = ItemList.Type.STACKABLE_HP + randomNumber % 6;

            return item;
        }


        public static bool creating = false;
        public static bool failed = false;

        private string createItem(Dictionary<string, int> itemPool, System.Random seed)
        {
            string item = "";
            int[] weight = new int[itemPool.Count];
            int total = 0;
            for (int i = 0; i < itemPool.Count; i++)
            {
                total += itemPool.ElementAt(i).Value;
                weight[i] = total;
            }
            int itemIndex = seed.Next(total);
            for (int i = 0; i < weight.Length; i++)
                if (itemIndex < weight[i])
                {
                    item = itemPool.ElementAt(i).Key;
                    break;
                }
            if (String.IsNullOrEmpty(item))
                item = "PANIC";
            return item;
        }

        public bool newSeed(Random seed)
        {
            if (seed == null) seed = new System.Random();

            Area startarea = areas.Find(x => x.Name == "Thanatara Canyon");
            Dictionary<string, int> startItems = new();
            CustomOptions(seed);                 //Extra stuff

            string traverseMode = TeviSettings.traverseMode;
            if (traverseMode == "Transition")
            {
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
                List<Area> toBePlaced = new(transitions);


                while (toBePlaced.Count > 0)
                {
                    List<Area> availabeTransition = recursivAreaSearch(startarea);
                    Console.WriteLine(availabeTransition.Count);
                    Area nextTarget = availabeTransition[seed.Next(availabeTransition.Count)];
                    Area newEntrance;
                    if (availabeTransition.Count > toBePlaced.Count)
                    {
                        Console.WriteLine("Something went wrong");
                        foreach (Area area in availabeTransition)
                        {
                            Console.WriteLine($"{area.Name} Available");
                        }
                        foreach (Area area in toBePlaced)
                        {
                            Console.WriteLine($"{area.Name} leftover");
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
                TeviSettings.customFlags[(CustomFlags.TeleporterRando)] = true;
                startarea.addConnection(areas.Find(x => x.Name == "TeleportHub"), "");
                foreach (Area area in transitions)
                {
                    area.Connections[0].to = null;
                }
            }



            foreach (var loc in locations)
                if (!loc.Itemname.Contains("EVENT"))
                    loc.newItem = "";
            Dictionary<string, int> ItemPoolTry = new(itemPool);


            //Progression Items
            Dictionary<string, int> progressionItems = new();
            foreach (var key in Enum.GetNames(typeof(Progression_Items)))
            {
                if (ItemPoolTry.ContainsKey(key))
                {
                    progressionItems[(Enum.Parse(typeof(Progression_Items),key).ToString())] = ItemPoolTry[key];
                }
            }
            if (traverseMode == "Teleporter")
            {
                for (int i = 0; i < 37; i++)
                {
                    progressionItems[$"Teleporter {i}"] = 1;
                }
            }
            //Placing Progression Items
            while (progressionItems.Count > 0)
            {

                string item = createItem(progressionItems, seed);

                int slotId = 0;

                if (item.Contains("Teleporter"))
                {
                    slotId = int.Parse(item.Split(' ')[1]);
                }


                progressionItems[item]--;

                if (progressionItems[item] <= 0)
                {
                    progressionItems.Remove(item);
                    if (ItemPoolTry.ContainsKey(item))
                        ItemPoolTry.Remove(item);
                }

                List<Location> locs = AssumedFill.AssumedSearch(startarea, progressionItems);

                var loc = locs[seed.Next(locs.Count)];
                loc.newItem = item;
                loc.newSlotId = slotId;


            }

            var tes = AssumedFill.AssumedSearch(startarea, startItems);
            while (tes.Count > 0)
            {
                var loc = tes[seed.Next(tes.Count)];
                tes.Remove(loc);
                string item = createItem(ItemPoolTry, seed);
                ItemPoolTry[item]--;
                if (ItemPoolTry[item] <= 0)
                    ItemPoolTry.Remove(item);

                loc.newItem = item;
            }

            return validate();
        }

        public async void CreateSeed(System.Random seed)
        {
            if (creating || ArchipelagoInterface.Instance.isConnected) { return; }
            creating = true;
            failed = false;
            UI.UI.seedCreationLoading();
            await Task.Run(() =>
            {
                if (newSeed(seed))
                {
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
                        TeviSettings.transitionData = transitionData;
                        saveSpoilerLog(RandomizerPlugin.__itemData);
                    }
                }
                else
                    failed = true;

                creating = false;
            });
        }
        public void SyncCreateSeed(System.Random seed)
        {
            if (creating || ArchipelagoInterface.Instance.isConnected) { return; }
            failed = false;
            creating = true;
            //seedCreationLoading();

            if (!ArchipelagoInterface.Instance.isConnected && newSeed(seed))
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
                TeviSettings.transitionData = transitionData;
                saveSpoilerLog(RandomizerPlugin.__itemData);
            }
            else
                failed = true;
            creating = false;
        }

        private List<Area> recursivAreaSearch(Area startArea, HashSet<Area> visited = null)
        {
            List<Area> area = new List<Area>();
            if (visited == null) visited = new HashSet<Area>();
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

        Dictionary<string, int> itemList = new Dictionary<string, int>();
        List<Area> areaList;

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

            List<Location> preCheck = AssumedFill.AssumedSearch(areaList[0]);
            itemList = AssumedFill.debugItems;
            if (AssumedFill.debugLocation.Count < locations.Count)
            {
                Console.WriteLine("Not all Location are reachable");
                foreach (Location loc in locations)
                {
                    if (!preCheck.Contains(loc))
                        Console.WriteLine($"Location {loc.Locationname} is not reachable");
                }
                //saveSpoilerLog(GetData());

                return false;
            }

            switch (TeviSettings.goalType)
            {
                case GoalType.AstralGear:
                default:
                    if (itemList.ContainsKey("STACKABLE_COG") && itemList["STACKABLE_COG"] >= TeviSettings.GoMode)
                    {
                        finished = true;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Not Enoguh Astral Gears {itemList["STACKABLE_COG"]}:{TeviSettings.GoMode}");
                        return false;
                    }
                case GoalType.BossDefeat:
                    if (itemList.ContainsKey("EVENT_BOSS") && itemList["EVENT_BOSS"] >= 21)
                        finished = true;
                    return true;
            }

            /*
            if (!itemList.ContainsKey("STACKABLE_COG"))
                return false;
            if (itemList["STACKABLE_COG"] > Math.Floor((float)TeviRandomizerSettings.GoMode / 2f))
            {
                //Console.WriteLine("CheckIn");
                if (areaList.Count != areas.Count)
                {
                    foreach (Area ar in areaList)
                    {
                        bool f = false;
                        foreach (Area a in areas)
                        {
                            if (a.Name == ar.Name) { f = true; break; }
                        }
                        if (!f) { Console.WriteLine($"{ar.Name} Not IN LIST"); }
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

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (!TeviSettings.customFlags[CustomFlags.RandomMoney])
                locations.AddRange(Money);
            if (!TeviSettings.customFlags[CustomFlags.RandomResource])
                locations.AddRange(resources);
            foreach (Location loc in locations)
            {



                string item1 = loc.Locationname;
                string item2 = loc.newItem;
                try
                {
                    if (item2 == TeviSettings.PortalItem.ToString())
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
                    Console.WriteLine($"Already changed {item1}. Dropping {item2}");

                }
            }


            return data;
        }

        static public Dictionary<string, OptionObserver> settings = new Dictionary<string, OptionObserver>();


        public void saveSpoilerLog(Dictionary<string, string> itemData)
        {
            if (!Directory.Exists(TeviSettings.pluginPath + "/Data")) Directory.CreateDirectory(TeviSettings.pluginPath + "/Data");
            DateTime today = DateTime.Now;
            StreamWriter spoilerLog = File.CreateText(TeviSettings.pluginPath + "/Data/" + $"{today.Year}_{today.Month}_{today.Day}_{today.Hour}_{today.Minute}_Spoilerlog.txt");
            spoilerLog.WriteLine("Options:\n");
            foreach (var option in settings)
            {
                spoilerLog.WriteLine($"{option.Key + ":",-50}{option.Value.Value.ToString()}");
            }


            spoilerLog.WriteLine("\n\n\n\nSpheres:\n");

            Area startarea = areas.Find(x => x.Name == "Thanatara Canyon");
            var spheres = AssumedFill.SphereSearch(startarea);

            int currHint = 0;
            int currBackHint = 1;

            for (int i = 0; i < spheres.Count; i++)
            {
                spoilerLog.WriteLine($"{i + 1}: {{");
                foreach (var loc in spheres[i])
                {
                    if (Enum.TryParse<Progression_Items>(loc.newItem, out _) || loc.newItem.Contains("Teleporter"))
                    {
                        string item = APItemNames.ContainsKey(loc.newItem) ? APItemNames[loc.newItem] : loc.newItem;
                        spoilerLog.WriteLine($"    {loc.Locationname} => {item}");
                    }
                    if (Enum.IsDefined(typeof(MajorItemFlag), loc.newItem) && currHint < ChatSystemPatch.numberOfHints)
                    {
                        if (TeviSettings.customFlags[CustomFlags.NormalItemCraft] && loc.Loaction.Contains("Upgrade") && currBackHint + currHint < ChatSystemPatch.numberOfHints)
                        {

                            ChatSystemPatch.hintList[ChatSystemPatch.hintList.Length - currBackHint] = (loc.Locationname, loc.newItem, (byte)loc.newSlotId);
                            currBackHint++;
                        }
                        else
                        {
                            ChatSystemPatch.hintList[currHint] = (loc.Locationname, loc.newItem, (byte)loc.newSlotId);
                            currHint++;
                        }
                    }
                }
                spoilerLog.WriteLine("}");
            }


            spoilerLog.WriteLine("\n\n\n\nLocations:\n");
            foreach (KeyValuePair<string, string> item in itemData)
            {


                string item2 = APItemNames.ContainsKey(item.Value) ? APItemNames[item.Value] : item.Value;



                spoilerLog.WriteLine($"{item.Key} => {item2}");
                
            }
            spoilerLog.WriteLine("\n\n\n\nTransitions:\n");
            if (TeviSettings.transitionData != null)
            {
                foreach (var entry in TeviSettings.transitionData)
                {
                    if (transitionIdToName != null)
                        spoilerLog.WriteLine($"{transitionIdToName[entry.Key]} -> {transitionIdToName[entry.Value]}");
                }
            }
            spoilerLog.Close();
        }
    }
}
