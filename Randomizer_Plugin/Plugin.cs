using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;
using TMPro;


using UnityEngine.UI;
using UnityEngine;
using Bullet;
using QFSW.QC;
using TeviRandomizer;
using Map;
using System.Linq;

using Newtonsoft.Json;
using Unity.Curl;
using Character;







public class ItemData
{
    public int itemID;
    public int slotID;
    public ItemData(int _itemID, int _slotID)
    {
        itemID = _itemID;
        slotID = _slotID;
    }

    public override bool Equals(object obj)
    {
        if (obj is ItemData other)
            return itemID == other.itemID && slotID == other.slotID;
        else 
            return false;
    }
    public override int GetHashCode()
    {
        return itemID ^ slotID;

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

public enum CustomFlags : short
{
    OrbStart = 0,
    CebleStart = 1,
}


[BepInPlugin("tevi.plugins.randomizer", "Randomizer", "0.9.9.1")]
[BepInProcess("TEVI.exe")]
public class RandomizerPlugin : BaseUnityPlugin
{

    static public Dictionary<ItemData, ItemData> __itemData = new Dictionary<ItemData, ItemData>();

    static public int  customDiff = -1; //fake diff

    static public bool[] customFlags = new bool[Enum.GetNames(typeof(CustomFlags)).Length];

    static public string pluginPath = BepInEx.Paths.PluginPath + "/tevi_randomizer/";

    public enum EventID
    {
        IllusionPalace = 9999
    }

    static public Randomizer randomizer;

    private void Awake()
    {
        Localize.GetLocalizeTextWithKeyword("", false);
        System.IO.Directory.CreateDirectory(pluginPath+"Data");
        randomizer = Randomizer.Instance;

        var instance = new Harmony("Randomizer");
        instance.PatchAll(typeof(RandomizerPlugin));
        instance.PatchAll(typeof(CraftingPatch));
        instance.PatchAll(typeof(ShopPatch));
        instance.PatchAll(typeof(EventPatch));
        instance.PatchAll(typeof(ItemObtainPatch));
        instance.PatchAll(typeof(UI));
        instance.PatchAll(typeof(SaveGamePatch));
        instance.PatchAll(typeof(ScalePatch));
        instance.PatchAll(typeof(OrbPatch));
        instance.PatchAll(typeof(RabiSmashPatch));
        instance.PatchAll(typeof(BonusFeaturePatch));


        // test Localizazion




        //instance.PatchAll(typeof(BonusFeaturePatch));
        Logger.LogInfo($"Plugin Randomizer is loaded!");

    }


    static void addLang()
    {
        var t = new Traverse(typeof(Localize));

        {
            Localize.SystemText newText = new Localize.SystemText();
            newText.keyword = "ITEMNAME.I19";
            newText.tchinese = "Celia";
            newText.japanese = "Celia";
            newText.english = "Celia";
            newText.spanish = "Celia";
            newText.russian = "Celia";
            newText.ukrainian = "Celia";
            newText.schinese = "Celia";
            newText.korean = "Celia";
            t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
            t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
        }
        {
            Localize.SystemText newText = new Localize.SystemText();
            newText.keyword = "ITEMDESC.I19";
            newText.tchinese = "Celia is now available";
            newText.japanese = "Celia is now available";
            newText.english = "Celia is now available";
            newText.schinese = "Celia is now available";
            newText.russian = "Celia is now available";
            newText.ukrainian = "Celia is now available";
            newText.korean = "Celia is now available";
            newText.spanish = "Celia is now available";
            t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
            t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
        }

        {
            Localize.SystemText newText = new Localize.SystemText();
            newText.keyword = "ITEMNAME.I20";
            newText.tchinese = "Sable";
            newText.japanese = "Sable";
            newText.english = "Sable";
            newText.schinese = "Sable";
            newText.spanish = "Sable";
            newText.korean = "Sable";
            newText.russian = "Sable";
            newText.ukrainian = "Sable";
            t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
            t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
        }
        {
            Localize.SystemText newText = new Localize.SystemText();
            newText.keyword = "ITEMDESC.I20";
            newText.tchinese = "Sable is now available";
            newText.japanese = "Sable is now available";
            newText.english = "Sable is now available";
            newText.schinese = "Sable is now available";
            newText.korean = "Sable is now available";
            newText.spanish = "Sable is now available";
            newText.russian = "Sable is now available";
            newText.ukrainian = "Sable is now available";
            t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
            t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
        }

        }


    [HarmonyPatch(typeof(Localize), "GetLocalizeTextWithKeyword")]
    [HarmonyPrefix]
    static bool addLan(ref List<Localize.SystemText> ___jsonlistSysTxt,ref Dictionary<string, Localize.SystemText> ___jsonlistSysTxtDictionary)
    {
        if (GemaLocalizeManager.Instance == null)
        {
            return true;
        }
        if (___jsonlistSysTxt == null)
        {
            ___jsonlistSysTxt = (List<Localize.SystemText>)JsonConvert.DeserializeObject(GemaLocalizeManager.Instance.SystemText_TextAsset.text, typeof(List<Localize.SystemText>));
            ___jsonlistSysTxtDictionary = new Dictionary<string, Localize.SystemText>();
            foreach (Localize.SystemText item in ___jsonlistSysTxt)
            {
                if (!___jsonlistSysTxtDictionary.ContainsKey(item.keyword))
                {
                    ___jsonlistSysTxtDictionary.Add(item.keyword, item);
                }
            }


            addLang();
        }
        return true;
    }


    [Command("reloadRandomizer", Platform.AllPlatforms, MonoTargetType.Single)]
    private void reloadItems()
    {
        try
        {
            string path = $"{BepInEx.Paths.PluginPath}/tevi_randomizer/data/file.dat";
            string json = File.ReadAllText(path);
            string[] blocks = json.Split(';');
            __itemData.Clear();
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
        }
    }

    static public void createSeed(System.Random seed)
    {
        randomizer.createSeed(seed);

    }


    static public ItemData getRandomizedItem(ItemList.Type itemid, byte slotid)
    {
        ItemData data;
        try
        {

            data = __itemData[new ItemData((int)itemid, (int)slotid)];
        }
        catch
        {
            //Debug.LogWarning($"Could not find {itemid.ToString()} {slotid}");
            data = new ItemData((int)itemid, (int)slotid);
        }
        return data;
    }

    static public ItemData getRandomizedItem(int itemid, int slotid)
    {
        ItemData data;
        try
        {

            data = __itemData[new ItemData(itemid, slotid)];
            
        }
        catch
        {
            Debug.LogWarning($"Could not find {((ItemList.Type)itemid).ToString()} {slotid}");
            data = new ItemData(itemid, slotid);
        }
        return data;
    }

    static public Dictionary<ItemData,ItemData> saveRando()
    {
        return __itemData;
    }
    static public void loadRando(Dictionary<ItemData, ItemData> data)
    {
        __itemData = data;
    }
    static public void deloadRando()
    {
        __itemData.Clear();
    }
    static public bool checkItemGot(ItemList.Type item,byte slot)
    {
        Upgradable itemref;
        if (!item.ToString().Contains("STACKABLE"))
        {

            if (Enum.TryParse(item.ToString(), out itemref))
            {
                return SaveManager.Instance.GetStackableItem((ItemList.Type)itemref, slot);
            }
            else
            {
                return SaveManager.Instance.GetItem(item) > 0;
            }
        }
        else
        {
            return SaveManager.Instance.GetStackableItem(item, slot);
        }

    }    
    static public bool checkRandomizedItemGot(ItemList.Type item,byte slot)
    {

        ItemData t = getRandomizedItem(item,slot);
        item = t.getItemTyp();
        slot = t.getSlotId();
        Upgradable itemref;
        if (!item.ToString().Contains("STACKABLE"))
        {

            if (Enum.TryParse(item.ToString(), out itemref))
            {
                    return SaveManager.Instance.GetStackableItem((ItemList.Type)itemref, slot);
            }
            else
            {
                return SaveManager.Instance.GetItem(item) > 0;
            }
        }
        else
        {
            return SaveManager.Instance.GetStackableItem(item,slot);
        }
    }

    static public Sprite getSprite(int itemID,bool custom = false)
    {
        return CommonResource.Instance.GetItem(itemID);
    }

    [HarmonyPatch(typeof(WorldManager), "FindNearestItem_Room")]
    [HarmonyPostfix]
    static void findNearestRandomizedItem(ref ItemTile tile,ref ItemList.Type nearestType)
    {

        nearestType =getRandomizedItem(tile.itemid, tile.GetSlotID()).getItemTyp();
    }

    // change how the item Bell Works
    [HarmonyPatch(typeof(CharacterBase),"UseItem")]
    [HarmonyPrefix]
    static bool WarpBell(ref ItemList.Type item,ref bool playvoice,ref ObjectPhy ___phy_perfer, ref playerController ___playerc_perfer, ref CharacterBase __instance)
    {
        if(item == ItemList.Type.Useable_Bell)
        {
            if (!EventManager.Instance.isBossMode() && EventManager.Instance.getMode() == Mode.OFF && EventManager.Instance.getSubMode() == Mode.OFF)
            {
                if (GemaMissionMode.Instance.isInMission() || WorldManager.Instance.Area == 30 || (EventManager.Instance.GetCurrentEventBattle() != Mode.Chap7StartRibauldChase && EventManager.Instance.GetCurrentEventBattle() != 0) || EventManager.Instance.isBossMode())
                {
                    __instance.PlaySound(AllSound.SEList.MENUFAIL);
                    __instance.ChangeLogicStatus(Character.PlayerLogicState.NORMAL);
                    EventManager.Instance.EFF_CreateEmotion(__instance, null, __instance.t.position, EffectSprite.EMOTION_QUESTION);
                    return false;
                }


                int num5 = (int)___phy_perfer.GetCounter(4);

                //EventManager.Instance.StartWarp(1, 1, 1, 1);
                GemaUIPauseMenu.Instance.OpenPauseMenu(true);
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
                __instance.ChangeLogicStatus(Character.PlayerLogicState.NORMAL);
            }
            else
            {
                __instance.PlaySound(AllSound.SEList.MENUFAIL);
                __instance.ChangeLogicStatus(Character.PlayerLogicState.NORMAL);
                EventManager.Instance.EFF_CreateEmotion(__instance, null, __instance.t.position, EffectSprite.EMOTION_QUESTION);
            }
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(SettingManager),nameof(SettingManager.SetAchievement))]
    [HarmonyPrefix]
    static bool removeAchievements()
    {
        return false;
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
                else if (SaveManager.Instance.GetItem(data.getItemTyp())>0)
                {
                    Debug.Log("[ItemTile] Item " + data.getItemTyp().ToString() + " visible in camera. Removed from map because player already obtained it. GotItem = " + SaveManager.Instance.GetItem(data.getItemTyp()).ToString());
                    __instance.DisableMe();
                    return false;
                }
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


    public static void addOrbStatus(int amount = 0)
    {
        if(SaveManager.Instance.GetOrb() < 3)
            SaveManager.Instance.SetOrb((byte)(SaveManager.Instance.GetOrb() + amount));
        if (SaveManager.Instance.GetOrb() >= 3)
        {
            if(SaveManager.Instance.GetItem(ItemList.Type.ITEM_BoostSystem) > 0) {
                SaveManager.Instance.SetOrb(4);
            }
            SaveManager.Instance.FirstTimeEnableOrbColors();
        }
    }

    [HarmonyPatch(typeof(CommonResource),"GetItem")]
    [HarmonyPrefix]
    static bool addCustomIcons()
    {
        return true;  //copy and insert custom item icon 
    }

    //Craftig Orb Fix
    //No set SlotId, maybe reserver slots for Potions?
    [HarmonyPatch(typeof(SaveManager), "GetOrbTypeObtained")]
    [HarmonyPostfix]
    static void orbTypeFix(ref int __result, ref SaveManager __instance)
    {
        __result = 0;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeC2, 1))
            __result++;
        else
            return;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeS2, 1))
            __result++;
        else
            return;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeC3, 1))
            __result++;
        else
            return;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeS3, 1))
            __result++;
        else
             return;
    }

    [HarmonyPatch(typeof(SaveManager), "GetOrbBoostObtained")]
    [HarmonyPostfix]
    static void OrbBoostCount(ref int __result, SaveManager __instance)
    {
        __result = 0;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbBoostD, 1))
        {
            __result++;
        }
        else return;
        if (checkRandomizedItemGot(ItemList.Type.ITEM_OrbBoostU, 1)){
            __result++;
        }
        else return;
    }

    //Orb!
    [HarmonyPatch(typeof(SaveManager), "FirstTimeEnableOrbColors")]
    [HarmonyPrefix]
    static bool disableOrbOverride(SaveManager __instance)
    {
        if (__instance.GetMiniFlag(Mini.OrbStatus) >= 3) return true;
        return false;
    }

}

