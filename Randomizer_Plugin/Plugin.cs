using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;
using TMPro;
using Character;

using UnityEngine.UI;
using UnityEngine;












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







[BepInPlugin("tevi.plugins.randomizer", "Randomizer", "0.9.0.0")]
[BepInProcess("TEVI.exe")]
public class Randomizer : BaseUnityPlugin
{

    static Dictionary<ItemData, ItemData> __itemData = new Dictionary<ItemData, ItemData>(new ItemData.EqualityComparer());

    public enum Upgradable
    {
        ITEM_KNIFE = 10,
        ITEM_ORB = 11,
        ITEM_RapidShots = 12,
        ITEM_AttackRange = 13,
        ITEM_EasyStyle = 14,
        ITEM_LINEBOMB = 15,
        ITEM_AREABOMB = 16,
        ITEM_SPEEDUP = 17,
        ITEM_AirDash = 18,
        ITEM_WALLJUMP = 19,
        ITEM_JETPACK = 20,
        ITEM_BoostSystem = 21,
        ITEM_BombLengthExtend = 22,
        ITEM_MASK = 23,
        ITEM_TempRing = 24,
        ITEM_DodgeShot = 25,
        ITEM_Rotater = 26,
        ITEM_GoldenGlove = 27,
        ITEM_OrbAmulet = 28,
        ITEM_BOMBFUEL = 29,
        ITEM_Explorer = 30,
    }

