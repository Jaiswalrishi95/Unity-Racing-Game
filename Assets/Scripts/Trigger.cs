using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Record ms;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
           Debug.Log("Name:"+ other.gameObject.name);
           ms.start_recording();
        }
    }
}