class ItemObtainPatch()
{

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
    //craftingMenuRefresh
    [HarmonyPatch(typeof(HUDObtainedItem), "GiveItem")]
    [HarmonyPostfix]
    static void CraftingRefresh()
    {
        if (GemaUIPauseMenu_CraftGrid.Instance != null)
            Traverse.Create(GemaUIPauseMenu_CraftGrid.Instance).Method("UpdateCraftList").GetValue();
        else
        {
            Debug.LogWarning("This was triggerd to Early");
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
                    if(value == 1) { value = 4; }
                    if (value > 3)
                    {
                        value = (byte)(__instance.GetItem(item) + 1);
                        if (!__instance.GetStackableItem((ItemList.Type)itemRef, 0))
                        {
                            __instance.SetStackableItem((ItemList.Type)itemRef, 0, true);
                        }
                        if(item == ItemList.Type.ITEM_BoostSystem)
                        {
                            if(SaveManager.Instance.GetOrb() == 3) {
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
    [HarmonyPatch(typeof(SaveManager),"SetItem")]
    [HarmonyPostfix]
    static void dynamicOrbChange(ref ItemList.Type item)
    {
        if (item == ItemList.Type.I19 || item == ItemList.Type.I20) EventManager.Instance.ReloadOrbStatus();
    }


    //change Map Icon 
    [HarmonyPatch(typeof(WorldManager),"CollectMapItem")]
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
            else
            {
                Debug.LogWarning("[EventDetect] Invalid Item obtained!");
            }
            data2.DisableMe();
        

        return false;
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
            if (RandomizerPlugin.getRandomizedItem(4200, 0).slotID > 0)
            {
                SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 4);
                SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 5);
                SaveManager.Instance.SetItem(ItemList.Type.ITEM_Explorer, 6);
            }
            for (int i = RandomizerPlugin.getRandomizedItem(4006,0).slotID ;i > 0; i--)
            {
                SaveManager.Instance.SetStackableItem(ItemList.Type.STACKABLE_RATK, (byte)(64-i),true);
            }
            for (int i = RandomizerPlugin.getRandomizedItem(4005,0).slotID ;i > 0; i--)
            {
                SaveManager.Instance.SetStackableItem(ItemList.Type.STACKABLE_MATK, (byte)(64-i),true);
            }

            SaveManager.Instance.SetOrb((byte)0);
            //RandomizerPlugin.addOrbStatus(3);

            ItemData data = RandomizerPlugin.getRandomizedItem(ItemList.Type.ITEM_ORB, 1);
            if (data.getItemTyp().ToString().Contains("STACKABLE"))
                SaveManager.Instance.SetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            else
                SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);

            //adding 1 item to the pool for orbs
            data = RandomizerPlugin.getRandomizedItem(ItemList.Type.I20, 1);
            if (data.getItemTyp().ToString().Contains("STACKABLE"))
                SaveManager.Instance.SetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            else
                SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);

            data = RandomizerPlugin.getRandomizedItem(ItemList.Type.ITEM_KNIFE, 1);
            if(data.getItemTyp().ToString().Contains("STACKABLE"))
                SaveManager.Instance.SetStackableItem((ItemList.Type)data.itemID, (byte)data.slotID, true);
            else
                SaveManager.Instance.SetItem((ItemList.Type)data.itemID, (byte)data.slotID, true);

            if (RandomizerPlugin.customFlags[(int)CustomFlags.CebleStart])
            {
                SaveManager.Instance.SetItem(ItemList.Type.I19,1);
                SaveManager.Instance.SetItem(ItemList.Type.I20,1);
            }
            int tmp;
            if(RandomizerPlugin.customDiff >= 0)
            {
                tmp = RandomizerPlugin.customDiff;
                RandomizerPlugin.customDiff = SaveManager.Instance.GetDifficulty();
                SaveManager.Instance.SetDifficulty(tmp);
            }

            // Make a Path to Morose
            SaveManager.Instance.AddBreakTile(1, 302, 189);
            SaveManager.Instance.AddBreakTile(1, 303, 189);
            SaveManager.Instance.AddBreakTile(1, 304, 189);
            ShopPatch.alreadyClaimed();

            em.SetStage(30);
            
        }
    }
    
    [HarmonyPatch(typeof(AfterMemineChallenge), "EVENT")]
    [HarmonyPrefix]
    static void MemineAllChallangesChecl(ref CharacterBase ___m)
    {
        EventManager em = EventManager.Instance;
        switch (EventManager.Instance.EventStage) {
            case 30:
                em.NextStage();
                break;
            case 40:
        {
             MusicManager.Instance.PlayRoomMusic();
            GemaMissionMode.Instance.MissionCleared();
            em.StopEvent();
            ___m.DoNotDelete = false;
            ___m.ID = 32;

            int num = 0;
            for (int i = 0; i <= 5; i++)
            {
                if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_SHARD, (byte)i))
                {
                    num++;
                }
                    }
            if (num >= 3 && SaveManager.Instance.GetMiniFlag(Mini.UnlockExplorerUpgrade) <= 0)
            {
                SaveManager.Instance.SetMiniFlag(Mini.UnlockExplorerUpgrade, 1);
                HUDPopupMessage.Instance.StartCraftAddedMessage();
            }

            if (num == 6)
            {
               Debug.Log(EventManager.Instance.TryStartEvent(Mode.AllMemineWon, force: true));
            }
                    break;
        } 

    }
    }

    [HarmonyPatch(typeof(EventManager), "MovePlayerToWarpDevice3")]
    [HarmonyPrefix]
    static bool noWarp()
    {
        return false;
    }

    [HarmonyPatch(typeof(Chap1FreeRoamVena7x7), "REQUIREMENT")]
    [HarmonyPrefix]
    static bool Vena7x7Fix(ref bool __result)
    {
        ItemData data = RandomizerPlugin.getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);

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
                        ItemData data = RandomizerPlugin.getRandomizedItem(ItemList.Type.STACKABLE_COG, 23);
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

    [HarmonyPatch(typeof(EventManager),"CheckAfterMapChange")]
    [HarmonyPrefix]
    static void dontTakeHands(ref (byte,byte) __state)
    {
        __state = (SaveManager.Instance.GetItem(ItemList.Type.QUEST_GHandL), SaveManager.Instance.GetItem(ItemList.Type.QUEST_GHandR));
    }
    [HarmonyPatch(typeof(EventManager),"CheckAfterMapChange")]
    [HarmonyPostfix]
    static void returnHands(ref (byte,byte) __state)
    {
        if(__state.Item1 > 0)
        SaveManager.Instance.SetItem(ItemList.Type.QUEST_GHandL, __state.Item1);
        if(__state.Item2 > 0)
        SaveManager.Instance.SetItem(ItemList.Type.QUEST_GHandR, __state.Item2);
    }

    [HarmonyPatch(typeof(enemyController), "handexhange_delaystart2")]
    [HarmonyPrefix]
    static bool noHandExchange()
    {
        SettingManager.Instance.SetAchievement(Achievements.ACHI_ITEM_GOLDENHANDS);
        return false;
    }

    //End Requierment
    static bool EventReq(int customEventId)
    {
        bool flag = false;


        switch (RandomizerPlugin.getRandomizedItem(customEventId, 1).slotID)
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
        if (EventReq((int)RandomizerPlugin.EventID.IllusionPalace)) 
        {
            __result = true;
        }
        return false;
    }
}

