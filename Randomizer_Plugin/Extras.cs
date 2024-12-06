using EventMode;
using Game;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    class Extras
    {



        static Harmony WhiteFlashPatch = new Harmony("FlashPatch");

        static public void patchWhiteFlash(bool enable = false)
        {
            if (enable)
            {
                WhiteFlashPatch.PatchAll(typeof(WhiteFlash));
            }
            else
            {
                WhiteFlashPatch.UnpatchSelf();
            }
        }

        class WhiteFlash
        {
            [HarmonyPatch(typeof(CharacterBase), "_Update")]
            [HarmonyPrefix]
            static bool noFlash(ref float ___teleport, ref CharacterBase __instance, ref Transform ___t)
            {
                if (___teleport > 0f)
                {


                    if (___teleport > 199f && ___teleport < 300f)
                    {
                        EventManager.Instance.EFF_CreateWarpEffect(___t.position);
                        __instance.spranim_prefer.NoFlash();
                        __instance.spranim_prefer.Invisible(t: true);
                        EventManager.Instance.HideOrbs(t: true);
                        ___teleport = 300f;
                        return false;
                    }
                    if (___teleport > 299f && ___teleport < 1000f && EventManager.Instance.IsWarping() >= 1000f)
                    {
                        __instance.ChangeDirectionToCenter();
                        __instance.spranim_prefer.NoFlash();
                        __instance.spranim_prefer.Invisible(t: false);
                        EventManager.Instance.EFF_CreateWarpEffect(___t.position);
                        EventManager.Instance.HideOrbs(t: false);
                        ___teleport = 1000f;
                        return false;
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(FadeManager), "SetAll")]
            [HarmonyPrefix]
            static bool ripWhiteFade(ref float r, ref float g, ref float b)
            {
                if (r == 1 && g == 1 && b == 1)
                {
                    return false;
                }
                return true;
            }
            [HarmonyPatch(typeof(END_EIDOLON), "EVENT")]
            [HarmonyPrefix]
            static void eidolonFix(ref enemyController ___boss)
            {
                EventManager em = EventManager.Instance;
                if (em.EventStage == 20)
                {
                    MainVar.instance.RunSpeed = 1f;
                    em.NextStage();
                    if (SaveManager.Instance.GetMiniFlag(Mini.BookmarkUsed) >= 2)
                    {
                        ___boss.DoNotDelete = true;
                        if (SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            SaveManager.Instance.SetEventFlag(Mode.END_EIDOLON, 1, force: true);
                        }
                        em.StopEvent();
                        em.TryStartEvent(Mode.END_BOOKMARK, force: true);
                    }
                }
            }


        }

        public class RandomEnemy
        {
            static public void randomEnemies()
            {
                EnemyPatch.enemyReplace = new short[(int)Character.Type.MAX];
                EnemyPatch.eventReplace = new short[(int)Mode.MAX];
                for(int i = 0;i < EnemyPatch.enemyReplace.Length;i++)
                {
                    EnemyPatch.enemyReplace[i] = -1;
                }
                for(int i = 0;i < EnemyPatch.eventReplace.Length;i++)
                {
                    EnemyPatch.eventReplace[i] = -1;
                }
                List<short> enemies = new List<short>();
                enemies.AddRange(new short[] {
                (short)Character.Type.GH_Member_Cat,
                (short)Character.Type.GH_Member_Dog,
                (short)Character.Type.Hermitcrab_Cans,
                (short)Character.Type.Hermitcrab_Shoes,
                (short)Character.Type.Jacoben,
                (short)Character.Type.Gromp,
                (short)Character.Type.TR_Member_Employee,
                (short)Character.Type.TR_Member_Miner,
                (short)Character.Type.Greasetrap,
                (short)Character.Type.Mouse,
                (short)Character.Type.GH_Bot,
                (short)Character.Type.GH_Bot_Range,
                (short)Character.Type.Bat,
                (short)Character.Type.Spider,
                (short)Character.Type.GH_CleanStaff,
                (short)Character.Type.Cobra,
                (short)Character.Type.Cobra_Armor,
                (short)Character.Type.Cactus,
                (short)Character.Type.Jackal,
                (short)Character.Type.Scorpion,
                (short)Character.Type.Vulture,
                (short)Character.Type.Shroom,
                (short)Character.Type.Kodama,
                (short)Character.Type.Kodama_pollution,
                (short)Character.Type.Mouse_Forest,
                (short)Character.Type.Snapweed,
                (short)Character.Type.Physalis,
                (short)Character.Type.Dragonweed,
                (short)Character.Type.Snaptrap,
                (short)Character.Type.Crawler_Forest,
                (short)Character.Type.Drone_Forest,
                (short)Character.Type.Coal,
                (short)Character.Type.CoalMinecart,
                (short)Character.Type.SteamMiner,
                (short)Character.Type.SteamTank,
                (short)Character.Type.Miasma,
                (short)Character.Type.Lamp,
                (short)Character.Type.Rabiball,
                (short)Character.Type.Ruinserf,
                (short)Character.Type.Scrapper_Armor,
                (short)Character.Type.Doggy_Armor,
                (short)Character.Type.Drone_Ice,
                (short)Character.Type.Shieldbot,
                (short)Character.Type.Crawler,
                (short)Character.Type.Crawler_Sup,
                (short)Character.Type.Drone,
                (short)Character.Type.Frog,
                (short)Character.Type.Rabbit,
                (short)Character.Type.Rabbit_Purple,
                (short)Character.Type.PK_Guard_B_Pollution,
                (short)Character.Type.PK_Guard_G_Pollution,
                (short)Character.Type.A_Guard_Pollution,
                (short)Character.Type.Pomp,
                (short)Character.Type.Bourbon,
                (short)Character.Type.Anisette,
                (short)Character.Type.Amaretto,
                (short)Character.Type.T_Guard_Elite,
                (short)Character.Type.V_Guard_Elite,
                (short)Character.Type.Shroom_Mutant,
                (short)Character.Type.Toothberry,
                (short)Character.Type.Porapora,
                (short)Character.Type.Shieldbot_Forest,
                (short)Character.Type.Crawler_Forest_Sup,
                (short)Character.Type.Shieldbot_Ice,
                (short)Character.Type.Shieldbot_Fire,
                (short)Character.Type.Angelweed,
                (short)Character.Type.Pestilence,
                (short)Character.Type.Physalis_Pollution,
                (short)Character.Type.Shieldbot_Pollution,
                (short)Character.Type.Drone_Pollution,
                (short)Character.Type.Crawler_Pollution,
                (short)Character.Type.Crawler_Pollution_Sup,
                (short)Character.Type.Pestilence_Range,
                (short)Character.Type.Jelly,
                (short)Character.Type.Jelly_Elite,
                (short)Character.Type.Jelly_Forest,
                (short)Character.Type.Jelly_Forest_Elite,
                (short)Character.Type.Dragonweed_Pollution,
                (short)Character.Type.Cragmiester,
                (short)Character.Type.Cragmiester_Elite,
                (short)Character.Type.Cragmiester_Pollution,
                (short)Character.Type.Bat_Pollution,
                (short)Character.Type.Mouse_Pollution,
                (short)Character.Type.GroundRose,
                (short)Character.Type.FloatRose,
                (short)Character.Type.Mersentry,
                (short)Character.Type.Seahorse,
                (short)Character.Type.Hermitcrab_Gold,
                (short)Character.Type.Cragmiester_Pollution_Mini,
                (short)Character.Type.Butterfly_Blue,
                (short)Character.Type.Butterfly_Red,
                (short)Character.Type.Butterfly_Yellow,
                (short)Character.Type.Butterfly_White,
                (short)Character.Type.V_Chariot,
                (short)Character.Type.T_Skeleton,
                (short)Character.Type.T_Clown,
                (short)Character.Type.Cobra_Dark,
                (short)Character.Type.Protector,
                (short)Character.Type.Monitor,
                (short)Character.Type.Crawler_Fire,
                (short)Character.Type.Crawler_Fire_Sup,
                (short)Character.Type.Crawler_Ice,
                (short)Character.Type.Crawler_Ice_Sup,
                (short)Character.Type.Drone_Fire,
                (short)Character.Type.Monitor_Elite,
                (short)Character.Type.T_Curse,
                (short)Character.Type.T_Creeper,
                (short)Character.Type.T_Creeper_Elite,
                (short)Character.Type.V_Archer,
                (short)Character.Type.V_Unicorn,
                (short)Character.Type.V_Ironmaiden,
                (short)Character.Type.T_Scientist_B,
                (short)Character.Type.T_Scientist_G,
                (short)Character.Type.Landmine,
                (short)Character.Type.Phantom,
                (short)Character.Type.Frost_Hive,
                (short)Character.Type.Frost_Hornet,
                (short)Character.Type.SnowSock,
                (short)Character.Type.GH_Member_EliteCat,
                (short)Character.Type.GH_Member_EliteDog,
                (short)Character.Type.GH_Member_EliteMouse,
                (short)Character.Type.Boomball_Ice,
                (short)Character.Type.SnowBear,
                (short)Character.Type.Slime,
                (short)Character.Type.WallGolem,
                (short)Character.Type.Knights_A,
                (short)Character.Type.Knights_B,
                (short)Character.Type.Knights_C,
                (short)Character.Type.PetBunny,
                (short)Character.Type.PetCat,
                }) ;
                List<short> placed = new List<short>();

                foreach (short a in enemies)
                {
                    while (true)
                    {
                        short num1 = (short)UnityEngine.Random.Range(0, enemies.Count);
                        if (placed.Contains(num1))
                            continue;
                        else
                        {
                            EnemyPatch.enemyReplace[a] = enemies[num1];
                            break;
                        }
                    }
                }
                List<short> events = new List<short>();
                events.AddRange(new short[]
                {
                    (short) Mode.BOSS_AMARYLLIS,
                    (short) Mode.BOSS_BARADOS,
                    (short) Mode.BOSS_CAPRICE,
                    (short) Mode.BOSS_CYRIL,
                    (short) Mode.BOSS_DEMONFRAY,
                    (short) Mode.BOSS_EIDOLON,
                    (short) Mode.BOSS_FRANKIE,
                    (short) Mode.BOSS_JETHRO,
                    (short) Mode.BOSS_JEZBELLE,
                    (short) Mode.BOSS_KATU,
                    (short) Mode.BOSS_LILY,
                    (short) Mode.BOSS_MALPHAGE,
                    (short) Mode.BOSS_RIBAULD,
                    (short) Mode.BOSS_ROLEO,
                    (short) Mode.BOSS_TEVIB11x11,
                    (short) Mode.BOSS_THETIS,
                    (short) Mode.BOSS_AMARYLLIS,
                    (short) Mode.BOSS_TYBRIOUS,
                    (short) Mode.BOSS_VASSAGO,
                });
                foreach (short a in events)
                {
                    while (true)
                    {
                        short num1 = (short)UnityEngine.Random.Range(0, events.Count);
                        if (placed.Contains(num1))
                            continue;
                        else
                        {
                            EnemyPatch.eventReplace[a] = events[num1];
                            break;
                        }
                    } 
                }

            }
        }
    }
}
