using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLCManager : MonoBehaviour
{
    [SerializeField] RawImage levelTransitionImage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(FadeIn));
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
}
