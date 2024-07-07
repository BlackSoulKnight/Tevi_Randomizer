using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using QFSW.QC.Containers;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeviRandomizer
{
    enum MajorItemFlag :int
    {
        ITEM_LINEBOMB =         0b1,
        ITEM_ORB =              0b10,
        ITEM_AirDash =          0b100,
        ITEM_WALLJUMP =         0b1000,
        ITEM_JETPACK =          0b10000,
        ITEM_Rotater =          0b100000,
        ITEM_DOUBLEJUMP =       0b1000000,
        ITEM_SLIDE =            0b10000000,
        ITEM_BOMBFUEL =         0b100000000,
        ITEM_WATERMOVEMENT =    0b1000000000,
        ITEM_KNIFE =            0b10000000000,
    }

    class HintSystem
    {
        // Find Text
        // Replace Text Dynamicaly
        // Remove Found Item from Text
        // for ease change all zema dialogue 


        // Custom chat need to replace certain strings parts


        static List<ChatSystem.ChatRow> extraList = new List<ChatSystem.ChatRow>();

        static ChatSystem.ChatRow createChatRow(string section,string text,string character = "Info")
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
            chatRow.emotion = "";
            chatRow.flag = "hidden";
            chatRow.pose = "";
            chatRow.position = "";
            return chatRow;
        }

        static void getHints(string section)
        {
            extraList.Clear();
            //search from top to bottom for progression items but not more than once
            int flags = 0;
            List<(string,string)> hints = new List<(string,string)>();
            foreach(KeyValuePair<ItemData,ItemData> item in RandomizerPlugin.__itemData)
            {
                if(Enum.IsDefined(typeof(MajorItemFlag), item.Value.getItemTyp().ToString()))
                {
                    int val = (int)Enum.Parse(typeof(MajorItemFlag),item.Value.getItemTyp().ToString());
                    if((val&flags) == 0)
                    {
                        string loc = RandomizerPlugin.randomizer.findLocationName(item.Key.itemID, item.Key.slotID);
                        if (loc == "Start Area") continue;
                        hints.Add((loc ,item.Value.getItemTyp().ToString()));
                        flags += val;
                        //works but it need to skip start items
                    }
                    
                }
            }
            foreach ((string,string)item in hints)
            {
                Debug.Log($"{item.Item1} {item.Item2}");
                extraList.Add(createChatRow(section, $"You may find a valuable item in {item.Item1}", "Zema"));

            }
        }

        [HarmonyPatch(typeof(ChatSystem),"StartChat")]
        [HarmonyPostfix]
        static void tryToFindNewText(ref List<ChatSystem.ChatRow> ___chatdb,ref string section, ref StoryVoiceGroup ____currentSectionVoice, ref bool ____isCurrentSectionHasVoice,ref float ___PlayFirstLine)
        {
            if (section == "chapter6_zemahouse_zema1" || section == "chapter2_zemahouse_zema1" || section == "chapter1_zemahouse_zema1" || section == "chapter1_zemahouse_zema2")
            {
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
    }
}
