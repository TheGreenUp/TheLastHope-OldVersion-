using UnityEngine;

public class DoorCellOpen : MonoBehaviour
{
    public float theDistance;//расстояние до объекта, берем из рейкастинга в 19 строчке

    public GameObject actionKey;//[ e ]
    public GameObject actionText;//"Открыть дверь"
    public GameObject door;//сама дверь, что открывается
    public GameObject extraCross;//Дополнительный ободок у прицела

    public AudioSource openSound;//звук открытия двери

    void Update()
    {
        theDistance = PlayerCasting.DistanceFromTarget;
    }
    void OnMouseOver()
    {
        if (theDistance < 1.2) //когда просто дистанция меньше (игрок смотрит на дверь)
        {
            actionKey.SetActive(true);
            actionText.SetActive(true);
            extraCross.SetActive(true);
        }
        if (Input.GetButtonDown("Action"))//когда нажали кнопку (е)
        {
            if (theDistance < 1.2 && this.GetComponent<BoxCollider>().enabled == true)//когда дверь закрыта
            {
                this.GetComponent<BoxCollider>().enabled = false;
                actionKey.SetActive(false);
                actionText.SetActive(false);
                door.GetComponent<Animation>().Play("FirstDoorOpenAnim");
                openSound.Play();
            }
        }
    }
     void OnMouseExit()//увели мышку с триггера
    {
        actionKey.SetActive(false);
        actionText.SetActive(false);
        extraCross.SetActive(false);
    }
}
