using Character;
using EventMode;
using HarmonyLib;
using Map;
using System;
using System.Linq;
using UnityEngine;
using TeviRandomizer.TeviRandomizerSettings;

namespace TeviRandomizer
{
    class ItemSystemPatch()
    {
        // Items that need to be handled differently
        static ItemList.Type[] itemExceptions = { RandomizerPlugin.PortalItem, RandomizerPlugin.CoreUpgradeItem, RandomizerPlugin.ItemUpgradeItem, RandomizerPlugin.MoneyItem };

        //Hotswap item recieved
        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPrefix]
        static bool ObtainItem(ref ItemList.Type type, ref byte value, ref bool doRandomBadge, ref (ItemList.Type, byte) __state)
        {
            if (itemExceptions.Contains(type))
            {
                var em = EventManager.Instance;
                switch (type)
                {
                    case RandomizerPlugin.PortalItem:
                        SaveManager.Instance.SetStackableItem(type, value, true); // do i need this?
                        TeleporterRando.setTeleporterIcon(value);
                        value = 255;
                        return true;
                    // this can lead to an infinite loop
                    case ItemList.Type.I16:
                    case ItemList.Type.I15:
                    case ItemList.Type.I14:
                        Debug.LogError("[HUDObtainItem] Item I16 -> I14 Should not be given to the player with this System");
                        return true;
                }
            }

            value = 1;

            if (type.ToString().Contains("ITEM") || type.ToString().Contains("Useable"))
                value = 255;
            return true;
        }

        public static Sprite ChangeItemSpriteTemp = null;
        [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
        [HarmonyPostfix]
        static void changeSpriteInUI(ref SpriteRenderer ___itemicon, ref (ItemList.Type, byte) __state, ref ItemList.Type type, ref bool doRandomBadge)
        {

            if (type == ArchipelagoInterface.remoteItem || type == ArchipelagoInterface.remoteItemProgressive)
            {
                if (ArchipelagoInterface.Instance.isConnected)
                {
                    string itemName = type.ToString();
                    if (!doRandomBadge)
                        itemName = ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APLocationName[$"{__state.Item1} #{__state.Item2}"]);
                    ItemList.Type item;
                    if (Enum.TryParse(itemName, out item))
                    {
                        ___itemicon.sprite = CommonResource.Instance.GetItem((int)item);
                    }
                }
            }
            if (ChangeItemSpriteTemp != null)
            {
                ___itemicon.sprite = ChangeItemSpriteTemp;
                ChangeItemSpriteTemp = null;
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
                case ItemList.Type.ITEM_KNIFE:
                    if (SaveManager.Instance.GetUnlockedLogic(PlayerLogicState.TEVI_STRONG_GROUND_FRONT) <= 0)
                    {
                        SaveManager.Instance.SetUnlockLogic(PlayerLogicState.TEVI_STRONG_GROUND_FRONT, 1, usePopUp: true);
                    }
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


            if (SaveManager.Instance.GetOrb() == 2 && (SaveManager.Instance.GetItem(ItemList.Type.I19) > 0 || SaveManager.Instance.GetItem(ItemList.Type.I20) > 0 || item == ItemList.Type.I20 || item == ItemList.Type.I19) || (SaveManager.Instance.GetOrb() == 1 && TeviSettings.customFlags[CustomFlags.CebleStart]))
            {
                if ((SaveManager.Instance.GetOrb() == 1 && TeviSettings.customFlags[CustomFlags.CebleStart]))
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

            if (ArchipelagoInterface.Instance.isConnected)
            {
                if (item == ArchipelagoInterface.remoteItem || item == ArchipelagoInterface.remoteItemProgressive)
                {
                    return false;
                }
            }

            if (item >= ItemList.Type.BADGE_START && item <= ItemList.Type.BADGE_MAX)
            {
                if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedBadge) <= 0)
                    SaveManager.Instance.SetMiniFlag(Mini.UnlockedBadge, 1);

            }
            if (item.ToString().Contains("ITEM"))
            {
                if (value == 0)
                {
                    if (item == ItemList.Type.ITEM_KNIFE && SaveManager.Instance.GetUnlockedLogic(PlayerLogicState.TEVI_STRONG_GROUND_FRONT) > 0)
                        SaveManager.Instance.SetUnlockLogic(PlayerLogicState.TEVI_STRONG_GROUND_FRONT, 0, usePopUp: true);

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
                SaveManager.Instance.SetStackableItem(item, value, true);
                return false;
            }
            var em = EventManager.Instance;
            switch (item)
            {
                case ItemList.Type.I16:
                    CollectManager.Instance.CreateCollect(em.mainCharacter.t.position, ElementType.B_UPGRADE, ItemList.Resource.UPGRADE);
                    return false;
                case ItemList.Type.I15:
                    CollectManager.Instance.CreateCollect(em.mainCharacter.t.position, ElementType.B_RESOURCE, ItemList.Resource.CORE);
                    return false;
                case ItemList.Type.I14:
                    CollectManager.Instance.CreateCollect(em.mainCharacter.t.position, ElementType.B_COIN, ItemList.Resource.COIN);
                    return false;
                default:
                    return true;
            }
        }

        [HarmonyPatch(typeof(SaveManager), "SetItem")]
        [HarmonyPostfix]
        static void dynamicOrbChange(ref ItemList.Type item)
        {
            if (item == ItemList.Type.I19 || item == ItemList.Type.I20) EventManager.Instance.ReloadOrbStatus();
        }

        [HarmonyPatch(typeof(SaveManager), "SetStackableItem")]
        [HarmonyPrefix]
        static bool StackableItemChanges(ref ItemList.Type item, ref byte id, ref SaveManager __instance, ref float ___renewTimer_Item)
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
                if (item == ItemList.Type.STACKABLE_BAG)
                {
                    __instance.savedata.stackableItemList[(int)(item - 1), Math.Min((byte)(val - 1 + 30), (byte)35)] = true;
                }
            }

