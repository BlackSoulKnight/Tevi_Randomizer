using EventMode;
using HarmonyLib;
using Map;
using System;
using UnityEngine;

namespace TeviRandomizer
{
    class ItemObtainPatch()
    {

        // may be used for custom icons
        [HarmonyPatch(typeof(CommonResource), "GetItem")]
        [HarmonyPrefix]
        static bool addCustomIcons()
        {
            return true;  //copy and insert custom item icon 
        }

        //Hotswap item recieved
        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPrefix]
        static void ObtainItem(ref ItemList.Type type, ref byte value)
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



            ItemData data = RandomizerPlugin.getRandomizedItem(type, value);

            value = (byte)data.slotID;
            type = (ItemList.Type)data.itemID;



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
            if (item >= ItemList.Type.BADGE_START && item <= ItemList.Type.BADGE_MAX && SaveManager.Instance.GetMiniFlag(Mini.UnlockedBadge) <= 0)
            {
                SaveManager.Instance.SetMiniFlag(Mini.UnlockedBadge, 1);
            }

            if (item.ToString().Contains("ITEM"))
            {
                Upgradable itemRef;
                if (Enum.TryParse<Upgradable>(item.ToString(), out itemRef))
                {
                    if (value == 0)
                    {
                        __instance.SetStackableItem((ItemList.Type)itemRef, 0, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 1, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 2, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 3, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 4, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 5, false);
                        __instance.SetStackableItem((ItemList.Type)itemRef, 6, false);
                        return true;
                    }

                    if (!__instance.GetStackableItem((ItemList.Type)itemRef, value))
                    {
                        __instance.SetStackableItem((ItemList.Type)itemRef, value, true);
                        if (value == 1) { value = 4; }
                        if (value > 3)
                        {
                            value = (byte)(__instance.GetItem(item) + 1);
                            if (!__instance.GetStackableItem((ItemList.Type)itemRef, 0))
                            {
                                __instance.SetStackableItem((ItemList.Type)itemRef, 0, true);
                            }
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
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[RANDOMIZER] Item {item}, Slot {value} was already claimed");
                        return false;
                    }
                }

            }


            return true;
        }
        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPostfix]
        static void dynamicOrbChange(ref ItemList.Type item)
        {
            if (item == ItemList.Type.I19 || item == ItemList.Type.I20) EventManager.Instance.ReloadOrbStatus();
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
                itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, data2.GetSlotID()).getItemTyp();
            }
            else
            {
                HUDObtainedItem.Instance.GiveItem(itemid, 1);
                itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, 1).getItemTyp();
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
            else if (itemid.ToString().Contains("ITEM") || itemid.ToString().Contains("QUEST") || itemid == ItemList.Type.STACKABLE_BAG)
            {
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.ITEM);
            }
            else if (itemid.ToString().Contains("Useable"))
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

    }

}
