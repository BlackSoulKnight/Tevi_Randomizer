import json
import os
import random
import sys

## Reading main Data files

    

Path = os.path.dirname(os.path.realpath(__file__))


from enum import Enum

class  Type(Enum):
		OFF = 0
		STACKABLE_COG = 1
		STACKABLE_HP = 2
		STACKABLE_MP = 3
		STACKABLE_EP = 4
		STACKABLE_MATK = 5
		STACKABLE_RATK = 6
		STACKABLE_BAG = 7
		STACKABLE_SHARD = 8
		I9 = 9
		I10 = 10
		I11 = 11
		I12 = 12
		I13 = 13
		I14 = 14
		I15 = 15
		I16 = 16
		I17 = 17
		I18 = 18
		I19 = 19
		I20 = 20
		I21 = 21
		ITEM_KNIFE = 22
		ITEM_ORB = 23
		ITEM_LINEBOMB = 24
		ITEM_AREABOMB = 25
		ITEM_BOMBFUEL = 26
		ITEM_HIJUMP = 27
		ITEM_SPEEDUP = 28
		ITEM_SLIDE = 29
		ITEM_WALLJUMP = 30
		ITEM_DOUBLEJUMP = 31
		ITEM_JETPACK = 32
		ITEM_WATERMOVEMENT = 33
		ITEM_PKBADGE = 34
		ITEM_MASK = 35
		ITEM_OrbTypeS2 = 36
		ITEM_OrbTypeS3 = 37
		ITEM_OrbTypeC2 = 38
		ITEM_OrbTypeC3 = 39
		ITEM_AntiDecay = 40
		ITEM_BombLengthExtend = 41
		ITEM_AirDash = 42
		ITEM_RailPass = 43
		ITEM_AirshipPass = 44
		ITEM_Royal = 45
		ITEM_AntiIce = 46
		ITEM_Explorer = 47
		ITEM_TempRing = 48
		ITEM_BoostSystem = 49
		ITEM_OrbBoostU = 50
		ITEM_OrbBoostD = 51
		ITEM_AttackRange = 52
		ITEM_DodgeShot = 53
		ITEM_EasyStyle = 54
		ITEM_Journal = 55
		ITEM_WonderNote = 56
		ITEM_RapidShots = 57
		ITEM_TeviBag = 58
		ITEM_OrbAmulet = 59
		ITEM_GoldenGlove = 60
		ITEM_Alterscope = 61
		ITEM_GoldenHands = 62
		ITEM_Rotater = 63
		ITEM_AirSlide = 64
		ITEM_VenaCharm = 65
		ITEM_ZCrystal = 66
		ITEM_MAX = 67
		ITEM_XC2 = 68
		ITEM_46 = 69
		ITEM_47 = 70
		ITEM_48 = 71
		ITEM_49 = 72
		ITEM_50 = 73
		ITEM_51 = 74
		ITEM_52 = 75
		ITEM_53 = 76
		ITEM_54 = 77
		ITEM_55 = 78
		ITEM_56 = 79
		ITEM_57 = 80
		ITEM_58 = 81
		ITEM_59 = 82
		ITEM_60 = 83
		ITEM_61 = 84
		ITEM_62 = 85
		ITEM_63 = 86
		ITEM_64 = 87
		ITEM_65 = 88
		ITEM_66 = 89
		ITEM_67 = 90
		ITEM_68 = 91
		Function_MaterialExchangeA = 92
		Function_MaterialExchangeB = 93
		QUEST_Necklace = 94
		QUEST_Seal = 95
		QUEST_Crest = 96
		QUEST_Heart = 97
		QUEST_SusDevice = 98
		QUEST_Flute = 99
		QUEST_Memory = 100
		QUEST_Decay = 101
		QUEST_Compass = 102
		QUEST_GNotebook = 103
		QUEST_AirCam = 104
		QUEST_Reliever = 105
		QUEST_GHandL = 106
		QUEST_GHandR = 107
		QUEST_Holostat = 108
		QUEST_RabiPillow = 109
		QUEST_LibraryKey = 110
		ITEM_88 = 111
		ITEM_89 = 112
		ITEM_90 = 113
		ITEM_91 = 114
		ITEM_92 = 115
		ITEM_93 = 116
		QUEST_MAX = 117
		Other_Unknown = 118
		BADGE_START = 119
		BADGE_ANTIENERGYBALL = 120
		BADGE_BACKSTAB = 121
		BADGE_RangeCombo = 122
		BADGE_MAXHPUP = 123
		BADGE_CELIATYPEBANGLE = 124
		BADGE_Normal2AntiHealth = 125
		BADGE_RevengeA = 126
		BADGE_DodgeWeakGroundUp = 127
		BADGE_DodgeWeakAirDown = 128
		BADGE_DodgeWeakAirDash = 129
		BADGE_DodgeStrongAirDown = 130
		BADGE_DodgeSlide = 131
		BADGE_DodgeEPBoost = 132
		BADGE_StyleComboAirNormal3A = 133
		BADGE_ChangeOrbCharger = 134
		BADGE_WallCharger = 135
		BADGE_CeliaAShortRangeBurst = 136
		BADGE_Lucky7Defend = 137
		BADGE_LateCharge = 138
		BADGE_AutoAirCombo = 139
		BADGE_WeakGroundUpModOneHit = 140
		BADGE_GroundNormal4Mod = 141
		BADGE_AirComboQuicken = 142
		BADGE_SableALongRangeSnipe = 143
		BADGE_InvincibilityExtend = 144
		BADGE_CommonDropRecover = 145
		BADGE_MapDiscoverRecover = 146
		BADGE_BaseComboBooster = 147
		BADGE_BattleStartCharger = 148
		BADGE_BombNormal4Buffer = 149
		BADGE_ChargeShotDiscount = 150
		BADGE_WeakGroundUpModCombo = 151
		BADGE_DodgeCharger = 152
		BADGE_StepUpCharge = 153
		BADGE_CeliaALongStun = 154
		BADGE_DodgeHealthCounter = 155
		BADGE_ComboBreakBoost = 156
		BADGE_ComboAntiFullHealth = 157
		BADGE_WeakAirNormal3ModDamageBoost = 158
		BADGE_BackflipSnipe = 159
		BADGE_AirComboExtend = 160
		BADGE_EnergyDefend = 161
		BADGE_QuickMeleeLagA = 162
		BADGE_QuickMeleeLagB = 163
		BADGE_Normal3HRedCancel = 164
		BADGE_ComboTimeExtendA = 165
		BADGE_ComboTimeExtendB = 166
		BADGE_ComboTimeExtendC = 167
		BADGE_ToArmorReducer = 168
		BADGE_ComboDamageBoost = 169
		BADGE_NormalShotWhenChargedShot = 170
		BADGE_FrameCancel = 171
		BADGE_LowerThan20 = 172
		BADGE_GroundWeakUpAirExcute = 173
		BADGE_InstantDodgeMeter = 174
		BADGE_Normal2MultHit = 175
		BADGE_MAXHPBIGUP = 176
		BADGE_ConvertDef2Off = 177
		BADGE_ConvertOff2Def = 178
		BADGE_ComboDodgeMeter = 179
		BADGE_BreakTimeExtend = 180
		BADGE_BreakDamageBoost = 181
		BADGE_BreakRangeChaser = 182
		BADGE_ComboSaver = 183
		BADGE_1HealthTypeA = 184
		BADGE_EPCombo = 185
		BADGE_SimpleBreak = 186
		BADGE_QuickDropExtendA = 187
		BADGE_QuickDropExtendB = 188
		BADGE_QuickDropDouble = 189
		BADGE_CrystalAbsorberS = 190
		BADGE_CrystalAbsorberC = 191
		BADGE_GroundNormalComboSpeedup = 192
		BADGE_AirNormalComboSpeedup = 193
		BADGE_DoubleNormalComboSpeedup = 194
		BADGE_NormalShotReducerA = 195
		BADGE_NormalShotReducerB = 196
		BADGE_QuickBomber = 197
		BADGE_Normal3HModStabA = 198
		BADGE_Normal3HModStabB = 199
		BADGE_BombDamageBoost = 200
		BADGE_WeakAirDownFreqUpA = 201
		BADGE_WeakAirDownFreqUpB = 202
		BADGE_AreaBombThrower = 203
		BADGE_StyleComboAirDashA = 204
		BADGE_DoubleDispelLineBombHigh = 205
		BADGE_DoubleDispelLineBombLow = 206
		BADGE_StyleComboAirDashS = 207
		BADGE_StyleComboHeavyGroundFrontA = 208
		BADGE_GroundBombQuickExcute = 209
		BADGE_StyleComboNormal4HAAA = 210
		BADGE_StyleComboSlidingS = 211
		BADGE_StyleComboBackflipA = 212
		BADGE_StyleComboPassiveA = 213
		BADGE_StyleComboPassiveS = 214
		BADGE_StyleComboPassiveMAX = 215
		BADGE_StrongFrontEnhance = 216
		BADGE_Unlucky7 = 217
		BADGE_BoostMeleeOffense = 218
		BADGE_Cursed6 = 219
		BADGE_BoostCharging = 220
		BADGE_PerfectCharge = 221
		BADGE_StyleCharge = 222
		BADGE_ComboCharge = 223
		BADGE_HealCharge = 224
		BADGE_BreakHeal = 225
		BADGE_SableBNormalShotDebuff = 226
		BADGE_BoostDodgeAdd = 227
		BADGE_DodgeMeterBoost = 228
		BADGE_StyleComboAddDamageA = 229
		BADGE_StyleComboReduceDamageS = 230
		BADGE_DebuffCounter = 231
		BADGE_CrystalIncrease = 232
		BADGE_Recover3 = 233
		BADGE_AntiAir = 234
		BADGE_Cannon = 235
		BADGE_AllMeleeSpeedUp = 236
		BADGE_Instant1 = 237
		BADGE_AutoDodge = 238
		BADGE_BuffRush = 239
		BADGE_DodgeChargedDodgeShot = 240
		BADGE_BoostShieldAbsorb = 241
		BADGE_Hurt2Punisher = 242
		BADGE_SupportShot = 243
		BADGE_CeliaTypeBAmount = 244
		BADGE_DoubleJumpStrike = 245
		BADGE_QuickMPRegainA = 246
		BADGE_QuickMPRegainB = 247
		BADGE_PerfectCost = 248
		BADGE_BoostTimeExtend = 249
		BADGE_BackflipMultiHit = 250
		BADGE_DamageToMaxHP = 251
		BADGE_FallDamageReduce = 252
		BADGE_DoubleGroundNormal3N1 = 253
		BADGE_DoubleGroundNormal3N2 = 254
		BADGE_1HealthTypeB = 255
		BADGE_1HealthTypeC = 256
		BADGE_StraightCost = 257
		BADGE_HitMP = 258
		BADGE_1HealthTypeD = 259
		BADGE_RapidShotsEnhance = 260
		BADGE_MAXHPCost = 261
		BADGE_SableAHitIncrease = 262
		BADGE_CeliaASlowShot = 263
		BADGE_SableBReturnStyle = 264
		BADGE_SableCBigExplode = 265
		BADGE_HP2MP = 266
		BADGE_MAXHP2MP = 267
		BADGE_RythemCharge = 268
		BADGE_NonChargeShotRapid = 269
		BADGE_GoldenLuck = 270
		BADGE_SableALastHitEnhance = 271
		BADGE_CeliaCShotIncrease = 272
		BADGE_CeliaCShotAllDirection = 273
		BADGE_SableCSaver = 274
		BADGE_MAXHP2MAXMP = 275
		BADGE_100COMBO = 276
		BADGE_120CHARGE = 277
		BADGE_NormalSupportShot = 278
		BADGE_AutoChargeCombo = 279
		BADGE_Lucky7DamageReduce = 280
		BADGE_8thDamageHeal = 281
		BADGE_SuperKnockBomb = 282
		BADGE_LateAttack = 283
		BADGE_StrongFrontRapid = 284
		BADGE_StrongFrontFly = 285
		BADGE_EPMeleeReduce = 286
		BADGE_EasyDodge = 287
		BADGE_JetpackMP = 288
		BADGE_Lucky7C = 289
		BADGE_KnockConvert = 290
		BADGE_ChargeHeal = 291
		BADGE_BombCharge = 292
		BADGE_MAXMPCost = 293
		BADGE_MPAllBurst = 294
		BADGE_AirCombo2Hits = 295
		BADGE_StrongAirUpDebuff = 296
		BADGE_Amulet10 = 297
		BADGE_AutoBombChain = 298
		BADGE_PrepDef = 299
		BADGE_SuperArmor2 = 300
		BADGE_BOnceHeal = 301
		BADGE_AmuletMPCharge = 302
		BADGE_AutoHealing = 303
		BADGE_ChargeIntoEnemy = 304
		BADGE_GStrongUpBulletCancelS = 305
		BADGE_GStrongUpBulletCancelC = 306
		BADGE_HitCountChargeRate = 307
		BADGE_SafeItemUse = 308
		BADGE_4Knifes = 309
		BADGE_GStrongUpWidth = 310
		BADGE_KnockHitEnhance = 311
		BADGE_PowerDrop = 312
		BADGE_CrystalAttack = 313
		BADGE_SlideThenStrongUp = 314
		BADGE_LaserRes = 315
		BADGE_ConsumeableCharge = 316
		BADGE_GroundNormalComboAutoSpeedup = 317
		BADGE_CrystalBulletCancel = 318
		BADGE_AntiGround = 319
		BADGE_EnemyDefeatExp = 320
		BADGE_SuperArmor33 = 321
		BADGE_ArrowDamageUp = 322
		BADGE_StyleComboBackImageS = 323
		BADGE_BadgeCDReduce = 324
		BADGE_HP2Damage6 = 325
		BADGE_BoostQuickErase = 326
		BADGE_CrystalHeal = 327
		BADGE_BoostEnhance = 328
		BADGE_BoostHitCount = 329
		BADGE_AmuletQuicken = 330
		BADGE_CrystalGen = 331
		BADGE_RevengeB = 332
		BADGE_PurchaseBadgeCost = 333
		BADGE_CraftBadgeCost = 334
		BADGE_RememberBrutalForce = 335
		BADGE_GroundNormalCombo4AltDmg = 336
		BADGE_DashFlashInv = 337
		BADGE_UpperDoubleHitAfterOutline = 338
		BADGE_BoostSizeIncrease = 339
		BADGE_BoostCostCut = 340
		BADGE_AmuletDouble = 341
		BADGE_ComboStyleDamageUp = 342
		BADGE_FasterTeviStrongGroundUp = 343
		BADGE_BounceTeviStrongAirUp = 344
		BADGE_UpperHigh = 345
		BADGE_DoubleAirDash = 346
		BADGE_StyleComboWeakGroundUpA = 347
		BADGE_StyleComboNormal3H = 348
		BADGE_StyleComboNormal1QuickFar = 349
		BADGE_SlideHalt = 350
		BADGE_BossPassing = 351
		BADGE_StyleComboBackflipBomb = 352
		BADGE_StyleComboNewAttack = 353
		BADGE_WeakAirDownRotate = 354
		BADGE_DoubleJumpAttack = 355
		BADGE_EarlyKnockRecover = 356
		BADGE_BombRecoverDebuff = 357
		BADGE_BombDispelDebuff = 358
		BADGE_FreeFoodRefill = 359
		BADGE_SuperFrameCancel = 360
		BADGE_AutoFoodCraft = 361
		BADGE_AutoFoodUse = 362
		BADGE_AutoCombo = 363
		BADGE_GroundNormalCombo4AltTiming = 364
		BADGE_TauntThink = 365
		BADGE_TauntKnife = 366
		BADGE_TauntPhoto = 367
		BADGE_TauntStare = 368
		BADGE_TauntYawn = 369
		BADGE_TauntSit = 370
		BADGE_DominantEffectDownA = 371
		BADGE_DominantEffectDownB = 372
		BADGE_ArmorCritCrit = 373
		BADGE_BoostFullOffense = 374
		BADGE_RangeBreak = 375
		BADGE_AntiAirPlat = 376
		BADGE_AutoPilot = 377
		BADGE_MAX = 378
		Useable_CocoBall = 379
		Useable_Puff = 380
		Useable_Lollipop = 381
		Useable_EnergyDrink = 382
		Useable_MintIceCream = 383
		Useable_Donut = 384
		Useable_VoodooPie = 385
		Useable_RumiCake = 386
		Useable_MilkTea = 387
		Useable_WaffleWonderTemp = 388
		Useable_Mysterious = 389
		Useable_WaffleAHoneycloud = 390
		Useable_WaffleBMeringue = 391
		Useable_WaffleCMorning = 392
		Useable_WaffleDJellydrop = 393
		Useable_WaffleElueberry = 394
		Useable_WaffleWonderFull = 395
		Useable_Bell = 396
		Useable_VenaBombSmall = 397
		Useable_VenaBombBig = 398
		Useable_VenaBombBunBun = 399
		Useable_VenaBombDispel = 400
		Useable_VenaBombHealBlock = 401
		Useable_Bookmark = 402
		Useable_BSnack = 403
		Useable_Biscuit = 404
		MAX = 405
	

