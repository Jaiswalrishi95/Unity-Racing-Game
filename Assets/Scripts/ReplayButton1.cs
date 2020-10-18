using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayButton1 : MonoBehaviour
{
    public Record ms;
    public GameObject canvas;
    public Laps ls;
    public void OnClick()
    {
        canvas.SetActive(false);
        ms.start_replay();
        ls.ResetLap();
    }
}
