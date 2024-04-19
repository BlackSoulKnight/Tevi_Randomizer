# Tevi Randomizer

## Requirements

[BepInEx V 5.4 64 bit](https://docs.bepinex.dev/articles/user_guide/installation/index.html#tabpanel_bHGHmlrG6S_tabid-win)<br>

## Building the Project

For self building the Project, the following DLLs from the Game are required and found in TEVI_data/Managed
+ Assembly-CSharp.dll
+ Assembly-CSharp-firstpass.dll
+ QFSW.QC.dll
+ Unity.TextMeshPro.dll
+ UnityEngine.UI.dll
  

## Install
Download the latest pre-Build version from here [https://github.com/BlackSoulKnight/Tevi_Randomizer/releases/tag/v0.9.4](https://github.com/BlackSoulKnight/Tevi_Randomizer/releases/tag/v0.9.5.3)<br>
or self build it with the source files. <br>

Extract the zip in the BepInEx Plugins Folder<br><br>

If the the Project is build from source create a tevi_randomizer folder in the folder BepInEx/plugins.
Insert the fresh build TeviRandomizer.dll into the tevi_randomizer folder. 
After that copy the resource folder and create + copy the necessary Logic file in Tools/Resources.

## Usage
+ Start Tevi to play the Randomizer
+ Go into the new Main Menu option Randomizer
+ Setup Option for Randomizer and hit generate
+ Start a new Game (Random Badge will break the Randomizer)
  
Savefiles are seperated and not syncronized with the Original Game and Steam Cloud. <br>
The Randomizer data is saved within the Savefile.

## Disabling Randomizer
Either remove the TeviRandomizer.dll or rename the file.dat in the data Folder in the Randomizer Plugin

## Uninstalling
Delete the Randomizer Plugin from the plugins folder