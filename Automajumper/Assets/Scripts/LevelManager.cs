using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] List<CinemachineVirtualCamera> vcamList;
    private int cameraIndex = -1;
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

        // need to be in start to make sure Level Creator instance is assigned
        LevelCreator.instance.ParseLevel(levelNum);
        StartCoroutine(nameof(fadeIn));
    }

    public void nextLevel()
    {
        SceneManager.LoadScene("Level " + (levelNum + 1));
    }

    public void nextCamera()
    {
        if (cameraIndex != -1)
            vcamList[cameraIndex].gameObject.SetActive(false);
        cameraIndex += 1;
        vcamList[cameraIndex].gameObject.SetActive(true);
    }

    public void levelTransition()
    {
        levelTransitionImage.gameObject.SetActive(true);
        StartCoroutine(nameof(fadeOut));
    }

    IEnumerator fadeOut()
    {
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1f / steps);
            levelTransitionImage.color = new Color(0, 0, 0, i / 100f);
        }
        nextLevel();
    }

    IEnumerator fadeIn()
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
}
