﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField] GameObject normalBlock;
    [SerializeField] GameObject foresightNormalBlock;
    [SerializeField] GameObject disappearNormalBlock;
    [SerializeField] GameObject killerBlock;
    [SerializeField] GameObject foresightKillerBlock;
    [SerializeField] GameObject disappearKillerBlock;

    [SerializeField] GameObject line;

    [SerializeField] GameObject checkpoint;
    [SerializeField] GameObject finishLine;

    private GameObject blocksParent;

    private GameObject checkpointParent;

    private int[,] normalBlockMap;
    private int[,] killerBlockMap;
    private int[,] nextNormalBlockMap;
    private int[,] nextKillerBlockMap;
    private GameObject[,] normalBlocks;
    private GameObject[,] killerBlocks;

    private List<int[]> disappearNormalBlocks;
    private List<int[]> appearNormalBlocks;
    private List<int[]> disappearKillerBlocks;
    private List<int[]> appearKillerBlocks;
    private List<GameObject> allForeSightBlocks = new List<GameObject>();

    [SerializeField] float curTime;
    private bool paused;

    [SerializeField] float secondsPerUpdate;

    [SerializeField] AudioSource music;

    void Awake()
    {
        instance = this;

        secondsPerUpdate = Config.secondsPerUpdateNormal;

        music = GameObject.Find("Music").GetComponent<AudioSource>();
    }

    public void SpeedUp()
    {
        secondsPerUpdate = Config.secondsPerUpdateFaster;

        music.pitch = 3;

        // reset the cur time
        curTime = Mathf.Min(curTime, secondsPerUpdate);
    }

    public void SlowDown()
    {
        secondsPerUpdate = Config.secondsPerUpdateNormal;

        music.pitch = 1;
    }

    public void CreateLevel(int[,] normalBlockMapToCreate,
                            int[,] killerBlockMapToCreate,
                            string[] checkpointCoords,
                            string[] finishLineCoord)
    {
        normalBlockMap = normalBlockMapToCreate;
        killerBlockMap = killerBlockMapToCreate;
        nextNormalBlockMap = updateMap(normalBlockMap, disappearNormalBlocks, appearNormalBlocks);
        nextKillerBlockMap = updateMap(killerBlockMap, disappearKillerBlocks, appearKillerBlocks);

        // initialize blocks array and blocks parent
        normalBlocks = new GameObject[normalBlockMap.GetLength(0), normalBlockMap.GetLength(1)];
        killerBlocks = new GameObject[killerBlockMap.GetLength(0), killerBlockMap.GetLength(1)];
        blocksParent = new GameObject("Blocks");

        // generate the normal blocks
        for (int i = 0; i < normalBlockMap.GetLength(0); i++)
        {
            for (int j = 0; j < normalBlockMap.GetLength(1); j++)
            {
                if (normalBlockMap[i, j] == 1)
                {
                    normalBlocks[i, j] = Instantiate(normalBlock, GetWorldPosFromArrayIndices(i, j, normalBlockMap), Quaternion.identity, blocksParent.transform);
                }
            }
        }

        // generate the killer blocks
        for (int i = 0; i < killerBlockMap.GetLength(0); i++)
        {
            for (int j = 0; j < killerBlockMap.GetLength(1); j++)
            {
                if (killerBlockMap[i, j] == 1)
                {
                    killerBlocks[i, j] = Instantiate(killerBlock, GetWorldPosFromArrayIndices(i, j, killerBlockMap), Quaternion.identity, blocksParent.transform);
                }
            }
        }

        // create the finishe line
        Vector3 pos = new Vector3(float.Parse(finishLineCoord[0]), float.Parse(finishLineCoord[1]), 0);
        Instantiate(finishLine, pos, Quaternion.identity);

        // create the checkpoints
        Debug.Assert(checkpointCoords.Length % 2 == 0);
        GameObject checkpointParent = new GameObject("CPs");
        for (int i = 0; i < checkpointCoords.Length; i += 2) {
            //Vector3 pos = Vector3.back;
            pos = new Vector3(float.Parse(checkpointCoords[i]), float.Parse(checkpointCoords[i+1]), 0);
            //Debug.Log(pos.x.ToString());
            Instantiate(checkpoint, pos, Quaternion.identity, checkpointParent.transform);
        }

        // generate the lines of the grid
        // GameObject gridParent = new GameObject("Grid");

        // // horizontal lines
        // float lineLength = map.GetLength(1);
        // line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        // for (int i = 0; i < map.GetLength(0); i++)
        // {
        //     Instantiate(line, new Vector3((lineLength - 1) / 2, i + 0.5f, 0), Quaternion.identity, gridParent.transform);
        // }
        // Instantiate(line, new Vector3((lineLength - 1) / 2, -0.5f, 0), Quaternion.identity, gridParent.transform);

        // // vertical lines
        // lineLength = map.GetLength(0);
        // line.transform.localScale = new Vector3(lineLength, 0.1f, 1);
        // for (int i = 0; i < map.GetLength(1); i++)
        // {
        //     Instantiate(line, new Vector3(i + 0.5f, (lineLength - 1) / 2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);
        // }
        // Instantiate(line, new Vector3(-0.5f, (lineLength - 1) / 2, 0), Quaternion.Euler(0, 0, 90), gridParent.transform);

        // start the time
        curTime = secondsPerUpdate;
    }

    private void Update()
    {
        if (!paused)
            curTime -= Time.deltaTime;

        // for every transition time
        if (curTime <= 0)
        {
            curTime = secondsPerUpdate;

            // update the map 
            normalBlockMap = nextNormalBlockMap;
            killerBlockMap = nextKillerBlockMap;
            nextNormalBlockMap = updateMap(normalBlockMap, disappearNormalBlocks, appearNormalBlocks);
            nextKillerBlockMap = updateMap(killerBlockMap, disappearKillerBlocks, appearKillerBlocks);


            //handleBlocks(normalBlock, )


            // update foresight blocks
            destroyForesightBlocks();
            createForesightBlocks(normalBlockMap, foresightNormalBlock);
            createForesightBlocks(killerBlockMap, foresightKillerBlock);
        }
    }

    int[,] updateMap(int[,] map, List<int[]> disappearBlocks, List<int[]> appearBlocks)
    {
        int[,] newMap = new int[map.GetLength(0), map.GetLength(1)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                newMap[i, j] = map[i, j];

                // rules for alive cell
                if (map[i, j] != 0)
                {
                    int numOfAliveNeighbors = CountAliveNeighbors(map, i, j);
                    if (numOfAliveNeighbors < 2 || numOfAliveNeighbors > 3)
                    {
                        disappearBlocks.Add(new int[] {i, j});
                        newMap[i, j] = 0;
                    }
                }
                // rules for dead cell
                else
                {
                    if (CountAliveNeighbors(map, i, j) == 3)
                    {
                        appearBlocks.Add(new int[] { i, j });
                        newMap[i, j] = 1;
                    }
                }
            }
        }

        return newMap;
    }

    void handleBlocks(int[,] map, List<int[]> disappearBlocks, List<int[]> appearBlocks, GameObject[,] blocks, GameObject block, GameObject disappearBlock)
    {
        foreach (int[] coord in disappearBlocks)
        {
            Destroy(blocks[coord[0], coord[1]]);
            blocks[coord[0], coord[1]] = Instantiate(disappearBlock,
                GetWorldPosFromArrayIndices(coord[0], coord[1], map), Quaternion.identity, blocksParent.transform);
        }
        
        Instantiate(block, GetWorldPosFromArrayIndices(i, j, map), Quaternion.identity, blocksParent.transform);
    }

    void destroyForesightBlocks()
    {
        for (int i = allForeSightBlocks.Count - 1; i >= 0; i--)
        {
            Destroy(allForeSightBlocks[i]);
            allForeSightBlocks.RemoveAt(i);
        }
    }

    void createForesightBlocks(int[,] map, GameObject foresightBlock)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                // rules for alive cell
                if (map[i, j] != 0)
                {
                    int numOfAliveNeighbors = CountAliveNeighbors(map, i, j);
                    if (numOfAliveNeighbors >= 2 && numOfAliveNeighbors <= 3)
                    {
                        allForeSightBlocks.Add(Instantiate(foresightBlock,
                        GetWorldPosFromArrayIndices(i, j, map), Quaternion.identity, blocksParent.transform));
                    }
                }
                // rules for dead cell
                else
                {
                    if (CountAliveNeighbors(map, i, j) == 3)
                    {
                        allForeSightBlocks.Add(Instantiate(foresightBlock,
                            GetWorldPosFromArrayIndices(i, j, map), Quaternion.identity, blocksParent.transform));
                    }
                }
            }
        }
    }

    Vector3 GetWorldPosFromArrayIndices(int i, int j, int[,] map)
    {
        // transpose the map and reflect it across the middle line
        return new Vector3(j, map.GetLength(0) - i - 1, 0);
    }

    // count the alive neighbors of this cell in coordinate i, j in the map
    int CountAliveNeighbors(int[,] map, int i, int j)
    {
        int ans = 0;

        // left and right neighbors
        if (j - 1 >= 0 && map[i, j - 1] != 0)
            ans++;
        if (j + 1 < map.GetLength(1) && map[i, j + 1] != 0)
            ans++;

        // top three neighbors
        if (i - 1 >= 0)
        {
            if (map[i - 1, j] != 0)
                ans++;
            if (j - 1 >= 0 && map[i - 1, j - 1] != 0)
                ans++;
            if (j + 1 < map.GetLength(1) && map[i - 1, j + 1] != 0)
                ans++;
        }

        // top three neighbors
        if (i + 1 < map.GetLength(0))
        {
            if (map[i + 1, j] != 0)
                ans++;
            if (j - 1 >= 0 && map[i + 1, j - 1] != 0)
                ans++;
            if (j + 1 < map.GetLength(1) && map[i + 1, j + 1] != 0)
                ans++;
        }

        return ans;
    }
}

