using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutScreenManager : MonoBehaviour
{
    [SerializeField] int levelNum;
    [SerializeField] GameObject skipInstruction;
    [SerializeField] bool skipInstructionShown;
    [SerializeField] int displayCount = 0;

    public void OnSkipForward()
    {
        SceneManager.LoadScene("Level " + (levelNum + 1));
    }

    public void OnShowSkipInstruction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            skipInstruction.SetActive(true);
            StartCoroutine(nameof(HideSkipInstruction));
        }
    }

    IEnumerator HideSkipInstruction()
    {
        displayCount++;

        yield return new WaitForSeconds(3f);

        displayCount--;

        if (displayCount == 0)
            skipInstruction.SetActive(false);
    }
}
