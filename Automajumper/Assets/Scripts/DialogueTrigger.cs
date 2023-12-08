using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialogueObject;
    public TextMeshProUGUI textMesh;

    void Awake() {
        GameObject canvas = GameObject.Find("Canvas");
        dialogueObject = canvas.transform.Find("Dialogue").gameObject;
        textMesh = dialogueObject.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        dialogueObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dialogue"))
        {
            // Display custom text
            string fullText = other.GetComponent<DialogueString>().dialogueString;
            Destroy(other.gameObject);

            string[] splitText = fullText.Split("$");
            // Show dialogue object
            dialogueObject.SetActive(true);
            StartCoroutine(DisplayLinesOverTime(splitText));


            // foreach (string text in splitText)
            // {
            //     textMesh.text = text;
            //     // Hide dialogue object after a delay (e.g., 3 seconds)
            // }
            
        }
    }

    IEnumerator DisplayLinesOverTime(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            textMesh.text = lines[i];
            yield return new WaitForSeconds(3f);

            // Check if this is the last line
            if (i == lines.Length - 1)
            {
                HideDialogue(); // Invoke a function to hide the dialogue after the last line
            }
        }
        
        // foreach (string line in lines)
        // {
        //     textMesh.text = line; // Set the TextMesh text to the current line
        //     yield return new WaitForSeconds(3f); // Wait for a few seconds
        // }
        // Invoke("HideDialogue", 3f);
    }

    private void HideDialogue()
    {
        // Hide dialogue object
        dialogueObject.SetActive(false);
    }
}

