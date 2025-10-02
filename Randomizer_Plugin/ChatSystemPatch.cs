using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace TeviRandomizer
{
    enum MajorItemFlag :int
    {
        ITEM_LINEBOMB =         0b000000000001, //3
        ITEM_ORB =              0b000000000010, //3
        ITEM_AirDash =          0b000000000100, //1
        ITEM_WALLJUMP =         0b000000001000, //3
        ITEM_JETPACK =          0b000000010000, //3
        ITEM_Rotater =          0b000000100000, //3
        ITEM_DOUBLEJUMP =       0b000001000000, //1
        ITEM_SLIDE =            0b000010000000, //1
        ITEM_BOMBFUEL =         0b000100000000, //1
        ITEM_WATERMOVEMENT =    0b001000000000, //1
        ITEM_KNIFE =            0b010000000000, //3
        ITEM_AirSlide =         0b100000000000, //1
    }

    class ChatSystemPatch
    {


        // Custom chat need to replace certain strings parts
        public const int numberOfHints = 24;
        public static (string,string,byte)[] hintList = new (string,string,byte)[numberOfHints];

        static List<ChatSystem.ChatRow> extraList = new List<ChatSystem.ChatRow>();
        static Dictionary<string,List<ChatSystem.ChatRow>> customTexts = loadCustomTexts();
        static private Dictionary<string, List<ChatSystem.ChatRow>> loadCustomTexts()
        {
            string path = RandomizerPlugin.pluginPath + "/resource/customTexts/";
            Dictionary<string, List<ChatSystem.ChatRow>> dict = new Dictionary<string, List<ChatSystem.ChatRow>>();
            JObject texts = JObject.Parse(File.ReadAllText(path + "textCollection.json"));
            JObject jp = JObject.Parse(File.ReadAllText(path + "jp.json"));
            JObject ko = JObject.Parse(File.ReadAllText(path + "ko.json"));
            JObject ru = JObject.Parse(File.ReadAllText(path + "ru.json"));
            JObject sc = JObject.Parse(File.ReadAllText(path + "sc.json"));
            JObject sp = JObject.Parse(File.ReadAllText(path + "sp.json"));
            JObject tc = JObject.Parse(File.ReadAllText(path + "tc.json"));
            JObject uk = JObject.Parse(File.ReadAllText(path + "uk.json"));

            foreach (var chat in texts)
            {
                List<ChatSystem.ChatRow> chatRows = new List<ChatSystem.ChatRow>();
                foreach(JObject row in (JArray)chat.Value)
                {
                    var newRow = createChatRow(chat.Key,(string)row["text"], (string)row["character"], (string)row["flag"], (string)row["position"], (string)row["emotion"], (string)row["pose"]);
                    if (jp.ContainsKey((string)row["textId"]))
                        newRow.dialog_japanese = (string)jp[(string)row["textId"]];
                    if (ko.ContainsKey((string)row["textId"]))
                        newRow.dialog_korean = (string)jp[(string)row["textId"]];
                    if (ru.ContainsKey((string)row["textId"]))
                        newRow.dialog_russian = (string)jp[(string)row["textId"]];
                    if (sc.ContainsKey((string)row["textId"]))
                        newRow.dialog_schinese = (string)jp[(string)row["textId"]];
                    if (sp.ContainsKey((string)row["textId"]))
                        newRow.dialog_spanish = (string)jp[(string)row["textId"]];
                    if (tc.ContainsKey((string)row["textId"]))
                        newRow.dialog_tchinese = (string)jp[(string)row["textId"]];
                    if (uk.ContainsKey((string)row["textId"]))
                        newRow.dialog_ukrainian = (string)jp[(string)row["textId"]];
                    chatRows.Add(newRow);
                }
                dict.Add(chat.Key,chatRows);
            }
            return dict;
        }



        static ChatSystem.ChatRow createChatRow(string section,string text,string character = "Info",string flag = "hidden",string position = "",string emotion ="",string pose="" )
        {
            ChatSystem.ChatRow chatRow = new ChatSystem.ChatRow();
            chatRow.character = character;
            chatRow.section = section;
            chatRow.dialog_english = text;
            chatRow.dialog_spanish = chatRow.dialog_english;
            chatRow.dialog_japanese = chatRow.dialog_english;
            chatRow.dialog_korean = chatRow.dialog_english;
            chatRow.dialog_schinese = chatRow.dialog_english;
            chatRow.dialog_tchinese = chatRow.dialog_english;
            chatRow.dialog_russian = chatRow.dialog_english;
            chatRow.dialog_spanish = chatRow.dialog_english;
            chatRow.dialog_ukrainian = chatRow.dialog_english;
            chatRow.emotion = emotion;
            chatRow.flag = flag;
            chatRow.pose = pose;
            chatRow.position = position;
            return chatRow;
        }

        static void getHints(string section)
        {
            if (RandomizerPlugin.__itemData.Count == 0) return;
            extraList.Clear();
            //search from top to bottom for progression items
            if(ArchipelagoInterface.Instance.isConnected) { 
                    extraList.Add(createChatRow(section, $"Please use the common AP client to get Hints.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
                return;
            }
            int collected = LocationTracker.getListLenght();
            int a = (int)(RandomizerPlugin.__itemData.Count*0.75f * (float)(1f / numberOfHints));
            int nextHint = a  - (collected % a);
            extraList.Add(createChatRow(section, $"You have checked {collected} Locations from a total of {RandomizerPlugin.__itemData.Count}", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
            System.Random rand = new System.Random(RandomizerPlugin.seed.GetHashCode());

            for (int i = 0; i < Math.Floor((double)collected / a); i++)
            {

                if (i >= hintList.Length) {
                    if(extraList.Count == 0) extraList.Add(createChatRow(section, $"No more Hints left.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
                    return;
                };
                if (!LocationTracker.checkLocation(hintList[i].Item1))
                {
                    string localizeItem = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + hintList[i].Item2, false);
                    extraList.Add(createChatRow(section, $"You may find {localizeItem} in {hintList[i].Item1}.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
                }
            }
            int alreadyCollectedItems = 0;
            for(int i = (int)Math.Floor((double)collected / a); i< numberOfHints;i++)
            {
                if (RandomizerPlugin.checkItemGot((ItemList.Type)Enum.Parse(typeof(ItemList.Type), hintList[i].Item2), hintList[i].Item3)) {
                    alreadyCollectedItems++;
                    continue; 
                }
                else
                {
                    break;
                }


            }
            extraList.Add(createChatRow(section, $"The next Hint is after {nextHint + alreadyCollectedItems * a} Items available.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));




        }
        static public void addNewChatLine(string section, string text, string character = "Info", string flag = "hidden", string position = "", string emotion = "", string pose = "")
        {
            extraList.Add(createChatRow(section, text, character, flag, position, emotion, pose));
        }
        static public void startChat()
        {
            ChatSystem.Instance.setStatus(SystemVar.Status.OPEN);
            ChatSystem.Instance.StartChat(1, "CustomChat");
        }

        [HarmonyPatch(typeof(ChatSystem),"StartChat")]
        [HarmonyPostfix]
        static void tryToFindNewText(ref List<ChatSystem.ChatRow> ___chatdb,ref string section, ref StoryVoiceGroup ____currentSectionVoice, ref bool ____isCurrentSectionHasVoice,ref float ___PlayFirstLine)
        {
            if (section == "chapter6_zemahouse_zema1" || section == "chapter2_zemahouse_zema1" || section == "chapter1_zemahouse_zema1" || section == "chapter1_zemahouse_zema2")
            {
                if (RandomizerPlugin.__itemData.Count == 0) return;
                ____isCurrentSectionHasVoice = false;
                ___PlayFirstLine = 0f;
                ___chatdb.Clear();
                getHints(section);
                ___chatdb.CopyFrom(extraList);
                extraList.Clear();

            }
            if (___chatdb.Count == 0)
            {
                Debug.Log($"[Randomizer] Search for Custom Chat");
                if (customTexts.ContainsKey(section))
                {
                    ___chatdb.CopyFrom(customTexts[section]);
                }
                else
                {
                    Debug.LogWarning($"[CustomChat] Failed to load {section}");
                    ___chatdb.Add(createChatRow("meh", $"Failed to load Chat {section}"));
                }
            }
        }
        [HarmonyPatch(typeof(CharacterVoiceManager), "ReleaseVoiceGroup")]
        [HarmonyPostfix]
        static void test1()
        {
            //Debug.LogWarning("ITS ME");

        }
        [HarmonyPatch(typeof(GemaChatLogManager), "AddLog")]
        [HarmonyPostfix]
        static void test2()
        {
            //Debug.LogWarning("NO,ITS ME");
        }

        [HarmonyPatch(typeof(ChatSystem),"FindChatExist")]
        [HarmonyPostfix]
        static void checkForCustomChat(ref bool __result,ref string section)
        {
            if (!__result)
            {
                customTexts.ContainsKey(section);
                __result = true;
            }
        }
    }

}
