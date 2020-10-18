using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brakes : MonoBehaviour
{
    public AICarEngine ai;
    public AICarEngine1 ai1;
    public AICarEngine2 ai2;
    public int brakeTorque;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            ai.brakeTorque = brakeTorque;
        }
        if (other.gameObject.name == "Player1")
        {
            ai1.brakeTorque = brakeTorque;
        }
        if (other.gameObject.name == "Player2")
        {
            ai2.brakeTorque = brakeTorque;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            ai.brakeTorque = 0;
        }
        if (other.gameObject.name == "Player1")
        {
            ai1.brakeTorque = 0;
        }
        if (other.gameObject.name == "Player2")
        {
            ai2.brakeTorque = 0;
        }
    }
}
