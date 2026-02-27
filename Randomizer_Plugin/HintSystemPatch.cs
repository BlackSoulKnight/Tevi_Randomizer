using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using EventMode;
using HarmonyLib;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace TeviRandomizer
{
    class TeviHint : IEquatable<TeviHint>
    {
        public string ItemName;
        public string LocationName;
        byte area,x,y;
        public TeviHint(string itemName,string locationName, byte area, byte x, byte y)
        {
            ItemName = itemName;
            LocationName = locationName;
            this.area = area;
            this.x = x;
            this.y = y;
        }
        public TeviHint(byte _a,byte _x,byte _y)
        {
            this.area = _a;
            this.x = _x;
            this.y = _y;
        }

        public bool Equals(TeviHint? other) => other is not null && other.area == area && other.x == x && other.y == y;
        public override bool Equals(object? obj) => Equals(obj as TeviHint);
        public override int GetHashCode() => (area*10000+x*100+y).GetHashCode();
        public static int HashCode(byte area, byte x, byte y) => (area * 10000 + x * 100 + y).GetHashCode();
    }
    class HintSystemPatch : MonoBehaviour
    {
        private static List<TeviHint> HintQueue = new();
        static Hashtable teviHints = new();
        public static bool CreateCustomTodo(string loc,string ItemName)
        {
            if (!LocationTracker.LocationMapPositions.ContainsKey(loc))
            {
                Debug.LogError($"No Location found for {loc}");
                return false;
            }
            if (LocationTracker.checkLocation(loc))
                return true;
            var spot = LocationTracker.LocationMapPositions[loc];
            if (spot.Area > 35)
                return false;
            var hint = new TeviHint(ItemName, loc, spot.Area, spot.X, spot.Y);

            if (teviHints.ContainsKey(hint.GetHashCode())) {
                if (((TeviHint)teviHints[hint.GetHashCode()]).LocationName != loc)
                    HintQueue.Add(hint);
                else
                    return false;
            }
            else {
                teviHints.Add(hint.GetHashCode(), hint);
                Traverse.Create(SaveManager.Instance).Method("Really_AddTodo", new object[] { spot.Area, spot.X, spot.Y, Todo.MAX, false }).GetValue();
            }
            return true;
        }
        public static void RemoveCustomTodo(string loc)
        {
            var spot = LocationTracker.LocationMapPositions[loc];
            TeviHint tmp = new(spot.Area, spot.X, spot.Y);
            if (!teviHints.ContainsKey(TeviHint.HashCode(spot.Area, spot.X, spot.Y)))
            {
                if (HintQueue.Contains(tmp))
                {
                    int index = HintQueue.FindIndex(h => h.LocationName == loc);
                    if (index >= 0)
                        HintQueue.RemoveAt(index);
                }
                UpdateHintQueue();
                return;
            }
            teviHints.Remove(TeviHint.HashCode(spot.Area, spot.X, spot.Y));
            ref var savedata = ref SaveManager.Instance.savedata;
            var MAXTODO = Traverse.Create(SaveManager.Instance).Field<int>("MAXTODO").Value;
            for (int i = 0; i < MAXTODO; i++)
            {
                if ((uint)savedata.todoID[i] == (uint)Todo.MAX && savedata.todoX[i] == spot.X && savedata.todoY[i] == spot.Y && savedata.todoA[i] == spot.Area)
                {
                    Debug.Log("[SaveManager] Removed Custom Todo from save file : " + " (" + savedata.todoA[i] + " , " + savedata.todoX[i] + " , " + savedata.todoY[i] + ")");
                    FullMap.Instance.SetMiniMapIcon(savedata.todoA[i], savedata.todoX[i], savedata.todoY[i], Icon.WALK);
                    savedata.todoID[i] = 0;
                    savedata.todoX[i] = 0;
                    savedata.todoY[i] = 0;
                    savedata.todoA[i] = 0;
                }
            }
            UpdateHintQueue();
        }
        static void UpdateHintQueue()
        {
            foreach(var hint in HintQueue.ToArray())
            {
                if(CreateCustomTodo(hint.LocationName,hint.ItemName))
                    HintQueue.Remove(hint);
            }
        }


        static void TestTodo()
        {
            foreach (var loc in LocationTracker.LocationMapPositions.Keys)
            {
                try {
                    CreateCustomTodo(loc, loc);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        [HarmonyPatch(typeof(PauseFrame), "UpdateDetailText")]
        [HarmonyPostfix]
        static void changeHintText(ref Todo id,ref PauseFrame __instance,ref TextMeshPro ___todoDetailText,ref TextMeshPro ___todoDetailDescText)
        {
            if (id == Todo.MAX)
            {
                GemaTodoSelection gemaTodoSelection = PauseMenu.Instance.TodoSelections[__instance.inTodoSelected];
                var area = gemaTodoSelection.GetA();
                var x = gemaTodoSelection.GetX();
                var y = gemaTodoSelection.GetY();
                int hash = TeviHint.HashCode(area, x, y);
                if (teviHints.ContainsKey(hash))
                    ___todoDetailDescText.text = ((TeviHint)teviHints[hash]).ItemName;
                else
                    ___todoDetailDescText.text = $"{area} {x} {y}";
                ___todoDetailText.text = ((TeviHint)teviHints[hash]).LocationName;
            }

        }
        [HarmonyPatch(typeof(GemaTodoSelection), "SetData")]
        [HarmonyPostfix]
        static void changeHintText2(ref byte _a, ref byte _x, ref byte _y,ref byte _ID,ref TextMeshPro ___tmpro)
        {
            if (_ID == (byte)Todo.MAX)
            {
                int hash = TeviHint.HashCode(_a, _x, _y);
                if (teviHints.ContainsKey(hash))
                    ___tmpro.text = "<sprite name=\"mark_3\"> " + ((TeviHint)teviHints[hash]).ItemName;
            }

        }
        [HarmonyPatch(typeof(PauseMenu),"Start")]
        [HarmonyPostfix]
        static void increaseAmount(ref GemaTodoSelection[] ___TodoSelections, ref GemaTodoSelection ___gts_perfer,ref GameObject ___tss_holder, ref float ___TODOOFFY,ref PauseMenu __instance)
        {
            var newArray = new GemaTodoSelection[200];
            for (var i = 0; i< ___TodoSelections.Length;i++)
            {
                newArray[i] = ___TodoSelections[i];
            }
            for (int i = ___TodoSelections.Length; i < newArray.Length; i++)
            {
                GemaTodoSelection gemaTodoSelection = UnityEngine.Object.Instantiate(___gts_perfer);
                gemaTodoSelection.transform.SetParent(___tss_holder.transform);
                gemaTodoSelection.transform.localPosition = new Vector3(17f, ___TODOOFFY * (float)i, 0f);
                newArray[i] = gemaTodoSelection;
            }
            ___TodoSelections = newArray;
            Traverse.Create(__instance).Field<int>("MAXTODOSELECTION").Value = 200;
        }

    }
}
