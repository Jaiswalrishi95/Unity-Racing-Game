using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger1 : MonoBehaviour
{
    public Record ms;
    public GameObject canvas;
    public GameObject back;
    public GameObject Cbutton;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "f22e4123")
        {
            canvas.SetActive(true);
            back.SetActive(false);
            Cbutton.SetActive(false);
            ms.stop_recording();
            ms.stop_replay();
        }
        
    }
}