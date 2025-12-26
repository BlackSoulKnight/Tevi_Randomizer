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
        public TeviItemInfo(ItemList.Type type,byte value, bool randomized, string name = "", string description = "", bool skipHUD = false)
        {
            Type = type;
            Value = value;
            Randomized = randomized;
            Name = name;
            Description = description;
            SkipHUD = skipHUD;
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
                                HUDObtainedItem.Instance.GiveItem(item.Type, item.Value, item.Randomized);

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
            if (HUDSaveMenu.Instance?.isDisplaying() == true)
                return true;
            if (PauseFrame.Instance?.isActiveAndEnabled == true)
                return true;
            if (GemaUIChangeDifficulty.Instance?.isDisplaying() == true)
                return true;
            if (HUDUseItemWindow.Instance?.isDisplaying() == true)
                return true;
            if (HUDObtainedItem.Instance?.isDisplaying() == true)
                return true;
            if (HUDHelpScreen.Instance?.isDisplaying() == true)
                return true;
            if (GemaQuestionWindow.Instance?.isDisplaying() == true)
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



    }
}
