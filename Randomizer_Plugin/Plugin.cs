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
using static Localize;
using System.Runtime.InteropServices.ComTypes;






namespace TeviRandomizer
{

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
        CompassStart = 2,
        TempOption = 3,

    }


    [BepInPlugin("tevi.plugins.randomizer", "Randomizer", "1.1")]
    [BepInProcess("TEVI.exe")]
    public class RandomizerPlugin : BaseUnityPlugin
    {


        public enum EventID
        {
            IllusionPalace = 9999
        }


        static public Dictionary<string, string> __itemData = new Dictionary<string, string>();

        static public int customDiff = -1; //fake diff

        static public bool[] customFlags = new bool[Enum.GetNames(typeof(CustomFlags)).Length];
        static public int[] extraPotions = [0, 0]; // Hardcoded omo
        static public int GoMode = -1;
        static public string pluginPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static public string seed;


        static public Randomizer randomizer;
        static private bool randomizerEnabled = false;
        static private Harmony harmonyPatchInstance = new Harmony("Randomizer");

        private void Awake()
        {
            this.gameObject.AddComponent<ArchipelagoInterface>();
            ArchipelagoInterface.Instance = gameObject.GetComponent<ArchipelagoInterface>();
            Localize.GetLocalizeTextWithKeyword("", false);
            System.IO.Directory.CreateDirectory(pluginPath + "Data");
            randomizer = Randomizer.Instance;

            harmonyPatchInstance.PatchAll(typeof(UI));
            toggleRandomizerPlugin();

            Logger.LogInfo($"Plugin Randomizer is loaded!");

        }


        static public bool toggleRandomizerPlugin()
        {
            if (randomizerEnabled)
            {
                harmonyPatchInstance.UnpatchSelf();
                harmonyPatchInstance.PatchAll(typeof(UI));
                randomizerEnabled = false;
                return false;
            }
            else
            {
                harmonyPatchInstance.PatchAll(typeof(RandomizerPlugin));
                harmonyPatchInstance.PatchAll(typeof(CraftingPatch));
                harmonyPatchInstance.PatchAll(typeof(ShopPatch));
                harmonyPatchInstance.PatchAll(typeof(EventPatch));
                harmonyPatchInstance.PatchAll(typeof(ItemSystemPatch));
                harmonyPatchInstance.PatchAll(typeof(SaveGamePatch));
                //instance.PatchAll(typeof(ScalePatch));
                harmonyPatchInstance.PatchAll(typeof(OrbPatch));
                harmonyPatchInstance.PatchAll(typeof(RabiSmashPatch));
                harmonyPatchInstance.PatchAll(typeof(BonusFeaturePatch));
                harmonyPatchInstance.PatchAll(typeof(HintSystem));
                harmonyPatchInstance.PatchAll(typeof(CustomMap));
                randomizerEnabled = true;
                return true;
            }
        }


        // Create a new Text without Localization
        static Localize.SystemText createNewText(string keyword, string text)
        {
            Localize.SystemText newText = new Localize.SystemText();
            newText.keyword = keyword;
            newText.tchinese = text;
            newText.japanese = text;
            newText.english = text;
            newText.spanish = text;
            newText.russian = text;
            newText.ukrainian = text;
            newText.schinese = text;
            newText.korean = text;

            return newText;
        }

        static public void changeSystemText(string keyword,string text)
        {
            var t = new Traverse(typeof(Localize));
            var library = t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, SystemText>>();
            if (library != null)
            {
                if (!library.ContainsKey(keyword))
                {
                    Localize.SystemText newText = createNewText(keyword, text);
                    t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                    t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
                }
                else
                {
                    library[keyword].english = text;
                }
            }
        }
        static void addLang()
        {
            var t = new Traverse(typeof(Localize));
            { 
                Localize.SystemText newText = createNewText("ITEMNAME.I19", "Celia");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I19", "Celia is now available");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMNAME.I20", "Sable");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I20", "Sable is now available");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }



        }

        [HarmonyPatch(typeof(Localize), "GetLocalizeTextWithKeyword")]
        [HarmonyPrefix]
        static bool addLan(ref List<Localize.SystemText> ___jsonlistSysTxt, ref Dictionary<string, Localize.SystemText> ___jsonlistSysTxtDictionary)
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
        [HarmonyPatch(typeof(Localize), "GetLocalizeTextWithKeyword")]
        [HarmonyPostfix]
        static void changeText(ref string __result,ref string keyword)
        {
            if(keyword == "Todo.GoalTipFreeRoam")
            {
                __result = __result.Replace("16", GoMode.ToString());
            }
            if(ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected)
            {

            }
        }


        static public void createSeed()
        {
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.disconnect();
            }
            System.Random rando;
            if (seed == "")
            {
                seed = new System.Random().Next(int.MaxValue).ToString();
            }
            rando = new System.Random(seed.GetHashCode());

            randomizer.createSeed(rando);
        }


        static public ItemList.Type getRandomizedItem(ItemList.Type itemid, byte slotid) => getRandomizedItem(itemid.ToString(), slotid);
 
        static public ItemList.Type getRandomizedItem(string item, byte slot)
        {

            ItemList.Type data;

            if (ArchipelagoInterface.Instance.isConnected && !ArchipelagoInterface.Instance.isItemNative($"{item} #{slot}") || item == "Remote")
            {
                data = ArchipelagoInterface.Instance.isItemProgessive($"{item} #{slot}") ? ArchipelagoInterface.remoteItemProgressive : ArchipelagoInterface.remoteItem;
            }
            else
            {
                try { 
                    data = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), __itemData[LocationTracker.APLocationName[$"{item} #{slot}"]]);
                }
                catch
                {
                    //Debug.LogWarning($"Could not find {itemid.ToString()} {slotid}");
                    data = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item);
                }
            }
            return data;
        }




        static public Dictionary<string, string> saveRando()
        {
            return __itemData;
        }
        static public void loadRando(Dictionary<string, string> data)
        {
            __itemData = data;
        }
        static public void deloadRando()
        {
            __itemData.Clear();
            LocationTracker.clearItemList();
        }

        static public bool checkItemGot(ItemList.Type item, byte slot) //not working?
        {
            return LocationTracker.hasItem(item,slot); 
        }

        static public bool checkRandomizedItemGot(ItemList.Type item, byte slot)
        {
            return checkItemGot(item, slot);
        }

        static public Sprite getSprite(int itemID, bool custom = false)
        {
            return CommonResource.Instance.GetItem(itemID);
        }

        [HarmonyPatch(typeof(WorldManager), "FindNearestItem_Room")]
        [HarmonyPostfix]
        static void findNearestRandomizedItem(ref ItemTile tile, ref ItemList.Type nearestType)
        {
            if (tile == null) return;
            nearestType = getRandomizedItem(tile.itemid, tile.GetSlotID());
        }

        // change how the item Bell Works
        [HarmonyPatch(typeof(CharacterBase), "UseItem")]
        [HarmonyPrefix]
        static bool WarpBell(ref ItemList.Type item, ref bool playvoice, ref ObjectPhy ___phy_perfer, ref playerController ___playerc_perfer, ref CharacterBase __instance)
        {
            if (item == ItemList.Type.Useable_Bell)
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

        [HarmonyPatch(typeof(SettingManager), nameof(SettingManager.SetAchievement))]
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
            ItemList.Type data = getRandomizedItem(__instance.itemid, ___slotid);
            var spr = CommonResource.Instance.GetItem((int)data);

            if (data >= ItemList.Type.BADGE_START && data <= ItemList.Type.BADGE_MAX)
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
                    secondTile.transform.localPosition = new Vector3(0, 0, 0);
                    secondTile.transform.localScale = new Vector3(1, 1, 1);
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


            if (checkRandomizedItemGot(__instance.itemid, ___slotid)) {
                Debug.Log($"[ItemTile] {data} # visible in camera. Removed from map because player already obtained it.");

                __instance.DisableMe();
                return false;
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
            if (SaveManager.Instance.GetOrb() < 3)
                SaveManager.Instance.SetOrb((byte)(SaveManager.Instance.GetOrb() + amount));

            if (SaveManager.Instance.GetOrb() >= 3)
            {
                if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_BoostSystem) > 0)
                {
                    SaveManager.Instance.SetOrb(4);
                }
                SaveManager.Instance.FirstTimeEnableOrbColors();
            }
        }

         //Orb!
        [HarmonyPatch(typeof(SaveManager), "FirstTimeEnableOrbColors")]
        [HarmonyPrefix]
        static bool disableOrbOverride(SaveManager __instance)
        {
            if (__instance.GetMiniFlag(Mini.OrbStatus) >= 3) return true;
            return false;
        }

        //
        [HarmonyPatch(typeof(SaveManager),"GetMemineCleared")]
        [HarmonyPrefix]
        static bool GetMemineCleared(ref int __result)
        {
            int num = 0;
            for (int i = 0; i <= 5; i++)
            {
                if (checkRandomizedItemGot(ItemList.Type.STACKABLE_SHARD, (byte)i))
                {
                    num++;
                }
            }
            return false;
        }

        //Replace I10 and I11 with new Sprites

        static Sprite createNewArchipelagoSprite(string file)
        {
            string path = pluginPath + "/resource/Archipelago/" + file;
            Texture2D texture = new Texture2D(2,2);
            if (!File.Exists(path)) { return null; }
            byte[] imageAsset = System.IO.File.ReadAllBytes(path);
            ImageConversion.LoadImage(texture, imageAsset);
            Sprite sprite = Sprite.Create(texture,new Rect(0,0,28,28),new Vector2(0.5f,0.5f),28);
            return sprite;
        }

        [HarmonyPatch(typeof(CommonResource), "Awake")]
        [HarmonyPostfix]
        static void replaceSprite(ref Sprite[] ___items,ref Sprite[] ___questitems)
        {
            if (___items.Length > 0)
            {

                ___items[10] = createNewArchipelagoSprite("nonProgression.png");
                ___items[11] = createNewArchipelagoSprite("Progression.png");
                if (___items[10] == null)
                    ___items[10] = ___questitems[9];
                if (___items[11] == null)
                    ___items[11]= ___questitems[9];
            }


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
            if (EventManager.Instance.EventStage == 11)
            {
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
            else if (em.EventStage == 180)
            {
                if (em.EventTime >= 1f)
                {
                    MusicManager.Instance.PlayMusic(Music.MUTE);
                    em.SetStage(190);
                    em.StopEvent();
                    MusicManager.Instance.RabiEasterEggBGM = true;
                    SettingManager.Instance.SetAchievement(Achievements.ACHI_EASTEREGG_RABIRIBI);
                    SaveManager.Instance.AutoSave(forced: true);
                }
            }
        }

    }

    class OrbPatch
    {
        [HarmonyPatch(typeof(HUDPosition), "Update")]
        [HarmonyPrefix]
        static bool gibHUD(ref UIType.HUD ___Type, ref RectTransform ___rt, ref float ___startX, ref float ___startY, ref float ___targetX)
        {
            if (!___rt) return false;

            if (___Type == UIType.HUD.MAIN)
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
        [HarmonyPatch(typeof(EventManager), "ReloadOrbStatus")]
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
        static bool newPrepareSwitchOrb(ref Character.OrbType ot, ref bool forceType, ref CharacterBase ___cb_perfer, ref CharacterPhy __instance)
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

        [HarmonyPatch(typeof(CharacterPhy), "UseBoost")]
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
        [HarmonyPatch(typeof(CharacterPhy), "UseBoost")]
        [HarmonyPrefix]
        static bool test(ref CharacterPhy __instance, ref bool __result)
        {
            if (SaveManager.Instance.GetItem(ItemList.Type.I19) < 1 && SaveManager.Instance.GetItem(ItemList.Type.I20) < 1)
            {
                __result = true; return false;
            }


            return true;
        }

        [HarmonyPatch(typeof(Cyril), "ALWAYS")]
        [HarmonyPrefix]
        static void removeFrocedBoost(ref float ___useboost)
        {
            if (___useboost > 0f)
            {
                ___useboost = 0f;
            }
        }

        [HarmonyPatch(typeof(CharacterPhy), "SwitchTypeForced")]
        [HarmonyPrefix]
        static bool forcedTypeSwitch(ref CharacterPhy __instance, ref bool __result)                 // SwitchType MODE AABBCC Not Working
        {
            if ((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
            {
                __instance.PrepareSwitchOrb();
                __result = false;
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(CharacterPhy), "SwitchType")]
        [HarmonyPrefix]
        static bool typeSwitch(ref CharacterPhy __instance, ref bool __result)
        {
            if ((__instance.orbUsing == Character.OrbType.BLACK && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0) || (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I19) == 0))
            {
                __result = false;
                return false;
            }
            return true;
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
        static CharacterBase lastHit;
        [HarmonyPatch(typeof(SaveManager), "TryRenewLevel")]
        [HarmonyPostfix]
        static void resetBonusDmg()
        {
            bonusDropKickDmg = 0;
        }


        [HarmonyPatch(typeof(CharacterBase), "BulletHurtPlayer")]
        [HarmonyPostfix]
        static void HurtCheck(ref CharacterBase owner, float damage, BulletType type, ref CharacterBase __instance, ref bool __result)
        {
            //Debug.Log($"{type} {damage} {owner}");
            if (__result)
            {
                if (type == BulletType.QUICK_DROP)
                {
                    bonusDropKickDmg++;

                }
                else if (damage > 0 && (owner != null && owner.isPlayer()) || __instance.isPlayer())
                {
                    if (type == BulletType.TEVI_WEAK_DASH || type == BulletType.TEVI_WEAK_ATTACK || type == BulletType.ORB_SHOT_NORMAL || type == BulletType.SUMMONBUNNY_HIT) return;
                    Debug.Log("Combo was broken by " + type);
                    bonusDropKickDmg = 0;
                }
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
        [HarmonyPatch(typeof(playerController), "_Update")]
        [HarmonyPostfix]
        static void updateDropkickDamage(ref PlayerLogicState ___logicStatus, ref ObjectPhy ___phy_perfer, ref CharacterPhy ___cphy_perfer, ref CharacterBase __instance)
        {
            if (___logicStatus == PlayerLogicState.QUICKDROP && !DropKickDmgUpdated && currentDropKick != null && __instance.isPlayer())
            {
                DropKickDmgUpdated = true;
                float num3 = 0.343525f;
                num3 += testValue * bonusDropKickDmg;
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

        [HarmonyPatch(typeof(GemaUIChangeDifficulty), "Update")]
        [HarmonyPrefix]
        static bool changeCustomDiff(ref GemaUIChangeDifficulty __instance, ref bool ___isEnable, ref TextMeshProUGUI ___promotetext, ref int ___difficulty)
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
        [HarmonyPatch(typeof(Chap1ChangeDifficulty), "EVENT")]
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
        [HarmonyPatch(typeof(ObjectPhy), "WallHit")]
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
        static Dictionary<string, string> newLocalize = new Dictionary<string, string>();

        [HarmonyPatch(typeof(Localize), "GetLocalizeTextWithKeyword")]
        [HarmonyPrefix]
        static bool getNewLocalize(ref string keyword, ref string __result)
        {
            if (newLocalize.ContainsKey(keyword))
            {
                __result = newLocalize[keyword];
                return false;
            }
            return true;
        }
    }
}