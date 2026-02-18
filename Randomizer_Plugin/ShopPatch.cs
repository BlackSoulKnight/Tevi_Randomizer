using EventMode;
using Game;
using HarmonyLib;
using Map;
using System;
using TMPro;
using UnityEngine;

namespace TeviRandomizer
{
    class ShopPatch
    {
        static void AddItem(ItemList.Type item)
        {
            if (RandomizerPlugin.checkRandomizedItemGot(item, 1))
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

        static bool FreeShop()
        {
            switch (WorldManager.Instance.Area)
            {
                case 3:
                    //Morose
                    if (SaveManager.Instance.GetItem(ItemList.Type.QUEST_Memory) > 0)
                        return true;
                    break;
                case 8:
                    //Ana thema
                    if (SaveManager.Instance.GetItem(ItemList.Type.QUEST_Flute) > 0)
                        return true;
                    break;
                case 15:
                    //Taratrus
                    if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_RailPass) > 0)
                        return true;
                    break;
                case 16:
                    //Snow City
                    if (SaveManager.Instance.GetItem(ItemList.Type.QUEST_Compass) > 0)
                        return true;
                    break;
                case 20:
                    //Valhalla
                    if (SaveManager.Instance.GetItem(ItemList.Type.ITEM_AirshipPass) > 0)
                        return true;
                    break;
            }
            return false;

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
                byte slot = 1;
                if (item.ToString().Contains("STACKABLE"))
                    slot = (byte)(___ShopID + 30);



                if (item == ItemList.Type.STACKABLE_BAG)
                {
                    MainVar.instance.BagID = (byte)(___ShopID + 1);
                }

                if (RandomizerPlugin.checkRandomizedItemGot(item,slot) || ___CurrentMaxItem >= 15)
                    return false;


                if(ArchipelagoInterface.Instance?.isConnected == true && ArchipelagoInterface.Instance?.isItemProgessive(item,slot) == true)
                {
                    //ArchipelagoInterface.Instance.announceScoutedLocation(item, slot);
                }

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



