using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public vehicleList list;
    public CarCon RR;
    public Text kph;
    public Text gearNum;
    public GameObject startPosition;
    public GameObject neeedle;
    private float starPosiziton = 200f, endPosition = -200;
    private float desiredPosition;
    public Text win;
    public Text start;
    private bool End;
    public Text around;



    public Slider nitrusSlider;

    private void Start()
    {
        End = false;
    }
    private void Awake()
    {
        Instantiate(list.vehicles[PlayerPrefs.GetInt("pointer")], startPosition.transform.position, startPosition.transform.rotation);
        RR = GameObject.FindGameObjectWithTag("Player").GetComponent<CarCon>();

       
    }
    private void Update()
    {
        if (RR.gold == 2)
        {
            around.text = "1/2";
        }
        if (RR.gold == 3)
        {
            around.text = "2/2";
            win.text = "Win";
            start.text = "Press P for start";
            End = true;
        }
       
        if (End = true && Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Start");
            Debug.Log("GO");
        }
    }
    private void FixedUpdate()
    {
        kph.text = RR.KPM.ToString("0");
        updateNeedle();
        nitrusUI();

      
    }
    public void updateNeedle()
    {
        desiredPosition = starPosiziton - endPosition;
        float temp = RR.engineRPM / 40000;
        neeedle.transform.eulerAngles = new Vector3(0, 0, (starPosiziton - temp * desiredPosition));
    }

    public void changGear()
    {
        gearNum.text = (!RR.reverse) ? (RR.gearNum + 1).ToString() : "R";
    }
    public void nitrusUI()
    {
        nitrusSlider.value = RR.nitrusValue / 45;
    }
   
}
