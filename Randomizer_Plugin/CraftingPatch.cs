using EventMode;
using Game;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.UI;

namespace TeviRandomizer
{
    class CraftingPatch
    {

        //Craftig Orb Fix
        //No set SlotId, maybe reserver slots for Potions?
        [HarmonyPatch(typeof(SaveManager), "GetOrbTypeObtained")]
        [HarmonyPostfix]
        static void orbTypeFix(ref int __result, ref SaveManager __instance)
        {
            __result = 0;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeC2, 1))
                __result++;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeS2, 1))
                __result++;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeC3, 1))
                __result++;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbTypeS3, 1))
                __result++;
        }

        [HarmonyPatch(typeof(SaveManager), "GetOrbBoostObtained")]
        [HarmonyPostfix]
        static void OrbBoostCount(ref int __result, SaveManager __instance)
        {
            __result = 0;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbBoostD, 1))
            {
                __result++;
            }
            else return;
            if (RandomizerPlugin.checkRandomizedItemGot(ItemList.Type.ITEM_OrbBoostU, 1))
            {
                __result++;
            }
            else return;
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


        [HarmonyPatch(typeof(GemaUIPauseMenu_CraftGrid), "UpdateSelectedText")]
        [HarmonyPrefix]
        static bool itemDescChange(ref GemaUIPauseMenu_CraftGrid __instance, ref Image ___iconbg, ref Transform ___costBox, ref GemaUIPauseMenu_CraftGridSlot[] ___craftList, ref int ___selected, ref TextMeshProUGUI ___selectedName, ref TextMeshProUGUI ___selectedDesc,
            ref int ___CurrentMaxCraft, ref TextMeshProUGUI ___costTitle, ref TextMeshProUGUI ___costValue, ref Image ___selectedIcon, ref TextMeshProUGUI ___mownedText, ref TextMeshProUGUI ___useableText, ref bool ___setFontOutline, ref TextMeshProUGUI[] ___materialrequiredList,
            ref TextMeshProUGUI ___mrequiredText, ref byte[] ___currentMaterialNeeded, ref ItemList.Type ___currentItemType)
        {
            Traverse t = Traverse.Create(__instance);
            if (___craftList[___selected].GetItemType().ToString().Contains("Useable")) return true;



            ___iconbg.enabled = false;
            ___costBox.gameObject.SetActive(value: false);
            ItemList.Type data = RandomizerPlugin.getRandomizedItem(___craftList[___selected].GetItemType(), 1);
            byte slot = 1;


            ItemList.Type itemType = ___craftList[___selected].GetItemType();
            if (itemType.ToString().Contains("ITEM"))
            {
                slot = (byte)(SaveManager.Instance.GetItem(itemType) + 1);
                data = RandomizerPlugin.getRandomizedItem(itemType, slot);
                if (___craftList[___selected].isUpgrade)
                {
                    slot = (byte)(getItemUpgradeCount(itemType) + 1);
                    data = RandomizerPlugin.getRandomizedItem(itemType, slot);
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
                    ___costValue.text = GemaItemManager.Instance.GetItemCost(data).ToString();
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
                else if (data == ArchipelagoInterface.remoteItem|| data == ArchipelagoInterface.remoteItemProgressive)
                {
                    if (ArchipelagoInterface.Instance.isConnected)
                    {

                        string itemName = ArchipelagoInterface.Instance.getLocItemName(itemType, slot);
                        string playerName = ArchipelagoInterface.Instance.getLocPlayerName(itemType, slot);

                        ___selectedDesc.text = $"You found {itemName} for {playerName}";

                        ItemList.Type item;
                        if (Enum.TryParse(itemName, out item))
                            {
                            ___selectedDesc.text = Localize.GetLocalizeTextWithKeyword("ITEMDESC." + item.ToString(), true);

                            ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(item);

                            if (___selectedDesc.text.Contains("[c2]"))
                            {
                                if (___craftList[___selected].isUpgrade)
                                {
                                    //___selectedDesc.text = Localize.FilterLevelDescFromItem2(data.getItemTyp(), ___selectedDesc.text);
                                    ___selectedDesc.text = Localize.FilterLevelDescFromItem2(item, ___selectedDesc.text);
                                }
                                else
                                {
                                    ___selectedDesc.text = Localize.FilterLevelDescFromItem(item, ___selectedDesc.text);
                                }
                            }
                            if (item == ItemList.Type.Useable_WaffleWonderTemp)
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
                            if (item.ToString().Contains("Useable"))
                            {
                                TextMeshProUGUI textMeshProUGUI2 = ___selectedDesc;
                                textMeshProUGUI2.text = textMeshProUGUI2.text + "<br>" + Localize.GetLocalizeTextWithKeyword("ITEMDESC.USEABLETIPS", contains: false);
                            }
                            if (item == ItemList.Type.ITEM_OrbAmulet && SaveManager.Instance.GetItem(ItemList.Type.ITEM_OrbAmulet) <= 0)
                            {
                                int num2 = ___selectedDesc.text.IndexOf("<br>");
                                if (num2 >= 0)
                                {
                                    ___selectedDesc.text = ___selectedDesc.text.Substring(0, num2);
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (___craftList[___selected].isUpgrade)
                        ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(itemType);
                    else
                        ___selectedDesc.text = "<font-weight=200>" + Localize.AddColorToBadgeDesc(data);

                    if (itemType.ToString().Contains("MaterialExchange"))
                        ___selectedDesc.text += "<br>It also creates 250 Zenny.";
                    if (___selectedDesc.text.Contains("[c2]"))
                    {
                        if (___craftList[___selected].isUpgrade)
                        {
                            //___selectedDesc.text = Localize.FilterLevelDescFromItem2(data.getItemTyp(), ___selectedDesc.text);
                            ___selectedDesc.text = Localize.FilterLevelDescFromItem2(itemType, ___selectedDesc.text);
                        }
                        else
                        {
                            ___selectedDesc.text = Localize.FilterLevelDescFromItem(data, ___selectedDesc.text);
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

            if (o.Field("CurrentMaxCraft").GetValue<int>() >= ___craftList.Length)
            {
                return false;
            }

            if (iType.ToString().Contains("BADGE"))
            {
                if (RandomizerPlugin.checkRandomizedItemGot(iType, 1))
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


            ItemList.Type data = RandomizerPlugin.getRandomizedItem(itype, 1);
            __instance.isUpgrade = _isUpgrade;
            __instance.SetVisible(isVisible: true);
            ___itemType = itype;

            ___iconbg.enabled = false;
            __instance.UpdateIcon(itype);



            ItemList.Type item;



            byte b = 0;
            if (itype == ItemList.Type.Function_MaterialExchangeA || itype == ItemList.Type.Function_MaterialExchangeB)
            {
                b = 3;
            }
            else if (itype >= ItemList.Type.BADGE_START && itype <= ItemList.Type.BADGE_MAX)
            {
                b = 1;
                __instance.UpdateIcon(data);
                string itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + data.ToString(), true);

                if (data == ArchipelagoInterface.remoteItem || data == ArchipelagoInterface.remoteItemProgressive)
                {
                    if (ArchipelagoInterface.Instance.isConnected)
                    {
                        itemName = ArchipelagoInterface.Instance.getLocItemName(itype, 1);

                        if (Enum.TryParse(itemName, out item))
                        {
                            __instance.UpdateIcon(item);
                            itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item.ToString(), true);
                        }
                    }
                }
                ___nameText.text = Localize.GetLocalizeTextWithKeyword("CRAFT_CraftBadge", contains: false) + " <color=#FFF>" + itemName;

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
            if (ArchipelagoInterface.Instance.isConnected && (itype.ToString().Contains("BADGE")) && (data == ArchipelagoInterface.remoteItem || data == ArchipelagoInterface.remoteItemProgressive))
            {
                string itemName = ArchipelagoInterface.Instance.getLocItemName(itype, 1);
                if (Enum.TryParse(itemName, out item))
                {
                    itemName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + item.ToString(), true);
                }
                ___nameText.text = Localize.GetLocalizeTextWithKeyword("CRAFT_CraftBadge", contains: false) + " <color=#FFF>" + itemName;
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
                    num = (RandomizerPlugin.checkRandomizedItemGot(itype, 1) ? 1:0);
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
                        num = (RandomizerPlugin.checkRandomizedItemGot(itype, 1) ? 1 : 0);
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
            if (SaveManager.Instance.GetItem(_item)>0)
            {
                num++;
            }
            else return num;
            if (RandomizerPlugin.checkRandomizedItemGot(_item, 2))
            {
                num++;
            }
            else return num;
            if (RandomizerPlugin.checkRandomizedItemGot(_item, 3))
            {
                num++;
            }
            else return num;
            
            return num;
        }

        [HarmonyPatch(typeof(GemaItemManager), "GetMat")]
        [HarmonyPrefix]
        static bool CustomMats(ref ItemList.Type iType, ref int count, ref int __result, ref GemaItemManager __instance)
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
                                __result = 0;
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
            ref GemaUIPauseMenu_ItemGridSub[] ___bagItems, ref (Character.OrbType, Character.OrbShootType, Character.OrbShootType, bool) __state)
        {
            Traverse trav = Traverse.Create(__instance);
            __state.Item4 = false;
            if (InputButtonManager.Instance.GetButtonDown(13) && !HUDObtainedItem.Instance.isDisplaying())
            {
                __state.Item1 = EventManager.Instance.mainCharacter.cphy_perfer.orbUsing;
                __state.Item2 = EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[0];
                __state.Item3 = EventManager.Instance.mainCharacter.cphy_perfer.orbShootType[1];
                __state.Item4 = true;
                if (___currentItemType.ToString().Contains("ITEM") && ___craftList[___selected].isUpgrade || ___currentItemType.ToString().Contains("BADGE") || ___currentItemType.ToString().Contains("_OrbBoost") || ___currentItemType.ToString().Contains("_OrbType")|| ___currentItemType.ToString().Contains("MaterialExchange"))
                {

                    int num5 = 1;
                    ItemList.Resource resource = ItemList.Resource.COIN;

                    Debug.Log(___currentItemType);
                    ItemList.Type rnd = RandomizerPlugin.getRandomizedItem(___currentItemType, 1);

                    if (___currentItemType.ToString().Contains("_OrbBoost") )
                    {
                        if(SaveManager.Instance.GetOrbBoostObtained() >= 2)
                            num5 = -3;
                    }
                    else if (___currentItemType.ToString().Contains("_OrbType"))
                    {
                        if (SaveManager.Instance.GetOrbTypeObtained() >= 4)
                        {
                            num5 = -3;
                        }
                    }
                   else if (___currentItemType == ItemList.Type.Function_MaterialExchangeA || ___currentItemType == ItemList.Type.Function_MaterialExchangeB)
                    {
                        if (!__instance.canExchange(___currentItemType))
                        {
                            num5 = ((___currentItemType != ItemList.Type.Function_MaterialExchangeA) ? (-8) : (-6));
                        }
                        else if (SaveManager.Instance.GetFunctionExchangeRemain() <= 0)
                        {
                            num5 = -7;
                        }
                    }
                    else if (getItemUpgradeCount(___currentItemType) >= 3) // helperfunction 
                    {
                        num5 = -3;
                    }
                    else if (!Enum.IsDefined(typeof(Upgradable), ___currentItemType.ToString()) && RandomizerPlugin.checkRandomizedItemGot(___currentItemType,1))
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
                            Debug.LogWarning($"[Randomizer] Already has {rnd.ToString()}");
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
                        if (___currentItemType.ToString().Contains("MaterialExchange"))
                        {
                            SaveManager.Instance.AddResource(ItemList.Resource.COIN, 250);
                            return true;
                        }
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
                        else if (___currentItemType.ToString().Contains("_OrbTyp"))
                        {
                            switch (SaveManager.Instance.GetOrbTypeObtained())
                            {
                                case 0:
                                    ___currentItemType = ItemList.Type.ITEM_OrbTypeC2;
                                    break;
                                case 1:
                                    ___currentItemType = ItemList.Type.ITEM_OrbTypeS2;
                                    break;
                                case 2:
                                    ___currentItemType = ItemList.Type.ITEM_OrbTypeC3;
                                    break;
                                case 3:
                                    ___currentItemType = ItemList.Type.ITEM_OrbTypeS3;
                                    break;
                            }
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

            if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 || true)
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

}
