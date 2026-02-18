using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TeviRandomizer.TeviRandomizerSettings;

/*
 * Things missing:
 * () Automatic reconnect after loosing connection
 * () Send all Locations once after a disconnect
 * () Disconnect from a server
 */

namespace TeviRandomizer
{


    public class ArchipelagoInterface : MonoBehaviour
    {
        public const ItemList.Type remoteItem = ItemList.Type.I10;
        public const ItemList.Type remoteItemProgressive = ItemList.Type.I11;
        
        public const string AP_WORLD_VERSION = "0.6.7";
        public string connectedVersion = "";
        public const string ConnectionLost = "APLost";

        private class LocationData
        {
            public string item { get; set; }
            public long player { get; set; }
            public bool progressive { get; set; }
            public long id { get; set; }
        }

        public static ArchipelagoInterface Instance = null;
        public string connectVersion;
        private ArchipelagoSession session = null;
        private string uri, user, password;
        private int port;
        private long player;
        private LoginResult loginResult = null;
        public bool isConnected = false;
        public bool isSynced = false;
        private DeathLinkService deathLink = null;
        private bool deathLinkTriggered = false;
        private bool lostConnection = false;
        private JObject APNameToTevi;
        public JObject TeviToAPName;
        private const long baseID = 44966541000;
        private Dictionary<string, LocationData> locations = new Dictionary<string, LocationData>();
        private Dictionary<int, int> transitionData = new Dictionary<int, int>();
        public int currentItemNR = 0;
        private Dictionary<long, string> PlayerNames = new Dictionary<long, string>();
        public bool connectToRoom(string uri, int port, string user, string password = null)
        {
            if (session != null && session.Socket.Connected)
            {
                session.Socket.DisconnectAsync();
                isConnected = false;
            }


            session = ArchipelagoSessionFactory.CreateSession(uri, port);
            try
            {
                loginResult = session.TryConnectAndLogin("Tevi", user, ItemsHandlingFlags.IncludeStartingInventory, password: password);
            }
            catch (Exception e)
            {
                loginResult = new LoginFailure(e.GetBaseException().Message);
            }

            if (!loginResult.Successful)
            {
                LoginFailure failure = (LoginFailure)loginResult;
                string errorMessage = $"Failed to Connect as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                return false; // Did not connect, show the user the contents of `errorMessage`
            }

            LoginSuccessful success = (LoginSuccessful)loginResult;
            Debug.Log(success.SlotData);

            if (success.SlotData.ContainsKey("version"))
            {
                if((string)success.SlotData["version"] != AP_WORLD_VERSION)
                    Debug.LogWarning($"AP World version: {(string)success.SlotData["version"]} does not match with Client \n Expected: {AP_WORLD_VERSION}");
            }
            else
            {
                Debug.LogWarning($"AP World version: not existence");
            }
            this.uri = uri;
            this.port = port;
            this.user = user;
            this.password = password;
            this.player = session.ConnectionInfo.Slot;
            this.isConnected = true;
            this.isSynced = false;
            this.currentItemNR = 0;
            if ((bool)Randomizer.settings["DeathLink"].Value)
                enableDeathLink();
            lostConnection = false;
            PlayerNames.Clear();
            foreach(var info in session.Players.AllPlayers)
            {
                PlayerNames.Add(info.Slot, info.Alias);
            }

            if (success.SlotData.ContainsKey("options"))
                setCustomFlags((JObject)success.SlotData["options"]);
            else
                oldSlotData(success.SlotData);
            if(success.SlotData.ContainsKey("version"))
                connectVersion = (string)success.SlotData["version"];
            getOwnLocationData().Wait();
            getOwnTransitionData(success.SlotData["transitionData"]);
            connectedVersion = (string)success.SlotData["version"];
            UI.UI.checkApWorldLocationCheck = true;
            return true;
        }


