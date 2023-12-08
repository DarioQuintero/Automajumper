using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject PLCScreen;
    [SerializeField] GameObject controlScreen;
    [SerializeField] GameObject levelSelectScreen;

    private void Awake()
    {
        BackToPLC();
    }

    public void Play()
    {
        SceneManager.LoadScene("Level " + 1);
    }

    public void LevelSelect()
    {
        levelSelectScreen.SetActive(true);
    }

    public void Control()
    {
        controlScreen.SetActive(true);
    }

    public void PlayLevel(int levelNum)
    {
        SceneManager.LoadScene("Level " + levelNum);
    }

    public void BackToPLC()
    {
        controlScreen.SetActive(false);
        levelSelectScreen.SetActive(false);
        PLCScreen.SetActive(true);
    }
}
