using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using UnityEngine;


/*
 * Things missing:
 * () Automatic reconnect after loosing connection
 * () Send all Locations once after a disconnect
 * () Disconnect from a server
 * () Teleport To morose Give an ItemCheck?
 */


namespace TeviRandomizer
{


    class ArchipelagoInterface : MonoBehaviour
    {
        public const ItemList.Type remoteItem = ItemList.Type.I10;
        public const ItemList.Type remoteItemProgressive = ItemList.Type.I11;


        private class LocationData
        {
            public string item { get; set; }
            public long player { get; set; }
            public bool progressive { get; set; }
            public long id { get; set; }
        }

        public static ArchipelagoInterface Instance = null;

        private ArchipelagoSession session = null;
        private string uri, user, password;
        private int port;
        private long player;
        private LoginResult loginResult = null;
        public bool isConnected = false;
        public bool isSynced = false;

        private const long baseID = 44965410000;
        private Dictionary<string, LocationData> locations = new Dictionary<string, LocationData>();
        public int currentItemNR = 0;
        private Dictionary<long, string> PlayerNames = new Dictionary<long, string>();
        public bool connectToRoom(string uri, int port, string user, string password = null)
        {
            if (session != null && session.Socket.Connected)
            {
                if (uri == this.uri && port == this.port && user == this.user)
                {
                    return true;
                }
                session.Socket.DisconnectAsync();
                isConnected = false;
            }


            session = ArchipelagoSessionFactory.CreateSession(uri, port);
            try
            {
                loginResult = session.TryConnectAndLogin("Tevi", user, ItemsHandlingFlags.IncludeStartingInventory, version: Version.Parse("0.5.0"), password: password);
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
            this.uri = uri;
            this.port = port;
            this.user = user;
            this.password = password;
            this.player = session.ConnectionInfo.Slot;
            this.isConnected = true;
            this.isSynced = false;
            this.currentItemNR = 0;
            PlayerNames.Clear();
            foreach(var info in session.Players.AllPlayers)
            {
                PlayerNames.Add(info.Slot, info.Alias);
            }

            long extraPotions = (long)success.SlotData["attackMode"];
            RandomizerPlugin.extraPotions = [(int)extraPotions,(int)extraPotions];
            RandomizerPlugin.customFlags[(int)CustomFlags.TempOption] = (long)success.SlotData["openMorose"] >0;
            RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart] = (long)success.SlotData["CeliaSable"] >0;
            RandomizerPlugin.GoMode = (int)(long)success.SlotData["GoalCount"];
            getOwnLocationData(success.SlotData["locationData"]);
            storeData();
            return true;
        }


        public void disconnect()
        {
            if (session?.Socket?.DisconnectAsync() != null)
            {
                this.isConnected = false;
            }

        }



        public void getOwnLocationData(object slotData) {
            locations.Clear();
            foreach (JObject item in (JArray)slotData) {
                LocationData locationData = new LocationData();
                locationData.item = (string)item.GetValue("item");
                locationData.player = (int)item.GetValue("player");
                locationData.progressive = (int)item.GetValue("progressive") > 0;
                locationData.id = session.Locations.GetLocationIdFromName("Tevi", (string)item.GetValue("location"));
                
                locations.Add((string)item.GetValue("location"),locationData);
            }
        }

        private List<ItemInfo> alreadyReceivedOnceItems = new List<ItemInfo>();
        public void refreshRecievedItems(int oldIndex)
        {
            /*
             * From Last item saved to the newest HUD shown Item
             */
            if (this.isConnected)
            {
                for (; oldIndex < currentItemNR; oldIndex++)
                {
                    ItemInfo item = session.Items.AllItemsReceived[oldIndex];
                    alreadyReceivedOnceItems.Add(item);
                }
            }
        }

        public void checkoutLocation(string location)
        {
            if (this.isConnected)
            {
                long id;
                try
                {
                    id = locations[location].id;
                    session.Locations.CompleteLocationChecks(id);
                }
                catch
                {
                    Debug.LogError("location not found in Location Dictionary");
                    return;
                }
            }
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
                if (!Enum.IsDefined(typeof(ItemList.Type), item))
                {
                    item = "Remote";
                }
                RandomizerPlugin.__itemData.Add(entry.Key,item);
            }
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


        public string getLocItemName(string Location) => locations[Location].item;
        public string getLocItemName(ItemList.Type item,byte slot) => getLocItemName(LocationTracker.APLocationName[$"{item} #{slot}"]);

        public string getLocPlayerName(string Location) => PlayerNames[locations[Location].player];
        public string getLocPlayerName(ItemList.Type item, byte slot) => getLocPlayerName(LocationTracker.APLocationName[$"{item} #{slot}"]);


        void Update()
        {
            if (session?.Socket?.Connected != true)
            {
                isConnected = false;
                return;
            }

            if (WorldManager.Instance != null && !EventManager.Instance.IsChangingMap() && WorldManager.Instance.MapInited && !HUDObtainedItem.Instance.isDisplaying() && GemaUIPauseMenu.Instance.GetAllowPause())
            {

                ItemList.Type teviItem;
                if (alreadyReceivedOnceItems.Count > 0)
                {
                    ItemInfo item = alreadyReceivedOnceItems[0];
                    if (!Enum.TryParse(item.ItemName, out teviItem))
                    {
                        Debug.LogWarning($"[Archipelago] Recieved Item {item.ItemName} is not a Tevi Item");
                        alreadyReceivedOnceItems.Remove(item);
                        return;
                    }
                    SaveManager.Instance.SetItem(teviItem, 1);
                    alreadyReceivedOnceItems.Remove(item);
                }
                else if(currentItemNR < session.Items.AllItemsReceived.Count)
                {
                    ItemInfo item = session.Items.AllItemsReceived[currentItemNR];
                    if (!Enum.TryParse(item.ItemName, out teviItem))
                    {
                        Debug.LogWarning($"[Archipelago] Recieved Item {item.ItemName} is not a item that exist in Tevi");
                        return;
                    }
                    HUDObtainedItem.Instance.GiveItem(teviItem, 1, true);
                    currentItemNR++;
                }

            }
        }
    }
    
}
