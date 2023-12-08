using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<CinemachineVirtualCamera> vcamList;
    private int cameraIndex = 0;
    [SerializeField] int levelNum;

    [SerializeField] RawImage levelTransitionImage;

    [SerializeField] GameObject levelCreator;
    [SerializeField] GameObject mapManager;

    private void Awake()
    {
        instance = this;

        //Application.targetFrameRate = -1;
    }

    private void Start()
    {
        // instantiate 
        Instantiate(levelCreator);
        Instantiate(mapManager);

        MapManager.instance.ChangeForesightBlockColor(levelNum);

        // need to be in start to make sure Level Creator instance is assigned
        LevelCreator.instance.ParseLevel(levelNum);

        StartCoroutine(nameof(FadeIn));
    }


    public void LevelTransition()
    {
        StartCoroutine(nameof(WaitForNextLevel));
    }

    IEnumerator WaitForNextLevel()
    {
        if (levelNum == 1)
        {
            yield return new WaitForSeconds(6f);
        }
        else if (levelNum == 2)
        {
            yield return new WaitForSeconds(9f);
        }


        levelTransitionImage.gameObject.SetActive(true);
        StartCoroutine(nameof(FadeOut));
    }

    IEnumerator FadeOut()
    {
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1f / steps);
            levelTransitionImage.color = new Color(0, 0, 0, i / 100f);
        }
        NextLevel();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Cut Scene " + levelNum);
    }

    IEnumerator FadeIn()
    {
        levelTransitionImage.gameObject.SetActive(true);
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1f / steps);
            levelTransitionImage.color = new Color(0, 0, 0, 1 - i / 100f);
        }
        levelTransitionImage.gameObject.SetActive(false);
    }

    public void nextCamera()
    {
        vcamList[cameraIndex].gameObject.SetActive(false);
        cameraIndex += 1;
        vcamList[cameraIndex].gameObject.SetActive(true);
    }
}
