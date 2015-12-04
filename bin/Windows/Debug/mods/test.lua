-- Mod testing

testEntityID = 0;

--if Input:Held(Keys.K) then
function PostGenerate()
	testEntityID = API:ClaimID("test", "Content/Sprites/Entities/player.png"); -- Create an entity

	--[[spawn entity ten times
	for i = 1, 40 do
		SpawnEntity();
	end
	--]]
end

function SpawnEntity()
	--print("\n[dungeontest] creating entity 'blackGuy'\n");

	local x = 0;
	local y = 0;
	local block = API:GetBlock(x, y);

	while block["solid"] do
		x = math.random()*100;
		y = math.random()*100;
		block = API:GetBlock(x, y);
	end

	API:SpawnEntity(testEntityID, x, y); -- Spawn the entity
	--print("\n[dungeontest] spawning entity 'blackGuy'\n");
end
