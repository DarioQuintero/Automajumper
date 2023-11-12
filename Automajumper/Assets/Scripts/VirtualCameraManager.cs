using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class VirtualCameraManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CameraChange"))
        {
            LevelManager.instance.nextCamaera();
            Destroy(other.gameObject);
        }
    }
}
