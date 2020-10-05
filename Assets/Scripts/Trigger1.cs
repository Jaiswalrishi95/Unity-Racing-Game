using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{
    public Record ms;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
            ms.Replay();
        }
        
    }
}