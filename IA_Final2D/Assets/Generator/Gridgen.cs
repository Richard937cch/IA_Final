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

    public GameObject river;

    public GameObject Adventurer;

    public GameObject Enemy;

    private Vector3 spawnpoint;

    private string mapcheck;

    public int enemyAmount = 5;

    public int smoother = 8;

    public float riverThreshold = 3.0f;

    public float riverNoiseScale = 3.0f;
    

    void Start()
    {
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

        // Apply cellular automata rules
        for (int i = 0; i < smoother; i++) // Repeat for a few iterations for smoother results 
        {
            int[,] newGrid = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int aliveNeighbors = CountAliveNeighbors(x, y);
                
                    if (grid[x, y] == 1) //tree
                    {
                        if (aliveNeighbors < 2 || aliveNeighbors > 3)
                            newGrid[x, y] = 0; // Cell change to dirt
                        else
                            newGrid[x, y] = 1; 
                    }
                    else //dirt
                    {
                        if (aliveNeighbors == 3)
                            newGrid[x, y] = 1; // Cell change to tree
                        else
                            newGrid[x, y] = 0; 
                    }
                }
            }

            // Use Perlin noise to generate a river
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float perlinValue = Mathf.PerlinNoise(x * riverNoiseScale, y * riverNoiseScale);

                    if (perlinValue < riverThreshold)
                    {
                        newGrid[x, y] = 2;
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
                else if (grid[x, y] == 2) //river => 2
                {
                    GameObject newRiver = Instantiate(river, new Vector3(x, y, 0.1f), rotation);
                    newRiver.transform.parent = transform;
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
                    count += grid[neighborX, neighborY]; //+1 if grid is 1 (tree)
                }
            }
        }
        count -= grid[x, y]; // Exclude the cell itself
        return count;
    }


    void borderGen() // generate border for map
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

        // Find all available positions in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != 1) // Not tree
                {
                    // Add available position to the list
                    dirtPositions.Add(new Vector3(x, y, 0.0f));
                }
            }
        }

        // Select a random available position
        if (dirtPositions.Count > 0)
        {
            Vector3 randomPosition = dirtPositions[Random.Range(0, dirtPositions.Count)];

            // Instantiate the adventurer at the selected position
            spawnpoint = randomPosition;
            GameObject adventurer = Instantiate(Adventurer, randomPosition, Quaternion.identity);
        }
        else
        {
            Debug.Log("No dirt grids available to spawn adventurer.");
            spawnpoint = new Vector3(width/2, height/2, 0.0f);
            GameObject adventurer = Instantiate(Adventurer, spawnpoint, Quaternion.identity);
        }
    }

    void spawnEnemy(int enemyCount)
    {
        // List to store available positions to spawn
        List<Vector3> dirtPositions = new List<Vector3>();

        // Find all available positions in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != 1) // Not tree
                {
                    // Add available position to the list (except spawnpoint)
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
            // Select a random available position
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
