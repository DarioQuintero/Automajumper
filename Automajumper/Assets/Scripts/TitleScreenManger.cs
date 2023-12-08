using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] GameObject levelCreator;
    [SerializeField] GameObject mapManager;

    [SerializeField] GameObject block;

    [SerializeField] float curTime;
    [SerializeField] bool started;

    [SerializeField] GameObject blocksParent;

    [SerializeField] bool[,] blockBoolMap;
    [SerializeField] GameObject whiteCover;
    [SerializeField] GameObject[,] blockObjectMap;

    private void Awake()
    {
        Camera.main.transform.position = new Vector3(257 / 2, 54 / 2, -10);
    }

    private void Start()
    {
        ParseLevel();
        StartCoroutine(nameof(FadeOut));
    }

    private void Update()
    {
        if (started)
        {
            curTime += Time.deltaTime;
        }


        // for every transition time
        if (curTime > 0.1f)
        {
            curTime = 0;

            blockBoolMap = UpdateMap(blockBoolMap);
        }
    }

    IEnumerator FadeOut()
    {
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(3f / steps);
            block.GetComponent<Renderer>().sharedMaterial.color = new Color(i / 100f, i / 100f, i / 100f);
        }

        block.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 0, 0);
        whiteCover.SetActive(true);
        SceneManager.LoadScene("PLC");
    }

    public void ParseLevel()
    {
        // get string to process 
        string toProcess = Resources.Load<TextAsset>("TitleScreen").ToString();
        // fix Window's problem
        toProcess = toProcess.Replace("\r\n", "\n");

        // get the spawn position
        int separatorIndex = toProcess.IndexOf('\n');
        string[] respawnPositionCoord = toProcess.Substring(0, separatorIndex).Split();

        // skip an empty line in between
        toProcess = toProcess.Substring(separatorIndex + 2);

        // get the size of the map
        separatorIndex = toProcess.IndexOf('\n');
        string[] sizeList = toProcess.Substring(0, separatorIndex).Split();

        // Format: Map Size: x y
        // Use indices of 3 and 2 to change to row column coordinates
        int[] size = { int.Parse(sizeList[3]), int.Parse(sizeList[2]) };

        // go to next line
        toProcess = toProcess.Substring(separatorIndex + 1);

        // get checkpoints coordinates from next line 
        int newline2 = toProcess.IndexOf('\n');

        // get block data after skipping an empty line
        toProcess = toProcess.Substring(newline2 + 1);

        // initialize for the loop
        blockBoolMap = new bool[size[0], size[1]];
 
        // assign normal blocks
        AssignBlocks(toProcess);

        // create the blocks, checkpoints, and finishline in the level
        CreateLevel();
    }

    int AssignBlocks(string data)
    {
        int index = 0;
        int[] curIndices = { 0, 0 };
        string curNum = "";
        while (data[index] != '!')
        {
            // build up the current number
            if (System.Char.IsDigit(data[index]))
            {
                curNum += data[index];
            }
            else
            {
                // get count
                int count = 0;
                if (curNum == "")
                    count = 1;
                else
                    count = int.Parse(curNum);
                curNum = "";

                // empty spaces
                if (data[index] == 'b')
                {
                    curIndices[1] += count;
                }

                // blocks
                else if (data[index] == 'o')
                {
                    while (count-- > 0)
                    {
                        blockBoolMap[curIndices[0], curIndices[1]] = true;
                        curIndices[1]++;
                    }
                }

                // skip line
                else if (data[index] == '$')
                {
                    curIndices[0] += count;
                    curIndices[1] = 0;
                }
            }

            index++;
        }

        // +1 so it syncs with indexing with "\n"
        return index + 1;
    }

    public void CreateLevel()
    {
        blocksParent = new("Block Parent");

        blockObjectMap = new GameObject[blockBoolMap.GetLength(0), blockBoolMap.GetLength(1)];

        // generate the normal blocks
        for (int i = 0; i < blockBoolMap.GetLength(0); i++)
        {
            for (int j = 0; j < blockBoolMap.GetLength(1); j++)
            {
                if (blockBoolMap[i, j])
                {
                     blockObjectMap[i, j] = Instantiate(block,
                         GetWorldPosFromArrayIndices(i, j, blockBoolMap), Quaternion.identity, blocksParent.transform);
                }
            }
        }
    }

    bool[,] UpdateMap(bool[,] map)
    {
        bool[,] newMap = new bool[map.GetLength(0), map.GetLength(1)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                newMap[i, j] = map[i, j];

                // rules for alive cell to die
                if (map[i, j])
                {
                    int numOfAliveNeighbors = CountAliveNeighbors(map, i, j);
                    if (numOfAliveNeighbors < 2 || numOfAliveNeighbors > 3)
                    {
                        Destroy(blockObjectMap[i, j]);
                        newMap[i, j] = false;
                    }
                }
                // rules for dead cell to be alive
                else
                {
                    if (CountAliveNeighbors(map, i, j) == 3)
                    {
                        blockObjectMap[i, j] = Instantiate(block,
                         GetWorldPosFromArrayIndices(i, j, blockBoolMap), Quaternion.identity, blocksParent.transform);
                        newMap[i, j] = true;
                    }
                }
            }
        }

        return newMap;
    }

    int CountAliveNeighbors(bool[,] map, int i, int j)
    {
        int ans = 0;

        // left and right neighbors
        if (j - 1 >= 0 && map[i, j - 1])
            ans++;
        if (j + 1 < map.GetLength(1) && map[i, j + 1])
            ans++;

        // top three neighbors
        if (i - 1 >= 0)
        {
            if (map[i - 1, j])
                ans++;
            if (j - 1 >= 0 && map[i - 1, j - 1])
                ans++;
            if (j + 1 < map.GetLength(1) && map[i - 1, j + 1])
                ans++;
        }

        // top three neighbors
        if (i + 1 < map.GetLength(0))
        {
            if (map[i + 1, j])
                ans++;
            if (j - 1 >= 0 && map[i + 1, j - 1])
                ans++;
            if (j + 1 < map.GetLength(1) && map[i + 1, j + 1])
                ans++;
        }

        return ans;
    }

    Vector3 GetWorldPosFromArrayIndices(int i, int j, bool[,] map)
    {
        // transpose the map and reflect it across the middle line
        return new Vector3(j, map.GetLength(0) - i - 1, 0);
    }
}
