using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using HarmonyLib;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine.ResourceManagement.ResourceLocations;
using Bullet;
using Map;
using System.Linq;
using static TeviRandomizer.Extras;


namespace TeviRandomizer
{
    class ResourcePatch
    {
        static public List<ObjectPooler> pooler = new List<ObjectPooler>();
        static public Scene ResourceScene;
        static public AreaResource AreaResource;
        static public GameObject[] resources = [];
        static Harmony PatchResources = new Harmony("ResourcePatch");
        static bool enabled = false;

        static public void patchResources(bool enable = false)
        {
            if (enable)
            {
                if (!enabled)
                {
                    PatchResources.PatchAll(typeof(ResourcePatch));
                    Debug.Log("Resource Patch Enabled");
                    enabled = true;
                }
            }
            else
            {
                PatchResources.UnpatchSelf();
                Debug.Log("Resource Patch Disabled");
                enabled = false;
            }
        }


        static public bool AddressableResourceExists(string key)
        {
            foreach (var l in Addressables.ResourceLocators)
            {
                if (l.Keys.Contains(key))
                    return true;
            }
            return false;
        }


        static async Task getAllResources()
        {


            ResourceScene = SceneManager.CreateScene("AllResource");
            GameObject newObj = new GameObject("Resource");
            AreaResource = newObj.AddComponent<AreaResource>();
            //newObj.AddComponent<Resources>();
            SceneManager.MoveGameObjectToScene(newObj, ResourceScene);

            List<AsyncOperationHandle<SceneInstance>> sceneHandle = new List<AsyncOperationHandle<SceneInstance>>();
            for (int i = 0; i < MainVar.instance.MAXAREA - 2; i++)
            {
                if (AddressableResourceExists($"resource{i}"))
                {
                    Debug.Log($"[Tevi Randomizer] Loading Resources from Map {i}");
                    sceneHandle.Add(Addressables.LoadSceneAsync($"resource{i}", LoadSceneMode.Additive));
                }


            }
            List<GameObject> rootScene = new List<GameObject>();
            //Object.DontDestroyOnLoad(newObj);
            foreach(AsyncOperationHandle<SceneInstance> SceneInstance in sceneHandle)
            {
                while (!SceneInstance.IsDone)
                {
                    await Task.Yield();
                }
                if (SceneInstance.Result.Scene.IsValid())
                {
                    rootScene.Add(SceneInstance.Result.Scene.GetRootGameObjects()[0]);
                    AreaResource tmp = SceneInstance.Result.Scene.GetRootGameObjects()[0].GetComponent<AreaResource>();

                    if (AreaResource.debris1 == null && tmp.debris1.Length >0)
                        AreaResource.debris1 = tmp.debris1;

                    if (AreaResource.debris2 == null && tmp.debris2.Length >0)
                        AreaResource.debris2 = tmp.debris2;

                    if (AreaResource.electricity1 == null && tmp.electricity1.Length >0)
                        AreaResource.electricity1 = tmp.electricity1;

                    if (AreaResource.electricity2 == null && tmp.electricity2.Length >0)
                        AreaResource.electricity2 = tmp.electricity2;

                    if (AreaResource.electricity3 == null && tmp.electricity3.Length >0)
                        AreaResource.electricity3 = tmp.electricity3;

                    if (AreaResource.electricity4 == null && tmp.electricity4.Length >0)
                        AreaResource.electricity4 = tmp.electricity4;

                    if (AreaResource.energy1 == null && tmp.energy1.Length >0)
                        AreaResource.energy1 = tmp.energy1;

                    if (AreaResource.energy2 == null && tmp.energy2.Length >0)
                        AreaResource.energy2 = tmp.energy2;

                    if (AreaResource.explosionBlue == null && tmp.explosionBlue.Length >0)
                        AreaResource.explosionBlue = tmp.explosionBlue;

                    if (AreaResource.flame1 == null && tmp.flame1.Length >0)
                        AreaResource.flame1 = tmp.flame1;

                    if (AreaResource.flame2 == null && tmp.flame2.Length >0)
                        AreaResource.flame2 = tmp.flame2;

                    if (AreaResource.flame3 == null && tmp.flame3.Length >0)
                        AreaResource.flame3 = tmp.flame3;

                    if (AreaResource.flame4 == null && tmp.flame4.Length >0)
                        AreaResource.flame4 = tmp.flame4;

                    if (AreaResource.flame5 == null && tmp.flame5.Length >0)
                        AreaResource.flame5 = tmp.flame5;

                    if (AreaResource.flame6 == null && tmp.flame6.Length >0)
                        AreaResource.flame6 = tmp.flame6;

                    if (AreaResource.flash1 == null && tmp.flash1.Length >0)
                        AreaResource.flash1 = tmp.flash1;

                }
            }
            resources = rootScene.ToArray();
            //foreach(GameObject a in resources)
            //{
            //    Object.DontDestroyOnLoad(a);
            //}
            //Resources.resources = resources;
        }


