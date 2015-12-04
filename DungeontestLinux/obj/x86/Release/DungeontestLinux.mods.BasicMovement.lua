-- Spawns entity that chases when the player hits K

testEntityID = -1;
deltaTime = 0;

function PostGenerate()
	-- Create an ID for tracking our entities
	testEntityID = API:ClaimID("test", "Content/Sprites/Entities/player.png"); -- Create an entity
end

function ServerUpdate(DeltaTime)
	-- update delta time
	deltaTime = DeltaTime;

	-- if server host tapped K
	if Input:Tapped(Keys["K"]) then
		-- Spawn an entity with this ID
		API:SpawnEntity(testEntityID, 5, 5);
	end
end

function EntityUpdate(entity)
	-- if the entity is one of ours 
	if entity["id"] == testEntityID then
		closestPlayer = API:GetNearestPlayer(entity);
		
		entity.Chase(closestPlayer, 1);
	end
end