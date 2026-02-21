using System;
using System.Collections.Generic;

namespace TeviRandomizer
{
    namespace TeviRandomizerSettings {
        public enum Upgradable
        {
            ITEM_KNIFE = 10,
            ITEM_ORB = 11,
            ITEM_RapidShots = 12,
            ITEM_AttackRange = 13,
            ITEM_EasyStyle = 14,
            ITEM_LINEBOMB = 15,
            ITEM_AREABOMB = 16,
            ITEM_SPEEDUP = 17,
            ITEM_AirDash = 18,
            ITEM_WALLJUMP = 19,
            ITEM_JETPACK = 20,
            ITEM_BoostSystem = 21,
            ITEM_BombLengthExtend = 22,
            ITEM_MASK = 23,
            ITEM_TempRing = 24,
            ITEM_DodgeShot = 25,
            ITEM_Rotater = 26,
            ITEM_GoldenGlove = 27,
            ITEM_OrbAmulet = 28,
            ITEM_BOMBFUEL = 29,
            ITEM_Explorer = 30,
        }
        public enum Progression_Items : int
        {
            STACKABLE_COG = ItemList.Type.STACKABLE_COG,
            I13 = ItemList.Type.I10,
            I15 = ItemList.Type.I15,
            I16 = ItemList.Type.I16,
            I19 = ItemList.Type.I19,
            I20 = ItemList.Type.I20,
            ITEM_KNIFE = ItemList.Type.ITEM_KNIFE,
            ITEM_ORB = ItemList.Type.ITEM_ORB,
            ITEM_LINEBOMB = ItemList.Type.ITEM_LINEBOMB,
            ITEM_AREABOMB = ItemList.Type.ITEM_AREABOMB,
            ITEM_BOMBFUEL = ItemList.Type.ITEM_BOMBFUEL,
            ITEM_HIJUMP = ItemList.Type.ITEM_HIJUMP,
            ITEM_SPEEDUP = ItemList.Type.ITEM_SPEEDUP,
            ITEM_SLIDE = ItemList.Type.ITEM_SLIDE,
            ITEM_WALLJUMP = ItemList.Type.ITEM_WALLJUMP,
            ITEM_DOUBLEJUMP = ItemList.Type.ITEM_DOUBLEJUMP,
            ITEM_JETPACK = ItemList.Type.ITEM_JETPACK,
            ITEM_WATERMOVEMENT = ItemList.Type.ITEM_WATERMOVEMENT,
            ITEM_MASK = ItemList.Type.ITEM_MASK,
            ITEM_OrbTypeS2 = ItemList.Type.ITEM_OrbTypeS2,
            ITEM_OrbTypeS3 = ItemList.Type.ITEM_OrbTypeS3,
            ITEM_OrbTypeC2 = ItemList.Type.ITEM_OrbTypeC2,
            ITEM_OrbTypeC3 = ItemList.Type.ITEM_OrbTypeC3,
            ITEM_AntiDecay = ItemList.Type.ITEM_AntiDecay,
            ITEM_BombLengthExtend = ItemList.Type.ITEM_BombLengthExtend,
            ITEM_AirDash = ItemList.Type.ITEM_AirDash,
            ITEM_RailPass = ItemList.Type.ITEM_RailPass,
            ITEM_AirshipPass = ItemList.Type.ITEM_AirshipPass,
            ITEM_Explorer = ItemList.Type.ITEM_Explorer,
            ITEM_TempRing = ItemList.Type.ITEM_TempRing,
            ITEM_BoostSystem = ItemList.Type.ITEM_BoostSystem,
            ITEM_AttackRange = ItemList.Type.ITEM_AttackRange,
            ITEM_EasyStyle = ItemList.Type.ITEM_EasyStyle,
            ITEM_DodgeShot = ItemList.Type.ITEM_DodgeShot,
            ITEM_RapidShots = ItemList.Type.ITEM_RapidShots,
            ITEM_OrbAmulet = ItemList.Type.ITEM_OrbAmulet,
            ITEM_GoldenGlove = ItemList.Type.ITEM_GoldenGlove,
            ITEM_Rotater = ItemList.Type.ITEM_Rotater,
            ITEM_AirSlide = ItemList.Type.ITEM_AirSlide,
            ITEM_ZCrystal = ItemList.Type.ITEM_ZCrystal,
            QUEST_Memory = ItemList.Type.QUEST_Memory,
            QUEST_Flute = ItemList.Type.QUEST_Flute,
            QUEST_Compass = ItemList.Type.QUEST_Compass,
            QUEST_LibraryKey = ItemList.Type.QUEST_LibraryKey,
            QUEST_RabiPillow = ItemList.Type.QUEST_RabiPillow,
            QUEST_GHandL = ItemList.Type.QUEST_GHandL,
            QUEST_GHandR = ItemList.Type.QUEST_GHandR,
            BADGE_AmuletQuicken = ItemList.Type.BADGE_AmuletQuicken,
            Useable_VenaBombHealBlock = ItemList.Type.Useable_VenaBombHealBlock,
            Useable_VenaBombDispel = ItemList.Type.Useable_VenaBombDispel,
            Useable_VenaBombBunBun = ItemList.Type.Useable_VenaBombBunBun,
            Useable_VenaBombBig = ItemList.Type.Useable_VenaBombBig,
            Useable_VenaBombSmall = ItemList.Type.Useable_VenaBombSmall,
        }
        public enum CustomFlags
        {
            OrbStart = 0,
            CebleStart = 1,
            CompassStart = 2,
            TempOption = 3,
            RandomizedEnemy = 4,
            RandomizedBoss = 7,
            AlwaysRandomizeEnemy = 5,
            SuperBosses = 6,
            RandomizedMusic = 8,
            RandomizedBG = 9,
            RevealPaths = 10,
            TeleporterRando = 11,
            EntranceRando,
            BackFlip,
            RabbitJump,
            RabbitWalljump,
            CKick,
            HiddenP,
            tmpOption,
            EarlyDream,
            NormalItemCraft,
            RandomMoney,
            RandomResource

        }
        public enum GoalType
        {
            AstralGear,
            BossDefeat
        }
        public enum FreePot :int
        {
            Range,
            Melee,
            Mana,
            HP,
            EP
        }
        class TeviSettings
        {

            public const ItemList.Type PortalItem = ItemList.Type.I13;
            public const ItemList.Type MoneyItem = ItemList.Type.I14;
            public const ItemList.Type CoreUpgradeItem = ItemList.Type.I15;
            public const ItemList.Type ItemUpgradeItem = ItemList.Type.I16;

            public static readonly HashSet<string> ProgressionsItems = new (Enum.GetNames(typeof(Progression_Items)));
            static Dictionary<CustomFlags, bool> SetUpFlags()
            {
                Dictionary<CustomFlags, bool> ret = new();
                foreach(var a in Enum.GetValues(typeof(CustomFlags)))
                {
                    ret.Add((CustomFlags)a, false);
                }
                return ret;
            }

            public static int[] extraPotions = [0,0,0,0,0]; // Hardcoded omo
            public static Dictionary<CustomFlags, bool> customFlags = SetUpFlags();
            public static TeleporterRando.TeleporterLoc StartLocation = TeleporterRando.TeleporterLoc.Canyon;
            public static GoalType goalType = GoalType.AstralGear;
            public static int customAtkDiff = -1;
            public static int customHpDiff = -1;
            public static int customStartDiff = -1;
            public static int GoMode = -1;
            public static string traverseMode;
            public static Dictionary<int, int> transitionData;
            public static string pluginPath = "";

            static public bool inGame = false;
        }
    }
}
