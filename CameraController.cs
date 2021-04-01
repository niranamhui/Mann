using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public GameObject cameraPos;
    private float speed ;
   
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        cameraPos = Player.transform.Find("camera constraint").gameObject;

    }

    private void FixedUpdate()
    {
        follow();
        
    }
    private void follow()
    {
       
        gameObject.transform.position = Vector3.Lerp(transform.position, Player.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.gameObject.transform.position);
    }
   

}
