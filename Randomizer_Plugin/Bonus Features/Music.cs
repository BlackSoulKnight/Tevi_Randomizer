using Bullet;
using Character;
using EventMode;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TeviRandomizer.TeviRandomizerSettings;

namespace TeviRandomizer.Bonus_Features
{
    class MusicPatch()
    {


        [HarmonyPatch(typeof(MusicManager), "PlayMusic")]
        [HarmonyPrefix]
        static void changeMusic(ref Music musicname, ref Music __state)
        {
            if (TeviSettings.customFlags[CustomFlags.RandomizedMusic])
            {
                if (Extras.RandomizeExtra.randomizedMusic[(byte)musicname] == 0)
                {
                    __state = Music.OFF;
                    return;
                }

                __state = musicname;
                if (musicname == Music.LOOP) { return; }
                musicname = (Music)Extras.RandomizeExtra.randomizedMusic[(byte)musicname];
            }
        }
        [HarmonyPatch(typeof(MusicManager), "PlayMusic")]
        [HarmonyPostfix]
        static void saveLastMusic(ref Music ___lastMusic, ref Music __state, ref Music ___readyMusic)
        {

            if (TeviSettings.customFlags[CustomFlags.RandomizedMusic])
            {

                if (__state == Music.LOOP || __state == Music.OFF) { return; }
                ___lastMusic = __state;
                ___readyMusic = Music.OFF;
            }
        }
    }


}
