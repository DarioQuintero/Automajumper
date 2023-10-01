using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int levelNum;

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LevelCreator.instance.createLevel(levelNum);
    }

    public void nextLevel()
    {
        SceneManager.LoadScene("Level " + (levelNum + 1));
    }
}
