using UnityEngine;
using System;
using StartApp;

public class StartAppBackPlugin : MonoBehaviour
{
    private void Start()
    {
        StartAppWrapper.loadAd();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !StartAppWrapper.onBackPressed(gameObject.name))
        {
            exit();
        }
    }

    void exit()
    {
        Application.Quit();
    }
}
