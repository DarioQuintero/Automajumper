using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject cube;

    public static Map instance;

    public bool[,] map = new bool[10, 10];
    public GameObject[,] cubes = new GameObject[10, 10];

    void Awake()
    {
        instance = this;

        for (int i = 0; i < 10; i++)
        {
            cubes[i, 0] = Instantiate(cube, new Vector3(i, 0, 0), Quaternion.identity);
            map[i, 0] = true;
        }
    }

    private void Update()
    {
        if (Time.frameCount % 100 == 0)
        {
            bool[,] newMap = (bool[,]) map.Clone();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    newMap[i, j] = map[i, j];

                    if (map[i, j])
                    {
                        Destroy(cubes[i, j]);
                        newMap[i, j] = false;
                    }
                    else
                    {
                        if (HasALiveNeighbor(map, i, j))
                        {
                            cubes[i, j] = Instantiate(cube, new Vector3(i, j, 0), Quaternion.identity);
                            newMap[i, j] = true;
                        }
                    }
                }
            }

            map = newMap;
        }
    }

    bool HasALiveNeighbor(bool[,] map, int i, int j)
    {
        if (i - 1 >= 0 && map[i - 1, j]) return true;
        if (j - 1 >= 0 && map[i, j - 1]) return true;
        if (i + 1 < map.GetLength(0) && map[i + 1, j]) return true;
        if (j + 1 < map.GetLength(1) && map[i, j + 1]) return true;
        return false;
    }
}

