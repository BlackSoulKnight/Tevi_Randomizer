using Bullet;
using Character;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TeviRandomizer
{
    class TeviTraps:MonoBehaviour
    {
        public enum Traps:int
        {
            ReverseCam,
            DoubleTime,
            Yeet,
            Debuff,
            Taunt,
            ReduceJump,
            None
        }
        private static void ChangeCam(float zoomLevel) => MainVar.instance.CamZoom = zoomLevel;
        private static void ChangeGameSpeed(float speed) => MainVar.instance.GameSpeed = speed;
        private static readonly int[] Debuffs = [74, 75, 77, 78, 79, 82, 85, 86, 88, 90, 92, 98, 99, 102, 103, 105, 107, 108, 109, 111]; //99+100 104???
        public static int RandomDebuff => Debuffs[UnityEngine.Random.Range(0, Debuffs.Length)];
        public static float ReverseCamDuration = 0;
        public static float DoubleTimeDuration = 0;
        public static float ReducedJumpHeightDuration = 0;
        public static bool YeetBunny = false;
        public static Queue<Taunt> TauntQueue = new Queue<Taunt>();
        public static void ApplyDebuff(int debuff)
        {
            float time = 1;
            int level = 1;
            switch (debuff)
            {
                case 74:
                case 75:
                    level = 17;
                    time = 15;
                    break;
                case 77:
                    level = 10;
                    time = 5;
                    break;
                case 78:
                    time = 30;
                    level = SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP) / 10;
                    level = (level < 2 ? 2 : level);
                    break;
                case 79:
                    level = 100;
                    time = 5;
                    break;
                case 82:
                    level = 255;
                    time = 30;
                    break;
                case 85:
                    level = 40;
                    time = 15;
                    break;
                case 86:
                    level = 1;
                    time = 10;
                    break;
                case 88:
                    level = 11;
                    time = 5;
                    break;
                case 90:
                    level = 34;
                    time = 10;
                    break;
                case 92:
                    level = 10;
                    time = 30;
                    break;
                case 98:
                    level = 255;
                    time = 10;
                    break;
                case 99:
                    time = 30;
                    BuffManager.Instance.GiveBuff(EventManager.Instance.mainCharacter, (BuffType)100, time, (byte)level, 0);
                    break;
                case 102:
                    level = 10;
                    time = 15;
                    break;
                case 103:
                    level = 5;
                    time = 120;
                    break;
                case 105:
                    level = 10;
                    time = 60;
                    break;
                case 107:
                    level = 1;
                    time = 5;
                    break;
                case 108:
                    level = 5;
                    time = 30;
                    break;
                case 109:
                    level = UnityEngine.Random.RandomRangeInt(1, SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_EP)+1);
                    time = 60;
                    break;
                case 111:
                    level = UnityEngine.Random.RandomRangeInt(1, SaveManager.Instance.GetStackableCount(ItemList.Type.STACKABLE_HP)+1);
                    time = 30;
                    break;
            }
            BuffManager.Instance.GiveBuff(EventManager.Instance.mainCharacter, (BuffType)debuff, time, (byte)level, 0);
        }
        public static void SpawnBun()
        {
            EventManager instance = EventManager.Instance;
            CharacterBase characterBase = instance.CreateEnemy(Character.Type.Rabbit, Character.BossType.SUMMON);
            instance.AddActor(characterBase);
            instance.SetCharacterPosition(characterBase, EventMode.CreateType.BASEONPLAYER, EventMode.CreateType.BASEONPLAYER, 0f, 0f);
            characterBase.direction = Character.Direction.NOTTOPLAYER;
            characterBase.enemy_perfer.InitAI();
            //characterBase.spranim_prefer.pixel.anim.runtimeAnimatorController = ResourcePatch.SnowBunny;
        }
        private static ObjectPhy Playert_phys => EventManager.Instance.mainCharacter.phy_perfer;
        public static void useTaunt()
        {
            int taunt = UnityEngine.Random.Range(0,(int)Taunt.MAX);

            EventManager.Instance.mainCharacter.ChangeLogicStatus(PlayerLogicState.TAUNT);
            EventManager.Instance.mainCharacter.phy_perfer.SetCounter(10, taunt);
            EventManager.Instance.mainCharacter.phy_perfer.SetCounter(11, 0f);
            EventManager.Instance.mainCharacter.phy_perfer.SetCounter(12, 0f);
            EventManager.Instance.mainCharacter.phy_perfer.SetCounter(13, UnityEngine.Random.Range(0f, 99f));
            EventManager.Instance.mainCharacter.phy_perfer.SetCounter(14, 0f);
            GameSystem.Instance.NoCharacterInput = 0.25f;
        }
        /*
        public void shootBullet()
        {
            
            bulletScript bulletScript3 = BulletManager.Instance.ShootBullet(b, BulletType.Tevi_AIBulletOrb, (k == 0) ? SpriteType.BulletOrbS : SpriteType.BulletOrbC);
            if (bulletScript3 != null)
            {
                bulletScript3.SetSpeedAngleDamageSize(0f, 0f, 0f, 0f, 0f);
                bulletScript3.SetMaterialDefault();
                bulletScript3.SetOrder(235 + k * 2);
                bulletScript3.SetLife(99999f);
                bulletScript3.SetSpriteSize(2f, justSpawn: false);
                bulletScript3.SetCounter(9, k);
                bulletScript3.SetMaterial(AreaResource.Instance.GetMaterialByName("TeviB Material"));
                bulletScript3.SetPosition(b.t.position);
                bulletScript3._render.sprite;
            }
        }
        */
        public static Traps NameToTrap(string name)
        {
            switch (name)
            {
                case "Yeet":
                    return Traps.Yeet;
                case "Debuff":
                    return Traps.Debuff;
                case "Double Time":
                    return Traps.DoubleTime;
                case "Reverse Camera":
                    return Traps.ReverseCam;
                case "Taunt":
                    return Traps.Taunt;
                case "Reduce Jump Height":
                    return Traps.ReduceJump;
                default:
                    return Traps.None;
            }
        }
        public static string TrapToName(Traps trap)
        {
            switch (trap)
            {
                case Traps.Yeet:
                    return "Yeet";
                case Traps.Debuff:
                    return "Debuff";
                case Traps.DoubleTime:
                    return "Double Time";
                case Traps.ReverseCam:
                    return "Reverse Camera";
                case Traps.Taunt:
                    return "Taunt";
                case Traps.ReduceJump:
                    return "Reduce Jump Height";
                default:
                    return "None";
            }
        }
        
        void Update()
        {
            if (GameSystem.Instance == null || GameSystem.Instance.isAnyPause()) return;
            if (DoubleTimeDuration>0 && EventManager.Instance.Mode == EventMode.Mode.OFF)
            {
                DoubleTimeDuration -= Time.unscaledDeltaTime;
                ChangeGameSpeed(2);
                if (DoubleTimeDuration <= 0)
                    ChangeGameSpeed(1);
            }
            if (ReducedJumpHeightDuration>0 && EventManager.Instance.Mode == EventMode.Mode.OFF)
            {
                ReducedJumpHeightDuration -= Time.unscaledDeltaTime;
                Playert_phys.jumpHeight = 100;
                if (ReducedJumpHeightDuration <= 0)
                    Playert_phys.jumpHeight = 200;
            }

            if (ReverseCamDuration>0 && EventManager.Instance.Mode == EventMode.Mode.OFF)
            {
                ReverseCamDuration -= Time.unscaledDeltaTime;
                ChangeCam(-1);
                if (ReverseCamDuration <= 0)
                    ChangeCam(1);
            }
            if (YeetBunny)
            {
                var direction = UnityEngine.Random.Range(-3f,3f);
                EventManager.Instance.mainCharacter.DoKnockToPlayer(null, EventManager.Instance.mainCharacter, 0.5f, 10*direction, 1, true);
                EventManager.Instance.mainCharacter.PlaySound(AllSound.SEList.PLAYERKNOCKOUT);
                YeetBunny = false;
            }
            if(TauntQueue.Count > 0 && EventManager.Instance.mainCharacter.logicStatus == PlayerLogicState.NORMAL)
            {
                TauntQueue.Dequeue();
                useTaunt();
            }

        }
        
    }
}