    public enum EventID
    {
        IllusionPalace = 9999
    }

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
                    Logger.LogWarning($"Already changed {data1.getItemTyp()} slot {data1.getSlotId()}");

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
        instance.PatchAll(typeof(CraftingPatch));
        instance.PatchAll(typeof(ShopPatch));
        instance.PatchAll(typeof(EventPatch));

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
            //Debug.LogError("[Randomizer] Could not load the randomized Item " + itemid.ToString() + "! It has the ID: " + (int)itemid + " and the SlotID: " + slotid);
            data = new ItemData((int)itemid, (int)slotid);
        }
        return data;
    }

    static public ItemData getRandomizedItem(int itemid, int slotid)
    {
        ItemData data;
        try
        {
            //Debug.Log($"[Randomizer] Load item {itemid}:{slotid}");

            data = __itemData[new ItemData(itemid, slotid)];
            //Debug.Log($"[Randomizer] Found item {(ItemList.Type)data.itemID}:{data.slotID}");
        }
        catch
        {
            //Debug.LogError("[Randomizer] Could not load the randomized Item " + itemid.ToString() + "! It has the ID: " + (int)itemid + " and the SlotID: " + slotid);
            data = new ItemData(itemid, slotid);
        }
        return data;
    }


    // change how the item Bell Works
    [HarmonyPatch(typeof(CharacterBase),"UseItem")]
    [HarmonyPrefix]
    static bool WarpBell(ref ItemList.Type item,ref bool playvoice,ref ObjectPhy ___phy_perfer, ref playerController ___playerc_perfer)
    {
        if(item == ItemList.Type.Useable_Bell)
        {
            int num5 = (int)___phy_perfer.GetCounter(4);
            EventManager.Instance.StartWarp(1, 1, 1, 1);
            SaveManager.Instance.RemoveItemFromBagSlot(num5);
            HUDResourceGotPopup.Instance.AddPopup(item, useTop: true, forcepop: true);
            if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_ConsumeableCharge))
            {
                SaveManager.Instance.SetItem(ItemList.Type.BADGE_ConsumeableCharge, (byte)(SaveManager.Instance.GetItem(ItemList.Type.BADGE_ConsumeableCharge) + 1), output: false);
            }
            if (playvoice)
            {
                if (item == ItemList.Type.Useable_BSnack)
                {
                    ___playerc_perfer.PlayItemVoice(isbad: true);
                }
                else
                {
                    ___playerc_perfer.PlayItemVoice(isbad: false);
                }
            }

            return false;
        }
        return true;
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
        //if (Enum.IsDefined(typeof(Upgradable),type.ToString()))
            //return;

        ItemData data = getRandomizedItem(type, value);

        value = (byte)data.slotID;
        type = (ItemList.Type)data.itemID;



    }
    //craftingMenuRefresh
    [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
    [HarmonyPostfix]
    static void CraftingRefresh()
    {
        if(GemaUIPauseMenu_CraftGrid.Instance != null) 
            Traverse.Create(GemaUIPauseMenu_CraftGrid.Instance).Method("UpdateCraftList").GetValue();
        else
        {
            Debug.LogWarning("This was triggerd to Early");
        }
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
            else
            {
                GameObject secondTile = new GameObject("SecondTile");
                secondTile.layer = 14;
                secondTile.AddComponent<SpriteRenderer>();
                SpriteRenderer sSP = secondTile.GetComponent<SpriteRenderer>();
                sSP.material = ____sprite.material;
                sSP.sortingOrder = 45;
                secondTile.transform.SetParent(__instance.transform);
                secondTile.transform.localPosition=new Vector3(0,0,0);
                secondTile.transform.localScale = new Vector3(1,1,1);
                ____secondsprite = sSP;
                ____secondsprite.sprite = spr;
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
            else if (data.getItemTyp().ToString().Contains("ITEM"))
            {
                Upgradable type;
                if (Enum.TryParse(data.getItemTyp().ToString(), out type))
                {
                    if (SaveManager.Instance.GetStackableItem((ItemList.Type)type, data.getSlotId()))
                    {
                        Debug.Log("[ItemTile] Item " + ((ItemList.Type)data.itemID).ToString() + " visible in camera. Removed from map because player already obtained it. GotItem = " + SaveManager.Instance.GetItem((ItemList.Type)data.itemID).ToString());
                        __instance.DisableMe();
                        return false;
                    }
                }
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
        Upgradable item;

        if (!data1.getItemTyp().ToString().Contains("STACKABLE"))
        {

            if(Enum.TryParse(data1.getItemTyp().ToString(),out item))
            {
                if(SaveManager.Instance.GetStackableItem((ItemList.Type)item, data1.getSlotId()))
                    num++;
            }
            else if (__instance.GetItem(data1.getItemTyp()) > 0)
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
            if (Enum.TryParse(data2.getItemTyp().ToString(), out item))
            {
                if(SaveManager.Instance.GetStackableItem((ItemList.Type)item, data2.getSlotId()))
                    num++;
            }
            else if (__instance.GetItem(data2.getItemTyp()) > 0)
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
            if (Enum.TryParse(data3.getItemTyp().ToString(), out item))
            {
                if(SaveManager.Instance.GetStackableItem((ItemList.Type)item, data3.getSlotId()))
                    num++;
            }
            else if (__instance.GetItem(data3.getItemTyp()) > 0)
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
            if (Enum.TryParse(data4.getItemTyp().ToString(), out item))
            {
                if (SaveManager.Instance.GetStackableItem((ItemList.Type)item, data4.getSlotId()))
                    num++;
            }
            else if (__instance.GetItem(data4.getItemTyp()) > 0)
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
        switch (item)
        {

            case ItemList.Type.ITEM_ORB:

                addOrbStatus(1);

                break;

            default:
                break;
        }
        return true;
    }


    [HarmonyPatch(typeof(SaveManager), "SetItem")]
    [HarmonyPrefix]
    static bool ItemChanges(ref ItemList.Type item, ref byte value, ref SaveManager __instance)
    {
        ItemData data = getRandomizedItem(item, value);
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

                    if (value > 3)
                    {
                        value = (byte)(__instance.GetItem(item) + 1);
                        if (!__instance.GetStackableItem((ItemList.Type)itemRef, 0)){
                            __instance.SetStackableItem((ItemList.Type)itemRef, 0, true);
                        }
                    }
                    else if(value > 1 && value<=3)
                    {
                        item = data.getItemTyp();
                        value = data.getSlotId();
                        if (item.ToString().Contains("STACKABLE"))
                        {
                            __instance.SetStackableItem(item, value, true);
                        }
                        if (Enum.TryParse<Upgradable>(data.getItemTyp().ToString(), out itemRef))
                        {
                            value = (byte)(__instance.GetItem(data.getItemTyp()) + 1);
                        }
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





}


class EventPatch
{
    // Free Start Items
    [HarmonyPatch(typeof(Chap0GetKnife), "EVENT")]
    [HarmonyPrefix]
    static void StartEvent()
    {
        EventManager em = EventManager.Instance;

        if (em.EventStage == 10)
        {
            SaveManager.Instance.SetOrb((byte)0);
            Randomizer.addOrbStatus(3);
            ItemData data = Randomizer.getRandomizedItem(ItemList.Type.ITEM_ORB, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            data = Randomizer.getRandomizedItem(ItemList.Type.ITEM_KNIFE, 1);
            SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            em.SetStage(30);
        }
    }


    [HarmonyPatch(typeof(Chap1FreeRoamVena7x7), "REQUIREMENT")]
    [HarmonyPrefix]
    static bool Vena7x7Fix(ref bool __result)
    {
        ItemData data = Randomizer.getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);

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
                        ItemData data = Randomizer.getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);
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
                        if(component.mode == Mode.BOSS_TAHLIA || component.mode == Mode.BOSS_REVENANCE)
                        {
                            if (WorldManager.Instance.CheckIsWall(component.transform.position, any: false) == 1)
                            {
                                Debug.Log("[EventDetect] Event is inside wall, cannot trigger : " + component.mode);
                                flag = false;
                            }
                            if (EventManager.Instance.TryStartEvent(component.mode, force: false))
                            {
                                EventManager.Instance.LastHitTrigger = component;
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


    //End Requierment
    static bool EventReq(int customEventId)
    {
        bool flag = false;


        switch (Randomizer.getRandomizedItem(customEventId, 1).slotID)
        {
            case 1:
                flag = SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_COG) < MainVar.instance.FREEROAM_COGNEEDED;
                break;
            default:
                flag = SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_COG) < MainVar.instance.FREEROAM_COGNEEDED;
                break;
        }
        

        return flag;
    }


    [HarmonyPatch(typeof(Chap8FreeRoamNoIllusionPalace7x7), "REQUIREMENT")]
    [HarmonyPrefix]
    static bool IllusionReq(ref bool __result)
    {
        __result = false;
        if (EventReq((int)Randomizer.EventID.IllusionPalace)) 
        {
            __result = true;
        }
        return false;
    }
}



class CraftingPatch
{
    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "UpdateSelectedText")]
    [HarmonyPrefix]
    static bool itemDescChange(ref GemaUIPauseMenu_CraftGrid __instance, ref Image ___iconbg, ref Transform ___costBox, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___selected, ref TextMeshProUGUI ___selectedName, ref TextMeshProUGUI ___selectedDesc,
        ref int ___CurrentMaxCraft, ref TextMeshProUGUI ___costTitle, ref TextMeshProUGUI ___costValue, ref Image ___selectedIcon, ref TextMeshProUGUI ___mownedText, ref TextMeshProUGUI ___useableText, ref bool ___setFontOutline, ref TextMeshProUGUI[] ___materialrequiredList,
        ref TextMeshProUGUI ___mrequiredText, ref byte[] ___currentMaterialNeeded, ref ItemList.Type ___currentItemType)
    {
        Traverse t = Traverse.Create(__instance);



        ___iconbg.enabled = false;
        ___costBox.gameObject.SetActive(value: false);
        ItemData data = Randomizer.getRandomizedItem(___craftList[___selected].GetItemType(), 1);



        ItemList.Type itemType = ___craftList[___selected].GetItemType();
        if (itemType.ToString().Contains("ITEM"))
        {
            data = Randomizer.getRandomizedItem(itemType, (byte)(SaveManager.Instance.GetItem(itemType) + 1));
            if (___craftList[___selected].isUpgrade)
            {
                data = Randomizer.getRandomizedItem(itemType, (byte)(getItemUpgradeCount(itemType)+1));
            }
        }

        if (__instance.isSortMode)
        {
            itemType = t.Field("bagItems").GetValue<GemaUIPauseMenu_ItemGridSub[]>()[(int)t.Field("sortingSelected").GetValue<byte>()].GetItemType();
        }
        if (itemType == ItemList.Type.OFF)
        {
            ___selectedIcon.enabled = false;
            ___costBox.gameObject.SetActive(value: false);

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
            else if (data.getItemTyp().ToString().Contains("ITEM"))
            {
                Randomizer.Upgradable upitem;
                if(Enum.TryParse(data.getItemTyp().ToString(),out upitem)&&SaveManager.Instance.GetStackableItem((ItemList.Type)upitem,data.getSlotId()))
                {
                    return false;
                }
                else if (SaveManager.Instance.GetItem(data.getItemTyp()) > 0)
                {
                    return false;
                }
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
            if ((getItemUpgradeCount(iType) > 0 && !isUpgrade)
            || (getItemUpgradeCount(iType) <= 0 && isUpgrade)
            || (getItemUpgradeCount(iType) >= 3 && isUpgrade))
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

    static bool removeUpdateMadness(ref ItemList.Type itype, GameObject important, ref ItemList.Type ___itemType, ref GemaUIPauseMenu_CraftGridSlot __instance, ref bool _isUpgrade, ref Image ___iconbg, ref TextMeshPro ___nameText, ref TextMeshPro ___carryText, ref Image ___canCrarftLightBG)
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
                num = getItemUpgradeCount(itype);
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

    //Update need to be fixed for progessive items

    static int getItemUpgradeCount(ItemList.Type _item)
    {
        int num = 0;
        Randomizer.Upgradable item;
        if (Enum.TryParse(_item.ToString(), out item))
        {
            if (SaveManager.Instance.GetStackableItem((ItemList.Type)item, 0))
            {
                num++;
            }
            if (SaveManager.Instance.GetStackableItem((ItemList.Type)item, 2))
            {
                num++;
            }
            if (SaveManager.Instance.GetStackableItem((ItemList.Type)item, 3))
            {
                num++;
            }
        }
        return num;
    }


    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "Update")]
    [HarmonyPrefix]
    static bool progressiveItemCrafting(ref GemaUIPauseMenu_CraftGrid __instance, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___selected, ref ItemList.Type ___currentItemType, ref GemaUIPauseMenu_CraftMaterialSlot[] ___materialownedList,
        ref byte[] ___currentMaterialNeeded, ref float ___isJustCraftedBadge, ref float ___errorflashing, ref float ___flashing, Image ___synthesisBox, Image ___synthesisBoxOutline, ref GameObject[] ___specialcrafts, ref Image ___craftedFlash,
        ref GemaUIPauseMenu_ItemGridSub[] ___bagItems,ref (Character.OrbType, OrbShootType[],bool) __state)
    {
        Traverse trav = Traverse.Create(__instance);
        __state.Item3 = false;
        if (InputButtonManager.Instance.GetButtonDown(13) && !HUDObtainedItem.Instance.isDisplaying())
        {
            __state.Item1 = EventManager.Instance.mainCharacter.cphy_perfer.orbUsing;
            __state.Item2 = EventManager.Instance.mainCharacter.cphy_perfer.orbShootType;
            __state.Item3 = true;
            if (___currentItemType.ToString().Contains("ITEM") && ___craftList[___selected].isUpgrade || ___currentItemType.ToString().Contains("BADGE"))
            {

                int num5 = 1;
                ItemList.Resource resource = ItemList.Resource.COIN;


                if (getItemUpgradeCount(___currentItemType) >= 3) // helperfunction 
                {
                    num5 = -3;
                }
                ItemData rnd = Randomizer.getRandomizedItem(___currentItemType, 1);
                if (!Enum.IsDefined(typeof(Randomizer.Upgradable), ___currentItemType.ToString())&&SaveManager.Instance.GetItem(rnd.getItemTyp())>0)
                {
                    
                    num5 = -3;
                }
                if (num5 >= 1)
                {
                    for (int m = 0; m < GemaItemManager.Instance.maxMaterial; m++)
                    {
                        if (__instance.GetMat(___currentItemType, m) > SaveManager.Instance.GetResource((ItemList.Resource)m))
                        {
                            int num6 = m - 1;
                            if (num6 < 0)
                            {
                                num6 = GemaItemManager.Instance.maxMaterial - 1;
                            }
                            else
                            {
                                ___materialownedList[num6].SetFlash(byte.MaxValue, 0, 0, byte.MaxValue);
                            }
                            ___currentMaterialNeeded[num6] = 2;
                            num5 = 0;
                        }
                    }
                }
                if (EventManager.Instance.isBossMode())
                {
                    num5 = -5;
                }
                if (num5 <= 0)
                {
                    CameraScript.Instance.PlaySound(AllSound.SEList.MENUFAIL);
                    if (num5 == 0)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.NotEnoughMaterials");
                        ___errorflashing = 0.8f;
                    }
                    if (num5 == -1)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.BagFull");
                    }
                    if (num5 == -2)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.ItemLimitReached");
                    }
                    if (num5 == -3)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.LevelLimitReached");
                        Debug.LogWarning($"[Randomizer] Already has {rnd.getItemTyp().ToString()}");
                    }
                    if (num5 == -4)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.AlreadyOwned");
                    }
                    if (num5 == -5)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.NoCraftingInBattle");
                    }
                    if (num5 == -6)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.ExchangeFailA");
                    }
                    if (num5 == -7)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.ExchangeAllUsed");
                    }
                    if (num5 == -8)
                    {
                        GemaUIPauseMenu_BottomBarPrompt.Instance.ShowErrorText("BottomBarError.ExchangeFailB");
                    }
                    return false;
                }
                else
                {
                    for (int n = 0; n < GemaItemManager.Instance.maxMaterial; n++)
                    {
                        if (n > 0)
                        {
                            ___materialownedList[(n - 1) % GemaItemManager.Instance.maxMaterial].NoAdd();
                        }
                    }
                    ItemList.Type type = ItemList.Type.OFF;
                    for (int num7 = 0; num7 < GemaItemManager.Instance.maxMaterial; num7++)
                    {
                        if (__instance.GetMat(___currentItemType, num7) <= 0)
                        {
                            continue;
                        }
                        SaveManager.Instance.SubResource((ItemList.Resource)num7, __instance.GetMat(___currentItemType, num7));
                        if (num7 > 0)
                        {
                            ___materialownedList[(num7 - 1) % GemaItemManager.Instance.maxMaterial].SetFlash(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                            if (___currentItemType == ItemList.Type.Function_MaterialExchangeA || ___currentItemType == ItemList.Type.Function_MaterialExchangeB)
                            {
                                ___materialownedList[(num7 - 1) % GemaItemManager.Instance.maxMaterial].SetAdd(byte.MaxValue, 0, 0, -9);
                                resource = (ItemList.Resource)num7;
                                break;
                            }
                            ___materialownedList[(num7 - 1) % GemaItemManager.Instance.maxMaterial].SetAdd(byte.MaxValue, 0, 0, -__instance.GetMat(___currentItemType, num7));
                        }
                    }
                    int num8 = -1;

                    if (___craftList[___selected].isUpgrade)
                    {
                        //HUDObtainedItem.Instance.GiveItem(___currentItemType, (byte)(getItemUpgradeCount(___currentItemType) + 1)); // helper function to get current craft number
                        SaveManager.Instance.SetItem(___currentItemType, (byte) (getItemUpgradeCount(___currentItemType) + 1), true);
                        ___isJustCraftedBadge = 1.75f;
                    }
                    else
                        HUDObtainedItem.Instance.GiveItem(___currentItemType, 1);

                        Debug.Log("[Craft] Crafting " + ___currentItemType);
                    ___flashing = 0.333f;


                    CameraScript.Instance.PlaySound(AllSound.SEList.LEVELUP);
                    ___synthesisBox.color = new Color(0f, 1f, 1f, 1f);
                    ___synthesisBoxOutline.color = new Color(0f, 1f, 1f, 1f);
                    ___synthesisBoxOutline.transform.localScale = new Vector3(1f, 1f, 1f);
                    trav.Method("UpdateSelectedText").GetValue();
                    trav.Method("UpdateTotalText").GetValue();
                    trav.Method("UpdateBag").GetValue();
                    for (int num14 = 0; num14 < ___specialcrafts.Length; num14++)
                    {
                        ___specialcrafts[num14].SetActive(value: false);
                    }
                    ___craftList[___selected].SetItem(___craftList[___selected].GetItemType(), ___craftList[___selected].isUpgrade, null);
                    if (num8 >= 0)
                    {
                        ___craftedFlash.transform.position = ___bagItems[num8].transform.position;
                        ___craftedFlash.sprite = ___bagItems[num8].GetSprite();
                        ___craftedFlash.color = new Color(0f, 1f, 1f, 1f);
                        ___craftedFlash.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    if (type != 0)
                    {
                        trav.Method("UpdateCraftList").GetValue();
                        for (int num15 = 0; num15 < ___craftList.Length; num15++)
                        {
                            if (___craftList[num15].GetItemType() == type)
                            {
                                ___selected = num15;
                                trav.Method("MoveCursor", new object[] { true });
                                trav.Method("UpdateSelectedText").GetValue();
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int num16 = 0; num16 < ___craftList.Length; num16++)
                        {
                            ___craftList[num16].UpdateCanCraft();
                        }
                        trav.Method("UpdateSelectedText").GetValue();

                        return false;
                    }
                }

            }
        }

        return true;
    }

    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "Update")]
    [HarmonyPostfix]
    static void fixOrbShootType(ref (Character.OrbType, OrbShootType[],bool) __state)
    {
        if (__state.Item3)
        {
            EventManager.Instance.mainCharacter.cphy_perfer.PrepareSwitchOrb(false, true, __state.Item1);
            EventManager.Instance.mainCharacter.cphy_perfer.orbShootType = __state.Item2;
            
        }
    }



    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "UpdateCraftList")]
    [HarmonyPrefix]
    static bool addsBell(ref GemaUIPauseMenu_CraftGrid __instance, ref GameObject[] ___specialcrafts, ref int ___CurrentMaxCraft, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___selected)
    {
        for (int i = 0; i < ___specialcrafts.Length; i++)
        {
            ___specialcrafts[i].SetActive(value: false);
        }
        byte b = 1;
        ___CurrentMaxCraft = 0;
        for (int j = 0; j < ___craftList.Length; j++)
        {
            ___craftList[j].SetVisible(isVisible: false);
        }
        bool customGame = SaveManager.Instance.GetCustomGame(CustomGame.Explorer);


        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Bell, false, (byte)0 }).GetValue();

        if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Bookmark, false, (byte)0 }).GetValue();

        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_CocoBall, false, (byte)0 }).GetValue();

        if (SaveManager.Instance.GetChapter() >= 1 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Puff, false, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Lollipop, false, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_EnergyDrink, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 3 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_MintIceCream, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 4 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Donut, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 5 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VoodooPie, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 6 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_RumiCake, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 7 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_MilkTea, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 1 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_Mysterious, false, (byte)0 }).GetValue();
        }
        if ((SaveManager.Instance.GetChapter() >= 3 && SaveManager.Instance.GetItem(ItemList.Type.ITEM_PKBADGE) > 0) || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_BSnack, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleA) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleAHoneycloud, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleB) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleBMeringue, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleC) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleCMorning, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleD) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleDJellydrop, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleE) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleElueberry, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaSmall) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VenaBombSmall, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBig) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VenaBombBig, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaD) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VenaBombDispel, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaHB) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VenaBombHealBlock, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBB) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_VenaBombBunBun, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_WonderNote) > 0)
        {
            if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) < 9)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleWonderTemp, false, (byte)0 }).GetValue();
            }
            else
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Useable_WaffleWonderFull, false, (byte)0 }).GetValue();
            }
        }
        bool customGame2 = SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam);
        if (SaveManager.Instance.GetOrb() >= 3 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_OrbTypeS2, false, (byte)b++ }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 4 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_OrbBoostU, false, (byte)b++ }).GetValue();
        }
        if ((SaveManager.Instance.GetOrbTypeObtained() >= 2 || SaveManager.Instance.GetChapter() >= 6 || customGame2 || customGame) && !SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_OrbAmulet, true, (byte)b++ }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 1)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Function_MaterialExchangeA, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.Function_MaterialExchangeB, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 || SaveManager.Instance.GetMiniFlag(Mini.UnlockExplorerUpgrade) > 0 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_Explorer, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 4 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_KNIFE, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 3 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_ORB, true, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_RapidShots, true, (byte)0 }).GetValue();
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_AttackRange, true, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 2 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_EasyStyle, true, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_LINEBOMB, true, (byte)0 }).GetValue();
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_AREABOMB, true, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 2 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_SPEEDUP, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 5 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_AirDash, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 7 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_WALLJUMP, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetEventFlag(Mode.END_CHARON) > 0 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_JETPACK, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 5 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_BoostSystem, true, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_OrbAmulet, true, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 2 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_BOMBFUEL, true, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 7 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_BombLengthExtend, true, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_MASK, true, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 6 || customGame2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_TempRing, true, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_DodgeShot, true, (byte)0 }).GetValue();
        }
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_Rotater, true, (byte)0 }).GetValue();
        Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.ITEM_GoldenGlove, true, (byte)0 }).GetValue();
        if (SaveManager.Instance.GetChapter() >= 1 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableALastHitEnhance, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_RailPass) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableALongRangeSnipe, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableAHitIncrease, false, (byte)0 }).GetValue();
            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbTypeS2) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableBNormalShotDebuff, false, (byte)0 }).GetValue();
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableBReturnStyle, false, (byte)0 }).GetValue();
            }
            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbTypeS3) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableCBigExplode, false, (byte)0 }).GetValue();
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SableCSaver, false, (byte)0 }).GetValue();
            }
        }
        if (SaveManager.Instance.GetChapter() >= 1 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaALongStun, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_AirshipPass) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaASlowShot, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaAShortRangeBurst, false, (byte)0 }).GetValue();
            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbTypeC2) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaTypeBAmount, false, (byte)0 }).GetValue();
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CELIATYPEBANGLE, false, (byte)0 }).GetValue();
            }
            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbTypeC3) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaCShotIncrease, false, (byte)0 }).GetValue();
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CeliaCShotAllDirection, false, (byte)0 }).GetValue();
            }
        }
        if (SaveManager.Instance.GetChapter() >= 2 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_ComboStyleDamageUp, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_FrameCancel, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_NormalShotReducerB, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_StyleComboWeakGroundUpA, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 3 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_NormalShotWhenChargedShot, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_120CHARGE, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_AllMeleeSpeedUp, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_PowerDrop, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_DominantEffectDownA, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetChapter() >= 4 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_QuickDropExtendA, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_Cannon, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_NormalSupportShot, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_MAXHPBIGUP, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_BoostFullOffense, false, (byte)0 }).GetValue();
            if (SaveManager.Instance.GetDifficultyName() > Difficulty.D2)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_AutoCombo, false, (byte)0 }).GetValue();
            }
        }
        if (SaveManager.Instance.GetChapter() >= 5 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_MAXHP2MP, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_CraftBadgeCost, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_StyleComboBackImageS, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_AntiAirPlat, false, (byte)0 }).GetValue();
            if (SaveManager.Instance.GetUnlockedLogic(PlayerLogicState.TEVI_STRONG_GROUND_FRONT) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_StyleComboHeavyGroundFrontA, false, (byte)0 }).GetValue();
            }
        }
        if (SaveManager.Instance.GetChapter() >= 6 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_AutoBombChain, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_BounceTeviStrongAirUp, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_SuperFrameCancel, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_ArmorCritCrit, false, (byte)0 }).GetValue();
            if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbAmulet) > 0)
            {
                Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_Amulet10, false, (byte)0 }).GetValue();
            }
        }
        if (SaveManager.Instance.GetChapter() >= 7 || customGame)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_MAXMPCost, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_100COMBO, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_DominantEffectDownB, false, (byte)0 }).GetValue();
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_RangeBreak, false, (byte)0 }).GetValue();
        }
        if (SaveManager.Instance.GetItem(ItemList.Type.BADGE_AmuletQuicken) > 0)
        {
            Traverse.Create(__instance).Method("AddItem", new object[] { ItemList.Type.BADGE_AutoPilot, false, (byte)0 }).GetValue();
        }
        if (___selected >= ___CurrentMaxCraft)
        {
            ___selected = ___CurrentMaxCraft - 1;
        }
        if (___selected < 0)
        {
            ___selected = 0;
        }
        return false;
    }

}