class Item:
    def __init__(self,name,slotid,requierment) -> None:
        self.name = name
        self.slotID = slotid
        self.requierment = []
        self.requierment = requierment

    def isPossible(self,checklist):
        for k in self.requierment:

            print("\n")
        return False



class Area():


    def __init__(self,name,money,connection) -> None:
        self.name = name
        self.money = money
        self.items = {}
        self.connection = connection


class Map:

    def __init__(self) -> None:
        self.startArea = None
        self.quicklink = None
        self.items = []
        self.createMap() 

    def createMap(self):
        self.quicklink = {}
        ## Load Areas
        for val in json.load(open(Path+"\Area.json")).values():
            for v in val:
                if v["Name"] not in self.quicklink:
                    self.quicklink[v["Name"]] = Area(v["Name"],v["Money"],v["Connections"])
                if v["Name"] == "Start Area":
                    self.startArea = self.quicklink[v["Name"]]

        # Connect Areas
        tmp = {}
        for i in self.quicklink.values(): 
            for n in i.connection:
                if n["Exit"] ==  "":
                    continue
                n["Exit"] = self.quicklink[n["Exit"]]
        
        #Load Items 
        self.items = json.load(open(Path+"\Location.json"))


    def addItemsToMap(self):
        for item in self.items:
            #if requirement / difficulty allowed
            if item["Location"] == "Crafting":
                item["Location"] = "Crafting"
            if item["Location"] == "Memine":
                item["Location"] = "Start Area"
            if item["Location"] == "Vena":
                item["Location"] = "Ana Thema"
            self.quicklink[item["Location"]].items[item["Itemname"]] = Item(item["Itemname"],item["slotId"],item["Requirement"])


