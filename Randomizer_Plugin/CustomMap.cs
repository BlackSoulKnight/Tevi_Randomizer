using Archipelago.MultiClient.Net.Packets;
using BepInEx;
using EditorVar;
using EventMode;
using HarmonyLib;
using Map;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UIElements;
using static MonoMod.RuntimeDetour.DynamicHookGen;
using static UnityEngine.UIElements.UIR.Allocator2D;


namespace TeviRandomizer
{
    class CustomMap
    {
        public static float firstBlockMultiplierX = 23.894736f;
        public static float firstBlockMultiplierY = 13.941176f;
        static WorldManager.TileData setUpTileData(int x, int y, int spriteID, bool flipH, bool flipV, Layer layer)
        {
            WorldManager.TileData tileData = new WorldManager.TileData();
            tileData.spriteID = spriteID;
            tileData.x = x;
            tileData.y = y;
            tileData.flipH = flipH;
            tileData.flipV = flipV;
            tileData.layer = layer;
            return tileData;
        }

        public static void createItemTile(int x, int y, int spriteID, bool flipH, bool flipV)
        {
            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("itemset_prefab").GetValue<GameObject>());                          //Create new Item
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            //gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);
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

        public static void createNormalTile(int x, int y, int spriteID, bool flipH, bool flipV, int area = -1)
        {
            if (area == -1) { area = WorldManager.Instance.Area; }
            Layer layer = Layer.NORMAL;
            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("tileset_prefab").GetValue<GameObject>());                          //Create new Item
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            BoxCollider2D boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
            EdgeCollider2D edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();
            AreaResource resource = ResourcePatch.getAreaResource(area);
            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);

            WorldManager.TileData tileData = setUpTileData(x, y, spriteID, flipH, flipV, layer);

            float num5 = (float)x * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f;
            Vector3 localPosition = new Vector3(num5, (float)(-y) * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f, 1f);
            gameObject.transform.localPosition = localPosition;


            TileLink component = gameObject.GetComponent<TileLink>();
            SpriteRenderer spriteRenderer = null;
            spriteRenderer = (component ? component.sprite_prefab : gameObject.GetComponent<SpriteRenderer>());
            spriteRenderer.material = CommonResource.Instance.mat_SpriteLighting;
            spriteRenderer.enabled = true;

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
                spriteRenderer.sprite = resource.GetTile(spriteID);
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


