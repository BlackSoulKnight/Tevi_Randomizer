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


namespace TeviRandomizer
{
    class ResourcePatch
    {
        static public List<ObjectPooler> pooler = new List<ObjectPooler>();
        static public Scene ResourceScene;
        static public AreaResource AreaResource;
        static public GameObject[] resources = [];
        static async Task getAllResources()
        {


            ResourceScene = SceneManager.CreateScene("AllResource");
            GameObject newObj = new GameObject("Resource");
            AreaResource = newObj.AddComponent<AreaResource>();
            SceneManager.MoveGameObjectToScene(newObj, ResourceScene);


            List<AsyncOperationHandle<SceneInstance>> sceneHandle = new List<AsyncOperationHandle<SceneInstance>>();
            for (int i = 0; i < MainVar.instance.MAXAREA - 2; i++)
            {
                sceneHandle.Add( Addressables.LoadSceneAsync($"resource{i}", LoadSceneMode.Additive));

            }
            List<GameObject> rootScene = new List<GameObject>();

            foreach(AsyncOperationHandle<SceneInstance> SceneInstance in sceneHandle)
            {
                while (!SceneInstance.IsDone)
                {
                    await Task.Yield();
                }
                if (SceneInstance.Result.Scene.IsValid())
                {
                    rootScene.Add(SceneInstance.Result.Scene.GetRootGameObjects()[0]);
                }
            }
            resources = rootScene.ToArray();

        }

        static public AreaResource getAreaResource(int area)
        {
            if(resources != null && resources.Length >area)
            {
                return resources[area].GetComponent<AreaResource>();
            }
            return AreaResource;
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
                await getAllResources();
            }
            if (__instance.Area < resources.Length)
            {
                AreaResource.Instance = resources[__instance.Area].GetComponent<AreaResource>();
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

    }
}
