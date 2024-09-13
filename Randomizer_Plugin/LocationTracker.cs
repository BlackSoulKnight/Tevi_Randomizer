/* Define:
        how Items are recieved
        how Items are send
        how to hide collected Items
        Item ID resolve

    v recheck for Archipelago items
 () Crafting
 () Shop
 () WorldItems
     Add item sending to Archipelago server
     
*/

using System.Collections.Generic;
using System.Linq;

namespace TeviRandomizer
{
    class LocationTracker
    {
        public static bool active = true;
        static List<string> collectedLocationList = new List<string>();
        public static void setCollectedLocationList(string[] list)
        {
            collectedLocationList.Clear();
            collectedLocationList.AddRange(list);
        }
        public static string[] getCollectedLocationList() { 

            return collectedLocationList.ToArray(); 
        }
        public static bool hasItem(ItemList.Type item,byte slot)
        {
            return collectedLocationList.Contains($"{item} #{slot}");
        }
        public static void clearItemList() => collectedLocationList.Clear();
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            collectedLocationList.Add($"{item} #{slot}");
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.checkoutLocation($"{item} #{slot}");
            }
        }
        public static void addItemToList(string location)
        {
            collectedLocationList.Add(location);
            if (ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.checkoutLocation(location);
            }
        }
        public static int getListLenght()
        {
            return collectedLocationList.Count();
        }

        
    }
}