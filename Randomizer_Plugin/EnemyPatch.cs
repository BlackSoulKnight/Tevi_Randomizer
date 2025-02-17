using HarmonyLib;
using System;
using EventMode;
using Character;
using UnityEngine;
using System.Linq;

namespace TeviRandomizer
{
    class EnemyPatch
    {
        static Mode setBoss = Mode.BOSS_JEZBELLE;
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
            if(RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedBoss] &&mode.ToString().Contains("END") && mode != Mode.END_BOOKMARK)
            {
                Debug.Log(mode);
                if (originalBoss.ToString().Contains("VENA"))
                    mode = Mode.END_VENA;
                else if (originalBoss.ToString().Contains("TEVIB"))
                    mode = Mode.END_TEVIB;
                else
                    mode = (Mode)Enum.Parse(typeof(Mode), originalBoss.ToString().Replace("BOSS_", "END_"));

            }
        }
        [HarmonyPatch(typeof(EventManager), "DoEvent")]
        [HarmonyPrefix]
        static void lockCam()
        {
            EventManager em = EventManager.Instance;
            byte nr = 0;
            if (em.Mode.ToString().Contains("BOSS") && em.EventStage == 0 && RandomizerPlugin.customFlags[(int)CustomFlags.RandomizedBoss])
            {
                if(originalBoss == Mode.BOSS_DEMONFRAY || originalBoss == Mode.BOSS_FRANKIE)
                    nr = 1;
                BossPatch.lockCameraToArena(em,nr);
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
