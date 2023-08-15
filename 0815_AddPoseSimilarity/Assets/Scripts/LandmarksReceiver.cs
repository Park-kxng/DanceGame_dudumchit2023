using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;

public class LandmarksReceiver : MonoBehaviour
{
    // 서버 주소와 포트번호
    private string SERVER_IP = "127.0.0.1";
    private int SERVER_PORT = 9999;
    // 서버와 연결되어 있는지 여부를 저장하는 변수
    private bool isConnected = false;

    // TcpClient 변수
    private TcpClient client;

    // NetworkStream 변수
    private NetworkStream stream;

    // Thread 변수
    private Thread receiveThread;

    // landmarks 데이터를 저장하는 배열
    private float[] landmarks;

    // landmarks를 적용할 GameObject
    public GameObject targetObject;

    // landmarks를 적용할 JointType
    public ArticulationJointType[] jointTypes;

    /*
    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    */

    // 초기화 함수
    void Start()
    {
        // TcpClient 객체 생성
        client = new TcpClient();

        // 서버와 연결
        client.Connect(SERVER_IP, SERVER_PORT);

        // NetworkStream 객체 생성
        stream = client.GetStream();

        // 연결 상태 변경
        isConnected = true;
        Debug.Log("연결됨");

        // Thread 생성 및 실행
        receiveThread = new Thread(new ThreadStart(ReceiveThread));
        receiveThread.Start();
    }

    // Update 함수
    void Update()
    {
        // landmarks가 존재할 때
        if (landmarks != null)
        {
            Debug.Log("landmarks 존재할 때");
            // landmarks 적용
            
            for (int i = 0; i < jointTypes.Length; i++)
            {
                int index = (int)jointTypes[i];
                if (index >= 0 && index < landmarks.Length)
                {

                    Vector3 pos = new Vector3(landmarks[index * 4], landmarks[index * 4 + 1], landmarks[index * 4 + 2]);
                    targetObject.GetComponent<Animator>().SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(pos));
                }
            }
            /*
            // landmarks 적용
            for (int i = 0; i <= 32; i++)
            {
                int index = i;
               
                Body[i].transform.localPosition = new Vector3(landmarks[index * 4], landmarks[index * 4 + 1], landmarks[index * 4 + 2]);

            }
            */
            // landmarks 초기화
            landmarks = null;

        }
    }

    // 수신 Thread 함수
    void ReceiveThread()
    {
        while (isConnected)
        {
            //Thread.Sleep(30);
            try
            {
                // landmarks 데이터 수신
                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes > 0)
                {
                    landmarks = new float[bytes / 4];
                    MemoryStream ms = new MemoryStream(buffer);
                    BinaryReader br = new BinaryReader(ms);
                    for (int i = 0; i < landmarks.Length; i++)
                    {
                        landmarks[i] = br.ReadSingle();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                isConnected = false;
            }
        }
    }

    // 종료 함수
    void OnApplicationQuit()
    {
        // Thread 종료
        receiveThread.Abort();

        // 연결 종료
        stream.Close();
        client.Close();
    }
}
