﻿using EventMode;
using HarmonyLib;
using Map;
using Rewired.ComponentControls.Data;
using Spine;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeviRandomizer
{
    class ItemSystemPatch()
    {
        // Items that need to be handled differently
        static ItemList.Type[] itemExceptions = { RandomizerPlugin.PortalItem };

        //Hotswap item recieved
        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPrefix]
        static bool ObtainItem(ref ItemList.Type type, ref byte value, ref bool doRandomBadge, ref (ItemList.Type, byte) __state)
        {
            string itemName = "";
            string desc;
            ItemList.Type original = type;
            if (!doRandomBadge)
            {
                if(RandomizerPlugin.checkItemGot(type,value))
                    return false;
                LocationTracker.addItemToList(type, value);

                ItemList.Type data = RandomizerPlugin.getRandomizedItem(type, value);

                if (ArchipelagoInterface.Instance.isConnected)
                {
                    if (data == ArchipelagoInterface.remoteItem || data == ArchipelagoInterface.remoteItemProgressive)
                    {

                        itemName = ArchipelagoInterface.Instance.getLocItemName(type, value);
                        string playerName = ArchipelagoInterface.Instance.getLocPlayerName(type, value);
                        desc = $"You found {itemName} for {playerName}";
                        __state = (type,value);

                        ItemList.Type item;
                        if (Enum.TryParse(itemName, out item))
                        {
                            itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item.ToString(), true);

                            desc = "<color=\"red\">"+$"                                                                          <size=200%> FOOL!</color><size=100%>\n\n\n<font-weight=100>{itemName} was stolen by {playerName}";
                        }

                        RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(data), itemName);
                        RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(data), desc);
                    }
                }

                type = data;
                itemName = RandomizerPlugin.__itemData[LocationTracker.APLocationName[$"{original} #{value}"]];

                if (type == RandomizerPlugin.PortalItem)
                {
                    value = byte.Parse(itemName.Split(["Teleporter "], StringSplitOptions.RemoveEmptyEntries)[0]);
                    itemName = (string)ArchipelagoInterface.Instance.TeviToAPName[itemName];
                }
            }
            else
            {
                //Archipelago implementation
                itemName = (string)ArchipelagoInterface.Instance.TeviToAPName[$"Teleporter {value}"];

            }


            if (itemExceptions.Contains(type))
            {
                switch (type)
                {
                    case RandomizerPlugin.PortalItem:
                        desc = $"{itemName} has been Unlocked.";
                        RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(type), itemName);
                        RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(type), desc);
                        SaveManager.Instance.SetStackableItem(type, value, true); // do i need this?
                        TeleporterRando.setTeleporterIcon(value);
                        value = 255;
                        return true;
                }
            }

            value = 1;

            if (type.ToString().Contains("ITEM") || type.ToString().Contains("Useable"))
                value = 255;
            return true;
        }

        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPostfix]
        static void changeSpriteInUI(ref SpriteRenderer ___itemicon,ref (ItemList.Type, byte) __state,ref ItemList.Type type)
        {

            if (type == ArchipelagoInterface.remoteItem || type == ArchipelagoInterface.remoteItemProgressive)
            {
                if (ArchipelagoInterface.Instance.isConnected)
                {
                    string itemName = ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APLocationName[$"{__state.Item1} #{__state.Item2}"]);
                    ItemList.Type item;
                    if (Enum.TryParse(itemName, out item))
                    {
                        ___itemicon.sprite = CommonResource.Instance.GetItem((int)item);
                    }
                }
            }
        }

        // Called everytime when an Item is obtained through any means
        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPrefix]
        static bool setItemAdditionals(ref ItemList.Type item, byte value)
        {
            switch (item)
            {

                case ItemList.Type.ITEM_ORB:

                    RandomizerPlugin.addOrbStatus(1);

                    break;
                default:
                    break;

            }
            if (item.ToString().Contains("Useable"))
            {
                SaveManager.Instance.AddItemToBag(item);
                if (item == ItemList.Type.Useable_WaffleAHoneycloud)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleA, 1);
                }
                if (item == ItemList.Type.Useable_WaffleBMeringue)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleB, 1);
                }
                if (item == ItemList.Type.Useable_WaffleCMorning)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleC, 1);
                }
                if (item == ItemList.Type.Useable_WaffleDJellydrop)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleD, 1);
                }
                if (item == ItemList.Type.Useable_WaffleElueberry)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleE, 1);
                }
                if (item == ItemList.Type.Useable_VenaBombSmall)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaSmall, 1);
                }
                if (item == ItemList.Type.Useable_VenaBombBig)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaBig, 1);
                }
                if (item == ItemList.Type.Useable_VenaBombBunBun)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaBB, 1);
                }
                if (item == ItemList.Type.Useable_VenaBombHealBlock)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaHB, 1);
                }
                if (item == ItemList.Type.Useable_VenaBombDispel)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaD, 1);
                }
                return false;
            }


            if (SaveManager.Instance.GetOrb() == 2 && (SaveManager.Instance.GetItem(ItemList.Type.I19) > 0 || SaveManager.Instance.GetItem(ItemList.Type.I20) > 0 || item == ItemList.Type.I20 || item == ItemList.Type.I19) || (SaveManager.Instance.GetOrb() == 1 && RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart]))
            {
                if((SaveManager.Instance.GetOrb() == 1 && RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart]))
                    RandomizerPlugin.addOrbStatus(1);
                RandomizerPlugin.addOrbStatus(1);

            }

            return true;
        }


        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPrefix]
        static bool ItemChanges(ref ItemList.Type item, ref byte value, ref SaveManager __instance)
        {
            if (itemExceptions.Contains(item))
            {
                switch (item)
                {
                    case RandomizerPlugin.PortalItem:
                        return false;
                }
            }

            if(ArchipelagoInterface.Instance.isConnected)
            {
                if(item == ArchipelagoInterface.remoteItem || item == ArchipelagoInterface.remoteItemProgressive)
                {
                    return false;
                }
            }

            if (item >= ItemList.Type.BADGE_START && item <= ItemList.Type.BADGE_MAX)
            {
                if(SaveManager.Instance.GetMiniFlag(Mini.UnlockedBadge) <= 0)
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedBadge, 1);

            }
            if (item.ToString().Contains("ITEM"))
            {
                if(value == 0)
                {
                    return true;
                }
                value = SaveManager.Instance.GetItem(item);
                value = (byte)Math.Min(value + 1, byte.MaxValue);
                if (item == ItemList.Type.ITEM_BoostSystem)
                {
                    if (SaveManager.Instance.GetOrb() == 3)
                    {
                        SaveManager.Instance.SetOrb(4);
                    }
                    Debug.Log($"[SaveManager] Set Item {item} from {SaveManager.Instance.savedata.itemflag[(int)item]} to {value} | ITEM ID : {(int)item}");
                    SaveManager.Instance.savedata.itemflag[(int)item] = value;
                    SaveManager.Instance.RenewGetTotalItemCompletePercent();
                    SaveManager.Instance.SetMiniFlag(Mini.CanDropCrystal, 1);
                    SaveManager.Instance.GiveMaxCrystal();
                    return false;
                }
            }
            else if (item.ToString().Contains("STACKABLE"))
            {
                SaveManager.Instance.SetStackableItem(item, value,true);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPostfix]
        static void dynamicOrbChange(ref ItemList.Type item)
        {
            if (item == ItemList.Type.I19 || item == ItemList.Type.I20) EventManager.Instance.ReloadOrbStatus();
        }

        [HarmonyPatch(typeof(SaveManager),"SetStackableItem")]
        [HarmonyPrefix]
        static bool StackableItemChanges(ref ItemList.Type item,ref byte id,ref SaveManager __instance,ref float ___renewTimer_Item)
        {
            // execptions
            if (itemExceptions.Contains(item))
            {
                return true;
            }
            bool value = true;
            if (id == 0)
            {
                value = false;
                __instance.savedata.stackableCount[(int)(item - 1)] = 0;
            }
            else
            {
                byte val = (byte)Math.Min(__instance.savedata.stackableCount[(int)(item - 1)] + 1, byte.MaxValue);
                __instance.savedata.stackableCount[(int)(item - 1)] = val;
                if(item == ItemList.Type.STACKABLE_BAG)
                {
                    __instance.savedata.stackableItemList[(int)(item - 1), Math.Min((byte)(val - 1+30), (byte)35)] = true;
                }
            }

            Debug.Log("[SaveManager] " + item.ToString() + " set to "+ __instance.savedata.stackableCount[(int)(item - 1)]);
            if (Application.isEditor || value)
            {
                ___renewTimer_Item = 0.03f;
            }

            if (item == ItemList.Type.STACKABLE_BAG)
            {
                __instance.RenewBagSize();
            }
            if (item == ItemList.Type.STACKABLE_HP && value)
            {
                EventManager.Instance.mainCharacter.maxhealth = __instance.GetMaxHP();
                EventManager.Instance.mainCharacter.AddHealth(MainVar.instance.HP_ITEM, showNumber: true, fullyHeal: true);
                Debug.Log("[SaveManager] Total HP collected is " + __instance.savedata.stackableCount[(int)(item - 1)] + ". Player MaxHP is now " + __instance.GetMaxHP());
            }
            if (item == ItemList.Type.STACKABLE_SHARD && value)
            {
                EventManager.Instance.mainCharacter.maxhealth = __instance.GetMaxHP();
                EventManager.Instance.mainCharacter.AddHealth(MainVar.instance.HP_SHARD_ITEM, showNumber: true, fullyHeal: true);
                Debug.Log("[SaveManager] Total SHARD collected is " + __instance.savedata.stackableCount[(int)(item - 1)] + ". Player MaxHP is now " + __instance.GetMaxHP());
            }
            EventManager.Instance.mainCharacter.maxhealth = __instance.GetMaxHP();
            return false;
        }


        //change Map Icon 
        [HarmonyPatch(typeof(WorldManager), "CollectMapItem")]
        [HarmonyPrefix]
        static bool collectIconChange(ref ItemTile data2, ref WorldManager __instance)
        {
            _ = __instance.Area;
            ItemList.Type itemid = data2.itemid;
            short atRoomX = __instance.CurrentRoomX;
            short atRoomY = __instance.CurrentRoomY;
            __instance.GetRoomWithPosition(data2.transform.position.x, data2.transform.position.y, out atRoomX, out atRoomY);
            Debug.Log("Collecting : X = " + atRoomX + " , Y = " + atRoomY + " , Type : " + itemid);

            HUDObtainedItem.Instance.GiveItem(itemid, data2.GetSlotID());
            itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, data2.GetSlotID());


			if (ArchipelagoInterface.Instance.isConnected && (itemid == ArchipelagoInterface.remoteItem || itemid == ArchipelagoInterface.remoteItemProgressive))
			{
                string itemName = ArchipelagoInterface.Instance.getLocItemName(data2.itemid, data2.GetSlotID());
                ItemList.Type item;
				if (Enum.TryParse(itemName, out item)) {
                    itemid = item;
                }
                else
                {
                    FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.ITEM);
                    data2.DisableMe();
                    return false;
                }
			}
			if (itemid == ItemList.Type.STACKABLE_COG)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.ITEM);

            }
            else if (itemid == ItemList.Type.STACKABLE_HP)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.HP);
            }
            else if (itemid == ItemList.Type.STACKABLE_MP)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.MP);
            }
            else if (itemid == ItemList.Type.STACKABLE_EP)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.BP);
            }
            else if (itemid == ItemList.Type.STACKABLE_MATK)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.MATK);
            }
            else if (itemid == ItemList.Type.STACKABLE_RATK)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.RATK);
            }
            else if (itemid == ItemList.Type.STACKABLE_SHARD)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.SHARD);
            }
            else if (itemid >= ItemList.Type.BADGE_START && itemid <= ItemList.Type.BADGE_MAX)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.BADGE);
            }
            else if (itemid.ToString().Contains("ITEM") || itemid.ToString().Contains("QUEST") || itemid == ItemList.Type.STACKABLE_BAG || itemid.ToString().Contains("Useable"))
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.ITEM);
            }
            else
            {
                Debug.LogWarning("[EventDetect] Invalid Item obtained!");
            }
            data2.DisableMe();


            return false;
        }

        static readonly ItemList.Type[] newPins = {ArchipelagoInterface.remoteItemProgressive, ArchipelagoInterface.remoteItem, ItemList.Type.I19,RandomizerPlugin.PortalItem, ItemList.Type.I20, ItemList.Type.STACKABLE_COG };

        //autoPin Icons
        [HarmonyPatch(typeof(GemaItemExplorer),"StartMe")]
        [HarmonyPostfix]
        static void changeDiscoveryPin()
        {
            if (WorldManager.Instance.MapTime >= 2.4f && GameSystem.Instance.frame >= 149)
            {
                ItemList.Type nearestType = ItemList.Type.OFF;
                ItemTile tile = null;
                float num = WorldManager.Instance.FindNearestItem_Room(out nearestType, out tile);
                if (tile == null) return;
                if (WorldManager.Instance.CheckIsWall(tile.transform.position, any: false) > 0 || num != 0f)
                {
                    return;
                }
                if (WorldManager.Instance.GetFrontFadeAlpha() > 0f && WorldManager.Instance.isNearFrontfade(tile.transform.position, MainVar.instance.TILESIZE))
                {
                    return;
                }
                nearestType = RandomizerPlugin.getRandomizedItem(tile.itemid, tile.GetSlotID());
				if (ArchipelagoInterface.Instance.isConnected && (nearestType == ArchipelagoInterface.remoteItem || nearestType == ArchipelagoInterface.remoteItemProgressive))
				{
					Icon icon = Icon.PIN9;
					string itemName = ArchipelagoInterface.Instance.getLocItemName(tile.itemid, tile.GetSlotID());
					ItemList.Type item;
					if (Enum.TryParse(itemName, out item))
					{
						nearestType = item;
						if (nearestType.ToString().Contains("STACKABLE"))
						{
							icon = Icon.PIN2;
							if (WorldManager.Instance.CurrentRoomArea == AreaType.SNOWCAVE)
							{
								if (nearestType == ItemList.Type.STACKABLE_EP && WorldManager.Instance.CurrentRoomX == 20 && WorldManager.Instance.CurrentRoomY == 13)
								{
									icon = Icon.OFF;
								}
								if (nearestType == ItemList.Type.STACKABLE_EP && WorldManager.Instance.CurrentRoomX == 20 && WorldManager.Instance.CurrentRoomY == 14)
								{
									icon = Icon.OFF;
								}
							}
							if (WorldManager.Instance.CurrentRoomArea == AreaType.CLIFF && nearestType == ItemList.Type.STACKABLE_MP && (WorldManager.Instance.CurrentRoomType == RoomType.YONLY || WorldManager.Instance.CurrentRoomType == RoomType.YONLY2))
							{
								icon = Icon.OFF;
							}
						}
						else if (nearestType.ToString().Contains("BADGE"))
						{
							icon = Icon.PIN1;
							if (WorldManager.Instance.CurrentRoomArea == AreaType.BLUSHFOREST)
							{
								if (WorldManager.Instance.CurrentRoomX == 12 && WorldManager.Instance.CurrentRoomY == 13)
								{
									icon = Icon.OFF;
								}
								if (WorldManager.Instance.CurrentRoomX == 12 && WorldManager.Instance.CurrentRoomY == 14)
								{
									icon = Icon.OFF;
								}
							}
							if (WorldManager.Instance.CurrentRoomArea == AreaType.A_GALLERY)
							{
								if (WorldManager.Instance.CurrentRoomX == 28 && WorldManager.Instance.CurrentRoomY == 5)
								{
									icon = Icon.OFF;
								}
								if (WorldManager.Instance.CurrentRoomX == 28 && WorldManager.Instance.CurrentRoomY == 6)
								{
									icon = Icon.OFF;
								}
							}
						}
						else if (nearestType.ToString().Contains("ITEM") || nearestType.ToString().Contains("QUEST") || nearestType == ItemList.Type.STACKABLE_BAG || nearestType.ToString().Contains("Useable"))
						{
							icon = Icon.PIN9;
						}

					}
					FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, WorldManager.Instance.CurrentRoomX, WorldManager.Instance.CurrentRoomY, icon);

				}
				else if (newPins.Contains(nearestType))
                {
                    Debug.Log("[GemaItemExplorer] Change icon to item : " + Icon.PIN9);
                    FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, WorldManager.Instance.CurrentRoomX, WorldManager.Instance.CurrentRoomY, Icon.PIN9);
                }

            }
            return;
        }
    }

}