        private void setCustomFlags(JObject optionData)
        {
            Debug.Log(optionData);
            LocationData locationData = new LocationData();


            if(optionData.ContainsKey("free_MATK"))
                TeviSettings.extraPotions[(int)FreePot.Melee] = (int)(long)optionData["free_MATK"];

            if(optionData.ContainsKey("free_RATK"))
                TeviSettings.extraPotions[(int)FreePot.Range] = (int)(long)optionData["free_RATK"];

            if(optionData.ContainsKey("free_HP"))
                TeviSettings.extraPotions[(int)FreePot.HP] = (int)(long)optionData["free_HP"];

            if(optionData.ContainsKey("free_MP"))
                TeviSettings.extraPotions[(int)FreePot.Mana] = (int)(long)optionData["free_MP"];

            if(optionData.ContainsKey("free_EP"))
                TeviSettings.extraPotions[(int)FreePot.EP] = (int)(long)optionData["free_EP"];


            TeviSettings.customFlags[CustomFlags.TempOption] = (bool)optionData.GetValue("open_morose");
            TeviSettings.customFlags[CustomFlags.CebleStart] = (bool)optionData.GetValue("celia_sable");
            TeviSettings.customFlags[CustomFlags.SuperBosses] = (bool)optionData.GetValue("superBosses");
            TeviSettings.GoMode = (int)(long)optionData.GetValue("goal_count");
            if (optionData.ContainsKey("traverse_mode"))
            {
                TeviSettings.customFlags[CustomFlags.TeleporterRando] = (int)optionData.GetValue("traverse_mode") == 2;

            }
            if (optionData.ContainsKey("goal_type"))
            {
                switch ((int)optionData.GetValue("goal_type"))
                {
                    case 1:
                        TeviSettings.goalType = GoalType.BossDefeat;
                        break;
                    case 0:
                    default:
                        TeviSettings.goalType = GoalType.AstralGear;
                        break;
                }
            }
            else
                TeviSettings.goalType = GoalType.AstralGear;

            //depcriated
            if (optionData.ContainsKey("free_attack_up"))
            {
                long extraPotions = (long)optionData.GetValue("free_attack_up");
                TeviSettings.extraPotions = [(int)extraPotions, (int)extraPotions];
            }
            if (optionData.ContainsKey("teleporter_mode"))
                TeviSettings.customFlags[CustomFlags.TeleporterRando] = (bool)optionData.GetValue("teleporter_mode");

        }
        private void oldSlotData(Dictionary<string,object> SlotData)
        {
            
            long extraPotions = (long)SlotData["attackMode"];
            TeviSettings.extraPotions = [(int)extraPotions, (int)extraPotions];
            TeviSettings.customFlags[CustomFlags.TempOption] = (long)SlotData["openMorose"] > 0;
            TeviSettings.customFlags[CustomFlags.CebleStart] = (long)SlotData["CeliaSable"] > 0;
            TeviSettings.GoMode = (int)(long)SlotData["GoalCount"];
        }
        private void toggleDeathLink()
        {
            if (deathLink == null)
            {
                deathLink = session.CreateDeathLinkService();
                deathLink.EnableDeathLink();
                deathLink.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    deathLinkTriggered = true;
                };
            }
            else
            {
                deathLink.DisableDeathLink();
                deathLink = null;
            }
        }
        private void enableDeathLink()
        {
            deathLink = session.CreateDeathLinkService();
            deathLink.EnableDeathLink();
            deathLink.OnDeathLinkReceived += (deathLinkObject) =>
            {
                deathLinkTriggered = true;
            };            
        }
        private void disableDeathLink()
        {
            if (deathLink != null)
            {
                deathLink.DisableDeathLink();
                deathLink = null;
            }
        }

        public void disconnect()
        {
            if (session?.Socket?.DisconnectAsync() != null)
            {
                this.isConnected = false;
            }

        }

        public string causeOfDeath = "Rabbit Sleepy";
        public void triggerDeathLink()
        {
            if (deathLink == null) return;
            if (!deathLinkTriggered)
            {
                deathLink.SendDeathLink(new DeathLink(user, causeOfDeath));
                causeOfDeath = "Rabbit Sleepy";
            }
            deathLinkTriggered = false;
        }
        public bool isDeathLinkTriggered()
        {
            return deathLinkTriggered;
        }

        public void updateTransitionVisited(int[] transitionList)
        {
            if (isConnected)
            {
                session.DataStorage[Scope.Slot, "transitionVisited"] = transitionList;
            }
        }
        public void updateCurretMap(int map)
        {
            if (isConnected)
            {
                session.DataStorage[Scope.Slot, "currentMap"] = map;
            }
        }
        public async Task getOwnLocationData() {
            locations.Clear();
            long[] locs = session.Locations.AllLocations.ToArray();
            var a = session.Locations.ScoutLocationsAsync(locs).Result;
            foreach (long loc in locs)
            {
                if (!a.ContainsKey(loc))
                {
                    Debug.Log("==SAD==");
                    continue;
                }
                LocationData locationData = new LocationData();
                if (APNameToTevi.ContainsKey(a[loc].ItemName))
                {
                    locationData.item = (string)APNameToTevi[a[loc].ItemName];
                }
                else
                    locationData.item = a[loc].ItemName;
                locationData.player = a[loc].Player;
                locationData.progressive = (a[loc].Flags & ItemFlags.Advancement) != 0;
                locationData.id = loc;
                locations.Add(session.Locations.GetLocationNameFromId(loc,"Tevi"), locationData);
            }
            storeData();
        }
        public void getOwnTransitionData(object slotData) {
            transitionData.Clear();
            foreach (JObject item in (JArray)slotData) {
                transitionData.Add((int)item.GetValue("from"), (int)item.GetValue("to"));
            }
        }
        public void sendMessage(string msg)
        {
            if (isConnected)
            {
                session.Say(msg);
            }
        }
        public string[] getApLocationNames()
        {
            return locations.Keys.ToArray();
        }
        public bool checkoutLocation(string location)
        {
            if (this.isConnected)
            {
                long id;
                if (locations.ContainsKey(location))
                {
                    id = locations[location].id;
                    session.Locations.CompleteLocationChecks(id);
                }
                else
                {
                    Debug.LogError("location not found in Location Dictionary");
                    return false;
                }
                return true;
            }
            return false;
        }

