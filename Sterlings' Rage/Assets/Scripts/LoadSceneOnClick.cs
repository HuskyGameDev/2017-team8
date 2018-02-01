using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour {

    public Image loadingScreen;

    public void startGame (int n)
    {
        SceneManager.LoadScene("MapDesign/Colin Scene");
        loadingScreen.enabled = true;

    }
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("Scenes/mainMenu");
        }
    }
}
