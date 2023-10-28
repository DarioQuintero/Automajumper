using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTransition : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FinishLine"))
        {
            LevelManager.instance.levelTransition();
        }

        else if (collision.gameObject.CompareTag("Killer"))
        {
            transform.position = LevelCreator.instance.respawnPosition;
        }

        //else
        //{
        //    Vector3 collidePos = collision.gameObject.transform.position;
        //    transform.position = new Vector3(collidePos.x, collidePos.y + 1, collidePos.z); 
        //}
    }
}
