using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class VirtualCameraManager : MonoBehaviour
{
    [SerializeField] int cameraIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LevelManager.instance.NextCamera(cameraIndex);
            Destroy(gameObject);
        }
    }
}
