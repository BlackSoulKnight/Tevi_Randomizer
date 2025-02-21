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
                if(originalBoss == Mode.BOSS_DEMONFRAY || originalBoss == Mode.BOSS_FRANKIE || originalBoss == Mode.BOSS_CYRIL)
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

        public static short[] enemyReplace = null;
        static bool isMission;
        [HarmonyPatch(typeof(EventManager),"CreateEnemy")]
        [HarmonyPrefix]
        static void changeEnemy(ref Character.Type type)
        {
            EventManager em = new EventManager();
            if (!em.eventBattle.ToString().Contains("Mission") && !em.Mode.ToString().Contains("Mission") && !(em.Mode == Mode.Chap1S_Wasteland_Greasetrap) &&!(em.Mode == Mode.Chap2S_Beach_Friedman) &&!(em.Mode == Mode.Chap4MonitorRoom))
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
                if (RandomizerPlugin.checkItemGot(ItemList.Type.STACKABLE_COG, (byte)id) || SaveManager.Instance.GetChapter() < 4)
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
