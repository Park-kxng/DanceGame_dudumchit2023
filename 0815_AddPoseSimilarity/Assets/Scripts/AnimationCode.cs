using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class AnimationCode : MonoBehaviour
{
    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        lines = System.IO.File.ReadLines("Assets/AnimationFile.txt").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        //print(lines[0]);
        // 각 조인트 연결 라인
        string[] points = lines[counter].Split(',');

        for (int i = 0; i <= 32; i++)
        {
            ///print(points[0]);
            float x = float.Parse(points[0 +(i*3)]) / 100;
            float y = float.Parse(points[1 + (i * 3)]) / 100;
            float z = float.Parse(points[2 + (i * 3)]) / 100;
            Body[0].transform.localPosition = new Vector3(x, y, z);
        }
        
        counter += 1;

        // counter이 frame과 같아지면 종료
        if (counter == lines.Count) { counter = 0; }

        Thread.Sleep(30);

    }
}
