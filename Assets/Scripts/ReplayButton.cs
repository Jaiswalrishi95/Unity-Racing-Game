using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    public Record ms;
    public GameObject canvas;
    public GameObject back;
    public GameObject Cbutton;
    public void OnClick()
    {
        canvas.SetActive(false);
        back.SetActive(true);
        Cbutton.SetActive(false);
        ms.start_replay();
    }
}