class RabiSmashPatch
{
    [HarmonyPatch(typeof(Chap4RabiSmashFullCleared), "EVENT")]
    [HarmonyPrefix]
    static void reduceMaxPointReq()
    {
        if (EventManager.Instance.EventStage == 10)
        {
            EventManager.Instance.SetStage(11);
        }
        if(EventManager.Instance.EventStage == 11) { 
            EventManager.Instance.StopEvent();
            if (!RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.QUEST_RabiPillow, 1) && SaveManager.Instance.savedata.minigame_highest_score >= 100)
            {
                EventManager.Instance.TryStartEvent(Mode.Chap4RabiSmashPillowObtain, true);
            }
            else
            {
                SaveManager.Instance.AutoSave();
            }
        }
    }

    [HarmonyPatch(typeof(RabiRibiEasterEgg), "EVENT")]
    [HarmonyPrefix]
    static void newItemSpot()
    {
        EventManager em = EventManager.Instance;
        if (em.EventStage == 170)
        {
            em.StartChat(11, "chapterall_rabieasteregg");
            em.SetStage(171);
        }
        else if (em.EventStage == 171)
        {
            if (em.EventTime > 0.145f && em.EventTime < 100f)
            {
                HUDObtainedItem.Instance.GiveItem(ItemList.Type.I19, 1);
                em.EventTime = 100f;
            }
            if (em.EventTime >= 100.5f && !HUDObtainedItem.Instance.isDisplaying())
            {
                em.SetStage(180);
            }
        }
    }
}



