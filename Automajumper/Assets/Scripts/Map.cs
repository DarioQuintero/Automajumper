using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Map : MonoBehaviour
{
    public bool paused;

    [SerializeField] GameObject cube;
    [SerializeField] GameObject line;

    public static Map instance;

    public int[,] map;
    public GameObject[,] cubes;

    [SerializeField] float transitionTime;
    [SerializeField] float curTime;

    void Awake()
    {
        instance = this;

        // initialize the map
        map = new int[30, 70];

        // create the patterns

        // blocks
        CreateBlock(map, 18, 0);
        CreateBlock(map, 17, 5);
        CreateBlock(map, 15, 9);

        // honey combs
        CreateHorizontalHoneyComb(map, 15, 17);
        CreateHorizontalHoneyComb(map, 12, 32);

        CreateVerticalHoneyComb(map, 20, 15);
        CreateVerticalHoneyComb(map, 15, 39);

        // blinker
        CreateHorizontalBlinker(map, 13, 22);
        CreateHorizontalBlinker(map, 8, 30);
        CreateHorizontalBlinker(map, 11, 50);
        CreateHorizontalBlinker(map, 17, 60);

        CreateVerticalBlinker(map, 11, 41);
        CreateVerticalBlinker(map, 17, 55);

        // transpose the map and reflect it across the middle line
        //int[,] newMap = new int[map.GetLength(1), map.GetLength(0)];

        //for (int i = 0; i < map.GetLength(0); i++)
        //{
        //    for (int j = 0; j < map.GetLength(1); j++)
        //    {
        //        newMap[j, map.GetLength(0) - i - 1] = map[i, j];
        //    }
        //}
        //map = newMap;

        // test
        //map = new int[,] {
        //    { 0, 0, 0, 0, 0, 0 },
        //    { 0, 1, 1, 1, 0, 0 },
        //    { 0, 0, 0, 0, 0, 0 },
        //    { 0, 0, 0, 0, 0, 0 },
        //};

        // initialize cube array
        cubes = new GameObject[map.GetLength(0), map.GetLength(1)];

        // camera position and size
        Camera.main.transform.position = new Vector3(map.GetLength(1) / 2, map.GetLength(0) / 2, -10);
        //Camera.main.orthographicSize = Mathf.Max(map.GetLength(1) / 2, map.GetLength(0) / 2);
        Camera.main.orthographicSize = 25;

        // generate the cubes
        GameObject cubesParent = new GameObject("Cube");
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

        // generate the lines of the grid
        GameObject gridParent = new GameObject("Grid");

        // horizontal lines
        float lineLength = map.GetLength(1);
        line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            Instantiate(line, new Vector3((lineLength-1)/2, i + 0.5f, 0), Quaternion.identity, gridParent.transform);
        }
        Instantiate(line, new Vector3((lineLength-1)/2, -0.5f, 0), Quaternion.identity, gridParent.transform);

        // vertical lines
        lineLength = map.GetLength(0);
        line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        for (int i = 0; i < map.GetLength(1); i++)
        {
            Instantiate(line, new Vector3(i + 0.5f, (lineLength-1)/2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);
        }
        Instantiate(line, new Vector3(-0.5f, (lineLength-1)/2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);

        // start the time
        curTime = transitionTime;
    }

    #region Patterns

    private void CreateBlock(int[,] map, int i, int j)
    {
        map[i, j] = 1;
        map[i + 1, j] = 1;
        map[i, j + 1] = 1;
        map[i + 1, j + 1] = 1;
    }

    private void CreateHorizontalHoneyComb(int[,] map, int i, int j)
    {
        map[i, j + 1] = 1;
        map[i, j + 2] = 1;

        map[i + 1, j] = 1;
        map[i + 1, j + 3] = 1;

        map[i + 2, j + 1] = 1;
        map[i + 2, j + 2] = 1;
    }

    private void CreateVerticalHoneyComb(int[,] map, int i, int j)
    {
        map[i, j + 1] = 1;

        map[i + 1, j] = 1;
        map[i + 1, j + 2] = 1;

        map[i + 2, j] = 1;
        map[i + 2, j + 2] = 1;

        map[i + 3, j + 1] = 1;
    }

    private void CreateHorizontalBlinker(int[,] map, int i, int j)
    {
        map[i, j] = 1;
        map[i, j + 1] = 1;
        map[i, j + 2] = 1;
    }

    private void CreateVerticalBlinker(int[,] map, int i, int j)
    {
        map[i, j] = 1;
        map[i + 1, j] = 1;
        map[i + 2, j] = 1;
    }

    #endregion

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
                            cubes[i, j] = Instantiate(cube, GetWorldPosFromArrayIndices(i, j), Quaternion.identity);
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

