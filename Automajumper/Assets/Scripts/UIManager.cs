using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject LevelSelectPopup;

    private void Awake()
    {
        LevelSelectPopup.SetActive(false);
    }

    public void Play()
    {
        LevelSelectPopup.SetActive(true);
    }

    public void PlayLevel(int levelNum)
    {
        SceneManager.LoadScene("Level " + levelNum);
    }

    public void Close()
    {
        LevelSelectPopup.SetActive(false);
    }
}
