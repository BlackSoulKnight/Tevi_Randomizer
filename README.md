# Tevi Randomizer

## Requirements

[BepInEx V 5.4 64 bit](https://docs.bepinex.dev/articles/user_guide/installation/index.html#tabpanel_bHGHmlrG6S_tabid-win)<br>
Python version 3.8 or greater

## Building the Project

For self building the Project, the following DLLs from the Game are required and found in TEVI_data/Managed
+ Assembly-CSharp.dll
+ QFSW.QC.dll
+ Unity.TextMeshPro.dll
+ UnityEngine.UI.dll

## Install
Download a pre-Build version from https://github.com/BlackSoulKnight/Tevi_Randomizer/releases/tag/v0.9.4<br>
or self build it with the source files. <br>

Extract the zip in the BepInEx Plugins Folder<br><br>

If the the Project is build from source create a tevi_randomizer folder in the folder BepInEx/plugins.
Insert the fresh build TeviRandomizer.dll and SeedGenerator.py, Items.json and Locations.json into the tevi_randomizer folder. 

## Usage
Run SeedGenerator.py to generate a seed for the Randomizer.<br>
Start Tevi to play the Randomizer

## Disabling Randomizer
Either remove the TeviRandomizer.dll or rename the file.dat in the data Folder in the Randomizer Plugin

## Uninstalling
Delete the Randomizer Plugin from the plugins folder
