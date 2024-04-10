using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridgen : MonoBehaviour
{
    public int width;
    public int height;
    public float fillProbability = 0.5f;

    private int[,] grid;

    public GameObject tree;

    public GameObject dirt;

    public GameObject Adventurer;

    public GameObject Enemy;

    private Vector3 spawnpoint;

    private string mapcheck;

    public int enemyAmount = 5;

    
    

    void Start()
    {
        //GameObject adventurer = Instantiate(Adventurer, new Vector3(width/2, 0.5f, height/2), Quaternion.identity);
        GenerateGrid();
        borderGen();
        spawnAdventurer();
        spawnEnemy(enemyAmount);
        

    }

    void GenerateGrid()
    {
        // Initialize the grid randomly (0 or 1)
        grid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = (Random.value < fillProbability) ? 1 : 0;
            }
        }

        //widen path
        //widenPath();
        /////////////////////////////////////////////////////

        // Apply cellular automata rules
        for (int i = 0; i < 8; i++) // Repeat for a few iterations for smoother results (5)
        {
            int[,] newGrid = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int aliveNeighbors = CountAliveNeighbors(x, y);
                
                    if (grid[x, y] == 1)
                    {
                        if (aliveNeighbors < 2 || aliveNeighbors > 3)
                            newGrid[x, y] = 0; // Cell dies
                        else
                            newGrid[x, y] = 1; // Cell survives
                    }
                    else
                    {
                        if (aliveNeighbors == 3)
                            newGrid[x, y] = 1; // Cell becomes alive
                        else
                            newGrid[x, y] = 0; // Cell remains dead
                    }
                }
            }
            // Update the grid with the new values
            grid = newGrid;
        }

        // Generate map based on the final grid state
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1) //tree => 1
                {
                    GameObject newTree = Instantiate(tree, new Vector3(x, y, 0.1f), rotation);
                    newTree.transform.parent = transform;
                }
                else                 //dirt => 0
                {
                    GameObject newDirt = Instantiate(dirt, new Vector3(x, y, 0.1f), rotation);
                    newDirt.transform.parent = transform;
                }
            }
        }
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborX = x + i;
                int neighborY = y + j;
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    count += grid[neighborX, neighborY];
                }
            }
        }
        count -= grid[x, y]; // Exclude the cell itself
        return count;
    }

    void widenPath()
    {
        int[,] newGrid1 = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == 1)
                    {
                    for (int i = -2; i <= 1; i++)
                    {
                        
                        for (int j = -1; j <= 1; j++)
                        {   
                            int neighborX = x + i;
                            int neighborY = y + j;
                            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                            {
                                if (newGrid1[neighborX, neighborY] == 1)
                                {
                                    newGrid1[x,y] = 0;
                                }
                            }            
                        }
                        
                    }
                    
                    }
                    
                }
            }

            // Update the grid with the new values
            grid = newGrid1;
    }

    void borderGen()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        for (int i = -1; i <= width ; i++)
        {
            GameObject newTree = Instantiate(tree, new Vector3(i, -1, 0.1f), rotation);
            newTree.transform.parent = transform;
            GameObject newTree1 = Instantiate(tree, new Vector3(i, height, 0.1f), rotation);
            newTree1.transform.parent = transform;
        }
        for (int i = 0; i < height ; i++)
        {
            GameObject newTree = Instantiate(tree, new Vector3(-1, i, 0.1f), rotation);
            newTree.transform.parent = transform;
            GameObject newTree1 = Instantiate(tree, new Vector3(width, i, 0.1f), rotation);
            newTree1.transform.parent = transform;
        }
    }

    void spawnAdventurer()
    {
        // List to store available positions to spawn
        List<Vector3> dirtPositions = new List<Vector3>();

        // Find all dirt positions in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0) // Dirt grid
                {
                    // Add dirt position to the list
                    dirtPositions.Add(new Vector3(x, y, 0.0f));
                }
            }
        }

        // Select a random dirt position
        if (dirtPositions.Count > 0)
        {
            Vector3 randomPosition = dirtPositions[Random.Range(0, dirtPositions.Count)];

            // Instantiate the adventurer at the selected position
            spawnpoint = randomPosition;
            GameObject adventurer = Instantiate(Adventurer, randomPosition, Quaternion.identity);
            //Adventurer.transform.position = spawnpoint;
        }
        else
        {
            Debug.Log("No dirt grids available to spawn adventurer.");
            spawnpoint = new Vector3(width/2, height/2, 0.0f);
            GameObject adventurer = Instantiate(Adventurer, spawnpoint, Quaternion.identity);
            //Adventurer.transform.position = spawnpoint;
        }
    }

    void spawnEnemy(int enemyCount)
    {
        // List to store available positions to spawn
        List<Vector3> dirtPositions = new List<Vector3>();

        // Find all dirt positions in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0) // Dirt grid
                {
                    // Add dirt position to the list (except spawnpoint)
                    if (x != spawnpoint.x && y != spawnpoint.y)
                    {
                        dirtPositions.Add(new Vector3(x, y, 0.0f));
                    }
                    
                }
            }
        }

        // Spawn enemies based on the enemyCount
        for (int i = 0; i < enemyCount; i++)
        {
            // Select a random dirt position
            if (dirtPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, dirtPositions.Count);
                Vector3 randomPosition = dirtPositions[randomIndex];

                // Remove the selected position from the list
                dirtPositions.RemoveAt(randomIndex);

                // Instantiate the enemy at the selected position
                GameObject enemy = Instantiate(Enemy, randomPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No dirt grids available to spawn enemy.");
                return; // Stop spawning if no dirt grids available
            }
        }
    }


}
