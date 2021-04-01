using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeZone : MonoBehaviour {

    public GameObject AICar;
    public float brakingPower = 1f;
    public float enginePower = 20f;
    private float startTorque = 0f;

    void Start()
    {
        //This is where the script finds the AI car and plugs it into the variable of this script.
        AICar = GameObject.FindGameObjectWithTag("AI");

        //grab reference to user-set engine torque for default to reset to later
        startTorque = AICar.GetComponent<AIVehicle>().engineTorque; 
    }

    // Hit the brakes when the AI enters the trigger
    void OnTriggerEnter(Collider other)
    {
        AICar.GetComponent<AIVehicle> ().brakePower = (brakingPower);
        AICar.GetComponent<AIVehicle> ().engineTorque = (enginePower);
    }

    void OnTriggerExit(Collider other)
    {
        AICar.GetComponent<AIVehicle> ().brakePower = 0f; 
        AICar.GetComponent<AIVehicle>().engineTorque = startTorque;
    }
}
