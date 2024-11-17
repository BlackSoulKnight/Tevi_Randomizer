# Tevi Randomizer

## Tips/Rando exclusive knowledge
+ If you're stuck or need to back track, there is always a free crafting option for bell which teleports you to any Teleporter you have visited
+ The rando may require the freeroam exclusive tech (wall jumps and wall kicks) for vertical mobility before finding any vertical mobility items
  + To do a wall jump without the wall jump item, face away from a wall then jump, turn towards the wall then press away and jump
  + To do a wall kick, do the wall jump then quick drop into another wall
  + [Hidden Paths](Hidden_Paths.md) are spread around the Game World
+ You may need to lure an enemy then quick drop on them to get additional height for some jumps/ledges
+ If you don't start with the knife, you can still use cross bombs/tornado slash/spanner bash
  + For cross bombs, use the cross bomb shortcut
  + For tornado slash/spanner bash, the same inputs still work, you just need to unlock the moves first
    note: the rando doesn't require these moves and doesn't check for them when placing items
+ Charge shots now require level 2 orbitars
+ Sable / Celia require level 2 orbitars
+ If you don't start with Sable/Celia, the core expansion is not usable until one of them is found. And you're locked to that one character until the other is found
+ Cluster bomb is not usable without Cross bombs
+ The left/right golden hands and the complete golden hands are all separate items which can be found. You can still turn in the left/right hands even if you've found the complete hands for example
+ There will always be atleast one of each progression item on the field or in crafting, so even if you randomize upgrades you don't need to worry about being softlocked by purple crystals
+ Defeating bosses progresses the internal chapter count which in turn unlocks crafting options and options for Vena's shop
  + Fray and Memloch also unlock more shop options in Morose and Ana Thema respectively
+ Memine races won't have mobility items as rewards
+ If you're unfamiliar with the item locations, starting with level 3 compass might be a good idea. Professor Zema will also provide hints if you talk to him (non Archipelago Randomizer)

### Join the Tevi Speedrunning [Discord](https://discord.gg/e4SW6AaBuj) to gain more Information about tricks for the Game

## Requirements

[BepInEx V 5.4 64 bit](https://docs.bepinex.dev/articles/user_guide/installation/index.html#tabpanel_bHGHmlrG6S_tabid-win)<br>

### Optional
[Tevi Map Tracker](https://github.com/vegemii/Tevi-Tracker/releases) made by Vegemii <br>
[Tevi Item Tracker](https://github.com/cramps-man/tevi-rando-progression-ui) made by Cramps-Man
 

## Install
Download the latest released Version from here [Tevi Randomizer releases](https://github.com/BlackSoulKnight/Tevi_Randomizer/releases)<br>

Extract the zip in the BepInEx Plugins Folder<br><br>

Make sure that Build contains no .cs files <- those are C# source files and cannot be run by BepInEX

## Usage
### Normal Randomizer
+ Start Tevi to play the Randomizer
+ Go into the new Main Menu option Randomizer
+ Setup Option for Randomizer and hit generate
+ Start a new Game (Random Badge will break the Randomizer)

### Archipelago Randomizer
+ Click on Tab 2 in the Randomizer menu
+ Enter Server data
+ Click on connect
+ If the menu closes you are connected to the Server
+ After connecting start a new Free Roam game (you will receive 2 checks after the first Cutscene)
  
Savefiles are seperated and not syncronized with the Original Game and Steam Cloud. <br>
The Randomizer data is saved within the Savefile.

## Changes to the base Game
+ Morose is accessable without Cross Bombs (as an Option)
+ Quickdrop damage is increased with each consecutive Quickdrop without receiving Damage or dealing Damage with anything else
+ Rabi Ribi Minigame is in Logic and requires atleast 100 Points and full Clear
+ Rabi Ribi Easteregg and Library contains a Item location
+ It is possible to play the game with Sable and Celia locked behind extra Items
+ Talking to Professor Zema will provide hints as to general item locations

## Disabling Randomizer
Press in the Randomizer Menu A+S or LT+RT.
Or remove the TeviRandomizer.dll

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

## Custom Features
### Custom Difficulty
Change the Enemy HP and ATK to the set Difficulty instead of the original.

### Anti Flash
Removes all White Flashes from Teleport or other Events.