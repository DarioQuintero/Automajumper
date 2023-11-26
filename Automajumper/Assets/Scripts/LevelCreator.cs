using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public static LevelCreator instance;

    public Vector3 respawnPosition;

    [SerializeField] GameObject character;

    //[SerializeField] CinemachineVirtualCamera vCamera;

    public void Awake()
    {
        instance = this;
    }

    public void ParseLevel(int levelNum)
    {
        // get string to process 
        string toProcess = Resources.Load<TextAsset>("Level" + levelNum).ToString();
        // fix Window's problem
        toProcess = toProcess.Replace("\r\n", "\n");

        // get the spawn position
        int separatorIndex = toProcess.IndexOf('\n');
        string[] respawnPositionCoord = toProcess.Substring(0, separatorIndex).Split();

        // Format: Spawn Position: x y
        // so use 2 and 3 for indices
        respawnPosition = new Vector3(float.Parse(respawnPositionCoord[2]), float.Parse(respawnPositionCoord[3]), 0);

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
        string[] checkpoints = toProcess.Substring("Checkpoints: ".Length,
                                        newline2 - "Checkpoints: ".Length).Split(); // ignore the prefix "Checkpoints: "

        // get block data after skipping an empty line
        toProcess = toProcess.Substring(newline2 + 2);

        // initialize for the loop
        int[,] blockMap = new int[size[0], size[1]];
        int[,] killerBlockMap = new int[0, 0];

        // assign normal blocks
        int index = AssignBlocks(toProcess, blockMap);

        // get the rest of the data by skipping an empty line
        toProcess = toProcess.Substring(index + 2);
        string[] dataLines = toProcess.Split("\n");

        // if there is only one line left, then there is no red block
        if (dataLines.Length != 1)
        {
            killerBlockMap = new int[size[0], size[1]];

            // assign red blocks
            index = AssignBlocks(toProcess, killerBlockMap);
            // get the rest of the data by skipping an empty line
            toProcess = toProcess.Substring(index + 2);
        }

        // finishline coordinates
        string[] finishLineCoord = toProcess.Substring("Finishline: ".Length).Split();

        // create scene objects
        CreateSceneObjects();

        // create the blocks, checkpoints, and finishline in the level
        MapManager.instance.CreateLevel(blockMap, killerBlockMap, checkpoints, finishLineCoord);
    }

    void CreateSceneObjects()
    {
        // instantiate the character at respawn position
        GameObject curCharacter = Instantiate(character, respawnPosition, character.transform.rotation);

        // create camera and set it to follow player
        //CinemachineVirtualCamera curCamera = Instantiate(vCamera);

        for (int i = 0; i < 3; i++) {
            LevelManager.instance.vcamList[i].Follow = curCharacter.transform;
        }
    }

    int AssignBlocks(string data, int[,] map)
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
                        map[curIndices[0], curIndices[1]] = 1;
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
}
