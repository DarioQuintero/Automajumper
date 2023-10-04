using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public static LevelCreator instance;

    public Vector3 respawnPosition;

    public void Awake()
    {
        instance = this;
    }

    public void createLevel(int levelNum)
    {
        // get string to process 
        string toProcess = Resources.Load<TextAsset>("Level" + levelNum).ToString();

        // get the size of the map
        int separatorIndex = toProcess.IndexOf('\n');
        string[] sizeList = toProcess.Substring(0, separatorIndex).Split();
        int[] size = { int.Parse(sizeList[0]), int.Parse(sizeList[1]) };

        string data_and_checkpoint = toProcess.Substring(separatorIndex + 1);
        
        int newline2 = data_and_checkpoint.IndexOf('\n');
        string[] checkpoints = data_and_checkpoint.Substring(3, newline2 - 3).Split(); // ignore the letters "CP"

        // block data is everything after the size data
        string data = data_and_checkpoint.Substring(newline2 + 1);

        // initialize for the loop
        int[,] map = new int[size[0], size[1]];

        // assign normal blocks
        int index = AssignBlocks(data, map, 1);

        // get the rest of the data
        data = data.Substring(index + 3); // +3 to get rid of the two new lines
        string[] dataLines = data.Split("\n");

        // if there is only one line left, then there is no red block
        if (dataLines.Length != 1)
        {
            // assign red blocks
            index = AssignBlocks(data, map, 2);
            data = data.Substring(index + 3); // +3 to get rid of the two new lines
            dataLines = data.Split("\n");
        }
        processFinishLine(dataLines, map, checkpoints);
    }

    void processFinishLine(string[] lastLine, int[,] map, string[] checkpoints)
    {
        // create the game objects for the level with finish line
        Map.instance.CreateLevel(map, lastLine[0].Split(), checkpoints);
    }
    

    int AssignBlocks(string data, int[,] map, int type)
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
                        map[curIndices[0], curIndices[1]] = type;
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

        return index;
    }
}
