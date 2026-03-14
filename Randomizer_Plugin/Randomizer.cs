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
        private int IncreaseItemQuantity(string item, int amount = 1) => ChangeItemQuantity(item, amount); 
        private int DecreaseItemQuantity(string item, int amount = 1) => ChangeItemQuantity(item, -amount);
        private void SetItemQuantity(string item, int amount) {
            if (TeviSettings.ProgressionItems.Contains(item))
                ProgressionPool[item] = amount;
            else
                ItemPool[item] = amount; 
        }
        private bool RemoveItemFromPool(string item) => ChangeItemQuantity(item,deleteItem:true) <0?false:true;
        private int ChangeItemQuantity(string item, int amount = 1, bool deleteItem = false)
        {
            var pool = TeviSettings.ProgressionItems.Contains(item)?ProgressionPool:ItemPool;

            if (pool.ContainsKey(item))
            {
                if (deleteItem)
                {
                    pool.Remove(item);
                    return 0;
                }

                if (pool[item] + amount < 0)
                {
                    amount += pool[item];
                    pool.Remove(item);
                    return amount;
                }
                else if (pool[item] == -amount)
                    pool.Remove(item);
                else
                    pool[item] += amount;
                return 0;
            }
            else if (amount > 0)
            {
                pool[item] = amount;
            }
            return -1;
        }

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
                                    string loc = "Thanatara Canyon - Dagger";
                                    Locations[loc].setNewItem(ItemList.Type.ITEM_KNIFE.ToString());
                                    DecreaseItemQuantity(ItemList.Type.ITEM_KNIFE.ToString());
                                }
                                break;
                            case "Orb":
                                if (isOn)
                                {
                                    string loc = "Thanatara Canyon - Orbitars";
                                    Locations[loc].setNewItem(ItemList.Type.ITEM_ORB.ToString());
                                    DecreaseItemQuantity(ItemList.Type.ITEM_ORB.ToString());
                                }
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
                                        string loc = $"Item Upgrade - {TeviSettings.TeviToDisplayName(item.ToString())} #1";
                                        string loc2 = $"Item Upgrade - {TeviSettings.TeviToDisplayName(item.ToString())} #2";
                                        ItemList.Type t = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item.ToString());

                                        Locations[loc].setNewItem(t.ToString());
                                        Locations[loc2].setNewItem(t.ToString());
                                        DecreaseItemQuantity(t.ToString(),2);
                                    }
                                }
                                break;
                            case "Ceble":
                                TeviSettings.customFlags[CustomFlags.CebleStart] = isOn;
                                if (isOn)
                                {
                                    DecreaseItemQuantity(RandomizerPlugin.CeliaItem.ToString());
                                    DecreaseItemQuantity(RandomizerPlugin.SableItem.ToString());
                                    StartItems[RandomizerPlugin.SableItem.ToString()] = 1;
                                    StartItems[RandomizerPlugin.CeliaItem.ToString()] = 1;
                                }
                                break;
                            case "SuperBosses":
                                if (!isOn)
                                {
                                    string loc = "Ana Thema - GemaYue";
                                    Locations[loc].setNewItem(ItemList.Type.STACKABLE_COG.ToString());
                                    
                                    loc = "Ana Thema - EinLee";
                                    Locations[loc].setNewItem(ItemList.Type.STACKABLE_COG.ToString());

                                    loc = "Ana Thema - Waero";
                                    Locations[loc].setNewItem(ItemList.Type.STACKABLE_COG.ToString());
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
                                {
                                    foreach (var loc in MoneyLocations)
                                        loc.newItem = loc.Itemname;
                                    RemoveItemFromPool(TeviSettings.MoneyItem.ToString());
                                }
                                break;
                            case "RandomResource":
                                TeviSettings.customFlags[CustomFlags.RandomResource] = isOn;
                                if (!isOn)
                                {
                                    foreach (var loc in ResourceLocations)
                                        loc.newItem = loc.Itemname;
                                    RemoveItemFromPool(TeviSettings.CoreUpgradeItem.ToString());
                                    RemoveItemFromPool(TeviSettings.ItemUpgradeItem.ToString());
                                }
                                break;
                            default: break;
                        }
                        break;
                    case "Slider":
                        int value = (int)option.Value;
                        switch (option.Name)
                        {
                            case "GearReq":
                                TeviSettings.GoMode = Math.Max(Math.Min(value, 24), 1);
                                break;
                            case "MananiteAmount":
                                SetItemQuantity(TeviSettings.ItemUpgradeItem.ToString(),value);
                                break;
                            case "MagititeAmount":
                                SetItemQuantity(TeviSettings.CoreUpgradeItem.ToString(),value);
                                break;
                            case "GearMax":
                                int max = Math.Max(value, TeviSettings.GoMode);
                                SetItemQuantity(ItemList.Type.STACKABLE_COG.ToString(), max);
                                break;
                            case "TrapAmount":
                                TrapsInPercent = value;
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
                                switch (selected)
                                {
                                    case "Teleporter":
                                        StartArea.addConnection(Areas["TeleportHub"]);
                                        foreach (Area area in Transitions)
                                        {
                                            area.Connections[0].to = null;
                                        }
                                        for (int i = 0; i < 37; i++)
                                        {
                                            ProgressionPool[$"Teleporter {i}"] = 1;
                                        }
                                        break;
                                    case "Entrance":
                                            foreach (Area area in Transitions)
                                            {
                                                area.Connections[0].to = null;
                                            }
                                            List<Area> toBePlaced = new(Transitions);
                                            while (toBePlaced.Count > 0)
                                            {
                                                List<Area> availabeTransition = recursivAreaSearch(StartArea);
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
                                        break;
                                    default:
                                        break;
                                }

                                TeviSettings.customFlags[CustomFlags.TeleporterRando] = selected == "Teleporter";
                                TeviSettings.customFlags[CustomFlags.EntranceRando] = selected == "Entrance";
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            foreach (var item in ItemPool.ToArray())
            {
                if (item.Value <= 0)
                    ItemPool.Remove(item.Key);
            }
        }



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
                    if(!flag)
                        flag = LogicHelper(itemList, "Goal");
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
                            case "ITEM_OrbTypeC2":
                            case "ITEM_OrbTypeC3":
                                flag = itemList.ContainsKey("I19");
                                break;
                            case "ITEM_OrbTypeS2":
                            case "ITEM_OrbTypeS3":
                                flag = itemList.ContainsKey("I20");
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
        public class Location
        {


            public string Itemname;
            public string ParentArea;
            public string Locationname;
            public string newItem;
            public List<Requirement> Requirement;
            public Location(int itemId, string loaction, string locationName, int slotId, string itemname = "")
            {
                Itemname = itemname;
                ParentArea = loaction;
                this.Locationname = locationName;
                this.Requirement = new List<Requirement>();
                this.newItem = "";
            }
            public bool isReachAble(Dictionary<string, int> itemList)
            {
                foreach (Requirement req in Requirement)
                {
                    if (req.evaluate(itemList)) return true;
                }
                return false;
            }
            public void setNewItem(string item)
            {
                newItem = item;
            }
            public void addRule(string rule)
            {
                Requirement.Add(new Requirement(rule));
            }
            public override string ToString() => Locationname;

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
            public void addConnection(Area to, string method = "")
            {
                this.Connections.Add(new Entrance(this, to, method));
            }
            public override string ToString() => Name;

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
        private Dictionary<string, int> ItemPool = new();
        private Dictionary<string, int> ProgressionPool = new();
        private Area StartArea;
        public bool Finished = false;
        private Dictionary<string,Location> Locations = new();
        private Dictionary<string,Area> Areas = new();
        private static Dictionary<int, string> TransitionIdToName = null;
        private List<Area> Transitions = new List<Area>();
        private List<Location> ResourceLocations = new List<Location>();
        private List<Location> MoneyLocations = new List<Location>();
        private string EventPrefix = "EVENT_";
        private HashSet<Location> ignoreLocationList = new HashSet<Location>();
        private int TrapsInPercent = 0;
        public Randomizer()
        {

            string resourcePath = TeviSettings.pluginPath + "/resource/";
            ItemPool = new Dictionary<string, int>();
            JObject areaJson = JObject.Parse(File.ReadAllText(resourcePath + "Area.json"));
            foreach (var map in areaJson)
            {
                foreach (var area in (JArray)map.Value)
                {
                    Area newArea = new Area((string)area["Name"]);
                    if (int.TryParse((string)area["Name"], out _))
                    {
                        Transitions.Add(newArea);
                    }
                    Areas[newArea.ToString()] = newArea;
                }
            }
            foreach (var map in areaJson)
            {
                foreach (var area in (JArray)map.Value)
                {
                    foreach (var con in area["Connections"])
                    {
                        if( Areas.ContainsKey((string)con["Exit"]))
                            Areas[(string)area["Name"]].addConnection(Areas[(string)con["Exit"]], (string)con["Method"]);
                    }
                }
            }
            if (TransitionIdToName == null)
            {
                TransitionIdToName = new Dictionary<int, string>();
                foreach (string line in File.ReadLines(resourcePath + "TransitionId.txt"))
                {
                    string[] para = line.Split(':');
                    if (para.Count() == 2)
                        TransitionIdToName.Add(int.Parse(para[0]), para[1]);
                }
            }

            JArray locationJson = JArray.Parse(File.ReadAllText(resourcePath + "Location.json"));


            foreach (var location in locationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["slotId"], location["Itemname"].ToString());
                if (location["Itemname"].ToString().Contains(EventPrefix))
                {
                    newloc.newItem = (string)location["Itemname"];
                }
                if (Enum.TryParse(location["Itemname"].ToString(), out ItemList.Type item))
                {
                    IncreaseItemQuantity(item.ToString());
                }
                foreach (var method in location["Requirement"])
                {
                    newloc.addRule(method["Method"].ToString());
                }
                if (Areas.ContainsKey(newloc.ParentArea))
                {
                    Locations[newloc.ToString()] = newloc;
                    Areas[newloc.ParentArea].Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.ParentArea} to place {newloc.Itemname}");
                }
            }

            IncreaseItemQuantity(ItemList.Type.QUEST_Memory.ToString());
            IncreaseItemQuantity(ItemList.Type.QUEST_Flute.ToString());
            IncreaseItemQuantity(ItemList.Type.QUEST_Compass.ToString());

            JArray moneyLocationJson = JArray.Parse(File.ReadAllText(resourcePath + "MoneyLocations.json"));
            JArray resourceLocationJson = JArray.Parse(File.ReadAllText(resourcePath + "UpgradeResourceLocation.json"));

            foreach (var location in moneyLocationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["area"] * 100_000_000 + (int)location["blockId"], location["Itemname"].ToString());
                if (Enum.TryParse(location["Itemname"].ToString(), out ItemList.Type item))
                {
                    IncreaseItemQuantity(item.ToString());

                }
                foreach (var method in location["Requirement"])
                {
                    newloc.addRule(method["Method"].ToString());
                }

                MoneyLocations.Add(newloc);
                Locations.Add(newloc.Locationname,newloc);
                if (Areas.ContainsKey(newloc.ParentArea))
                {
                    Areas[newloc.ParentArea].Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.ParentArea} to place {newloc.Itemname}");
                }
            }
            foreach (var location in resourceLocationJson)
            {
                Location newloc = new Location(0, location["Location"].ToString(), location["LocationName"].ToString(), (int)location["area"] * 100_000_000 + (int)location["blockId"], location["Itemname"].ToString());
                if (Enum.TryParse(location["Itemname"].ToString(), out ItemList.Type item))
                {
                    IncreaseItemQuantity(item.ToString());
                }

                foreach (var method in location["Requirement"])
                {
                    newloc.addRule(method["Method"].ToString());
                }

                ResourceLocations.Add(newloc);
                Locations.Add(newloc.Locationname, newloc);
                if (Areas.ContainsKey(newloc.ParentArea))
                {
                    Areas[newloc.ParentArea].Locations.Add(newloc);
                }
                else
                {
                    Console.WriteLine($"Could not find {newloc.ParentArea} to place {newloc.Itemname}");
                }
            }
            StartArea = Areas["Thanatara Canyon"];
        }



        public static bool creating = false;
        public static bool failed = false;

        private string createItem(System.Random seed, Dictionary<string, int> itemPool)
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
            {
                item = "PANIC";
                foreach (var a in itemPool)
                    Console.WriteLine($"{a.Key}:{a.Value}");
                Console.WriteLine(itemIndex);

            }
            return item;
        }

        private List<string> createTrapItems(System.Random seed,ref Dictionary<string,int> currentItemPool, int amount = 1) { 
            List<string> items = new List<string>();
            if (amount == 0)
                return items;
            List<TeviItem> itemPool = new(TeviSettings.Traps);
            List<int> cum_weights = new();
            int curr_weight = 0;
            foreach (var item in itemPool)
            {
                curr_weight += item.Weight;
                cum_weights.Add(curr_weight);
            }
            while(amount > 0)
            {
                var val =seed.Next(curr_weight + 1);
                for (int i = 0; i < cum_weights.Count(); i++)
                    if (val < cum_weights[i])
                    {
                        var item = itemPool[i];
                        if (!currentItemPool.ContainsKey(item.Name))
                            currentItemPool[item.Name] = 0;
                        if (item.Max_Quantity > currentItemPool[item.Name])
                        {
                            amount--;
                            currentItemPool[item.Name]++;
                            items.Add(item.Name);
                        }
                        break;
                    }
            }
            
            return items;
        }

        private List<string> createFillerItems(System.Random seed,ref Dictionary<string,int> currentItemPool, int amount = 1) { 
            List<string> items = new List<string>();
            if (amount == 0)
                return items;
            List<TeviItem> itemPool = new(TeviSettings.Filler);
            itemPool.AddRange(TeviSettings.Useful);
            List<int> cum_weights = new();
            int curr_weight = 0;
            foreach (var item in itemPool)
            {
                curr_weight += item.Weight;
                cum_weights.Add(curr_weight);
            }
            int tryCounter = 0;
            int maxTry = 1000;
            while(amount > 0)
            {
                tryCounter++;
                var val =seed.Next(curr_weight + 1);
                for (int i = 0; i < cum_weights.Count(); i++)
                    if (val < cum_weights[i])
                    {
                        var item = itemPool[i];
                        if (!currentItemPool.ContainsKey(item.Name))
                            currentItemPool[item.Name] = 0;
                        if (item.Default_Quantity > currentItemPool[item.Name])
                        {
                            amount--;
                            currentItemPool[item.Name]++;
                            items.Add(item.Name);
                            tryCounter = 0;
                        }
                        else if(tryCounter > maxTry && item.Default_Quantity == 0 && item.Max_Quantity > 1000)
                        {
                            amount--;
                            currentItemPool[item.Name]++;
                            items.Add(item.Name);
                            tryCounter = 0;
                        }
                        break;
                    }
            }
            
            return items;
        }

        Dictionary<string, int> StartItems = new();

        public bool newSeed(Random seed)
        {
            if (seed == null) seed = new System.Random();

            foreach (var loc in Locations)
                if (!loc.Value.Itemname.Contains(EventPrefix))
                    loc.Value.newItem = "";
            StartItems.Clear();
            CustomOptions(seed);                 //Evaluate Options and setup the correct ItemPool

            Dictionary<string, int> progressionItems = new(ProgressionPool);
            Dictionary<string, int> ItemPoolTry = new();

            Dictionary<string, int> combinedList = new(progressionItems);

            foreach (var item in StartItems)
                if (combinedList.ContainsKey(item.Key))
                    combinedList[item.Key] += item.Value;
                else
                    combinedList.Add(item.Key, item.Value);

            var test = AssumedFill.TestReachability(StartArea, [.. Locations.Values], combinedList);
            if (test.Count > 0)
            {
                Console.WriteLine($"Not everything is reachable with all missing {test.Count}");
                foreach (var loc in test)
                    Console.Write(loc.Locationname);
                return false;
            }
                
            //Placing Progression Items
            while (progressionItems.Count > 0)
            {
                string item = createItem(seed, progressionItems);
                progressionItems[item]--;
                combinedList[item]--;
                if (progressionItems[item] <= 0)
                {
                    progressionItems.Remove(item);
                }
                
                if (combinedList[item] <= 0)
                {
                    combinedList.Remove(item);
                }

                List<Location> locs = AssumedFill.AssumedSearch(StartArea, combinedList);
                var loc = locs[seed.Next(locs.Count)];
                loc.newItem = item;
            }

            var tes = AssumedFill.AssumedSearch(StartArea, StartItems);
            int trapAmount = (tes.Count * TrapsInPercent)/100;
            var otherItems = createTrapItems(seed, ref ItemPoolTry, trapAmount);
            otherItems.AddRange(createFillerItems(seed, ref ItemPoolTry, tes.Count - trapAmount));
            for(int i = 0; i < otherItems.Count;i++)
            {
                var loc = tes[seed.Next(tes.Count)];
                tes.Remove(loc);
                loc.newItem = otherItems[i];
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
                        foreach (Area area in Transitions)
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
                foreach (Area area in Transitions)
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
                areaList = [StartArea];
            else
                areaList = [startArea];

            //Check if its beatable

            HashSet<Location> preCheck = new(AssumedFill.AssumedSearch(areaList[0],StartItems));
            itemList = AssumedFill.debugItems;
            if (AssumedFill.debugLocation.Count < Locations.Count)
            {
                Console.WriteLine("Not all Location are reachable");
                foreach (var loc in Locations)
                {
                    if (!preCheck.Contains(loc.Value))
                        Console.WriteLine($"Location {loc.Value.Locationname} is not reachable");
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
                        Finished = true;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Not Enoguh Astral Gears {itemList["STACKABLE_COG"]}:{TeviSettings.GoMode}");
                        return false;
                    }
                case GoalType.BossDefeat:
                    if (itemList.ContainsKey("EVENT_BOSS") && itemList["EVENT_BOSS"] >= 21)
                        Finished = true;
                    return true;
            }
        }

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var loc in Locations)
            {



                string item1 = loc.Value.Locationname;
                string item2 = loc.Value.newItem;
                try
                {
                    if (item2 == TeviSettings.PortalItem.ToString())
                    {
                        Console.WriteLine("This item should not be in this form");
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

            var spheres = AssumedFill.SphereSearch(StartArea);

            int currHint = 0;
            int currBackHint = 1;

            for (int i = 0; i < spheres.Count; i++)
            {
                spoilerLog.WriteLine($"{i + 1}: {{");
                foreach (var loc in spheres[i])
                {
                    if (Enum.TryParse<Progression_Items>(loc.newItem, out _) || loc.newItem.Contains("Teleporter"))
                    {
                        string item = TeviSettings.NametoItem.ContainsKey(loc.newItem) ? TeviSettings.TeviToDisplayName(loc.newItem) : loc.newItem;
                        spoilerLog.WriteLine($"    {loc.Locationname} => {item}");
                    }
                    if (Enum.IsDefined(typeof(MajorItemFlag), loc.newItem) && currHint < ChatSystemPatch.numberOfHints)
                    {
                        if (TeviSettings.customFlags[CustomFlags.NormalItemCraft] && loc.ParentArea.Contains("Upgrade") && currBackHint + currHint < ChatSystemPatch.numberOfHints)
                        {

                            ChatSystemPatch.hintList[ChatSystemPatch.hintList.Length - currBackHint] = (loc.Locationname, TeviSettings.TeviToDisplayName(loc.newItem));
                            currBackHint++;
                        }
                        else
                        {
                            ChatSystemPatch.hintList[currHint] = (loc.Locationname, TeviSettings.TeviToDisplayName(loc.newItem));
                            currHint++;
                        }
                    }
                }
                spoilerLog.WriteLine("}");
            }


            spoilerLog.WriteLine("\n\n\n\nLocations:\n");
            foreach (KeyValuePair<string, string> item in itemData)
            {


                string item2 = TeviSettings.NametoItem.ContainsKey(item.Value) ? TeviSettings.TeviToDisplayName(item.Value) : item.Value;



                spoilerLog.WriteLine($"{item.Key} => {item2}");
                
            }
            spoilerLog.WriteLine("\n\n\n\nTransitions:\n");
            if (TeviSettings.transitionData != null)
            {
                foreach (var entry in TeviSettings.transitionData)
                {
                    if (TransitionIdToName != null)
                        spoilerLog.WriteLine($"{TransitionIdToName[entry.Key]} -> {TransitionIdToName[entry.Value]}");
                }
            }
            spoilerLog.Close();
        }
    }
}