class Validator:
    def __init__(self,_map) -> None:
        self.checkList=[]
        self.mapList=[]
        self.itemList=[]
        self.bossCount = 0
        self.explorerMode = False
        self.UpgradeAble= ["ITEM_KNIFE",
        "ITEM_ORB" ,
        "ITEM_RapidShots" ,
        "ITEM_AttackRange" ,
        "ITEM_EasyStyle" ,
        "ITEM_LINEBOMB" ,
        "ITEM_AREABOMB" ,
        "ITEM_SPEEDUP" ,
        "ITEM_AirDash",
        "ITEM_WALLJUMP",
        "ITEM_JETPACK",
        "ITEM_BoostSystem" ,
        "ITEM_BombLengthExtend",
        "ITEM_MASK" ,
        "ITEM_TempRing" ,
        "ITEM_DodgeShot" ,
        "ITEM_Rotater" ,
        "ITEM_GoldenGlove" ,
        "ITEM_OrbAmulet" ,
        "ITEM_BOMBFUEL",
        "ITEM_Explorer"]

        self.map = _map


    def isPossible(self,requirement):
        for s in requirement["Method"].split("&&"):
            cut = s.split()
            if len(cut) == 0:
                continue
            elif cut[0] == "Coins":
                continue
            elif cut[0] == "Upgrade" or cut[0] == "Core"  or "EliteChallange" in cut[0]:
                return False
            elif cut[0] == "Boss":
                self.bossCount += 1
                continue
            elif cut[0] == "Chapter":
                if self.explorerMode == True:
                    continue
                elif cut[1] == "1" and self.bossCount >= 1:
                    continue
                elif cut[1] == "2" and self.bossCount >= 3:
                    continue
                elif cut[1] == "3" and self.bossCount >= 5:
                    continue
                elif cut[1] == "4" and self.bossCount >= 7:
                    continue
                elif cut[1] == "5" and self.bossCount >= 10:
                    continue
                elif cut[1] == "6" and self.bossCount >= 13:
                    continue
                elif cut[1] == "7" and self.bossCount >= 16:
                    continue
                elif cut[1] == "8" and self.bossCount >= 20:
                    continue
                else:
                    return False                   
            elif cut[0] not in self.checkList:
                return False
                
        return True

    def checkItems(self,currArea = "Start Area",area = []):
        if len(self.mapList) == 0:
            self.mapList.append(currArea)
        else:
            if currArea in self.mapList:
                return
            else:
                self.mapList.append(currArea)

        for item in self.map.quicklink[currArea].items.values():
            for req in item.requierment:                   
                if self.isPossible(req) and (item.name,item.slotID) not in self.itemList:
                    self.itemList.append((item.name,item.slotID))
                    if item.name not in self.checkList:
                        self.checkList.append(item.name)
                    elif item.name+"2" not in self.checkList:
                        self.checkList.append(item.name+"2")
                    elif item.name+"3" not in self.checkList:
                        self.checkList.append(item.name+"3")

        for edge in self.map.quicklink[currArea].connection:
            if(edge["Exit"] == ""):
                continue
            if self.isPossible(edge):
                self.checkItems(edge["Exit"].name)

    def validate(self):
        self.checkList = []
        self.itemList = []
        self.mapList = []
        
        lastLen = -1
        self.checkList.append("ItemUse")
        while(lastLen != len(self.checkList)):
            lastLen = len(self.checkList)
            self.mapList = [] 
            self.bossCount = 0
            self.checkItems()
            if("ITEM_KNIFE" in self.checkList and "SpinnerBash" not in self.checkList and  "TornadoSpin" not in self.checkList):
                    self.checkList.append("SpinnerBash")
                    self.checkList.append("TornadoSpin") 


        self.checkItems()
        self.checkItems()
        GearCount = 0
        for item in self.itemList:
            if item[0] == "STACKABLE_COG":
                GearCount += 1
        
		#check if game is completable
        if "ITEM_SLIDE" not in self.checkList or "ITEM_LINEBOMB" not in self.checkList or "ITEM_AirSlide" not in self.checkList or  "ITEM_Rotater" not in self.checkList or GearCount < 16 or self.bossCount < 16:
            return False
        return True

            

