using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;



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



        var instance = new Harmony("Randomizer");
        instance.PatchAll(typeof(Randomizer));
        Logger.LogInfo($"Plugin Randomizer is loaded!");



    }

    static ItemData loadRandomizedItem(ItemList.Type itemid, byte slotid)
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

        ItemData data = loadRandomizedItem(type, value);
        type = (ItemList.Type)data.itemID;
        value = (byte)data.slotID;
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
        ItemData data = loadRandomizedItem(__instance.itemid, ___slotid);
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
            if (SaveManager.Instance.GetItem((ItemList.Type)data.itemID) > 0)
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



    //Crafting UI TextDescription change

    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGridSlot), "GetItemType")]
    [HarmonyPrefix]
    static bool IHopeIdontBreakStuff(ItemList.Type ___itemType, ref ItemList.Type __result)
    {
        if (__itemData.ToString().Contains("OrbType")) return false;
        __result = loadRandomizedItem(___itemType, 1).getItemTyp();
        return false;
    }

    //Fix item selection caused by randomiying
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "UpdateSelectedText")]
    [HarmonyPostfix]
    static void reverseItemType(ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, int ___selected, ref ItemList.Type ___currentItemType)
    {
        ___currentItemType = Traverse.Create(___craftList[___selected]).Field("itemType").GetValue<ItemList.Type>();

    }


    //Crafting menu
    //NO POTIONS!!!!!!!!!!!
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "AddItem")]
    [HarmonyPrefix]
    static bool disableCraftedItems(ref GemaUIPauseMenu_CraftGrid __instance, ref ItemList.Type iType, ref bool isUpgrade, ref byte isImportant, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___CurrentMaxCraft, ref GameObject[] ___specialcrafts)
    {

        Traverse o = Traverse.Create(__instance);
        ItemData data = loadRandomizedItem(iType,1);
        Debug.LogWarning(SaveManager.Instance.GetOrbTypeObtained());
        Debug.LogWarning(data.getItemTyp().ToString());

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
            //progression not possible for now maybe later
            if (SaveManager.Instance.GetOrbTypeObtained() >= 4)
            {
                
                return false;
            }
        }
        else if (iType.ToString().Contains("OrbBoost"))
        {
            //this ONE can be randomized
            if (SaveManager.Instance.GetOrbBoostObtained() >= 2)
            {
                return false;
            }
        }
        else if (iType.ToString().Contains("ITEM") && ((SaveManager.Instance.GetItem(iType) > 0 && !isUpgrade) || (SaveManager.Instance.GetItem(iType) <= 0 && isUpgrade) || (SaveManager.Instance.GetItem(iType) >= 3 && isUpgrade))) // Find multiple items of the same type on the overworld?
        {
            return false;
        }
        if (iType != 0)
        {
            GameObject gameObject = new GameObject();
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

    //replicate SetItem
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGridSlot), "SetItem")]
    [HarmonyPrefix]

    static void removeUpdateMadness(ref ItemList.Type itype, ref ItemList.Type __state, GameObject important, ref ItemList.Type ___itemType)
    {
        if (important == null)
        {
            itype = ___itemType;
        }
        __state = 0;
        if (itype >= ItemList.Type.BADGE_START && itype <= ItemList.Type.BADGE_MAX)
        {
            __state = itype;
            itype = loadRandomizedItem(itype, 1).getItemTyp();
        }

    }
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGridSlot), "SetItem")]
    [HarmonyPostfix]
    static void Part2(ref ItemList.Type __state, ref ItemList.Type ___itemType)
    {
        if (__state > 0)
        {
            ___itemType = __state;
        }
    }



    
    //Craftig Orb Fix
    //No set SlotId, maybe reserver slots for Potions?
    [HarmonyPatch(typeof(SaveManager),"GetOrbTypeObtained")]
    [HarmonyPostfix]
    static void orbTypeFix(ref int __result,ref SaveManager __instance)
    {
        int num = 0;
        ItemData data1 = loadRandomizedItem(ItemList.Type.ITEM_OrbTypeC2, 1);
        ItemData data2 = loadRandomizedItem(ItemList.Type.ITEM_OrbTypeC3, 1);
        ItemData data3 = loadRandomizedItem(ItemList.Type.ITEM_OrbTypeS2, 1);
        ItemData data4 = loadRandomizedItem(ItemList.Type.ITEM_OrbTypeS3, 1);

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
    static void OrbBoostCount(ref int __result,SaveManager __instance)
    {
        int num = 0;
        ItemData data1 = loadRandomizedItem(ItemList.Type.ITEM_OrbBoostD,1);
        ItemData data2 = loadRandomizedItem(ItemList.Type.ITEM_OrbBoostU,1);
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
                data = loadRandomizedItem(item, (byte)(___ShopID + 30));
            else
                data = loadRandomizedItem(item, 1);


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
            ItemData data = loadRandomizedItem(ItemList.Type.ITEM_ORB, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            data = loadRandomizedItem(ItemList.Type.ITEM_KNIFE, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            em.SetStage(30);
        }
    }


    [HarmonyPatch(typeof(Chap1FreeRoamVena7x7), "REQUIREMENT")]
    [HarmonyPrefix]
    static bool Vena7x7Fix(ref bool __result)
    {
        ItemData data = loadRandomizedItem(ItemList.Type.STACKABLE_COG, 23);

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
                        ItemData data = loadRandomizedItem(ItemList.Type.STACKABLE_COG, 23);
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


