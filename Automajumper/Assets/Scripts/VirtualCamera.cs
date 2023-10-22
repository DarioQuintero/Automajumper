using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class VirtualCamera : MonoBehaviour
{
    // Start is called before the first frame update

    public CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;

    void Start()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        //if (scene.name == "Level 2" && vcam.transform.position.x > 80) {
        //    transposer.m_ScreenY = 0.2f;
        //    transposer.m_DeadZoneHeight = 0;
        //}
    }
}
