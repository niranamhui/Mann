using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class awakeManager : MonoBehaviour
{
    public GameObject toRotate;
    public vehicleList listOfVehicles;
    public int vehiclePointer = 0;
    public Text currency;
    public float rotateSpeed;

    private void Awake()
    {
        vehiclePointer = PlayerPrefs.GetInt("pointer");
       //PlayerPrefs.SetInt("currency", 15000);
        currency.text = "$" + PlayerPrefs.GetInt("currency").ToString();


        GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        childObject.transform.parent = toRotate.transform;
    }
    private void FixedUpdate()
    {
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    }
    public void rightButton()
    {
        if (vehiclePointer < listOfVehicles.vehicles.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, toRotate.transform.rotation) as GameObject;
            childObject.transform.parent = toRotate.transform;
        }
    }
    public void leftButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, toRotate.transform.rotation) as GameObject;
            childObject.transform.parent = toRotate.transform;
        }
    }
   
    public void startGameButton()
    {
        SceneManager.LoadScene("GameScnenstest");
    }
    
  
}
