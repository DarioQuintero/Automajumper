using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject cube;

    public static Map instance;

    public int[,] map;
    public GameObject[,] cubes = new GameObject[10, 10];

    [SerializeField] float transitionTime = 1f;

    void Awake()
    {
        instance = this;

        map = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };

        int[,] newMap = new int[map.GetLength(1), map.GetLength(0)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                newMap[j, i] = map[i, j];
            }
        }

        map = newMap;

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
    }

    private void Update()
    {
        transitionTime -= Time.deltaTime;

        if (transitionTime < 0)
        {
            transitionTime = 0.3f;

            int[,] newMap = new int[map.GetLength(0), map.GetLength(1)];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    newMap[i, j] = map[i, j];

                    if (map[i, j] == 1 )
                    {
                        Destroy(cubes[i, j]);
                        newMap[i, j] = 0;
                    }
                    else
                    {
                        if (HasALiveNeighbor(map, i, j))
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

    bool HasALiveNeighbor(int[,] map, int i, int j)
    {
        if (i - 1 >= 0 && map[i - 1, j] == 1) return true;
        if (j - 1 >= 0 && map[i, j - 1] == 1) return true;
        if (i + 1 < map.GetLength(0) && map[i + 1, j] == 1) return true;
        if (j + 1 < map.GetLength(1) && map[i, j + 1] == 1) return true;
        return false;
    }
}

