-- Mod testing

--if input:Held(Keys.K) then
print("\n[dungeontest] creating entity 'blackGuy'\n");
local blackGuy = dungeon:ClaimID("blackGuy", "Content/Sprites/Entities/player.png"); -- Create an entity
dungeon:SpawnEntity(blackGuy, 5, 3); -- Spawn the entity
print("\n[dungeontest] spawning entity 'blackGuy'\n");
--end
