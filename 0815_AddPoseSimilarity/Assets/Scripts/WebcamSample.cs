using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamSample : MonoBehaviour
{
    public RawImage display;
    WebCamTexture camTexture;
    private int currentIndex = 0;
    // Start is called before the first frame update
    private void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
        }
        if (camTexture != null)
        {
            display.texture = null;
            camTexture.Stop();
            camTexture = null;
        }
        WebCamDevice device = WebCamTexture.devices[currentIndex];
        camTexture = new WebCamTexture(device.name);
        display.texture = camTexture;
        camTexture.Play();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
