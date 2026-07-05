
using Game;
using HarmonyLib;
using Map;
using Rewired.ComponentControls.Data;
using System.Collections.Generic;
using System.Reflection.Emit;
using TeviRandomizer.TeviRandomizerSettings;
using TMPro;
using UnityEngine;

namespace TeviRandomizer
{
    internal class TeviSkin
    {
        public string SkinName;
        public RuntimeAnimatorController Skin;
        public Material Material = null;
        public TeviSkin(string skinname, RuntimeAnimatorController skin, Material material = null)
        {
            SkinName = skinname;
            Skin = skin;
            Material = material;
        }
    }

    internal class TeviSkinManager
    {
        static AssetBundle TeviSkins = AssetBundle.LoadFromFile(TeviSettings.pluginPath + "/resource/Tevi skins/tevi_skins");
        static private CharacterBase Player => EventManager.Instance.mainCharacter;

        public static readonly TeviSkin Invisibile = new("Invisible", TeviSkins.LoadAsset<RuntimeAnimatorController>("tevi_base_effects"));


        //{ "tevi_bunny", },
        //{ "tevi_erina", TeviSkins.LoadAsset<RuntimeAnimatorController>("tevi_erina")},

        static List<TeviSkin> SkinList = new() {
            new("Bunny",TeviSkins.LoadAsset<RuntimeAnimatorController>("tevi_bunny")),
        };
        const short BaseSkinCount = 3;


        static public RuntimeAnimatorController LoadMainCharacterSkin()
        {
            var skinNr = SettingManager.Instance.GetSettingValue(SettingName.S_DISPLAY_SKIN_TEVI);
            if (skinNr <= BaseSkinCount)
            {
                string text = SaveManager.Instance.CheckSkin(EventManager.Instance.mainCharacter.type.ToString());
                return AreaResource.Instance.GetNPC(text);
            }
            if(skinNr - BaseSkinCount-1 >= SkinList.Count)
                return AreaResource.Instance.GetNPC("Tevi");

            if (SkinList[skinNr - BaseSkinCount - 1].Material != null)
                Player.spranim_prefer.SetMaterial(SkinList[skinNr - BaseSkinCount - 1].Material);
            else
                Player.spranim_prefer.SetMaterial(CommonResource.Instance.mat_SpriteLightingVertex);

            return SkinList[skinNr-BaseSkinCount-1].Skin;
        }

        static public bool FindSkin(string name,out RuntimeAnimatorController skin)
        {
            skin = null;
            foreach (var tmp in SkinList)
            {
                if (tmp.SkinName == name)
                {
                    skin = tmp.Skin;
                    return true;
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(GemaSettingSlot),"SetValue")]
        [HarmonyPrefix]
        static bool ExtraSettingsText(ref int value,ref SettingName ___settingName, ref TextMeshProUGUI ___valueText)
        {
            switch (___settingName)
            {
                case SettingName.S_DISPLAY_SKIN_TEVI:
                    if (value <= BaseSkinCount)
                        return true;
                    if (value - BaseSkinCount-1 >= SkinList.Count)
                        ___valueText.text = "OutOfBounds";
                    else
                        ___valueText.text = SkinList[value - BaseSkinCount-1].SkinName;
                    break;
                default:
                    return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(SettingManager), "GetSettingMaxValue")]
        [HarmonyPostfix]
        static void MaxSkin(ref SettingName settingName, ref int __result)
        {
            switch (settingName)
            {
                case SettingName.S_DISPLAY_SKIN_TEVI:
                    __result = BaseSkinCount + SkinList.Count;
                    break;
            }
        }

        [HarmonyPatch(typeof(SaveManager), "CheckSkin")]
        [HarmonyPrefix]
        static bool CheckSkin(ref string usetype, ref string __result)
        {
            if (!usetype.Equals(Character.Type.Tevi.ToString()) && !usetype.Equals("Tevi!"))
                return true;
            int skinNR = SettingManager.Instance.GetSettingValue(SettingName.S_DISPLAY_SKIN_TEVI);
            if (skinNR == 1 || skinNR == 0)
            {
                __result = "tevi";
                return false;
            }
            if (MainVar.instance.isDLCInstalled(DLC.Story_DLC1) && skinNR == 2)
            {
                __result = "tevi_b";
                return false;
            }
            if (MainVar.instance.isDLCInstalled(DLC.Skin_Bunny) && skinNR == 3)
            {
                __result = "tevi_c";
                return false;
            }

            if (skinNR - BaseSkinCount-1 >= SkinList.Count)
                __result = "tevi";
            else
                __result = SkinList[skinNR - BaseSkinCount-1].SkinName;
            return false;
        }

    }
}