class OrbPatch
{
    [HarmonyPatch(typeof(HUDPosition), "Update")]
    [HarmonyPrefix]
    static bool gibHUD(ref UIType.HUD ___Type, ref RectTransform ___rt,ref float ___startX,ref float ___startY,ref float ___targetX)
    {
        if (!___rt) return false;

        if(___Type == UIType.HUD.MAIN)
        {
            UnityEngine.Vector3 vector = ___rt.anchoredPosition;
            float num = ___startX;
            float b = ___startY;

            if (Utility.isHideHUD())
            {
                num = ___targetX;
            }
            vector.x = Mathf.Lerp(vector.x, num, Utility.GetSmooth2(15f));
            vector.y = Mathf.Lerp(vector.y, b, Utility.GetSmooth2(15f));
            ___rt.anchoredPosition = vector;

            return false;
        }
        return true;
    }
    [HarmonyPatch(typeof(EventManager),"ReloadOrbStatus")]
    [HarmonyPrefix]
    static bool orbRealoadStuff(ref CharacterBase ___mainCharacter, ref EventManager __instance)
    {
        (___mainCharacter.phy_perfer as CharacterPhy).canShootFlag = false;
        (___mainCharacter.phy_perfer as CharacterPhy).canChargeFlag = false;
        (___mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag = false;
        (___mainCharacter.phy_perfer as CharacterPhy).canBlackFlag = false;
        WorldManager.Instance.CanShootWall = false;
        if (SaveManager.Instance.GetOrb() <= 0)
        {
            __instance.ShowAllOrbs(t: false);
        }
        else
        {
            (___mainCharacter.phy_perfer as CharacterPhy).canShootFlag = true;
            __instance.ShowAllOrbs(t: true);
            if (SaveManager.Instance.GetOrb() >= 2)
            {
                WorldManager.Instance.CanShootWall = true;

                (___mainCharacter.phy_perfer as CharacterPhy).canChargeFlag = true;
                ReloadBar.Instance.EnableMe();
                if (SaveManager.Instance.GetOrb() >= 3)
                {
                (___mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag = SaveManager.Instance.GetItem(ItemList.Type.I19) > 0;
                (___mainCharacter.phy_perfer as CharacterPhy).canBlackFlag = SaveManager.Instance.GetItem(ItemList.Type.I20) > 0;
                }
                __instance.HidePoweredOrbs(t: false);
            }
            else
            {
                __instance.HidePoweredOrbs(t: true);
            }
        }
        GemaSpineHUDTopLeft.Instance.UpdateSkin();
        if (Time.timeSinceLevelLoad > 2.5f)
        {
            Debug.Log("[EventManager] Orb Status = " + SaveManager.Instance.GetOrb() + " | CanShoot = " + (___mainCharacter.phy_perfer as CharacterPhy).canShootFlag + " | CanCharge = " + (___mainCharacter.phy_perfer as CharacterPhy).canChargeFlag + " | CanShootWall = " + WorldManager.Instance.CanShootWall + " | canWhite = " + (___mainCharacter.phy_perfer as CharacterPhy).canWhiteFlag + " | canBlack = " + (___mainCharacter.phy_perfer as CharacterPhy).canBlackFlag);
        }


        return false;
    }



    [HarmonyPatch(typeof(CharacterPhy), "PrepareSwitchOrb")]
    [HarmonyPrefix]
    static bool newPrepareSwitchOrb(ref Character.OrbType ot,ref bool forceType, ref CharacterBase ___cb_perfer, ref CharacterPhy __instance)
    {
        if (forceType)
        {
            return true;
        }
        else if (__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0)
        {
            return false;
        }
        else if (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0)
        {
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(CharacterPhy),"UseBoost")]
    [HarmonyPostfix]
    static void switchOrbBeforeSummon(ref CharacterPhy __instance, ref bool __result)
    {
        if (__result)
        {
            if (SaveManager.Instance.GetItem(ItemList.Type.I19) < 1 ^ SaveManager.Instance.GetItem(ItemList.Type.I20) < 1)
            {
                __instance.PrepareSwitchOrb(false, true, (Character.OrbType)((int)__instance.orbUsing ^ 1));
                //__instance.orbUsing = (OrbType)((int)__instance.orbUsing ^ 1);
            }
        }
    }    
    [HarmonyPatch(typeof(CharacterPhy),"UseBoost")]
    [HarmonyPrefix]
    static bool test(ref CharacterPhy __instance, ref bool __result)
    {
        if (SaveManager.Instance.GetItem(ItemList.Type.I19) <1 &&SaveManager.Instance.GetItem(ItemList.Type.I20) <1)
        {
            __result = true; return false;
        }


        return true;
    }

    [HarmonyPatch(typeof(CharacterPhy),"SwitchTypeForced")]
    [HarmonyPrefix]
    static bool forcedTypeSwitch(ref CharacterPhy __instance,ref bool __result)                 // SwitchType MODE AABBCC Not Working
    {
        if((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
        {
            __instance.PrepareSwitchOrb();
            __result = false;
            return false;
        }
        return true;
    }    
    [HarmonyPatch(typeof(CharacterPhy),"SwitchType")]
    [HarmonyPrefix]
    static bool typeSwitch(ref CharacterPhy __instance,ref bool __result)
    {
        if((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
        {
            __result = false;
            return false;
        }
        return true;
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
        ItemData data = RandomizerPlugin.getRandomizedItem(___craftList[___selected].GetItemType(), 1);



        ItemList.Type itemType = ___craftList[___selected].GetItemType();
        if (itemType.ToString().Contains("ITEM"))
        {
            data = RandomizerPlugin.getRandomizedItem(itemType, (byte)(SaveManager.Instance.GetItem(itemType) + 1));
            if (___craftList[___selected].isUpgrade)
            {
                data = RandomizerPlugin.getRandomizedItem(itemType, (byte)(getItemUpgradeCount(itemType)+1));
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
                if (___craftList[___selected].isUpgrade)
                    ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(itemType);
                else
                    ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data.getItemTyp());

                if (___selectedDesc.text.Contains("[c2]"))
                {
                    if (___craftList[___selected].isUpgrade)
                    {
                        //___selectedDesc.text = Localize.FilterLevelDescFromItem2(data.getItemTyp(), ___selectedDesc.text);
                        ___selectedDesc.text = Localize.FilterLevelDescFromItem2(itemType, ___selectedDesc.text);
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
                if ((ItemList.Resource)((i + 1) % GemaItemManager.Instance.maxMaterial) == 0) mat = 0;
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
        ItemData data = RandomizerPlugin.getRandomizedItem(iType, 1);

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
            else if (data.getItemTyp().ToString().Contains("ITEM") || data.getItemTyp().ToString().Contains("QUEST"))
            {
                Upgradable upitem;
                if(Enum.TryParse(data.getItemTyp().ToString(),out upitem))
                {
                    if(SaveManager.Instance.GetStackableItem((ItemList.Type)upitem, data.getSlotId()))
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


        ItemData data = RandomizerPlugin.getRandomizedItem(itype, 1);
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

    static public int getItemUpgradeCount(ItemList.Type _item)
    {
        int num = 0;
        Upgradable item;
        if (Enum.TryParse(_item.ToString(), out item))
        {
            if (SaveManager.Instance.GetStackableItem((ItemList.Type)item, 0))
            {
                num++;
            }
            if (RandomizerPlugin.checkRandomizedItemGot(_item, 2))
            {
                num++;
            }
            if (RandomizerPlugin.checkRandomizedItemGot(_item, 3))
                {
                num++;
            }
        }
        return num;
    }

    [HarmonyPatch(typeof(GemaItemManager), "GetMat")]
    [HarmonyPrefix]
    static bool CustomMats(ref ItemList.Type iType,ref int count,ref int __result,ref GemaItemManager __instance)
    {
        int num = 0;
        switch (iType)
        {
            case ItemList.Type.Useable_WaffleWonderTemp:
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 0 && count == 4)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 1 && count == 3)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 2 && count == 7)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 3 && count == 9)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 4 && count == 8)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 5 && count == 12)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 6 && count == 11)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 7 && count == 10)
                {
                    num = 9;
                }
                if (SaveManager.Instance.GetMiniFlag(Mini.WaffleWonderCrafted) == 8 && count >= 3 && count <= 12 && count != 5 && count != 6)
                {
                    num = 7;
                }
                break;
            case ItemList.Type.Function_MaterialExchangeA:
                {
                    int num5 = 0;
                    int num6 = -1;
                    for (int j = 3; j < 5; j++)
                    {
                        if (SaveManager.Instance.GetResource((ItemList.Resource)j) >= 9 && SaveManager.Instance.GetResource((ItemList.Resource)j) > num5)
                        {
                            num5 = SaveManager.Instance.GetResource((ItemList.Resource)j);
                            num6 = j;
                        }
                    }
                    num = ((count == num6) ? 9 : 0);
                    break;
                }
            case ItemList.Type.Function_MaterialExchangeB:
                {
                    int num3 = 0;
                    int num4 = -1;
                    for (int i = 7; i < 13; i++)
                    {
                        if (SaveManager.Instance.GetResource((ItemList.Resource)i) >= 9 && SaveManager.Instance.GetResource((ItemList.Resource)i) > num3)
                        {
                            num3 = SaveManager.Instance.GetResource((ItemList.Resource)i);
                            num4 = i;
                        }
                    }
                    num = ((count == num4) ? 9 : 0);
                    break;
                }
            default:
                {
                    if (iType.ToString().Contains("_OrbBoost"))
                    {
                        if (count == 1)
                        {
                            num = 4;
                            if (SaveManager.Instance.GetOrbBoostObtained() > 0)
                            {
                                num += 4;
                            }
                        }
                        if (count % __instance.maxMaterial == 0)
                        {
                            num = 250;
                        }
                        break;
                    }
                    if (iType.ToString().Contains("_OrbType"))
                    {
                        if (count == 1)
                        {
                            num = 4;
                            if (SaveManager.Instance.GetOrbTypeObtained() > 0)
                            {
                                num++;
                            }
                            if (SaveManager.Instance.GetOrbTypeObtained() > 1)
                            {
                                num += 2;
                            }
                            if (SaveManager.Instance.GetOrbTypeObtained() > 2)
                            {
                                num++;
                            }
                        }
                        if (count % __instance.maxMaterial == 0)
                        {
                            num = 250;
                        }
                        break;
                    }
                    if (iType >= ItemList.Type.BADGE_START && iType <= ItemList.Type.BADGE_MAX)
                    {
                        if (count == 1)
                        {
                            __result= 0;
                            return false;
                        }
                    }
                    else if (iType.ToString().Contains("Useable_"))
                    {
                        if (count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                            return false;
                        }
                    }
                    else
                    {
                        if (iType == ItemList.Type.ITEM_SPEEDUP && getItemUpgradeCount(iType) > 0 && count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_AttackRange && getItemUpgradeCount(iType) > 0 && count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_RapidShots && getItemUpgradeCount(iType) > 0 && count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_GoldenGlove && getItemUpgradeCount(iType) > 0 && count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_OrbAmulet && getItemUpgradeCount(iType) <= 0 && count == 2)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_OrbAmulet && getItemUpgradeCount(iType) > 0 && count == 1)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_ORB && getItemUpgradeCount(iType) >= 2 && count == 10)
                        {
                            __result = 0;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_KNIFE && getItemUpgradeCount(iType) >= 2 && count == 10)
                        {
                            __result = 3;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_BoostSystem && getItemUpgradeCount(iType) >= 2 && count == 10)
                        {
                            __result = 4;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_OrbAmulet && getItemUpgradeCount(iType) >= 2 && count == 10)
                        {
                            __result = 5;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_BombLengthExtend && getItemUpgradeCount(iType) >= 2 && count == 7)
                        {
                            __result = 5;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_GoldenGlove && count == 2)
                        {
                            __result = 1;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_TempRing && count == 2)
                        {
                            __result = 1;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_AntiDecay && count == 2)
                        {
                            __result = 1;
                            return false;
                        }
                        if (iType == ItemList.Type.ITEM_MASK && count == 2)
                        {
                            __result = 1;
                            return false;
                        }
                    }
                    GemaItemManager.ItemData db = __instance.GetItemDB(iType);
                    num = __instance.GetItemDBMaterialData(db, count % __instance.maxMaterial);
                    if (iType.ToString().Contains("ITEM") && SaveManager.Instance.GetItem(iType) > 0)
                    {
                        if (count % __instance.maxMaterial == 0)
                        {
                            __result = 0;
                        }
                        if (iType == ItemList.Type.ITEM_Explorer)
                        {
                            switch (count)
                            {
                                case 2:
                                    __result = 1;
                                    return false;
                                case 1:
                                    if (SaveManager.Instance.GetCustomGame(CustomGame.Explorer))
                                    {
                                        if (getItemUpgradeCount(iType) == 1)
                                        {
                                            __result = 1;
                                            return false;
                                        }
                                        if (getItemUpgradeCount(iType) >= 2)
                                        {
                                            __result = 1;
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (getItemUpgradeCount(iType) == 1)
                                        {
                                            __result = 3;
                                            return false;
                                        }
                                        if (getItemUpgradeCount(iType) >= 2)
                                        {
                                            __result = 6;
                                            return false;
                                        }
                                    }
                                    break;
                            }
                        }
                        if (count == 2 && num > 10 && num < 100)
                        {
                            int num2 = num % 10;
                            num /= 10;
                            if (getItemUpgradeCount(iType) >= 2)
                            {
                                num += num2;
                            }
                        }
                    }
                    if (__instance.isBadge(iType) && count == 3 && num <= 0)
                    {
                        num = 3 + (int)((float)__instance.GetItemCost(iType) * 1.25f);
                    }
                    break;
                }
        }
        if (count % __instance.maxMaterial == 0)
        {
            num = ((!iType.ToString().Contains("Useable_") && !iType.ToString().Contains("Function_")) ? 250 : 0);
        }
        __result = num;
        return false;
    }

    [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "Update")]
    [HarmonyPrefix]
    static bool progressiveItemCrafting(ref GemaUIPauseMenu_CraftGrid __instance, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___selected, ref ItemList.Type ___currentItemType, ref GemaUIPauseMenu_CraftMaterialSlot[] ___materialownedList,
        ref byte[] ___currentMaterialNeeded, ref float ___isJustCraftedBadge, ref float ___errorflashing, ref float ___flashing, Image ___synthesisBox, Image ___synthesisBoxOutline, ref GameObject[] ___specialcrafts, ref Image ___craftedFlash,
        ref GemaUIPauseMenu_ItemGridSub[] ___bagItems,ref (Character.OrbType, Character.OrbShootType, Character.OrbShootType, bool) __state)
    {
        Traverse trav = Traverse.Create(__instance);
        __state.Item4 = false;
        if (InputButtonManager.Instance.GetButtonDown(13) && !HUDObtainedItem.Instance.isDisplaying())
        {
            __state.Item1 = EventManager.Instance.mainCharacter.cphy_perfer.orbUsing;
            __state.Item2 = EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[0];
            __state.Item3 = EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[1];
            __state.Item4 = true;
            if (___currentItemType.ToString().Contains("ITEM") && ___craftList[___selected].isUpgrade || ___currentItemType.ToString().Contains("BADGE") || ___currentItemType.ToString().Contains("_OrbBoost"))
            {

                int num5 = 1;
                ItemList.Resource resource = ItemList.Resource.COIN;


                if (getItemUpgradeCount(___currentItemType) >= 3) // helperfunction 
                {
                    num5 = -3;
                }
                ItemData rnd = RandomizerPlugin.getRandomizedItem(___currentItemType, 1);
                if (!Enum.IsDefined(typeof(Upgradable), ___currentItemType.ToString()) && RandomizerPlugin.checkItemGot(rnd.getItemTyp(),rnd.getSlotId()))
                {
                    
                    num5 = -3;
                }
                if (num5 >= 1)
                {
                    for (int m = 1; m < GemaItemManager.Instance.maxMaterial; m++)
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
                if (EventManager.Instance.isBossMode() || GemaHUDTrainingMode.Instance.isTraining() || GemaMushroomMode.Instance.isMushroom())
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
                    for (int num7 = 1; num7 < GemaItemManager.Instance.maxMaterial; num7++)
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

                    if (___currentItemType.ToString().Contains("_OrbBoost"))
                    {
                        ___currentItemType = RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbBoostD, 1) ? ItemList.Type.ITEM_OrbBoostU : ItemList.Type.ITEM_OrbBoostD;
                    }

                    if (___craftList[___selected].isUpgrade)
                    {
                        HUDObtainedItem.Instance.GiveItem(___currentItemType, (byte)(getItemUpgradeCount(___currentItemType) + 1));
                        ___isJustCraftedBadge = 1.75f;
                    }
                    else
                        HUDObtainedItem.Instance.GiveItem(___currentItemType, 1);
                    if (___currentItemType.ToString().Contains("BADGE"))
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.BadgeCrafted, (byte)(SaveManager.Instance.GetMiniFlag(Mini.BadgeCrafted) + 1));
                    }
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
    static void fixOrbShootType(ref (Character.OrbType, Character.OrbShootType, Character.OrbShootType, bool) __state)
    {
        if (__state.Item4)
        {
            EventManager.Instance.mainCharacter.cphy_perfer.PrepareSwitchOrb(false, true, __state.Item1);
            EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[0] = __state.Item2;
            EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[1] = __state.Item3;
            
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
            if (SaveManager.Instance.GetUnlockedLogic(Character.PlayerLogicState.TEVI_STRONG_GROUND_FRONT) > 0)
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
    static void AddItem(ItemList.Type item) {
        if (RandomizerPlugin.checkRandomizedItemGot(item,1))
        {
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
            SaveManager.Instance.savedata.coinUsedIan += num;
        }
    }
    static public void alreadyClaimed()
    {
        //all Items in IANs Shop
        AddItem(ItemList.Type.BADGE_BossPassing);
        AddItem(ItemList.Type.BADGE_StyleComboAirDashS);
        AddItem(ItemList.Type.BADGE_MAXHPCost);
        AddItem(ItemList.Type.BADGE_EnemyDefeatExp);
        AddItem(ItemList.Type.BADGE_1HealthTypeA);
        AddItem(ItemList.Type.ITEM_SPEEDUP);
        AddItem(ItemList.Type.BADGE_TauntThink);
        AddItem(ItemList.Type.ITEM_RapidShots);
        AddItem(ItemList.Type.BADGE_QuickMPRegainA);
        AddItem(ItemList.Type.BADGE_MapDiscoverRecover);
        AddItem(ItemList.Type.BADGE_StyleComboAirNormal3A);
        AddItem(ItemList.Type.BADGE_UpperDoubleHitAfterOutline);
        AddItem(ItemList.Type.BADGE_Hurt2Punisher);
        AddItem(ItemList.Type.BADGE_GroundNormalCombo4AltTiming);
        AddItem(ItemList.Type.BADGE_BreakTimeExtend);
        AddItem(ItemList.Type.BADGE_StrongFrontFly);
        AddItem(ItemList.Type.BADGE_1HealthTypeB);
        AddItem(ItemList.Type.ITEM_AttackRange);
        AddItem(ItemList.Type.BADGE_StepUpCharge);
        AddItem(ItemList.Type.BADGE_GroundWeakUpAirExcute);
        AddItem(ItemList.Type.BADGE_StyleComboNormal4HAAA);
        AddItem(ItemList.Type.BADGE_SupportShot);
        AddItem(ItemList.Type.BADGE_StraightCost);
        AddItem(ItemList.Type.BADGE_WeakAirNormal3ModDamageBoost);
        AddItem(ItemList.Type.BADGE_ComboBreakBoost);
        AddItem(ItemList.Type.BADGE_DodgeHealthCounter);
        AddItem(ItemList.Type.BADGE_AutoAirCombo);
        AddItem(ItemList.Type.BADGE_SuperArmor33);
        AddItem(ItemList.Type.BADGE_FreeFoodRefill);
        AddItem(ItemList.Type.ITEM_RailPass);
        AddItem(ItemList.Type.BADGE_StyleComboBackflipA);
        AddItem(ItemList.Type.BADGE_RevengeB);
        AddItem(ItemList.Type.BADGE_ComboCharge);
        AddItem(ItemList.Type.BADGE_SimpleBreak);
        AddItem(ItemList.Type.BADGE_PurchaseBadgeCost);
        AddItem(ItemList.Type.BADGE_DoubleJumpStrike);
        AddItem(ItemList.Type.BADGE_BoostTimeExtend);
        AddItem(ItemList.Type.BADGE_CrystalHeal);
        AddItem(ItemList.Type.BADGE_FallDamageReduce);
        AddItem(ItemList.Type.BADGE_DoubleDispelLineBombLow);
        AddItem(ItemList.Type.ITEM_AirshipPass);
        AddItem(ItemList.Type.BADGE_StyleComboAirDashA);
        AddItem(ItemList.Type.BADGE_StrongFrontRapid);
        AddItem(ItemList.Type.BADGE_BackflipMultiHit);
        AddItem(ItemList.Type.BADGE_ComboDodgeMeter);
        AddItem(ItemList.Type.BADGE_RythemCharge);
        AddItem(ItemList.Type.BADGE_PerfectCost);
        AddItem(ItemList.Type.BADGE_StyleComboSlidingS);
        AddItem(ItemList.Type.BADGE_DamageToMaxHP);
        AddItem(ItemList.Type.BADGE_Normal3HRedCancel);
        AddItem(ItemList.Type.BADGE_BoostQuickErase);
        AddItem(ItemList.Type.ITEM_GoldenGlove);
        AddItem(ItemList.Type.BADGE_GroundNormalCombo4AltDmg);
        AddItem(ItemList.Type.BADGE_AutoChargeCombo);
        AddItem(ItemList.Type.BADGE_DodgeSlide);
        AddItem(ItemList.Type.BADGE_1HealthTypeD);
        AddItem(ItemList.Type.BADGE_KnockConvert);
        AddItem(ItemList.Type.BADGE_BombCharge);
        AddItem(ItemList.Type.BADGE_MPAllBurst);
        AddItem(ItemList.Type.BADGE_FasterTeviStrongGroundUp);

        //All Potions from CC
        for (int i = 0; i < 5; i++)
        {
            int num = 2000 + i * 1000;
            if (num >= 5000)
            {
                num -= 1000;
            }
            if (num < 3000)
            {
                num = 3000;
            }

            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_BAG, (byte)i))
            {
                if (i == 0)
                {
                    SaveManager.Instance.savedata.coinUsedCC += 250;
                }
                else if (i == 1)
                {
                    SaveManager.Instance.savedata.coinUsedCC += 1000;
                }
                else
                {
                    SaveManager.Instance.savedata.coinUsedCC += num /= 2;

                }
            }
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_EP, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += num;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_HP, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += num;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_MATK, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += num;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_RATK, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += num;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_MP, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += num;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.STACKABLE_SHARD, (byte)i)) SaveManager.Instance.savedata.coinUsedCC += 5000;

        }
    }


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
                data = RandomizerPlugin.getRandomizedItem(item, (byte)(___ShopID + 30));
            else
                data = RandomizerPlugin.getRandomizedItem(item, 1);


            Upgradable upItem;
            bool flag = Enum.TryParse<Upgradable>(data.getItemTyp().ToString(), out upItem);

            if (item == ItemList.Type.STACKABLE_BAG)
            {
                MainVar.instance.BagID = (byte)(___ShopID + 1);
            }

            if (RandomizerPlugin.checkItemGot(data.getItemTyp(), data.getSlotId()))
                return false;




            switch (___typeN)
            {
                case Character.Type.Ian:
                    ___itemslots[___CurrentMaxItem].SetItem(item, num, false);
                    ___CurrentMaxItem++;
                    break;
                case Character.Type.CC:
                    if (item == ItemList.Type.STACKABLE_SHARD)
                    {
                        num = 5000;
                    }
                    else if (item.ToString().Contains("STACKABLE"))
                    {
                        num = 2000 + ___ShopID * 1000;
                        if (num >= 5000)
                        {
                            num -= 1000;
                        }
                        if (num < 3000)
                        {
                            num = 3000;
                        }
                        if (item == ItemList.Type.STACKABLE_BAG)
                        {
                            num /= 2;
                            if (WorldManager.Instance.Area == 3)
                            {
                                num = 250;
                            }
                            if (WorldManager.Instance.Area == 8)
                            {
                                num = 1000;
                            }
                        }
                    }
                    ___itemslots[___CurrentMaxItem].SetItem(item, num, false);
                    ___CurrentMaxItem++;
                    break;
                case Character.Type.Vena:
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
                        data = RandomizerPlugin.getRandomizedItem(___itemslots[___Selected].GetItem(), 1);
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
                        data = RandomizerPlugin.getRandomizedItem(___itemslots[___Selected].GetItem(), (byte)(30 + ___ShopID));

                    }
                    else
                    {
                        data = RandomizerPlugin.getRandomizedItem(___itemslots[___Selected].GetItem(), 1);
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
            ___itype = RandomizerPlugin.getRandomizedItem(t, (byte)(shopID + 30)).getItemTyp();
        }
        else
        {
            ___itype = RandomizerPlugin.getRandomizedItem(t, 1).getItemTyp();
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
                data = RandomizerPlugin.getRandomizedItem(item, (byte)(___ShopID + 30));
                ___item_desc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data.getItemTyp());
            }
            else
            {
                data = RandomizerPlugin.getRandomizedItem(item, 1);
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

class SaveGamePatch()
{
    //how is saveslot 0 be handled?


    static void decreaseBackupSaveSlot()
    {
        MainVar.instance._backupsaveslot--;
        if (MainVar.instance._backupsaveslot < SettingManager.Instance.GetBackupSaveSlotStart())
        {
            MainVar.instance._backupsaveslot = 99;
        }
        Debug.Log("[SystemManager] Backup save slot is now (Decreased) : " + MainVar.instance._backupsaveslot);
    }

    [HarmonyPatch(typeof(SaveManager),"LoadGame")]
    [HarmonyPostfix]
    static void loadRandomData(ref SaveManager __instance)
    {

        byte saveslot = MainVar.instance._saveslot;
        Dictionary<ItemData, ItemData> data = new Dictionary<ItemData, ItemData>();

        string result = "";
        customSaveFileNames(ref result, ref saveslot);
        if (ES3.FileExists(result))
        {
            ES3File eS3File = new ES3File(result);
            try 
            {
                int[] keyItem = eS3File.Load<int[]>("RandoKeyItem");
                int[] keySlot = eS3File.Load<int[]>("RandoKeySlot");
                int[] valItem = eS3File.Load<int[]>("RandoValItem");
                int[] valSlot = eS3File.Load<int[]>("RandoValSlot");
                for(int i =0; i < keyItem.Length; i++)
                {
                    data.Add(new ItemData(keyItem[i], keySlot[i]),new ItemData(valItem[i], valSlot[i]));
                }
            }
            catch (Exception e) 
            {
                Debug.LogError(e);
            }
            RandomizerPlugin.__itemData = data;
            if (eS3File.KeyExists("CustomDifficulty"))
            {
                RandomizerPlugin.customDiff = eS3File.Load<int>("CustomDifficulty");
            }
            
        }
    }

    [HarmonyPatch(typeof(SaveManager),"SaveGame")]
    [HarmonyPostfix]
    static void saveRandomData(ref bool backup)
    {

        byte saveslot = MainVar.instance._saveslot;
        if (MainVar.instance.CHAPTERRESET_Event > 0)
        {
            saveslot = 100;
        }
        else if (MainVar.instance._isAutoSave)
        {
            saveslot = 0;
        }
        else if (backup && (bool)WorldManager.Instance)
        {
            SettingManager.Instance.LoadSystem("backupSaveSlot");
            decreaseBackupSaveSlot();
            saveslot = MainVar.instance._backupsaveslot;
            SettingManager.Instance.IncreaseBackupSaveSlot();
        }

        string result = "";
        customSaveFileNames(ref result, ref saveslot);
        ES3File eS3File = new ES3File(result);
        Dictionary<ItemData, ItemData> s = RandomizerPlugin.__itemData;

        int[] keyItem = new int[s.Count];
        int[] keySlot = new int[s.Count];
        int[] valSlot = new int[s.Count];
        int[] valItem = new int[s.Count];
        for (int i = 0; i < s.Count; i++)
        {
            KeyValuePair<ItemData,ItemData> pair = s.ElementAt(i);
            keyItem[i] = pair.Key.itemID;
            keySlot[i] = pair.Key.slotID;
            valItem[i] = pair.Value.itemID;
            valSlot[i] = pair.Value.slotID;
        }

        eS3File.Save("RandoKeyItem",keyItem);
        eS3File.Save("RandoKeySlot",keySlot);
        eS3File.Save("RandoValItem", valItem);
        eS3File.Save("RandoValSlot", valSlot);
        eS3File.Save("CustomDifficulty", RandomizerPlugin.customDiff);
        eS3File.Sync();
        Randomizer.saveSpoilerLog($"rando.SpoilerSave{saveslot}.txt",s);
    }

    [HarmonyPatch(typeof(SaveManager),"GetSaveFileName")]
    [HarmonyPrefix]
    static bool customSaveFileNames(ref string __result,ref byte saveslot) {
        __result = "randomizer/rando.tevisave" + saveslot+".sav";
        return false;

    }

}


class BonusFeaturePatch()
{
    /*
    [HarmonyPatch(typeof(GemaChargedShotCombo),"AddMeter")]
    static void MOREPOWAR(ref int add)
    {
        add *= 1;
    }*/

    // new dropkick mechanic
    static int bonusDropKickDmg;
    static bulletScript currentDropKick;
    static bool DropKickDmgUpdated = false;
    static (CharacterBase, BulletType) lastHit;
    [HarmonyPatch(typeof(SaveManager),"TryRenewLevel")]
    [HarmonyPostfix]
    static void resetBonusDmg()
    {
        bonusDropKickDmg = 0;
    }


    [HarmonyPatch(typeof(CharacterBase),"BulletHurtPlayer")]
    [HarmonyPostfix]
    static void test(ref CharacterBase owner,float damage,BulletType type,ref CharacterBase __instance)
    {
        if(type == BulletType.QUICK_DROP)
        {
            bonusDropKickDmg++;
        }
        else if(owner.isPlayer() || __instance.isPlayer())
        {

            bonusDropKickDmg = 0;
        }

    }

    [HarmonyPatch(typeof(BulletManager), "ShootBullet")]
    [HarmonyPostfix]
    static void getNewBullets(ref bulletScript __result, ref BulletType type)
    {

        if (type == BulletType.QUICK_DROP)
        {
            DropKickDmgUpdated = false;
            currentDropKick = __result;
        }

    }
    static float testValue = 0.05f;
    [HarmonyPatch(typeof(CharacterBase),"_Update")]
    [HarmonyPostfix]
    static void updateDropkickDamage(ref PlayerLogicState ___logicStatus,ref ObjectPhy ___phy_perfer, ref CharacterPhy ___cphy_perfer)
    {
        if(___logicStatus == PlayerLogicState.QUICKDROP && !DropKickDmgUpdated && currentDropKick != null)
        {
            DropKickDmgUpdated = true;
            float num3 = 0.343525f; 
            num3 += testValue*bonusDropKickDmg; 
            if (___cphy_perfer != null)
            {
                num3 += (float)(int)___cphy_perfer.quickdrophit * 0.02f;
            }
            if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_DoubleJumpStrike) && ___phy_perfer.jumped >= 2)
            {
                num3 *= 1.17f;
            }
            currentDropKick.SetDamage(num3);
        }
    }

}


