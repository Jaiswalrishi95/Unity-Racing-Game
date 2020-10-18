using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject Startline;
    public GameObject Laps;
    public Record ms;
    public Laps ls;
    public GameObject FinishLine;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
           Debug.Log("Name:"+ other.gameObject.name);
           ms.start_recording();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
            ls.lapValue += 1;
        }
        if(ls.lapValue == 10)
        {
            FinishLine.SetActive(true);
        }
    }
}
