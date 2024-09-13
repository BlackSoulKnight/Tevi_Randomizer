using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using TMPro;
using Game;
using Rewired;

using UnityEngine;

using Bullet;
using QFSW.QC;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using RewiredConsts;
using System.Net;
using UnityEngine.UI;



namespace TeviRandomizer
{




    class UI()
    {
        static GameObject randoSetting;

        public static Dictionary<string,object> settings = new Dictionary<string,object>();

        static public object getSettings(string settingName ="")
        {

            return settings;

        }
        static public GameObject finishedText;


        static byte menuSlot;
        static GameObject RandoUIPrefab = AssetBundle.LoadFromFile(BepInEx.Paths.PluginPath + "/tevi_randomizer/resource/randomizerui").LoadAsset<GameObject>("Randomizer Setting");

        [HarmonyPatch(typeof(GemaTitleScreenManager),"delayAwake")]
        [HarmonyPostfix]
        public static void addOption(ref GemaMainMenuSelectionSlot[] ___selections)
        {
            settings.Clear();
            RandomizerPlugin.deloadRando();
            RandomizerPlugin.customDiff = -1;
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
            randoSetting.AddComponent<RandomizerUI>();
            randoSetting.SetActive(false);



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
                GemaSuperSample.Instance.ChangeRenderScale(Screen.height/720f,false);
                UnityEngine.Cursor.visible = true;
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

    public class RandomizerUI : MonoBehaviour
    {
        private GameObject[][][] options;
        private int tab = 1, side = 0, selected = 0;
        private bool isEditing = false; 
        private bool finishEditing = false;
        private Rewired.Player player;
        Transform []Gears;  

        void Awake()
        {
            
            player = ReInput.players.GetPlayer(0);
            addAllOptions(this.gameObject);
            Gears = new Transform[2];
            Gears[0]=gameObject.transform.Find("Gear 1");
            Gears[1]=gameObject.transform.Find("Gear 2");
        }

        private GameObject[] getOptions(int tab,int side)
        {
            GameObject settingsAt = gameObject.transform.GetChild(4).GetChild(tab).GetChild(side).gameObject;
            GameObject[] option;
            option = new GameObject[settingsAt.transform.childCount];
            for (int i = 0; i < settingsAt.transform.childCount; i++)
            {
                GameObject t = settingsAt.transform.GetChild(i).gameObject;
                option[i] = t;
                if (t.name.Contains("Toggle"))
                {
                    UI.settings.Add(t.name, t.GetComponentInChildren<UnityEngine.UI.Toggle>());
                }
                else if (t.name.Contains("Slider"))
                {
                    UnityEngine.UI.Slider slider = t.GetComponentInChildren<UnityEngine.UI.Slider>();
                    slider.onValueChanged.AddListener(delegate { t.transform.Find("Number").gameObject.GetComponent<TextMeshProUGUI>().text = slider.value.ToString(); });
                    UI.settings.Add(t.name, slider);
                }
                
                else if(t.name.Contains("TextInput")){
                    TMP_InputField inputField = t.GetComponentInChildren<TMP_InputField>();
                    inputField.onSubmit.AddListener(delegate
                    {
                        finishEditing = true;
                        EventSystem.current.SetSelectedGameObject(null);
                    });
                    UI.settings.Add(t.name, inputField);
                }
                else if (t.name.Contains("Button"))
                {

                    if (t.name.Contains("Connect"))
                    {
                        t.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {

                            string uri = ((TMP_InputField)UI.settings["TextInput Server"]).text;
                            int port = int.Parse(((TMP_InputField)UI.settings["TextInput Port"]).text);
                            string user = ((TMP_InputField)UI.settings["TextInput User"]).text;
                            string password = ((TMP_InputField)UI.settings["TextInput Password"]).text;

                            if (ArchipelagoInterface.Instance.connectToRoom(uri, port, user, password))
                            {
                                UnityEngine.Cursor.visible = false;
                                GemaSuperSample.Instance.ChangeRenderScaleAnimation(1);
                                gameObject.SetActive(false);
                            }

                        });
                    }
                }
                
            }
            return option; 
        }

        private void addAllOptions(GameObject gameObject)
        {
            GameObject settingsAt = gameObject.transform.GetChild(4).gameObject;
            UI.finishedText = gameObject.transform.Find("Finished").gameObject;

            GameObject s,r,g;
            s = gameObject.transform.Find("Seed").gameObject;
            UI.settings.Add("Seed", s.GetComponentInChildren<TMPro.TMP_InputField>());


            s.GetComponentInChildren<TMP_InputField>().onSubmit.AddListener(delegate
            {
                finishEditing = true;
                EventSystem.current.SetSelectedGameObject(null);
            });


            r = gameObject.transform.Find("Return").gameObject;


            r.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                UnityEngine.Cursor.visible = false;
                GemaSuperSample.Instance.ChangeRenderScaleAnimation(1);

                gameObject.SetActive(false);
            });

