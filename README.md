# Dungeontest
This is a test about retro-styled games.
The codename of the project is Dungeontest, but repo is DungeonCrawler, as we do not know what the name of the game will be, nor what it will be about. All we know, is it will be a retro dungeon crawler.

# Compiling and Running
Load up the Visual Studio project, build and run, and you should be good to go.

# Tech
The tech behind Dungeontest

###### Framework
We are using MonoGame in C#, which is a cooler version of XNA.

###### Modding API
This is not complete currently, but we do have a Lua interpreter. We use Moon#, to interprete Lua, and convert to C#. In the future, it will be used for mods, but currently all it does is interpret Lua to C#.

###### Rendering Engine
We are currently using a Raycasting engine to create a 3D space, even though all of the logic is in 2D.
