﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Map : MonoBehaviour
{
    public static Map instance;

    [SerializeField] GameObject cube;
    [SerializeField] GameObject line;

    private GameObject cubesParent;
    private int[,] map;
    private GameObject[,] cubes;

    [SerializeField] float transitionTime;
    [SerializeField] float curTime;

    public bool paused;

    void Awake()
    {
        instance = this;
    }

    public void CreateLevel(int[,] mapToCreate, string[] finishLineCoord)
    {
        map = mapToCreate;

        // initialize cube array
        cubes = new GameObject[map.GetLength(0), map.GetLength(1)];

        // camera position and size
        ////Camera.main.transform.position = new Vector3(map.GetLength(1) / 2, map.GetLength(0) / 2, -10);
        //Camera.main.transform.position = new Vector3(17, 14, -10);
        ////Camera.main.orthographicSize = Mathf.Max(map.GetLength(1) / 2, map.GetLength(0) / 2);
        //Camera.main.orthographicSize = 12;

        // generate the cubes
        cubesParent = new GameObject("Cube");
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 1)
                {
                    cubes[i, j] = Instantiate(cube, GetWorldPosFromArrayIndices(i, j), Quaternion.identity, cubesParent.transform);
                }
            }
        }

        // mark the finishline cube
        cubes[int.Parse(finishLineCoord[0]), int.Parse(finishLineCoord[1])].tag = "FinishLine";

        // generate the lines of the grid
        GameObject gridParent = new GameObject("Grid");

        // horizontal lines
        float lineLength = map.GetLength(1);
        line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            Instantiate(line, new Vector3((lineLength - 1) / 2, i + 0.5f, 0), Quaternion.identity, gridParent.transform);
        }
        Instantiate(line, new Vector3((lineLength - 1) / 2, -0.5f, 0), Quaternion.identity, gridParent.transform);

        // vertical lines
        lineLength = map.GetLength(0);
        line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        for (int i = 0; i < map.GetLength(1); i++)
        {
            Instantiate(line, new Vector3(i + 0.5f, (lineLength - 1) / 2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);
        }
        Instantiate(line, new Vector3(-0.5f, (lineLength - 1) / 2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);

        // start the time
        curTime = transitionTime;
    }

    private void Update()
    {
        if (!paused)
            curTime -= Time.deltaTime;

        // for every transition time
        if (curTime < 0)
        {
            curTime = transitionTime;

            // update the map
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
                            cubes[i, j] = Instantiate(cube, GetWorldPosFromArrayIndices(i, j), Quaternion.identity, cubesParent.transform);
                            newMap[i, j] = 1;
                        }
                    }
                }
            }

            map = newMap;
        }
    }

    Vector3 GetWorldPosFromArrayIndices(int i, int j)
    {
        // transpose the map and reflect it across the middle line
        return new Vector3(j, map.GetLength(0) - i - 1, 0);
    }

    // count the alive neighbors of this cell in coordinate i, j in the map
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
        if (i + 1 < map.GetLength(0))
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

