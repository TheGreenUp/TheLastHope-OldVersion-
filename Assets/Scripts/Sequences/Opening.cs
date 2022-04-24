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
        player.GetComponent<FirstPersonController>().enabled = false;//������� �������� ���������
        StartCoroutine(ScenePlayer());  

    }
    IEnumerator ScenePlayer()
    {
        yield return new WaitForSeconds(1f);//���� �������
        fadeScreenIn.SetActive(false);//������� ���������
        textbox.GetComponent<Text>().text = "�� �� ������� ��������� ��...";//�����
        yield return new WaitForSeconds(2f);//���� ��� ����������
        textbox.GetComponent<Text>().text = "";//������� �����
        player.GetComponent<FirstPersonController>().enabled = true;//���� �������� ��� ����������
    }

}



