using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAIScriipt : MonoBehaviour
{
    public CharacterController EnemyController;
    public BoxCollider Enemy;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 100.0f;
    public float car = 1f;

    private float speed = 10.0f;

    public GameObject target1, target2, target3, target4, target5, target6;
    // Start is called before the first frame update
    void Start()
    {
        EnemyController = GetComponent<CharacterController>();
        Enemy = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyController.isGrounded == true)
        {
            moveDirection = transform.TransformDirection(Vector3.forward);
        }

        if( Enemy.gameObject.tag == "AI")
        {
            moveDirection = transform.TransformDirection(Vector3.forward);
        }

        moveDirection.y -= gravity * Time.time;
        EnemyController.Move(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
      if(other.gameObject.tag == "P1")
        {
            transform.LookAt(target2.transform.position);
        }
        if (other.gameObject.tag == "P2")
        {
            transform.LookAt(target3.transform.position);
        }
        if (other.gameObject.tag == "P3")
        {
            transform.LookAt(target4.transform.position);
        }
        if (other.gameObject.tag == "P4")
        {
            transform.LookAt(target5.transform.position);
        }
        if (other.gameObject.tag == "P5")
        {
            transform.LookAt(target6.transform.position);
        }
    }
}
