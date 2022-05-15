using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [Header ("����� ������")]
    [SerializeField] public int sceneIndex;
    private void OnTriggerEnter(Collider myCollider)
    {
        if (myCollider.tag == "Player")
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
