using Bullet;
using Character;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace TeviRandomizer.Bonus_Features
{
    internal class QuickdropPatch
    {
        static float QuickDropCombo;
        static bulletScript currentDropKick;
        static bool DropKickDmgUpdated = false;
        static CharacterBase lastHit;
        public static bool isQuickdrop = false;

        static float QuickdropCooldown = 0.08f;
        public static float QuickdropTimer = 0f;
        static float BonusDamage = 1f;
        static float multiplier = 0.33f;


        public static void ChangeQuickDropBadgeDescription()
        {
            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(ItemList.Type.BADGE_QuickDropExtendA), "^Quickdrops^ combo increased by additionally $+1$. After ^quickdrop^ hits enemy, all melee attack power $+5 %$, max cumulation $33 %$\nThe cumulative effect begins to decrease after landing and disappears completely after about 5s");
            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(ItemList.Type.BADGE_QuickDropExtendB), "^Quickdrops^ combo increased by additionally $+3$.");
            RandomizerPlugin.changeSystemText("ITEMDESC." + GemaItemManager.Instance.GetItemString(ItemList.Type.BADGE_QuickDropDouble), "Number of ^quickdrops^ combo gained increased by $33 %$");
        }

        static void updateDropkickDamage(ref PlayerLogicState ___logicStatus, ref ObjectPhy ___phy_perfer, ref CharacterPhy ___cphy_perfer, ref CharacterBase __instance)
        {
            if (___logicStatus == PlayerLogicState.QUICKDROP && !DropKickDmgUpdated && currentDropKick != null && __instance.isPlayer())
            {
                QuickdropTimer = 0f;
                DropKickDmgUpdated = true;

                return;
                float num3 = 0.343525f;
                num3 += BonusDamage * QuickDropCombo;
                if (___cphy_perfer != null)
                {
                    num3 += (float)(int)___cphy_perfer.quickdrophit * 0.02f;
                }
                if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_DoubleJumpStrike) && ___phy_perfer.jumped >= 2)
                {
                    num3 *= 1.17f;
                }
                currentDropKick.SetDamage(num3);
            }
        }

        public static float CalculateAdditionalDamage()
        {
            if (isQuickdrop)
            {
                QuickdropTimer = 0f;
                return BonusDamage * QuickDropCombo;
            }
            return 0;
        }

        [HarmonyPatch(typeof(SaveManager), "TryRenewLevel")]
        [HarmonyPostfix]
        static void resetBonusDmg()
        {
            QuickDropCombo = 0;
        }

        [HarmonyPatch(typeof(CharacterBase), "BulletHurtPlayer")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> replaceFinalDamage(IEnumerable<CodeInstruction> instructions)
        {
            var original = AccessTools.Method(typeof(CharacterBase), "ReduceHealth");
            var prevCall = AccessTools.Method(typeof(CharacterBase), "AddHealth");

            var replacement = AccessTools.Method(typeof(QuickdropPatch), "CalculateAdditionalDamage");
            var line = new List<CodeInstruction>(instructions);
            for (int i = 0; i < line.Count - 6; i++)
            {
                if (line[i].opcode == OpCodes.Call && line[i].operand is MethodInfo m && m == prevCall &&
                    line[i + 1].opcode == OpCodes.Ldarg_0 &&
                    line[i + 2].opcode == OpCodes.Ldarg_S &&
                    line[i + 3].opcode == OpCodes.Ldind_R4 &&
                    line[i + 4].opcode == OpCodes.Conv_I4 &&
                    line[i + 5].opcode == OpCodes.Ldc_I4_1 &&
                    line[i + 6].opcode == OpCodes.Call &&
                    line[i + 6].operand is MethodInfo f &&
                    f == original
                    )
                {
                    i++;
                    line.Insert(i+1,new CodeInstruction(OpCodes.Ldarg_S,16));
                    line.Insert(i+2,new CodeInstruction(OpCodes.Ldarg_S,16));
                    line.Insert(i+3,new CodeInstruction(OpCodes.Ldind_R4,null));
                    line.Insert(i+4,new CodeInstruction(OpCodes.Call,replacement));
                    line.Insert(i+5,new CodeInstruction(OpCodes.Add,null));
                    line.Insert(i+6,new CodeInstruction(OpCodes.Stind_R4,null));
                    break;
                }
            }
            return line;
        }

        [HarmonyPatch(typeof(playerController), "_Update")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> removeNativeScaling(IEnumerable<CodeInstruction> instructions)
        {
            var outer = AccessTools.Field(typeof(CharacterBase), "cphy_perfer");
            var inner = AccessTools.Field(typeof(CharacterPhy), "quickdrophit");

            var line = new List<CodeInstruction>(instructions);
            for (int i = 0; i < line.Count - 8; i++)
            {
                if (line[i].opcode == OpCodes.Ldloc_S &&
                    line[i + 1].opcode == OpCodes.Ldarg_0 &&
                    line[i + 2].opcode == OpCodes.Ldfld && 
                    line[i + 2].operand is FieldInfo per && per == outer &&
                    line[i + 3].opcode == OpCodes.Ldfld &&
                    line[i + 3].operand is FieldInfo phy && phy == inner &&
                    line[i + 4].opcode == OpCodes.Conv_R4 &&
                    line[i + 5].opcode == OpCodes.Ldc_R4 &&
                    line[i + 6].opcode == OpCodes.Mul &&
                    line[i + 7].opcode == OpCodes.Add &&
                    line[i + 8].opcode == OpCodes.Stloc_S
                    )
                {
                    for(int j = 0; j <= 8; j++)
                    {
                        line[i + j].opcode = OpCodes.Nop;
                        line[i + j].operand = null;
                    }

                    break;
                }
            }
            return line;
        }
        [HarmonyPatch(typeof(CharacterBase), "BulletHurtPlayer")]
        [HarmonyPrefix]
        static void CheckForQuickdrop(BulletType type, CharacterBase owner)
        {
            isQuickdrop = type == BulletType.QUICK_DROP && owner.isPlayer();
        }

        [HarmonyPatch(typeof(CharacterBase), "BulletHurtPlayer")]
        [HarmonyPostfix]
        static void HurtCheck(ref CharacterBase owner, float damage, BulletType type, ref CharacterBase __instance, ref bool __result)
        {
            //Debug.Log($"{type} {damage} {owner}");
            if (__result)
            {
                if (type == BulletType.QUICK_DROP)
                {
                    float bonus = 1f;
                    if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_QuickDropExtendA))
                        bonus++;
                    if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_QuickDropExtendB))
                        bonus += 3;
                    if (SaveManager.Instance.GetBadgeEquipped(ItemList.Type.BADGE_QuickDropDouble))
                        bonus *= (1f+multiplier);
                    QuickDropCombo += bonus;
                    Traverse.Create(owner.phy_perfer).Field<byte>("quickDropRemaining").Value += 1;

                }
                else if (damage > 0 && (owner != null && owner.isPlayer()) || __instance.isPlayer())
                {
                    if (type == BulletType.SUMMONBUNNY_HIT) return;
                    QuickDropCombo = 0;
                }
            }
        }

        [HarmonyPatch(typeof(CharacterPhy), "canQuickDrop")]
        [HarmonyPostfix]
        static void timeCheck(ref bool __result,ref byte ___quickdrophit)
        {
            if (__result)
                __result = QuickdropTimer > QuickdropCooldown;
            if (___quickdrophit > 12)
                ___quickdrophit = 12;
        }



    }
}
