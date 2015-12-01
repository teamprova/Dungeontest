# Dungeontest
This is a test about retro-styled games. We have no clue what this game will be about, but currently we are developing the engine, and then having a Lua content side, so we can make game content, while allowing modders to use it.

# Compiling and Running
Load up the Visual Studio project, build and run, and you should be good to go. The application should export to the bin/ folder.

Further instruction on compiling will be given in a later version of this README.

# Tech
The tech behind Dungeontest

###### Framework
Dungeontest Engine uses MonoGame in C#, which is a cooler version of XNA, and Dungeontest API uses Moon# for Lua modifications.

###### Modding API
We use Moon# to interpret Lua code to C#, so we have C# code loading all scripts from Lua, and then interpreting them into C#, so we can make modifcations.

###### Rendering Engine
We are currently using a Raycasting engine to create a 3D space, even though all of the logic is in 2D.

###### Multiplayer
Uses .Net UdpClients. Currently featuring sending map data, sending entity data, sending texture data, and kicking idle players.

###### Inputs
Loosly based on Windows Forms controls, has buttons, text inputs, and containers.
