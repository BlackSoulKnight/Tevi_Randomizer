/* Define:
        how Items are recieved
        how Items are send
        how to hide collected Items
        Item ID resolve


 (x) Rework item checks into a seperate List
 (x) Crafting
 (x) Shop
 (x) WorldItems
 (x) Vena Boss fight
     
    Change this to the standart (unlimited item level possible)
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
        public static string[] getCollectedLocationList() {  return collectedLocationList.ToArray(); }
        public static bool hasItem(ItemList.Type item,byte slot)
        {
            return collectedLocationList.Contains($"{item} #{slot}");
        }
        public static void addItemToList(ItemList.Type item, byte slot)
        {
            collectedLocationList.Add($"{item} #{slot}");
        }

        
    }
}