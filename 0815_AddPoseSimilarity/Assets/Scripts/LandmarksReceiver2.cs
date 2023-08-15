using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;

public class LandmarksReceiver2 : MonoBehaviour
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
    public float[] landmarks;

    public float[] accuracy = new float[33];

    // landmarks를 적용할 GameObject
    public GameObject targetObject;

    // landmarks를 적용할 JointType
    //public ArticulationJointType[] jointTypes;


    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    // landmarks 데이터를 저장하는 리스트
    private List<float> landmarksList = new List<float>();


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

        var sphere = GetComponent<Renderer>();
    }

    // Update 함수
    void Update()
    {

        lock (this)
        {
            // landmarks가 존재할 때
            if (landmarksList.Count > 0)
            {
                // landmarks 적용
                int i = 0;
                foreach (Transform child in targetObject.transform)
                {
                    child.localPosition = new Vector3(landmarksList[i]*10, landmarksList[i + 1] * 10, landmarksList[i + 2] * 10);
                    accuracy[i / 4] = landmarksList[i + 3];
                    i += 4;
                }
                // landmarks 비우기
                landmarksList.Clear();
            }
            
            
            
        }
       
    }

    
    // landmarks 데이터를 txt 파일로 저장하는 메소드
    void SaveLandmarksToFile(float[] landmarks, String filename)
    {
        // 파일 경로
        string filePath = Application.dataPath + "/" + filename+".txt";

        // 파일 쓰기
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            for (int i = 0; i < landmarks.Length; i++)
            {
                writer.WriteLine(landmarks[i]);
            }
        }
    }

    // 수신 쓰레드 함수
    void ReceiveThread()
    {
        // 연결 상태가 유지되는 동안 반복
        while (isConnected)
        {
            try
            {
                // 수신 데이터 읽기
                byte[] buffer = new byte[4096];
                int nbytes = stream.Read(buffer, 0, buffer.Length);
                if (nbytes > 0)
                {
                    // 수신한 데이터를 float 배열로 변환하여 landmarks에 추가
                    string landmarksStr = System.Text.Encoding.Default.GetString(buffer);
                    string[] landmarkStrArr = landmarksStr.Split(',');
                    for (int i = 0; i < landmarkStrArr.Length - 1; i += 4)
                    {
                        float x = float.Parse(landmarkStrArr[i]);
                        float y = float.Parse(landmarkStrArr[i + 1]);
                        float z = float.Parse(landmarkStrArr[i + 2]);
                        float r = float.Parse(landmarkStrArr[i + 3]);
                        //mediapipe의 x, y, z 좌표계: x우, y하, z앞
                        //Unity의 x, y, z 좌표계: x우, y상,z앞
                        //r은 춤과 비교해 조인트 정확 유무 (맞으면 1, 아니면 0)
                        landmarksList.Add(x);
                        landmarksList.Add(-y);
                        landmarksList.Add(z);
                        landmarksList.Add(r);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                break;
            }
        }
    }

    /*
    // 수신 Thread 함수
    void ReceiveThread()
    {
        while (isConnected)
        {
            Thread.Sleep(30);

            try
            {
                //byte[] buffer = new byte[132];
                byte[] buffer = new byte[528];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                Debug.Log("Bytes received: " + bytes);
                if (bytes > 0)
                {
                    // landmarks 변경
                    lock (this)
                    {
                       
                        landmarks = new float[bytes / 4];
                        MemoryStream ms = new MemoryStream(buffer);
                        BinaryReader br = new BinaryReader(ms);

                        for (int i = 0; i < landmarks.Length; i++)
                        {
                            landmarks[i] = br.ReadSingle();
                            ///landmarks[i] = BitConverter.ToSingle(buffer, i * 4);
                            Debug.Log("Landmark " + i + ": " + landmarks[i]);
                        }
                       
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
    */

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
