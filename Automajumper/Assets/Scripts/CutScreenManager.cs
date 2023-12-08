using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScreenManager : MonoBehaviour
{
    [SerializeField] int levelNum;

    public void OnSkipForward()
    {
        SceneManager.LoadScene("Level " + (levelNum + 1));
    }
}
