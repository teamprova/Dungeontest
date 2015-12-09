WIDTH = 100;
HEIGHT = 100;

MIN_ROOMS = 12;
MAX_ROOMS = 15;
MIN_ROOM_SIZE = 8;
MAX_ROOM_SIZE = 15;

rooms = {};
roomsToMake = 0;

CEMENT = 0;
CEMENT_BRICKS = 0;

function LoadContent()
	-- Claim IDs
	CEMENT = API:ClaimID("Cement", "Content/Sprites/Blocks/cement.png");
	CEMENT_BRICKS = API:ClaimID("CementBricks", "Content/Sprites/Blocks/brick.png");
end

function PreGenerate()
	-- reset settings
	rooms = {};
	roomsToMake = math.random(MIN_ROOMS, MAX_ROOMS);
	
	MakeRooms();
	SquashRooms();
	PlaceBlocks();
	UpdateData();
end

function MakeRooms()
	-- Generate the rooms
	API:ChangeTask("MAKING ROOMS");
	print "Making Rooms";

	while #rooms ~= roomsToMake do
		local room = {X = 0, Y = 0, Width = 0, Height = 0};

		room.X = math.random(1, WIDTH - MAX_ROOM_SIZE - 1);
		room.Y = math.random(1, HEIGHT - MAX_ROOM_SIZE - 1);
		room.Width = math.random(MIN_ROOM_SIZE, MAX_ROOM_SIZE);
		room.Height = math.random(MIN_ROOM_SIZE, MAX_ROOM_SIZE);

		if DoesCollide(room, -1) ~= true then
			-- add the room
			room.Width = room.Width - 1;
			room.Height = room.Height - 1;
			
			table.insert(rooms, room);
		end
	end
end

function SquashRooms()
	API:ChangeTask("SQUISHING ROOMS");
	print "Packing Rooms";
	
	for i = 0, 9 do
		for j = 1, #rooms do
			local room = rooms[j];
			
			while true do
				local oldRoom = { X = room.X, Y = room.Y};
				
				-- try to move to 1, 1
				if room.X > 1 then
					room.X = room.X - 1;
				end
				
				if room.Y > 1 then
					room.Y = room.Y - 1;
				end
				
				-- set new data
				rooms[j] = room;
				
				-- stop if at 1, 1
				if room.X == 1 and room.Y == 1 then
					break;
				end
				
				if DoesCollide(room, j) then
					-- hit another room
					-- reset position
					room.X = oldRoom.X;
					room.Y = oldRoom.Y;
					
					rooms[j] = room;
					break;
				end
			end
		end
	end
end

function PlaceBlocks()
	-- making pathways
	API:ChangeTask("CREATING PATHS");
	print "Generating pathways";

	for i = 1, #rooms do
		local room = rooms[i];
		
		local closestRoom = FindClosestRoom(room, room);
		local secondClosestRoom = FindClosestRoom(room, closestRoom);
		
		CreatePath(room, closestRoom);
		CreatePath(room, secondClosestRoom);
	end
	
	-- place rooms
	API:ChangeTask("PLACING ROOMS");
	print "Placing Rooms";
	
	for i = 1, #rooms do
		local room = rooms[i];
		
		for x = room.X, room.X + room.Width - 1 do
			for y = room.Y , room.Y + room.Height - 1 do
				API:SetBlock(x, y, CEMENT, false);
			end
		end
	end

	-- set null blocks to solid cobble
	API:ChangeTask("PLACING BLOCKS");
	print "Placing Blocks";

	for x = 0, WIDTH - 1 do
		for y = 0, HEIGHT - 1 do
			if API:GetBlock(x, y) == nil then
				API:SetBlock(x, y, CEMENT_BRICKS, true);
			end
		end
	end
end

function CreatePath(roomA, roomB)
	local pointA = {
		X = math.random(roomA.X, roomA.X + roomA.Width - 1),
		Y = math.random(roomA.Y, roomA.Y + roomA.Height - 1)
	};

	local pointB = {
		X = math.random(roomB.X, roomB.X + roomB.Width - 1),
		Y = math.random(roomB.Y, roomB.Y + roomB.Height - 1)
	};
	
	while pointA.X ~= pointB.X or pointA.Y ~= pointB.Y do
		if pointB.X ~= pointA.X then
			if pointB.X > pointA.X then
				pointB.X = pointB.X - 1;
			else
				pointB.X = pointB.X + 1;
			end
		elseif pointB.Y ~= pointA.Y then
			if pointB.Y > pointA.Y then
				pointB.Y = pointB.Y - 1;
			else
				pointB.Y = pointB.Y + 1;
			end
		end
		
		API:SetBlock(pointB.X, pointB.Y, CEMENT, false);
	end
end

function DoesCollide(room, ignore)
	for i = 1, #rooms do
		if i ~= ignore then
			local check = rooms[i];
			
			if (room.X + room.Width < check.X or room.X > check.X + check.Width or room.Y + room.Height < check.Y or room.Y > check.Y + check.Height) == false then
				return true;
			end
		end
	end
	-- did not collide with anything
	return false;
end

function FindClosestRoom(room, ignore)
	local mid = GetCenter(room);
	local closest = {X = 0, Y = 0, Width = 0, Height = 0};
	
	local closestDistance = 1000;
	
	for i = 1, #rooms do
		local check = rooms[i];
		
		if Equals(check, room) == false and Equals(check, ignore) == false then
			-- get the center of the room to check
			local checkMid = GetCenter(check);
			
			-- get the distance
			local distance = math.abs(mid.X - checkMid.X) + math.abs(mid.Y - checkMid.Y);
			
			-- this room is closer than the stored one
			if distance < closestDistance then
				closestDistance = distance;
				closest = check;
			end
		end
	end
	
	return closest;
end

function Equals(roomA, roomB)
	return roomA.X == roomB.X and roomA.Y == roomB.Y;
end

function GetCenter(room)
	local center = {X = 0, Y = 0};
	
	center.X = room.X + math.floor(room.Width / 2);
	center.Y = room.Y + math.floor(room.Height / 2);
	
	return center;
end