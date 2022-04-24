using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Opening : MonoBehaviour
{
    public GameObject player;
    public GameObject fadeScreenIn;
    public GameObject textbox;

    void Start()
    {
        player.GetComponent<FirstPersonController>().enabled = false;//убираем контроль персонажа
        StartCoroutine(ScenePlayer());  

    }
    IEnumerator ScenePlayer()
    {
        yield return new WaitForSeconds(1f);//ждем секунду
        fadeScreenIn.SetActive(false);//убираем затухание
        textbox.GetComponent<Text>().text = "Ну че залупыш проснулся да...";//текст
        yield return new WaitForSeconds(2f);//ждем еще полсекунды
        textbox.GetComponent<Text>().text = "";//убираем текст
        player.GetComponent<FirstPersonController>().enabled = true;//даем контроль над персонажем
    }

}



