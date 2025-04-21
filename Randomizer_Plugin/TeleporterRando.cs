using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    class TeleporterRando
    {

        public enum TeleporterLoc
        {
            DesertBase,
            Canyon,
            Oasis,
            Morose,
            ForestMaze,
            Forest,
            Mines,
            Industry,
            CopperForest,
            Anathema,
            GloamWood,
            Plague,
            Ulvosa,
            SnowVillage,
            Sea,
            Ocean,
            Atlantis,
            Tartarus,
            SnowTown,
            Magma,
            OutsideDream,
            InsideDream,
            DeepDream,
            BreathEast,
            Valhalla,
            ValleyWest,
            BreathWest,
            Ruins,
            Sinner,
            Relicts,
            Catacombs,
            Lab,
            Cloister,
            MirrorGallery,
            SoulsGallery,
            Blush,
            Garden

        }

        // Enable Teleporter
        public static void setTeleporterIcon(TeleporterLoc teleporter)
        {
            byte area;
            short x, y;
            switch (teleporter)
            {
                case TeleporterLoc.Garden:
                    area = 29;
                    x = 20;
                    y = 10;
                    break;
                case TeleporterLoc.Blush:
                    area = 28;
                    x = 14;
                    y = 10;
                    break;
                case TeleporterLoc.SoulsGallery:
                    area = 27;
                    x = 15;
                    y = 13;
                    break;
                case TeleporterLoc.MirrorGallery:
                    area = 27;
                    x = 25;
                    y = 8;
                    break;
                case TeleporterLoc.Cloister:
                    area = 26;
                    x = 28;
                    y = 11;
                    break;
                case TeleporterLoc.Lab:
                    area = 25;
                    x = 23;
                    y = 12;
                    break;
                case TeleporterLoc.Catacombs:
                    area = 25;
                    x = 11;
                    y = 7;
                    break;
                case TeleporterLoc.Relicts:
                    area = 24;
                    x = 11;
                    y = 3;
                    break;
                case TeleporterLoc.Sinner:
                    area = 23;
                    x = 6;
                    y = 6;
                    break;
                case TeleporterLoc.Ruins:
                    area = 22;
                    x = 17;
                    y = 11;
                    break;
                case TeleporterLoc.BreathWest:
                    area = 21;
                    x = 23;
                    y = 3;
                    break;
                case TeleporterLoc.ValleyWest:
                    area = 21;
                    x = 10;
                    y = 12;
                    break;
                case TeleporterLoc.Valhalla:
                    area = 20;
                    x = 21;
                    y = 10;
                    break;
                case TeleporterLoc.BreathEast:
                    area = 19;
                    x = 13;
                    y = 8;
                    break;
                case TeleporterLoc.DeepDream:
                    area = 18;
                    x = 18;
                    y = 20;
                    break;
                case TeleporterLoc.InsideDream:
                    area = 18;
                    x = 18;
                    y = 11;
                    break;
                case TeleporterLoc.OutsideDream:
                    area = 18;
                    x = 18;
                    y = 7;
                    break;
                case TeleporterLoc.Magma:
                    area = 17;
                    x = 14;
                    y = 9;
                    break;
                case TeleporterLoc.SnowTown:
                    area = 16;
                    x = 10;
                    y = 12;
                    break;
                case TeleporterLoc.Tartarus:
                    area = 15;
                    x = 19;
                    y = 13;
                    break;
                case TeleporterLoc.Atlantis:
                    area = 14;
                    x = 14;
                    y = 14;
                    break;
                case TeleporterLoc.Ocean:
                    area = 14;
                    x = 22;
                    y = 6;
                    break;
                case TeleporterLoc.Sea:
                    area = 13;
                    x = 12;
                    y = 16;
                    break;
                case TeleporterLoc.SnowVillage:
                    area = 12;
                    x = 20;
                    y = 9;
                    break;
                case TeleporterLoc.Ulvosa:
                    area = 11;
                    x = 9;
                    y = 11;
                    break;
                case TeleporterLoc.Plague:
                    area = 10;
                    x = 12;
                    y = 13;
                    break;
                case TeleporterLoc.GloamWood:
                    area = 9;
                    x = 12;
                    y = 17;
                    break;
                case TeleporterLoc.Anathema:
                    area = 8;
                    x = 18;
                    y = 11;
                    break;
                case TeleporterLoc.CopperForest:
                    area = 7;
                    x = 4;
                    y = 19;
                    break;
                case TeleporterLoc.DesertBase:
                    area = 0;
                    x = 8;
                    y = 14;
                    break;
                case TeleporterLoc.Canyon:
                    area = 1;
                    x = 3;
                    y = 8;
                    break;
                case TeleporterLoc.Oasis:
                    area = 2;
                    x = 14;
                    y = 10;
                    break;
                case TeleporterLoc.Morose:
                    area = 3;
                    x = 19;
                    y = 13;
                    break;
                case TeleporterLoc.ForestMaze:
                    area = 4;
                    x = 3;
                    y = 21;
                    break;
                case TeleporterLoc.Forest:
                    area = 4;
                    x = 17;
                    y = 9;
                    break;
                case TeleporterLoc.Mines:
                    area = 5;
                    x = 7;
                    y = 12;
                    break;
                case TeleporterLoc.Industry:
                    area = 6;
                    x = 15;
                    y = 15;
                    break;
                default:
                    area = 0;
                    x = 0; y = 0;
                    break;

            }
            SaveManager.Instance.savedata.mapiconflag[area, x, y] = 4;
            for (int i = area * 256; i < area * 256 + 256; i++)
            {
                if (i < FullMap.Instance.roomtilelist.Length && FullMap.Instance.roomtilelist[i] != null && FullMap.Instance.roomtilelist[i].GetX() == x && FullMap.Instance.roomtilelist[i].GetY() == y)
                {
                    FullMap.Instance.roomtilelist[i].SetIcon(FullMap.Instance.GetRoomIcon(1));
                    FullMap.Instance.roomtilelist[i].SetVisible2(true);
                    FullMap.Instance.roomtilelist[i].gameObject.SetActive(value: true);
                }
            }
        }
        public static void setTeleporterIcon(int teleporter) => setTeleporterIcon((TeleporterLoc)teleporter);
        static void setTeleporterIcon(byte area, short x, short y)
        {
            SaveManager.Instance.savedata.mapiconflag[area, x, y] = 4;
            for (int i = area * 256; i < area * 256 + 256; i++)
            {
                if (i < FullMap.Instance.roomtilelist.Length && FullMap.Instance.roomtilelist[i] != null && FullMap.Instance.roomtilelist[i].GetX() == x && FullMap.Instance.roomtilelist[i].GetY() == y)
                {
                    FullMap.Instance.roomtilelist[i].SetIcon(FullMap.Instance.GetRoomIcon(1));
                    FullMap.Instance.roomtilelist[i].SetVisible2(true);
                }
            }
        }
    }
}
