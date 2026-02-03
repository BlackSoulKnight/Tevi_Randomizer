using BepInEx;
using EventMode;
using System.Collections.Generic;
using UnityEngine;

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
        public static Queue<TeviItemInfo> ItemQueue = new Queue<TeviItemInfo>();
        public static Queue<ItemList.Resource> ResourceQueue = new Queue<ItemList.Resource>();

        void Update()
        {
            if (WorldManager.Instance?.MapInited == true && !EventManager.Instance.IsChangingMap() && GemaUIPauseMenu.Instance.GetAllowPause())
            {
                if (!checkResourcePause())
                    if (ResourceQueue.Count > 0 )
                    {
                        var em = EventManager.Instance;
                        var resource = ResourceQueue.Dequeue();
                        ElementType type = resource == ItemList.Resource.COIN ? ElementType.B_COIN : ElementType.B_UPGRADE;
                        CollectManager.Instance.CreateCollect(em.mainCharacter.t.position, type, resource);

                    }

                if (!checkHUDObtainPause())
                    if (ItemQueue.Count > 0)
                    {
                        var item = ItemQueue.Dequeue();
                        if (item != null)
                        {
                            //change Text if possible and item is alread randomized 
                            string localizeName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), false);
                            string localizeDesc = Localize.GetLocalizeTextWithKeyword("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), false);
                            if (item.Randomized)
                            {
                                var checkItem = ItemSystemPatch.itemToResource(item.Type);
                                if (checkItem != ItemList.Resource.RESOURCE_MAX)
                                {
                                    ResourceQueue.Enqueue(checkItem);
                                    return;
                                }

                                if (!item.Description.IsNullOrWhiteSpace())
                                    RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), item.Name);
                                if (!item.Name.IsNullOrWhiteSpace())
                                    RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), item.Description);
                            }
                            if (item.SkipHUD && item.Randomized)
                                //@TODO Make it work
                                SaveManager.Instance.SetItem(item.Type, item.Value);
                            else
                            {
                                ItemSystemPatch.ChangeItemSpriteTemp = item.ItemIcon;
                                HUDObtainedItem.Instance.GiveItem(item.Type, item.Value, item.Randomized);
                            }

                            // reverse Text change
                            if (item.Randomized)
                            {
                                if (!item.Description.IsNullOrWhiteSpace())
                                    RandomizerPlugin.changeSystemText("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Type), localizeName);
                                if (!item.Name.IsNullOrWhiteSpace())
                                    RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Type), localizeDesc);
                            }
                        }
                    }
            }
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

        static TeviItemInfo[] asItemQueue;
        static ItemList.Resource[] asResourceQueue;



        //Savegame stuff

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
            saveFile.Save<TeviItemInfo[]>("ItemQueue", asItemQueue);
            saveFile.Save<ItemList.Resource[]>("ResourceQueue", asResourceQueue);
        }
        public static void saveToTmp()
        {
            asItemQueue = ItemQueue.ToArray();
            asResourceQueue = ResourceQueue.ToArray();
        }


    }
}
