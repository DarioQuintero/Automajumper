using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransition : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FinishLine"))
        {
            LevelManager.instance.nextLevel();
        }

        else if (collision.gameObject.CompareTag("Bound"))
        {
            transform.position = LevelCreator.instance.respawnPosition;
        }
    }
}
