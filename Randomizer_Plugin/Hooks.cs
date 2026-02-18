using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    internal class Hooks
    {
        public static void GiveItemReplace(HUDObtainedItem instance,ItemList.Type type, byte slotid,bool isRandomBadge)
        {
            ItemDistributionSystem.EnqueueItem(new(type, slotid, false));
        }
    }
}
