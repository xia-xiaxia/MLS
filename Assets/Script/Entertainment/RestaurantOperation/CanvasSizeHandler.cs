using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Orthographic ÆÁÄ» 
//Scale ±ÈÀý³ß
//Rate ±ÈÂÊ
public class CanvasSizeHandler : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    private Vector2 windowsResolution = new Vector2(880, 1040);




    public void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            canvasScaler.referenceResolution = windowsResolution;
        }
        else
        {
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }
    }
}
