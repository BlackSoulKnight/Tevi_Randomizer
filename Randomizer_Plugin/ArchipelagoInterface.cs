using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks.Ugc;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

namespace TeviRandomizer
{


    class ArchipelagoInterface : MonoBehaviour
    {
        public const ItemList.Type remoteItem = ItemList.Type.I10;
        public const ItemList.Type remoteItemProgressive = ItemList.Type.I10;


        private class LocationData
        {
            public string item { get; set; }
            public long player { get; set; }
            public bool progressive { get; set; }
            public long id { get; set; }
        }

        public static ArchipelagoInterface Instance = null;

        private ArchipelagoSession session = null;
        private string uri, user;
        private int port;
        private long player;
        private LoginResult loginResult = null;
        public bool isConnected = false;

        private const long baseID = 44965410000;
        private Dictionary<string, LocationData> locations = new Dictionary<string, LocationData>();
        public int currentItemNR = 0;
        private int sessionsItemNR = 0;

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
                loginResult = session.TryConnectAndLogin("Tevi", user, ItemsHandlingFlags.RemoteItems, version: Version.Parse("0.5.0"), password: password);
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
            this.player = session.ConnectionInfo.Slot;
            this.isConnected = true;
            sessionsItemNR = 0;

            session.Items.ItemReceived += this.addItemToQueue;
            long extraPotions = (long)success.SlotData["attackMode"];
            RandomizerPlugin.extraPotions = [(int)extraPotions,(int)extraPotions];
            RandomizerPlugin.customFlags[(int)CustomFlags.TempOption] = (long)success.SlotData["openMorose"] >0;
            getOwnLocationData(success.SlotData["locationData"]);
            storeData();
            return true;
        }


        public void refreshRecievedItems()
        {
            if (this.isConnected)
            {
                sessionsItemNR = Math.Min(sessionsItemNR, session.Items.AllItemsReceived.Count);
                for (;  sessionsItemNR > currentItemNR; sessionsItemNR--)
                {
                    ItemInfo item = session.Items.AllItemsReceived[sessionsItemNR-1];
                    newItems.Insert(0,item);
                }
            }
        }

        public void getOwnLocationData(object slotData) {
            locations.Clear();
            foreach (JObject item in (JArray)slotData) {
                LocationData locationData = new LocationData();
                locationData.item = (string)item.GetValue("item");
                locationData.player = (int)item.GetValue("player");
                locationData.progressive = (bool)item.GetValue("progressive");
                locationData.id = session.Locations.GetLocationIdFromName("Tevi", (string)item.GetValue("location"));
                
                locations.Add((string)item.GetValue("location"),locationData);
            }
        }

        public void addItemToQueue(ReceivedItemsHelper recievedItemsHelper)
        {

            var item = recievedItemsHelper.PeekItem();
            if (!newItems.Contains(item))
            {
                newItems.Add(item);
            }
            recievedItemsHelper.DequeueItem();

        }

        private List<ItemInfo> newItems = new List<ItemInfo>();

        public void checkoutLocation(string location)
        {
            long id;
            try
            {
                id = locations[location].id;
            }
            catch 
            {
                Debug.LogError("location not found in Location Dictionary");
                return;
            }
            session.Locations.CompleteLocationChecks(id);
        }

        public void sendGOAL()
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            session.Socket.SendPacket(statusUpdatePacket);
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
            return false;
        }
        public bool isItemNative(ItemList.Type item,byte slot) => isItemNative($"{item} #{slot}");

        public bool isItemProgessive(string Location)
        {
            if (locations.ContainsKey(Location))
                return locations[Location].progressive;
            return false;
        }
        public bool isItemProgessive(ItemList.Type item, byte slot) => isItemProgessive($"{item} #{slot}");




        void Update()
        {
            if (newItems.Count > 0 && WorldManager.Instance != null && !EventManager.Instance.IsChangingMap() && WorldManager.Instance.MapInited && !HUDObtainedItem.Instance.isDisplaying())
            {
                ItemList.Type teviItem;
                ItemInfo item = newItems[0];
                if (!Enum.TryParse(item.ItemName, out teviItem))
                {
                    Debug.LogWarning($"[Archipelago] Recieved Item {item.ItemName} is not a Tevi Item");
                    newItems.Remove(item);
                    return;
                }

                if (sessionsItemNR < currentItemNR)
                {
                    sessionsItemNR++;
                    newItems.Remove(item);
                    return;
                }
                if (currentItemNR < sessionsItemNR)
                {
                    currentItemNR++;
                    newItems.Remove(item);
                    SaveManager.Instance.SetItem(teviItem, 1);
                    return;
                }

                //using setitem for debugging
                SaveManager.Instance.SetItem(teviItem, 1);
                //HUDObtainedItem.Instance.GiveItem(teviItem, 1, true);
                sessionsItemNR++;
                currentItemNR++;
                newItems.Remove(item);

            }
        }
    }
    
}
