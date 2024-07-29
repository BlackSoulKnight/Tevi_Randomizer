using System;
using System.Collections.Generic;
using System.ComponentModel;
using EditorVar;
using HarmonyLib;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UIElements;


namespace TeviRandomizer
{
    class CustomMap
    {
        static void createItemTile(int x,int y,int spriteID,bool flipH,bool flipV,Layer layer)
        {
            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("itemset_prefab").GetValue<GameObject>());
            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);


            WorldManager.TileData tileData = new WorldManager.TileData();
            tileData.spriteID = spriteID;
            tileData.x = x;
            tileData.y = y;
            tileData.flipH = flipH;
            tileData.flipV = flipV;
            tileData.layer = layer;


            float num5 = (float)x * MainVar.instance.TILESIZE;
            Vector3 localPosition = new Vector3(num5, (float)(-y) * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f, 1f);
            gameObject.transform.localPosition = localPosition;


            TileLink component = gameObject.GetComponent<TileLink>();
            SpriteRenderer spriteRenderer = null;
            spriteRenderer = (component ? component.sprite_prefab : gameObject.GetComponent<SpriteRenderer>());

            spriteRenderer.sprite = CommonResource.Instance.GetItem(spriteID);
            ItemTile component4 = gameObject.GetComponent<ItemTile>();
            component4.itemid = (ItemList.Type)spriteID;
            component4.SetSprite(spriteRenderer.sprite);
            WorldManager.Instance.areadata.itemlist.Add(component4);

            spriteRenderer.sortingOrder = 44;
            WorldManager.Instance.areadata.tilelist.Add(tileData);

        }


        [HarmonyPatch(typeof(WorldManager), "DoMapInited")]
        [HarmonyPostfix]
        static void AdditionalChanges(ref WorldManager __instance)
        {
            if (WorldManager.Instance.Area == 8)
            {
                Traverse t = Traverse.Create(__instance);
                createItemTile(334,205,20,false,false,Layer.ITEM);
                Debug.LogWarning("TRIGGERD");

            }
        }

    }
}