class ShopPatch
{


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
                data = Randomizer.getRandomizedItem(item, (byte)(___ShopID + 30));
            else
                data = Randomizer.getRandomizedItem(item, 1);


            Randomizer.Upgradable upItem;
            bool flag = Enum.TryParse<Randomizer.Upgradable>(data.getItemTyp().ToString(), out upItem);

            if (item == ItemList.Type.STACKABLE_BAG)
            {
                MainVar.instance.BagID = (byte)(___ShopID + 1);
            }
            if (data.getItemTyp().ToString().Contains("STACKABLE") && SaveManager.Instance.GetStackableItem(data.getItemTyp(), data.getSlotId()))
            {
                return false;
            }
            else if (flag && SaveManager.Instance.GetStackableItem((ItemList.Type)upItem, data.getSlotId()))
            {
                return false;
            }
            else if
            (SaveManager.Instance.GetItem((ItemList.Type)data.itemID) > 0 && !flag) //implement a way to get only upgradeable items
            {
                return false;
            }


            switch (___typeN)
            {
                case Character.Type.Ian:
                    ___itemslots[___CurrentMaxItem].SetItem(item, num, false);
                    ___CurrentMaxItem++;
                    break;
                case Character.Type.CC:

                    ___itemslots[___CurrentMaxItem].SetItem(item, num, false);
                    ___CurrentMaxItem++;
                    break;
                default:
                    return true;

            }
            return false;
        }


        return true;
    }



    [HarmonyPatch(typeof(HUDShopMenu), "Update")]
    [HarmonyPrefix]
    static bool ChangeBuySystem(ref HUDShopMenu __instance, ref GemaShopItemSlot[] ___itemslots, ref int ___Selected, ref Character.Type ___typeN, ref SpriteRenderer ___buyo, ref bool ___bought, ref byte ___ShopID)
    {
        Traverse trav = Traverse.Create(__instance);
        if (InputButtonManager.Instance.GetButtonDown(13) && __instance.ShopType == 0)
        {
            ItemData data;
            if (___itemslots[___Selected].CanPurchase())
            {
                int price = ___itemslots[___Selected].GetPrice();
                bool flag2 = true;
                if (___itemslots[___Selected].GetItem().ToString().Contains("Useable") && SaveManager.Instance.isBagFull())
                {
                    trav.Method("PlayShopVoice", new object[] { ___typeN, ShopVoiceType.NoSpace }).GetValue();
                    string line = "SHOP." + ___typeN.ToString() + "_NoSpace";
                    trav.Method("StartNewLine", new object[] { line, true }).GetValue();

                    flag2 = false;
                }
                if (SaveManager.Instance.GetResource(ItemList.Resource.COIN) >= price && flag2)
                {
                    SaveManager.Instance.SubResource(ItemList.Resource.COIN, price);
                    if (___typeN == Character.Type.Ian)
                    {
                        SaveManager.Instance.savedata.coinUsedIan += price;
                    }
                    else if (___typeN == Character.Type.CC)
                    {
                        SaveManager.Instance.savedata.coinUsedCC += price;
                    }
                    if (___itemslots[___Selected].GetItem().ToString().Contains("Useable"))
                    {
                        data = Randomizer.getRandomizedItem(___itemslots[___Selected].GetItem(), 1);
                        SaveManager.Instance.AddItemToBag(___itemslots[___Selected].GetItem());
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_WaffleAHoneycloud)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleA, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_WaffleBMeringue)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleB, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_WaffleCMorning)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleC, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_WaffleDJellydrop)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleD, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_WaffleElueberry)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedWaffleE, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_VenaBombSmall)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaSmall, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_VenaBombBig)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaBig, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_VenaBombBunBun)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaBB, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_VenaBombHealBlock)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaHB, 1);
                        }
                        if (___itemslots[___Selected].GetItem() == ItemList.Type.Useable_VenaBombDispel)
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.UnlockedVenaD, 1);
                        }
                    }


                    else if (___itemslots[___Selected].GetItem().ToString().Contains("STACKABLE"))
                    {
                        data = Randomizer.getRandomizedItem(___itemslots[___Selected].GetItem(), (byte)(30 + ___ShopID));

                    }
                    else
                    {
                        data = Randomizer.getRandomizedItem(___itemslots[___Selected].GetItem(), 1);
                    }


                    if (data.getItemTyp().ToString().Contains("STACKABLE"))
                    {
                        SaveManager.Instance.SetStackableItem(data.getItemTyp(), data.getSlotId(), value: true);
                        if (data.getItemTyp() == ItemList.Type.STACKABLE_BAG)
                        {
                            SettingManager.Instance.SetAchievement(Achievements.ACHI_SHOP_BUYBAG);
                        }
                    }
                    else
                    {
                        SaveManager.Instance.SetItem(data.getItemTyp(), data.getSlotId());
                    }


                    if (___itemslots[___Selected].GetItem().ToString().Contains("BADGE_"))
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.BadgeBought, (byte)(SaveManager.Instance.GetMiniFlag(Mini.BadgeBought) + 1));
                    }
                    ___bought = true;
                    CameraScript.Instance.PlaySound(AllSound.SEList.Purchase);
                    ___buyo.transform.position = ___itemslots[___Selected].transform.position + new Vector3(122.5f, 0f, 0f);
                    ___buyo.transform.position = ___itemslots[___Selected].transform.position + new Vector3(122.5f, 0f, 0f);
                    ___buyo.transform.localScale = new Vector3(1f, 1f, 1f);
                    ___buyo.color = Color.white;
                    ___buyo.color = Color.white;
                    ___itemslots[___Selected].Purchased();
                    trav.Method("PlayShopVoice", new object[] { ___typeN, ShopVoiceType.Purchased }).GetValue();
                    string line2 = "SHOP." + ___typeN.ToString() + "_Purchased";
                    trav.Method("StartNewLine", new object[] { line2, true }).GetValue();
                }
            }
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(GemaShopItemSlot), "SetItem")]
    [HarmonyPrefix]
    static bool ChangeShopItemVisual(ref GemaShopItemSlot __instance, ref ItemList.Type t, ref int _price, ref SpriteRenderer ___itemicon, ref ItemList.Type ___itype)
    {
        if (HUDShopMenu.Instance.ShopType == 2 || HUDShopMenu.Instance.ShopType == 1)
        {
            return true;

        }
        Traverse trav = Traverse.Create(__instance);
        __instance.gameObject.SetActive(value: true);
        byte shopID = Traverse.Create(HUDShopMenu.Instance).Field("ShopID").GetValue<byte>();
        if (Traverse.Create(HUDShopMenu.Instance).Field("typeN").GetValue<Character.Type>() == Character.Type.CC)
        {
            ___itype = Randomizer.getRandomizedItem(t, (byte)(shopID + 30)).getItemTyp();
        }
        else
        {
            ___itype = Randomizer.getRandomizedItem(t, 1).getItemTyp();
        }

        trav.Field("price").SetValue(_price);

        ___itemicon.sprite = CommonResource.Instance.GetItem((int)___itype);
        ___itemicon.color = Color.white;
        SpriteRenderer bgicon = trav.Field("bgicon").GetValue<SpriteRenderer>();
        bgicon.color = Color.white;
        trav.Field("coinicon").GetValue<SpriteRenderer>().enabled = true;
        TextMeshPro[] texts = trav.Field("texts").GetValue<TextMeshPro[]>();


        ___itemicon.enabled = true;
        texts[0].text = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString(___itype), contains: false);
        texts[0].rectTransform.anchoredPosition = new Vector2(158.3f, 9.9f);
        texts[0].color = new Color(1f, 1f, 1f, 1f);
        texts[1].enabled = true;
        texts[2].enabled = true;
        bgicon.enabled = false;
        if (___itype >= ItemList.Type.BADGE_START && ___itype <= ItemList.Type.BADGE_MAX)
        {
            texts[1].text = Localize.GetLocalizeTextWithKeyword("ITEMTYPE.Badge", contains: false);
            texts[1].color = new Color32(byte.MaxValue, 186, 95, byte.MaxValue);
            bgicon.enabled = true;
        }
        else
        {
            texts[1].color = new Color32(152, 222, byte.MaxValue, byte.MaxValue);
            texts[1].text = Localize.GetLocalizeTextWithKeyword("ITEMTYPE.Item", contains: false);
        }
        texts[2].text = _price.ToString();
        ___itype = t;
        return false;
    }


    [HarmonyPatch(typeof(HUDShopMenu), "UpdateShopItemDetail")]
    [HarmonyPostfix]
    static void ItemShopDescriptionFix(ref HUDShopMenu __instance, ref TextMeshPro ___item_desc, ref byte ___ShopID, ref GemaShopItemSlot[] ___itemslots, ref int ___Selected)
    {

        if (__instance.ShopType == 0)
        {
            ItemList.Type item = ___itemslots[___Selected].GetItem();
            ItemData data;
            if (item.ToString().Contains("STACKABLE"))
            {
                data = Randomizer.getRandomizedItem(item, (byte)(___ShopID + 30));
                ___item_desc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data.getItemTyp());
            }
            else
            {
                data = Randomizer.getRandomizedItem(item, 1);
                ___item_desc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data.getItemTyp());
            }
            if (___item_desc.text.Contains("[c2]"))
            {
                ___item_desc.text = Localize.FilterLevelDescFromItem(data.getItemTyp(), ___item_desc.text);
            }
            if (data.getItemTyp().ToString().Contains("Useable_"))
            {
                ___item_desc.text += Localize.GetLocalizeTextWithKeyword("ITEMDESC.WAFFLEBUY", contains: false);
            }
            if (data.getItemTyp() >= ItemList.Type.BADGE_START && data.getItemTyp() <= ItemList.Type.BADGE_MAX)
            {
                TextMeshPro textMeshPro = ___item_desc;
                textMeshPro.text = textMeshPro.text + "<br><br>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.EQUIPBADGETIPS", contains: false);
            }
            ___item_desc.text = InputButtonManager.Instance.AddButtonsToPromote(___item_desc.text);
        }
    }

}