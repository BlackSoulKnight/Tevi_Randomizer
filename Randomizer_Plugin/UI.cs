using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using EventMode;
using Game;
using TMPro;
using Character;

using UnityEngine.UI;
using UnityEngine;

using Bullet;
using QFSW.QC;
using UnityEngine.UIElements;
using JetBrains.Annotations;


namespace TeviRandomizer
{




    class UI()
    {
        static GameObject randoSetting;

        static Dictionary<string,object> settings = new Dictionary<string,object>();

        static public object getSettings(string settingName)
        {

            return settings;

        }
        static  GameObject finishedText;

        static public void addAllOptions(GameObject gameObject)
        {
            GameObject settingsAt = gameObject.transform.GetChild(4).gameObject;
            finishedText = gameObject.transform.Find("Finished").gameObject;
            for (int i = 0; i < settingsAt.transform.childCount; i++)
            {
                GameObject t = settingsAt.transform.GetChild(i).gameObject;

                if (t.name.Contains("Toggle"))
                {
                    settings.Add(t.name,t.GetComponentInChildren<UnityEngine.UI.Toggle>());
                }
                else if (t.name.Contains("Slider"))
                {
                    UnityEngine.UI.Slider slider = t.GetComponentInChildren<UnityEngine.UI.Slider>();
                    slider.onValueChanged.AddListener(delegate { t.transform.Find("Number").gameObject.GetComponent<TextMeshProUGUI>().text = slider.value.ToString(); });
                    settings.Add(t.name, slider);
                }
                else if (t.name.Contains("Seed"))
                {
                    settings.Add(t.name, t.GetComponentInChildren<TMPro.TMP_InputField>());
                }
            }
            gameObject.transform.Find("Return").gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { gameObject.SetActive(false); });
            gameObject.transform.Find("Generate").gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                string input = ((TMP_InputField)settings["Seed"]).text;
                if (input == "")
                {
                    RandomizerPlugin.createSeed(null);

                }
                else
                {
                    RandomizerPlugin.createSeed(new System.Random(input.GetHashCode()));
                }

                finishedText.SetActive(true);
            });
            Randomizer.settings = settings;

        }

        static byte menuSlot;
        static GameObject RandoUIPrefab = AssetBundle.LoadFromFile(BepInEx.Paths.PluginPath + "/tevi_randomizer/resource/randomizerui").LoadAsset<GameObject>("Randomizer Setting");

        [HarmonyPatch(typeof(GemaTitleScreenManager),"delayAwake")]
        [HarmonyPostfix]
        public static void addOption(ref GemaMainMenuSelectionSlot[] ___selections)
        {
            settings.Clear();
            RandomizerPlugin.deloadRando();
            string path = BepInEx.Paths.PluginPath + "/tevi_randomizer/resource/";

            //GameObject newSelection = new GameObject("Select Slot", typeof(RectTransform));
            //newSelection.AddComponent<GemaMainMenuSelectionSlot>();
            //var prefab = AssetBundle.LoadFromFile(path).LoadAsset<GameObject>("Select Slot");

            GameObject newSelection = MonoBehaviour.Instantiate(GameObject.Find("Select Slot"), GameObject.Find("Selections").transform);
            newSelection.GetComponentInChildren<TextMeshProUGUI>().text = "Randomizer";

            GemaMainMenuSelectionSlot[] s = new GemaMainMenuSelectionSlot[___selections.Length +1];
            ___selections.CopyTo(s, 0);
            s[s.Length - 1] = newSelection.GetComponent<GemaMainMenuSelectionSlot>();
            ___selections = s;
            menuSlot = (byte)(s.Length - 1);


            randoSetting = MonoBehaviour.Instantiate(RandoUIPrefab, GameObject.Find("Titile Screen Manager").transform);
            addAllOptions(randoSetting);



        }

        [HarmonyPatch(typeof(GemaMainMenuSelectionSlot),"UpdateFont")]
        [HarmonyPostfix]
        static void fixText(ref TextMeshProUGUI ___text,ref bool ___isNewGameSelect,ref int ID)
        {
            if(!___isNewGameSelect && ID == menuSlot)
            {
                ___text.text = "Randomizer";
            }
        }

        [HarmonyPatch(typeof(GemaTitleScreenManager),"InTitleScreen")]
        [HarmonyPrefix]
        static bool mep(ref bool __result)
        {
            if (randoSetting.activeInHierarchy)
            {
                __result = false;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GemaTitleScreenManager), "MainTitleScreen")]
        [HarmonyPostfix]
        public static void openRandomizerOtions(ref byte ___selected,ref bool ___needbottombarupdate,ref CanvasGroup ___titlescreen)
        {
            if(___titlescreen.alpha >= 0.8f && InputButtonManager.Instance.GetButtonDown(13) && ___selected == menuSlot)
            {
                finishedText.SetActive(false);
                randoSetting.SetActive(true);
            }
        }

    }

    public class sliderUpdate : MonoBehaviour
    {
        private GameObject slider = null;
        private GameObject sliderNumber = null;
        // Start is called before the first frame update
        void Start()
        {
            slider = transform.GetChild(0).gameObject;
            sliderNumber = transform.GetChild(2).gameObject;
            slider.GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(delegate { updateNumber(); });
        }

        private void updateNumber()
        {
             transform.gameObject.SetActive(false);
            sliderNumber.GetComponent<TextMeshProUGUI>().text = slider.GetComponent<UnityEngine.UI.Slider>().value.ToString();


        }

    }
}
