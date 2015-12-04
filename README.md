# Dungeontest
This is a test about retro-styled games. We have no clue what this game will be about, but currently we are developing the engine, and then having a Lua content side, so we can make game content, while allowing modders to use it.

# Compiling and Running
###### Windows
Load up the Visual Studio project, build and run, and you should be good to go. The application should export to the bin/ folder.

###### Ubuntu
Install Mono, MonoDevelop, and the MonoGame add-in for MonoDevelop, and open the solution in "/DungeontestLinux", and you can build and run, completely fine. Application should export to bin/.

This will export to an .exe, but because you have Mono, you goto the /bin directory in terminal and type "mono DungeontestLinux.exe", and it will run.

###### Disclaimer
Further instruction on compiling will be given in a later version of this README. And with detailed answers to why it sometimes does not work with Linux, and how to solve it.

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

# Dungeontest License
These are the different licenses from other libraries/frameworks/softwares we used. The full license for Dungeontest is under LICENSE.md

###### Dungeontest
Copyright (C) 2015 acerio, Preston Cammarata <preston@cammarata.info>

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; using version 3.0 of this License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

###### MoonSharp

The program and libraries are released under a 3-clause BSD license - see https://github.com/xanathar/moonsharp/blob/master/LICENSE

Parts of the string library are based on the KopiLua project (https://github.com/NLua/KopiLua). Debugger icons are from the Eclipse project (https://www.eclipse.org/).

###### MonoGame

MonoGame is released under [Microsoft Public License (Ms-PL)](https://github.com/mono/MonoGame/blob/develop/LICENSE.txt).

###### Lua

Lua is licensed under the terms of the MIT license reproduced below.
This means that Lua is free software and can be used for both academic
and commercial purposes at absolutely no cost.

For details and rationale, see http://www.lua.org/license.html .

Copyright (C) 1994-2008 Lua.org, PUC-Rio.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
