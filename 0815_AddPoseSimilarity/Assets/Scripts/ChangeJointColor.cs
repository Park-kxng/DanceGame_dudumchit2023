using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChangeJointColor : MonoBehaviour
{
    GameObject obj;
    Dictionary<GameObject, Renderer> MySphere = new Dictionary<GameObject, Renderer>();
    int numberOfSphere = 0;
    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Manager");
        SetSphereNumber(33);
        for (int i = 0; i < numberOfSphere; i++)
            MySphere.Add(GetSphere(i), GetSphere(i).GetComponent<Renderer>());

        var sphere = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.GetComponent<LandmarksReceiver2>().accuracy != null)
        {
            for (int i = 0; i < 33; i++)
            {
                // ¸ÂÀ¸¸é ÆÄ¶û Æ²¸®¸é »¡°­
                MySphere[GetSphere(i)].material.SetColor("_Color", obj.GetComponent<LandmarksReceiver2>().accuracy[i] == 0 ? Color.red : Color.blue);
            }
        }
    }

    void SetSphereNumber(int number)
    {
        numberOfSphere = number;
    }



    GameObject GetSphere(int i)
    {
        return GameObject.Find("Sphere (" + i + ")");
    }
}

