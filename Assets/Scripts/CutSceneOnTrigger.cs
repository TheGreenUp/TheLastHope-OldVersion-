using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CutSceneOnTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject cutsceneCamera;

    bool isTrue = true;

    void OnTriggerEnter(Collider other)
    {
     if (other.tag == "Player")
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            cutsceneCamera.SetActive(true);
            player.SetActive(false);
            StartCoroutine(FinishCutscene());    
        }   
    }

    IEnumerator FinishCutscene()   
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
