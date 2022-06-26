using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWayToSpawn : MonoBehaviour
{
    public GameObject stair,spawnPoint;
    GameObject player;
    bool moveCharacter = false;
    bool moveSpawn = false;
    public float spawnSpeed = 1f;

    void Spawnla()
    {
        Instantiate(stair, spawnPoint.transform.position, spawnPoint.transform.rotation);
        player.transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, transform.position.z);
        Quaternion.FromToRotation(new Vector3(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z), new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
        player.transform.LookAt(new Vector3(spawnPoint.transform.position.x, player.transform.position.y, spawnPoint.transform.position.z));
        moveSpawn = true;
        
    }
    private void Update()
    {
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Transform>().gameObject.tag=="Player"  && !moveCharacter)
        {
            player = other.gameObject.GetComponentInParent<Character>().gameObject;
            if(player.GetComponent<Character>().isRunning)
            {
                
                TranslateCharacter();
                moveCharacter = true;
            }
            
            

        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    void TranslateCharacter()
    {
        if (moveSpawn)
        {
            Invoke("Spawnla", 0f);
        }

        else
        Invoke("Spawnla", spawnSpeed);
    }

}
