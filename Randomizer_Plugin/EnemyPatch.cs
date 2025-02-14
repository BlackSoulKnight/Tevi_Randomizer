using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using EventMode;
using Character;
using static Rewired.ComponentControls.Data.CustomControllerElementSelector;
using UnityEngine;
using static Localize;
using UnityEngine.Analytics;
using static UnityEngine.UI.Image;
using System.Linq;

namespace TeviRandomizer
{
    class EnemyPatch
    {
        static Mode setBoss = Mode.BOSS_VASSAGO;
        static Mode originalBoss;
        public static short[] eventReplace = null;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "TryStartEvent")]
        static public void testBossReplace(ref Mode setMode)
        {
            originalBoss = setMode;
            if (eventReplace != null && eventReplace.Length > (short)setMode && eventReplace[(short)setMode] != -1 && RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedBoss]) {
                setMode = (Mode)eventReplace[(short)setMode];
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveManager), "SetEventFlag")]
        static void EndBossReplace(ref Mode mode)
        {
            if(RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedBoss] &&mode.ToString().Contains("END"))
            {
                if (mode.ToString().Contains("VENA"))
                    mode = Mode.END_VENA;
                else
                mode = (Mode)Enum.Parse(typeof(Mode), originalBoss.ToString().Replace("BOSS_", "END_"));

            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveManager), "CheckEnemyDefeatedTypeCount", [typeof(enemyController)] )]
        static void dropCore(ref bool __result,ref enemyController ec)
        {
            if(!__result)
            {
                byte b = 0;
                if(ec.type == Character.Type.Pestilence || ec.type == Character.Type.Pestilence_Range)
                {
                    b = 16;
                }
                if (ec.dropCore)
                {
                    b = 24;
                }
                if(b>0 && SaveManager.Instance.savedata.enemyDefeatedType[(int)ec.type] == b)
                {
                    __result = true;
                }
            }
        }

        static void fixTahlia()
        {
            if (EventManager.Instance.EventStage == 0)
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
            EventManager.Instance.SetStage(10);
            if (EventManager.Instance.EventStage == 20)
                EventManager.Instance.SetStage(110);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_AMARYLLIS), "EVENT")]
        static void fixAmarzllis()
        {
            if (EventManager.Instance.EventStage == 0 && WorldManager.Instance.Area != 29)
            {
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
                UnityEngine.Object.Instantiate(TeviRandomizer.ResourcePatch.resources[29].GetComponent<AreaResource>().GetMapObject(10));
                UnityEngine.Object.Instantiate(TeviRandomizer.ResourcePatch.resources[29].GetComponent<AreaResource>().GetMapObject(10));
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_BARADOS), "EVENT")]
        static void fixBarados(ref float ___leftx, ref enemyController ___barados)
        {
            EventManager em = EventManager.Instance;
            if (EventManager.Instance.EventStage == 0 && WorldManager.Instance.Area != 5)
            {
                em.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
                em.mainCharacter.ChangeDirection(Direction.RIGHT);
                em.mainCharacter.AIMove(245f);
                ___leftx = WorldManager.Instance.CurrentRoomLeft;
                CharacterBase characterBase = em.CreateEnemy(Character.Type.Barados, BossType.BOSS);
                em.AddActor(characterBase);
                characterBase.SetPosition(CameraScript.Instance.GetTrueX() + 177f, CameraScript.Instance.GetTrueY() + 100f);
                characterBase.direction = Direction.NOTTOPLAYER;
                ___barados = characterBase as enemyController;
                em.SetBossBarOwner(characterBase);
                characterBase.SetDefeatEvent(Mode.END_BARADOS);
                MusicManager.Instance.SetTargetVolume(0f, 0.6f);
                SaveManager.Instance.CheckIsFirstTimePlaying(em.getMode());
                em.SetStage(5);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_TYBRIOUS), "EVENT")]
        static void fixTybrious(ref CharacterBase ___cb)
        {
            EventManager em = EventManager.Instance;
            if (EventManager.Instance.EventStage == 30 && WorldManager.Instance.Area != 26)
            {
                movePlayerToLeftSide(false);
                ___cb = em.CreateEnemy(Character.Type.Tybrious, BossType.BOSS);
                em.AddActor(___cb);
                //em.SetCharacterPosition(cb, CreateType.FIXEDPOSITION, CreateType.BASEONPLAYER, elementTile.transform.position.x - MainVar.instance.TILESIZE / 2f, -0.02f);
                ___cb.SetPosition(CameraScript.Instance.GetTrueX() + 177f, CameraScript.Instance.GetTrueY() + 100f);

                ___cb.t.position += new Vector3(-3f, 0f, 0f);
                ___cb.direction = Direction.NOTTOPLAYER;
                em.SetBossBarOwner(___cb);
                ___cb.SetDefeatEvent(Mode.END_TYBRIOUS);
                CameraScript.Instance.SetTargetXY(___cb.t, MainVar.instance.TILESIZE * 4f, 0f, forcePositionIgnoreRoom: false);
                MusicManager.Instance.SetTargetVolume(0.25f);
                em.mainCharacter.t.position += new Vector3(640f, 0f, 0f);
                em.NextStage();


            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_FRANKIE), "EVENT")]
        static void fixFrankie()
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 150 && WorldManager.Instance.Area != 27)
            {
                em.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
                em.NextStage();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_DEMONFRAY), "EVENT")]
        static void fixFray()
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 25) {
                if (em.EventStage == 0)
                {
                    em.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
                    WorldManager.Instance.RoomTime = -1;
                }
                if (em.EventStage == 50)
                {
                    em.NextStage();
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_TEVIB11x11), "EVENT")]
        static void fixTeviB(ref enemyController ___s, ref enemyController ___c)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 18)
            {
                if (em.EventStage == 0)
                {
                    movePlayerToLeftSide(true);
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);
                    CustomMap.createElementTile(originX + 10, originY + 4, 155, 18);
                    CustomMap.createElementTile(originX + 10, originY + 5, 43, 18);
                    CustomMap.createElementTile(originX + 11, originY + 7, 73, 18);
                    CustomMap.createElementTile(originX + 12, originY + 7, 93, 18);

                }
                if (em.EventStage == 80)
                {
                    ___s.DespawnMe();
                    ___c.DespawnMe();
                    em.NextStage();
                    

                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_MALPHAGE), "EVENT")]
        static void fixMALPHAGE()
        {
            EventManager em = EventManager.Instance;

            if (em.EventStage == 0 && WorldManager.Instance.Area != 10)
            {
                em.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());

                //spawn every object for MALPHAGE
                if (WorldManager.Instance.Area != 10)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);
                    CustomMap.createElementTile(originX - 5, originY - 3, 158, 10);
                    CustomMap.createElementTile(originX - 5, originY - 3, 158, 10);
                    //ID0
                    CustomMap.createElementTile(originX - 5, originY + 16, 72, 10);
                    CustomMap.createElementTile(originX - 5, originY + 15, 162, 10);

                    CustomMap.createElementTile(originX - 4, originY + 18, 73, 10);
                    CustomMap.createElementTile(originX - 4, originY + 17, 161, 10);

                    CustomMap.createElementTile(originX - 2, originY + 15, 73, 10);
                    CustomMap.createElementTile(originX - 2, originY + 17, 74, 10);
                    CustomMap.createElementTile(originX - 2, originY + 14, 163, 10);
                    CustomMap.createElementTile(originX - 2, originY + 16, 162, 10);

                    CustomMap.createElementTile(originX - 1, originY + 13, 74, 10);
                    CustomMap.createElementTile(originX - 1, originY + 17, 72, 10);
                    CustomMap.createElementTile(originX - 1, originY + 12, 163, 10);
                    CustomMap.createElementTile(originX - 1, originY + 16, 162, 10);

                    CustomMap.createElementTile(originX + 1, originY + 13, 73, 10);
                    CustomMap.createElementTile(originX + 1, originY + 15, 72, 10);
                    CustomMap.createElementTile(originX + 1, originY + 12, 162, 10);
                    CustomMap.createElementTile(originX + 1, originY + 14, 163, 10);

                    CustomMap.createElementTile(originX + 2, originY + 17, 74, 10);
                    CustomMap.createElementTile(originX + 2, originY + 18, 82, 10);
                    CustomMap.createElementTile(originX + 2, originY + 16, 161, 10);

                    CustomMap.createElementTile(originX + 3, originY + 16, 73, 10);
                    CustomMap.createElementTile(originX + 3, originY + 18, 72, 10);
                    CustomMap.createElementTile(originX + 3, originY + 15, 162, 10);
                    CustomMap.createElementTile(originX + 3, originY + 17, 162, 10);

                    CustomMap.createElementTile(originX + 4, originY + 14, 74, 10);
                    CustomMap.createElementTile(originX + 4, originY + 13, 161, 10);

                    CustomMap.createElementTile(originX + 6, originY + 18, 73, 10);
                    CustomMap.createElementTile(originX + 6, originY + 17, 161, 10);

                    CustomMap.createElementTile(originX + 7, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 8, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 8, originY + 12, 73, 10);
                    CustomMap.createEnemyTile(originX + 8, originY + 13, 164, 10); //MalphageCore
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 8, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 8, originY + 12, 73, 10);
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 6, originY + 2, 75, 10);
                    CustomMap.createEnemyTile(originX + 6, originY + 3, 164, 10); //MalphageCore

                    CustomMap.createElementTile(originX - 2, originY + 4, 74, 10);
                    CustomMap.createEnemyTile(originX - 2, originY + 5, 164, 10); //MalphageCore

                    CustomMap.createEnemyTile(originX + 17, originY + 4, 164, 10); //MalphageCore


                    CustomMap.createElementTile(originX + 11, originY + 11, 164, 10);

                    CustomMap.createElementTile(originX + 11, originY + 8, 165, 10);


                    CustomMap.createElementTile(originX + 16, originY - 2, 159, 10);
                    CustomMap.createElementTile(originX + 6, originY - 2, 160, 10);
                    CustomMap.createElementTile(originX - 5, originY - 2, 158, 10);
                    CustomMap.createElementTile(originX + 6, originY + 11, 157, 10);



                }


                em.mainCharacter.ChangeDirection(Direction.RIGHT);
                MusicManager.Instance.SetTargetVolume(0.25f, 0.5f);

                em.SetStage(3);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_KATU), "EVENT")]
        static void fixKATU(ref float ___center)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 22)
            {
                float size = MainVar.instance.TILESIZE;
                if (em.EventStage == 0)
                {
                    if (WorldManager.Instance.Area == 22) return;
                    movePlayerToLeftSide(true);
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);


                    CustomMap.createNormalTile(originX + 2, originY + 7, 92, false, false, 22);
                    CharacterBase characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    //characterBase.SetPosition((originX + 2)*size, (originY + 7)*size);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);

                    characterBase.direction = Direction.RIGHT;
                    em.AddActor(characterBase);



                    CustomMap.createNormalTile(originX + 5, originY + 7, 92, false, false, 22);
                    //characterBase.SetPosition((originX + 5) * size, (originY + 7) * size);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);

                    characterBase.direction = Direction.RIGHT;
                    em.AddActor(characterBase);


                    CustomMap.createNormalTile(originX + 16, originY + 7, 92, false, false, 22);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);
                    //characterBase.SetPosition((originX + 16) * size, (originY + 7) * size);
                    characterBase.direction = Direction.LEFT;
                    em.AddActor(characterBase);

                    CustomMap.createNormalTile(originX + 19, originY + 7, 92, false, false, 22);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);
                    //characterBase.SetPosition((originX + 19) * size, (originY + 7) * size);
                    characterBase.direction = Direction.LEFT;
                    em.AddActor(characterBase);

                    CustomMap.createElementTile(originX + 13, originY + 7, 63, 22);

                    em.SetStage(1);

                }
                if (em.EventStage == 1)
                {
                    em.mainCharacter.ChangeDirection(Direction.RIGHT);
                    em.mainCharacter.AIMove(300f);
                    if (em.EventTime >= 1.35f)
                    {

                        SaveManager.Instance.CheckIsFirstTimePlaying(em.getMode());
                        MusicManager.Instance.SetTargetVolume(0.25f);
                        ElementTile elementTile = WorldManager.Instance.FindElementTileNearCamera(EventMode.ElementType.MapPoint, MainVar.instance.SCREEN_WIDTH);
                        if ((bool)elementTile)
                        {
                            ___center = elementTile.transform.position.x - MainVar.instance.TILESIZE / 2f;
                        }
                        em.SetStage(10);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(BOSS_FRANKIE),"EVENT")]
        [HarmonyPrefix]
        static void fixFRANKIE(ref enemyController ___frankie,ref float ___playery)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 27)
            {
                if (em.EventStage == 0)
                {
                    movePlayerToLeftSide(false);
                    em.SetStage(1);

                }
                if (em.EventStage == 1)
                {
                    if (em.EventTime >= 0.75f && em.mainCharacter.onGround())
                    {
                        CameraScript.Instance.NoLimitLR();
                        enemyController frankie = CharacterManager.Instance.GetCharacter(Character.Type.Frankie) as enemyController;
                        if (frankie == null)
                        {
                            ___playery = em.mainCharacter.t.position.y;
                            frankie = em.CreateEnemy(Character.Type.Frankie, BossType.NONE) as enemyController;
                            em.SetCharacterPosition(frankie, CreateType.FIXEDPOSITION, CreateType.FIXEDPOSITION, em.mainCharacter.t.position.x - MainVar.instance.TILESIZE * 8.5f, ___playery + 2f);
                            frankie.ChangeDirByPlayer();
                            ___frankie = frankie;
                            em.SetStage(110);
                        }
                        else
                        {
                            em.SetStage(10);
                        }
                        CameraScript.Instance.SetCameraSpeed(7.5f);
                        CameraScript.Instance.SetTargetXY(frankie.t, 285f, 0f, forcePositionIgnoreRoom: false);
                    }
                }
            }
        }


        [HarmonyPatch(typeof(BOSS_JEZBELLE), "EVENT")]
        [HarmonyPrefix]
        static void fixJEZBELLE()
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 0 && WorldManager.Instance.Area != 16)
            {
                
                movePlayerToLeftSide(false);
                short rX = WorldManager.Instance.CurrentRoomX;
                short rY = WorldManager.Instance.CurrentRoomY;
                // x:-5 y: -3
                int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                int originY = (int)(rY * CustomMap.firstBlockMultiplierY);

                CustomMap.createElementTile(originX + 12, originY + 4, 63, 16);
                CustomMap.createNormalTile(originX + 4, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 5, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 6, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 7, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 8, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 9, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 10, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 11, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 12, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 13, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 14, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 15, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 16, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 17, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 18, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 19, originY + 9,21,false,false, 16);
                CustomMap.createNormalTile(originX + 20, originY + 9,21,false,false, 16);

                em.SetStage(5);
            }
        }


        static void fixCHARON(ref float ___center,ref enemyController ___m,ref enemyController ___c,ref float ___centeroffx,ref float ___ypos)
        {
            EventManager em = EventManager.Instance;
            if(em.EventStage == 0) {
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());

            }
            if  (em.EventStage == 4) {
                em.SetStage(5);
            }
            if(em.EventStage == 5)
            {
                em.SetCutSceneBarAlpha(0f);
                ___center = CameraScript.Instance.GetTrueX();
                CameraScript.Instance.SetLimitLR(___center, ___center);
                CameraScript.Instance.SetCameraSpeed(100f);
                em.mainCharacter.ChangeDirection(Direction.RIGHT);
                CharacterBase characterBase = em.CreateEnemy(Character.Type.Charon, BossType.BOSS);
                em.AddActor(characterBase);
                characterBase.SetPositionX(___center + MainVar.instance.TILESIZE * 6f);
                characterBase.SetPositionY(em.mainCharacter.t.position.y);
                characterBase.direction = Direction.TOPLAYER;
                characterBase.SetDefeatEvent(Mode.END_CHARON);
                ___c = characterBase as enemyController;
                em.SetBossBarOwner(characterBase);
                CharacterBase characterBase2 = em.CreateEnemy(Character.Type.MagnaLegacy, BossType.NONE);
                em.AddActor(characterBase2);
                characterBase2.SetPositionX(___center);
                characterBase2.SetPositionY(em.mainCharacter.t.position.y + MainVar.instance.TILESIZE * 1.5f);
                characterBase2.direction = Direction.LEFT;
                characterBase2.phy_perfer.AIGravity(0f);
                characterBase2.phy_perfer.AIFloatMode();
                characterBase2.DoNotDelete = true;
                ___m = characterBase2 as enemyController;
                em.SetStage(6);
            }
        }

        [HarmonyPatch(typeof(BOSS_VASSAGO), "EVENT")]
        [HarmonyPrefix]
        static void fixVASSAGO()
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 25) {
                if (em.EventStage == 0)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);
                    CustomMap.createElementTile(originX + 10, originY + 13, 158, 25);
                    CustomMap.createElementTile(originX + 10, originY + 14, 159, 25);
                }
                if (em.EventStage == 110) {
                    em.NextStage();
                }
            } 
        }

            static void movePlayerToLeftSide(bool left)
        {
            if (left)
            {
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX() - 625, CameraScript.Instance.GetTrueY());
            }
            else
            {
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX() + 625, CameraScript.Instance.GetTrueY());
            }
        }

        public static short[] enemyReplace = null;
        static bool isMission;
        [HarmonyPatch(typeof(EventManager),"CreateEnemy")]
        [HarmonyPrefix]
        static void changeEnemy(ref Character.Type type)
        {
            if (!EventManager.Instance.eventBattle.ToString().Contains("Mission") && !EventManager.Instance.Mode.ToString().Contains("Mission"))
            {
                Debug.Log(type.ToString());
                if (enemyReplace != null && enemyReplace.Length > (short)type)
                {

                    if (enemyReplace[(short)type] != -1 && RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedEnemy])
                        type = (Character.Type)enemyReplace[(short)type];

                }
                if (RandomizerPlugin.customFlags[(int)CustomFlags.AlwaysRandomizeEnemy] && Extras.RandomizeExtra.enemies != null)
                {
                    if (Extras.RandomizeExtra.enemies.Contains((short)type))

                        type = (Character.Type)Extras.RandomizeExtra.enemies[(short)UnityEngine.Random.Range(0, Extras.RandomizeExtra.enemies.Count)];
                }
            }
        }


        //Library Bosses

        [HarmonyPatch(typeof(TR_Member_Employee), "INIT")]
        [HarmonyPrefix]
        static bool enableLibraryBoss(ref enemyController ___en, ref TR_Member_Employee __instance,ref byte ___isWalk,ref float ___x)
        {
            Character.Type[] type = {Character.Type.GemaYue, Character.Type.Waero, Character.Type.EinLee};
            if (type.Contains(___en.type))
            {
                ___x = ___en.t.position.x;
                ___en.isBoss = BossType.NPC;
                int id = 0;

                if (___en.type ==  Character.Type.EinLee || ___en.type == Character.Type.Waero) {
                    ___en.alwaysFaceToPlayer = true;
                    if(___en.type == Character.Type.EinLee)
                        id = (int)Character.Type.EinLee_B % 3 + 200;
                    if(___en.type == Character.Type.Waero)
                        id = (int)Character.Type.Waero_B % 3 + 200;
                }
                if(___en.type == Character.Type.GemaYue)
                {
                    ___en.t.position += new Vector3(-15f, MainVar.instance.TILESIZE * 0.425f, 0f);
                    ___en.AIGravity(0f);
                    ___en.phy_perfer._velocity.y = 0f;
                    ___en.SetCounter(9, UnityEngine.Random.Range(0, 999));
                    ___en.SetCounter(3, ___en.t.position.x);
                    ___en.SetCounter(4, ___en.t.position.y);
                    Traverse.Create(__instance).Method("GemaYueStand").GetValue();
                    id = (int)Character.Type.GemaYue_B % 3 + 200;
                }
                if (RandomizerPlugin.checkItemGot(ItemList.Type.STACKABLE_COG, (byte)id) || !RandomizerPlugin.customFlags[(int)CustomFlags.SuperBosses])
                    ___en.DespawnMe();
                byte iDFromBelow = EventManager.Instance.GetIDFromBelow(___en.t, 4.5f);
                if (iDFromBelow >= 1 && iDFromBelow < byte.MaxValue)
                {
                    ___isWalk = iDFromBelow;
                    ___en.spranim_prefer.NoForceAnimation();
                }
                return false;
            }
            return true;
        }
    }

}

/* DONE
 *  BARADOS
 *  AMRYLLIS
 *  TAHLIA
 *  Ribauld
 *  TYVRIOUS
 *  ROLEO
 *  THETIS
 *  CAPRICE
 *  EIDOLON
 *  DEMONFRAY
 *  CYRIL
 *  MALPHAGE
 *  KATU
 *  FARNKIE
 *  JEZBELLE
 *  TEVIB Kinda Works (charge shot not working)
 *  VASSAGE
 *  
 *  SKIPED
 *  REVENANCE
 *  MEMLOCH
 *  CHARON
 *  VENA
 */
