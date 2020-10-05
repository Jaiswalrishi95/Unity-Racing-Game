using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
   public Record ms;
    
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.name == "StartLine")
        {
            ms.Records();
        }
    }
}
