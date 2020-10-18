using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Laps : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public int lapValue;
    Text Lap;
    // Start is called before the first frame update
    void Start()
    {
        lapValue = 0;
        lapText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        lapText.text = "Laps:" + lapValue;
    }
    public void ResetLap()
    {
        lapValue = 0;
        Debug.Log("Reset");
    }
}
