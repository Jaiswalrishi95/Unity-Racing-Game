using UnityEngine;
using System.Collections;

public class DataImplementer : MonoBehaviour
{
public int carSelected;

    public GameObject Mercedes;
    public GameObject Huracan;
    public GameObject Aventador;


    void Start()
    {

        carSelected = GameObject.Find("DataHandler").GetComponent<DataHandler>().carSel;

        if (carSelected == 1)
        {
            Mercedes.SetActive(true);
            Huracan.SetActive(false);
            Aventador.SetActive(false);
        }

        else if (carSelected == 2)
        {

            Mercedes.SetActive(false);
            Huracan.SetActive(true);
            Aventador.SetActive(false);
        }

        else if (carSelected == 3)
        {

            Mercedes.SetActive(false);
            Huracan.SetActive(false);
            Aventador.SetActive(true);
        }
    }

    void Update()
    {

    }
}