        static private void addEffectsToArea(ref AreaResource tmp)
        {

            if (AreaResource.debris1 != null && tmp.debris1.Length == 0)
               tmp.debris1 = AreaResource.debris1;

            if (AreaResource.debris2 != null && tmp.debris2.Length == 0)
                tmp.debris2 = AreaResource.debris2;

            if (AreaResource.electricity1 != null && tmp.electricity1.Length == 0)
                tmp.electricity1 = AreaResource.electricity1;

            if (AreaResource.electricity2 != null && tmp.electricity2.Length == 0)
               tmp.electricity2 = AreaResource.electricity2;

            if (AreaResource.electricity3 != null && tmp.electricity3.Length == 0)
                tmp.electricity3 = AreaResource.electricity3;

            if (AreaResource.electricity4 != null && tmp.electricity4.Length == 0)
                tmp.electricity4 = AreaResource.electricity4;

            if (AreaResource.energy1 != null && tmp.energy1.Length == 0)
               tmp.energy1 = AreaResource.energy1 ;

            if (AreaResource.energy2 != null && tmp.energy2.Length == 0)
               tmp.energy2 = AreaResource.energy2;

            if (AreaResource.explosionBlue != null && tmp.explosionBlue.Length == 0)
                tmp.explosionBlue = AreaResource.explosionBlue;

            if (AreaResource.flame1 != null && tmp.flame1.Length == 0)
                tmp.flame1 = AreaResource.flame6;

            if (AreaResource.flame2 != null && tmp.flame2.Length == 0)
                tmp.flame2 = AreaResource.flame6;

            if (AreaResource.flame3 != null && tmp.flame3.Length == 0)
                tmp.flame3 = AreaResource.flame6;

            if (AreaResource.flame4 != null && tmp.flame4.Length == 0)
                tmp.flame4 = AreaResource.flame6;

            if (AreaResource.flame5 != null && tmp.flame5.Length == 0)
                tmp.flame5 = AreaResource.flame6;

            if (AreaResource.flame6 != null && tmp.flame6.Length == 0)
                tmp.flame6 = AreaResource.flame6;

            if (AreaResource.flash1 != null && tmp.flash1.Length == 0)
                tmp.flash1 = AreaResource.flash1;
        }


