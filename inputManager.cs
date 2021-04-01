using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour
{
   

    public float vertical;
    public float horizontal;
    public bool handbrake;
    public bool boosting;
   
    private CarCon RR;
    private carEffects Effects;
    private AIVehicle A;

    private void Start()
    {

        RR = GetComponent<CarCon>();
        Effects = GetComponent<carEffects>();
        A = GetComponent<AIVehicle>();
        //print(gameObject.name + "offset distance " + distanceOffset + "steer force = " + sterrForce + "acc " + acceleration);
    }
    private void Awake()
    {
       
    }
    private void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        //handbrake = (Input.GetKey(KeyCode.Space) != 0) ? true : false;
        
        if (Input.GetKey(KeyCode.Space)) 
        {
            boosting = true;
            Effects.startNitrusEmitter();
            //RR.activateNitrus();
            Debug.Log("Yes");
        }
        else
        {
            Effects.stopNitrusEmitter();
            boosting = false;
        }
    }

   
    private void keyboard()
    {
       

    }

    
   
}