            g = gameObject.transform.Find("Generate").gameObject;

            g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                RandomizerPlugin.customDiff = -1;
                string input = ((TMP_InputField)UI.settings["Seed"]).text;

                RandomizerPlugin.seed = input;
                
                RandomizerPlugin.createSeed();
            });
            Randomizer.settings = UI.settings;


            GameObject[]optionsMenu = [s,r,g]; 
            options = new GameObject[settingsAt.transform.childCount][][];
            for (int i = 0; i < settingsAt.transform.childCount; i++)
            {
                options[i] = new GameObject[3][];
                options[i][0] = getOptions(i, 0);
                options[i][1] = getOptions(i, 1);
                options[i][2] = optionsMenu;

            }
        }

        private void nextYOption(float i = 0)
        {
            options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(false);
            if (i < 0)
            {
                if (side == 2)
                {
                    if (selected < 2)
                    {
                        side = 0;
                    }
                    else
                    {
                        side = 1;
                    }
                    selected = 0;
                }
                else
                {
                    if (options[tab][side].Length - 1 == selected)
                    {
                        selected = side + 1;
                        side = 2;
                    }
                    else
                    {
                        selected++;
                    }
                }
            }
            else if (i > 0)
            {
                if (side == 2)
                {
                    if (selected < 2)
                    {
                        side = 0;
                    }
                    else
                    {
                        side = 1;
                    }
                    selected = options[tab][side].Length - 1;

                }
                else
                {
                    if (0 == selected)
                    {
                        selected = side + 1;
                        side = 2;
                    }
                    else
                    {
                        selected--;
                    }
                }
            }
            options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(true);
        }
        private void nextXOption(float i = 0)
        {
            options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(false);
            if (side == options[tab].Length - 1)
            {
              selected = (selected + options[tab][2].Length + (int)i) % options[tab][2].Length;
            }
            else
            {
                side = (side +options[tab].Length-1+(int)i) % (options[tab].Length - 1);
                if(selected >= options[tab][side].Length)
                {
                    selected = options[tab][side].Length - 1;
                }
            }
            options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(true);

        }

        private void setVisibleTab(int tab,bool state)
        {
            gameObject.transform.GetChild(4).GetChild(tab).gameObject.SetActive(state);
        }

        void OnEnable()
        {
            GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[0]);

            for (int t = 0; t < options.Length; t++)
            {
                for (int s = 0; s < options[t].Length; s++)
                {
                    for (int se = 0; options[t][s].Length < se; se++)
                    {
                        options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                setVisibleTab(t, false);

            }
            setVisibleTab(tab, true);
            options[tab][side][selected].transform.GetChild(0).gameObject.SetActive(true);
        }

        private void menuMovement() {
            float yAxisRepeat = InputAxisManager.Instance.GetYAxisRepeat();
            float xAxisRepeat = InputAxisManager.Instance.GetXAxisRepeat();
            if (yAxisRepeat != 0)
            {
                nextYOption(yAxisRepeat);
            }

            if (xAxisRepeat != 0)
            {
                nextXOption(xAxisRepeat);
            }
        }
        private float dtime;

        private string[] text = ["{PAGEL}+{PAGER} Disable Randomizer  {CONFIRM}Select  {BACK}Return", "{LEFT}{RIGHT}Change Value  {CONFIRM}Confirm", "{CONFIRM}Confirm"];

        void Update()
        {
            dtime += Time.deltaTime;
            for (int i = 0;i< Gears.Length;i++)
            {
                if (Gears[i] != null)
                {
                    Gears[i].Rotate(0, 0, 0.01f*((float)Math.Pow(-1,i%2)));
                }
            }
            if(isEditing)
            {
                if (dtime < 0.16f)
                {
                    return;
                }
                if (options[tab][side][selected].name.Contains("Slider"))
                {
                    UnityEngine.UI.Slider slide = options[tab][side][selected].GetComponentInChildren<UnityEngine.UI.Slider>();
                    if (slide != null)
                    {
                        slide.value += InputAxisManager.Instance.GetXAxisRepeat();
                    }
                    if (InputButtonManager.Instance.GetButton(13))
                    {
                        isEditing = false;
                        options[tab][side][selected].transform.GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(true);
                        options[tab][side][selected].transform.GetChild(2).GetChild(1).GetChild(2).gameObject.SetActive(false);
                        GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[0]);

                    }
                }
                if(finishEditing)
                {
                    isEditing = false;
                    GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[0]);
                    finishEditing = false;
                }
               else if(InputButtonManager.Instance.isUsingJoypad() && InputButtonManager.Instance.GetButton(14))
                {
                    isEditing = false;
                    finishEditing = false;
                    EventSystem.current.SetSelectedGameObject(null);
                    GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[0]);
                }
                return;
            }
            dtime = 0;
            menuMovement();
            Controller con = player.controllers.GetLastActiveController();
            if (con != null)
            {
                if(con.type == ControllerType.Mouse)
                {
                    return;
                }
            }
            if (InputButtonManager.Instance.GetButtonDown(13))
            {
                if (options[tab][side][selected].name.Contains("Toggle"))
                {
                    UnityEngine.UI.Toggle toggle = options[tab][side][selected].GetComponentInChildren<UnityEngine.UI.Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn ^= true;
                    }
                    return;
                }
                if (options[tab][side][selected].name.Contains("Seed"))
                {
                    TMPro.TMP_InputField input = options[tab][side][selected].GetComponentInChildren<TMP_InputField>();
                    if (input != null)
                    {
                        input.ActivateInputField();
                        isEditing = true;
                        GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[2]);
                    }
                    //(Steamworks.SteamUtils.ShowGamepadTextInput(Steamworks.GamepadTextInputMode.Normal, Steamworks.GamepadTextInputLineMode.SingleLine, "Enter Seed", 16, ""))                    
                    return;
                }
                if (options[tab][side][selected].name.Contains("Slider"))
                {
                    isEditing = true;
                    options[tab][side][selected].transform.GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(false);
                    options[tab][side][selected].transform.GetChild(2).GetChild(1).GetChild(2).gameObject.SetActive(true);
                    GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[1]);
                    return;
                }
                UnityEngine.UI.Button button = options[tab][side][selected].GetComponentInChildren<UnityEngine.UI.Button>();
                if(button != null)
                {
                    button.onClick.Invoke();
                }
                

            }
            if (InputButtonManager.Instance.GetButtonDown(14))
            {
                //UnityEngine.Cursor.visible = false;
                //GemaSuperSample.Instance.ChangeRenderScaleAnimation(1);
                //this.gameObject.SetActive(false);
            }
            if(InputButtonManager.Instance.GetButton(7) && InputButtonManager.Instance.GetButton(8))
            {
                if (RandomizerPlugin.toggleRandomizerPlugin())
                {
                    text[0] = text[0].Replace("Enable", "Disable");
                }
                else
                {
                    text[0] = text[0].Replace("Disable", "Enable");
                }
                GemaUIPauseMenu_BottomBarPrompt.Instance.TopBarUpdateForce(text[0]);
            }
        }
    }
}