class ScalePatch()
{

    [HarmonyPatch(typeof(enemyController), "SetMaxBossHealth")]
    [HarmonyPrefix]
    static bool setMaxBossHealth(ref enemyController __instance)
    {
        if (RandomizerPlugin.customDiff < 0) return true;

        int num = RandomizerPlugin.customDiff - 5;
        float num2 = 0f;
        float num3 = 0f;
        if (num > 0)
        {
            float num4 = 0f;
            if (num <= 5)
            {
                float num5 = (float)num * 0.067f;
                if (num5 > 0.3f)
                {
                    num5 = 0.3f;
                }
                num2 += num5;
            }
            else
            {
                num2 += 0.3f + (float)(num - 5) * 0.001f;
            }
            num4 = num2;
            if (num >= 0)
            {
                num4 += (float)(num - 5) * 5E-05f;
            }
            __instance.health = (int)((float)__instance.health * (1f + num2));
            num3 = num4;
            num3 = ((!(num3 < 0f)) ? (num3 / 3f) : 0f);
            __instance.maxToBreak = (int)(__instance.toBreak * (1f + num3));
            __instance.toBreak = __instance.maxToBreak;
        }
        Debug.Log("[BossHealth] " + __instance.type.ToString() + "'s health set to " + __instance.health + ", BreakBar set to " + __instance.toBreak + " Mod : " + num2 + " BMod : " + num3);
        return false;
    }

