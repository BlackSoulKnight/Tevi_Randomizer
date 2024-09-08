using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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

    class HintSystem
    {


        // Custom chat need to replace certain strings parts
        public const int numberOfHints = 24;
        public static (string,string,byte)[] hintList = new (string,string,byte)[numberOfHints];

        static List<ChatSystem.ChatRow> extraList = new List<ChatSystem.ChatRow>();

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
            int collected = 0;
            List<(string,string,bool)> hints = new List<(string,string,bool)>();
            foreach(KeyValuePair<ItemData,ItemData> item in RandomizerPlugin.__itemData)
            {
                bool f = false;
                if (RandomizerPlugin.checkRandomizedItemGot(item.Key.getItemTyp(), item.Key.getSlotId()))
                {
                    collected++;
                    f = true;
                }
                if (Enum.IsDefined(typeof(MajorItemFlag), item.Value.getItemTyp().ToString()))
                {

                    string loc = RandomizerPlugin.randomizer.findLocationName(item.Key.itemID, item.Key.slotID);
                    hints.Add((loc ,item.Value.getItemTyp().ToString(),f));
                }

            }
            int a = (int)(RandomizerPlugin.__itemData.Count*0.75f * (float)(1f / numberOfHints));
            int nextHint = a  - (collected % a);
            bool[] order = new bool[hints.Count];
            System.Random rand = new System.Random(RandomizerPlugin.seed.GetHashCode());
            for (int i = 0; i < Math.Floor((double)collected / a); i++)
            {

                if (i >= hints.Count) {
                    if(extraList.Count == 0) extraList.Add(createChatRow(section, $"No more Hints left.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
                    return;
                };
                if (!RandomizerPlugin.checkItemGot((ItemList.Type)Enum.Parse(typeof(ItemList.Type), hintList[i].Item2), hintList[i].Item3))
                {
                    string localizeItem = Localize.GetLocalizeTextWithKeyword("ITEMNAME." + hintList[i].Item2, false);
                    extraList.Add(createChatRow(section, $"You may find {localizeItem} in {hintList[i].Item1}.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));
                }
            }

            extraList.Add(createChatRow(section,$"The next Hint is in {nextHint} Items available.", "Professor Zema", "", "left", "e_1happy", "a_1thinking"));




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

            }
            if (___chatdb.Count == 0)
            {
                Debug.Log($"[Randomizer] Search for Custom Chat");


                ___chatdb.CopyFrom(extraList);
                extraList.Clear();
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

    }

}
