using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private Button _navigateBtn;

    private void OnEnable()
    {
        VisualElement root  = GetComponent<UIDocument>().rootVisualElement;
        _navigateBtn = root.Q<Button>("navigateBtn");

        _navigateBtn.clicked += () => SceneManager.LoadScene("Level 1");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
