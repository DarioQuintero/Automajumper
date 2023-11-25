using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject LevelSelectPopup;

    private void Awake()
    {
        LevelSelectPopup.SetActive(false);
    }

    public void play()
    {
        LevelSelectPopup.SetActive(true);
    }
}