        public static void createEnemyTile(int x, int y, int spriteID,int Area = -1)
        {
            bool flipH = false;
            bool flipV = false;
            float TILESIZE = MainVar.instance.TILESIZE;
            Layer layer = Layer.ENEMY;
            WorldManager wm = WorldManager.Instance;
            if (Area == -1) Area = WorldManager.Instance.Area;
            AreaResource areaResource = ResourcePatch.getAreaResource(Area);
            if (x < 0 || x < 0 || x >= MainVar.instance.MaxTileX || y >= MainVar.instance.MaxTileY)
            {
                return;
            }
            if (Application.isPlaying && WorldManager.Instance.FindTile(x, y, layer) != null)
            {
                {
                    Debug.Log("[WorldManager] Warning! File have duplicated tiles in same position! (" + (float)x * TILESIZE + "," + (float)y * (0f - TILESIZE) + ")");
                    return;
                }
            }

            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("enemy_prefab").GetValue<GameObject>());                          //Create new Item
            //gameObject.transform.GetChild(0).gameObject.SetActive(false);
            BoxCollider2D boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
            EdgeCollider2D edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();

            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);

            WorldManager.TileData tileData = setUpTileData(x, y, spriteID, flipH, flipV, layer);

            float num5 = (float)x * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f;
            Vector3 localPosition = new Vector3(num5, (float)(-y) * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f, 1f);
            gameObject.transform.localPosition = localPosition;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            TileLink component = gameObject.GetComponent<TileLink>();
            SpriteRenderer spriteRenderer = null;
            spriteRenderer = (component ? component.sprite_prefab : gameObject.GetComponent<SpriteRenderer>());
            spriteRenderer.material = CommonResource.Instance.mat_SpriteLighting;
            spriteRenderer.enabled = false;

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
                spriteRenderer.sprite = areaResource.GetTile(spriteID);
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



                spriteRenderer.sprite = CommonResource.Instance.GetDefaultTile();
                EnemyTile component5 = gameObject.GetComponent<EnemyTile>();
                component5.type = (Character.Type)spriteID;
                wm.areadata.enemylist.Add(component5);



                spriteRenderer.sortingOrder = 1000;                                                                                                         //Normal Layer

            spriteRenderer.enabled = false;
            gameObject.GetComponent<TextMeshPro>().enabled = false;


            WorldManager.Instance.areadata.tilelist.Add(tileData);
        }

        
        public static void createElementTile(int x, int y, int spriteID,int Area = -1)
        {
            bool flipH = false;
            bool flipV = false;
            float TILESIZE = MainVar.instance.TILESIZE;
            Layer layer = Layer.ELEMENT;
            WorldManager wm = WorldManager.Instance;
            if (Area == -1) Area = WorldManager.Instance.Area;

            AreaResource areaResource = ResourcePatch.getAreaResource(Area);
            if (x < 0 || x < 0 || x >= MainVar.instance.MaxTileX || y >= MainVar.instance.MaxTileY)
            {
                return;
            }
            if (Application.isPlaying && WorldManager.Instance.FindTile(x, y, layer) != null)
            {
                {
                    Debug.Log("[WorldManager] Warning! File have duplicated tiles in same position! (" + (float)x * TILESIZE + "," + (float)y * (0f - TILESIZE) + ")");
                    return;
                }
            }

            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("elementset_prefab").GetValue<GameObject>());                          //Create new Item
            //gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

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
            spriteRenderer.enabled = false;
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
                spriteRenderer.sprite = areaResource.GetTile(spriteID);
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



            spriteRenderer.sortingOrder = 1200;                                                                                                         //Normal Layer


            if (layer == Layer.ELEMENT)
            {
                spriteRenderer.sprite = CommonResource.Instance.GetDefaultTile();
                ElementTile component6 = gameObject.GetComponent<ElementTile>();
                component6.elementtype = (ElementType)spriteID;
                component6.SaveWorldPosition();
                gameObject.gameObject.name = "ElementTile(Clone) " + gameObject.transform.position.x + " " + gameObject.transform.position.y;
                
                if ((int)component6.elementtype >= 72 && (int)component6.elementtype <= 104)
                {
                    wm.areadata.idlist.Add(component6);
                }
                wm.areadata.elementlist.Add(component6);
                if ((int)component6.elementtype >= 1 && (int)component6.elementtype <= 30)
                {
                    worldmng.Method("destroyableList", new object[] { component6 }).GetValue();
                }
                int num11 = -1;
                float zpos2 = 0f;
                float ypos2 = 0f;
                float xpos2 = 0f;
                float size2 = 0f;
                switch (component6.elementtype)
                {
                    case ElementType.HealthDrop:
                        num11 = 1;
                        ypos2 = -0.215f;
                        size2 = 0.01785714f;
                        break;
                    case ElementType.SavePoint:
                        num11 = 0;
                        ypos2 = -0.55f;
                        size2 = 1f;
                        break;
                    case ElementType.INDUSTRY_SWING_PLATFORM:
                        num11 = 3;
                        break;
                    case ElementType.FOREST_FLOATING_WOOD:
                        num11 = 5;
                        break;
                    case ElementType.SHANTY_HOLE_LIGHT:
                        num11 = 7;
                        xpos2 = 0.5f;
                        zpos2 = -1f;
                        break;
                    case ElementType.ChangeMapEnter:
                        num11 = 14;
                        break;
                    case ElementType.EnterableDoor:
                        num11 = 15;
                        ypos2 = -0.55f;
                        size2 = 1f;
                        break;
                    case ElementType.WarpDevice:
                        num11 = 16;
                        ypos2 = -0.55f;
                        size2 = 1f;
                        break;
                    case ElementType.NPCOBJECT:
                        num11 = 1003;
                        ypos2 = 0f;
                        size2 = 0.01785714f;
                        break;
                    case ElementType.B_WOOD:
                    case ElementType.B_WOOD_CHAIN:
                    case ElementType.B_WOOD_ONCE:
                    case ElementType.B_SHOOT:
                    case ElementType.B_SHOOT_CHAIN:
                    case ElementType.B_SHOOT_ONCE:
                    case ElementType.B_ENEMY:
                    case ElementType.B_ENEMY_ONCE:
                    case ElementType.B_UPGRADE:
                    case ElementType.B_COIN:
                        if (num2 > 0)
                        {
                            num11 = 1004;
                            ypos2 = 0f;
                            size2 = 0.5f;
                        }
                        break;
                }
                if (num11 >= 0)
                {
                    GameObject gameObject3 = null;
                    if (num11 < 1000)
                    {
                        gameObject3 = UnityEngine.Object.Instantiate(worldmng.Field("elementObjects").GetValue<GameObject[]>()[num11]);
                    }
                    else
                    {
                        gameObject3 = UnityEngine.Object.Instantiate(CommonResource.Instance.GetCommonObject(num11 - 1000));
                        if (num11 == 1004)
                        {
                            BreakableHint component7 = gameObject3.GetComponent<BreakableHint>();
                            if (component7 != null)
                            {
                                wm.areadata.breakhintlist.Add(component7);
                                byte force = 0;
                                if (component6.elementtype.ToString().Contains("WOOD"))
                                {
                                    component7.SetColor(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
                                    force = 9;
                                }
                                if (component6.elementtype.ToString().Contains("SHOOT"))
                                {
                                    component7.SetColor(new Color32(0, 146, 210, byte.MaxValue));
                                    force = 7;
                                }
                                if (component6.elementtype.ToString().Contains("ENEMY"))
                                {
                                    component7.SetColor(new Color32(146, 236, 28, byte.MaxValue));
                                    force = 8;
                                }
                                if (component6.elementtype.ToString().Contains("COIN"))
                                {
                                    component7.SetColor(new Color32(236, 163, 28, byte.MaxValue));
                                }
                                if (component6.elementtype.ToString().Contains("UPGRADE"))
                                {
                                    component7.SetColor(new Color32(230, 63, 238, byte.MaxValue));
                                }
                                component7.RandomBreakTile(x, y, force);
                            }
                        }
                        if (num11 == 1005)
                        {
                            BreakableHint component8 = gameObject3.GetComponent<BreakableHint>();
                            if (component8 != null)
                            {
                                wm.areadata.breakhintlist.Add(component8);
                            }
                        }
                    }
                    wm.FinalCreateObject(gameObject3, xpos2, ypos2, zpos2, size2, -1f, 1f, gameObject.transform);
                }
                if ((int)component6.elementtype >= 155 && (int)component6.elementtype <= 187)
                {
                    float sizey = -1f;
                    float sizez = 1f;
                    if (areaResource.GetMapObject((int)(component6.elementtype - 155)) != null)
                    {
                        GameObject gameObject4 = UnityEngine.Object.Instantiate(areaResource.GetMapObject((int)(component6.elementtype - 155)));
                        xpos2 = 0f;
                        ypos2 = 0f;
                        size2 = 1f;
                        switch ((Name)Area)
                        {
                            case Name.RACE1:
                                if (gameObject4.name.Contains("Race Trap Square"))
                                {
                                    size2 = 0.425f;
                                }
                                else if (gameObject4.name.Contains("Race Trap Spike"))
                                {
                                    size2 = 0.425f;
                                }
                                else if (gameObject4.name.Contains("Race1 Oneway"))
                                {
                                    size2 = 1f / MainVar.instance.TILESIZE;
                                    xpos2 = 0.5f;
                                    ypos2 = -0.65f;
                                }
                                break;
                            case Name.BASE:
                                if (gameObject4.name.Equals("Base Tevi Bag(Clone)"))
                                {
                                    ypos2 = 0f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Base Wall Switch Trigger"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Equals("Base Openable Vent(Clone)"))
                                {
                                    xpos2 = -0.49f;
                                    ypos2 = -0.48f;
                                }
                                else if (gameObject4.name.Equals("Base No Light Block(Clone)"))
                                {
                                    size2 = 6f;
                                    sizey = 3f;
                                    ypos2 = 0.37f;
                                }
                                else if (gameObject4.name.Equals("Base Static Light(Clone)"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Equals("Base Swing Light(Clone)"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Equals("Base Flick Light(Clone)"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Equals("Base Room Light(Clone)"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Equals("Sewer Ceiling Light(Clone)"))
                                {
                                    ypos2 = -0.4f;
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Equals("Sewer Wall Well Light(Clone)"))
                                {
                                    xpos2 = 0.5f;
                                    zpos2 = -2f;
                                    size2 = 0.035f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect Outpost"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 13.214286f;
                                }
                                else if (gameObject4.name.Equals("1x2 EmptyHitbox (BaseStart)(Clone)"))
                                {
                                    size2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Sewer Boss Room Floor"))
                                {
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.CLIFF:
                                if (gameObject4.name.Contains("Zema House B1 Light"))
                                {
                                    ypos2 = 0.4f;
                                    zpos2 = -1.5f;
                                    size2 = -1f;
                                }
                                else if (gameObject4.name.Contains("ZemaHouse 2F BG"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                    size2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 13.214286f;
                                }
                                else if (gameObject4.name.Contains("Bed Difficulty Setting"))
                                {
                                    ypos2 = -0.55f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Chap 8 Change"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Map1 Grave"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.FORBIDDEN:
                                if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.OASIS:
                                if (gameObject4.name.Equals("Air Camera(Clone)"))
                                {
                                    ypos2 = -0.4825f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.MOROSE:
                                if (gameObject4.name.Contains("PK Office Lunessa"))
                                {
                                    xpos2 = -0.5f;
                                    ypos2 = -0.5f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("M_Airshop"))
                                {
                                    xpos2 += 0.5f;
                                    ypos2 += -0.5f;
                                }
                                break;
                            case Name.MINE:
                                if (gameObject4.name.Contains("Big Rock"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -0.49f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Small Rock"))
                                {
                                    xpos2 = 0f;
                                    ypos2 = 0f;
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Mine Light 1"))
                                {
                                    xpos2 = -0.125f;
                                    ypos2 = 0.3f;
                                    zpos2 = -1f;
                                }
                                break;
                            case Name.LAVACAVE:
                                if (gameObject4.name.Contains("LavaCave Ember"))
                                {
                                    size2 = 32f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                else if (gameObject4.name.Contains("Platform 1x"))
                                {
                                    size2 = 1f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("IceBlockArea"))
                                {
                                    size2 = 0.017857f;
                                }
                                break;
                            case Name.RUINS:
                                if (gameObject4.name.Contains("Ruins Statue"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                    size2 = 1f;
                                }
                                break;
                            case Name.FOREST:
                            case Name.UNDERWATER:
                            case Name.FORGOTTENCITY:
                            case Name.LAVASTAGE:
                                if (gameObject4.name.Contains("Platform 1x"))
                                {
                                    size2 = 1f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Forest Maze Completed Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Water Flow Type"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("Lava Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                else if (gameObject4.name.Contains("Swamp Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect Beach"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 3.5714285f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect SnowBeach"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 8.928572f;
                                }
                                else if (gameObject4.name.Contains("Map13 Underwater Device Fixed"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("NVBoids Fish Tevi"))
                                {
                                    size2 = 0.05f;
                                    sizez = 0.05f;
                                }
                                break;
                            case Name.VALHALLASTAGE:
                            case Name.HEAVENVALLEY:
                                if (gameObject4.name.Contains("Platform 1x"))
                                {
                                    size2 = 1f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                break;
                            case Name.SNOWBASE:
                                if (gameObject4.name.Contains("SnowBase Gate"))
                                {
                                    size2 = 1f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Base Wall Switch Trigger"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Jez Stage Light"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("RibauldSpawnPoint"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Base No Light Block"))
                                {
                                    size2 = 6f;
                                    sizey = 3f;
                                    ypos2 = 0.37f;
                                }
                                else if (gameObject4.name.Contains("Base Static Light"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Base Swing Light"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Base Flick Light"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Base Room Light"))
                                {
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Sewer Ceiling Light"))
                                {
                                    ypos2 = -0.4f;
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Sewer Wall Well Light"))
                                {
                                    xpos2 = 0.5f;
                                    zpos2 = -2f;
                                    size2 = 0.035f;
                                }
                                else if (gameObject4.name.Contains("IceBlockArea"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("SnowBase BattleWall"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("SnowCave Unlock 3 Mechanism"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.SNOWVEIL:
                                if (gameObject4.name.Contains("Wind Flow Type"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("IceBlockArea"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.FINALPALACE:
                                if (gameObject4.name.Contains("Water Reflect Map30 FINALPALACE"))
                                {
                                    size2 = 23f;
                                    sizey = 3f;
                                    xpos2 = 0f;
                                    ypos2 = 1.495f;
                                }
                                else if (gameObject4.name.Contains("Final Palace Enter"))
                                {
                                    size2 = 100f;
                                    sizey = 100f;
                                }
                                else if (gameObject4.name.Contains("Tahlia Fight BG"))
                                {
                                    size2 = 0.179f;
                                    sizey = 0.179f;
                                    ypos2 = 999f;
                                }
                                else if (gameObject4.name.Contains("Revenance BG Warp"))
                                {
                                    size2 = 0.33f;
                                    sizey = 0.33f;
                                    ypos2 = 999f;
                                }
                                else if (gameObject4.name.Contains("Revenance BG Blur"))
                                {
                                    size2 = 0.33f;
                                    sizey = 0.33f;
                                    ypos2 = 999f;
                                }
                                else if (gameObject4.name.Contains("FinalPalace Spine Background 1"))
                                {
                                    size2 = 3f;
                                    sizez = 3f;
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("FinalPalace Spine Background 2"))
                                {
                                    size2 = 3f;
                                    sizez = 3f;
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("FinalPalace Spine Background 3"))
                                {
                                    size2 = 3f;
                                    sizez = 3f;
                                    xpos2 = 0.5f;
                                    ypos2 = 2.5f;
                                }
                                else if (gameObject4.name.Contains("Final Boss Last Platform"))
                                {
                                    size2 = 32f;
                                    sizey = 1f;
                                }
                                else if (gameObject4.name.Contains("Illusion Zema House BG"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -1.02f;
                                    size2 = 0.525f;
                                    sizey = 0.525f;
                                }
                                else if (gameObject4.name.Contains("Final Palace Transition"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -2.02f;
                                    size2 = 0.012f;
                                }
                                else if (gameObject4.name.Contains("Reven Wind Up"))
                                {
                                    size2 = 5f;
                                }
                                break;
                            case Name.VALHALLACITY:
                                if (gameObject4.name.Contains("Unlock Cloister Stage Detect"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Before Cloister Stage Statue"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Platform 1x"))
                                {
                                    size2 = 1f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect Map20 VHALL"))
                                {
                                    size2 = 23f;
                                    sizey = 2f;
                                    xpos2 = 0.55f;
                                    ypos2 = 1.3f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect Map20 CITY"))
                                {
                                    size2 = 46f;
                                    sizey = 5f;
                                    xpos2 = 0.55f;
                                    ypos2 = 1.3f;
                                }
                                else if (gameObject4.name.Contains("VHall Back Effect"))
                                {
                                    size2 = 6f;
                                    xpos2 -= 0.5f;
                                }
                                else if (gameObject4.name.Contains("Valhalla All Angels"))
                                {
                                    size2 = 1f;
                                    xpos2 = -0.5f;
                                }
                                break;
                            case Name.TARTARUSCITY:
                                if (gameObject4.name.Contains("Vassago Sit"))
                                {
                                    size2 = 1f;
                                    xpos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Platform"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Sable Home Front Light"))
                                {
                                    zpos2 = -1.5f;
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                }
                                break;
                            case Name.ULVOSA:
                                if (gameObject4.name.Contains("Water Reflect"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 13.214286f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                break;
                            case Name.HEAVENGARDEN:
                                if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                else if (gameObject4.name.Contains("Portal ("))
                                {
                                    size2 = 0.0535f;
                                    sizez = 0.0535f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Back Effect"))
                                {
                                    size2 = 7.5f;
                                    xpos2 -= 0.5f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect HeavenGarden"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 3.5714285f;
                                }
                                else if (gameObject4.name.Contains("Falling Light Pink"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("heavengarden_specialtree"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Platform Half Oneway Right"))
                                {
                                    size2 = 1f;
                                    sizey = 1f;
                                    sizey = 1f;
                                    xpos2 = 0f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Platform Half Oneway Left"))
                                {
                                    size2 = 1f;
                                    sizey = 1f;
                                    sizey = 1f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Amary Temp Platform 1"))
                                {
                                    size2 = 0.03571f;
                                    ypos2 = 0.4f;
                                    xpos2 = -0.05f;
                                }
                                else if (gameObject4.name.Contains("Amary Temp Platform 2"))
                                {
                                    size2 = 0.03571f;
                                    ypos2 = 0.4f;
                                    xpos2 = -0.05f;
                                }
                                break;
                            case Name.SNOWCAVE:
                                if (gameObject4.name.Contains("Portal ("))
                                {
                                    size2 = 0.0535f;
                                    sizez = 0.0535f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Wind Flow Type"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("IceBlockArea"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                else if (gameObject4.name.Contains("MCave Spine Background 1"))
                                {
                                    size2 = 3f;
                                    sizez = 3f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("MCave Spine Background 2"))
                                {
                                    size2 = 3f;
                                    sizez = 3f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("TeviB Fight BG"))
                                {
                                    size2 = 0.179f;
                                    sizey = 0.179f;
                                }
                                break;
                            case Name.WASTELAND:
                                if (gameObject4.name.Contains("Trap Elec"))
                                {
                                    size2 = -1f;
                                }
                                break;
                            case Name.ANATHEMA:
                                if (gameObject4.name.Contains("Ana Blacksmith Shop Flick"))
                                {
                                    zpos2 = -1.5f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0f;
                                }
                                else if (gameObject4.name.Contains("Library Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.GALLERY:
                                if (gameObject4.name.Contains("A_Gallery Base Lamp"))
                                {
                                    zpos2 = -1f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0.3f;
                                }
                                else if (gameObject4.name.Contains("A_Gallery Event Lamp"))
                                {
                                    zpos2 = -1f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0.3f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Flick Light Gallery"))
                                {
                                    zpos2 = -1.5f;
                                    xpos2 = -0.5f;
                                    ypos2 = 0f;
                                }
                                else if (gameObject4.name.Contains("Green Barrier"))
                                {
                                    size2 = 0.035f;
                                    sizey = 0.035f;
                                }
                                else if (gameObject4.name.Contains("Platform "))
                                {
                                    size2 = 1f;
                                    ypos2 = 0f;
                                }
                                else if (gameObject4.name.Contains("Ground Fire"))
                                {
                                    size2 = 60f;
                                    sizey = 60f;
                                    sizez = 60f;
                                }
                                else if (gameObject4.name.Contains("Fall Damage Area Effect"))
                                {
                                    size2 = 0.178f;
                                    sizez = 0.178f;
                                }
                                break;
                            case Name.CATACOMBS:
                                if (gameObject4.name.Contains("Catacombs Light 1"))
                                {
                                    xpos2 = -0.125f;
                                    ypos2 = 0.3f;
                                    zpos2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Portal ("))
                                {
                                    size2 = 0.0535f;
                                    sizez = 0.0535f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Seal Queen"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0.53f;
                                    ypos2 = -0.5f;
                                }
                                else if (gameObject4.name.Contains("Red Lab Ember"))
                                {
                                    size2 = 32f;
                                }
                                else if (gameObject4.name.Contains("LabLaser"))
                                {
                                    size2 = 0.017857f;
                                    sizez = size2;
                                }
                                else if (gameObject4.name.Contains("Lab Rail"))
                                {
                                    size2 = 0.035714f;
                                    sizez = size2;
                                }
                                break;
                            case Name.TRAIN:
                                if (gameObject4.name.Contains("TrainLaser"))
                                {
                                    size2 = 0.017857f;
                                    sizez = size2;
                                }
                                else if (gameObject4.name.Contains("Water Reflect Island"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 3.5714285f;
                                }
                                else if (gameObject4.name.Contains("TrainBarrier"))
                                {
                                    size2 = 0.035f;
                                    sizey = 0.035f;
                                }
                                break;
                            case Name.CLOISTERSTAGE:
                                if (gameObject4.name.Contains("Embers Tybr"))
                                {
                                    size2 = 1f;
                                    sizez = 1f;
                                    sizey = 1f;
                                }
                                else if (gameObject4.name.Contains("cloisterfight_deadbody_b3"))
                                {
                                    size2 = 1f;
                                    xpos2 = 0.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Prison BG"))
                                {
                                    xpos2 = 0.5f;
                                    ypos2 = -0.5f;
                                    size2 = -1f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                break;
                            case Name.PLAGUE:
                                if (gameObject4.name.Contains("Malphage Tentacle"))
                                {
                                    size2 = 0.017857f;
                                }
                                else if (gameObject4.name.Contains("Free Roam Only Platform"))
                                {
                                    xpos2 = 0.5f;
                                    size2 = 1f / 56f;
                                }
                                else if (gameObject4.name.Contains("Platform 1x"))
                                {
                                    size2 = 1f;
                                    ypos2 = 0f;
                                }
                                break;
                            case Name.INTRO:
                                if (gameObject4.name.Contains("Intro Map Flame"))
                                {
                                    size2 = 1f;
                                }
                                else if (gameObject4.name.Contains("Water Reflect"))
                                {
                                    size2 = 23.214285f;
                                    sizey = 13.214286f;
                                }
                                else if (gameObject4.name.Contains("Intro Door"))
                                {
                                    size2 = 4f;
                                    xpos2 = 1.5f;
                                    ypos2 = 0.5f;
                                }
                                else if (gameObject4.name.Contains("Chap0 Intro Line"))
                                {
                                    size2 = 1f / 56f;
                                }
                                break;
                        }
                        wm.FinalCreateObject(gameObject4, xpos2, ypos2, zpos2, size2, sizey, sizez, gameObject.transform);
                        component6.RenameText(gameObject4.name, num2);
                    }
                    else
                    {
                        Debug.Log("[WorldManger] Warning! " + component6.elementtype.ToString() + " do not exist in resource.");
                    }
                }
                component6._ElementStart(num2);
              
            }

            if (flag2)
            {
                boxCollider2D.enabled = false;
                if ((bool)edgeCollider2D && edgeCollider2D.enabled)
                {
                    WorldManager.Instance.areadata.SetHitBox(x, y, byte.MaxValue);
                }
            }
            Color color = spriteRenderer.color;

            color.a = 0f;
            spriteRenderer.enabled = false;
            gameObject.GetComponent<TextMeshPro>().enabled = false;
            WorldManager.Instance.areadata.tilelist.Add(tileData);
        }

                
        public static void createBackGroundTile(int x, int y, int spriteID,int Area = -1)
        {
            bool flipH = false;
            bool flipV = false;
            float TILESIZE = MainVar.instance.TILESIZE;
            Layer layer = Layer.BACKDROP;
            WorldManager wm = WorldManager.Instance;
            if (Area == -1) Area = WorldManager.Instance.Area;

            AreaResource areaResource = ResourcePatch.getAreaResource(Area);
            if (x < 0 || x < 0 || x >= MainVar.instance.MaxTileX || y >= MainVar.instance.MaxTileY)
            {
                return;
            }
            if (Application.isPlaying && WorldManager.Instance.FindTile(x, y, layer) != null)
            {
                {
                    Debug.Log("[WorldManager] Warning! File have duplicated tiles in same position! (" + (float)x * TILESIZE + "," + (float)y * (0f - TILESIZE) + ")");
                    return;
                }
            }

            Traverse worldmng = Traverse.Create(WorldManager.Instance);
            GameObject gameObject = UnityEngine.Object.Instantiate(worldmng.Field("backdrop_prefab").GetValue<GameObject>());                          //Create new Item
            //gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            BoxCollider2D boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
            EdgeCollider2D edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();

            gameObject.transform.SetParent(worldmng.Field("TileHolder").GetValue<GameObject>().transform);

            WorldManager.TileData tileData = setUpTileData(x, y, spriteID, flipH, flipV, layer);

            float num5 = (float)x * MainVar.instance.TILESIZE;
            Vector3 localPosition = new Vector3(num5, (float)(-y) * MainVar.instance.TILESIZE + MainVar.instance.TILESIZE / 2f, 1f);
            if (layer == Layer.BACKDROP)
            {
                localPosition.z += (float)x / MainVar.instance.TILESIZE * 0.1f + (float)y / MainVar.instance.TILESIZE * 0.0001f;
            }
            gameObject.transform.localPosition = localPosition;


            TileLink component = gameObject.GetComponent<TileLink>();
            SpriteRenderer spriteRenderer = null;
            spriteRenderer = (component ? component.sprite_prefab : gameObject.GetComponent<SpriteRenderer>());
            spriteRenderer.material = CommonResource.Instance.mat_SpriteLighting;
            spriteRenderer.enabled = false;
            //backdrop stuff

            spriteRenderer.sprite = areaResource.GetBackdrop(spriteID);
            Vector3 position = spriteRenderer.transform.position;
            position.y += TILESIZE * -0.5f;
            spriteRenderer.transform.position = position;
            if (EditorManager.instance.Editor_LayerSelected == Layer.BACKDROP)
            {
                gameObject.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
            }
            int num10 = -1;
            float zpos = 0f;
            float ypos = 0f;
            float xpos = 0f;
            float size = -1f;
            string text = "";
            bool flag5 = false;
            if (spriteRenderer.sprite != null)
            {
                if (spriteRenderer.sprite.name.Contains("_f1") || spriteRenderer.sprite.name.Contains("_f2") || spriteRenderer.sprite.name.Contains("_f3") || spriteRenderer.sprite.name.Contains("_f4"))
                {
                    flag5 = true;
                }
                ScrollingBackdrop component3 = gameObject.GetComponent<ScrollingBackdrop>();
                if (flag5)
                {
                    component3.SetSR(spriteRenderer);
                }
            }
            if (text.Length > 2)
            {
                num10 = 99999;
            }
            if (num10 >= 0)
            {
                GameObject gameObject2 = null;
                gameObject2 = ((num10 != 99999) ? UnityEngine.Object.Instantiate(worldmng.Field("elementObjects").GetValue<GameObject[]>()[num10]) : UnityEngine.Object.Instantiate(AreaResource.Instance.GetMapObjectByName(text)));
                wm.FinalCreateObject(gameObject2, xpos, ypos, zpos, size, -1f, 1f, gameObject.transform);
            }
            if (spriteRenderer.sprite != null && spriteRenderer.sprite.name.Contains("_a_"))
            {
                AnimatedBackdrop animatedBackdrop = areaResource.GetAnimatedBackdrop(spriteRenderer.sprite.name);
                AnimatedBackdrop animatedBackdrop2 = gameObject.AddComponent<AnimatedBackdrop>();
                if ((bool)animatedBackdrop2 && (bool)animatedBackdrop)
                {
                    animatedBackdrop2.CloneData(animatedBackdrop);
                }
            }
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }

            //BackDrop end
            switch (layer) {
                case Layer.BACKDROP:
            spriteRenderer.sortingOrder = 1200;                                                                                                         //Normal Layer
                    {
                        spriteRenderer.sortingOrder = 20;
                        if (spriteRenderer.sprite == null)
                        {
                            spriteRenderer.sprite = areaResource.GetBackdrop(1);
                        }
                        if (spriteRenderer.sprite.name.Contains("_f1") || spriteRenderer.sprite.name.Contains("_f2") || spriteRenderer.sprite.name.Contains("_f3") || spriteRenderer.sprite.name.Contains("_f4") || spriteRenderer.sprite.name.Contains("_f5") || spriteRenderer.sprite.name.Contains("_f6"))
                        {
                            if (spriteRenderer.sprite.name.Contains("_f1"))
                            {
                                spriteRenderer.sortingOrder = 406;
                            }
                            if (spriteRenderer.sprite.name.Contains("_f2"))
                            {
                                spriteRenderer.sortingOrder = 407;
                            }
                            if (spriteRenderer.sprite.name.Contains("_f3"))
                            {
                                spriteRenderer.sortingOrder = 408;
                            }
                            if (spriteRenderer.sprite.name.Contains("_f4"))
                            {
                                spriteRenderer.sortingOrder = 409;
                            }
                            if (spriteRenderer.sprite.name.Contains("_f5"))
                            {
                                spriteRenderer.sortingOrder = 410;
                            }
                            if (spriteRenderer.sprite.name.Contains("_f6"))
                            {
                                spriteRenderer.sortingOrder = 411;
                            }
                            spriteRenderer.gameObject.layer = 31;
                            ScrollingBackdrop component9 = gameObject.GetComponent<ScrollingBackdrop>();
                            component9.t = component9.transform;
                            wm.areadata.scrolllist.Add(component9);
                            break;
                        }
                        ScrollingBackdrop component10 = gameObject.GetComponent<ScrollingBackdrop>();
                        if (!Application.isPlaying)
                        {
                            UnityEngine.Object.DestroyImmediate(component10);
                        }
                        else
                        {
                            UnityEngine.Object.Destroy(component10);
                        }
                        if (spriteRenderer.sprite.name.Contains("_light20"))
                        {
                            spriteRenderer.material = CommonResource.Instance.mat_SpriteAdditiveFull;
                            Color color3 = spriteRenderer.color;
                            color3.a = 0.2f;
                            spriteRenderer.color = color3;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_light25"))
                        {
                            spriteRenderer.material = CommonResource.Instance.mat_SpriteAdditiveFull;
                            Color color4 = spriteRenderer.color;
                            color4.a = 0.25f;
                            spriteRenderer.color = color4;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_light50"))
                        {
                            spriteRenderer.material = CommonResource.Instance.mat_SpriteAdditiveFull;
                            Color color5 = spriteRenderer.color;
                            color5.a = 0.5f;
                            spriteRenderer.color = color5;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_light75"))
                        {
                            spriteRenderer.material = CommonResource.Instance.mat_SpriteAdditiveFull;
                            Color color6 = spriteRenderer.color;
                            color6.a = 0.75f;
                            spriteRenderer.color = color6;
                        }
                        if (spriteRenderer.sprite.name.Contains("_t1"))
                        {
                            spriteRenderer.sortingOrder = 52;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b7"))
                        {
                            spriteRenderer.sortingOrder = 4;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b6"))
                        {
                            spriteRenderer.sortingOrder = 5;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b5"))
                        {
                            spriteRenderer.sortingOrder = 14;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b4"))
                        {
                            spriteRenderer.sortingOrder = 16;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b3"))
                        {
                            spriteRenderer.sortingOrder = 17;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b2"))
                        {
                            spriteRenderer.sortingOrder = 18;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_b1"))
                        {
                            spriteRenderer.sortingOrder = 19;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_l1"))
                        {
                            spriteRenderer.sortingOrder = 21;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_l2"))
                        {
                            spriteRenderer.sortingOrder = 22;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_l3"))
                        {
                            spriteRenderer.sortingOrder = 23;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_l4"))
                        {
                            spriteRenderer.sortingOrder = 24;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_c1"))
                        {
                            spriteRenderer.sortingOrder = 401;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_c2"))
                        {
                            spriteRenderer.sortingOrder = 402;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_c3"))
                        {
                            spriteRenderer.sortingOrder = 403;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_c4"))
                        {
                            spriteRenderer.sortingOrder = 404;
                        }
                        else if (spriteRenderer.sprite.name.Contains("_c5"))
                        {
                            spriteRenderer.sortingOrder = 405;
                        }
                        else if (spriteRenderer.sprite.name.Contains("crack"))
                        {
                            wm.areadata.backdrop.Add(spriteRenderer);
                            spriteRenderer.sortingOrder = 52;
                        }
                        if (spriteRenderer.sortingOrder >= 300)
                        {
                            spriteRenderer.gameObject.layer = 31;
                        }
                        else
                        {
                            spriteRenderer.gameObject.layer = 30;
                        }

                    }
                    break;
            }
            WorldManager.Instance.areadata.tilelist.Add(tileData);
            spriteRenderer.enabled = true;
        }


        private static void customRoom(WorldManager.MapData customroom)
        {
            WorldManager.MapData data = WorldManager.Instance.areadata.maplist.Find(room => room.y == customroom.y && room.x == customroom.x);
            if (data == default)
            {
                Traverse traverse = new Traverse(WorldManager.Instance);
                object[] array3 = new object[] { customroom.x, customroom.y, customroom.areatype, customroom.bgtype, customroom.waterlevel, customroom.suntype, customroom.camerafixTopY };
                traverse.Method("CreateMapData", array3).GetValue();
            }
            data.y = customroom.y;
            data.x = customroom.x;
            data.roomtype = customroom.roomtype;
            data.areatype = customroom.areatype;
            data.waterlevel = customroom.waterlevel;
            data.bgtype = customroom.bgtype;
            data.suntype = customroom.suntype;
            data.camerafixTopY = customroom.camerafixTopY;

            var a = MainVar.instance.MAPSIZEX * MainVar.instance.MAPSIZEY * WorldManager.Instance.Area + customroom.y * MainVar.instance.MAPSIZEX + customroom.x;

            Traverse fullMap = new Traverse(FullMap.Instance);
            fullMap.Method("SetRoomData", (object[])[WorldManager.Instance.Area, customroom.x, customroom.y, customroom.roomtype, customroom.areatype]).GetValue();
            var tile = fullMap.Method("CreateRoomTile", (object[])[a]).GetValue<FullMapTile>();
            Color32 color = new Color32(255,255,255,255);
            tile.SetColor(color);

            WorldManager.Instance.ResetRoomListCache();
            WorldManager.Instance.UpdateRoomListCache();
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
            loadMap();
            Traverse t = Traverse.Create(__instance);
            if (WorldManager.Instance.Area == 8)
            {
                createItemTile(334, 205, 20, false, false);
            }
            t.Field("breakingtile").Method("Clear").GetValue();
            if(ArchipelagoInterface.Instance.isConnected)
            {
                ArchipelagoInterface.Instance.updateCurretMap(__instance.Area);
            }

            //Send AP World Transition Data and change CurrentMap Value
        }

        [HarmonyPatch(typeof(ChangeMapTrigger), "OnBecameVisible")]
        [HarmonyPrefix]
        static bool switchTargetMap(ref byte ___targetPosID, ref byte ___targetArea, ref bool ___gotData, ref BoxCollider2D ___box, ref ChangeMapTrigger __instance)
        {
            if (RandomizerPlugin.transitionData == null) return true;
            Debug.Log($"Before ID:{___targetPosID} Area:{___targetArea}");
            byte targetPos = (byte)(EventManager.Instance.GetElmData(__instance.transform, 0f, -56f) - 72);
            byte area = WorldManager.Instance.Area;
            if ((area == 20 && targetPos == 2) || (area == 29 && targetPos == 3))
                targetPos = (byte)(targetPos ^ 1);
            int targetMapAndID = WorldManager.Instance.Area * 100 + targetPos;
            if (!___gotData && RandomizerPlugin.transitionData.ContainsKey(targetMapAndID))
            {
                ___targetArea = (byte)(EventManager.Instance.GetElmData(__instance.transform, 0f, 56f) - 72);
                ___targetPosID = (byte)(EventManager.Instance.GetElmData(__instance.transform, 0f, -56f) - 72);
                int val = RandomizerPlugin.transitionData[targetMapAndID];

                // Save New Trasition to a static variable to be used when the player changes the Map
                ___gotData = true;
                if (___targetArea == 14 && ___targetPosID == 4)
                {
                    ___box.offset = new Vector2(0f, -85f);
                    ___box.size = new Vector2(56f, 240f);
                }
                ___targetPosID = (byte)(val % 100);
                ___targetArea = (byte)(val / 100);
            }
            Debug.Log($"After ID:{___targetPosID} Area:{___targetArea}");
            return true;

        }

        [HarmonyPatch(typeof(ChangeMapTrigger), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        static void addTransitionInfo(ref Collider2D col,ref byte ___targetArea,ref byte ___targetPosID)
        {

            if (!col.name.Equals("Event Detect") || EventManager.Instance.isBossMode())
            {
                return;
            }
            if (GameSystem.Instance.isGameOver() > 0f || EventManager.Instance.mainCharacter.health <= 0)
            {
                return;
            }
            if (EventManager.Instance.IsChangingMap())
            {
                return;
            }
            if (EventManager.Instance.getMode() != 0 && EventManager.Instance.DisableMapChangeTrigger)
            {
                return;
            }
            int targetMapAndID = ___targetArea * 100 + ___targetPosID;
            if (ArchipelagoInterface.Instance.isConnected)
            {
                if (!RandomizerPlugin.transitionVisited.Contains(targetMapAndID))
                {
                    RandomizerPlugin.transitionVisited.Add(targetMapAndID);
                    ArchipelagoInterface.Instance.updateTransitionVisited(RandomizerPlugin.transitionVisited.ToArray());
                }
            }
        }
        static bool loadingCustomMap = false;

        public static void loadMap()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte area = WorldManager.Instance.Area;
            string text = $"{RandomizerPlugin.pluginPath}/CustomMaps/CustomMap{area}.dat";
            if (File.Exists(text))
            {
                loadingCustomMap = true;
                FileStream fileStream = File.Open(text, FileMode.Open);
                List<WorldManager.TileData> tmpRemovedTile = (List<WorldManager.TileData>)binaryFormatter.Deserialize(fileStream);
                List<WorldManager.TileData> tmpAddedTile = (List<WorldManager.TileData>)binaryFormatter.Deserialize(fileStream);
                List<WorldManager.MapData> tmpRooms = (List<WorldManager.MapData>)binaryFormatter.Deserialize(fileStream);
                float TILESIZE = MainVar.instance.TILESIZE;
                foreach (WorldManager.TileData tileData in tmpRemovedTile)
                {
                    GameObject[] array = GameObject.FindGameObjectsWithTag("TileSprite");
                    foreach (GameObject gameObject in array)
                    {

                        float num4 = gameObject.transform.position.x / TILESIZE;
                        if (tileData.layer == Layer.BACKDROP && gameObject.transform.localEulerAngles.y == 180f)
                        {
                            num4 += 1f;
                        }
                        if ((int)(num4 - 0.5f) == tileData.x && (int)(-1 * (gameObject.transform.position.y / TILESIZE) - 0.5f) == tileData.y - 1 && gameObject.name.Equals("SPR"))
                        {
                            bool flag = false;
                            SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
                            if (component != null)
                            {

                                if (tileData.layer == Layer.BACK1 && (component.sortingOrder == 30 || component.sortingOrder == 9))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.BACKDROP && (component.sortingOrder == 305 || component.sortingOrder == 46 || component.sortingOrder == 52 || (component.sortingOrder >= 401 && component.sortingOrder <= 411) || component.sortingOrder == 4 || component.sortingOrder == 5 || (component.sortingOrder >= 10 && component.sortingOrder <= 24 && component.sortingOrder != 15)))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.BACK2 && (component.sortingOrder == 35 || component.sortingOrder == 8))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.BACK3 && (component.sortingOrder == 39 || component.sortingOrder == 7))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.BACK4 && (component.sortingOrder == 42 || component.sortingOrder == 6))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.FRONT1 && (component.sortingOrder == 225 || component.sortingOrder == 60))
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.NORMAL && component.sortingOrder == 45)
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.ITEM && component.sortingOrder == 44)
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.ENEMY && component.sortingOrder == 1000)
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.EVENT && component.sortingOrder == 1100)
                                {
                                    flag = true;
                                }
                                else if (tileData.layer == Layer.ELEMENT && component.sortingOrder == 1200)
                                {
                                    flag = true;
                                }
                                if (flag)
                                {
                                    WorldManager.Instance.TryDestoryTile(tileData.x, tileData.y, gameObject.transform.parent.gameObject, true, tileData.layer);
                                    break;
                                }
                            }
                        }
                    }

                }
                Traverse t = new Traverse(WorldManager.Instance);
                foreach (WorldManager.TileData tileData in tmpAddedTile)
                {
                    object[] obj = [tileData.x, tileData.y, tileData.spriteID, tileData.flipH, tileData.flipV, tileData.layer];

                    t.Method("CreateTile", obj).GetValue();


                }
                foreach (WorldManager.MapData room in tmpRooms)
                {
                    customRoom(room);
                }
                fileStream.Close();
                //WorldManager.Instance.areadata.SetTilePixelLighting();
                //WorldManager.Instance.ToogleTileMode(true);
                //WorldManager.Instance.ToogleTileMode(false);
                loadingCustomMap = false;
            }

        }

        [HarmonyPatch(typeof(WorldManager), "CreateTile")]
        [HarmonyPostfix]
        private static void fetchNewTile(ref GameObject ___TileHolder, ref bool __result)
        {
            if (__result && loadingCustomMap)
            {
                GameObject newObject = ___TileHolder.transform.GetChild(___TileHolder.transform.childCount-1).gameObject;
                var renderer = newObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.enabled = false;
                TextMeshPro text = newObject.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.enabled = false;
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
