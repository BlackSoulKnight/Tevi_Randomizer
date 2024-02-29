using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




[BepInPlugin("tevi.plugins.mapEditor", "MapEditor", "0.1.0.0")]
[BepInProcess("TEVI.exe")]

public class MapEditor : BaseUnityPlugin
{
    private void Awake()
    {

        var instance = new Harmony("MapEditor");
        instance.PatchAll(typeof(MapEditor));

        Logger.LogInfo($"Plugin MapEditor is loaded!");

    }



    // Editor Testing


    static public GameObject buttonPrefab()
    {

        GameObject tilesetButton = new GameObject("TilesetButton", typeof(RectTransform));


        tilesetButton.tag = "TileButton";
        tilesetButton.layer = 5;
        GameObject button = new GameObject("Button", typeof(RectTransform));
        GameObject text1 = new GameObject("text", typeof(RectTransform));
        GameObject text2 = new GameObject("text", typeof(RectTransform));
        button.AddComponent<CanvasRenderer>();
        button.AddComponent<Image>();
        button.AddComponent<Button>();
        button.AddComponent(typeof(TilesetButton));
        TilesetButton tb = button.GetComponent<TilesetButton>();

        button.layer = 11;
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(tb.ClickButton);
        text1.AddComponent<CanvasRenderer>();
        text2.AddComponent<CanvasRenderer>();
        text1.AddComponent(typeof(TMPro.TextMeshProUGUI));
        text1.layer = 5;
        text2.layer = 5;


        button.AddComponent(typeof(GridLayout));


        button.transform.SetParent(tilesetButton.transform);
        text1.transform.SetParent(button.transform);
        text2.transform.SetParent(tilesetButton.transform);


        return tilesetButton;


    }



    [HarmonyPatch(typeof(TilesetEditor), "Start")]
    [HarmonyPrefix]
    static void b(ref TilesetEditor __instance, ref GameObject ___tb_prefer, ref WorldManager ___worldmanager_prefer, ref Transform ___canvasRoot, ref GameObject ___editorMenu)
    {
        if (TilesetEditor.Instance == null)
        {
            ___tb_prefer = buttonPrefab();
            ___worldmanager_prefer = WorldManager.Instance;
            ___canvasRoot = __instance.transform;
            ___editorMenu = __instance.gameObject;
            Traverse.Create(WorldManager.Instance).Field("_tileeditor_prefab").SetValue(__instance);
            Traverse.Create(__instance).Method("CUstomStart").GetValue();
        }
    }


    [HarmonyPatch(typeof(EditorHUD), "Awake")]
    [HarmonyPostfix]
    static void test(ref EditorHUD __instance, ref Button ___togglebutton, ref Button[] ___layerbuttonlist, ref Button[] ___numberbuttonlist, ref TextMeshProUGUI[] ___numberbuttontext)
    {

        GameObject button = new GameObject("Button");
        button.AddComponent(typeof(Button));
        button.AddComponent<TextMeshProUGUI>();
        button.transform.SetParent(__instance.transform);
        ___togglebutton = button.GetComponent<Button>();


        for (int i = 0; i < 10; i++)
        {
            ___numberbuttonlist[i] = button.GetComponent<Button>();
            ___numberbuttontext[i] = button.GetComponent<TextMeshProUGUI>();
            if (i < 5)
            {
                ___layerbuttonlist[i] = button.GetComponent<Button>();
            }
        }



        GameObject canvas = new GameObject("TestCanvas", typeof(RectTransform));
        GameObject tilesetEditor = new GameObject("TilesetEditor", typeof(RectTransform));
        RectTransform tilesetSize = tilesetEditor.GetComponent<RectTransform>();



        canvas.AddComponent<Canvas>();
        Canvas can = canvas.GetComponent<Canvas>();
        can.renderMode = RenderMode.ScreenSpaceCamera;
        can.sortingOrder = 32767;
        can.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

        canvas.AddComponent<GraphicRaycaster>();

        tilesetEditor.transform.SetParent(canvas.transform);
        tilesetEditor.AddComponent(typeof(TilesetEditor));
        tilesetEditor.AddComponent<GridLayoutGroup>();
        GridLayoutGroup glg = tilesetEditor.GetComponent<GridLayoutGroup>();
        glg.spacing = new Vector2(1, 1);
        glg.cellSize = new Vector2(128, 128);

        tilesetSize.sizeDelta = new Vector2(1113, 300);
        tilesetSize.localPosition = new Vector3(-70, 0, 0);



        RectTransform rT = tilesetEditor.GetComponent<RectTransform>();



    }

    [HarmonyPatch(typeof(TilesetEditor), "CreateTileButton")]
    [HarmonyPrefix]
    static bool overrideCreateTileButton(int x, int y, ref Transform ___canvasRoot, ref GameObject ___tb_prefer)
    {
        GameObject gameObject = Object.Instantiate<GameObject>(___tb_prefer);
        gameObject.transform.SetParent(___canvasRoot);
        Image componentInChildren = gameObject.GetComponentInChildren<Image>();
        int num = x + y * 12;
        componentInChildren.sprite = null;
        TilesetButton _button = gameObject.GetComponentInChildren<TilesetButton>();
        gameObject.GetComponentInChildren<TilesetButton>().tileID = num;
        Button button = gameObject.GetComponentInChildren<Button>();
        button.onClick.AddListener(_button.ClickButton);
        return false;
    }


    [HarmonyPatch(typeof(WorldManager), "_Update")]
    [HarmonyPostfix]
    static void addLayerSelection()
    {
        if (PauseMenu.Instance.getStatus() == SystemVar.Status.OFF && EditorHUD.Instance.ModeSelect != 0 && !Input.GetKeyDown(KeyCode.CapsLock) && (int)EditorManager.instance.Editor_LayerSelected >= 0 && (int)EditorManager.instance.Editor_LayerSelected <= 16)
        {

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                EditorHUD.Instance.ChangeModeSelect(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                EditorHUD.Instance.ChangeModeSelect(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                EditorHUD.Instance.ChangeModeSelect(3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                EditorHUD.Instance.ChangeModeSelect(4);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                EditorHUD.Instance.ChangeModeSelect(5);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                EditorHUD.Instance.ChangeModeSelect(6);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                EditorHUD.Instance.ChangeModeSelect(7);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                EditorHUD.Instance.ChangeModeSelect(8);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                EditorHUD.Instance.ChangeModeSelect(9);
            }

        }
    }





}
