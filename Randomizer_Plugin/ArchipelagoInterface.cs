﻿using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
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
        private DeathLinkService deathLink = null;
        private bool deathLinkTriggered = false;
        private bool lostConnection = false;

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
            if (((UnityEngine.UI.Toggle)Randomizer.settings["Toggle DeathLink"]).isOn)
                enableDeathLink();
            lostConnection = false;
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
            getOwnTransitionData(success.SlotData["transitionData"]);
            storeData();
            return true;
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
        public void deathLinkTrigger()
        {
            if (deathLink == null) return;
            if (!deathLinkTriggered)
            {
                deathLink.SendDeathLink(new DeathLink(user, causeOfDeath));
                causeOfDeath = "Rabbit Sleepy";
            }
            deathLinkTriggered = false;
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
        public void getOwnTransitionData(object slotData) {
            transitionData.Clear();
            foreach (JObject item in (JArray)slotData) {
                transitionData.Add((int)item.GetValue("from"), (int)item.GetValue("to"));
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
            RandomizerPlugin.transitionData = transitionData;
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

        public string getLocItemName(string Location) => locations[Location].item;
        public string getLocItemName(ItemList.Type item,byte slot) => getLocItemName(LocationTracker.APLocationName[$"{item} #{slot}"]);

        public string getLocPlayerName(string Location) => PlayerNames[locations[Location].player];
        public string getLocPlayerName(ItemList.Type item, byte slot) => getLocPlayerName(LocationTracker.APLocationName[$"{item} #{slot}"]);


        void Update()
        {
            if (WorldManager.Instance != null && !EventManager.Instance.IsChangingMap() && WorldManager.Instance.MapInited && GemaUIPauseMenu.Instance.GetAllowPause() && !GameSystem.Instance.isAnyPause() && (EventManager.Instance.getMode() == EventMode.Mode.OFF || EventManager.Instance.EventTime > 300f))

                if (lostConnection)
                {
                HintSystem.addNewChatLine("a", "Lost connection To the AP Server");
                HintSystem.startChat();
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
                    int itemID = (int)(item.ItemId - baseID);
                    teviItem = (ItemList.Type)itemID;
                    HUDObtainedItem.Instance.GiveItem(teviItem,1, true);
                    currentItemNR++;
                }

            }
        }
    }
    
}
