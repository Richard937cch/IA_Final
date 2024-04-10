# ECS7016P - Final Unity Project: Haunted Forest
## Map Generator:<br> 
。Grid map with two types of tiles: dirt(brown) and tree(green).<br>
。Start with a 2D grid map and assigned values randomly: 0(dirt) or 1(tree).<br>
。Apply and iterate cellular automata rule to smooth the map.<br>
。cellular automata: check every grid and count how many alive(tree) neighbor(3*3) it has. For dirt(0) grid, if the amount of alive neighbor is equal to 3, the grid change to tree(1). For tree(1) grid, if the the amount of alive neighbor is smaller than 2 or bigger than 3, the grid change to dirt(0).
。Set value 1(tree) to the edge of the map as border.<br>
。Instantiate actual prefab to generate actual map.<br>
## Main agent - Adventurer(yellow):<br>
。Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.<br>
## Enemy agent - Forest Spirit(red):<br>
。Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.<br>
