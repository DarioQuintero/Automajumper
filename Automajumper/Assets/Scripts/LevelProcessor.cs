using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProcessor : MonoBehaviour
{
    private void Start()
    {
        int[] size = { 16, 20 };

        string toProcess = "10b2o$10b2o3$9bo8b2o$7bo3b4o3b2o$7bo3bo$2o5bo$2o8bo$8b2o4$7bo$6bobo$7bo!";

        int[,] map = new int[size[0], size[1]];
        int[] curIndices = { 0, 0 };

        string curNum = "";
        for (int i = 0; i < toProcess.Length; i++)
        {
            if (System.Char.IsDigit(toProcess[i]))
            {
                curNum += toProcess[i];
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
                if (toProcess[i] == 'b')
                {
                    curIndices[1] += count;
                }

                // blocks
                else if (toProcess[i] == 'o')
                {
                    while (count-- > 0)
                    {
                        map[curIndices[0], curIndices[1]] = 1;
                        curIndices[1]++;
                    }
                }

                // skip line
                else if (toProcess[i] == '$')
                {
                    curIndices[0] += count;
                    curIndices[1] = 0;
                }
            }
        }

        Map.instance.CreateLevel(map);
    }
}
