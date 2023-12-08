using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PLCManager : MonoBehaviour
{
    [SerializeField] RawImage levelTransitionImage;

    [SerializeField] List<Image> buttonImages;
    [SerializeField] List<TextMeshProUGUI> texts;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(FadeIn));
    }

    IEnumerator FadeIn()
    {
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1f / steps);
            foreach (Image buttonImage in buttonImages)
            {
                buttonImage.color = new Color(255, 255, 255, i / 100f);
            }

            foreach (TextMeshProUGUI text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, i / 100f);
            }
        }
    }
}
