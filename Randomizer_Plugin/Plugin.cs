using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;
using TMPro;
using static SaveManager;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;



//Crafting Orb type bug






namespace UnityEngine;



public class ItemData
{
    public int itemID;
    public int slotID;
    public ItemData(int _itemID, int _slotID)
    {
        itemID = _itemID;
        slotID = _slotID;
    }

    public class EqualityComparer : IEqualityComparer<ItemData>
    {
        public bool Equals(ItemData x, ItemData y)
        {
            return x.itemID == y.itemID && x.slotID == y.slotID;
        }

        public int GetHashCode(ItemData x)
        {
            return x.itemID ^ x.slotID;
        }
    }
    public ItemList.Type getItemTyp()
    {
        return (ItemList.Type)itemID;
    }
    public byte getSlotId()
    {
        return (byte)slotID;
    }



}









[BepInPlugin("tevi.plugins.randomizer", "Randomizer", "0.2.5.0")]
[BepInProcess("TEVI.exe")]
public class Randomizer : BaseUnityPlugin
{

    static Dictionary<ItemData, ItemData> __itemData = new Dictionary<ItemData, ItemData>(new ItemData.EqualityComparer());





    private void Awake()
    {
        bool fileRead = true;
        try
        {
            string path = BepInEx.Paths.PluginPath + "/tevi_randomizer/data/file.dat";
            string json = File.ReadAllText(path);
            string[] blocks = json.Split(';');
            foreach (string block in blocks)
            {
                ItemData data1, data2;
                if (block.Length < 5) continue;
                try
                {
                    string[] completeItem = block.Split(':');

                    string[] itemDetails1 = completeItem[0].Split(',');
                    string[] itemDetails2 = completeItem[1].Split(',');
                    data1 = new ItemData(int.Parse(itemDetails1[0]), int.Parse(itemDetails1[1]));
                    data2 = new ItemData(int.Parse(itemDetails2[0]), int.Parse(itemDetails2[1]));
                }
                catch
                {
                    Logger.LogError($"Failed to parse {block}");
                    continue;
                }
                try
                {
                    __itemData.Add(data1, data2);
                }
                catch
                {
                    Logger.LogWarning($"Already changed {data1}");

                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            fileRead = false;
        }


        if (!fileRead)
        {
            Logger.LogWarning("[Randomizer] Could not initialize the Randomizer");
            Logger.LogWarning("[Randomizer] Plugin is disabled");
            return;
        }
        itemPairSetup();


        var instance = new Harmony("Randomizer");
        instance.PatchAll(typeof(Randomizer));
        instance.PatchAll(typeof(CraftingPatch));
        Logger.LogInfo($"Plugin Randomizer is loaded!");



    }

    static public ItemData getRandomizedItem(ItemList.Type itemid, byte slotid)
    {
        ItemData data;
        try
        {
            //Debug.Log($"[Randomizer] Load item {itemid}:{slotid}");

            data = __itemData[new ItemData((int)itemid, (int)slotid)];
            //Debug.Log($"[Randomizer] Found item {(ItemList.Type)data.itemID}:{data.slotID}");
        }
        catch
        {
            Debug.LogError("[Randomizer] Could not load the randomized Item " + itemid.ToString() + "! It has the ID: " + (int)itemid + " and the SlotID: " + slotid);
            data = new ItemData((int)itemid, (int)slotid);
        }
        return data;
    }
    static  (ItemList.Type, ItemList.Type)[] _upgradeAble = new (ItemList.Type, ItemList.Type)[21];

     private void itemPairSetup()
    {
        _upgradeAble[0] = (ItemList.Type.ITEM_KNIFE, ItemList.Type.ITEM_46);
        _upgradeAble[1] = (ItemList.Type.ITEM_ORB, ItemList.Type.ITEM_47);
        _upgradeAble[2] = (ItemList.Type.ITEM_RapidShots, ItemList.Type.ITEM_48);
        _upgradeAble[3] = (ItemList.Type.ITEM_AttackRange, ItemList.Type.ITEM_49);
        _upgradeAble[4] = (ItemList.Type.ITEM_EasyStyle, ItemList.Type.ITEM_50);
        _upgradeAble[5] = (ItemList.Type.ITEM_LINEBOMB, ItemList.Type.ITEM_51);
        _upgradeAble[6] = (ItemList.Type.ITEM_AREABOMB, ItemList.Type.ITEM_52);
        _upgradeAble[7] = (ItemList.Type.ITEM_SPEEDUP, ItemList.Type.ITEM_53);
        _upgradeAble[8] = (ItemList.Type.ITEM_AirDash, ItemList.Type.ITEM_54);
        _upgradeAble[9] = (ItemList.Type.ITEM_WALLJUMP, ItemList.Type.ITEM_55);
        _upgradeAble[10] = (ItemList.Type.ITEM_JETPACK, ItemList.Type.ITEM_56);
        _upgradeAble[11] = (ItemList.Type.ITEM_BoostSystem, ItemList.Type.ITEM_57);
        _upgradeAble[12] = (ItemList.Type.ITEM_BombLengthExtend, ItemList.Type.ITEM_58);
        _upgradeAble[13] = (ItemList.Type.ITEM_MASK, ItemList.Type.ITEM_59);
        _upgradeAble[14] = (ItemList.Type.ITEM_TempRing, ItemList.Type.ITEM_60);
        _upgradeAble[15] = (ItemList.Type.ITEM_DodgeShot, ItemList.Type.ITEM_61);
        _upgradeAble[16] = (ItemList.Type.ITEM_Rotater, ItemList.Type.ITEM_62);
        _upgradeAble[17] = (ItemList.Type.ITEM_GoldenGlove, ItemList.Type.ITEM_63);
        _upgradeAble[18] = (ItemList.Type.ITEM_OrbAmulet, ItemList.Type.ITEM_64);
        _upgradeAble[19] = (ItemList.Type.ITEM_BOMBFUEL, ItemList.Type.ITEM_65);
        _upgradeAble[20] = (ItemList.Type.ITEM_Explorer, ItemList.Type.ITEM_66);
    }

    static public ItemList.Type getItemPair(ItemList.Type type,bool reversed)
    {
        (ItemList.Type, ItemList.Type) result = (type,type);
        foreach ((ItemList.Type,ItemList.Type) item in _upgradeAble)
        {
            if (item.Item1 == type || item.Item2 == type) result = item;
        }
        if (!reversed)
        {
            return result.Item2;
        }
        else
        {
            return result.Item1;
        }
        
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
        ItemData data = getRandomizedItem(type, value);

        if (data.getItemTyp().ToString().Contains("ITEM"))
        {
             value = (byte)(SaveManager.Instance.GetItem(type) + 1);
            
        }
        else
        {
            value = (byte)data.slotID;
            type = (ItemList.Type)data.itemID;
        }

        
    }
    //craftingMenuRefresh
    [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
    [HarmonyPostfix]
    static void CraftingRefresh()
    {
        Traverse.Create(GemaUIPauseMenu_CraftGrid.Instance).Method("UpdateCraftList").GetValue();

    }




    // Change Sprite
    [HarmonyPatch(typeof(ItemTile), nameof(ItemTile.LoadItem))]
    [HarmonyPrefix]
    static bool changeSprite(ItemTile __instance, ref SpriteRenderer ____sprite, ref SpriteRenderer ____childsprite, ref SpriteRenderer ____secondsprite, ref byte ___slotid)
    {
        Traverse tree = Traverse.Create(__instance);

        if (__instance.itemid.ToString().Contains("STACKABLE"))
        {
            for (int i = 1; i <= 16; i++)
            {
                ___slotid = EventManager.Instance.GetIDFromAbove(__instance.transform, i);
                if (___slotid >= 0 && ___slotid <= 63)
                {
                    break;
                }
            }
            if (___slotid >= 64)
            {
                Debug.LogWarning("[ItemTile] This item " + __instance.itemid.ToString() + " is a mutli item but no Slot ID is set! ID Invalid : " + ___slotid, __instance.gameObject);
                return false;
            }


        }
        else
        {
            ___slotid = 1;
        }

        if (EventManager.Instance.GetElm(__instance.transform, 0f, MainVar.instance.TILESIZE) == ElementType.NotFreeRoamOnly)
        {
            __instance.DisableMe();
            return false;
        }
        ItemData data = getRandomizedItem(__instance.itemid, ___slotid);
        var spr = CommonResource.Instance.GetItem(data.itemID);

        if (data.itemID >= (int)ItemList.Type.BADGE_START && data.itemID <= (int)ItemList.Type.BADGE_MAX)
        {
            if (____secondsprite != null)
            {

                ____secondsprite.sprite = spr;
                ____secondsprite.enabled = true;

            }



            ____sprite.sprite = CommonResource.Instance.Sprite_BadgeBackground;
            ____childsprite.sprite = spr;

        }
        else
        {
            if (____secondsprite != null)
            {
                ____secondsprite.sprite = null;
                ____secondsprite.enabled = false;
            }
            ____childsprite.sprite = spr;
            ____sprite.sprite = spr;
        }

        if (((ItemList.Type)data.itemID).ToString().Contains("STACKABLE"))
        {
            if (SaveManager.Instance.GetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID))
            {
                Debug.Log(string.Concat(new string[]
                    {
                    "[ItemTile] ",
                    ((ItemList.Type)data.itemID).ToString(),
                    " #",
                    data.slotID.ToString(),
                    " visible in camera. Removed from map because player already obtained it."
                    }));
                __instance.DisableMe();
                return false;
            }
        }
        else
        {
            if (SaveManager.Instance.GetItem((ItemList.Type)data.itemID) > 0 && !data.getItemTyp().ToString().Contains("ITEM"))
            {
                Debug.Log("[ItemTile] Item " + ((ItemList.Type)data.itemID).ToString() + " visible in camera. Removed from map because player already obtained it. GotItem = " + SaveManager.Instance.GetItem((ItemList.Type)data.itemID).ToString());
                __instance.DisableMe();
                return false;
            }
            if (SaveManager.Instance.GetItem(global::ItemList.Type.ITEM_GoldenHands) > 0 && ((ItemList.Type)data.itemID == global::ItemList.Type.QUEST_GHandL || (ItemList.Type)data.itemID == global::ItemList.Type.QUEST_GHandR))
            {
                Debug.Log("[ItemTile] Item " + ((ItemList.Type)data.itemID).ToString() + " visible in camera. Removed from map because player already obtained it. GotItem = " + SaveManager.Instance.GetItem((ItemList.Type)data.itemID).ToString());
                __instance.DisableMe();
                return false;
            }
        }

        if (WorldManager.Instance.CheckIsWall(__instance.transform.position, any: false) > 0)
        {
            tree.Field("canFall").SetValue(true);
        }
        if (WorldManager.Instance.CheckIsWall(__instance.transform.position - new Vector3(0f, MainVar.instance.TILESIZE, 0f), any: false) > 0)
        {
            tree.Field("canFall").SetValue(true);

        }
        tree.Field("gotData").SetValue(true);

        return false;
    }




    //Orb!
    [HarmonyPatch(typeof(SaveManager), "FirstTimeEnableOrbColors")]
    [HarmonyPrefix]
    static bool disableOrbOverride(SaveManager __instance)
    {
        if (__instance.GetMiniFlag(Mini.OrbStatus) >= 3) return true;
        return false;
    }

    public static void addOrbStatus(int amount = 0)
    {
        SaveManager.Instance.SetOrb((byte)(SaveManager.Instance.GetOrb() + amount));
        if (SaveManager.Instance.GetOrb() >= 3)
        {
            SaveManager.Instance.FirstTimeEnableOrbColors();
        }
    }







    //Craftig Orb Fix
    //No set SlotId, maybe reserver slots for Potions?
    [HarmonyPatch(typeof(SaveManager), "GetOrbTypeObtained")]
    [HarmonyPostfix]
    static void orbTypeFix(ref int __result, ref SaveManager __instance)
    {
        int num = 0;
        ItemData data1 = getRandomizedItem(ItemList.Type.ITEM_OrbTypeC2, 1);
        ItemData data2 = getRandomizedItem(ItemList.Type.ITEM_OrbTypeC3, 1);
        ItemData data3 = getRandomizedItem(ItemList.Type.ITEM_OrbTypeS2, 1);
        ItemData data4 = getRandomizedItem(ItemList.Type.ITEM_OrbTypeS3, 1);

        if (!data1.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetItem(data1.getItemTyp()) > 0)
            {
                num++;
            }
        }
        else
        {
            if (__instance.GetStackableItem(data1.getItemTyp(), data1.getSlotId()))
            {
                num++;
            }
        }

        if (!data2.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetItem(data2.getItemTyp()) > 0)
            {
                num++;
            }
        }
        else
        {
            if (__instance.GetStackableItem(data2.getItemTyp(), data2.getSlotId()))
            {
                num++;
            }
        }

        if (!data3.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetItem(data3.getItemTyp()) > 0)
            {
                num++;
            }
        }
        else
        {
            if (__instance.GetStackableItem(data3.getItemTyp(), data3.getSlotId()))
            {
                num++;
            }
        }

