using System;
using System.Collections.Generic;
using System.ComponentModel;
using EditorVar;
using HarmonyLib;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UIR.Allocator2D;


namespace TeviRandomizer
{
    class CustomMap
    {
        static WorldManager.TileData setUpTileData(int x, int y, int spriteID, bool flipH, bool flipV, Layer layer) {
            WorldManager.TileData tileData = new WorldManager.TileData();
            tileData.spriteID = spriteID;
            tileData.x = x;
            tileData.y = y;
            tileData.flipH = flipH;
            tileData.flipV = flipV;
            tileData.layer = layer;
            return tileData;
        }

        static void createItemTile(int x, int y, int spriteID, bool flipH, bool flipV)
        {
            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("itemset_prefab").GetValue<GameObject>());                          //Create new Item
            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);
            Layer layer = Layer.ITEM;
            WorldManager.TileData tileData = setUpTileData(x, y, spriteID, flipH, flipV, layer);

            float num5 = (float)x * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f;
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

            spriteRenderer.sortingOrder = 44;                                                                                                         //Item Layer

            WorldManager.Instance.areadata.tilelist.Add(tileData);
        }

        static void createNormalTile(int x, int y, int spriteID, bool flipH, bool flipV)
        {
            Layer layer = Layer.NORMAL;
            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("tileset_prefab").GetValue<GameObject>());                          //Create new Item
            BoxCollider2D boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
            EdgeCollider2D edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();

            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);

            WorldManager.TileData tileData = setUpTileData(x, y, spriteID, flipH, flipV, layer);

            float num5 = (float)x * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f;
            Vector3 localPosition = new Vector3(num5, (float)(-y) * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f, 1f);
            gameObject.transform.localPosition = localPosition;


            TileLink component = gameObject.GetComponent<TileLink>();
            SpriteRenderer spriteRenderer = null;
            spriteRenderer = (component ? component.sprite_prefab : gameObject.GetComponent<SpriteRenderer>());
            spriteRenderer.material = CommonResource.Instance.mat_SpriteLighting;

            //normal layer stuff
            int num = spriteID;
            int num2 = 0;
            int num3 = 0;
            if (spriteID >= 100000)
            {
                num3 = (int)((float)spriteID / 100000f);
                spriteID %= 100000;
            }
            if (spriteID >= 10000)
            {
                num2 = (int)((float)spriteID / 10000f);
                spriteID %= 10000;
            }

            int num7 = 468;
            bool flag = false;

            if (spriteID >= num7 && spriteID < 576)
            {
                spriteID = num;
                tileData.spriteID = spriteID + num2 * 10000 + num3 * 100000;
                flag = true;
                spriteRenderer.sprite = CommonResource.Instance.GetResourceTile(spriteID - num7);
            }
            else
            {
                spriteRenderer.sprite = AreaResource.Instance.GetTile(spriteID);
            }

            int num8 = spriteID;
            int num9 = (int)((float)num8 / 1000f);
            num8 = (num8 - num9 * 1000) % 36;
            bool flag3 = true;
            bool flag4 = false;
            if (layer != Layer.BACK2)
            {
                if (layer != 0 && (num8 == 1 || num8 == 2 || num8 == 27 || num8 == 28 || num8 == 29 || num8 == 30))
                {
                    flag4 = true;
                }
                if (flag3 && (num8 == 20 || num8 == 21))
                {
                    flag4 = true;
                }
            }
            bool flag2 = false;

            if (!flag && flag4)
            {
                flag2 = true;
                if (layer == Layer.NORMAL || layer == Layer.BACK1 || layer == Layer.BACK3)
                {
                    edgeCollider2D.enabled = true;
                }
                gameObject.layer = 20;
            }



            spriteRenderer.sortingOrder = 45;                                                                                                         //Normal Layer


            if (flag2)
            {
                boxCollider2D.enabled = false;
                if ((bool)edgeCollider2D && edgeCollider2D.enabled)
                {
                    WorldManager.Instance.areadata.SetHitBox(x, y, byte.MaxValue);
                }
            }
            else if (layer == Layer.NORMAL)
            {
                int num12 = spriteID % 36;
                byte b = 1;
                bool flag6 = false;
                if (WorldManager.Instance.Area == 19 && spriteID > 35)
                {
                    flag6 = true;
                }
                if (WorldManager.Instance.Area == 21 && spriteID <= 35)
                {
                    flag6 = true;
                }
                if (WorldManager.Instance.Area == 23 && spriteID < 72)
                {
                    flag6 = true;
                }
                if (WorldManager.Instance.Area == 29 && spriteID >= 108 && spriteID < 144)
                {
                    flag6 = true;
                }
                if (!flag)
                {
                    if (num12 == 3)
                    {
                        boxCollider2D.transform.localEulerAngles = new Vector3(0f, 0f, 333.435f);
                        boxCollider2D.transform.localPosition = new Vector3(0.3945f, -0.5065f, 0f);
                        boxCollider2D.size = new Vector2(2.5f, 1f);
                        b = 3;
                    }
                    else if (num12 == 4 || num12 == 6 || num12 == 7)
                    {
                        b = (byte)num12;
                        boxCollider2D.enabled = false;
                    }
                    else if (num12 == 5)
                    {
                        boxCollider2D.transform.localEulerAngles = new Vector3(0f, 0f, 340f);
                        boxCollider2D.transform.localPosition = new Vector3(0.89f, -0.54f, 0f);
                        boxCollider2D.size = new Vector2(3.33f, 1f);
                        b = 5;
                    }
                    else if (num12 == 10 && !flag6)
                    {
                        boxCollider2D.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
                        boxCollider2D.transform.localPosition = new Vector3(-0.793f, 0.0855f, 0f);
                        boxCollider2D.offset = new Vector2(0f, -0.91382f);
                        boxCollider2D.size = new Vector2(1f, 2f);
                        b = 2;
                    }
                    else if (num12 == 23 && !flag6)
                    {
                        boxCollider2D.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
                        boxCollider2D.transform.localPosition = new Vector3(-0.793f, 0.0855f, 0f);
                        boxCollider2D.offset = new Vector2(0f, -0.91382f);
                        boxCollider2D.size = new Vector2(1f, 2f);
                        b = 2;
                    }
                }
                if (boxCollider2D.enabled)
                {
                    if (flipH)
                    {
                        boxCollider2D.transform.parent.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        spriteRenderer.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    }
                    WorldManager.Instance.areadata.linklist.Add(component);
                    object[] obj = { component.gameObject };
                    worldmng.Method("SetDestoryable", obj).GetValue();
                }
                if (b >= 2 && flipH)
                {
                    b += 100;
                }
                WorldManager.Instance.areadata.SetHitBox(x, y, b);
            }


            WorldManager.Instance.areadata.tilelist.Add(tileData);
        }

        [HarmonyPatch(typeof(WorldManager), "InitMap")]
        [HarmonyPrefix]
        static void RemoveAddedTiles(ref WorldManager __instance)
        {
            if (WorldManager.Instance.areadata == null) return;
            //Delete old Map info
            Debug.Log("[Randomizer] Deleting Custom Map Tile/Items");
            Traverse.Create(__instance).Method("DeleteMap").GetValue();
            //__instance.Invoke("DeleteMap", 0f);
        }

        [HarmonyPatch(typeof(WorldManager), "DoMapInited")]
        [HarmonyPostfix]
        static void AdditionalChanges(ref WorldManager __instance)
        {
            if (WorldManager.Instance.Area == 8)
            {
                Traverse t = Traverse.Create(__instance);
                createItemTile(334, 205, 20, false, false);
                for (int i = 0; i < 3; i++)
                {
                    //createNormalTile(296, 177-i, 109, false, false);
                }
            }
        }
        /*
         [HarmonyPatch(typeof(GemaFreeRoamOnlyObject),"disableme")]
        [HarmonyPrefix]
        static bool destroyFreeRoamOnlyObject(ref GemaFreeRoamOnlyObject __instance)
        {
            Debug.LogWarning($"[Removal] Removing GameObject {__instance.gameObject.name}");
            __instance.gameObject.SetActive(false);
            return false;
        }
        */
    }
}
