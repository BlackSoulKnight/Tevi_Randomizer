using BepInEx;
using Bullet;
using Character;
using EventMode;
using Game;
using HarmonyLib;
using Map;
using Newtonsoft.Json;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TeviRandomizer.TeviRandomizerSettings;



namespace TeviRandomizer
{


    [BepInPlugin("tevi.plugins.randomizer", "Randomizer", MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("TEVI.exe")]
    public class RandomizerPlugin : BaseUnityPlugin
    {

        public const ItemList.Type PortalItem = ItemList.Type.I13;
        public const ItemList.Type MoneyItem = ItemList.Type.I14;
        public const ItemList.Type CoreUpgradeItem = ItemList.Type.I15;
        public const ItemList.Type ItemUpgradeItem = ItemList.Type.I16;

        public enum EventID
        {
            IllusionPalace = 9999
        }

        RandomizerPlugin()
        {
            TeviSettings.pluginPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        static public Dictionary<string, string> __itemData = new();

        static public List<int> transitionVisited = new List<int>();
        static public string seed;
        static public Dictionary<int, int> transitionData;
        static public List<int> UniqueEnemiesKilled = new List<int>();
        static public Randomizer randomizer;
        static private bool randomizerEnabled = false;
        static private Harmony harmonyPatchInstance = new Harmony("Randomizer");

        public IEnumerator screenshot(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            yield return new WaitForEndOfFrame();
            Traverse t = Traverse.Create(GemaSuperSample.Instance);
            var h = t.Field<RenderTexture>("rt").Value.height;
            var w = t.Field<RenderTexture>("rt").Value.width;
            Texture2D s = new Texture2D(w, h, UnityEngine.TextureFormat.ARGB32, false);
            Rect r = new Rect(0, 0, w, h);
            s.ReadPixels(r, 0, 0);
            s.Apply();
            byte[] b = s.EncodeToPNG();
            Debug.Log(b.Length);
            System.IO.File.WriteAllBytes(UnityEngine.Application.dataPath + "/test.png", b);
        }
        public void takeScreenshot(int seconds)
        {
            
            StartCoroutine(screenshot(seconds));
        }

        private void Awake()
        {
            this.gameObject.AddComponent<ArchipelagoInterface>();
            this.gameObject.AddComponent<ItemDistributionSystem>();
            ArchipelagoInterface.Instance = gameObject.GetComponent<ArchipelagoInterface>();
            Localize.GetLocalizeTextWithKeyword("", false);
            randomizer = new();

            harmonyPatchInstance.PatchAll(typeof(UI.UI));
            toggleRandomizerPlugin();


            Logger.LogInfo($"Plugin Randomizer is loaded!");

        }

        [Command("sendLocation", QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private void SendLocation(string location)
        {
            if (!ArchipelagoInterface.Instance.isConnected)
            {
                Debug.Log("Not Connected to a Archipelago Server");
                return;
            }
            if(ArchipelagoInterface.Instance.checkoutLocation(location))
                ArchipelagoInterface.Instance.sendMessage($"[Debug] Send Location {location} via Console");

        }

        static public bool toggleRandomizerPlugin()
        {
            if (randomizerEnabled)
            {
                harmonyPatchInstance.UnpatchSelf();
                harmonyPatchInstance.PatchAll(typeof(UI.UI));
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
                harmonyPatchInstance.PatchAll(typeof(ScalePatch));
                harmonyPatchInstance.PatchAll(typeof(OrbPatch));
                harmonyPatchInstance.PatchAll(typeof(RabiSmashPatch));
                harmonyPatchInstance.PatchAll(typeof(BonusFeaturePatch));
                harmonyPatchInstance.PatchAll(typeof(ChatSystemPatch));
                harmonyPatchInstance.PatchAll(typeof(CustomMap));
                //harmonyPatchInstance.PatchAll(typeof(ResourcePatch));
                harmonyPatchInstance.PatchAll(typeof(EnemyPatch));
                harmonyPatchInstance.PatchAll(typeof(BossPatch));
                harmonyPatchInstance.PatchAll(typeof(Story_Mode.StoryEventPatch));
                harmonyPatchInstance.PatchAll(typeof(BaseGameFixes));
                harmonyPatchInstance.PatchAll(typeof(PlayerCharacterPatch));
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

        static public void changeSystemText(string keyword, string text)
        {
            var t = new Traverse(typeof(Localize));
            var library = t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>();
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
                Localize.SystemText newText = createNewText("ITEMDESC.I19", "Celia is now available.");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMNAME.I20", "Sable");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I20", "Sable is now available.");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMNAME.I14", "500 Zennie Pack");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I14", "You found 500 Zennies.");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMNAME.I15", "Magitite Shard");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I15", "You found a Magitite Shard.");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMNAME.I16", "Mananite Shard");
                t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Add(newText);
                t.Field("jsonlistSysTxtDictionary").GetValue<Dictionary<string, Localize.SystemText>>().Add(newText.keyword, t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>()[t.Field("jsonlistSysTxt").GetValue<List<Localize.SystemText>>().Count - 1]);
            }
            {
                Localize.SystemText newText = createNewText("ITEMDESC.I16", "You found a Mananite Shard.");
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
        static void changeText(ref string __result, ref string keyword)
        {
            if (keyword == "Todo.GoalTipFreeRoam")
            {
                switch (TeviSettings.goalType) {
                    case GoalType.BossDefeat:
                        __result = __result.Replace("16 Astral Gears", "21 Boss Kills");
                        break;
                    case GoalType.AstralGear:
                    default:
                        __result = __result.Replace("16", TeviSettings.GoMode.ToString());
                        break;
            }
            }
            if (ArchipelagoInterface.Instance != null && ArchipelagoInterface.Instance.isConnected)
            {

            }
        }


        static public void createSeed()
        {
            System.Random rando;
            if (seed == "")
            {
                seed = new System.Random().Next(int.MaxValue).ToString();
            }
            rando = new System.Random(seed.GetHashCode());
            randomizer = new();
            randomizer.CreateSeed(rando);
            //randomizer.createSeed(rando);
        }


        static public ItemList.Type getRandomizedItem(ItemList.Type itemid, byte slotid) => getRandomizedItem(itemid.ToString(), slotid);

        static public ItemList.Type getRandomizedResource(ItemList.Resource item, byte area, int blockID)
        {

            ItemList.Type data;
            if (ArchipelagoInterface.Instance.isConnected && LocationTracker.APResoucreLocationame.ContainsKey($"{area} #{blockID}"))
            {
                if(!ArchipelagoInterface.Instance.isItemNative(LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]))
                    data = ArchipelagoInterface.Instance.isItemProgessive(LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]) ? ArchipelagoInterface.remoteItemProgressive : ArchipelagoInterface.remoteItem;
                else
                // Try to Parse string into Item Type, if failed check for teleporter in string else give item back
                if (!Enum.TryParse(ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]),out data))
                    {
                    //Debug.LogWarning($"Could not find {itemid.ToString()} {slotid}");
                    if (ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]).Contains("Teleporter"))
                        data = PortalItem;
                    else
                        data = (ItemList.Type)((int)item + (int)ItemList.Type.I14);
                    }
            }
            else
            {
                try
                {
                    data = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), __itemData[LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]]);
                }
                catch  (Exception e)
                {
                    Debug.LogWarning($"{e}");
                    if (LocationTracker.APResoucreLocationame.ContainsKey($"{area} #{blockID}") && __itemData.ContainsKey(LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]) && __itemData[LocationTracker.APResoucreLocationame[$"{area} #{blockID}"]].Contains("Teleporter"))
                        data = PortalItem;
                    else
                        data = (ItemList.Type)((int)item + (int)ItemList.Type.I14);
                }
            }
            return data;
        }
        static public ItemList.Type getRandomizedItem(string item, byte slot)
        {

            ItemList.Type data;
            if (ArchipelagoInterface.Instance.isConnected && LocationTracker.APLocationName.ContainsKey($"{item} #{slot}"))
            {
                if(!ArchipelagoInterface.Instance.isItemNative(LocationTracker.APLocationName[$"{item} #{slot}"]) || item == "Remote")
                    data = ArchipelagoInterface.Instance.isItemProgessive(LocationTracker.APLocationName[$"{item} #{slot}"]) ? ArchipelagoInterface.remoteItemProgressive : ArchipelagoInterface.remoteItem;
                else
                    if (!Enum.TryParse(ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APLocationName[$"{item} #{slot}"]),out data))
                    {
                    //Debug.LogWarning($"Could not find {itemid.ToString()} {slotid}");
                    if (ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APLocationName[$"{item} #{slot}"]).Contains("Teleporter"))
                        data = PortalItem;
                    else
                        data = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), item);
                    }
            }
            else
            {
                try
                {
                    data = (ItemList.Type)Enum.Parse(typeof(ItemList.Type), __itemData[LocationTracker.APLocationName[$"{item} #{slot}"]]);
                }
                catch
                {
                    //Debug.LogWarning($"Could not find {itemid.ToString()} {slotid}");
                    if (LocationTracker.APLocationName.ContainsKey($"{item} #{slot}") && __itemData.ContainsKey(LocationTracker.APLocationName[$"{item} #{slot}"]) && __itemData[LocationTracker.APLocationName[$"{item} #{slot}"]].Contains("Teleporter"))
                        data = PortalItem;
                    else
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
            transitionVisited.Clear();
            LocationTracker.clearItemList();
            UniqueEnemiesKilled.Clear();
            ItemDistributionSystem.reset();
            transitionData = null;
            ResourcePatch.AreaResource = null;
            ResourcePatch.resources = new GameObject[0];
            ArchipelagoInterface.Instance.currentItemNR = 0;
        }

        static public bool checkItemGot(ItemList.Type item, byte slot) //not working?
        {
            return LocationTracker.hasItem(item, slot);
        }

        static public bool checkRandomizedItemGot(ItemList.Type item, byte slot)
        {
            return checkItemGot(item, slot);
        }

        static public Sprite getSprite(int itemID, bool custom = false)
        {
            return CommonResource.Instance.GetItem(itemID);
        }

        public static BulletType[] bulletSwap = new BulletType[(int)BulletType.MAX];
        [HarmonyPatch(typeof(BulletManager),"ShootBullet")]
        [HarmonyPrefix]
        static void randomBullet(ref BulletType type)
        {
            //Debug.Log($"original: {type.ToString()}");
           //type = bulletSwap[(int)type];
            //Debug.Log($"after: {type.ToString()}");

        }


        [HarmonyPatch(typeof(WorldManager), "FindNearestItem_Room")]
        [HarmonyPostfix]
        static void findNearestRandomizedItem(ref ItemTile tile, ref ItemList.Type nearestType)
        {
            if (tile == null) return;
            nearestType = getRandomizedItem(tile.itemid, tile.GetSlotID());
        }

        [HarmonyPatch(typeof(GameSystem), "GameOver")]
        [HarmonyPostfix]
        static void APDeathlinkg()
        {
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.triggerDeathLink();
            }
        }

        //Unlock all custome modes
        [HarmonyPatch(typeof(SettingManager), "GetAchievementUnlockedCount")]
        [HarmonyPostfix]
        static void removeCounter(ref int __result)
        {
            __result = 9999999;
        }

        //Increase Memine Race Timer
        [HarmonyPatch(typeof(GemaMissionMode),"StartMission")]
        [HarmonyPostfix]
        static void increaseTimer(ref float ___timer,ref byte ___maxHitChance)
        {
            ___timer = 3600;
            ___maxHitChance = 0;
        }

        // change how the item Bell Works
        [HarmonyPatch(typeof(CharacterBase), "UseItem")]
        [HarmonyPrefix]
        static bool WarpBell(ref ItemList.Type item, ref bool playvoice, ref ObjectPhy ___phy_perfer, ref playerController ___playerc_perfer, ref CharacterBase __instance, ref int ___health)
        {
            if (item == ItemList.Type.Useable_Bell)
            {
                if (!EventManager.Instance.isBossMode() && EventManager.Instance.getMode() == Mode.OFF && (EventManager.Instance.getSubMode() == Mode.OFF || EventManager.Instance.getSubMode() == Mode.Chap7WarpToQueensGarden))
                {
                    if (WorldManager.Instance.Area == 30 || (EventManager.Instance.GetCurrentEventBattle() != Mode.Chap7StartRibauldChase && EventManager.Instance.GetCurrentEventBattle() != 0) || EventManager.Instance.isBossMode())
                    {
                        __instance.PlaySound(AllSound.SEList.MENUFAIL);
                        __instance.ChangeLogicStatus(Character.PlayerLogicState.NORMAL);
                        EventManager.Instance.EFF_CreateEmotion(__instance, null, __instance.t.position, EffectSprite.EMOTION_QUESTION);
                        return false;
                    }


                    int num5 = (int)___phy_perfer.GetCounter(4);
                    if(GemaMissionMode.Instance.isInMission() && !TeviSettings.customFlags[CustomFlags.TeleporterRando])
                        EventManager.Instance.StartWarp(1, 1, 1, 1);
                    else
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
            if (item == ItemList.Type.Useable_Bookmark)
            {
                if (!EventManager.Instance.isBossMode() && EventManager.Instance.getMode() == Mode.OFF && (EventManager.Instance.getSubMode() == Mode.OFF || EventManager.Instance.getSubMode() == Mode.Chap7WarpToQueensGarden))
                {
                    if (GemaMissionMode.Instance.isInMission() || WorldManager.Instance.Area == 30 || (EventManager.Instance.GetCurrentEventBattle() != Mode.Chap7StartRibauldChase && EventManager.Instance.GetCurrentEventBattle() != 0) || EventManager.Instance.isBossMode())
                    {
                        __instance.PlaySound(AllSound.SEList.MENUFAIL);
                        __instance.ChangeLogicStatus(Character.PlayerLogicState.NORMAL);
                        EventManager.Instance.EFF_CreateEmotion(__instance, null, __instance.t.position, EffectSprite.EMOTION_QUESTION);
                        return false;
                    }


                    int num5 = (int)___phy_perfer.GetCounter(4);

                    if (SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) == 0)
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 1);
                        __instance.GiveBookMarkBuff();
                    }

                    if(___health > 0) {
                        GameObject pooledObject2 = GemaPoolManager.Instance.CommonEffectsPooler.GetPooledObject(42);
                        pooledObject2.transform.position = __instance.t.position;
                        pooledObject2.transform.localScale = new Vector3(180f, 180f, 180f);
                        pooledObject2.SetActive(value: true);
                        CameraScript.Instance.PlaySound(AllSound.SEList.Collect_Heal, 1f);
                    }
                    Debug.Log("[CharacterBase] " + __instance.type.ToString() + " used item " + item.ToString() + " Slot : " + num5 + " Heal : " + 0);

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

            if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
            {
                if (EventManager.Instance.GetElm(__instance.transform, 0f, MainVar.instance.TILESIZE) == ElementType.FreeRoamOnly)
                {
                    __instance.DisableMe();
                    Debug.Log($"DISABLE {__instance.itemid.ToString()} because Not Story mode");
                    return false;
                }
            }
            else if (EventManager.Instance.GetElm(__instance.transform, 0f, MainVar.instance.TILESIZE) == ElementType.NotFreeRoamOnly)
            {
                __instance.DisableMe();
                Debug.Log($"DISABLE {__instance.itemid.ToString()} because Not free roam");
                return false;
            }
            if (__instance.itemid == ItemList.Type.ITEM_RapidShots) { ___slotid = 29; }

            ItemList.Type data = getRandomizedItem(__instance.itemid, ___slotid);

            var spr = CommonResource.Instance.GetItem((int)data);

            if (data == ArchipelagoInterface.remoteItem || data == ArchipelagoInterface.remoteItemProgressive)
            {
                if (ArchipelagoInterface.Instance.isConnected)
                {
                    string itemName = ArchipelagoInterface.Instance.getLocItemName(LocationTracker.APLocationName[$"{__instance.itemid} #{___slotid}"]);
                    ItemList.Type item;
                    if (Enum.TryParse(itemName, out item))
                    {
                        spr = CommonResource.Instance.GetItem((int)item);
                    }
                }
            }

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


            if (checkRandomizedItemGot(__instance.itemid, ___slotid))
            {
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
                //SaveManager.Instance.SetOrb((byte)(SaveManager.Instance.GetOrb() + amount));
                SaveManager.Instance.SetOrb(3);

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
        [HarmonyPatch(typeof(SaveManager), "GetMemineCleared")]
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
            string path = TeviSettings.pluginPath + "/resource/Archipelago/" + file;
            Texture2D texture = new Texture2D(2, 2);
            if (!File.Exists(path)) { return null; }
            byte[] imageAsset = System.IO.File.ReadAllBytes(path);
            ImageConversion.LoadImage(texture, imageAsset);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 28, 28), new Vector2(0.5f, 0.5f), 28);
            return sprite;
        }
        static Sprite createNewSprite(string file)
        {
            string path = TeviSettings.pluginPath + "/resource/" + file;
            Texture2D texture = new Texture2D(2, 2);
            if (!File.Exists(path)) { return null; }
            byte[] imageAsset = System.IO.File.ReadAllBytes(path);
            ImageConversion.LoadImage(texture, imageAsset);
            if(texture.height != 28 || texture.width != 28)
            {
                return null;
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 28, 28), new Vector2(0.5f, 0.5f), 28);
            return sprite;
        }

        [HarmonyPatch(typeof(CommonResource), "Awake")]
        [HarmonyPostfix]
        static void replaceSprite(ref Sprite[] ___items, ref Sprite[] ___questitems,ref Sprite[] ___resources)
        {
            if (___items.Length > 0)
            {

                ___items[10] = createNewArchipelagoSprite("nonProgression.png");
                ___items[11] = createNewArchipelagoSprite("Progression.png");
                ___items[13] = createNewSprite("Teleporter.png");
                if (___items[13] == null)
                    ___items[13] = ___items[0];
                if (___items[10] == null)
                    ___items[10] = ___questitems[9];
                if (___items[11] == null)
                    ___items[11] = ___questitems[9];
                ___items[14] = ___resources[0];
                ___items[15] = ___resources[1];
                ___items[16] = ___resources[2];
            }


        }
        [HarmonyPatch(typeof(enemyController), "FreeRoamEnemyBoost")]
        [HarmonyPrefix]
        static bool FreeRoamEnemyBoost(ref int ___health, ref int ___atk, ref CharacterBase ___type)
        {
            if (WorldManager.Instance.CurrentRoomArea == AreaType.FINALPALACE2 || WorldManager.Instance.CurrentRoomArea == AreaType.FINALPALACE || WorldManager.Instance.CurrentRoomArea == AreaType.ILLUSIONPALACE)
            {
                int stackableCount = SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_COG);
                if (stackableCount < 16)
                {
                    stackableCount = 16;
                }
                float num = 1f + 0.085f * (float)(24 - stackableCount);
                if (num > 1f)
                {
                    ___health = (int)((float)___health * num);
                    num = 1f + 0.1f * (float)(24 - stackableCount);
                    ___atk = (int)((float)___atk * num);
                    Debug.Log("[BossHealth] Free Roam Enemy Boost COG (" + num + ") : " + "MEH" + "'s health set to " + ___health + ", ATK set to " + ___atk);
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid),"OnEnable")]
        [HarmonyPrefix]
        static void changeToCraftingMap()
        {
            ArchipelagoInterface.Instance.updateCurretMap(30);
        }

        [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid),"OnDisable")]
        [HarmonyPrefix]
        static void returnFromCraftingMap()
        {
            ArchipelagoInterface.Instance.updateCurretMap(WorldManager.Instance.Area);
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
                //return false;
            }
            else if (__instance.orbUsing == Character.OrbType.WHITE && SaveManager.Instance.GetItem(ItemList.Type.I20) == 0)
            {
                //return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterPhy), "UseBoost")]
        [HarmonyPostfix]
        static void switchOrbBeforeSummon(ref CharacterPhy __instance, ref bool __result)
        {
            if (__result)
            {
                if ((SaveManager.Instance.GetItem(ItemList.Type.I19) < 1 && __instance.orbUsing == OrbType.BLACK) ||  (SaveManager.Instance.GetItem(ItemList.Type.I20) < 1) && __instance.orbUsing == OrbType.WHITE)
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


        [HarmonyPatch(typeof(WorldManager), "StartFadeFrontLayer")]
        [HarmonyPrefix]
        static void fade1(ref float target)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])
                target = 0;
        }
        [HarmonyPatch(typeof(WorldManager), "SetFrontLayer")]
        [HarmonyPrefix]
        static void fade2(ref float target)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])

                target = 0;
        }
        [HarmonyPatch(typeof(WorldManager), "Awake")]
        [HarmonyPostfix]
        static void fade0(ref float ___FrontFadeTarget)
        {
            if (TeviSettings.customFlags[CustomFlags.RevealPaths])
                ___FrontFadeTarget = 0;
        }

        //test Features
        //Throw Clusterbomb without Crossbombs
        [HarmonyPatch(typeof(ObjectPhy), "UseBomb")]
        [HarmonyPostfix]
        static void useAreaBomb(ref ObjectPhy __instance, ref CharacterBase ___cb_perfer,ref bool __result)
        {
            if (SaveManager.Instance.CanUseItem(ItemList.Type.ITEM_AREABOMB) > 0 && SaveManager.Instance.CanUseItem(ItemList.Type.ITEM_LINEBOMB) == 0)
            {

                ___cb_perfer.playerc_perfer.meter_bomb.EnableMe(0f);
                __instance.SetCounter(0, 1f);
                __instance.SetCounter(1, 0.48f);
                __instance.SetCounter(5, 0f);
                __instance.SetCounter(18, 0f);
                ___cb_perfer.spranim_prefer.ToggleOther(0);
                ___cb_perfer.SetHitboxStarted(on: false);
                ___cb_perfer.ChangeLogicStatus(PlayerLogicState.TEVI_GROUND_ITEM);
                __result = true;
            }
        }

        //Use Sable and Celia without chargeshots
        [HarmonyPatch(typeof(CharacterPhy), "GetRangedControls")]
        [HarmonyPostfix]
        static void disableChargeShots(ref bool ___mustNormal,ref CharacterPhy __instance)
        {
            
            if(SaveManager.Instance.GetItem(ItemList.Type.ITEM_ORB) < (TeviSettings.customFlags[CustomFlags.CebleStart] ? 1:2))
                ___mustNormal = true;

        }

        [HarmonyPatch(typeof(OrbBall),"NormalShot")]
        [HarmonyPostfix]
        static void reduceChargeHeld(ref bool __result,ref CharacterPhy ___owner_phy)
        {
            if (__result)
            {
                float num10 = 9f;
                if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_NormalShotReducerA))
                {
                    num10 -= 3.15f;
                }
                if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_NormalShotReducerB))
                {
                    num10 -= 4.05f;
                }
                if (___owner_phy.charge - num10 <= 0 && ___owner_phy.chargeheld > 0)
                {
                    ___owner_phy.charge += 100;
                    ___owner_phy.chargeheld--;
                }
            }
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
                    //Debug.Log("Combo was broken by " + type);
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
        [HarmonyPatch(typeof(MusicManager), "PlayMusic")]
        [HarmonyPrefix]
        static void changeMusic(ref Music musicname, ref Music __state)
        {
            if (TeviSettings.customFlags[CustomFlags.RandomizedMusic])
            {
                if (Extras.RandomizeExtra.randomizedMusic[(byte)musicname] == 0) {
                    __state = Music.OFF;
                    return;
                        }

                __state = musicname;
                if (musicname == Music.LOOP) { return; }
                musicname = (Music)Extras.RandomizeExtra.randomizedMusic[(byte)musicname];
            }
        }
        [HarmonyPatch(typeof(MusicManager), "PlayMusic")]
        [HarmonyPostfix]
        static void saveLastMusic(ref Music ___lastMusic, ref Music __state, ref Music ___readyMusic)
        {

            if (TeviSettings.customFlags[CustomFlags.RandomizedMusic])
            {

                if (__state == Music.LOOP || __state == Music.OFF) { return; }
                ___lastMusic = __state;
                ___readyMusic = Music.OFF;
            }
        }
    }

    


    class ScalePatch()
    {

        [HarmonyPatch(typeof(enemyController), "SetMaxBossHealth")]
        [HarmonyPrefix]
        static bool setMaxBossHealth(ref enemyController __instance)
        {
            int customAttack = TeviSettings.customAtkDiff;
            int customHP = TeviSettings.customHpDiff;
            if (TeviSettings.customAtkDiff < 0)
                customAttack = (int)(SaveManager.Instance.GetDifficultyName());
            if (TeviSettings.customHpDiff < 0)
                customHP = (int)(SaveManager.Instance.GetDifficultyName());

            if (TeviSettings.customHpDiff < 0)
                customHP = (int)(SaveManager.Instance.GetDifficultyName());
            int num = customAttack - 5;
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
            int customAttack = TeviSettings.customAtkDiff;
            int customHP = TeviSettings.customHpDiff;
            if (TeviSettings.customAtkDiff < 0)
                customAttack = (int)(SaveManager.Instance.GetDifficultyName());
            if (TeviSettings.customHpDiff < 0)
                customHP = (int)(SaveManager.Instance.GetDifficultyName());

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
                        if (customAttack >= (short)Difficulty.D5)
                        {
                            num = 0.008f;
                        }
                        atk = (int)((float)atk * (1f + num * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP) + 0.001f * (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD)));
                        if (customAttack >= (short)Difficulty.D4)
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
                float num3 = customAttack - 5;
                if (customAttack == (short)Difficulty.D6)
                {
                    num3 = 2f;
                }
                else if (customAttack == (short)Difficulty.D7)
                {
                    num3 = 3f;
                }
                if (num3 < 0f)
                {
                    float num4 = 1f + num3 * 0.1f;
                    if (customAttack <= (short)Difficulty.D0)
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
                        //potion scaling
                        //float num5 = 1f + 0.008f * ((float)SaveManager.Instance.GetMainLevel() + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD) / 2f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK));
                        float num5 = 1f + 0.00f * (float)SaveManager.Instance.GetMainLevel();

                        if (SaveManager.Instance.GetCustomGame(CustomGame.HighBossScale) || GemaBossRushMode.Instance.isBossRushTypeEqualOrHigher(BossRushType.XTREME))
                        {
                            num5 += 0.000225f * (float)SaveManager.Instance.GetUsedCost(GetRemain: false);
                        }
                        else
                        {
                            float num6 = 0.005f;
                            if (customHP >= (short)Difficulty.D6)
                            {
                                num6 = 0.006f;
                            }
                            if (customHP >= (short)Difficulty.D7)
                            {
                                num6 = 0.007f;
                            }
                            if (customHP >= (short)Difficulty.D9)
                            {
                                num6 = 0.008f;
                            }
                            //num5 = 1f + num6 * ((float)SaveManager.Instance.GetMainLevel() + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_SHARD) / 4f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_MATK) / 3.5f + (float)(int)SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_RATK) / 3.5f);
                            num5 = 1f + num6 * (float)SaveManager.Instance.GetMainLevel();
                            if (customHP >= (short)Difficulty.D6 && SaveManager.Instance.GetChapter() >= 4)
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
                            if (customAttack >= (short)Difficulty.D5 && num7 > 8)
                            {
                                num7 = 8;
                            }
                            if (customAttack >= (short)Difficulty.D7 && num7 > 2)
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
                                if (customAttack <= (short)Difficulty.D7)
                                {
                                    float num8 = 1f - (float)num7 * 0.025f;
                                    if (customAttack >= (short)Difficulty.D5)
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
                                    if (customAttack >= (short)Difficulty.D7 && num8 < 0.85f)
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
                    num3 = (float)(customHP - 5);
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
                        if (customHP <= (short)Difficulty.D6)
                        {
                            health = (int)((float)health * 0.925f);
                        }
                        if (customHP <= (short)Difficulty.D5)
                        {
                            health = (int)((float)health * 0.925f);
                        }
                    }
                    if (customAttack >= (short)Difficulty.D5)
                    {
                        atk = (int)((float)atk * 1.025f);
                    }
                    if (customAttack >= (short)Difficulty.D10)
                    {
                        atk = (int)((float)atk * 1.125f);
                    }
                }

                __instance.maxhealth = health;
                __instance.health = health;
                __instance.atk = atk;

                if (customAttack <= (short)Difficulty.D0)
                {
                    atk = (int)((float)atk * 0.4f);
                }
                else if (customAttack <= (short)Difficulty.D1)
                {
                    atk = (int)((float)atk * 0.8f);
                }
                if (atk > 0)
                {
                    atk += (int)((float)customAttack / 2f);
                    if (customAttack >= (short)Difficulty.D10)
                    {
                        atk += SaveManager.Instance.GetChapter() / 2;
                    }
                    else if (customAttack <= (short)Difficulty.D0)
                    {
                        atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 3f);
                    }
                    else if (customAttack <= (short)Difficulty.D1)
                    {
                        atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 2f);
                    }
                    else if (customAttack <= (short)Difficulty.D3)
                    {
                        atk -= (int)((float)(int)SaveManager.Instance.GetChapter() * 1.5f);
                    }
                    else if (customAttack <= (short)Difficulty.D5)
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
                    health = __instance.health;
                    atk = __instance.atk;

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
            int customAttack = TeviSettings.customAtkDiff;
            int customHP = TeviSettings.customHpDiff;
            if (TeviSettings.customAtkDiff < 0)
                customAttack = (int)(SaveManager.Instance.GetDifficultyName());
            if (TeviSettings.customHpDiff < 0)
                customHP = (int)(SaveManager.Instance.GetDifficultyName());

            if (GemaBossRushMode.Instance.isBossRush() || (__instance.type != Character.Type.Barados && __instance.type != Character.Type.Caprice && __instance.type != Character.Type.Katu && __instance.type != Character.Type.Thetis && __instance.type != Character.Type.Roleo))
            {
                return false;
            }
            if (__instance.type == Character.Type.Katu)
            {
                if (customHP >= (short)Difficulty.D7)
                {
                    __instance.health += 125;
                }
                if (customHP >= (short)Difficulty.D10)
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
            int customAttack = TeviSettings.customAtkDiff;
            int customHP = TeviSettings.customHpDiff;
            if (TeviSettings.customAtkDiff < 0)
                customAttack = (int)(SaveManager.Instance.GetDifficultyName());
            if (TeviSettings.customHpDiff < 0)
                customHP = (int)(SaveManager.Instance.GetDifficultyName());
            float num = 0f;
            Difficulty difficultyName = (Difficulty)customHP;
            Difficulty difficultyNameATK = (Difficulty)customAttack;
            
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
                if (difficultyNameATK >= Difficulty.D6)
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

        [HarmonyPatch(typeof(ObjectPhy), "WallHit")]
        [HarmonyPrefix]
        static bool reduceWallDmg(ref CharacterBase ___cb_perfer)
        {
            if (___cb_perfer.isPlayer())
            {
                float maxMult = 1f;
                int customAttack = TeviSettings.customAtkDiff;
                int customHP = TeviSettings.customHpDiff;
                if (TeviSettings.customAtkDiff < 0)
                    customAttack = (int)(SaveManager.Instance.GetDifficultyName());
                if (TeviSettings.customHpDiff < 0)
                    customHP = (int)(SaveManager.Instance.GetDifficultyName());

                if (customAttack >= 21)
                    maxMult = 1.1f + (customAttack - 20) * 0.0001f;
                float diff = customAttack - 5 + SaveManager.Instance.DifficultyMinMaxOffset;
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