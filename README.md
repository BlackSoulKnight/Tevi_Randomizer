# Tevi Randomizer

## Tips/Rando exclusive knowledge
+ If you're stuck or need to back track, there is always a free crafting option for bell which teleports you to any Teleporter you have visited
+ The rando may require the freeroam exclusive tech (wall jumps and wall kicks) for vertical mobility before finding any vertical mobility items
  + To do a wall jump without the wall jump item, face away from a wall then jump, turn towards the wall then press away and jump
  + To do a wall kick, do the wall jump then quick drop into another wall
+ You may need to lure an enemy then quick drop on them to get additional height for some jumps/ledges
+ If you don't start with the knife, you can still use cross bombs/tornado slash/spanner bash
  + For cross bombs, use the cross bomb shortcut
  + For tornado slash/spanner bash, the same inputs still work, you just need to unlock the moves first
    note: the rando doesn't require these moves and doesn't check for them when placing items
+ Charge shots now require level 2 orbitars
+ If you don't start with Sable/Celia, the core expansion is not usable until one of them is found. And you're locked to that one character until the other is found
+ Cluster bomb is not usable without cross bombs
+ The left/right golden hands and the complete golden hands are all separate items which can be found. You can still turn in the left/right hands even if you've found the complete hands for example
+ There will always be atleast one of each progression item on the field or in crafting, so even if you randomize upgrades you don't need to worry about being softlocked by purple crystals
+ Defeating bosses progresses the internal chapter count which in turn unlocks crafting options and options for Vena's shop
  + Fray and Memloch also unlock more shop options in Morose and Ana Thema respectively
+ Memine races won't have mobility items as rewards
+ If you're unfamiliar with the item locations, starting with level 3 compass might be a good idea. Professor Zema will also provide hints if you talk to him

### Join the Tevi Speedrunning [Discord] (https://discord.gg/e4SW6AaBuj) to gain more Information about tricks of the Game

## Requirements

[BepInEx V 5.4 64 bit](https://docs.bepinex.dev/articles/user_guide/installation/index.html#tabpanel_bHGHmlrG6S_tabid-win)<br>

### Optional
[Tevi Map Tracker](https://github.com/vegemii/Tevi-Tracker/releases) made by Vegemii <br>
[Tevi Item Tracker](https://github.com/cramps-man/tevi-rando-progression-ui) made by Cramps-Man
 

## Install
Download the latest pre-Build version from here [Tevi Randomizer releases](https://github.com/BlackSoulKnight/Tevi_Randomizer/releases)<br>

Extract the zip in the BepInEx Plugins Folder<br><br>

If the the Project is build from source, create a tevi_randomizer folder in the folder BepInEx/plugins.
Insert the freshly build TeviRandomizer.dll into the tevi_randomizer folder, and copy the resource folder aswell.

## Usage
+ Start Tevi to play the Randomizer
+ Go into the new Main Menu option Randomizer
+ Setup Option for Randomizer and hit generate
+ Start a new Game (Random Badge will break the Randomizer)
  
Savefiles are seperated and not syncronized with the Original Game and Steam Cloud. <br>
The Randomizer data is saved within the Savefile.

## Changes to the base Game
+ Morose is accessable without Cross Bombs
+ Quickdrop damage is increased with each consecutive Quickdrop without receiving Damage or dealing Damage with anything else
+ Rabi Ribi Minigame is in Logic and requires atleast 100 Points and full Clear
+ Rabi Ribi Easteregg contains a Item location
+ It is possible to play the game with Sable and Celia locked behind extra Items
+ Talking to Professor Zema will provide hints as to general item locations

## Custom Difficulty
### Currently Disabled 
When selecting a positive number for Fake Difficulty the enemy speed and bullet behaviour will change.
The hp and dmg the enemy will deal, is set to the Difficulty selected in the New Game screen selection.
This value can be change with the Bed in Tevi's Home.<br>
If a negtive number or nothing is set, the game will handle everything as usual.

## Disabling Randomizer
Remove the TeviRandomizer.dll

## Uninstalling
Delete the Randomizer Plugin from the plugins folder


## Building the Project

For building the Project, the following DLLs from the Game are required and found in TEVI_data/Managed
+ Assembly-CSharp.dll
+ Assembly-CSharp-firstpass.dll
+ QFSW.QC.dll
+ Unity.TextMeshPro.dll
+ UnityEngine.UI.dll
+ Assembly-CSharp-firstpass.dll

