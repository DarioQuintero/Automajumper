using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProcessor : MonoBehaviour
{
    public static LevelProcessor instance;

    public void Awake()
    {
        instance = this;
    }

    public void processLevel(int levelNum)
    {
        // get string to process by slicing using Level [num] and Level [num + 1]
        string toProcess = Resources.Load<TextAsset>("LevelData").ToString();
        int startIndex = toProcess.IndexOf("Level " + levelNum);
        int endIndex = toProcess.IndexOf("Level " + (levelNum + 1));
        toProcess = toProcess.Substring(startIndex, endIndex - startIndex);
        Debug.Log(toProcess);

        // get the size of the map
        string[] sizeList = toProcess.Split("\n")[1].Split();
        int[] size = { int.Parse(sizeList[0]), int.Parse(sizeList[1]) };

        // block data is everything after the size data
        string blockData = toProcess.Substring(toProcess.IndexOf("!") + 1);

        // initialize for the loop
        int[,] map = new int[size[0], size[1]];
        int index = 0;
        int[] curIndices = { 0, 0 };
        string curNum = "";
        while (blockData[index] != '!')
        {
            // build up the current number
            if (System.Char.IsDigit(blockData[index]))
            {
                curNum += blockData[index];
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
                if (blockData[index] == 'b')
                {
                    curIndices[1] += count;
                }

                // blocks
                else if (blockData[index] == 'o')
                {
                    while (count-- > 0)
                    {
                        map[curIndices[0], curIndices[1]] = 1;
                        curIndices[1]++;
                    }
                }

                // skip line
                else if (blockData[index] == '$')
                {
                    curIndices[0] += count;
                    curIndices[1] = 0;
                }
            }

            index++;
        }

        Map.instance.CreateLevel(map);
    }
}
