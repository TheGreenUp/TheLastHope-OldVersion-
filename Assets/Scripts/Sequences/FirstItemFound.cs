using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstItemFound : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject textBox;

     void OnTriggerEnter()
    {
        StartCoroutine(ScenePlayer());

    }

    IEnumerator ScenePlayer()
    {
        textBox.GetComponent<Text>().text = "���� ��� ��� �������� �����";
        yield return new WaitForSeconds(1.5f);
        textBox.GetComponent<Text>().text = "";
        Destroy(this);

    }
}