        if (!data4.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetItem(data4.getItemTyp()) > 0)
            {
                num++;
            }
        }
        else
        {
            if (__instance.GetStackableItem(data4.getItemTyp(), data4.getSlotId()))
            {
                num++;
            }
        }
        __result = num;
    }

    [HarmonyPatch(typeof(SaveManager), "GetOrbBoostObtained")]
    [HarmonyPostfix]
    static void OrbBoostCount(ref int __result, SaveManager __instance)
    {
        int num = 0;
        ItemData data1 = getRandomizedItem(ItemList.Type.ITEM_OrbBoostD, 1);
        ItemData data2 = getRandomizedItem(ItemList.Type.ITEM_OrbBoostU, 1);
        if (data1.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetStackableItem(data1.getItemTyp(), data1.getSlotId()))
                num++;
        }
        else
        {
            if (__instance.GetItem(data1.getItemTyp()) > 0)
                num++;
        }
        if (data2.getItemTyp().ToString().Contains("STACKABLE"))
        {
            if (__instance.GetStackableItem(data2.getItemTyp(), data2.getSlotId()))
                num++;
        }
        else
        {
            if (__instance.GetItem(data2.getItemTyp()) > 0)
                num++;
        }
        __result = num;
    }



    // Called everytime when an Item is obtained through any means
    [HarmonyPatch(typeof(SaveManager), "SetItem")]
    [HarmonyPrefix]
    static bool setItemAdditionals(ref ItemList.Type item, byte value)
    {

        if (item > ItemList.Type.STACKABLE_COG && item < ItemList.Type.STACKABLE_BAG && value > 35)
        {
            //RandomizedData data = getRandomizedItem((int)item, (int)value);
            //dunno

        }

        switch (item)
        {
                
            case ItemList.Type.ITEM_ORB:

                addOrbStatus(3);

                break;

            default:
                break;
        }
        return true;
    }


    [HarmonyPatch(typeof(SaveManager), "SetItem")]
    [HarmonyPrefix]
    static void ItemChanges(ref ItemList.Type item, ref byte value, ref SaveManager __instance)
    {
        ItemData data = getRandomizedItem(item, value);
        if (item.ToString().Contains("ITEM"))
        {
            if (value == 0)
            {
                return;
            }
            if (item >= ItemList.Type.ITEM_46 && item <= ItemList.Type.ITEM_66)
            {
                __instance.savedata.itemflag[(int)item] = value;

                item = data.getItemTyp();
                value = data.getSlotId();

            }
            else
            {
                //OrbBoostD+U check? Cyril.Always()
                ItemList.Type tryGetItem = getItemPair(item, false);
                if (tryGetItem != item && __instance.GetItem(tryGetItem) == 0)
                {
                    __instance.savedata.itemflag[(int)tryGetItem] = 1;
                }
                value = (byte)(__instance.GetItem(item) + 1);

            }
        }

    }

    //Shop Items for now max 1 Stackable foreach type

    [HarmonyPatch(typeof(HUDShopMenu), "AddItem")]
    [HarmonyPrefix]
    static bool shopItems(HUDShopMenu __instance, ref ItemList.Type item, ref Character.Type ___typeN, ref GemaShopItemSlot[] ___itemslots, ref int ___CurrentMaxItem, ref byte ___ShopID, ref byte ___ShopType)
    {

        byte area = WorldManager.Instance.Area;


        int num = GemaItemManager.Instance.GetItemCoin(item);
        if (num <= 1000)
        {
            num = 250 * GemaItemManager.Instance.GetItemCost(item);
            if (num < 1250)
            {
                num += 250;
                if (num > 1250)
                {
                    num = 1250;
                }
            }
            if (num < 1000)
            {
                num = 1000;
            }
        }

        if (___ShopType == 0)
        {
            ItemData data;
            if (item.ToString().Contains("STACKABLE"))
                data = getRandomizedItem(item, (byte)(___ShopID + 30));
            else
                data = getRandomizedItem(item, 1);


            if ((ItemList.Type)data.itemID == ItemList.Type.STACKABLE_BAG)
            {
                MainVar.instance.BagID = (byte)(___ShopID + 1);
            }
            if ((((ItemList.Type)data.itemID).ToString().Contains("STACKABLE") && SaveManager.Instance.GetStackableItem((ItemList.Type)data.itemID, (byte)(30 + ___ShopID))) || SaveManager.Instance.GetItem((ItemList.Type)data.itemID) > 0)
            {
                return false;
            }


            switch (___typeN)
            {
                case Character.Type.Ian:
                    ___itemslots[___CurrentMaxItem].SetItem((ItemList.Type)data.itemID, num, false);
                    ___CurrentMaxItem++;
                    break;
                case Character.Type.CC:

                    ___itemslots[___CurrentMaxItem].SetItem((ItemList.Type)data.itemID, num, false);
                    ___CurrentMaxItem++;
                    break;
                default:
                    return true;

            }
            return false;
        }


        return true;
    }





    // Free Start Items
    [HarmonyPatch(typeof(Chap0GetKnife), "EVENT")]
    [HarmonyPrefix]
    static void StartEvent()
    {
        EventManager em = EventManager.Instance;

        if (em.EventStage == 10)
        {

            SaveManager.Instance.SetOrb((byte)0);
            ItemData data = getRandomizedItem(ItemList.Type.ITEM_ORB, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            data = getRandomizedItem(ItemList.Type.ITEM_KNIFE, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            em.SetStage(30);
        }
    }


    [HarmonyPatch(typeof(Chap1FreeRoamVena7x7), "REQUIREMENT")]
    [HarmonyPrefix]
    static bool Vena7x7Fix(ref bool __result)
    {
        ItemData data = getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);

        if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
        {
            __result = false;
            return false;
        }
        if (((ItemList.Type)data.itemID).ToString().Contains("STACKABLE"))
        {
            bool help = !SaveManager.Instance.GetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID);
            __result = help;
        }
        else
        {
            __result = SaveManager.Instance.GetItem((ItemList.Type)data.itemID) == 0;
        }
        return false;
    }

    [HarmonyPatch(typeof(EventDetect), "OnTriggerEnter2D")]
    [HarmonyPrefix]
    static bool VenaEXTRA(ref Collider2D col)
    {
        EventTile component = col.GetComponent<EventTile>();
        if ((bool)component)
        {
            if (EventManager.Instance.GetCurrentEvent() == Mode.OFF && !EventManager.Instance.isBossMode() && component.mode != Mode.LibraryPoint && component.mode != Mode.SnowCaveMazeDisabled && component.mode != Mode.MazeCompleted && !GemaBossRushMode.Instance.isBossRush())
            {
                if (EventManager.Instance.NoEventStartFromDetect)
                {
                    Debug.Log("[EventDetect] No Event Start From Detect is ON. This event cannot be triggered : " + component.mode);
                }
                else if (EventManager.Instance.CheckEventStartable(component.mode))
                {
                    if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                    {
                        ItemData data = getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);
                        bool flag = false;
                        if (data.getItemTyp().ToString().Contains("STACKABLE"))
                        {
                            flag = !SaveManager.Instance.GetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID);
                        }
                        else
                        {
                            flag = !((int)(SaveManager.Instance.GetItem((ItemList.Type)data.itemID)) > 0);
                        }
                        if (component.mode == Mode.Chap1FreeRoamVena7x7)
                        {
                            if (component.mode == Mode.Chap1FreeRoamVena7x7 && ((SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 && SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) == 1) || flag))
                            {
                                if (WorldManager.Instance.CheckIsWall(component.transform.position, any: false) == 1)
                                {
                                    Debug.Log("[EventDetect] Event is inside wall, cannot trigger : " + component.mode);
                                    flag = false;
                                }
                                if (flag && EventManager.Instance.TryStartEvent(component.mode, force: false))
                                {
                                    EventManager.Instance.LastHitTrigger = component;
                                }
                            }
                            return false;
                        }
                    }

                }
            }
        }

        return true;
    }

    [HarmonyPatch(typeof(Chap0Start), "CheckSR")]
    [HarmonyPrefix]
    static bool StartItemSetup()
    {
        if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
        {
            SaveManager.Instance.SetItem(ItemList.Type.ITEM_KNIFE, 0);
            SaveManager.Instance.SetItem(ItemList.Type.ITEM_ORB, 0);
            SaveManager.Instance.SetEventFlag(Mode.SWITCH_BASEKNIFE, 1, force: true);
            SaveManager.Instance.SetEventFlag(Mode.Chap0CandR, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0SeeBomb, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0SeeEnergyBall, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0SeeGoal, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0Outside, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0Outline7x7, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0CombatTut7x7, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap3ComboRank, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap1PinTool9x9, 1);
            SaveManager.Instance.SetEventFlag(Mode.Chap0UnlockFirstVent, 1, force: true);
            SaveManager.Instance.SetMiniFlag(Mini.UnlockedCraft, 1);
            EventManager.Instance.TryStartEvent(Mode.Chap0GetKnife, force: true);
            return false;
        }
        return true;
    }




}