        public void sendGOAL()
        {
            if (this.isConnected)
            {
                var statusUpdatePacket = new StatusUpdatePacket();
                statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
                session.Socket.SendPacket(statusUpdatePacket);
            }
        }

        public void storeData()
        {
            RandomizerPlugin.__itemData.Clear();
            foreach (var entry in locations)
            {
                string item = entry.Value.item;
                
                if (entry.Value.player != player)
                {
                    item = "Remote";
                }
                RandomizerPlugin.__itemData.Add(entry.Key,item);
            }
            TeviSettings.transitionData = transitionData;
        }


        public bool isItemNative(string Location)
        {
            if(locations.ContainsKey(Location))
                return locations[Location].player == this.player;
            return true;
        }
        public bool isItemNative(ItemList.Type item,byte slot) => isItemNative(LocationTracker.APLocationName[$"{item} #{slot}"]);

        public bool isItemProgessive(string Location)
        {
            if (locations.ContainsKey(Location))
                return locations[Location].progressive;
            return false;
        }
        public bool isItemProgessive(ItemList.Type item, byte slot) => isItemProgessive(LocationTracker.APLocationName[$"{item} #{slot}"]);
        public void announceScoutedLocation(ItemList.Type item, byte slot)
        {
            
            string location = LocationTracker.APLocationName[$"{item} #{slot}"];
            if (isItemNative(location) && !isItemProgessive(location)) return;
            long locationID = session.Locations.GetLocationIdFromName("Tevi", location);
            session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, locationID);
        }

        public string getLocItemName(string Location) => locations.ContainsKey(Location) ? locations[Location].item : "LOCATION NOT FOUND";
        public string getLocItemName(ItemList.Type item,byte slot) => getLocItemName(LocationTracker.APLocationName[$"{item} #{slot}"]);

        public string getLocPlayerName(string Location) => locations.ContainsKey(Location) ? PlayerNames[locations[Location].player] : "Yourself";
        public string getLocPlayerName(ItemList.Type item, byte slot) => getLocPlayerName(LocationTracker.APLocationName[$"{item} #{slot}"]);

        void Awake()
        {
            string path = TeviSettings.pluginPath + "/resource/";

            TeviToAPName = JObject.Parse(File.ReadAllText(path+ "ItemToReal.json"));
            APNameToTevi = new JObject();
            foreach (var item in TeviToAPName)
            {
                APNameToTevi.Add((string)item.Value, item.Key);
            }
        }
        void Update()
        {
            if (WorldManager.Instance != null && !EventManager.Instance.IsChangingMap() && WorldManager.Instance.MapInited && GemaUIPauseMenu.Instance.GetAllowPause() && !GameSystem.Instance.isAnyPause() && (EventManager.Instance.getMode() == EventMode.Mode.OFF || EventManager.Instance.EventTime > 300f))

                if (lostConnection)
                {
                ChatSystemPatch.addNewChatLine("a", "Lost connection To the AP Server");
                ChatSystemPatch.startChat(ConnectionLost);
                lostConnection = false;
                }

            if (session?.Socket?.Connected != true)
            {
                if (isConnected)
                {
                    lostConnection = true;
                }
                isConnected = false;
                return;
            }

            if (WorldManager.Instance != null && !EventManager.Instance.IsChangingMap() && WorldManager.Instance.MapInited && GemaUIPauseMenu.Instance.GetAllowPause() && !GameSystem.Instance.isAnyPause() && (EventManager.Instance.getMode() == EventMode.Mode.OFF || EventManager.Instance.EventTime > 300f))
            {
                if (deathLinkTriggered)
                {
                    GameObject.FindGameObjectWithTag("MainCharacter")?.GetComponent<playerController>()?.ReduceHealth(int.MaxValue, true);
                }

                ItemList.Type teviItem;
                if(currentItemNR < session.Items.AllItemsReceived.Count)
                {
                    ItemInfo item = session.Items.AllItemsReceived[currentItemNR];
                    byte value = 1;
                    int itemID = (int)(item.ItemId - baseID);
                    if(itemID >= 500 && itemID <= 536)
                    {
                        value = (byte)(itemID - 500);
                        itemID = (byte)TeviSettings.PortalItem;
                    }
                    teviItem = (ItemList.Type)itemID;
                    var em = EventManager.Instance;
                    ItemDistributionSystem.EnqueueItem(new(teviItem,value,true));
                    currentItemNR++;
                }

            }
        }
    }
    
}
