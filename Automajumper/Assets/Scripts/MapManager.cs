using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    private int[,] normalBlockMap;
    private int[,] killerBlockMap;
    private int[,] nextNormalBlockMap;
    private int[,] nextKillerBlockMap;
    private GameObject[,] normalBlocks;
    private GameObject[,] killerBlocks;

    private List<int[]> disappearNormalBlocks = new List<int[]>();
    private List<int[]> foresightNormalBlocks = new List<int[]>();
    private List<int[]> disappearKillerBlocks = new List<int[]>();
    private List<int[]> foresightKillerBlocks = new List<int[]>();
    private List<GameObject> blocksToDestroy = new List<GameObject>();

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

        nextNormalBlockMap = UpdateMap(normalBlockMap, disappearNormalBlocks, foresightNormalBlocks);
        nextKillerBlockMap = UpdateMap(killerBlockMap, disappearKillerBlocks, foresightKillerBlocks);
        UpdateMapObjects();
    }

    private void Update()
    {
        if (!paused)
            curTime -= Time.deltaTime;

        // for every transition time
        if (curTime <= 0)
        {
            curTime = secondsPerUpdate;

            // destroy all disappearing blocks from last generation
            DestroyBlocks();
            blocksToDestroy = new List<GameObject>();

            // change foresight blocks to blocks
            UpdateForesightBlocks(normalBlockMap, foresightNormalBlocks, normalBlocks, normalBlock);
            UpdateForesightBlocks(killerBlockMap, foresightKillerBlocks, killerBlocks, killerBlock);

            // change maps
            normalBlockMap = nextNormalBlockMap;
            killerBlockMap = nextKillerBlockMap;
            nextNormalBlockMap = UpdateMap(normalBlockMap, disappearNormalBlocks, foresightNormalBlocks);
            nextKillerBlockMap = UpdateMap(killerBlockMap, disappearKillerBlocks, foresightKillerBlocks);

            UpdateMapObjects();
        }
    }

    void UpdateMapObjects()
    {
        // disappear blocks
        CreateDisappearBlocks(normalBlockMap, disappearNormalBlocks, normalBlocks, disappearNormalBlock);
        CreateDisappearBlocks(killerBlockMap, disappearKillerBlocks, killerBlocks, disappearKillerBlock);

        // foresight blocks
        CreateForesightBlocks(normalBlockMap, foresightNormalBlocks, normalBlocks, foresightNormalBlock);
        CreateForesightBlocks(killerBlockMap, foresightKillerBlocks, killerBlocks, foresightKillerBlock);
    }

    int[,] UpdateMap(int[,] map, List<int[]> disappearBlocks, List<int[]> foresightBlocks)
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
                        foresightBlocks.Add(new int[] { i, j });
                        newMap[i, j] = 1;
                    }
                }
            }
        }

        return newMap;
    }

    void CreateDisappearBlocks(int[,] map, List<int[]> disappearBlocks,
        GameObject[,] blocks, GameObject disappearBlock)
    {
        // destroy blocks and create disappear blocks
        for (int i = disappearBlocks.Count - 1; i >= 0; i--)
        {
            int row = disappearBlocks[i][0];
            int col = disappearBlocks[i][1];
            Destroy(blocks[row, col]);
            blocks[row, col] = Instantiate(disappearBlock,
                GetWorldPosFromArrayIndices(row, col, map), Quaternion.identity, blocksParent.transform);
            blocksToDestroy.Add(blocks[row, col]);

            disappearBlocks.RemoveAt(i);
        }
    }

    void CreateForesightBlocks(int[,] map, List<int[]> foresightBlocks, GameObject[,] blocks, GameObject foresightBlock)
    {
        for (int i = foresightBlocks.Count - 1; i >= 0; i--)
        {
            int row = foresightBlocks[i][0];
            int col = foresightBlocks[i][1];
            blocks[row, col] = Instantiate(foresightBlock,
                GetWorldPosFromArrayIndices(row, col, map), Quaternion.identity, blocksParent.transform);
        }
    }

    void UpdateForesightBlocks(int[,] map, List<int[]> foresightBlocks, GameObject[,] blocks, GameObject block)
    {
        // destroy foresight blocks and create blocks
        for (int i = foresightBlocks.Count - 1; i >= 0; i--)
        {
            int row = foresightBlocks[i][0];
            int col = foresightBlocks[i][1];
            Destroy(blocks[row, col]);
            blocks[row, col] = Instantiate(block,
                GetWorldPosFromArrayIndices(row, col, map), Quaternion.identity, blocksParent.transform);

            foresightBlocks.RemoveAt(i);
        }
    }

    void DestroyBlocks()
    {
        foreach (GameObject block in blocksToDestroy)
        {
            Destroy(block);
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

