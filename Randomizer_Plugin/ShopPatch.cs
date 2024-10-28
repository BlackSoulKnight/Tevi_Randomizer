using EventMode;
using Game;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
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
        static void addValAndTar(ref string ___fname, ref Character.Type ___typeN, ref int ___CurrentMaxItem, ref GemaShopItemSlot[] ___itemslots, ref HUDShopMenu __instance)
        {
            byte area = WorldManager.Instance.Area;
            ChatStand chatStand = AreaResource.Instance.GetChatStand(___fname);
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
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);

                        }
                        break;
                    case Character.Type.Bones:
                        if (area != 20 && SaveManager.Instance.GetItem(ItemList.Type.ITEM_AirshipPass) > 0)
                        {
                            obj = [ItemList.Type.BADGE_AutoAirCombo, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        break;
                    case Character.Type.Vena:

                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaSmall) != 0)
                        {
                            obj = [ItemList.Type.Useable_VenaBombSmall, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBB) != 0 && SaveManager.Instance.GetChapter() >= 2)
                        {
                            obj = [ItemList.Type.Useable_VenaBombBunBun, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaD) != 0 && SaveManager.Instance.GetChapter() >= 3)
                        {
                            obj = [ItemList.Type.Useable_VenaBombDispel, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaBig) != 0 && SaveManager.Instance.GetChapter() >= 4)
                        {
                            obj = [ItemList.Type.Useable_VenaBombBig, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedVenaHB) != 0 && SaveManager.Instance.GetChapter() >= 5)
                        {
                            obj = [ItemList.Type.Useable_VenaBombHealBlock, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        break;

                    case Character.Type.Mia:
                        if (SaveManager.Instance.GetMiniFlag(Mini.UnlockedWaffleA) != 0)
                        {
                            obj = [ItemList.Type.Useable_WaffleAHoneycloud, false];
                            Traverse.Create(__instance).Method("AddItem", obj).GetValue();
                            ___itemslots[___CurrentMaxItem - 1].gameObject.SetActive(true);
                        }
                        break;
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
                        byte slot = 1;
                        if (___itemslots[___Selected].GetItem().ToString().Contains("STACKABLE"))
                        {
                            slot = (byte)(30 + ___ShopID);
                        }

                        data = RandomizerPlugin.getRandomizedItem(___itemslots[___Selected].GetItem(),slot);

                        // SetItem does not have a function to add a Location check to the list
                        LocationTracker.addItemToList(___itemslots[___Selected].GetItem(), slot);

                        if (data.ToString().Contains("STACKABLE"))
                        {
                            SaveManager.Instance.SetStackableItem(data, 1, value: true);
                            if (data == ItemList.Type.STACKABLE_BAG)
                            {
                                SettingManager.Instance.SetAchievement(Achievements.ACHI_SHOP_BUYBAG);
                            }
                        }
                        else
                        {
                            SaveManager.Instance.SetItem(data, 1);
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
            if (ArchipelagoInterface.Instance.isConnected && (___itype == ItemList.Type.I10 || ___itype == ItemList.Type.I11))
            {
                string itemName = ArchipelagoInterface.Instance.getLocItemName(item, slot);
                texts[0].text = itemName;
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
                if (ArchipelagoInterface.Instance.isConnected && (data == ItemList.Type.I10 || data == ItemList.Type.I11))
                {
                    string itemName = ArchipelagoInterface.Instance.getLocItemName(item, slot);
                    string playerName = ArchipelagoInterface.Instance.getLocPlayerName(item, slot);
                    ___item_desc.text = "<font-weight=200>" + $"You found {itemName} for {playerName}";
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
            }
        }

    }

}
