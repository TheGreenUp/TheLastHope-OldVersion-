using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTouchTheGround : MonoBehaviour
{
    public AudioSource ballSound;
    public AudioClip hitclip;
    bool hitGround;

    int touchCounter = 0;


    void PlayHitSound()
    {
        ballSound.Play();
        //AudioSource.PlayClipAtPoint(hitclip, transform.position);
        //ballSound.PlayOneShot(hitclip);
        //AudioSource.PlayClipAtPoint(hitclip, transform.position);
    }

    void OnCollisionEnter(Collision other)
    {
        if (touchCounter == 0) //when object appears we does not want to hear sound
        {
            touchCounter = 1;
            return;
        }
   
        ballSound.volume -= 0.2f;

        // we can add a condition here
        // if other.tag == "ground";
        if (touchCounter == 4) return;
        if (!hitGround)
        {
            PlayHitSound();

            hitGround = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        touchCounter++;
        hitGround = false;
    }
}