        static public AreaResource getAreaResource(int area)
        {
            AreaResource tmp = resources[area].GetComponent<AreaResource>();

            if (resources != null && resources.Length >area)
            {
                addEffectsToArea(ref tmp);
                return tmp;
            }
            return AreaResource;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AreaResource),"StartMe")]
        static bool skipSkeleton(ref SkeletonDataAsset[] ___spines)
        {
            if(___spines == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetNPC", new[] { typeof(string) })]
        static void getNPC(ref RuntimeAnimatorController __result,ref string name, ref AreaResource __instance)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetNPC(name);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetObjectSprite", new[] { typeof(string) })]
        static void gObjectSprite(ref Sprite __result,ref string name,ref AreaResource __instance)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetObjectSprite(name);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetAudioByName", new[] { typeof(string) })]
        static void getSound(ref AudioClip __result,ref string sname,ref AreaResource __instance)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetAudioByName(sname);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetSpine", new[] { typeof(string) })]
        static void GetSpine(ref SkeletonDataAsset __result,ref string sname,ref AreaResource __instance)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetSpine(sname);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetSkyBackground", new[] { typeof(string) })]
        static void getBG(ref AreaResource __instance,ref SkyBackground __result, ref string bgname)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetSkyBackground(bgname);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetBackdrop", new[] { typeof(string) })]
        static void GetBackdrop(ref AreaResource __instance,ref Sprite __result, ref string name)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {

                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetBackdrop(name);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetCG", new[] { typeof(string) })]
        static void GetCG(ref AreaResource __instance,ref Sprite __result, ref string name)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetCG(name);
                    if (__result != null) return ;
                }
                return ;
            }
            return ;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetMaterialByName")]
        static void GetMaterial(ref AreaResource __instance, ref Material __result, ref string name)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetMaterialByName(name);
                    if (__result != null) return;
                }
                return;
            }
            return;
        }

        [HarmonyPatch(typeof(ObjectPooler), "GetPooledObject", new[] {typeof(string) } )]
        [HarmonyPostfix]
        static void getPooledObject(ref GameObject __result,ref string search,ref ObjectPooler __instance)
        {
            if (__instance == AreaResource.Instance.AreaPooler && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area || getAreaResource(i).AreaPooler == null) continue;
                    __result = getAreaResource(i).AreaPooler.GetPooledObject(search);
                    if (__result != null) return;
                }
                return;
            }
            return;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "GetBossObjectByName")]
        static void GetBossObjectByName(ref AreaResource __instance, ref GameObject __result, ref string sname)
        {
            if (__instance == AreaResource.Instance && __result == null)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().GetBossObjectByName(sname);
                    if (__result != null) return;
                }
                return;
            }
            return;
        }
        static int BossObjectArea = -1;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AreaResource), "FindBossObjectByName")]
        static void GetBossObjectByName(ref AreaResource __instance, ref int __result, ref string sname)
        {
            if (__instance == AreaResource.Instance && __result == -1)
            {
                for (int i = 0; i < resources.Length; i++)
                {
                    if (i == WorldManager.Instance.Area) continue;
                    __result = resources[i].GetComponent<AreaResource>().FindBossObjectByName(sname);
                    if (__result != -1) {
                        BossObjectArea = i;
                        return;
                    }
                }
                return;
            }
            else if (__instance == AreaResource.Instance && __result != -1)
            {
                BossObjectArea = WorldManager.Instance.Area;
            }
            return;
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldManager), "SpawnBossObject", new[] {typeof(int),typeof(Vector3) })]
        static bool SpawnBossObject(ref WorldManager __instance, ref GameObject __result, ref Vector3 pos,ref int id)
        {
            GameObject bossObject = getAreaResource(BossObjectArea).GetBossObject(id);
            if (bossObject == null)
            {
                Debug.LogWarning("[WorldManager] No Boss Object in Map's Boss Object List : ID " + id);
                return false ;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate(bossObject);
            if (gameObject != null)
            {
                gameObject.transform.position = pos;
                __result = gameObject;
            }
            return false;
        }





        [HarmonyPrefix]
        [HarmonyPatch(typeof(AreaResource),"Awake")]
        static void overridePooler(ref ObjectPooler ___AreaPooler,ref AreaResource ___Instance, ref AreaResource __instance)
        {
            //___Instance = AreaResource;

        }
        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObjectPooler), "GetPooledObject", new[]{ typeof(string)})]
        static bool newSearch(ref GameObject __result, ref ObjectPooler __instance,ref string search)
        {
            Debug.Log($"Find Object: {search}");

            for (int x = 0; x < resources.Length && __result == null; x++)
            {


                ObjectPooler obj = resources[x].GetComponent<ObjectPooler>();
                if (obj != null)
                {
                    for (int i = 0; i < obj.itemsToPool.Count; i++)
                    {
                        if (obj.itemsToPool[i].objectToPool.name.Contains(search))
                        {
                            __result = obj.GetPooledObject(i);
                            return false;
                        }
                    }
                }
            }


            return false;
        }


        static async void loadStuff(WorldManager __instance) {
            string text = "resource";
            string text2 = text + __instance.Area;
            __instance.ResetRoomListCache();

            AsyncFastLoadHelper.FastLoad(active: true);
            AsyncOperationHandle<SceneInstance> sceneHandle;
            if(AreaResource == null)
            {
                Debug.Log("Load all Resources");
                await getAllResources();
            }
            if (__instance.Area < resources.Length)
            {
                AreaResource.Instance = getAreaResource(__instance.Area);
            }
            else
            {
                AreaResource.Instance = getAreaResource(resources.Length-1);
            }
            if(AreaResource.Instance.AreaPooler == null)
            {
                AreaResource.Instance.AreaPooler = new ObjectPooler();
            }
            text = "bakedmap";
            text2 = text + __instance.Area;
            if (!__instance.editorload.loadFromBaked && Application.isEditor)
            {
                text2 = text + (MainVar.instance.MAXAREA - 1);
            }
            Debug.Log("[WorldManager] Loading Baked Scene : " + text2, __instance.gameObject);
            sceneHandle = Addressables.LoadSceneAsync(text2, LoadSceneMode.Additive);
            while (!sceneHandle.IsDone)
            {
                await Task.Yield();
            }
            new Traverse(__instance).Method("OnSceneLoaded", new object[] { sceneHandle.Result.Scene }).GetValue();
            AsyncFastLoadHelper.FastLoad(active: false);
        }

        private static bool IsSceneLoaded(string name)
        {
            IList<IResourceLocation> list = Addressables.LoadResourceLocationsAsync(name).WaitForCompletion();
            if (list.Count > 0)
            {
                return SceneManager.GetSceneByPath(list[0].InternalId).isLoaded;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldManager),"UnloadMap")]
        static bool UnloadMap(ref WorldManager __instance)
        {
            Traverse t = new Traverse(__instance);
            __instance.MapInited = false;

            
            //t.Field("breakingtile").GetValue<List<object>>().Clear();
            t.Field("breakingtile").Property("breakingtile").Method("Clear").GetValue();
            string text = "resource";
            string text2 = text + __instance.Area;

            text = "bakedmap";
            text2 = text + __instance.Area;
            if (!__instance.editorload.loadFromBaked && Application.isEditor)
            {
                text2 = text + (MainVar.instance.MAXAREA - 1);
            }
            Debug.Log("[Unity] Try to unloaded scene : " + text2);
            if (IsSceneLoaded(text2))
            {
                SceneManager.UnloadSceneAsync(text2);
                Debug.Log("[Unity] Unloaded scene complete : " + text2);
            }
            text2 = "cutin";
            Debug.Log("[Unity] Try to unloaded scene : " + text2);
            if (IsSceneLoaded(text2))
            {
                SceneManager.UnloadSceneAsync(text2);
                Debug.Log("[Unity] Unloaded scene complete : " + text2);
                GemaUIPauseMenu_NoteBookContents.Instance.GetNotebookBossData().ResetAll();
            }
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldManager),"InitMap")]
        static bool loadResources(ref WorldManager __instance)
        {
            if (__instance.Area == 48) return true;
            loadStuff(__instance);

            return false;
        }




        // fix all Object Spirtes with numbers
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhyBManager), "SetSprite")]
        static bool fixPhyBSprites(ref phybScript es,ref SpriteType sid,ref Sprite ___temp)
        {
            bool flag = false;
            switch (sid)
            {
                case SpriteType.RIBAULD_BOMB1:
                    ___temp = resources[0].GetComponent<AreaResource>().GetObjectSprite(13);
                    flag = true;
                    break;
                case SpriteType.RIBAULD_BOMB2:
                    ___temp = resources[0].GetComponent<AreaResource>().GetObjectSprite(13);
                    flag = true;
                    break;
            }
            if(flag)
            {
                es._render.sprite = ___temp;
                return false;
            }
            return true;
        }

        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(bulletScript), "BulletSprite")]
        static void fixBulletSCriptSprites(ref SpriteType ___sprite,ref SpriteRenderer ____render,ref float ___time,ref float ___speed)
        {
            switch (___sprite)
            {
                case SpriteType.thornAttack1:
                    int num9 = (int)(___time / 0.0333f);
                    if (num9 >= 15)
                    {
                        ____render.sprite = null;
                    }
                    else
                        ____render.sprite = resources[4].GetComponent<AreaResource>().GetObjectSprite(8+num9);
                    break;
                case SpriteType.thornAttack2:
                    int num3 = (int)(___time / 0.0333f);
                    if (num3 >= 15)
                    {
                        ____render.sprite = null;
                    }
                    else
                        ____render.sprite = resources[4].GetComponent<AreaResource>().GetObjectSprite(23+num3);
                    break;
                case SpriteType.roleo_flower:
                    if (___speed > 0)
                    {
                        ____render.sprite = resources[4].GetComponent<AreaResource>().GetObjectSprite(8 + (int)(___time / 0.03f) % 3);
                    }
                    else
                        ____render.sprite = resources[4].GetComponent<AreaResource>().GetObjectSprite(7);

                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(phybScript),"Ani")]
        static bool ribauldBomb(ref SpriteType ___sprite,ref float ___time, ref SpriteRenderer ____render)
        {
            switch (___sprite)
            {
                case SpriteType.RIBAULD_BOMB1:
                    {
                        int num2 = (int)(___time * 15f) % 3;
                        if (num2 < 0)
                        {
                            num2 = 0;
                        }
                        num2 = 0;
                        ____render.sprite = resources[0].GetComponent<AreaResource>().GetObjectSprite(num2 + 13);
                        break;
                    }
                case SpriteType.RIBAULD_BOMB2:
                    {
                        int num = (int)(___time * 15f) % 3;
                        if (num < 0)
                        {
                            num = 0;
                        }
                        ____render.sprite = resources[0].GetComponent<AreaResource>().GetObjectSprite(num + 16);
                        break;
                    }
            }
            return false;
        }


        [HarmonyPatch(typeof(BackgroundData), "ChangeSprite")]
        [HarmonyPrefix]
        static bool fixRoomBG(ref RoomBG bgtype)
        {
            if (WorldManager.Instance.CurrentRoomArea == AreaType.FINALPALACE2 && RoomBG.HEAVENGARDEN3 == bgtype)
                return false;
            if (RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedBG])
            {
                Debug.Log($"Original BG:{(int)bgtype}");
                if(RandomizeExtra.randomizedBG[(byte)bgtype] != 0)
                    bgtype = (RoomBG)RandomizeExtra.randomizedBG[(byte)bgtype];

            }
            return true;
        }
    }
}
