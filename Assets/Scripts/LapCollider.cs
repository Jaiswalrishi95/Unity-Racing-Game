using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCollider : MonoBehaviour
{
    public GameObject Lap1;
    public GameObject Lap2;
    public GameObject Lap3;
    public GameObject Lap4;
    public GameObject Lap5;
    public GameObject Lap6;
    public GameObject Lap7;
    public GameObject Lap8;
    public GameObject Lap9;
    public GameObject Lap10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
            Lap1.SetActive(false);
            Lap2.SetActive(true);
            Lap2.SetActive(false);
            Lap3.SetActive(true);
        }
    }
}