        [HarmonyPatch(typeof(HUDShopMenu), "EnableMe")]
        [HarmonyPostfix]
        static void addValAndTar(ref string ___fname, ref Character.Type ___typeN, ref int ___CurrentMaxItem, ref GemaShopItemSlot[] ___itemslots, ref HUDShopMenu __instance,
            ref byte ___ShopType, ref TextMeshPro ___shopName, ref byte ___ShopID,ref SpriteRenderer[] ___bgfx,ref SpriteRenderer ___buyo,ref SpriteRenderer ___buyb,ref SpriteRenderer ___talkingcharacter,
            ref SpriteRenderer ___framemain, ref Transform ___frameitemdetail,ref SpriteRenderer[] ___blackline,ref TextMeshPro ___item_costtitle,ref TextMeshPro ___leaveShopText,
            ref bool ___exiting,ref float ___isDisplay,ref Transform ___item_detail,ref Transform ___transport_detail,ref Transform ___purchaselist,ref TextMeshPro ___purchaseText,
            ref CoinCounter ___cc,ref Transform ___transport_target, ref byte ___TransportTo,ref TextMeshPro ___tmp_talk,ref bool __result)
        {
            byte area = WorldManager.Instance.Area;
            ChatStand chatStand = AreaResource.Instance.GetChatStand(___fname);
            bool flag = ___CurrentMaxItem == 0;
            if ((bool)chatStand)
            {
                object[] obj = null;
                switch (___typeN)
                {
                    case Character.Type.Reese:
                        if (area != 15 && SaveManager.Instance.GetItem(ItemList.Type.ITEM_RailPass) > 0)
                        {
                            obj = [ItemList.Type.BADGE_ChangeOrbCharger, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);

                        }
                        break;
                    case Character.Type.Bones:
                        if (area != 20 && SaveManager.Instance.GetItem(ItemList.Type.ITEM_AirshipPass) > 0)
                        {
                            obj = [ItemList.Type.BADGE_AutoAirCombo, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        break;
                    case Character.Type.Vena:

                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaSmall) != 0)
                        {
                            obj = [ItemList.Type.Useable_VenaBombSmall, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBB) != 0 && SaveManager.Instance.GetChapter() >= 2)
                        {
                            obj = [ItemList.Type.Useable_VenaBombBunBun, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaD) != 0 && SaveManager.Instance.GetChapter() >= 3)
                        {
                            obj = [ItemList.Type.Useable_VenaBombDispel, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBig) != 0 && SaveManager.Instance.GetChapter() >= 4)
                        {
                            obj = [ItemList.Type.Useable_VenaBombBig, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaHB) != 0 && SaveManager.Instance.GetChapter() >= 5)
                        {
                            obj = [ItemList.Type.Useable_VenaBombHealBlock, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }


                        break;

                    case Character.Type.Mia:
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleA) != 0)
                        {
                            obj = [ItemList.Type.Useable_WaffleAHoneycloud, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            if(___CurrentMaxItem >0)
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (!SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleB) != 0 && SaveManager.Instance.GetEventFlag(Mode.Chap4FirstTartarusCity) > 0)
                            {
                                obj = [ItemList.Type.Useable_WaffleBMeringue, false];
                                Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                                if (___CurrentMaxItem > 0)
                                    ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                            }
                            if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleC) != 0 && SaveManager.Instance.GetEventFlag(Mode.Chap4FirstValhallaCity) > 0)
                            {
                                obj = [ItemList.Type.Useable_WaffleCMorning, false];
                                Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                                if (___CurrentMaxItem > 0)
                                    ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                            }
                            if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleD) != 0 && SaveManager.Instance.GetEventFlag(Mode.Chap5_Ulvosa_Intro) > 0)
                            {
                                obj = [ItemList.Type.Useable_WaffleDJellydrop, false];
                                Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                                if (___CurrentMaxItem > 0)
                                    ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                            }
                            if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleE) != 0 && SaveManager.Instance.GetChapter() >= 6)
                            {
                                obj = [ItemList.Type.Useable_WaffleElueberry, false];
                                Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                                if (___CurrentMaxItem > 0)
                                    ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                            }
                        }
                            break;
                }
                if (flag)
                {
                    flag = ___CurrentMaxItem == 0;
                }
                if (!flag)
                {
                    Traverse t = Traverse.Create(__instance);
                    Debug.Log("[HUDShopMenu] Opening shop " + ___typeN.ToString() + " ID : " + ___ShopID);
                    for (int i = ___CurrentMaxItem; i < ___itemslots.Length; i++)
                    {
                        ___itemslots[i].TurnOff();
                    }
                    if (___ShopType == 0)
                    {
                        ___shopName.text = Localize.GetLocalizeTextWithKeyword("ShopTitle.Shop", contains: false);
                    }
                    else if (___ShopID == 0)
                    {
                        ___shopName.text = Localize.GetLocalizeTextWithKeyword("ShopTitle.Train", contains: false);
                    }
                    else if (___ShopID == 1)
                    {
                        ___shopName.text = Localize.GetLocalizeTextWithKeyword("ShopTitle.Airship", contains: false);
                    }
                    else if (___ShopID == 2)
                    {
                        if (WorldManager.Instance.CurrentRoomArea == AreaType.DENENTRY)
                        {
                            ___shopName.text = Localize.GetLocalizeTextWithKeyword("ShopTitle.Colosseum", contains: false);
                        }
                        if (WorldManager.Instance.CurrentRoomArea == AreaType.CIRCUSSTAGE)
                        {
                            ___shopName.text = Localize.GetLocalizeTextWithKeyword("ShopTitle.Circus", contains: false);
                        }
                    }
                
                    t.Method("UpdateShopItemDetail").GetValue();
                    ___bgfx[0].color = new Color(1f, 1f, 1f, 0f);
                    ___bgfx[1].color = new Color(1f, 1f, 1f, 0f);
                    ___blackline[0].color = new Color(0f, 0f, 0f, 0f);
                    ___blackline[1].color = new Color(0f, 0f, 0f, 0f);
                    ___buyo.color = new Color(1f, 1f, 1f, 0f);
                    ___buyb.color = new Color(1f, 1f, 1f, 0f);
                    ___talkingcharacter.transform.localPosition = new Vector3(800f, -400f, 0f);
                    ___framemain.transform.localPosition = new Vector3(0f, -900f, 0f);
                    ___frameitemdetail.transform.localPosition = new Vector3(-1000f, 0f, 0f);
                    ___item_costtitle.text = Localize.GetLocalizeTextWithKeyword("COST_NAME", contains: false);
                    ___leaveShopText.text = Localize.GetLocalizeTextWithKeyword("LEAVENPC", contains: false);
                    ___leaveShopText.text = InputButtonManager.Instance.AddButtonsToPromote(___leaveShopText.text);
                    ___exiting = false;
                    ___isDisplay = 0.01f;
                    __instance.gameObject.SetActive(value: true);
                    __instance.enabled = true;
                    if (___ShopType == 0)
                    {
                        ___item_detail.gameObject.SetActive(value: true);
                        ___transport_detail.gameObject.SetActive(value: false);
                        ___purchaselist.transform.localPosition = new Vector3(-152.3f, 204.2f, 0f);
                        ___purchaseText.text = Localize.GetLocalizeTextWithKeyword("PURCHASE", contains: false);
                        ___purchaseText.text = InputButtonManager.Instance.AddButtonsToPromote(___purchaseText.text);
                        ___purchaseText.transform.localPosition = new Vector3(-435.4f, -50f, 0f);
                        ___cc.transform.parent.gameObject.SetActive(value: true);
                    }
                    else
                    {
                        ___item_detail.gameObject.SetActive(value: false);
                        ___transport_detail.gameObject.SetActive(value: true);
                        if (___ShopType == 1)
                        {
                            if (SaveManager.Instance.GetEventFlag(Mode.Chap2FirstCity) > 0)
                            {
                                ___transport_target.gameObject.SetActive(value: true);
                            }
                            else
                            {
                                ___transport_target.gameObject.SetActive(value: false);
                                ___TransportTo = 0;
                            }
                            ___purchaseText.text = Localize.GetLocalizeTextWithKeyword("TRAVEL", contains: false);
                        }
                        if (___ShopType == 2)
                        {
                            ___purchaseText.text = Localize.GetLocalizeTextWithKeyword("STARTTEXT", contains: false);
                        }
                        ___purchaselist.transform.localPosition = new Vector3(-543f, 190f, 0f);
                        ___purchaseText.text = InputButtonManager.Instance.AddButtonsToPromote(___purchaseText.text);
                        ___purchaseText.transform.localPosition = new Vector3(-435.4f, -75f, 0f);
                        ___cc.transform.parent.gameObject.SetActive(value: false);
                    }
                    if (___typeN == Character.Type.Mia && SaveManager.Instance.GetMiniFlag(Mini.OpenedMiaShop) <= 0)
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.OpenedMiaShop, 1);
                        string text = "SHOP." + ___typeN.ToString() + "_First";
                        ___tmp_talk.text = Localize.GetLocalizeTextWithKeyword(text, contains: false);
                        t.Method("CheckEmotion",new object[] {text}).GetValue();
                        t.Method("PlayShopVoice", new object[] {Character.Type.Mia,ShopVoiceType.First}).GetValue();
                    }
                    else if (___typeN == Character.Type.Ian && SaveManager.Instance.GetMiniFlag(Mini.OpenedIanShop) <= 0)
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.OpenedIanShop, 1);
                        string text2 = "SHOP." + ___typeN.ToString() + "_First";
                        ___tmp_talk.text = Localize.GetLocalizeTextWithKeyword(text2, contains: false);
                        t.Method("CheckEmotion",new object[] {text2}).GetValue();
                        t.Method("PlayShopVoice", new object[] {Character.Type.Ian,ShopVoiceType.First}).GetValue();
                    }
                    else if (___typeN == Character.Type.CC && SaveManager.Instance.GetMiniFlag(Mini.OpenedCCShop) <= 0)
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.OpenedCCShop, 1);
                        string text3 = "SHOP." + ___typeN.ToString() + "_First";
                        ___tmp_talk.text = Localize.GetLocalizeTextWithKeyword(text3, contains: false);
                        t.Method("CheckEmotion",new object[] {text3}).GetValue();
                        t.Method("PlayShopVoice", new object[] {Character.Type.CC,ShopVoiceType.First}).GetValue();
                    }
                    else if (___typeN == Character.Type.Vena && SaveManager.Instance.GetMiniFlag(Mini.OpenedVenaShop) <= 0)
                    {
                        SaveManager.Instance.SetMiniFlag(Mini.OpenedVenaShop, 1);
                        string text4 = "SHOP." + ___typeN.ToString() + "_First";
                        ___tmp_talk.text = Localize.GetLocalizeTextWithKeyword(text4, contains: false);
                        t.Method("CheckEmotion",new object[] {text4}).GetValue();
                        t.Method("PlayShopVoice", new object[] {Character.Type.Vena,ShopVoiceType.First}).GetValue();
                    }
                    else
                    {
                        string text5 = "SHOP." + ___typeN.ToString() + "_Welcome";
                        ___tmp_talk.text = Localize.GetLocalizeTextWithKeyword(text5, contains: false);
                        t.Method("CheckEmotion",new object[] {text5}).GetValue();
                        t.Method("PlayShopVoice", new object[] { ___typeN, ShopVoiceType.Welcome }).GetValue();
                    }
                    ___tmp_talk.text = Localize.AddColorToChat(___tmp_talk.text);
                    __result = true;
                }

            }
        }

        [HarmonyPatch(typeof(HUDShopMenu), "Update")]
        [HarmonyPrefix]
        static bool ChangeBuySystem(ref HUDShopMenu __instance, ref GemaShopItemSlot[] ___itemslots, ref int ___Selected, ref Character.Type ___typeN, ref SpriteRenderer ___buyo, ref bool ___bought, ref byte ___ShopID)
        {
            Traverse trav = Traverse.Create(__instance);
            if (InputButtonManager.Instance.GetButtonDown(13) && __instance.ShopType == 0)
            {
                ItemList.Type data;
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
                    bool freeShop = FreeShop();
                    if ((SaveManager.Instance.GetResource(ItemList.Resource.COIN) >= price || freeShop) && flag2)
                    {
                        if(!freeShop)
                            SaveManager.Instance.SubResource(ItemList.Resource.COIN, price);
                        if (___typeN == Character.Type.Ian)
                        {
                            SaveManager.Instance.savedata.coinUsedIan += price;
                        }
                        else if (___typeN == Character.Type.CC)
                        {
                            SaveManager.Instance.savedata.coinUsedCC += price;
                        }
                        byte slot = 1;
                        if (___itemslots[___Selected].GetItem().ToString().Contains("STACKABLE"))
                        {
                            slot = (byte)(30 + ___ShopID);
                        }


                         ItemDistributionSystem.EnqueueItem(new(___itemslots[___Selected].GetItem(), slot, false, skipHUD:true));

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
            ItemList.Type item = t;

            byte slot = 1;
            if (Traverse.Create(HUDShopMenu.Instance).Field("typeN").GetValue<Character.Type>() == Character.Type.CC)
            {
                slot = (byte)(shopID + 30);
                ___itype = RandomizerPlugin.getRandomizedItem(t, slot);
                
            }
            else
            {
                ___itype = RandomizerPlugin.getRandomizedItem(t, 1);
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
            if (ArchipelagoInterface.Instance.isConnected && (___itype == ArchipelagoInterface.remoteItem || ___itype == ArchipelagoInterface.remoteItemProgressive))
            {
                string itemName = ArchipelagoInterface.Instance.getLocItemName(item, slot);
                if (Enum.TryParse(itemName, out item))
                {
                    itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item.ToString(), true) + "?";
                    ___itemicon.sprite = CommonResource.Instance.GetItem((int)item);
                    if (item >= ItemList.Type.BADGE_START && item <= ItemList.Type.BADGE_MAX)
                    {
                        texts[1].text = Localize.GetLocalizeTextWithKeyword("ITEMTYPE.Badge", contains: false);
                        texts[1].color = new Color32(byte.MaxValue, 186, 95, byte.MaxValue);
                        bgicon.enabled = true;
                    }
                    else
                    {
                        texts[1].color = new Color32(152, 222, byte.MaxValue, byte.MaxValue);
                        texts[1].text = Localize.GetLocalizeTextWithKeyword("ITEMTYPE.Item", contains: false);
                        bgicon.enabled = false;
                    }
                }
                texts[0].text = itemName;
            }
            if (___itype == RandomizerPlugin.PortalItem)
            {
                string itemName = (string)ArchipelagoInterface.Instance.TeviToAPName[RandomizerPlugin.__itemData[LocationTracker.APLocationName[$"{item} #{slot}"]]];
                texts[0].text = itemName;
            }

            texts[2].text = FreeShop()? "0":_price.ToString();

            ___itype = t;
            return false;
        }


        [HarmonyPatch(typeof(HUDShopMenu), "UpdateShopItemDetail")]
        [HarmonyPostfix]
        static void ItemShopDescriptionFix(ref HUDShopMenu __instance, ref TextMeshPro ___item_desc, ref byte ___ShopID, ref GemaShopItemSlot[] ___itemslots, ref int ___Selected, ref TextMeshPro ___item_price)
        {

            if (__instance.ShopType == 0)
            {

                byte slot = 1;
                if (Traverse.Create(HUDShopMenu.Instance).Field("typeN").GetValue<Character.Type>() == Character.Type.CC)
                {
                    byte shopID = Traverse.Create(HUDShopMenu.Instance).Field("ShopID").GetValue<byte>();
                    slot = (byte)(shopID + 30);
                }

                ItemList.Type item = ___itemslots[___Selected].GetItem();
                ItemList.Type data;
                if (item.ToString().Contains("STACKABLE"))
                {
                    data = RandomizerPlugin.getRandomizedItem(item, (byte)(___ShopID + 30));
                    ___item_desc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data);
                }
                else
                {
                    data = RandomizerPlugin.getRandomizedItem(item, 1);
                    ___item_desc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data);
                }
                if (ArchipelagoInterface.Instance.isConnected && (data == ArchipelagoInterface.remoteItem || data == ArchipelagoInterface.remoteItemProgressive))
                {
                    string itemName = ArchipelagoInterface.Instance.getLocItemName(item, slot);
                    string playerName = ArchipelagoInterface.Instance.getLocPlayerName(item, slot);

                    ___item_desc.text = "<font-weight=200>" + $"You found {itemName} for {playerName}";
                    if (Enum.TryParse(itemName, out item))
                    {
                        itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item.ToString(), true);

                        ___item_desc.text = Localize.GetLocalizeTextWithKeyword("ITEMDESC." + item.ToString(), true);
                        ___item_desc.text = $"<font-weight=200>{playerName} will appreciate if you get this or not.\n\n" + Localize.AddColorToBadgeDesc(___item_desc.text);

                    }
                }
                if (___item_desc.text.Contains("[c2]"))
                {
                    ___item_desc.text = Localize.FilterLevelDescFromItem(data, ___item_desc.text);
                }
                if (data.ToString().Contains("Useable_"))
                {
                    ___item_desc.text += Localize.GetLocalizeTextWithKeyword("ITEMDESC.WAFFLEBUY", contains: false);
                }
                if (data >= ItemList.Type.BADGE_START && data <= ItemList.Type.BADGE_MAX)
                {
                    TextMeshPro textMeshPro = ___item_desc;
                    textMeshPro.text = textMeshPro.text + "<br><br>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.EQUIPBADGETIPS", contains: false);
                }
                ___item_desc.text = InputButtonManager.Instance.AddButtonsToPromote(___item_desc.text);
                if (FreeShop())
                {
                    ___item_price.text = "0";
                }
            }
        }

    }

}
