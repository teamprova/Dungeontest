-- Spawns test entities around the map

--[[testEntityID = 0;

function PostGenerate()
	testEntityID = API:ClaimID("test", "Content/Sprites/Entities/player.png"); -- Create an entity

	-- spawn 40 entities
	for i = 1, 25 do
		SpawnEntity();
	end
end

function SpawnEntity()
	--print("\n[dungeontest] creating entity 'blackGuy'\n");

	local x = 0;
	local y = 0;

	-- while the entity can't spawn
	while API:IsBlockSolid(x, y) do
		-- pick a new location
		x = math.random()*100;
		y = math.random()*100;
	end

	API:SpawnEntity(testEntityID, x, y); -- Spawn the entity
	--print("\n[dungeontest] spawning entity 'blackGuy'\n");
end]]
