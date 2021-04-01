using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCon : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField]private driveType drive;

    internal enum gearBox
    {
        automatic, manual
    }
    [SerializeField] private gearBox gearChange;

    public int gold = 0;
    public Gamemanager manager;
    private carEffects CarEffects;
    private inputManager IM;
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];
    private GameObject wheelMeshes, wheelColliders;
    private Rigidbody rb;
    public bool test;

    public float handBrakeFrictionMultiplier = 2f;

    public float wheelsRPM;
    public float totalPower;
    public float KPM;
    public float smoothTime = 0.01f;
    public float engineRPM;
    public float maxRPM, minRPM;
    public float[] gearChangeSpeed;
    public float[] gears;
    public int gearNum = 0;
    [HideInInspector] public bool playPauseSmoke = false, hasFinished;
    public AnimationCurve enginePower;
    public bool reverse = false;

    public int motorTorque = 100;
    public float steeringMax = 4;
    public float addDownForceValue = 50;
    public float radius = 6;
    public float thrust = 1000f;
    public GameObject centerOfMass;
    public float brakPower , driftFactor, horizontal , vertical, lastValue;
    public float[] slip = new float[4];
    private bool flag = false;
    //[HideInInspector] public bool nitrusFlag = false;
    //[HideInInspector] public float nitrusValue;
    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    public GameObject Lights;
    public ParticleSystem[] nitrusSmoke;
    public float nitrusValue;
    public bool nitrusFlag = false;
    // Start is called before the first frame update

    // Car Shop
    //public int carPrice;
    // public string  carName;

    void Start()
    {
        getObjects();
        StartCoroutine(timedLoop());
    }

    // Update is called once per frame
    private void Update()
    {

        horizontal = IM.horizontal;
        vertical = IM.vertical;

        lastValue = engineRPM;
       // getObjects();
        animateWheels();
        steerVehicle();
        addDownForce();
        //getFriction();
        //shifter();
        calculateEnginePower();
        adjustTraction();
        if (gold == 3)
        {
            this.GetComponent<CarCon>().enabled = false;
            this.GetComponent<inputManager>().enabled = false;
            Debug.Log("Win");
        }

    }
    private void calculateEnginePower()
    {

        wheelRPM();

        if (vertical != 0)
        {
            rb.drag = 0.005f;
        }
        if (vertical == 0)
        {
            rb.drag = 0.1f;
        }
        totalPower = 3.6f * enginePower.Evaluate(engineRPM) * (vertical);

        float velocity = 0.0f;
        if (engineRPM >= maxRPM || flag)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 500, ref velocity, 0.05f);

            flag = (engineRPM >= maxRPM - 450) ? true : false;
            test = (lastValue > engineRPM) ? true : false;
        }
        else
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
            test = false;
        }
        if (engineRPM >= maxRPM + 1000) engineRPM = maxRPM + 1000; // clamp at max
        moveVehicle();
        shifter();
    }
    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;

        if (wheelsRPM < 0 && !reverse)
        {
            reverse = true;
            manager.changGear();
        }
        else if (wheelsRPM > 0 && reverse)
        {
            reverse = false;
            manager.changGear();
        }
    }
    private bool checkGears()
    {
        if (KPM >= gearChangeSpeed[gearNum]) return true;
        else return false;
    }
    private void shifter()
    {
         if (!isGrounded()) return;
         if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !reverse && checkGears() )
         {
             gearNum++;
             return;
         }
         if (engineRPM < minRPM && gearNum > 0)
         {
             gearNum--;
         }
        /*if (!isGrounded()) return;
        if (gearChange == gearBox.automatic)
        {
            if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !reverse)
            {
                gearNum++;
                manager.changGear();
            }
        }*/
       /* else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                gearNum++;
                manager.changGear();
            }
            if (engineRPM < minRPM && gearNum > 0)
            {
                gearNum--;
                manager.changGear();
            }


        }*/
    }
    private bool isGrounded()
    {
        if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
            return true;
        else
            return false;
    }
    private void moveVehicle()
    {
        brakeVehicle();
        /*
        if (drive == driveType.allWheelDrive)
         {
             for (int i = 0; i < wheels.Length; i++)
             {
                 wheels[i].motorTorque = totalPower / 4;
                 //wheels[i].brakeTorque = brakPower;
             }
         }
         else if (drive == driveType.rearWheelDrive)
         {
             for (int i = 0; i < wheels.Length; i++)
             {
                wheels[i].motorTorque = totalPower / 2;
            }
         }
         else
         {
             for (int i = 0; i < wheels.Length -2 ; i++)
             {
                wheels[i].motorTorque = ( totalPower / 2) ;
            }
         }*/
        if (drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 4);
            }
        }
        else if (drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 4);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 2);
            }
        }
        /*if (drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 4);
            }
        }*/
      

        KPM = rb.velocity.magnitude * 4f;

        if (IM.handbrake)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = brakPower;
        }
        else
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = 0;
        }
        if (IM.boosting)
        {
            var force = transform.forward * thrust;
            rb.AddForce(force, ForceMode.Acceleration);

        }
        else if (IM.boosting == false)
        {
            
            rb.AddForce(transform.forward / thrust);
        }
    }
    private void brakeVehicle()
    {
       
        if (vertical < 0)
        {
            brakPower = (KPM >= 10) ? 500 : 0;
        }
        else if (vertical == 0 && (KPM <= 10 || KPM >= -10))
        {
            brakPower = 10;
        }
        else
        {
            brakPower = 0;
        }


    }

    private void steerVehicle()
    {

        if (horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
        }

        else if (horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }

    }
    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    private void getObjects()
    {
        IM = GetComponent<inputManager>();
        rb = GetComponent<Rigidbody>();
        CarEffects = GetComponent<carEffects>();
        manager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<Gamemanager>();
        wheelColliders = GameObject.Find("WheelsColliders");
        wheelMeshes = GameObject.Find("Wheels");

        wheels[0] = wheelColliders.transform.Find("FrontDriver").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("FrontPassenger").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("RearDriver").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("RearPassenger").gameObject.GetComponent<WheelCollider>();

        wheelMesh[0] = wheelMeshes.transform.Find("FrontDriver").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("FrontPassenger").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("RearDriver").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("RearPassenger").gameObject;

        centerOfMass = GameObject.Find("mass");
        rb.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void addDownForce()
    {
        rb.AddForce(-transform.up * addDownForceValue * rb.velocity.magnitude);
    }

    private void getFriction()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.forwardSlip;
        }
    }
    //public float handBraFriction = 0;
    private void adjustTraction()
    {
        float driftSmothFactor = .7f * Time.deltaTime;

        if (IM.handbrake)
        {
            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier, ref velocity, driftSmothFactor);

            for (int i = 0; i < 4; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }
            rb.AddForce(transform.forward * (KPM / 400) * 10000);
        }
        //executed when handbrake is being held
        else
        {

            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((KPM * handBrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;

            }
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {

            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
            //smoke
            if (wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f || wheelHit.forwardSlip >= .3f || wheelHit.forwardSlip <= -0.3f)
                playPauseSmoke = true;
            else
                playPauseSmoke = false;


            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
        }


    }

    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPM / 20;

        }
    }

    public void activateNitrus()
    {
        
        if (!IM.boosting && nitrusValue <= 10)
        {
            nitrusValue += Time.deltaTime / 2;
            CarEffects.startNitrusEmitter();
        }
        else
        {
            nitrusValue -= (nitrusValue <= 0) ? 0 : Time.deltaTime;
        }

        if (IM.boosting)
        {
            if (nitrusValue > 0)
            {
                rb.AddForce(transform.forward * 5000);
            }
            else CarEffects.stopNitrusEmitter();
        }
        else CarEffects.stopNitrusEmitter();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Win")
        {
            gold = gold + 1;
            Debug.Log(gold);
        }

    }

}


