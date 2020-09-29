using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader1 : MonoBehaviour
{

    GameObject menu_Canvas;
    GameObject settings_Canvas;
    GameObject dataHandler;
    GameObject f22e4123;
    GameObject Lamborghini_Huracan_Variant;
    GameObject Aventador_tunnig_Variant;

    void Start()
    {
        dataHandler = GameObject.Find("DataHandler");
    }

    public void LoadScene(int a)
    {

        SceneManager.LoadScene(a);
    }

    public void Quit()
    {

        Application.Quit();
    }

    public void deleteDataHandler()
    {
        Destroy(dataHandler);
        f22e4123.SetActive(false);
        Lamborghini_Huracan_Variant.SetActive(false);
        Aventador_tunnig_Variant.SetActive(false);
    }
}