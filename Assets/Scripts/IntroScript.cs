using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public int introtime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNextLevel());
    }

    // Update is called once per frame

    IEnumerator LoadNextLevel()
    {

        yield return new WaitForSeconds(introtime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
