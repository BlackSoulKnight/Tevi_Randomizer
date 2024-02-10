using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;
using Map;
using UnityEngine.Analytics;







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
}









[BepInPlugin("tevi.plugins.randomizer", "Randomizer", "0.0.0.1")]
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
            data = __itemData[new ItemData((int)itemid, (int)slotid)];
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
            if (EventManager.Instance.GetElm(__instance.transform, 0f, MainVar.instance.TILESIZE) == ElementType.NotFreeRoamOnly)
            {
                __instance.DisableMe();
                return false;
            }

        }
        else
        {
            ___slotid = 1;
        }

        ItemData data = loadRandomizedItem(__instance.itemid, ___slotid);

        var spr = CommonResource.Instance.GetItem(data.itemID);

        if (data.itemID >= (int)ItemList.Type.BADGE_START && data.itemID <= (int)ItemList.Type.BADGE_MAX)
        {
            if (____secondsprite == null)
            {
                GameObject newobj = new GameObject("SecondTile");
                newobj.transform.SetParent(__instance.gameObject.transform);
                newobj.AddComponent<SpriteRenderer>();
                ____secondsprite = newobj.GetComponent<SpriteRenderer>();
                ____secondsprite.SetMaterial(CommonResource.Instance.mat_SpriteLighting);
                ____secondsprite.drawMode = SpriteDrawMode.Simple;
                ____secondsprite.sortingOrder = 43;
                ____secondsprite.sortingLayerName = "ItemAndEvent";
            }

            ____secondsprite.sprite = spr;
            ____secondsprite.enabled = true;

            ____sprite.sprite = CommonResource.Instance.Sprite_BadgeBackground;
            ____childsprite.sprite = ____sprite.sprite;

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

            ItemData data = loadRandomizedItem(item, (byte)(___ShopID + 30));


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

        }


        return false;
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

    //Boss Defeated
    [HarmonyPatch(typeof(END_BOOKMARK), "EVENT")]
    [HarmonyPrefix]
    static void BossDefeatEvent(ref AreaType ___justArea)
    {
        EventManager em = EventManager.Instance;

        switch (em.EventStage)
        {
            case 100:
                if (!(em.EventTime > 2.5f))
                {
                    break;
                }
                em.StopEvent();
                ItemData data;
                WorldManager.Instance.ToggleFlagGate(t: true);
                MusicManager.Instance.PlayRoomMusic();
                CameraScript.Instance.DisableEnemySpawn = true;
                SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 0);
                if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0)
                {
                    SaveManager.Instance.SetMiniFlag(Mini.NoBookmarkUntilMapChange, 1);
                }
                if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                {
                    byte value = (byte)(WorldManager.Instance.Area + 20);
                    if (WorldManager.Instance.CurrentRoomBG == RoomBG.SEAL)
                    {
                        value = 63;
                    }
                    if (WorldManager.Instance.CurrentRoomBG == RoomBG.A_UNDERPALACE)
                    {
                        value = 62;
                    }
                    if (WorldManager.Instance.CurrentRoomBG == RoomBG.SNOWCAVE)
                    {
                        value = (byte)((___justArea != AreaType.QUEENSGARDEN && ___justArea != AreaType.MACHINECAVE) ? 61 : 60);
                    }



                    data = loadRandomizedItem(ItemList.Type.STACKABLE_COG, value);
                    HUDObtainedItem.Instance.GiveItem((ItemList.Type)data.itemID, (byte)data.slotID);


                    if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) <= 0)
                    {

                        if (WorldManager.Instance.CurrentRoomBG == RoomBG.SEAL || WorldManager.Instance.CurrentRoomBG == RoomBG.ZENITH)
                        {
                            HUDResourceGotPopup.Instance.AddPopup(ItemList.Resource.UPGRADE, SaveManager.Instance.GetResource(ItemList.Resource.UPGRADE), 2);
                            SaveManager.Instance.AddResource(ItemList.Resource.UPGRADE, 2);
                            HUDResourceGotPopup.Instance.AddPopup(ItemList.Resource.CORE, SaveManager.Instance.GetResource(ItemList.Resource.CORE), 2);
                            SaveManager.Instance.AddResource(ItemList.Resource.CORE, 2);
                        }
                        HUDResourceGotPopup.Instance.AddPopup(ItemList.Resource.COIN, SaveManager.Instance.GetResource(ItemList.Resource.COIN), 1000);
                        SaveManager.Instance.AddResource(ItemList.Resource.COIN, 1000);
                    }
                }
                if (WorldManager.Instance.CurrentRoomArea == AreaType.FORESTMAZE)
                {
                    FogManager.Instance.ToggleMe(toggle: false);
                    FogManager.Instance.ToggleMe(toggle: true);
                    if (SaveManager.Instance.GetEventFlag(Mode.MazeCompleted) > 0)
                    {
                        WorldManager.Instance.UseRoomWarp = false;
                        GemaMazeHUD.Instance.DontShowMaze();
                    }
                    else
                    {
                        WorldManager.Instance.UseRoomWarp = true;
                        GemaMazeHUD.Instance.ShowMaze();
                    }
                    WorldManager.Instance.ToggleFlagGate(t: true);
                }
                if (WorldManager.Instance.CurrentRoomArea == AreaType.OASISSTAGE)
                {
                    ItemTile itemTile = null;
                    for (int i = 0; i < WorldManager.Instance.areadata.itemlist.Count; i++)
                    {
                        itemTile = WorldManager.Instance.areadata.itemlist[i];
                        if ((bool)itemTile && itemTile.itemid == ItemList.Type.ITEM_PKBADGE)     //could be problematic need to tested
                        {
                            itemTile.OnBecameVisible();
                            break;
                        }
                    }
                }
                if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam) && SaveManager.Instance.GetMiniFlag(Mini.GameCleared) <= 0)
                {
                    SaveManager.Instance.AutoSave(forced: true);
                }
                em.NextStage();
                break;
        }

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