class Generator:
    def __init__(self):
        self.placeditem = []
        self.map = Map()
        self.validator = Validator(self.map)
        self.extraRATK = -1
        self.gibCompass = -1
        self.knifeStart = -1
        self.options = []

    
    def newRandomItem(self):
        created = False

        while(not created):
            item = [0,0]
            val = random.randint(1,4)
            if val == 1:
                val = random.randint(1,8)
                #Potion
                if val==7:
                    item = [val,random.randint(0,4)]
                elif val==8:
                    item = [val,random.randint(0,14)]
                elif val==1:
                    item = [val,random.randint(0,24)]
                else:
                    item = [val,random.randint(0,34)]
            elif val == 2:
                #itemds
                val = random.randint(22,66)
                if val == 58 or val == 55:
                    continue
                if Type(val).name in self.validator.UpgradeAble:
                    item = [val,random.randint(4,6)]
                else:
                    item = [val,1]
            elif val == 3:
                #Badge
                val = random.randint(120,377)
                item = [val,1]
            else:
                #extra items
                val = random.randint(0,2)
                if val == 0:
                    item = [106,1]
                elif val == 1:
                    item = [107,1]
                else:
                    item = [110,1]

            if not(item in self.placeditem):
                self.placeditem.append(item)
                created = True
        return item 

    def randomize(self):
        flag = False
        count = 0
        while(not flag):
            count += 1
            self.placeditem = []
            self.map.createMap()
            random.shuffle(self.map.items)
            for k in self.map.items:
                item = self.newRandomItem()
                k["OldItem"] = k["Itemname"]
                k["OldSlotID"] = k["slotId"]
                k["Itemname"] = Type(item[0]).name
                k["slotId"] = item[1]
            self.map.addItemsToMap()     
            flag = self.validator.validate()
            print("Try Number:"+str(count))
        self.map.items = sorted(self.map.items, key=lambda d: d["slotId"])
        self.map.items = sorted(self.map.items, key=lambda d: d["Itemname"])
        self.generateFile()

    def newRandomie(self):
        finished = False
        while(not finished):
            itemPool = []
            self.map.createMap()
            for item in self.map.items:
                itemPool.append((item["Itemname"],item["slotId"]))
            


    def generateFile(self):
        output = ""
        output2= ""
        if not os.path.exists("./data"):
            os.makedirs("./data")
                    
        file = open("data/file.dat",'w+')
        spoilerLog = open("data/Spoiler.txt",'w+')
        #adding knife and orb to start tiem (2 items may be lost)
        if self.knifeStart !=0:
            output += "22,1:22,4;"
        if self.gibCompass != 0:
            output += "4200,0:4200,1;"
        output += "4201,0:4201,"+str(self.extraRATK)+";"
        
        for k in self.map.items:
            file.write(f"{Type[k['OldItem']].value},{k['OldSlotID']}:{Type[k['Itemname']].value},{k['slotId']};\n")
            line =f"Randomized Item: {Type[k['Itemname']]} Slot: {k['slotId']}"
            for x in range(0,70-len(line)):
                line += " "
            line += f"Original Item: {Type[k['OldItem']]} Slot: {k['OldSlotID']} "
            for x in range(0,140-len(line)):
                line+=" "
            line += f"Location: {k['Location']}\n"
            spoilerLog.write(line)
       
        file.close()
        spoilerLog.close()




