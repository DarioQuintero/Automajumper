using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] GameObject line;

    public static Map instance;

    public int[,] map;
    public GameObject[,] cubes = new GameObject[10, 10];

    [SerializeField] float transitionTime = 1f;

    void Awake()
    {
        instance = this;

        // initialize the map
        map = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };

        // transpose the map
        int[,] newMap = new int[map.GetLength(1), map.GetLength(0)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                newMap[j, i] = map[i, j];
            }
        }
        map = newMap;

        // generate the cubes
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (newMap[i, j] == 1)
                {
                    cubes[i, j] = Instantiate(cube, new Vector3(i, j, 0), Quaternion.identity);
                }
            }
        }

        // generate the lines
        for (int i = 0; i < map.GetLength(0); i++)
        {
            Instantiate(line, new Vector3(i + 0.5f, 0, 0), Quaternion.Euler(0, 0, 90));
        }
        Instantiate(line, new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 0, 90));
   

        for (int j = 0; j < map.GetLength(1); j++)
        {
            Instantiate(line, new Vector3(0, j + 0.5f, 0), Quaternion.identity);
        }
        Instantiate(line, new Vector3(0, -0.5f, 0), Quaternion.identity);

        transitionTime = 1f;
    }

    private void Update()
    {
        transitionTime -= Time.deltaTime;

        if (transitionTime < 0)
        {
            transitionTime = 1f;

            int[,] newMap = new int[map.GetLength(0), map.GetLength(1)];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    newMap[i, j] = map[i, j];

                    // rules for alive cell
                    if (map[i, j] == 1)
                    {
                        int numOfAliveNeighbors = CountAliveNeighbors(map, i, j);
                        if (numOfAliveNeighbors < 2 || numOfAliveNeighbors > 3)
                        {
                            Destroy(cubes[i, j]);
                            newMap[i, j] = 0;
                        }
                    }
                    // rules for dead cell
                    else
                    {
                        if (CountAliveNeighbors(map, i, j) == 3)
                        {
                            cubes[i, j] = Instantiate(cube, new Vector3(i, j, 0), Quaternion.identity);
                            newMap[i, j] = 1;
                        }
                    }
                }
            }

            map = newMap;
        }
    }

    int CountAliveNeighbors(int[,] map, int i, int j)
    {
        int ans = 0;

        // left and right neighbors
        if (j - 1 >= 0 && map[i, j - 1] == 1)
            ans++;
        if (j + 1 < map.GetLength(1) && map[i, j + 1] == 1)
            ans++;

        // top three neighbors
        if (i - 1 >= 0) {
            if (map[i - 1, j] == 1)
                ans++;
            if (j - 1 >= 0 && map[i - 1, j - 1] == 1)
                ans++;
            if (j + 1 < map.GetLength(1) && map[i - 1, j + 1] == 1)
                ans++;
        }

        // top three neighbors
        if (i + 1 < map.GetLength(1))
        {
            if (map[i + 1, j] == 1)
                ans++;
            if (j - 1 >= 0 && map[i + 1, j - 1] == 1)
                ans++;
            if (j + 1 < map.GetLength(1) && map[i + 1, j + 1] == 1)
                ans++;
        }

        return ans;
    }
}

