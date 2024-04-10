## ECS7016P - Final Unity Project: Haunted Forest
Map Generator:
  Grid map with two types of tiles: dirt(brown) and tree(green).
  Start with a 2D grid map and assigned values randomly: 0(dirt) or 1(tree).
  Apply cellular automata to smooth the map.
  Set value 1(tree) to the edge of the map as border.
  Instantiate actual prefab to generate actual map.
Main agent - Adventurer(yellow):
  Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.
Enemy agent - Forest Spirit(red):
  Script is based on NPBehave and the "Enemybehaviour.cs" proved in the NPBehave asset.
