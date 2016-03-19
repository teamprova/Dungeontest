_enemyID  = -1;
--_enemy2ID = -2;

function PostGenerate (  )
	_enemyID  = API:ClaimID ( "test", "Content/Sprites/Entities/player.png" );
	--_enemy2ID = API:ClaimID ( "test", "Content/Sprites/Entities/player.png" );

	API:SpawnEntity ( _enemyID, 5, 5 );
	--API:SpawnEntity ( _enemy2ID, 5, 5 );
end


function EntityUpdate( entity )
	if entity["id"] == _enemyID then
		_closestPlayer = API:GetNearestPlayer ( entity );
		
		entity.Chase ( _closestPlayer, 1 );
	end

	--if entity["id"] == _enemy2ID then
		--_closestPlayer = API:GetNearestPlayer ( entity );
		
		--entity.Evade ( _closestPlayer, 1 );
	--end

end