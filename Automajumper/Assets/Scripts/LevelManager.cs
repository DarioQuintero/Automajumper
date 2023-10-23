using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] List<CinemachineVirtualCamera> vcamList;
    private int cameraIndex = 0;
    [SerializeField] int levelNum;

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

    public void nextCamaera()
    {
        vcamList[cameraIndex].gameObject.SetActive(false);
        cameraIndex += 1;
    }
}
