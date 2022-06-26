using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public float height;
    void LateUpdate()
    {
        if(!player.GetComponent<Character>().gameOver)
        transform.position = Vector3.Lerp(transform.position, (new Vector3(transform.position.x, player.transform.position.y + height, transform.position.z)), Time.deltaTime * 5f);
    }
    public void PlaySound()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
