using UnityEngine;
using System.Collections;

public class CarSelector : MonoBehaviour
{

    public GameObject Mercedes;
    public GameObject Huracan;
    public GameObject Aventador;

    public int carSelected;

    void Start()
    {

        Mercedes.SetActive(true);
        Huracan.SetActive(false);
        Aventador.SetActive(false);

        carSelected = 1;
    }

    public void loadR8()
    {

        Mercedes.SetActive(true);
        Huracan.SetActive(false);
        Aventador.SetActive(false);

        carSelected = 1;
    }

    public void load458()
    {

        Mercedes.SetActive(false);
        Huracan.SetActive(true);
        Aventador.SetActive(false);

        carSelected = 2;
    }

    public void loadSLS()
    {

        Mercedes.SetActive(false);
        Huracan.SetActive(false);
        Aventador.SetActive(true);

        carSelected = 3;
    }
}