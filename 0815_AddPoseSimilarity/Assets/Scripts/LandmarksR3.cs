using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;

public class LandmarksR3 : MonoBehaviour
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
    }

    /*
    void Update()
    {
        lock (this)
        {
            // landmarks가 존재할 때
            if (landmarksList.Count > 0)
            {
                // landmarks 적용
                // 각 조인트에 대해 랜드마크 적용
                for (int i = 0; i < 33; i++)
                {
                    // 조인트에 대한 랜드마크 인덱스 계산
                    int landmarkIndex = i * 3;

                    // 조인트에 대한 랜드마크 추출
                    float x = landmarksList[landmarkIndex] * 10;
                    float y = landmarksList[landmarkIndex + 1] * 10;
                    float z = landmarksList[landmarkIndex + 2] * 10;

                    // 조인트에 랜드마크 적용
                    ApplyLandmarkToJoint(i, x, y, z);
                }

                // landmarks 비우기
                landmarksList.Clear();
            }
        }
    }

    // 조인트에 랜드마크 적용하는 메소드
    void ApplyLandmarkToJoint(int jointIndex, float x, float y, float z)
    {
        // 조인트에 대한 랜드마크 적용 로직 작성
        // targetObject에서 해당 조인트를 찾아 랜드마크 값을 적용하는 예시 코드
        // targetObject는 캐릭터나 캐릭터의 부모 객체

        // JointType에 따라 조인트를 찾아 랜드마크를 적용 => 수작업으로 해줘야 함
        switch (jointIndex)
        {
            case 0:
                // JointType 0에 해당하는 조인트에 랜드마크 적용
                // targetObject의 자식 객체인 Head 객체의 위치를 랜드마크 값으로 변경
                targetObject.transform.Find("Head").localPosition = new Vector3(x, y, z);
                break;
            case 1:
                // JointType 1에 해당하는 조인트에 랜드마크 적용
                // 예시: targetObject의 자식 객체인 Neck 객체의 위치를 랜드마크 값으로 변경
                targetObject.transform.Find("Neck").localPosition = new Vector3(x, y, z);
                break;
                // 나머지 조인트에 대한 적용 로직 작성해야함...이러나 저러나 수작업 예~
                // ...
        }
    }
    */
  
    void Update()
    {
        lock (this)
        {
            // landmarks가 존재할 때
            if (landmarksList.Count > 0)
            {
                // landmarks 적용 33개
                for (int i = 0; i < 33; i++)
                {
                    if (i>0 && i <11)
                    { // 코를 제외한 얼굴의 모든 부분은 pass
                        continue;
                    }
                    Body[i].transform.localPosition = new Vector3(landmarksList[i] / 100, landmarksList[i + 1] / 100, landmarksList[i + 2] /100);
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
        string filePath = Application.dataPath + "/" + filename + ".txt";

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
                    for (int i = 0; i < landmarkStrArr.Length - 1; i += 3)
                    {
                        float x = float.Parse(landmarkStrArr[i]);
                        float y = float.Parse(landmarkStrArr[i + 1]);
                        float z = float.Parse(landmarkStrArr[i + 2]);
                        //mediapipe의 x, y, z 좌표계: x우, y하, z앞
                        //Unity의 x, y, z 좌표계: x우, y상,z앞
                        landmarksList.Add(x);
                        landmarksList.Add(-y);
                        landmarksList.Add(z);
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