def convertAreaJsonToTxt():
    s = open("Area.json",'r')
    d = open("Area.txt",'w+')
    c = open("Connection.txt",'w+')
    s = json.load(s)
    for k,v in s.items():
        for a in v:
            d.write(f"{a['Name']}\n")
            for cs in a["Connections"]:
                c.write(f"{a['Name']}:{cs['Exit']}:{cs['Method']}\n")
    d.close()
    


def convertLocationJsonToTxt():
    s = open("Location.json",'r')
    d = open("Location.txt",'w+')
    s = json.load(s)
    for k in s:
        d.write(f"{k['Itemname']}:{k['Location']}:{k['slotId']}:")
        l = ""
        for req in k["Requirement"]:
            l+=f"{req['Method']},{req['Difficulty']};"
        l = l[:-1]
        d.write(l+"\n")
    d.close()

convertAreaJsonToTxt()
convertLocationJsonToTxt()


def generateSeed():
    #Small Interface
    t = Generator()


    print("Tevi Randomizer Seed Generator\n\n")
    print("Extra Range and Melee Attack Potion")
    while (t.extraRATK == -1):
        try:
            value = int(input("Number from 0 - 29:\n"))
            if value <30 and value >=0:
                t.extraRATK = value
            else:
                continue
        except:
            continue

    print("Start with level Compass 3")
    while(t.gibCompass == -1):
        try:
            t.gibCompass = int(input("Any Number: Yes, 0: No\n"))
        except:
            pass
        
    print("Start with Knife")
    while(t.knifeStart == -1):
        try:
            t.knifeStart = int(input("Any Number: Yes, 0: No\n")) 
        except:
            pass

    seed = input("Enter a Seed: ")    
    if(len(seed) == 0):
        random.seed()
        seed = random.randint(0,2**64)
        print("No Seed was enterd")
        print("Using seed: "+str(seed))

    random.seed(seed)

    t.validator.explorerMode = False
    t.randomize()

    print("Generating the Seed has finished")
    input()

