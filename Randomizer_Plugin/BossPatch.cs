using EventMode;
using UnityEngine;
using Bullet;
using Character;
using HarmonyLib;
using Game;
namespace TeviRandomizer
{
    class BossPatch
    {
        static Vector2 bossSpawnLocation;
        static float extraDis;
        static float extraDisY;

        static void movEntityToLeft(CharacterBase cb,bool left)
        {
            if (left)
            {
                cb.SetPosition(CameraScript.Instance.GetTrueX() - 625, cb.t.position.y);
            }
            else
            {
                cb.SetPosition(CameraScript.Instance.GetTrueX() + 625, cb.t.position.y);
            }
        }

        public static void lockCameraToArena(EventManager em, byte nr = 0)
        {
            float center;
            float num;
            byte Area = WorldManager.Instance.Area;
            extraDis = 0;
            extraDisY = 0;
            ElementTile elementTile;
            switch (Area)
            {

                case 0:
                    //RIBAULD
                    em.mainCharacter.SetPositionX(WorldManager.Instance.CurrentRoomLeft + 1150f);
                    extraDis = 1150f;
                    SetCharacterPosition(CreateType.BASEONPLAYERD, CreateType.BASEONCAMERA, 18f, 8f);
                    CameraScript.Instance.SetLimitLR(WorldManager.Instance.CurrentRoomLeft + 1530f, WorldManager.Instance.CurrentRoomLeft + 1530f);
                    break;
                case 2:
                    //LILY
                    SetCharacterPosition(CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    break;
                case 3:
                    //VENA
                    //CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    break;
                case 4:
                    //ROLEO
                    num = 1f;
                    if (em.mainCharacter.direction == Direction.LEFT)
                    {
                        num = -1f;
                    }
                    CameraScript.Instance.NoLimitLR();
                    em.mainCharacter.transform.position += new Vector3(600f, 0f, 0f);
                    extraDis = 800;
                    SetCharacterPosition(CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 3.75f * num, 8f);
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX()+ 800f, CameraScript.Instance.GetTrueX()+ 800f);

                    break;
                case 5:
                    //BARADOS
                    extraDisY = 15*56;
                    extraDis = 7*56;
                    
                    SetCharacterPosition(CreateType.BASEONPLAYERD, CreateType.BASEONCAMERAO, 16.25f, -20.5f);
                    em.mainCharacter.transform.position = new Vector3(2771f, -12208f, 0f);
                    bossSpawnLocation.y = -12208f;
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());

                    break;
                case 8:
                    //LIBRARY
                    break;
                case 10:
                    //MALPHAGE
                    SetCharacterPosition(CreateType.FIXEDPOSITION, CreateType.BASEONCAMERA, WorldManager.Instance.CurrentRoomRight - MainVar.instance.TILESIZE * 29f, 8.5f);
                    num = bossSpawnLocation.x - MainVar.instance.TILESIZE * 6.5f - 11f;
                    CameraScript.Instance.SetLimitLR(num, num);
                    break;
                case 11:
                    //EIDOLON
                    SetCharacterPosition(CreateType.FIXEDPOSITION, CreateType.FIXEDPOSITION, EventManager.Instance.mainCharacter.t.position.x - MainVar.instance.TILESIZE * 13f, EventManager.Instance.mainCharacter.t.position.y + MainVar.instance.TILESIZE * 10.25f);
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.t.position.x - 125f, CameraScript.Instance.t.position.x + 125f);
                    CameraScript.Instance.SetNoZoomIn(999f);
                    break;
                case 12:
                    //JETHRO
                    float dist = 14f;
                    bossSpawnLocation.x = CameraScript.Instance.GetTrueX() + MainVar.instance.SCREEN_WIDTH * 0.61f;
                    bossSpawnLocation.y = CameraScript.Instance.GetTrueY();
                    CameraScript.Instance.SetLimitLR(em.LastHitTrigger.transform.position.x + MainVar.instance.TILESIZE * dist + MainVar.instance.TILESIZE / 2f - 25f, 25f + em.LastHitTrigger.transform.position.x + MainVar.instance.TILESIZE * dist + MainVar.instance.TILESIZE / 2f);
                    break;
                case 14:
                    //THETIS
                    num = (WorldManager.Instance.CurrentRoomLeft + WorldManager.Instance.CurrentRoomRight) / 2f;
                    SetCharacterPosition(CreateType.FIXEDPOSITION, CreateType.BASEONPLAYER, num - MainVar.instance.TILESIZE * 5f, -0.7f);
                    CameraScript.Instance.SetZoomCenter(CameraScript.Instance.GetTruePos());
                    CameraScript.Instance.ForceYPos(0f);
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX() - 375f, CameraScript.Instance.GetTrueX() + 375f);
                    break;
                case 16:
                    //JEZBELLE
                    bossSpawnLocation.x = CameraScript.Instance.GetTrueX() - MainVar.instance.TILESIZE * 3f;
                    bossSpawnLocation.y = CameraScript.Instance.GetTrueY() + MainVar.instance.TILESIZE;
                    break;
                case 18:
                    if (nr == 0)
                    {
                        //TEVIB
                        bossSpawnLocation.x = CameraScript.Instance.GetTrueX() + MainVar.instance.SCREEN_WIDTH * 0.185f;
                        bossSpawnLocation.y = em.mainCharacter.t.position.y;
                    }
                    else
                    {
                        //Charon Skip him pls
                        center = em.LastHitTrigger.transform.position.x - MainVar.instance.TILESIZE * 1;
                        bossSpawnLocation.x = center + MainVar.instance.TILESIZE * 6f;
                        bossSpawnLocation.y = em.mainCharacter.t.position.y;
                        CameraScript.Instance.SetLimitLR(center - 195f, center + 195f);
                    }
                    break;
                case 22:
                    //KATU
                    elementTile = WorldManager.Instance.FindElementTileNearCamera(EventMode.ElementType.MapPoint, MainVar.instance.SCREEN_WIDTH);
                    if ((bool)elementTile)
                    {
                        center = elementTile.transform.position.x - MainVar.instance.TILESIZE / 2f;
                    }
                    else
                    {
                        center = CameraScript.Instance.GetTrueX();
                    }
                    bossSpawnLocation.y = elementTile.transform.position.y;
                    bossSpawnLocation.x = elementTile.transform.position.x;
                    
