# Tevi Randomizer

## Requirements

[BepInEx V 5.4 64 bit](https://docs.bepinex.dev/articles/user_guide/installation/index.html#tabpanel_bHGHmlrG6S_tabid-win)<br>

## Building the Project

For building the Project, the following DLLs from the Game are required and found in TEVI_data/Managed
+ Assembly-CSharp.dll
+ Assembly-CSharp-firstpass.dll
+ QFSW.QC.dll
+ Unity.TextMeshPro.dll
+ UnityEngine.UI.dll
+ Assembly-CSharp-firstpass.dll
  

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
+ Rabi Ribi Minigame is in Logic and requires atleast 100 Points and full Clear
+ Rabi Ribi Easteregg contains a Item location
+ It is possible to play the game with Sable and Celia locked behind extra Items  

## Custom Difficulty
### Currently Disabled 
When selecting a positive number for Fake Difficulty the enemy speed and bullet behaviour will change.
The hp and dmg the enemy will deal, is set to the Difficulty selected in the New Game screen selection.
This value can be change with the Bed in Tevi's Home.<br>
If a negtive number or nothing is set, the game will handle everything as usual.

## Disabling Randomizer
Either remove the TeviRandomizer.dll or rename the file.dat in the data Folder in the Randomizer Plugin

## Uninstalling
Delete the Randomizer Plugin from the plugins folder
