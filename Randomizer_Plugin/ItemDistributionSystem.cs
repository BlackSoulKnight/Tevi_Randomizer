using BepInEx;
using EventMode;
using Spine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace TeviRandomizer
{
    class TeviItemInfo
    {
        public ItemList.Type Type;
        public byte Value;
        public bool Randomized;
        public string Name;
        public string Description;
        public bool SkipHUD = false;
        public Sprite ItemIcon = null;
        public TeviItemInfo(ItemList.Type type,byte value, bool randomized, string name = "", string description = "", bool skipHUD = false, Sprite itemIcon = null)
        {
            Type = type;
            Value = value;
            Randomized = randomized;
            Name = name;
            Description = description;
            SkipHUD = skipHUD;
            ItemIcon = itemIcon;
        }
    }

    class ItemDistributionSystem : MonoBehaviour
    {
        private static Queue<TeviItemInfo> ItemQueue = new Queue<TeviItemInfo>();
        private static Queue<ItemList.Resource> ResourceQueue = new Queue<ItemList.Resource>();

        void Update()
        {
            if (WorldManager.Instance?.MapInited == true && !EventManager.Instance.IsChangingMap() && GemaUIPauseMenu.Instance.GetAllowPause())
            {
                if (!checkResourcePause() && ResourceQueue.Count > 0)
                    {
                        var em = EventManager.Instance;
                        var resource = ResourceQueue.Dequeue();
                        ElementType type = resource == ItemList.Resource.COIN ? ElementType.B_COIN : ElementType.B_UPGRADE;
                        ItemSystemPatch.customCollector(em.mainCharacter.t.position, type,resource);
                        CollectManager.Instance.CreateCollect(em.mainCharacter.t.position, type, resource);

                    }
                if (!checkHUDObtainPause() && ItemQueue.Count > 0)
                {
                    var item = ItemQueue.Dequeue();
                    if (item != null)
                    {
                        //save original
                        string localizeName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), false);
                        string localizeDesc = Localize.GetLocalizeTextWithKeyword("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), false);


                        //change
                        RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), item.Name);
                        if (!item.Name.IsNullOrWhiteSpace())
                            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), item.Description);

                        //give player
                        if (item.SkipHUD)
                            //@TODO Make it work
                            SaveManager.Instance.SetItem(item.Type, item.Value);
                        else
                        {
                            ItemSystemPatch.ChangeItemSpriteTemp = item.ItemIcon;
                            HUDObtainedItem.Instance.GiveItem(item.Type, item.Value, item.Randomized);
                        }

                        // reverse change
                        if (!item.Description.IsNullOrWhiteSpace())
                            RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), localizeName);
                        if (!item.Name.IsNullOrWhiteSpace())
                            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), localizeDesc);

                    }
                }
            }
        }

        void changeText(TeviItemInfo item)
        {



            RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Value), item.Name);
            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Value), item.Description);
        }

        bool checkResourcePause()
        {
            if (EventManager.Instance != null && EventManager.Instance.getMode() != EventMode.Mode.OFF)
                return true;

            return GameSystem.Instance?.isAnyPause() == true;
        }
        bool checkHUDObtainPause()
        {
            if (HUDSaveMenu.Instance != null && HUDSaveMenu.Instance.isDisplaying())
                return true;
            if (PauseFrame.Instance != null && PauseFrame.Instance.isActiveAndEnabled == true)
                return true;
            if (GemaUIChangeDifficulty.Instance != null && GemaUIChangeDifficulty.Instance.isDisplaying())
                return true;
            if (HUDUseItemWindow.Instance != null && HUDUseItemWindow.Instance.isDisplaying())
                return true;
            if (HUDObtainedItem.Instance != null && HUDObtainedItem.Instance.isDisplaying())
                return true;
            if (HUDHelpScreen.Instance != null && HUDHelpScreen.Instance.isDisplaying())
                return true;
            if (GemaQuestionWindow.Instance != null && GemaQuestionWindow.Instance.isDisplaying())
                return true;


            return false;
        }

        void test()
        {

            TeviItemInfo item = new TeviItemInfo(ItemList.Type.I16, 1, true);
            ItemQueue.Enqueue(item);
            ItemQueue.Enqueue(item);
            ItemQueue.Enqueue(item);
            ItemQueue.Enqueue(item);
            ItemQueue.Enqueue(item);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
            ResourceQueue.Enqueue(ItemList.Resource.COIN);
        }

        public static void EnqueueItem(TeviItemInfo item)
        {
            if (!item.Randomized)
            {
                if (RandomizerPlugin.checkItemGot(item.Type, item.Value))
                    return;
                LocationTracker.addItemToList(item.Type, item.Value);

                var originalItem = item.Type;
                item.Type = RandomizerPlugin.getRandomizedItem(item.Type, item.Value);
                if (item.Type == RandomizerPlugin.PortalItem)
                {
                    string itemName = "";
                    if (RandomizerPlugin.__itemData.ContainsKey(LocationTracker.APLocationName[$"{originalItem} #{item.Value}"]))
                        itemName = RandomizerPlugin.__itemData[LocationTracker.APLocationName[$"{originalItem} #{item.Value}"]];
                    item.Value = byte.Parse(itemName.Split(["Teleporter "], StringSplitOptions.RemoveEmptyEntries)[0]);
                    item.Name = (string)ArchipelagoInterface.Instance.TeviToAPName[item.Name];
                    item.Description = $"{itemName} is now available.";
                }

                if (ArchipelagoInterface.Instance.isConnected)
                {
                    if (item.Type == ArchipelagoInterface.remoteItem || item.Type == ArchipelagoInterface.remoteItemProgressive)
                    {

                        item.Name = ArchipelagoInterface.Instance.getLocItemName(originalItem, item.Value);
                        string playerName = ArchipelagoInterface.Instance.getLocPlayerName(originalItem, item.Value);
                        item.Description = $"You found {item.Name} for {playerName}";

                        if (Enum.TryParse(item.Name, out ItemList.Type fake))
                        {
                            item.Name = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + fake, true);
                            item.ItemIcon = CommonResource.Instance.GetItem((int)fake);
                            item.Description = "<color=\"red\">" + $"                                                                          <size=200%> FOOL!</color><size=100%>\n\n\n<font-weight=100>{item.Name} was stolen by {playerName}";
                        }
                    }
                }
            }

            switch (item.Type)
            {
                case RandomizerPlugin.MoneyItem:
                    ResourceQueue.Enqueue(ItemList.Resource.COIN);
                    break;
                case RandomizerPlugin.ItemUpgradeItem:
                    ResourceQueue.Enqueue(ItemList.Resource.UPGRADE);
                    break;
                case RandomizerPlugin.CoreUpgradeItem:
                    ResourceQueue.Enqueue(ItemList.Resource.CORE);
                    break;
                default:
                    ItemQueue.Enqueue(item);
                    break;
            }
        }

        //Traps





        //Savegame stuff


        static TeviItemInfo[] AutoSaveItemQueue;
        static ItemList.Resource[] AutoSaveResourceQueue;
        public static void reset()
        {
            ItemQueue.Clear();
            ResourceQueue.Clear();
        }
        public static void loadFromSlot(ES3File savefile)
        {
            if (savefile.KeyExists("ItemQueue"))
                ItemQueue = new(savefile.Load<TeviItemInfo[]>("ItemQueue"));
            if (savefile.KeyExists("ResourceQueue"))
                ResourceQueue = new(savefile.Load<ItemList.Resource[]>("ResourceQueue"));
            
        }
        public static void saveToSlot(ES3File saveFile)
        {
            saveFile.Save<TeviItemInfo[]>("ItemQueue",ItemQueue.ToArray());
            saveFile.Save<ItemList.Resource[]>("ResourceQueue", ResourceQueue.ToArray());
        }
        public static void saveToTmpSlot(ES3File saveFile)
        {
            saveFile.Save<TeviItemInfo[]>("ItemQueue", AutoSaveItemQueue);
            saveFile.Save<ItemList.Resource[]>("ResourceQueue", AutoSaveResourceQueue);
        }
        public static void saveToTmp()
        {
            AutoSaveItemQueue = ItemQueue.ToArray();
            AutoSaveResourceQueue = ResourceQueue.ToArray();
        }


    }
}
