# ECS7016P - Final Unity Project: Haunted Forest
## Map Generator:<br> 
。Grid map with three types of tiles: dirt(brown), tree(green), and river(blue).<br>
。Start with a 2D grid map and assigned values randomly: 0(dirt) or 1(tree).<br>
。Apply and iterate cellular automata rule to smooth the map.<br>
。cellular automata: check every grid and count how many alive(tree) neighbor(3*3) it has. For dirt(0) grid, if the amount of alive neighbor is equal to 3, the grid change to tree(1). For tree(1) grid, if the the amount of alive neighbor is smaller than 2 or bigger than 3, the grid change to dirt(0).
。River using Perlin noise: generate perlin noise value for every grid. If Perlin value is smaller than 0.3, change that grid value to 2(river).
。Set value 1(tree) to the edge of the map as border.<br>
。Instantiate actual prefab to generate actual map.<br>
## Main agent - Adventurer(yellow):<br>
。Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.<br>
。An Adventurer is spawned randomly on the dirt or river grid of the map.<br>
。Initially, a random position is given as initial target position, and Adventurer will go toward it.<br>
。If Adventurer reach target position, a new target position will be given.<br>
。Adventurer will keep checking if enemies is nearby(7.5f).<br>
。[Enemy_is_nearby] Adventurer will try to flee from enemies, the closer the enemies are, the faster it flees.<br>
。It will decide direction to flee based on the positions of all enemies nearby.<br>
。[Stuck] If Adventurer is stuck in a position for a while, it will do multiple raycast 360 degree around itself and find the furthest tree as new target to move forward.<br>
。[KeepStuck] If Adventure is stuck in a position for multiple times, it will set another random position as new target to move forward.<br>
。Both Stuck and KeepStuck will trigger [attack] action, the shape of Adventurer will change and kill any enemy that contact it.<br>
。If Enemy isn't nearby, it will continue move forward to the target position, it will check if it Stuck and KeepStuck as well.<br>
。Adventurer wll slow down when it is in river<br>
## Enemy agent - Forest Spirit(red):<br>
。Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.<br>
。Enemies are spawned randomly on the dirt or river grids of the map.<br>
。IF Adventurer is nearby(7.5f) but not attacking, enemies become red and chase the Adventurer.<br>
。IF Adventurer is nearby(7.5f) and attacking, enemies stay red but run away from the Adventurer.<br>
。IF Adventurer is NOT nearby, enemies stay stationary and become invisible.<br>
。Enemy speed won't be affect when they are in river.<br>

## Video Link<br>


