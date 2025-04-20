# REPO-IVSpawner

- Item and Valuable Spawner is a mod for R.E.P.O that allows you to spawn any item or valuable instantly through a simple user interface.

- It's my first project, and I prefer to give you all the content of my mod because when I created it, I would have liked to have this kind of help.

# How to use?

- To use it you need to install BepInEx 5 : https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/ in your R.E.P.O game directory, place the .dll in : BepInEx\plugins\

- You also need the REPOLib dependency : https://github.com/ZehsTeam/REPOLib
  Once it's installed, launch the game and press F1 to open the IV Spawner menu.
  Click on the desired tab and then click on any entry to spawn it in-game

# For modders who want to customize it

- For anyone who wants to create a spawnitem mod, go to this link : https://github.com/drilixyr/REPO-IVSpawner. You’ll find all the content of this mod and you can change anything

# How to build? (for developers only)

- You need to install .NET SDK 6.0+ and Visual Studio Code with C# Dev Kit, and optionally Git

- Step 1 : create a terminal with VS Code

- Step 2 : go to the mod folder path using cd C:\Users.........\IVSpawner

- Step 3 : dotnet build YourProjectName.csproj

- Step 4 : get the .dll in bin\Release\netstandard2.1\

# Help

- If the menu doesn't open when pressing F1, make sure BepInEx and REPOLib are correctly
  installed.

- Make sure the .dll is in the correct folder: BepInEx\plugins\

- If the mod builds but doesn't work in-game, double-check the project is targeting .NET Standard 2.1 and that you are using the correct REPOLib version.

- For assistance with developing your mod, check out this Wiki: https://repomods.com/overview.html

**Mod made by https://github.com/drilixyr**

**Logo made by https://github.com/spxnso**

**Thanks, Spxnso for the help <3**
