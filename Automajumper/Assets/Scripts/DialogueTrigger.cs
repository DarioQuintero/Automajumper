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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dialogue"))
        {
            // Display custom text
            textMesh.text = other.GetComponent<DialogueString>().dialogueString;
            Destroy(other.gameObject);
            // Show dialogue object
            dialogueObject.SetActive(true);
            
            // Hide dialogue object after a delay (e.g., 3 seconds)
            Invoke("HideDialogue", 3f);
        }
    }

    private void HideDialogue()
    {
        // Hide dialogue object
        dialogueObject.SetActive(false);
    }
}