                    CameraScript.Instance.SetLimitLR(center - MainVar.instance.TILESIZE * 5f, center + MainVar.instance.TILESIZE * 5f);
                    CameraScript.Instance.SetLimitY(CameraScript.Instance.t.position.y);
                    break;
                case 23:
                    // CYRIL
                    if (nr == 1)
                    {
                        bossSpawnLocation.x = CameraScript.Instance.GetTrueX() + MainVar.instance.SCREEN_WIDTH * 0.61f;
                        bossSpawnLocation.y = CameraScript.Instance.GetTrueY();
                        CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    }
                    //illusion palace?
                    //TAHLIA
                    // CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    break;
                case 24:
                    //CAPRICE
                    num = 1f;
                    if (em.mainCharacter.direction == Direction.LEFT)
                    {
                        num = -1f;
                    }
                    SetCharacterPosition(CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 5.75f * num, 8f);
                    em.mainCharacter.transform.position += new Vector3(100f, 0f, 0f);
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX()+625, CameraScript.Instance.GetTrueX()+625);
                    break;
                case 25:
                    if (nr == 0)
                    {
                        //VASSAGO
                        bossSpawnLocation.x = WorldManager.Instance.CurrentRoomRight - 315f;
                        bossSpawnLocation.y = em.mainCharacter.t.position.y + 1f;
                        CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    }
                    else
                    {
                        //DEMONFRAY
                        extraDis = 25 * 56;
                        num = EventManager.Instance.LastHitTrigger.transform.position.x + MainVar.instance.SCREEN_WIDTH * 0.8f;
                        SetCharacterPosition(CreateType.FIXEDPOSITION, CreateType.BASEONCAMERA, WorldManager.Instance.CurrentRoomLeft + MainVar.instance.TILESIZE * 20f, 9.725f);
                        CameraScript.Instance.SetLimitLR(num, num);
                    }
                    break;
                case 26:
                    //TYBRIOUS
                    elementTile = WorldManager.Instance.FindElementID(ElementType.EventPoint, 1);
                    extraDis = -25*56f;
                    SetCharacterPosition(CreateType.FIXEDPOSITION, CreateType.BASEONPLAYER, elementTile.transform.position.x - MainVar.instance.TILESIZE / 2f, -0.02f);
                    CameraScript.Instance.SetLimitLR(elementTile.transform.position.x, elementTile.transform.position.x);
                    break;
                case 27:
                    if (nr == 0)
                    {
                        //MEMLOCH
                        CameraScript.Instance.SetLimitLR(bossSpawnLocation.x, bossSpawnLocation.x);
                        CameraScript.Instance.SetLimitY(CameraScript.Instance.t.position.y);
                    }
                    else
                    {
                        //Frankie
                        center = CameraScript.Instance.GetTrueX() - MainVar.instance.TILESIZE;
                        SetCharacterPosition(CreateType.BASEONPLAYERD, CreateType.BASEONPLAYER, 2f, -0.25f);
                        CameraScript.Instance.SetLimitLR(center - 390f + MainVar.instance.TILESIZE, center + 390f + MainVar.instance.TILESIZE);
                    }
                    break;
                case 29:
                    //AMRYLLIS
                    bossSpawnLocation.x = CameraScript.Instance.GetTrueX() + 177f;
                    bossSpawnLocation.y = CameraScript.Instance.GetTrueY() + 100f;
                    CameraScript.Instance.SetNoZoomIn(99f);
                    EventManager.Instance.TurnOnFall(em.mainCharacter.t.position.x);
                    CameraScript.Instance.ForceYMode = false;
                    break;
                case 30:
                    break;
                default:
                    CameraScript.Instance.SetLimitLR(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueX());
                    break;
            }
        }

        static private void SetCharacterPosition(CreateType ctx, CreateType cty, float PlaceX, float PlaceY)
        {
            float TILESIZE = MainVar.instance.TILESIZE;
            Vector3 position = default(Vector3);
            Vector3 position2 = EventManager.Instance.mainCharacter.t.position;
            position.z = position2.z;
            Vector2 zero = Vector2.zero;
            zero = ((!EventManager.Instance.useCameraTargetPositionToCreate) ? new Vector2(CameraScript.Instance.GetScreenL(), CameraScript.Instance.GetScreenU()) : new Vector2(CameraScript.Instance.GetTargetLeftPos(), CameraScript.Instance.GetTargetTopPos()));
            if (ctx == CreateType.BASEONPLAYER)
            {
                position.x = position2.x + PlaceX * TILESIZE;
            }
            if (ctx == CreateType.BASEONPLAYERD)
            {
                float num = 1f;
                if (EventManager.Instance.mainCharacter.direction == Character.Direction.LEFT)
                {
                    num = -1f;
                }
                position.x = position2.x + PlaceX * TILESIZE * num;
            }
            if (ctx == CreateType.BASEONCAMERA)
            {
                position.x = zero.x + PlaceX * TILESIZE;
            }
            if (ctx == CreateType.BASEONCAMERAO)
            {
                position.x = zero.x + MainVar.instance.SCREEN_WIDTH + PlaceX * TILESIZE;
            }
            if (ctx == CreateType.BASEONCAMERAC)
            {
                position.x = CameraScript.Instance.GetTrueX() + PlaceX * TILESIZE;
            }
            if (ctx == CreateType.FIXEDPOSITION)
            {
                position.x = PlaceX;
            }
            if (cty == CreateType.BASEONPLAYER)
            {
                position.y = position2.y - PlaceY * TILESIZE;
            }
            if (cty == CreateType.BASEONCAMERA)
            {
                position.y = zero.y - PlaceY * TILESIZE;
            }
            if (cty == CreateType.BASEONCAMERAO)
            {
                position.y = zero.y - MainVar.instance.SCREEN_HEIGHT - PlaceY * TILESIZE;
            }
            if (cty == CreateType.BASEONCAMERAC)
            {
                position.y = CameraScript.Instance.GetTrueY() + PlaceY * TILESIZE;
            }
            if (cty == CreateType.FIXEDPOSITION)
            {
                position.y = PlaceY;
            }
            bossSpawnLocation = new Vector2(position.x, position.y);
        }


        [HarmonyPatch(typeof(EventManager), "CheckAfterMapChange")]
        [HarmonyPostfix]
        static void reEnableBookMark()
        {
            if (EventManager.Instance.isBossMode())
            {
                if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 || SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                {
                    SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 2);
                }
                else
                {
                    SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 0);
                }
            }
        }

        //Rework Boss Spawning

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_AMARYLLIS), "EVENT")]
        static void fixAmaryllis(ref CharacterBase ___b)
        {
            EventManager em = EventManager.Instance;
            if (EventManager.Instance.EventStage == 0 && WorldManager.Instance.Area != 29)
            {
                (em.mainCharacter.phy_perfer as CharacterPhy).orbUsing = OrbType.BLACK;
                EventManager.Instance.mainCharacter.SetPosition(CameraScript.Instance.GetTrueX(), CameraScript.Instance.GetTrueY());
                UnityEngine.Object.Instantiate(TeviRandomizer.ResourcePatch.resources[29].GetComponent<AreaResource>().GetMapObject(10));
                UnityEngine.Object.Instantiate(TeviRandomizer.ResourcePatch.resources[29].GetComponent<AreaResource>().GetMapObject(10));
                lockCameraToArena(em);
                em.SetStage(101);
            }
            switch(em.EventStage)
            {
                case 101:
                    em.mainCharacter.playerc_perfer.RegenHealth(75f, fullyHeal: true, addrec: false);
                    if (em.EventTime >= 1.75f)
                    {
                        MusicManager.Instance.SetTargetVolume(0f, 3.3f);
                        em.SetStage(102);
                    }
                    break;
                case 102:
                    if (em.EventTime >= 1.7f)
                    {
                        FadeManager.Instance.SetAll(1f, 1f, 1f, 1f, 0f, 15f);
                        ___b = em.CreateEnemy(Type.Amaryllis, BossType.BOSS);
                        ___b.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        ___b.ChangeDirection(Direction.LEFT);
                        ___b.enemy_perfer.AIFloatMode(7.5f);
                        ___b.spranim_prefer.ToggleFallStand(1);
                        ___b.spranim_prefer.ForceAnimation("teleport_over");
                        ___b.PlaySound("amary_teleport");
                        em.AddActor(___b);
                        ___b.SetDefeatEvent(Mode.END_AMARYLLIS);
                        em.SetBossBarOwner(___b);
                        em.SetStage(103);
                    }
                    break;
                case 103:
                    if (em.EventTime >= 0.1f)
                    {
                        ___b.spranim_prefer.LoadSpriteForced("amaryllis_awakened");
                        ___b.spranim_prefer.NoForceAnimation();
                        em.SetStage(104);
                    }
                    break;
                case 104:
                    if (em.EventTime >= 1.2f || em.GetCounter(4) <= 101f)
                    {
                        SaveManager.Instance.SetBuffInfo(BuffType.StrongRangedOnly_TOP, 2);
                        SaveManager.Instance.SetBuffInfo(BuffType.ASuperArmor_TOP, 2);
                        SaveManager.Instance.SetBuffInfo(BuffType.OrbWeakC_TOP, 2);
                        MusicManager.Instance.PlayMusic(Music.BOSS6);
                        em.StopEvent();
                        em.StartBoss();
                        ___b.enemy_perfer.SetAttackVoiceCoolDownTime(27.5f);
                        EventManager.Instance.TurnOnFall(em.mainCharacter.t.position.x);
                        if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 || SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 2);
                        }
                        else
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 0);
                        }
                    }
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_BARADOS), "EVENT")]
        static void fixBarados(ref float ___leftx, ref enemyController ___barados)
        {
            EventManager em = EventManager.Instance;
            if (EventManager.Instance.EventStage == 0 && WorldManager.Instance.Area != 5)
            {
                ___leftx = WorldManager.Instance.CurrentRoomLeft;
                CharacterBase characterBase = em.CreateEnemy(Character.Type.Barados, BossType.BOSS);
                em.AddActor(characterBase);
                characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                characterBase.direction = Direction.NOTTOPLAYER;
                ___barados = characterBase as enemyController;
                em.SetBossBarOwner(characterBase);
                characterBase.SetDefeatEvent(Mode.END_BARADOS);
                MusicManager.Instance.SetTargetVolume(0f, 0.6f);
                SaveManager.Instance.CheckIsFirstTimePlaying(em.getMode());
                em.SetStage(60);
            }
        }

        [HarmonyPatch(typeof(BOSS_CAPRICE), "EVENT")]
        [HarmonyPrefix]
        static void fixCAPRICE(ref CharacterBase ___cb)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 24) {
                if (em.EventStage == 0)
                {
                    ___cb = em.CreateEnemy(Type.Caprice, BossType.BOSS);
                    em.AddActor(___cb);
                    ___cb.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                    ___cb.direction = Direction.TOPLAYER;
                    em.SetBossBarOwner(___cb);
                    ___cb.SetDefeatEvent(Mode.END_CAPRICE);
                    MusicManager.Instance.SetTargetVolume(0.25f);
                    SaveManager.Instance.CheckIsFirstTimePlaying(em.getMode());
                    em.SetStage(51);
                }
                if(em.EventStage == 51) {
                    if (em.EventTime >= 0.8f)
                    {
                        MusicManager.Instance.PlayMusic(Music.BOSS1);
                        em.StopEvent();
                        em.StartBoss();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Caprice),"AI")]
        [HarmonyPrefix]
        static void fixCapriceAI(ref enemyController ___en)
        {
            float scrennWidth = MainVar.instance.SCREEN_WIDTH;
            float cameraPosy = CameraScript.Instance.GetTrueX();
            if(___en.phase == AIPhase.A_PHASE_2)
            {
                if (___en.direction == Direction.LEFT && ___en.transform.position.x < cameraPosy - scrennWidth / 2 + 50)
                    ___en.AI_FlipDir();
                if (___en.direction == Direction.RIGHT && ___en.transform.position.x > cameraPosy + scrennWidth / 2 - 50)
                    ___en.AI_FlipDir();

            }

        }

        [HarmonyPatch(typeof(BOSS_CYRIL), "EVENT")]
        [HarmonyPrefix]
        static void fixCYRIL(ref enemyController ___b)
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 0 && WorldManager.Instance.Area != 23)
            {
                CharacterBase characterBase = em.CreateEnemy(Type.Cyril, BossType.BOSS);
                em.AddActor(characterBase);
                characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                characterBase.direction = Direction.TOPLAYER;
                characterBase.SetDefeatEvent(Mode.END_CYRIL);
                ___b = characterBase as enemyController;
                em.SetBossBarOwner(characterBase);
                em.SetStage(71);
            }
            if(em.EventStage == 71)
            {
                MusicManager.Instance.PlayMusic(Music.BOSS8);
                ___b.enemy_perfer.SetNoCameraLimit(toggle: false);
                CameraScript.Instance.ResetCameraSpeed();
                em.StopEvent();
                em.StartBoss();
                ___b.SetAttackVoiceCoolDownTime(25f);
                SaveManager.Instance.SetBuffInfo(BuffType.EPReduce, 2);
                SaveManager.Instance.SetBuffInfo(BuffType.Combo20Reduce_TOP, 2);
                SaveManager.Instance.SetBuffInfo(BuffType.OrbWeakS_TOP, 2);
                SaveManager.Instance.SetBuffInfo(BuffType.OverEquip_TOP, 2);
                SaveManager.Instance.RemoveTodo(Todo.Chap8ToLavaCave, addFinished: false);
                SaveManager.Instance.RemoveTodo(Todo.Chap8ToLavaCave2);
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_DEMONFRAY), "EVENT")]
        static void fixFray(ref CharacterBase ___f)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 25)
            {

                switch(em.EventStage)
                {
                    case 0:
                        _ = em.mainCharacter.direction;
                        ___f = em.CreateEnemy(Type.Fray_Demon, BossType.BOSS);
                        em.AddActor(___f);
                        ___f.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        ___f.direction = Direction.NOTTOPLAYER;
                        em.SetBossBarOwner(___f);
                        ___f.SetDefeatEvent(Mode.END_DEMONFRAY);
                        MusicManager.Instance.SetTargetVolume(0.25f);
                        SaveManager.Instance.RemoveTodo(Todo.Chap5ToLaboratory);
                        em.SetStage(1);
                        break;
                    case 1:
                        if (em.EventTime >= 0.75f)
                        {
                            ___f.isBoss = BossType.BOSS;
                            MusicManager.Instance.PlayMusic(Music.BOSS3);
                            SaveManager.Instance.SetBuffInfo(BuffType.FrayBulletUp_TOP, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.DodgeTime_TOP, 2);
                            ___f.enemy_perfer.InitAI();
                            em.StopEvent();
                            em.StartBoss();
                            ___f.enemy_perfer.SetAttackVoiceCoolDownTime(28f);
                            WorldManager.Instance.RespawnTiles();
                            ___f.spranim_prefer.SetColorLerp(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 150f);
                            CameraScript.Instance.ResetCameraSpeed();
                            em.ForceAIPlayInEvent = false;
                            WorldManager.Instance.StartFadeFrontLayer(1f);
                            if (___f.enemy_perfer.phase != AIPhase.T_PHASE_1 || ___f.enemy_perfer.phase != AIPhase.T_PHASE_2 || ___f.enemy_perfer.phase != AIPhase.T_PHASE_3)
                            {
                                ___f.enemy_perfer.SwitchPhase(AIPhase.T_PHASE_1);
                            }
                            BackgroundManager.Instance.StopBackgroundUpdateBoss(t: false);
                            MusicManager.Instance.LimitVoicePlayback = 1;
                        }
                        break;

                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_EIDOLON), "EVENT")]
        static void fixEIDOLON(ref enemyController ___m)
        {
            EventManager em = EventManager.Instance;
            if (11 != WorldManager.Instance.Area)
            {
                switch (em.EventStage)
                {
                    case 0:
                        CharacterBase characterBase = em.CreateEnemy(Type.Eidolon, BossType.BOSS);
                        em.AddActor(characterBase);
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                        characterBase.SetDefeatEvent(Mode.END_EIDOLON);
                        characterBase.direction = Direction.TOPLAYER;
                        ___m = characterBase as enemyController;
                        ___m.AIGravity(0f);
                        ___m.phy_perfer._velocity.y = 0f;
                        ___m.spine.SetOrder(44);
                        em.SetBossBarOwner(characterBase);
                        em.SetStage(1);
                        break;
                    case 1:
                        if (em.EventTime > 0.9f)
                        {
                            ___m.slowAfterDefeat = false;
                            ___m.floatAfterDefeat = true;
                            ___m.effectAfterDefeat = false;
                            SaveManager.Instance.SetBuffInfo(BuffType.EidoShield_TOP, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.EnemyDefeatedRage_TOP, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.OverAbsorb, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.EidoSystemDisorder, 2);
                            MusicManager.Instance.PlayMusic(Music.BOSS5);
                            CameraScript.Instance.ResetCameraSpeed();
                            em.StopEvent();
                            em.StartBoss();
                            em.mainCharacter.SetBuffTime(BuffType.DefenceDown, 0.03f, setMaxTime: false);
                            BackgroundManager.Instance.StopBackgroundUpdateBoss(t: false);
                            ___m.SetAttackVoiceCoolDownTime(30f);
                        }
                        break;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Eidolon),"ALWAYS")]
        static void fixEidolonAI()
        {
            if (WorldManager.Instance.Area != 25)
            {
                CharacterBase[] chars = EnemyPatch.getCharacters(Type.EnergyBall_Pollution);
                float camY = CameraScript.Instance.GetTrueY();
                foreach (CharacterBase character in chars)
                {
                    if (character.transform.position.y > camY + 380)
                    {
                        character.SetPositionY(camY + 300);
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_FRANKIE), "EVENT")]
        static void fixFrankie(ref enemyController ___pkoa, ref enemyController ___g1, ref enemyController ___g2)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 27) {
                switch(em.EventStage)
                {
                    case 0:
                        ___pkoa = em.CreateEnemy(Type.PKOA, BossType.BOSS) as enemyController;
                        ___pkoa.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                        ___g1 = em.CreateEnemy(Type.PK_Guard_G_Pollution, BossType.NONE) as enemyController;
                        ___g1.SetPosition(bossSpawnLocation.x - MainVar.instance.TILESIZE * 5f, bossSpawnLocation.y);
                        ___g1.DoNotDelete = true;

                        ___g2 = em.CreateEnemy(Type.PK_Guard_B_Pollution, BossType.NONE) as enemyController;
                        ___g2.SetPosition(bossSpawnLocation.x - MainVar.instance.TILESIZE * 3.2f, bossSpawnLocation.y);
                        ___g2.DoNotDelete = true;

                        ___pkoa.spine.ChangeSkin("skin3");
                        ___pkoa.spine.SetAnimation("stand", loop: true);


                        ___pkoa.spine.SetAnimationSpeed(0f);
                        ___pkoa.spine.SetOrder(120);
                        ___pkoa.spine.ChangeSkin("skin1");

                        em.SetStage(171);
                        break;
                    case 171:
                        if (em.EventTime >= 1f)
                        {
                            em.SetBossBarOwner(___pkoa);

                            MusicManager.Instance.PlayMusic(Music.BOSS3);
                            CameraScript.Instance.ResetCameraSpeed();
                            ___pkoa.InitAI();
                            em.StopEvent();
                            em.StartBoss();
                            BackgroundManager.Instance.StopBackgroundUpdateBoss(t: false);
                        }
                        break;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_JETHRO), "EVENT")]
        static void fixJETHRO(ref enemyController ___b)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 12) {
                switch(em.EventStage)
                {
                    case 0:
                        CharacterBase characterBase = em.CreateEnemy(Type.Jethro, BossType.BOSS);
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        em.AddActor(characterBase);
                        characterBase.direction = Direction.TOPLAYER;
                        characterBase.SetDefeatEvent(Mode.END_JETHRO);
                        ___b = characterBase as enemyController;
                        em.SetBossBarOwner(characterBase);
                        em.SetStage(61);
                        break;
                    case 61:
                        if (em.EventTime >= 0.7f)
                        {
                            MusicManager.Instance.PlayMusic(Music.BOSS10);
                            CameraScript.Instance.ResetCameraSpeed();
                            em.StopEvent();
                            em.StartBoss();
                            ___b.SetAttackVoiceCoolDownTime(20f);
                            SaveManager.Instance.SetMiniFlag(Mini.CanChangeMusic, 1);
                            SaveManager.Instance.SetBuffInfo(BuffType.ArmorDown, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.JeAttackMode_TOP, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.JeAutoShield_TOP, 2);
                            SaveManager.Instance.SetBuffInfo(BuffType.BuffProtect, 2);
                        }
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(BOSS_JEZBELLE), "EVENT")]
        [HarmonyPrefix]
        static void fixJEZBELLE(ref enemyController ___b)
        {
            EventManager em = EventManager.Instance;
            if (em.EventStage == 0 && WorldManager.Instance.Area != 16)
            {

                short rX = WorldManager.Instance.CurrentRoomX;
                short rY = WorldManager.Instance.CurrentRoomY;
                // x:-5 y: -3
                int originX = (int)(rX * CustomMap.firstBlockMultiplierX + extraDis / MainVar.instance.TILESIZE);
                int originY = (int)(rY * CustomMap.firstBlockMultiplierY + extraDisY / MainVar.instance.TILESIZE);

                CustomMap.createElementTile(originX + 12, originY + 4, 63, 16);
                CustomMap.createNormalTile(originX + 4, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 5, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 6, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 7, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 8, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 9, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 10, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 11, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 12, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 13, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 14, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 15, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 16, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 17, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 18, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 19, originY + 9, 21, false, false, 16);
                CustomMap.createNormalTile(originX + 20, originY + 9, 21, false, false, 16);

                em.SetStage(1);
            }
            switch(em.EventStage)
            {
                case 1:
                    CharacterBase characterBase2 = em.CreateEnemy(Type.Jezbelle, BossType.BOSS);
                    em.AddActor(characterBase2);
                    characterBase2.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                    characterBase2.direction = Direction.TOPLAYER;
                    characterBase2.SetDefeatEvent(Mode.END_JEZBELLE);
                    ___b = characterBase2 as enemyController;
                    em.SetBossBarOwner(characterBase2);
                    MusicManager.Instance.SetTargetVolume(0.25f);
                    em.SetStage(101);
                    break;
                case 101:
                    if (em.EventTime >= 0.7f)
                    {
                        SaveManager.Instance.SetBuffInfo(BuffType.ArenaDuel_TOP, 2);
                        SaveManager.Instance.SetBuffInfo(BuffType.RingOut_TOP, 2);
                        MusicManager.Instance.PlayMusic(Music.BOSS1);
                        SaveManager.Instance.RemoveTodo(Todo.Chap7ToArena);
                        SaveManager.Instance.SetMiniFlag(Mini.CanChangeMusic, 1);
                        ___b.SetAttackVoiceCoolDownTime(20f);
                        em.StopEvent();
                        em.StartBoss();
                    }
                    break;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_KATU), "EVENT")]
        static void fixKATU(ref CharacterBase ___boss,ref CharacterBase ___sboss)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 22)
            {
                float size = MainVar.instance.TILESIZE;
                if (em.EventStage == 0)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX + extraDis / MainVar.instance.TILESIZE);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY + extraDisY / MainVar.instance.TILESIZE);


                    CustomMap.createNormalTile(originX + 2, originY + 7, 92, false, false, 22);
                    CharacterBase characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    //characterBase.SetPosition((originX + 2)*size, (originY + 7)*size);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);

                    characterBase.direction = Direction.RIGHT;
                    em.AddActor(characterBase);



                    CustomMap.createNormalTile(originX + 5, originY + 7, 92, false, false, 22);
                    //characterBase.SetPosition((originX + 5) * size, (originY + 7) * size);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);

                    characterBase.direction = Direction.RIGHT;
                    em.AddActor(characterBase);


                    CustomMap.createNormalTile(originX + 16, originY + 7, 92, false, false, 22);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);
                    //characterBase.SetPosition((originX + 16) * size, (originY + 7) * size);
                    characterBase.direction = Direction.LEFT;
                    em.AddActor(characterBase);

                    CustomMap.createNormalTile(originX + 19, originY + 7, 92, false, false, 22);
                    characterBase = em.CreateEnemy(Character.Type.Katu, BossType.BOSS);
                    em.SetCharacterPosition(characterBase, CreateType.BASEONCAMERAO, CreateType.BASEONCAMERA, 1f, 6.5f);
                    //characterBase.SetPosition((originX + 19) * size, (originY + 7) * size);
                    characterBase.direction = Direction.LEFT;
                    em.AddActor(characterBase);

                    CustomMap.createElementTile(originX + 13, originY + 7, 63, 22);

                    em.SetStage(1);

                }
                if (em.EventStage == 1)
                {
                    if (em.EventTime >= 1.35f)
                    {
                        lockCameraToArena(em);
                        ElementTile elementTile = WorldManager.Instance.FindElementTileNearCamera(EventMode.ElementType.MapPoint, MainVar.instance.SCREEN_WIDTH);
                        float center = 0;
                        if ((bool)elementTile)
                        {
                            center = elementTile.transform.position.x - MainVar.instance.TILESIZE / 2f;
                        }
                        foreach (CharacterBase character in CharacterManager.Instance.characters)
                        {
                            if (!character)
                            {
                                continue;
                            }
                            character.DoNotDelete = true;
                            if (character.type == Type.Katu)
                            {
                                if (character.direction != em.mainCharacter.direction && Utility.CheckDistX(center, character.t.position.x) > MainVar.instance.TILESIZE * 6.5f)
                                {
                                    ___boss = character;
                                }
                                if (character.direction == em.mainCharacter.direction && Utility.CheckDistX(center, character.t.position.x) > MainVar.instance.TILESIZE * 6.5f)
                                {
                                    ___sboss = character;
                                }
                            }
                        }
                        ___boss.isBoss = BossType.BOSS;
                        ___boss.InitEnemy();
                        em.SetBossBarOwner(___boss);
                        ___boss.SetDefeatEvent(Mode.END_KATU);
                        ___boss.ID = 99;
                        ___sboss.ID = 1;
                        em.SetStage(101);
                    }
                }
                if(em.EventStage == 101)
                {
                    EventManager.Instance.SetCounter(1, 0f);
                    MusicManager.Instance.PlayMusic(Music.BOSS2);
                    SaveManager.Instance.RemoveTodo(Todo.Chap1SToRuin, addFinished: false);
                    SaveManager.Instance.RemoveTodo(Todo.Chap1SToRuin2, addFinished: false);
                    SaveManager.Instance.RemoveTodo(Todo.Chap1SToRuin3);
                    em.StopEvent();
                    em.StartBoss();
                    ___boss.enemy_perfer.SwitchPhase(AIPhase.O_PHASE_1);
                    ___sboss.enemy_perfer.SwitchPhase(AIPhase.O_PHASE_1);
                    BackgroundManager.Instance.StopBackgroundUpdateBoss(t: false);
                }
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_LILY), "EVENT")]
        static void fixLILY(ref enemyController ___lily)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 2)
            {
                switch (em.EventStage)
                {
                    case 0:
                        CharacterBase characterBase = em.CreateEnemy(Type.Lily, BossType.BOSS);
                        em.AddActor(characterBase);
                        characterBase.direction = Direction.TOPLAYER;
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        characterBase.SetDefeatEvent(Mode.END_LILY);
                        ___lily = characterBase as enemyController;
                        em.SetBossBarOwner(characterBase);
                        MusicManager.Instance.SetTargetVolume(0.25f);
                        SaveManager.Instance.CheckIsFirstTimePlaying(em.getMode());
                        em.SetStage(21);
                        break;
                    case 21:
                        if (em.EventTime >= 1.05f)
                        {
                            MusicManager.Instance.PlayMusic(Music.BOSS2);
                            em.StopEvent();
                            em.StartBoss();
                            ___lily.SetAttackVoiceCoolDownTime(27.5f);
                        }
                        break;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_MALPHAGE), "EVENT")]
        static void fixMALPHAGE(ref enemyController ___m)
        {
            EventManager em = EventManager.Instance;

            if (em.EventStage == 0 && WorldManager.Instance.Area != 10)
            {

                //spawn every object for MALPHAGE
                if (WorldManager.Instance.Area != 10)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX + extraDis / MainVar.instance.TILESIZE);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY + extraDisY / MainVar.instance.TILESIZE);
                    CustomMap.createElementTile(originX - 5, originY - 3, 158, 10);
                    CustomMap.createElementTile(originX - 5, originY - 3, 158, 10);
                    //ID0
                    CustomMap.createElementTile(originX - 5, originY + 16, 72, 10);
                    CustomMap.createElementTile(originX - 5, originY + 15, 162, 10);

                    CustomMap.createElementTile(originX - 4, originY + 18, 73, 10);
                    CustomMap.createElementTile(originX - 4, originY + 17, 161, 10);

                    CustomMap.createElementTile(originX - 2, originY + 15, 73, 10);
                    CustomMap.createElementTile(originX - 2, originY + 17, 74, 10);
                    CustomMap.createElementTile(originX - 2, originY + 14, 163, 10);
                    CustomMap.createElementTile(originX - 2, originY + 16, 162, 10);

                    CustomMap.createElementTile(originX - 1, originY + 13, 74, 10);
                    CustomMap.createElementTile(originX - 1, originY + 17, 72, 10);
                    CustomMap.createElementTile(originX - 1, originY + 12, 163, 10);
                    CustomMap.createElementTile(originX - 1, originY + 16, 162, 10);

                    CustomMap.createElementTile(originX + 1, originY + 13, 73, 10);
                    CustomMap.createElementTile(originX + 1, originY + 15, 72, 10);
                    CustomMap.createElementTile(originX + 1, originY + 12, 162, 10);
                    CustomMap.createElementTile(originX + 1, originY + 14, 163, 10);

                    CustomMap.createElementTile(originX + 2, originY + 17, 74, 10);
                    CustomMap.createElementTile(originX + 2, originY + 18, 82, 10);
                    CustomMap.createElementTile(originX + 2, originY + 16, 161, 10);

                    CustomMap.createElementTile(originX + 3, originY + 16, 73, 10);
                    CustomMap.createElementTile(originX + 3, originY + 18, 72, 10);
                    CustomMap.createElementTile(originX + 3, originY + 15, 162, 10);
                    CustomMap.createElementTile(originX + 3, originY + 17, 162, 10);

                    CustomMap.createElementTile(originX + 4, originY + 14, 74, 10);
                    CustomMap.createElementTile(originX + 4, originY + 13, 161, 10);

                    CustomMap.createElementTile(originX + 6, originY + 18, 73, 10);
                    CustomMap.createElementTile(originX + 6, originY + 17, 161, 10);

                    CustomMap.createElementTile(originX + 7, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 8, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 8, originY + 12, 73, 10);
                    CustomMap.createEnemyTile(originX + 8, originY + 13, 164, 10); //MalphageCore
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 8, originY + 14, 72, 10);
                    CustomMap.createElementTile(originX + 8, originY + 12, 73, 10);
                    CustomMap.createElementTile(originX + 7, originY + 13, 163, 10);

                    CustomMap.createElementTile(originX + 6, originY + 2, 75, 10);
                    CustomMap.createEnemyTile(originX + 6, originY + 3, 164, 10); //MalphageCore

                    CustomMap.createElementTile(originX - 2, originY + 4, 74, 10);
                    CustomMap.createEnemyTile(originX - 2, originY + 5, 164, 10); //MalphageCore

                    CustomMap.createEnemyTile(originX + 17, originY + 4, 164, 10); //MalphageCore


                    CustomMap.createElementTile(originX + 11, originY + 11, 164, 10);

                    CustomMap.createElementTile(originX + 11, originY + 8, 165, 10);


                    CustomMap.createElementTile(originX + 16, originY - 2, 159, 10);
                    CustomMap.createElementTile(originX + 6, originY - 2, 160, 10);
                    CustomMap.createElementTile(originX - 5, originY - 2, 158, 10);
                    CustomMap.createElementTile(originX + 6, originY + 11, 157, 10);



                }


                MusicManager.Instance.SetTargetVolume(0.25f, 0.5f);

                em.SetStage(4);
            }

            switch(em.EventStage)
            {
                case 4:
                    if (em.EventTime >= 0.1333f)
                    {
                        CharacterBase characterBase = em.CreateEnemy(Type.Malphage, BossType.BOSS);
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                        em.AddActor(characterBase);
                        movEntityToLeft(characterBase, false);
                        characterBase.SetDefeatEvent(Mode.END_MALPHAGE);
                        characterBase.direction = Direction.TOPLAYER;
                        ___m = characterBase as enemyController;
                        em.SetBossBarOwner(characterBase);
                        em.SetStage(30);
                    }
                    break;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_RIBAULD), "EVENT")]
        static void fixRIBAULD()
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 0)
            {
                switch (em.EventStage)
                {
                    case 0:
                        em.mainCharacter.ChangeDirection(Direction.RIGHT);
                        em.mainCharacter.AIMove(400f);
                        CharacterBase characterBase = em.CreateEnemy(Type.Ribauld, BossType.BOSS);
                        em.AddActor(characterBase);
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        characterBase.direction = Direction.TOPLAYER;
                        em.SetBossBarOwner(characterBase);
                        characterBase.SetDefeatEvent(Mode.END_RIBAULD);
                        MusicManager.Instance.SetTargetVolume(0.25f);
                        em.SetStage(21);
                        break;
                    case 21:
                        if (em.EventTime >= 0.9f)
                        {
                            MusicManager.Instance.PlayMusic(Music.BOSS1);
                            em.StopEvent();
                            em.StartBoss();
                            SaveManager.Instance.SetMiniFlag(Mini.CanComboStyle, 1);
                        }
                        break;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_ROLEO), "EVENT")]
        static void fixROLEO(ref CharacterBase ___r)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 4)
            {
                switch (em.EventStage)
                {
                    case 0:
                        ___r = em.CreateEnemy(Type.Roleo, BossType.BOSS);
                        em.AddActor(___r);
                        ___r.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        ___r.direction = Direction.NOTTOPLAYER;
                        em.SetBossBarOwner(___r);
                        ___r.SetDefeatEvent(Mode.END_ROLEO);
                        MusicManager.Instance.SetTargetVolume(0.25f);
                        SaveManager.Instance.RemoveTodo(Todo.Chap2SToMazeBoss);
                        em.SetStage(101);
                        break;
                    case 101:
                        if (em.EventTime >= 0.75f)
                        {
                            MusicManager.Instance.PlayMusic(Music.BOSS1);
                            ___r.enemy_perfer.InitAI();
                            em.StopEvent();
                            em.StartBoss();
                        }
                        break;
                }
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_TEVIB11x11), "EVENT")]
        static void fixTeviB(ref enemyController ___b)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 18)
            {
                if (em.EventStage == 0)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);
                    CustomMap.createElementTile(originX + 10, originY + 4, 155, 18);
                    CustomMap.createElementTile(originX + 10, originY + 5, 43, 18);
                    CustomMap.createElementTile(originX + 11, originY + 7, 73, 18);
                    CustomMap.createElementTile(originX + 12, originY + 7, 93, 18);
                    em.SetStage(11);
                }
                switch (em.EventStage)
                {
                    case 11:
                        if (!(em.EventTime >= 1f) || !em.mainCharacter.onGround())
                        {
                            break;
                        }
                        CharacterBase characterBase3 = em.CreateEnemy(Type.Illusion_Alius, BossType.BOSS);
                        em.AddActor(characterBase3);
                        characterBase3.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        characterBase3.direction = Direction.TOPLAYER;
                        characterBase3.SetDefeatEvent(Mode.END_TEVIB);
                        characterBase3.enemy_perfer.SetNoCameraLimit(toggle: true);
                        characterBase3.spranim_prefer.LoadSpriteForced("Tevi");
                        ___b = characterBase3 as enemyController;
                        em.SetBossBarOwner(characterBase3);
                        ___b.spranim_prefer.ToggleRunWalk(1);
                        ___b.spranim_prefer.SetMaterial(AreaResource.Instance.GetMaterialByName("TeviB Material"));
                        em.NextStage();
                        MusicManager.Instance.SetTargetVolume(0f, 15f);
                        ___b.PlaySound("boss_appear");
                        GameObject pooledObject = AreaResource.Instance.AreaPooler.GetPooledObject("TeviB Aura");
                        pooledObject.transform.position = characterBase3.t.position;
                        pooledObject.SetActive(value: true);
                        DisableWhenNoCharacter component = pooledObject.GetComponent<DisableWhenNoCharacter>();
                        component.Setup(characterBase3);
                        component.transform.localScale = new Vector3(22f, 22f, 22f);
                        em.SetStage(90);
                        break;
                }
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_THETIS), "EVENT")]
        static void fixTHETIS(ref enemyController ___t,ref enemyController ___s)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 14)
            {
                switch (em.EventStage)
                {
                    case 0:
                        float num = (WorldManager.Instance.CurrentRoomLeft + WorldManager.Instance.CurrentRoomRight) / 2f;
                        CharacterBase characterBase = em.CreateEnemy(Type.Thetis, BossType.BOSS);
                        em.AddActor(characterBase);
                        characterBase.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                        characterBase.direction = Direction.RIGHT;
                        characterBase.SetDefeatEvent(Mode.END_THETIS);
                        characterBase.enemy_perfer.InitAI();
                        characterBase.enemy_perfer.floatAfterDefeat = true;
                        ___t = characterBase as enemyController;
                        em.SetBossBarOwner(characterBase);
                        CharacterBase characterBase2 = em.CreateEnemy(Type.Serulea, BossType.BOSS);
                        em.AddActor(characterBase2);
                        characterBase2.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                        characterBase2.direction = Direction.RIGHT;
                        characterBase2.enemy_perfer.InitAI();
                        ___s = characterBase2 as enemyController;
                        ___s.t.position -= new Vector3(100f, -200f);
                        ___t.t.position -= new Vector3(100f, -200f);
                        ___s.spranim_prefer.SetAlpha(0f);
                        ___t.spranim_prefer.SetAlpha(0f);
                        ___s.spranim_prefer.Invisible(t: true);
                        ___t.spranim_prefer.Invisible(t: true);
                        ___s.spranim_prefer.NoFlash();
                        ___t.spranim_prefer.NoFlash();
                        ___s.phy_perfer.AIFloatMode(0.002f);
                        ___t.phy_perfer.AIFloatMode(0.002f);
                        em.SetStage(61);
                        break;
                    case 61:
                        if (em.EventTime >= 1.4f)
                        {
                            MusicManager.Instance.PlayMusic(Music.BOSS9);
                            ___t.enemy_perfer.InitAI();
                            em.StopEvent();
                            em.StartBoss();
                            ___s.phy_perfer.AIFloatMode(2.125f);
                            ___t.phy_perfer.AIFloatMode(2.125f);
                            ___s.spranim_prefer.SetColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                            ___t.spranim_prefer.SetColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                            ___s.spranim_prefer.SyncOutlineAlpha();
                            ___t.spranim_prefer.SyncOutlineAlpha();
                            BackgroundManager.Instance.StopBackgroundUpdateBoss(t: false);
                            ___t.SetAttackVoiceCoolDownTime(30f);
                        }
                        break;
                }
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BOSS_TYBRIOUS), "EVENT")]
        static void fixTybrious(ref CharacterBase ___cb)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 26)
            {
                if (EventManager.Instance.EventStage == 0)
                {
                    ___cb = em.CreateEnemy(Character.Type.Tybrious, BossType.BOSS);
                    em.AddActor(___cb);
                    ___cb.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);

                    ___cb.t.position += new Vector3(-3f, 0f, 0f);
                    ___cb.direction = Direction.NOTTOPLAYER;
                    em.SetBossBarOwner(___cb);
                    ___cb.SetDefeatEvent(Mode.END_TYBRIOUS);
                    CameraScript.Instance.SetTargetXY(___cb.t, MainVar.instance.TILESIZE * 4f, 0f, forcePositionIgnoreRoom: false);
                    MusicManager.Instance.SetTargetVolume(0.25f);
                    em.SetStage(151);


                }
                if (em.EventStage == 151)
                {
                    if (em.EventTime >= 0.75f)
                    {
                        CameraScript.Instance.ResetTarget();
                        MusicManager.Instance.PlayMusic(Music.BOSS4);
                        BackgroundManager.Instance.StopBackgroundChange(t: true);
                        em.StopEvent();
                        em.StartBoss();
                        em.ForceBulletPlayInEvent = false;
                        ___cb.enemy_perfer.SetAttackVoiceCoolDownTime(35f);
                    }
                }
            }
        }


        [HarmonyPatch(typeof(BOSS_VASSAGO), "EVENT")]
        [HarmonyPrefix]
        static void fixVASSAGO(ref CharacterBase ___v)
        {
            EventManager em = EventManager.Instance;
            if (WorldManager.Instance.Area != 25)
            {
                if (em.EventStage == 0)
                {
                    short rX = WorldManager.Instance.CurrentRoomX;
                    short rY = WorldManager.Instance.CurrentRoomY;
                    // x:-5 y: -3
                    int originX = (int)(rX * CustomMap.firstBlockMultiplierX);
                    int originY = (int)(rY * CustomMap.firstBlockMultiplierY);
                    CustomMap.createElementTile(originX + 10, originY + 13, 158, 25);
                    CustomMap.createElementTile(originX + 10, originY + 14, 159, 25);
                    em.SetStage(111);
                }
                if (em.EventStage == 111)
                {
                    if (em.EventTime >= 0.25f)
                    {
                        SaveManager.Instance.SetEventFlag(Mode.BOSS_VASSAGO, 1, force: true);

                        ___v = em.CreateEnemy(Type.Vassago, BossType.BOSS);
                        em.AddActor(___v);
                        ___v.SetDefeatEvent(Mode.END_VASSAGO);
                        em.SetBossBarOwner(___v);
                        ___v.SetPosition(bossSpawnLocation.x, bossSpawnLocation.y);
                        ___v.enemy_perfer.ChangeDirByPlayer();
                        CameraScript.Instance.ResetTarget();
                        em.SetStage(171);
                    }
                }
                if(em.EventStage == 171)
                {
                    if (em.EventTime >= 2.3f)
                    {
                        em.mainCharacter.SetBuffTime(BuffType.DoomPassive_TOP, 0.03f, setMaxTime: false);
                        em.StopEvent();
                        em.StartBoss();
                        ___v.enemy_perfer.SetAttackVoiceCoolDownTime(27.5f);
                        if (SaveManager.Instance.GetMiniFlag(Mini.GameCleared) > 0 || SaveManager.Instance.GetCustomGame(CustomGame.FreeRoam))
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 2);
                        }
                        else
                        {
                            SaveManager.Instance.SetMiniFlag(Mini.BookmarkUsed, 0);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(FrayDemon),"ALWAYS")]
        [HarmonyPrefix]
        static bool fixFray(ref enemyController ___en,ref bool ___addedOff,ref float ___P2,ref float ___lockw,ref bool ___gotlimitx,ref ParticleSystem[] ___ps,ref GameObject ___aura)
        {
            if (WorldManager.Instance.Area != 25)
            {
                if (EventManager.Instance.BossTime >= 5f && !___addedOff && ___en.HPLeft() <= ___P2)
                {
                    ___addedOff = true;
                    ___en.GiveBuffWithAdd(BuffType.DefenceUp, 110f, (byte)(2f + SaveManager.Instance.GetDifficultyModMinMax(0.4f, 0f, 8f)), 0);
                    ___en.GiveBuffWithAdd(BuffType.OffenseUp, 110f, (byte)(5f + SaveManager.Instance.GetDifficultyModMinMax(0.5f, -4f, 5f)), 0);
                }
                if (___en.phase == AIPhase.MAIN_1)
                {
                    LightManager.Instance.AddSunLight(1.2f, 1.2f);
                }
                if (___lockw < 203f && EventManager.Instance.getMode() == Mode.OFF)
                {
                    if (GemaBossRushMode.Instance.isBossRush() && !___gotlimitx)
                    {
                        ___gotlimitx = true;
                        EventManager.Instance.SetCounter(9, CameraScript.Instance.GetLimitLRCenter());
                    }
                    ___lockw = Mathf.Lerp(___lockw, 203f, Utility.GetSmooth(4f));
                    if (___lockw > 202f)
                    {
                        ___lockw = 203f;
                    }
                    //CameraScript.Instance.SetLimitLR(EventManager.Instance.GetCounter(9) - ___lockw, EventManager.Instance.GetCounter(9) + ___lockw);
                }
                if ((bool)___aura)
                {
                    ___aura.transform.position = ___en.t.position;
                    if (___en.GetBuffLv(BuffType.FrayBulletUp_TOP) > 0)
                    {
                        for (int i = 0; i < ___ps.Length; i++)
                        {
                            ParticleSystem.EmissionModule emission = ___ps[i].emission;
                            emission.enabled = true;
                            emission.rateOverTime = (int)___en.GetBuffLv(BuffType.FrayBulletUp_TOP);
                            if (!___ps[i].isPlaying)
                            {
                                ___ps[i].Play();
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < ___ps.Length; j++)
                        {
                            ___ps[j].Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
                        }
                    }
                }
                //CameraScript.Instance.SetTargetXY(___en.t, (EventManager.Instance.mainCharacter.t.position.x - ___en.t.position.x) / 1.25f, (EventManager.Instance.mainCharacter.t.position.y - ___en.t.position.y) / 3f, forcePositionIgnoreRoom: false);

                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(FrayDemon), "ALWAYS")]
        [HarmonyPrefix]
        static bool fixFrayAI(ref enemyController ___en,ref GameObject ___aura,ref ParticleSystem[] ___ps,ref byte ___APhase)
        {
            if(WorldManager.Instance.Area != 25 && ___en.phase == AIPhase.MAIN_1)
            { 
                if (!___aura)
                {
                    ___aura = UnityEngine.Object.Instantiate(ResourcePatch.getAreaResource(25).GetBossObject(0));
                    ___aura.transform.localScale = new Vector3(48f, 48f, 48f);
                    ___ps = ___aura.GetComponentsInChildren<ParticleSystem>();
                    ___APhase = (byte)(UnityEngine.Random.Range(0, 99) % 2);
                }
            }   
            return true;
        }
        [HarmonyPatch(typeof(Jethro), "ALWAYS")]
        [HarmonyPrefix]
        static bool fixJethroAI(ref enemyController ___en,ref GameObject ___aura,ref ParticleSystem[] ___ps,ref byte ___KMode, ref GameObject ___aura2, ref ParticleSystem[] ___ps2)
        {
            if(WorldManager.Instance.Area != 12)
            { 
                if (!___aura)
                {
                    ___aura = UnityEngine.Object.Instantiate(ResourcePatch.getAreaResource(12).GetBossObject(0));
                    ___aura.transform.localScale = new Vector3(70f, 70f, 70f);
                    ___aura.SetActive(false);
                    ___ps = ___aura.GetComponentsInChildren<ParticleSystem>();
                }
                if (!___aura2)
                {
                    ___aura2 = UnityEngine.Object.Instantiate(ResourcePatch.getAreaResource(12).GetBossObject(1));
                    ___aura2.transform.localScale = new Vector3(70f, 70f, 70f);
                    ___aura2.SetActive(false);
                    ___ps2 = ___aura.GetComponentsInChildren<ParticleSystem>();
                }

                if(___en.phase == AIPhase.K_PHASE_3 && ___KMode == 0){
                    ___aura.SetActive(true);
                }
                if (___en.phase == AIPhase.K_PHASE_3 && ___KMode == 1)
                {
                    ___aura2.SetActive(true);
                }
            }   
            return true;
        }
    }
}

