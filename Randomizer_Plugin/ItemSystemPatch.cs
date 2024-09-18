using EventMode;
using HarmonyLib;
using Map;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static SaveManager;

namespace TeviRandomizer
{
    class ItemSystemPatch()
    {

        //Hotswap item recieved
        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPrefix]
        static void ObtainItem(ref ItemList.Type type, ref byte value, ref bool doRandomBadge)
        {
            if (type.ToString().Contains("_OrbType"))
            {
                switch (SaveManager.Instance.GetOrbTypeObtained())
                {
                    case 0:
                        type = ItemList.Type.ITEM_OrbTypeC2;
                        break;
                    case 1:
                        type = ItemList.Type.ITEM_OrbTypeS2;
                        break;
                    case 2:
                        type = ItemList.Type.ITEM_OrbTypeC3;
                        break;
                    case 3:
                        type = ItemList.Type.ITEM_OrbTypeS3;
                        break;
                }
            }

            //a native item pickup


            if (!doRandomBadge)
            {
                LocationTracker.addItemToList(type, value);

                ItemList.Type data = RandomizerPlugin.getRandomizedItem(type, value);

                if (ArchipelagoInterface.Instance.isConnected)
                {
                    if (data == ItemList.Type.I10 || data == ItemList.Type.I11)
                    {
                        string itemName = ArchipelagoInterface.Instance.getLocItemName(type, value);
                        string playerName = ArchipelagoInterface.Instance.getLocPlayerName(type, value);
                        RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(data), itemName);
                        string desc = $"You found {itemName} for {playerName}";
                        RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(data), desc);
                    }
                }

                type = data;

            }
            else
            {
                //Archipelago implementation

            }

            if (!type.ToString().Contains("STACKABLE"))
                value = 255;

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


            if (SaveManager.Instance.GetOrb() == 2 && (SaveManager.Instance.GetItem(ItemList.Type.I19) > 0 || SaveManager.Instance.GetItem(ItemList.Type.I20) > 0 || item == ItemList.Type.I20 || item == ItemList.Type.I19))
            {
                RandomizerPlugin.addOrbStatus(1);

            }

            return true;
        }


        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPrefix]
        static bool ItemChanges(ref ItemList.Type item, ref byte value, ref SaveManager __instance)
        {
            if(ArchipelagoInterface.Instance.isConnected)
            {
                if(item == ArchipelagoInterface.remoteItem || item == ArchipelagoInterface.remoteItemProgressive)
                {
                    return false;
                }
            }

            if (item >= ItemList.Type.BADGE_START && item <= ItemList.Type.BADGE_MAX)
            {
                value = 1;
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
            if (data2.itemid.ToString().Contains("STACKABLE"))
            {
                HUDObtainedItem.Instance.GiveItem(itemid, data2.GetSlotID());
                itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, data2.GetSlotID());
            }
            else
            {
                HUDObtainedItem.Instance.GiveItem(itemid, 1);
                itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, 1);
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
            else if (ArchipelagoInterface.Instance.isConnected && (itemid == ArchipelagoInterface.remoteItem || itemid == ArchipelagoInterface.remoteItemProgressive))
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

        static readonly ItemList.Type[] newPins = {ItemList.Type.I10, ItemList.Type.I11, ItemList.Type.I19, ItemList.Type.I20, ItemList.Type.STACKABLE_COG };

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
                if (WorldManager.Instance.CheckIsWall(tile.transform.position, any: false) > 0 || num != 0f)
                {
                    return;
                }
                if (WorldManager.Instance.GetFrontFadeAlpha() > 0f && WorldManager.Instance.isNearFrontfade(tile.transform.position, MainVar.instance.TILESIZE))
                {
                    return;
                }
                if (newPins.Contains(nearestType))
                {
                    Debug.Log("[GemaItemExplorer] Change icon to item : " + Icon.PIN9);
                    FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, WorldManager.Instance.CurrentRoomX, WorldManager.Instance.CurrentRoomY, Icon.PIN9);
                }
            }
            return;
        }
    }

}
