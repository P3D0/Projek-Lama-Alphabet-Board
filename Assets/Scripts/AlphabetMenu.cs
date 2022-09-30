using UnityEngine;
using System;
using StartApp;
public class AlphabetMenu : MonoBehaviour
{

    private void Start()
    {
        StartAppWrapper.addBanner(StartAppWrapper.BannerType.STANDARD, StartAppWrapper.BannerPosition.BOTTOM);
    }
}