    [HarmonyPatch(typeof(enemyController), "InitStart")]      
    [HarmonyPostfix]
    static void balanceAct(ref enemyController __instance)
    {
        if (RandomizerPlugin.customDiff < 0) return;

        if (!TeamManager.Instance.enemyMembers.Contains(__instance))
        {
            return;
        }

        enemyController.EnemyData enemyDB = __instance.GetEnemyDB(__instance.type.ToString());
        if (enemyDB == null)
        {
            if (Application.isEditor)
            {
                Debug.Log("[NPC] Warning : No enemy data for " + __instance.type.ToString() + ". Using DEFAULT Data.");
            }
            enemyDB = __instance.GetEnemyDB("DEFAULT");
        }
        if (enemyDB != null)
        {


            int atk = enemyDB.atk;
            int health = enemyDB.MaxHP;
            __instance.maxToBreak = enemyDB.toBreak;
            Traverse t = Traverse.Create(__instance);

            if (__instance.isBoss == Character.BossType.BOSS)
            {
                if (SaveManager.Instance.GetCustomGame(CustomGame.HighBossScale) || GemaBossRushMode.Instance.isBossRushTypeEqualOrHigher(BossRushType.XTREME))
                {
                    atk = (int)((float)atk * (1f + 0.014f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP) + 0.004f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD)));
                    atk = (int)((float)atk * (1f + 0.0004f * (float)SaveManager.Instance.GetUsedCost(GetRemain: false)));
                }
                else
                {
                    float num = 0.002f;
                    if (RandomizerPlugin.customDiff >= (short)Difficulty.D5)
                    {
                        num = 0.008f;
                    }
                    atk = (int)((float)atk * (1f + num * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP) + 0.001f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD)));
                    if (RandomizerPlugin.customDiff >= (short)Difficulty.D4)
                    {
                        atk = (int)((float)atk * (1f + 0.0002f * (float)SaveManager.Instance.GetUsedCost(GetRemain: false)));
                    }
                }
            }
            else if (!EventManager.Instance.isBossMode())
            {
                atk += SaveManager.Instance.GetChapter() / 2 + (int)((float)(SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP) + SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD)) * 0.25f);
            }

            float num2 = 1f;
            float num3 = RandomizerPlugin.customDiff - 5;
            if (RandomizerPlugin.customDiff == (short)Difficulty.D6)
            {
                num3 = 2f;
            }
            else if (RandomizerPlugin.customDiff == (short)Difficulty.D7)
            {
                num3 = 3f;
            }
            if (num3 < 0f)
            {
                float num4 = 1f + num3 * 0.1f;
                if (RandomizerPlugin.customDiff <= (short)Difficulty.D0)
                {
                    num4 -= 0.1f;
                }
                if (num4 < 0.1f)
                {
                    num4 = 0.1f;
                }
                num2 *= num4;
            }
            if (num3 > 0f)
            {
                num2 = ((!(num3 <= 5f)) ? (num2 + 0.75f) : (num2 + num3 * 0.15f));
            }
            if (num3 >= 0f && __instance.isBoss != Character.BossType.BOSS)
            {
                num2 += 0.1f;
            }
            if (num3 >= 0.99f && __instance.isBoss != Character.BossType.BOSS)
            {
                num2 += 0.1f;
            }
            atk = (int)((float)atk * num2);
            if (num3 > 0f)
            {
                atk += (int)(num3 * 1.75f);
            }

            // HP
            if (__instance.isBoss == Character.BossType.BOSS)
            {
                if (!GemaBossRushMode.Instance.isBossRush() || (int)GemaBossRushMode.Instance.BossRushType >= 1)
                {
                    float num5 = 1f + 0.008f * ((float)SaveManager.Instance.GetMainLevel() + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD) / 2f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK));
                    if (SaveManager.Instance.GetCustomGame(CustomGame.HighBossScale) || GemaBossRushMode.Instance.isBossRushTypeEqualOrHigher(BossRushType.XTREME))
                    {
                        num5 += 0.000225f * (float)SaveManager.Instance.GetUsedCost(GetRemain: false);
                    }
                    else
                    {
                        float num6 = 0.005f;
                        if (RandomizerPlugin.customDiff >= (short)Difficulty.D6)
                        {
                            num6 = 0.006f;
                        }
                        if (RandomizerPlugin.customDiff >= (short)Difficulty.D7)
                        {
                            num6 = 0.007f;
                        }
                        if (RandomizerPlugin.customDiff >= (short)Difficulty.D9)
                        {
                            num6 = 0.008f;
                        }
                        num5 = 1f + num6 * ((float)SaveManager.Instance.GetMainLevel() + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD) / 4f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) / 3.5f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK) / 3.5f);
                        if (RandomizerPlugin.customDiff >= (short)Difficulty.D6 && SaveManager.Instance.GetChapter() >= 4)
                        {
                            num5 += 0.000125f * (float)SaveManager.Instance.GetUsedCost(GetRemain: false);
                        }
                    }
                    health = (int)((float)health * num5);
                }
                subBossBoost(ref __instance);
                setMaxBossHealth(ref __instance);

                __instance.SetBreakTime();
                if (!GemaBossRushMode.Instance.isBossRush())
                {
                    if (__instance.type == Character.Type.PKOA || __instance.type == Character.Type.Frankie || __instance.type == Character.Type.Jezbelle || __instance.type == Character.Type.Lily || SaveManager.Instance.GetDifficultyName() <= Difficulty.D7)
                    {
                        int num7 = MainVar.instance.justGameOverTime;
                        if (num7 > 10)
                        {
                            num7 = 10;
                        }
                        if (RandomizerPlugin.customDiff <= (short)Difficulty.D5 && num7 > 8)
                        {
                            num7 = 8;
                        }
                        if (RandomizerPlugin.customDiff <= (short)Difficulty.D7 && num7 > 2)
                        {
                            num7 = 2;
                        }
                        if (atk > 1)
                        {
                            atk -= num7;
                            if (atk < 1)
                            {
                                atk = 1;
                            }
                            if (RandomizerPlugin.customDiff >= (short)Difficulty.D7)
                            {
                                float num8 = 1f - (float)num7 * 0.025f;
                                if (RandomizerPlugin.customDiff <= (short)Difficulty.D5)
                                {
                                    num8 = 1f - (float)num7 * 0.01f;
                                    num8 += 0.025f;
                                }
                                if (num8 < 0.7f)
                                {
                                    num8 = 0.7f;
                                }
                                if (num8 > 1f)
                                {
                                    num8 = 1f;
                                }
                                if (RandomizerPlugin.customDiff <= (short)Difficulty.D7 && num8 < 0.85f)
                                {
                                    num8 = 0.85f;
                                }
                                health = (int)((float)health * num8);
                            }
                            Debug.Log("[Boss Started] Base ATK R : " + num7 + " | " + atk + " - " + health);
                        }
                    }
                    if (SaveManager.Instance.GetCustomGame(CustomGame.HighBossScale))
                    {
                        __instance.maxToBreak += (int)(__instance.maxToBreak * (0.01f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK)));
                    }
                    else
                    {
                        __instance.maxToBreak += (int)(__instance.maxToBreak * (0.005f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK)));
                    }
                }
                t.Method("DifficultyBossHealth").GetValue();
                Debug.Log("[Boss Started] Base Health : " + enemyDB.MaxHP + " Spawn Health : " + health);
            }

            //More HP and ATK for normal Enemies
            else if (!EventManager.Instance.isBossMode())
            {
                health += (int)((float)(SaveManager.Instance.GetChapter() * 2 + SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD)) + (float)(SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) + SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK)) / 3f);
                float num9 = 0f;
                num3 = (float)(RandomizerPlugin.customDiff - 5);
                num9 = ((!(num3 > 0f)) ? 0f : ((!(num3 <= 5f)) ? (num9 + (0.375f + (num3 - 5f) * 0.0001f)) : (num9 + num3 * 0.075f)));
                float num10 = (float)(int)SaveManager.Instance.GetChapter() * 0.1f;
                if (num10 > 1f)
                {
                    num10 = 1f;
                }
                num9 *= num10;
                health = (int)((float)health * (1f + num9 / 3.5f));

                if ((WorldManager.Instance.Area != 0 || SaveManager.Instance.GetChapter() >= 1) && !EventManager.Instance.isBossMode() && SaveManager.Instance.GetChapter() <= 3 && EventManager.Instance.GetCurrentEventBattle() == Mode.OFF)
                {
                    if (RandomizerPlugin.customDiff <= (short)Difficulty.D6)
                    {
                        health = (int)((float)health * 0.925f);
                    }
                    if (RandomizerPlugin.customDiff <= (short)Difficulty.D5)
                    {
                        health = (int)((float)health * 0.925f);
                    }
                }
                if (RandomizerPlugin.customDiff >= (short)Difficulty.D5)
                {
                    atk = (int)((float)atk * 1.025f);
                }
                if (RandomizerPlugin.customDiff >= (short)Difficulty.D10)
                {
                    atk = (int)((float)atk * 1.125f);
                }
            }




            if (RandomizerPlugin.customDiff <= (short)Difficulty.D0)
            {
                atk = (int)((float)atk * 0.4f);
            }
            else if (RandomizerPlugin.customDiff <= (short)Difficulty.D1)
            {
                atk = (int)((float)atk * 0.8f);
            }
            if (atk > 0)
            {
                atk += (int)((float)RandomizerPlugin.customDiff / 2f);
                if (RandomizerPlugin.customDiff >= (short)Difficulty.D10)
                {
                    atk += SaveManager.Instance.GetChapter() / 2;
                }
                else if (RandomizerPlugin.customDiff <= (short)Difficulty.D0)
                {
                    atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 3f);
                }
                else if (RandomizerPlugin.customDiff <= (short)Difficulty.D1)
                {
                    atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 2f);
                }
                else if (RandomizerPlugin.customDiff <= (short)Difficulty.D3)
                {
                    atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 1.5f);
                }
                else if (RandomizerPlugin.customDiff <= (short)Difficulty.D5)
                {
                    atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 1f);
                }
                if (atk < 1)
                {
                    atk = 1;
                }
            }
            if (GemaBossRushMode.Instance.isBossRush())
            {
                //ChapterEnemyBoost();
                if (GemaBossRushMode.Instance.BossRushType == BossRushType.BEGINNER)
                {
                    health = (int)((float)health * 0.25f);
                    atk = (int)((float)atk * 0.5f);
                }
                if (GemaBossRushMode.Instance.BossRushType == BossRushType.STANDARD)
                {
                    health = (int)((float)health * 0.35f);
                    atk = (int)((float)atk * 0.6f);
                }
                if (GemaBossRushMode.Instance.BossRushType == BossRushType.MASTER)
                {
                    health = (int)((float)health * 0.55f);
                    atk = (int)((float)atk * 1.4f);
                }
                if (GemaBossRushMode.Instance.BossRushType == BossRushType.XTREME)
                {
                    health = (int)((float)health * 0.85f);
                    atk = (int)((float)atk * 1.4f);
                }
                if (GemaBossRushMode.Instance.BossRushType == BossRushType.LIBRARY)
                {
                    health = (int)((float)health * 1.42f);
                    atk = (int)((float)atk * 1.75f);
                }
                else
                {
                    if (__instance.type == Character.Type.GemaYue_B || __instance.type == Character.Type.Waero_B || __instance.type == Character.Type.EinLee_B)
                    {
                        health = (int)((float)health * 0.75f);
                    }
                    if (__instance.type == Character.Type.Jezbelle)
                    {
                        health = (int)((float)health * 0.9f);
                    }
                    if (__instance.type == Character.Type.Jethro)
                    {
                        health = (int)((float)health * 0.9f);
                    }
                }
                if (atk < 20)
                {
                    atk = 20;
                }
            }
            else
            {
                if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam) || SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0)
                {
                    float maxBkrea = __instance.maxToBreak;
                    t.Method("ChapterEnemyBoost").GetValue();
                    __instance.maxToBreak = maxBkrea;
                    __instance.toBreak = maxBkrea;
                }
                if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                {
                    t.Method("FreeRoamEnemyBoost").GetValue();
                }
                if (__instance.type == Character.Type.GemaYue_B || __instance.type == Character.Type.EinLee_B || __instance.type == Character.Type.Waero_B)
                {
                    health = (int)((float)health * 2.2f);
                    atk = (int)((float)atk * 3.125f);
                }
            }

            __instance.maxhealth = health;
            __instance.health = health;
            __instance.atk = atk;
        }
    }
    [HarmonyPatch(typeof(enemyController), "SubBossBoost")]
    [HarmonyPrefix]
    static bool subBossBoost(ref enemyController __instance)
    {
        if (RandomizerPlugin.customDiff < 0) return true;

        if (GemaBossRushMode.Instance.isBossRush() || (__instance.type != Character.Type.Barados && __instance.type != Character.Type.Caprice && __instance.type != Character.Type.Katu && __instance.type != Character.Type.Thetis && __instance.type != Character.Type.Roleo))
        {
            return false;
        }
        if (__instance.type == Character.Type.Katu)
        {
            if (RandomizerPlugin.customDiff >= (short)Difficulty.D7)
            {
                __instance.health += 125;
            }
            if (RandomizerPlugin.customDiff >= (short)Difficulty.D10)
            {
                __instance.health += 125;
            }
        }
        if (__instance.type != Character.Type.Thetis && __instance.type != Character.Type.Roleo)
        {
            if (SaveManager.Instance.GetMiniFlag(Mini.Sidequest1_Mine_Finished) > 0)
            {
                __instance.health += 200;
                __instance.atk++;
            }
            if (SaveManager.Instance.GetMiniFlag(Mini.Sidequest1_Ruin_Finished) > 0)
            {
                __instance.health += 300;
                __instance.atk += 2;
            }
            if (SaveManager.Instance.GetMiniFlag(Mini.Sidequest1_Wasteland_Finished) > 0)
            {
                __instance.health += 200;
                __instance.atk++;
            }
            if (SaveManager.Instance.GetChapter() >= 2)
            {
                __instance.health += 200;
                __instance.atk++;
            }
        }
        if (SaveManager.Instance.GetMiniFlag(Mini.Sidequest2_Beach_Finished) > 0)
        {
            __instance.health += 300;
            __instance.atk += 3;
        }
        if (SaveManager.Instance.GetChapter() >= 3)
        {
            __instance.health = (int)((float)__instance.health * (1f + 0.2525f * (float)(SaveManager.Instance.GetChapter() - 2)));
            __instance.atk += 4 * (SaveManager.Instance.GetChapter() - 2);
            __instance.maxToBreak = (int)(__instance.toBreak * (1f + 0.172f * (float)(SaveManager.Instance.GetChapter() - 2)));
            __instance.toBreak = __instance.maxToBreak;
            SaveManager.Instance.DifficultyMinMaxOffset = (int)((float)(SaveManager.Instance.GetChapter() - 2) * 0.75f);
            if (SaveManager.Instance.DifficultyMinMaxOffset > 4)
            {
                SaveManager.Instance.DifficultyMinMaxOffset = 4;
            }
        }
        if (SaveManager.Instance.GetChapter() >= 4)
        {
            if (__instance.type == Character.Type.Katu)
            {
                __instance.health = (int)((float)__instance.health * 1.015f);
            }
            else
            {
                __instance.health = (int)((float)__instance.health * 1.05f);
            }
            __instance.atk += 2;
        }
        if (SaveManager.Instance.GetChapter() >= 5)
        {
            if (__instance.type != Character.Type.Katu)
            {
                __instance.health = (int)((float)__instance.health * 1.05f);
            }
            __instance.atk += 3;
        }
        Debug.Log("[BossHealth] Boost Sub Boss : " + __instance.type.ToString() + "'s health set to " + __instance.health + ", ATK set to " + __instance.atk + " , break set to " + __instance.maxToBreak);
        return false;
    }


    [HarmonyPatch(typeof(enemyController), "DifficultyBossHealth")]
    [HarmonyPrefix]
    static bool difficultyBossHealth(ref enemyController __instance)
    {
        if (RandomizerPlugin.customDiff < 0) return true;

        float num = 0f;
        Difficulty difficultyName = (Difficulty)RandomizerPlugin.customDiff;
        if (difficultyName >= Difficulty.D6)
        {
            float num2 = 0f;
            if (__instance.type == Character.Type.Vassago || __instance.type == Character.Type.Amaryllis || __instance.type == Character.Type.Tahlia)
            {
                num2 += 0.3f;
            }
            else if (__instance.type == Character.Type.Charon || __instance.type == Character.Type.Jezbelle || __instance.type == Character.Type.Cyril || __instance.type == Character.Type.Jethro || __instance.type == Character.Type.Eidolon)
            {
                num2 += 0.25f;
            }
            if (!(num2 > 0f))
            {
                return false;
            }
            __instance.atk += (int)(40f * num2);
            if (num2 > 0.1f)
            {
                float num3 = (float)(SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) + SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK)) * 0.0667f;
                if (num3 > 1f)
                {
                    num3 = 1f;
                }
                int num4 = (int)((float)__instance.health * (num2 * num3));
                __instance.health += num4;
                float num5 = (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) * 0.022f;
                if (num5 > 1f)
                {
                    num5 = 1f;
                }
                int num6 = (int)(__instance.maxToBreak * ((1f + num2) * num5));
                __instance.maxToBreak += num6;
                Debug.Log("[enemyController] " + num2 + " | HP : " + num3 + " , " + num4 + " BREAK : " + num5 + " " + num6);
            }
            return false;
        }
        num = ((difficultyName <= Difficulty.D0) ? 0.4f : (difficultyName switch
        {
            Difficulty.D1 => 0.35f,
            Difficulty.D2 => 0.3f,
            Difficulty.D3 => 0.25f,
            Difficulty.D4 => 0.2f,
            Difficulty.D5 => 0.15f,
            _ => 0.1f,
        }));
        if (SaveManager.Instance.GetChapter() <= 1)
        {
            num /= 1.75f;
        }
        else if (SaveManager.Instance.GetChapter() <= 2)
        {
            num /= 1.5f;
        }
        else if (SaveManager.Instance.GetChapter() <= 3)
        {
            num /= 1.25f;
        }
        float num7 = 0f;
        if (__instance.type == Character.Type.Vassago || __instance.type == Character.Type.Amaryllis || __instance.type == Character.Type.Cyril || __instance.type == Character.Type.Tahlia)
        {
            num7 += 0.2f;
        }
        else if (__instance.type == Character.Type.Charon || __instance.type == Character.Type.Jezbelle || __instance.type == Character.Type.Jethro || __instance.type == Character.Type.Memloch || __instance.type == Character.Type.Eidolon)
        {
            num7 += 0.1f;
        }
        else if (__instance.type == Character.Type.Revenance)
        {
            num7 += 0.05f;
        }
        if (num7 > 0f)
        {
            if (difficultyName >= Difficulty.D7)
            {
                num7 /= 2f;
            }
            if (num <= 0f)
            {
                num += num7;
            }
            else if (num <= num7)
            {
                num7 *= 1f + num;
                num = num7;
            }
            else
            {
                num *= 1f + num7 / 2f;
            }
        }
        if (num > 0.5f)
        {
            num = 0.5f;
        }
        if (num > 0f)
        {
            __instance.health -= (int)((float)__instance.health * num);
        }
        return false;
    }

    [HarmonyPatch(typeof(GemaUIChangeDifficulty),"Update")]
    [HarmonyPrefix]
    static bool changeCustomDiff(ref GemaUIChangeDifficulty __instance, ref bool ___isEnable, ref TextMeshProUGUI ___promotetext,ref int ___difficulty)
    {
        if (RandomizerPlugin.customDiff < 0) return true;

        if (___promotetext.enabled && InputButtonManager.Instance.GetButtonDown(13))
        {
            ___isEnable = false;
            CameraScript.Instance.PlaySound(AllSound.SEList.MENUSELECT);
            FadeManager.Instance.SetAll(0f, 0f, 0f, 0.7f, 0f, 22f);
            if (!BackgroundManager.Instance.canHeal || EventManager.Instance.getMode() == Mode.Chap1BedSleep)
            {
                RandomizerPlugin.customDiff = ___difficulty;
            }
            else
            {
                EventManager.Instance.TryStartEvent(Mode.Chap1ChangeDifficulty, force: true);
            }
            return false;
        }
        return true;
    }
    [HarmonyPatch(typeof(Chap1ChangeDifficulty),"EVENT")]
    [HarmonyPrefix]
    static void changeCustomDiffi()
    {
        if (EventManager.Instance.EventStage == 60 && RandomizerPlugin.customDiff >= 0)
        {

            RandomizerPlugin.customDiff = GemaUIChangeDifficulty.Instance.GetSetDifficulty();
            MusicManager.Instance.SetTargetVolume(1f, 1f);
            EventManager.Instance.SetStage(70);
        }
    }
    [HarmonyPatch(typeof(ObjectPhy),"WallHit")]
    [HarmonyPrefix]
    static bool reduceWallDmg(ref CharacterBase ___cb_perfer)
    {
        if (RandomizerPlugin.customDiff < 0) return true;
        if (___cb_perfer.isPlayer())
        {
            float maxMult = 1f;
            if (RandomizerPlugin.customDiff >= 21)
                maxMult = 1.1f + (RandomizerPlugin.customDiff - 20) * 0.0001f;
            float diff = RandomizerPlugin.customDiff - 5 + SaveManager.Instance.DifficultyMinMaxOffset;
            int num = (int)(5f + Mathf.Lerp(4f, 100f * maxMult, diff * 1.25f)) + (int)((float)(int)___cb_perfer.GetBuffLv(Character.BuffType.WallDamageAmp) * (5f + Mathf.Lerp(4f, 100f * maxMult, diff * 0.8f)));
            ___cb_perfer.ReduceHealth(num, lethal: false);
            DamageManager.Instance.CreateDamage(num.ToString(), num, ___cb_perfer.t.position + new Vector3(0f, (float)___cb_perfer.damagePosition * 20f - 10f + ___cb_perfer.OverallOffsetY), ___cb_perfer, Character.DamageTextType.NORMAL, new Color32(225, 177, 177, byte.MaxValue), new Color32(byte.MaxValue, 77, 77, byte.MaxValue), 20f);
            return false;
        }
        return true;
    }

}

class LocalizePatch()
{
    static Dictionary<string,string> newLocalize = new Dictionary<string,string>();

    [HarmonyPatch(typeof(Localize), "GetLocalizeTextWithKeyword")]
    [HarmonyPrefix]
    static bool getNewLocalize(ref string keyword,ref string __result)
    {
        if (newLocalize.ContainsKey(keyword))
        {
            __result = newLocalize[keyword];
            return false;
        }
        return true;
    }
}