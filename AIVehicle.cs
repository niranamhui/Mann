using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class AIVehicle : MonoBehaviour {

    // These variables allow the script to power the wheels of the car.
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

   // public GUIText mphDisplay;

    public Vector3 vehicleCenterOfMass;
    public float steerPower = 12.0f;

    // These variables are for the gears, the array is the list of ratios. The script
    // uses the defined gear ratios to determine how much torque to apply to the wheels.
    public float[] gearRatio; //4.31, 2.71, 1.88, 1.41, 1.13, .93
    private int currentGear = 0;

    // These variables are just for applying torque to the wheels and shifting gears.
    // using the defined Max and Min Engine RPM, the script can determine what gear the
    // car needs to be in.
    public float engineTorque = 600.0f;
    public float brakePower = 0f;
    public float maxEngineRPM = 3000.0f;
    public float minEngineRPM = 1000.0f;
    public float engineRPM = 0.0f;

    // waypoint is used to determine which waypoint in the array the car is aiming for.
    public Transform[] waypoints;
    public float wayPointDistance = 20f; //distance to next waypoint, if more than this amount will seek next waypoint
    private int currentWaypoint = 0;
    private Vector3 relativeWaypointPosition = new Vector3(0f, 0f, 0f);

    // input steer and input torque are the values substituted out for the player input. The 
    // "NavigateTowardsWaypoint" function determines values to use for these variables to move the car
    // in the desired direction.
    private float inputSteer = 0.0f;
    private float inputTorque = 0.0f;

    private Rigidbody rb;

    void Start()
    {
        //Alter the center of mass to make the car more stable. Lower the center of mass. I'ts less likely to flip this way.
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = vehicleCenterOfMass;
    }

    void FixedUpdate()
    {
        var mph = rb.velocity.magnitude * 2.237;

       // if (mphDisplay) //display our speed visually
          //  mphDisplay.text = mph.ToString("F0") + " : MPH"; // displays one digit after the dot

        // This is to limith the maximum speed of the car, adjusting the drag probably isn't the best way of doing it,
        // but it's easy, and it doesn't interfere with the physics processing.
       rb.drag = rb.velocity.magnitude / 250f;

        // Call the funtion to determine the desired input values for the car. This essentially steers and
        // applies gas to the engine.
        NavigateTowardsWaypoint();

        // Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
        engineRPM = (frontLeftWheel.rpm + frontRightWheel.rpm) / 2 * gearRatio[currentGear];
        ShiftGears();

        // finally, apply the values to the wheels.	The torque applied is divided by the current gear, and
        // multiplied by the calculated AI input variable.
        frontLeftWheel.motorTorque = engineTorque * 10f / gearRatio[currentGear] * inputTorque;
        frontRightWheel.motorTorque = engineTorque * 10f / gearRatio[currentGear] * inputTorque;

        rearLeftWheel.brakeTorque = brakePower / 2f;
        rearRightWheel.brakeTorque = brakePower / 2f;

        // the steer angle is an arbitrary value multiplied by the calculated AI input.
        frontLeftWheel.steerAngle = (steerPower) * inputSteer;
        frontRightWheel.steerAngle = (steerPower) * inputSteer;
    }

    void ShiftGears()
    {
        int appropriateGear = currentGear;

        // this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
        // the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
        if (engineRPM >= maxEngineRPM)
        {

            for (int i = 0; i < gearRatio.Length; i++)
            {
                if (frontLeftWheel.rpm * gearRatio[i] < maxEngineRPM)
                {
                    appropriateGear = i;
                    break;
                }
            }
            currentGear = appropriateGear;
        }

        if (engineRPM <= minEngineRPM)
        {
            appropriateGear = currentGear;

            for (var j = gearRatio.Length - 1; j >= 0; j--)
            {
                if (frontLeftWheel.rpm * gearRatio[j] > minEngineRPM)
                {
                    appropriateGear = j;
                    break;
                }
            }
            currentGear = appropriateGear;
        }
    }

    void NavigateTowardsWaypoint()
    {
        // now we just find the relative position of the waypoint from the car transform,
        // that way we can determine how far to the left and right the waypoint is.
        if (waypoints.Length > 0)
        {
            relativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x,
            transform.position.y, waypoints[currentWaypoint].position.z));
        }

        // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
        inputSteer = relativeWaypointPosition.x / relativeWaypointPosition.magnitude;

        // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
        if (Mathf.Abs(inputSteer) < 0.5f)
        {
            inputTorque = relativeWaypointPosition.z / relativeWaypointPosition.magnitude - Mathf.Abs(inputSteer);
        }
        else
        {
            inputTorque = 0.0f;
        }

        // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the next in the list.
        if (relativeWaypointPosition.magnitude < wayPointDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }
    }
}
