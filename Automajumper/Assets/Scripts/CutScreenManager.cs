using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScreenManager : MonoBehaviour
{
    [SerializeField] int levelNum;
    [SerializeField] GameObject skipInstruction;

    public void OnSkipForward()
    {
        SceneManager.LoadScene("Level " + (levelNum + 1));
    }

    public void OnShowSkipInstruction()
    {
        skipInstruction.SetActive(true);
        Invoke(nameof(HideSkipInstruction), 3f);
    }

    void HideSkipInstruction()
    {
        skipInstruction.SetActive(false);
    } 
}