            Debug.Log("[SaveManager] " + item.ToString() + " set to " + __instance.savedata.stackableCount[(int)(item - 1)]);
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
            var slotId = data2.GetSlotID();
            ItemDistributionSystem.EnqueueItem(new(itemid, slotId,false));

            itemid = RandomizerPlugin.getRandomizedItem(data2.itemid, slotId);



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
            else if(itemid == ItemList.Type.I14)
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.COIN);
            else if(itemid == ItemList.Type.I15)
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.UPGRADE);
            else if(itemid == ItemList.Type.I16)
                FullMap.Instance.SetMiniMapIcon(WorldManager.Instance.Area, atRoomX, atRoomY, Icon.UPGRADE);
            else
            {
                Debug.LogWarning("[EventDetect] Invalid Item obtained!");
            }
            data2.DisableMe();


            return false;
        }

        static readonly ItemList.Type[] newPins = { ArchipelagoInterface.remoteItemProgressive, ArchipelagoInterface.remoteItem, ItemList.Type.I19, RandomizerPlugin.PortalItem, ItemList.Type.I20, ItemList.Type.STACKABLE_COG, ItemList.Type.I14, ItemList.Type.I15, ItemList.Type.I16 };

        //autoPin Icons
        [HarmonyPatch(typeof(GemaItemExplorer), "StartMe")]
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


        //ResourceCollection

        private enum collectResourceType
        {
            NONE = 0,
            BLOCK = 1,
            ENEMY = 2,
            EVENT = 3,
            ARCADE = 5,
        }
        private static collectResourceType collectType;

        [HarmonyPatch(typeof(WorldManager), "DestroyTileInArea")]
        [HarmonyPrefix]
        static void checkDestroyedBlock() => collectType = collectResourceType.BLOCK;
        [HarmonyPatch(typeof(WorldManager), "DestroyTileInArea")]
        [HarmonyPostfix]
        static void uncheckDestroyedBlock() => collectType = collectResourceType.NONE;



        [HarmonyPatch(typeof(enemyController), "AreaDefeatUpgradeRequirement")]
        [HarmonyPrefix]
        static void checkKillEnemy() => collectType = collectResourceType.ENEMY;

        [HarmonyPatch(typeof(enemyController), "DropItem")]
        [HarmonyPrefix]
        static void checkKillUniqueEnemy() => collectType = collectResourceType.ENEMY;

        [HarmonyPatch(typeof(enemyController), "DefeatEnemy")]
        [HarmonyPostfix]
        static void uncheckKilledEnemy() => collectType = collectResourceType.NONE;



        [HarmonyPatch(typeof(enemyController), "AreaDefeatUpgradeRequirement")]
        [HarmonyPostfix]
        static void reduceMinRewuirement(ref int __result) => __result = 0;

        [HarmonyPatch(typeof(CollectManager), "CreateCollect")]
        [HarmonyPrefix]
        static bool collectBlock(ref Vector3 position, ref ItemList.Resource r, ref ElementType element)
        {


            int blockPos = 0;
            int area = WorldManager.Instance.Area;

            switch (collectType)
            {
                case collectResourceType.BLOCK:
                    blockPos = Utility.PosToTileX(position.x) * 1000 + Utility.PosToTileY(position.y) * -1;
                    Debug.LogWarning($"Block Destroyed at {blockPos}, Type: {r.ToString()}");
                    break;
                case collectResourceType.ENEMY:
                    //place a switch for different resources gained
                    if (r != ItemList.Resource.UPGRADE && r != ItemList.Resource.CORE)
                    {
                        customCollector(position, element, r);
                        Debug.Log($"[Loot] The Enemy droped {r}");
                        return false;
                    }
                    if (r == ItemList.Resource.CORE)
                    {
                        area = 1;
                        blockPos = uniqueEnemies - number;
                    }
                    Debug.LogWarning($"[Loot] Collecting {r} from killed mob at Area:{WorldManager.Instance.Area}");
                    break;
                case collectResourceType.EVENT:
                    var eventmode = EventManager.Instance.GetCurrentEvent();
                    Debug.Log($"[TeviRandomizer] Collecting Resource from: {EventManager.Instance.GetCurrentEvent().ToString()}");
                    switch (eventmode)
                    {
                        default: break;
                    }
                    if (missionResource)
                    {
                        var submode = missionMode;
                        if (submode == Mode.StartMission3A)
                            blockPos = EliteMissionOffset;
                        if (submode == Mode.StartMission3B)
                            blockPos = EliteMissionOffset - 1 * 2;
                        if (submode == Mode.StartMission3C)
                            blockPos = EliteMissionOffset - 5 * 2;
                        if (submode == Mode.StartMission8A)
                            blockPos = EliteMissionOffset - 2 * 2;
                        if (submode == Mode.StartMission15A)
                            blockPos = EliteMissionOffset - 3 * 2;
                        if (submode == Mode.StartMission20A)
                            blockPos = EliteMissionOffset - 4 * 2;
                        if (r == ItemList.Resource.CORE)
                            blockPos--;
                    }
                    if (bigBadBoss)
                    {
                        if (r != ItemList.Resource.CORE && r != ItemList.Resource.UPGRADE)
                        {
                            Debug.Log(r);
                            customCollector(position, element,r);
                            return false;
                        }
                        
                        blockPos = BigBossOffest;
                        blockPos -= number;
                        if (r == ItemList.Resource.CORE)
                            blockPos -= 2;
                    }
                    break;
                case collectResourceType.ARCADE:
                    blockPos = ArcadeOffset - number;
                    break;
                case collectResourceType.NONE:
                default:
                    number = 0;
                    customCollector(position, element,r);
                    return false;
            }
            number = 0;
            if (!LocationTracker.hasResource((byte)area, blockPos))
            {
                LocationTracker.addResourceToList((byte)area, blockPos);
                ItemList.Type item = RandomizerPlugin.getRandomizedResource(r, (byte)area, blockPos);
                if (!(item == ItemList.Type.OFF || item == ItemList.Type.I14 || item == ItemList.Type.I15 || item == ItemList.Type.I16))
                {
                    ResourceQueueItem(item, (byte)area, blockPos);
                    return false;
                }

                r = itemToResource(item);
                if (r == ItemList.Resource.COIN)
                    element = ElementType.B_COIN;
                customCollector(position, element,r);
                return false;
            }
            Debug.LogWarning($"[Resource Collection] Resource {r.ToString()} Something went wrong: {collectType} -> Area:{area} ID:{blockPos}");
            customCollector(position, element,r);
            return false;
        }


        static bool collectBool = false;

        [HarmonyPatch(typeof(collectScript),"CollectMe")]
        [HarmonyPrefix]
        static void collectBoolEnable() => collectBool = true;

        [HarmonyPatch(typeof(collectScript),"CollectMe")]
        [HarmonyPostfix]
        static void collectBoolDisabled() => collectBool = false;





        public static void customCollector(Vector3 position, ElementType element, ItemList.Resource r)
        {
            int num = 1;
            ItemList.Resource resource = ItemList.Resource.COIN;
            if (element == ElementType.B_COIN)
            {
                resource = ItemList.Resource.COIN;
                num = 5;
            }
            if (element == ElementType.B_RESOURCE || element == ElementType.B_UPGRADE)
            {
                resource = r;
            }
            int spawned = 0;
            var collects = CollectManager.Instance.collects;
            for (int i = 0; i < num; i++)
            {
                bool flag = false;
                foreach (collectScript collect in collects)
                {
                    if (!collect.isActiveAndEnabled)
                    {
                        collect.resource = resource;
                        collect.transform.position = position;
                        collect.EnableMe();
                        flag = true;
                        break;
                    }
                }
                if (flag) 
                    spawned++;
            }
            if(spawned < num)
            {
                collectScript prefab = Traverse.Create(CollectManager.Instance).Field("collect_prefab").GetValue<collectScript>();

                foreach (var collect in collects.ToArray())
                {
                    collectScript collectScript2 = UnityEngine.Object.Instantiate(prefab);
                    collectScript2.DisableMe();
                    collectScript2.transform.parent = CollectManager.Instance.transform;
                    collects.Add(collectScript2);
                    if(spawned < num)
                    {
                        collectScript2.resource = resource;
                        collectScript2.transform.position = position;
                        collectScript2.EnableMe();
                        spawned++;
                    }
                }
            }
        }

        public static ItemList.Resource itemToResource(ItemList.Type item)
        {
            switch (item)
            {
                case ItemList.Type.I14:
                    return ItemList.Resource.COIN;
                case ItemList.Type.I15:
                    return ItemList.Resource.CORE;
                case ItemList.Type.I16:
                    return ItemList.Resource.UPGRADE;
                default:
                    return ItemList.Resource.RESOURCE_MAX;
            }
        }

        static void ResourceQueueItem(ItemList.Type item,byte area,int blockPos)
        {
            string name = "";
            string desc = "";
            Sprite icon = null;
            if (ArchipelagoInterface.Instance?.isConnected == true && (item == ArchipelagoInterface.remoteItemProgressive || item == ArchipelagoInterface.remoteItem))
            {
                var location = LocationTracker.getResourceLocationName(area, blockPos);

                name = ArchipelagoInterface.Instance.getLocItemName(location);
                string playerName = ArchipelagoInterface.Instance.getLocPlayerName(location);
                desc = $"You found {name} for {playerName}";

                if (Enum.TryParse(name, out ItemList.Type item2))
                {
                    name = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item2.ToString(), true);
                    desc = "<color=\"red\">" + $"                                                                          <size=200%> FOOL!</color><size=100%>\n\n\n<font-weight=100>{name} was stolen by {playerName}";
                    icon = CommonResource.Instance.GetItem((int)item2);
                }
                else
                {
                    name = item.ToString();
                }
            }
            string locname = LocationTracker.getResourceLocationName(area,blockPos);

            byte value = 1;
            if(item == RandomizerPlugin.PortalItem && RandomizerPlugin.__itemData.TryGetValue(locname, out string itemName))
                value = byte.Parse(itemName.Split(["Teleporter "], StringSplitOptions.RemoveEmptyEntries)[0]);

            ItemDistributionSystem.EnqueueItem(new TeviItemInfo(item, value, true, name, desc,false,icon));
        }

        const int ShopBonusOffset = -100;
        const int EliteMissionOffset = -200;
        const int BigBossOffest = -150;
        const int ArcadeOffset = -250;
        public static int number = 0;
        const int CraftingOffset = -300;
        const int uniqueEnemies = -10_000;

        [HarmonyPatch(typeof(ShopBonus), "EVENT")]
        [HarmonyPrefix]
        static void shopBoni()
        {

            EventManager em = EventManager.Instance;

            if (em.EventStage == 10)
            {
                em.NextStage();
            }
            if (em.EventStage == 20)
            {
                if (!(em.EventTime >= 0.23f))
                {
                    return;
                }
                if (em.GetCounter(3) == 0f)
                {
                    while (SaveManager.Instance.savedata.coinUsedIan >= 3000 + 7000 * SaveManager.Instance.GetMiniFlag(Mini.ShopBonusIan))
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.ShopBonusIan, (byte)(SaveManager.Instance.GetMiniFlag(Mini.ShopBonusIan) + 1));
                        var blockPos = ShopBonusOffset + SaveManager.Instance.GetMiniFlag(Mini.ShopBonusIan);
                        if (!LocationTracker.hasResource(3, blockPos))
                        {
                            LocationTracker.addResourceToList(3, blockPos);
                            var item = RandomizerPlugin.getRandomizedResource(ItemList.Resource.CORE, 3, blockPos);
                            ResourceQueueItem(item, 3, blockPos);
                        }
                    }
                }
                if (em.GetCounter(3) == 1f)
                {
                    while (SaveManager.Instance.savedata.coinUsedCC >= 4000 + 7500 * SaveManager.Instance.GetMiniFlag(Mini.ShopBonusCC))
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.ShopBonusCC, (byte)(SaveManager.Instance.GetMiniFlag(Mini.ShopBonusCC) + 1));
                        var blockPos = ShopBonusOffset - SaveManager.Instance.GetMiniFlag(Mini.ShopBonusCC);
                        if (!LocationTracker.hasResource(3, blockPos))
                        {
                            LocationTracker.addResourceToList(3, blockPos);
                            var item =RandomizerPlugin.getRandomizedResource(ItemList.Resource.CORE, 3,blockPos);
                            ResourceQueueItem(item, 3,blockPos);

                        }
                    }
                }
                CameraScript.Instance.PlaySound(AllSound.SEList.Collect_Resource2);
                em.NextStage();
            }
            if (em.EventStage == 30)
                em.StopEvent();
        }

        static bool bigBadBoss = false;
        static bool craftingResource = false;
        static bool missionResource = false;
        static Mode missionMode = Mode.OFF;




        [HarmonyPatch(typeof(SaveManager), "AddResource")]
        [HarmonyPrefix]
        static bool redirectToCollect(ref ItemList.Resource resource, ref int value)
        {
            if (bigBadBoss && !collectBool)
            {
                if (resource != ItemList.Resource.CORE && resource != ItemList.Resource.UPGRADE)
                {
                    return true;
                }
                number = 0;
                CollectManager.Instance.CreateCollect(EventManager.Instance.GetPlayerLastPosition(0), ElementType.B_RESOURCE, resource);
                number = 1;
                CollectManager.Instance.CreateCollect(EventManager.Instance.GetPlayerLastPosition(0), ElementType.B_RESOURCE, resource);
                number = 0;
                return false;
            }
            if (craftingResource && (resource == ItemList.Resource.CORE || resource == ItemList.Resource.UPGRADE))
            {
                Debug.Log($"[Resource Conversion] Converting {resource} into a Randomized Item");
                if (resource == ItemList.Resource.CORE)
                    number = SaveManager.Instance.GetCoreExchange();
                if (resource == ItemList.Resource.UPGRADE)
                    number = SaveManager.Instance.GetUpgradeExchange();

                var blockPos = CraftingOffset - number * 2;
                if (resource == ItemList.Resource.CORE)
                    blockPos--;
                if (!LocationTracker.hasResource(1, blockPos))
                {
                    LocationTracker.addResourceToList(1, blockPos);
                    ItemList.Type item = RandomizerPlugin.getRandomizedResource(resource, 1, blockPos);
                    if (item == ItemList.Type.I14 || item == ItemList.Type.I15 || item == ItemList.Type.I16)
                    {
                        resource = itemToResource(item);
                        if (resource == ItemList.Resource.COIN)
                            value = 500;
                        return true;
                    }
                    else
                        ResourceQueueItem(item, 1, blockPos);
                }

                return false;
            }

            return true;
        }


        // Crafting

        [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "Update")]
        [HarmonyPrefix]
        static void crafton(ref ItemList.Type ___currentItemType) => 
            craftingResource = (___currentItemType == ItemList.Type.Function_MaterialExchangeA || ___currentItemType == ItemList.Type.Function_MaterialExchangeB);
        
        [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "OnDisable")]
        [HarmonyPrefix]
        static void craftoff() => craftingResource = false;
        



        // Elite Missions

        [HarmonyPatch(typeof(AfterMission), "EVENT")]
        [HarmonyPrefix]
        static void eliteMissionON()
        {
            collectType = collectResourceType.EVENT;
            missionResource = true;
            missionMode = EventManager.Instance.getSubMode();
        }
        [HarmonyPatch(typeof(AfterMission), "EVENT")]
        [HarmonyPostfix]
        static void elitMissionOF()
        {
            collectType = collectResourceType.NONE;
            missionResource = false;
            missionMode = Mode.OFF;

        }




        // Tartarus Arcade

        [HarmonyPatch(typeof(SaveManager),"GiveUpgrade")]
        [HarmonyPrefix]
        static void acradeBonus()
        {
            collectType = collectResourceType.ARCADE;
            if (SaveManager.Instance.GetMiniFlag(Mini.PlayedMiniGame) == 0 && SaveManager.Instance.savedata.minigame_highest_score > 0)
                    number = 0;
            else
            {
                var i = SaveManager.Instance.savedata.minigame_highest_round+1;
                while ( i <= MainVar.instance.MINIGAME_ROUND_CLEARED)
                {
                    if (i == 1)
                    {
                        number = 1;
                        SaveManager.Instance.savedata.minigame_highest_round = (byte)i;
                        return;
                    }
                    if (i == 3)
                    {
                        number = 2;
                        SaveManager.Instance.savedata.minigame_highest_round = (byte)i;
                        return;
                    }
                    if (i == 5)
                    {
                        number = 3;
                        SaveManager.Instance.savedata.minigame_highest_round = (byte)i;
                        return;
                    }
                    if (i > 5)
                        return;
                    i++;
                }
            } 
        }
        [HarmonyPatch(typeof(SaveManager), "GiveUpgrade")]
        [HarmonyPostfix]
        static void disableArcadeBonsu() => collectType = collectResourceType.NONE;



        //Elite Bosses

        [HarmonyPatch(typeof(END_BOOKMARK),"EVENT")]
        [HarmonyPrefix]
        static void baddy()
        {
            if (EventManager.Instance.EventStage == 100 && (WorldManager.Instance.CurrentRoomBG == RoomBG.SEAL || WorldManager.Instance.CurrentRoomBG == RoomBG.ZENITH))
            {
                collectType = collectResourceType.EVENT;
                bigBadBoss = true;
            }
        }
        [HarmonyPatch(typeof(END_BOOKMARK),"EVENT")]
        [HarmonyPostfix]
        static void nobaddy()
        {
            if (EventManager.Instance.EventStage == 100 && (WorldManager.Instance.CurrentRoomBG == RoomBG.SEAL || WorldManager.Instance.CurrentRoomBG == RoomBG.ZENITH))
            {
                collectType = collectResourceType.NONE;
                bigBadBoss = false;
            }
        }

    }

}
