using System;
using System.Collections.Generic;
using System.Text;

namespace TeviRandomizer
{
    internal class ItemRenames
    {
        internal static TeviItemInfo ChangeItemNameAndDescription(TeviItemInfo item)
        {
            var originalName = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + GemaItemManager.Instance.GetItemString(item.Value),false);
            var originalText = Localize.GetLocalizeTextWithKeyword("ITEMDESC." + GemaItemManager.Instance.GetItemString(item.Value),false);
            switch (item.Type)
            {
                case ItemList.Type.QUEST_Memory:
                    item.Name = "Morose Shop";
                    item.Description = "Unlocks shops in Morose";
                    break;
                case ItemList.Type.QUEST_Flute:
                    item.Name = "Ana Thema Shop";
                    item.Description = "Unlocks shops in Ana Thema";
                    break;
                case ItemList.Type.ITEM_RailPass:
                    item.Description = originalText+"\nAlso unlocks shops in Tartarus";
                    break;
                case ItemList.Type.QUEST_Compass:
                    item.Name = "Snow City Shop";
                    item.Description = "Unlocks shops in Snow City";
                    break;
                case ItemList.Type.ITEM_AirshipPass:
                    item.Description = originalText+"\nAlso unlocks shops in Valhalla";
                    break;
                default:
                    break;
            }
            return item;
        }
    }
}
