using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject player;
    public GameObject stair;
    public float timer;
    public bool canSpawn=true;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position= new Vector3(player.transform.position.x,player.transform.position.y, player.transform.position.z);
        transform.rotation= player.transform.rotation;
        if(player.GetComponentInParent<Character>().isRunning && !canSpawn)
        {
            canSpawn = true;
            spawnStair();
        }
        //if(player.isStarted)
        //{
        //    gameObject.transform.RotateAround(player.rotatingTarget.transform.position, Vector3.up, player.rotateSpeed * Time.deltaTime);
        //    gameObject.transform.Translate(player.charactercircleHeight, player.characterCircleRadius, player.charactercircleHeight);
        //}
    }
    void spawnStair()
    {
        if(canSpawn)
        {
            Instantiate(stair, transform.position, transform.rotation);
            Invoke("continueSpawn", timer);
        }
    }
    void continueSpawn()
    {
        if (canSpawn)
        {
            Debug.Log("ContinueSpawn Çalýþtý");
            canSpawn = false;
        }
    }
}