class CraftingPatch
{
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid),"UpdateSelectedText")]
    [HarmonyPrefix]
    static bool itemDescChange(ref GemaUIPauseMenu_CraftGrid __instance,ref UI.Image ___iconbg,ref Transform ___costBox,ref GemaUIPauseMenu_CraftGridSlot[] ___craftList,ref int ___selected,ref TextMeshProUGUI ___selectedName,ref TextMeshProUGUI ___selectedDesc,
        ref int ___CurrentMaxCraft,ref TextMeshProUGUI ___costTitle,ref TextMeshProUGUI ___costValue,ref UI.Image ___selectedIcon,ref TextMeshProUGUI ___mownedText, ref TextMeshProUGUI ___useableText,ref bool ___setFontOutline,ref TextMeshProUGUI[] ___materialrequiredList,
        ref TextMeshProUGUI ___mrequiredText,ref byte[] ___currentMaterialNeeded,ref ItemList.Type ___currentItemType)
    {
        Traverse t = Traverse.Create(__instance);
        


        ___iconbg.enabled = false;
        ___costBox.gameObject.SetActive(value: false);
        ItemData data = Randomizer.getRandomizedItem(___craftList[___selected].GetItemType(), 1);



        ItemList.Type itemType = ___craftList[___selected].GetItemType();
        if (itemType.ToString().Contains("ITEM"))
        {
            data = Randomizer.getRandomizedItem(itemType, (byte)(SaveManager.Instance.GetItem(itemType)+1));
 
        }

        if (__instance.isSortMode)
        {
            itemType = t.Field("bagItems").GetValue<GemaUIPauseMenu_ItemGridSub[]>()[(int)t.Field("sortingSelected").GetValue<byte>()].GetItemType();
        }
        if (itemType == ItemList.Type.OFF)
        {
            ___selectedIcon.enabled = false;
            ___costBox.gameObject.SetActive(value:false);

            ___selectedName.text = string.Empty;
            ___selectedDesc.text = string.Empty;
        }
        else if (___CurrentMaxCraft > 0)
        {
            if (itemType >= ItemList.Type.BADGE_START && itemType <= ItemList.Type.BADGE_MAX)
            {
                ___iconbg.enabled = true;
                ___costBox.gameObject.SetActive(value: true);
                ___costTitle.text = Localize.GetLocalizeTextWithKeyword("COST_NAME", contains: false);
                ___costValue.text = GemaItemManager.Instance.GetItemCost(data.getItemTyp()).ToString();
            }
            else if (itemType.ToString().Contains("Useable"))
            {
                ___costBox.gameObject.SetActive(value: true);
                ___costTitle.text = Localize.GetLocalizeTextWithKeyword("ITEMLIMIT_NAME", contains: false);
                ___costValue.text = SaveManager.Instance.GetItemCountInBag(itemType) + "/" + GemaItemManager.Instance.GetItemCost(itemType);
            }
            ___selectedIcon.enabled = true;
            if (__instance.isSortMode)
            {
                ___selectedIcon.sprite = t.Field("bagItems").GetValue<GemaUIPauseMenu_ItemGridSub[]>()[(int)t.Field("sortingSelected").GetValue<byte>()].GetSprite();
                ___selectedName.color = Color.white;
                ___selectedName.text = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString(itemType), contains: false);
            }
            else
            {
                ___selectedIcon.sprite = ___craftList[___selected].GetSprite();
                ___selectedName.color = ___craftList[___selected].GetColor();
                ___selectedName.text = ___craftList[___selected].GetText();
            }
            if (itemType.ToString().Contains("_OrbBoost"))
            {
                ___selectedDesc.text = "<font-weight=200>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.ORBBOOSTSERIES", contains: false);
                ___selectedDesc.text = Localize.AddColorToBadgeDesc(___selectedDesc.text);
            }

            else if (itemType.ToString().Contains("_OrbType"))
            {
                ___selectedDesc.text = "<font-weight=200>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.ORBTYPESERIES", contains: false);
                ___selectedDesc.text = Localize.AddColorToBadgeDesc(___selectedDesc.text);
            }
            else
            {
                if(itemType.ToString().Contains("ITEM")){
                    ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(Randomizer.getItemPair(itemType,true));
                }
                else
                ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data.getItemTyp());

                if (___selectedDesc.text.Contains("[c2]"))
                {
                    if (___craftList[___selected].isUpgrade)
                    {
                        ___selectedDesc.text = Localize.FilterLevelDescFromItem2(data.getItemTyp(), ___selectedDesc.text);
                    }
                    else
                    {
                        ___selectedDesc.text = Localize.FilterLevelDescFromItem(data.getItemTyp(), ___selectedDesc.text);
                    }
                }
                if (itemType == ItemList.Type.Useable_WaffleWonderTemp)
                {
                    int num = (int)(100f * (1f * (float)(int)SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) / 10f));
                    if (num > 20)
                    {
                        num += 5;
                    }
                    if (num >= 85)
                    {
                        num += 5;
                    }
                    TextMeshProUGUI textMeshProUGUI = ___selectedDesc;
                    textMeshProUGUI.text = textMeshProUGUI.text + "<br>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC2.Useable_WaffleWonderTemp", contains: false) + " <color=#FFF>" + num + "%</color>";
                }
                if (itemType.ToString().Contains("Useable"))
                {
                    TextMeshProUGUI textMeshProUGUI2 = ___selectedDesc;
                    textMeshProUGUI2.text = textMeshProUGUI2.text + "<br>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.USEABLETIPS", contains: false);
                }
                if (itemType == ItemList.Type.Function_MaterialExchangeA || itemType == ItemList.Type.Function_MaterialExchangeB)
                {
                    TextMeshProUGUI textMeshProUGUI3 = ___selectedDesc;
                    textMeshProUGUI3.text = textMeshProUGUI3.text + "<br><br><color=#BBB>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.FUNCTIONRAREREMAIN", contains: false) + "</color>";
                    TextMeshProUGUI textMeshProUGUI = ___selectedDesc;
                    textMeshProUGUI.text = textMeshProUGUI.text + "<br><sprite=12>" + Localize.GetLocalizeTextWithKeyword("Colon", contains: false) + "<color=#D92>  " + SaveManager.Instance.GetCoreExchange() + "</color><color=#2CC> / " + (SaveManager.Instance.GetChapter() + 1);
                    textMeshProUGUI = ___selectedDesc;
                    textMeshProUGUI.text = textMeshProUGUI.text + "<br><sprite=13>" + Localize.GetLocalizeTextWithKeyword("Colon", contains: false) + "<color=#D92>  " + SaveManager.Instance.GetUpgradeExchange() + "</color><color=#2CC> / " + (SaveManager.Instance.GetChapter() + 1);
                }
                if (itemType == ItemList.Type.ITEM_OrbAmulet && SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbAmulet) <= 0)
                {
                    int num2 = ___selectedDesc.text.IndexOf("<br>");
                    if (num2 >= 0)
                    {
                        ___selectedDesc.text = ___selectedDesc.text.Substring(0, num2);
                    }
                }
            }
            ___selectedDesc.text = InputButtonManager.Instance.AddButtonsToPromote(___selectedDesc.text);
            ___mrequiredText.text = Localize.GetLocalizeTextWithKeyword("MATERIALREQUIRED", contains: false);
            ___mownedText.text = Localize.GetLocalizeTextWithKeyword("MATERIALOWNED", contains: false);
            ___useableText.text = Localize.GetLocalizeTextWithKeyword("CONSUMEOWNED", contains: false);
            if (!___setFontOutline)
            {
                ___mownedText.outlineColor = Color.black;
                ___mownedText.outlineWidth = 0.35f;
                ___useableText.outlineColor = Color.black;
                ___useableText.outlineWidth = 0.35f;
                Material material = new Material(___mrequiredText.fontSharedMaterial);
                material.SetFloat("_FaceDilate", 0.1f);
                material.SetFloat("_OutlineWidth", 0.1725f);
                material.SetFloat("_Sharpness", 1f);
                ___mrequiredText.fontSharedMaterial = material;
                ___mrequiredText.UpdateMeshPadding();
                ___setFontOutline = true;
            }
            if (___craftList[___selected].GetCanCraft())
            {
                ___mrequiredText.text += "<color=#4A6>";
                ___mrequiredText.text += Localize.GetLocalizeTextWithKeyword("CRAFTMENU_CANCRAFT1", contains: false);
            }
            else
            {
                ___mrequiredText.text += "<color=#A46>";
                ___mrequiredText.text += Localize.GetLocalizeTextWithKeyword("CRAFTMENU_CANCRAFT0", contains: false);
            }
            ___selectedIcon.color = Color.white;
            for (int i = 0; i < ___materialrequiredList.Length; i++)
            {
                int mat = __instance.GetMat(itemType, i + 1);
                int num3 = i;
                if (num3 == GemaItemManager.Instance.maxMaterial - 1)
                {
                    num3 = -1;
                }
                ___materialrequiredList[i].text = "<sprite=" + (12 + num3) + "> ";
                if (mat <= 0)
                {
                    ___materialrequiredList[i].transform.parent.gameObject.SetActive(value: false);
                }
                else
                {
                    ___materialrequiredList[i].transform.parent.gameObject.SetActive(value: true);
                    if (SaveManager.Instance.GetResource((ItemList.Resource)((i + 1) % GemaItemManager.Instance.maxMaterial)) < mat)
                    {
                        ___materialrequiredList[i].color = new Color32(178, 15, 25, byte.MaxValue);
                    }
                    else
                    {
                        ___materialrequiredList[i].color = new Color32(88, 182, 209, byte.MaxValue);
                    }
                }
                ___materialrequiredList[i].text += mat.ToString("00");
                if (mat > 0)
                {
                    ___currentMaterialNeeded[i] = 1;
                }
                else
                {
                    ___currentMaterialNeeded[i] = 0;
                }
            }
            ___currentItemType = ___craftList[___selected].GetItemType(); ;
        }
        if (!___selectedIcon.enabled)
        {
            ___selectedIcon.color = new Color(0f, 0f, 0f, 1f);
            ___selectedName.text = string.Empty;
            ___selectedDesc.text = string.Empty;
        }
        return false;
    }








    //Crafting menu
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "AddItem")]
    [HarmonyPrefix]
    static bool disableCraftedItems(ref GemaUIPauseMenu_CraftGrid __instance, ref ItemList.Type iType, ref bool isUpgrade, ref byte isImportant, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___CurrentMaxCraft, ref GameObject[] ___specialcrafts)
    {

        Traverse o = Traverse.Create(__instance);
        ItemData data = Randomizer.getRandomizedItem(iType, 1);

        if (o.Field("CurrentMaxCraft").GetValue<int>() >= ___craftList.Length)
        {
            return false;
        }

        if (iType.ToString().Contains("BADGE"))
        {
            if (data.getItemTyp().ToString().Contains("STACKABLE"))
            {
                if (SaveManager.Instance.GetStackableItem(data.getItemTyp(), data.getSlotId()))
                {
                    return false;
                }
            }
            else if (data.getItemTyp() >= ItemList.Type.BADGE_START && data.getItemTyp() <= ItemList.Type.BADGE_MAX && SaveManager.Instance.GetItem(data.getItemTyp()) > 0)
            {
                return false;
            }
        }

        if (iType.ToString().Contains("OrbType"))
        {
            if (SaveManager.Instance.GetOrbTypeObtained() >= 4)
            {
                return false;
            }
        }
        else if (iType.ToString().Contains("OrbBoost"))
        {
            if (SaveManager.Instance.GetOrbBoostObtained() >= 2)
            {
                return false;
            }
        }
        else if (iType.ToString().Contains("ITEM")) // Find multiple items of the same type on the overworld?
        {
            iType = Randomizer.getItemPair(iType, false);
            if ((SaveManager.Instance.GetItem(iType) > 0 && !isUpgrade)
            || (SaveManager.Instance.GetItem(iType) <= 0 && isUpgrade)
            || (SaveManager.Instance.GetItem(iType) >= 3 && isUpgrade))
            { return false; }
        }

        if (iType != 0)
        {
            GameObject gameObject = null;
            if (isImportant > 0 && isImportant - 1 < ___specialcrafts.Length)
            {
                gameObject = ___specialcrafts[isImportant - 1];
                gameObject.gameObject.SetActive(value: true);
            }
            ___craftList[___CurrentMaxCraft].SetItem(iType, isUpgrade, gameObject);
            ___CurrentMaxCraft++;
        }
        return false;
    }

    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGridSlot), "SetItem")]
    [HarmonyPrefix]

    static bool removeUpdateMadness(ref ItemList.Type itype, GameObject important, ref ItemList.Type ___itemType, ref GemaUIPauseMenu_CraftGridSlot __instance, ref bool _isUpgrade, ref UI.Image ___iconbg, ref TextMeshPro ___nameText, ref TextMeshPro ___carryText, ref UI.Image ___canCrarftLightBG)
    {
        

        ItemData data = Randomizer.getRandomizedItem(itype, 1);
        __instance.isUpgrade = _isUpgrade;
        __instance.SetVisible(isVisible: true);
        ___itemType = itype;

        ___iconbg.enabled = false;
        __instance.UpdateIcon(itype);

 
        byte b = 0;
        if (itype == ItemList.Type.Function_MaterialExchangeA || itype == ItemList.Type.Function_MaterialExchangeB)
        {
            b = 3;
        }
        else if (itype >= ItemList.Type.BADGE_START && itype <= ItemList.Type.BADGE_MAX)
        {
            b = 1;
            __instance.UpdateIcon(data.getItemTyp());

        }
        else if (itype.ToString().Contains("_OrbType") || itype.ToString().Contains("_OrbBoost"))
        {
            b = 2;
        }
        else if (itype.ToString().Contains("ITEM"))
        {
            b = (byte)(__instance.isUpgrade ? 4 : 2);
            __instance.UpdateIcon(Randomizer.getItemPair(itype, true));


        }
        ___nameText.color = GemaUIPauseMenu_CraftGrid.Instance.itemTypeColor[b];
        bool flag = true;
        if (itype == ItemList.Type.Function_MaterialExchangeA || itype == ItemList.Type.Function_MaterialExchangeB)
        {
            flag = GemaUIPauseMenu_CraftGrid.Instance.canExchange(itype);
        }
        else
        {
            for (int i = 0; i < GemaItemManager.Instance.maxMaterial; i++)
            {
                if (GemaUIPauseMenu_CraftGrid.Instance.GetMat(itype, i) > SaveManager.Instance.GetResource((ItemList.Resource)i))
                {
                    flag = false;
                    break;
                }
            }
        }
        int num = 0;
        int num2 = 0;
        switch (b)
        {
            case 0:
                num = SaveManager.Instance.GetItemCountInBag(itype);
                num2 = GemaItemManager.Instance.GetItemCost(itype);
                break;
            case 1:
                if (data.getItemTyp().ToString().Contains("STACKABLE"))
                {
                    num = (SaveManager.Instance.GetStackableItem(data.getItemTyp(), data.getSlotId()) ? 1 : 0);
                }
                else
                {
                    num = ((SaveManager.Instance.GetItem(data.getItemTyp()) > 0) ? 1 : 0);

                }
                num2 = 1;
                break;
            case 2:
                if (itype.ToString().Contains("_OrbType"))
                {
                    num = SaveManager.Instance.GetOrbTypeObtained();
                    num2 = 4;
                }
                else if (itype.ToString().Contains("_OrbBoost"))
                {
                    num = SaveManager.Instance.GetOrbBoostObtained();
                    num2 = 2;
                }
                else
                {
                    num = ((SaveManager.Instance.GetItem(itype) > 0) ? 1 : 0);
                    num2 = 1;
                }
                break;
            case 4:
                num = SaveManager.Instance.GetItem(itype);
                num2 = 3;
                break;
            case 3:
                num = 9 - SaveManager.Instance.GetFunctionExchangeRemain();
                num2 = 9;
                break;
        }
        ___carryText.text = num + "/" + num2;
        if (num == num2)
        {
            ___carryText.color = new Color32(154, 150, 228, byte.MaxValue);
        }
        else
        {
            ___carryText.color = new Color32(206, 247, byte.MaxValue, byte.MaxValue);
        }
        __instance.CanCraft(flag, updateps: true);
        if (important != null)
        {
            if (flag)
            {
                important.transform.SetParent(__instance.transform);
                important.transform.localPosition = ___canCrarftLightBG.transform.localPosition;
                important.SetActive(value: true);
            }
            else
            {
                important.SetActive(value: false);
            }
        }
        __instance.UpdateSlotNew();
        return false;
    }
}




/*
 * Crafting Item -> give random item depending on numcrafted 
 * map Upgrade items to Item_XX
 * this will likely break with the next DLC
 */