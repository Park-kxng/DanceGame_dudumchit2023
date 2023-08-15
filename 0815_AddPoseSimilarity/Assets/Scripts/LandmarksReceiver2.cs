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
    // ���� �ּҿ� ��Ʈ��ȣ
    private string SERVER_IP = "127.0.0.1";
    private int SERVER_PORT = 9999;
    // ������ ����Ǿ� �ִ��� ���θ� �����ϴ� ����
    private bool isConnected = false;

    // TcpClient ����
    private TcpClient client;

    // NetworkStream ����
    private NetworkStream stream;

    // Thread ����
    private Thread receiveThread;

    // landmarks �����͸� �����ϴ� �迭
    public float[] landmarks;

    public float[] accuracy = new float[33];

    // landmarks�� ������ GameObject
    public GameObject targetObject;

    // landmarks�� ������ JointType
    //public ArticulationJointType[] jointTypes;


    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    // landmarks �����͸� �����ϴ� ����Ʈ
    private List<float> landmarksList = new List<float>();


    // �ʱ�ȭ �Լ�
    void Start()
    {
        // TcpClient ��ü ����
        client = new TcpClient();

        // ������ ����
        client.Connect(SERVER_IP, SERVER_PORT);

        // NetworkStream ��ü ����
        stream = client.GetStream();

        // ���� ���� ����
        isConnected = true;
        Debug.Log("�����");

        // Thread ���� �� ����
        receiveThread = new Thread(new ThreadStart(ReceiveThread));
        receiveThread.Start();

        var sphere = GetComponent<Renderer>();
    }

    // Update �Լ�
    void Update()
    {

        lock (this)
        {
            // landmarks�� ������ ��
            if (landmarksList.Count > 0)
            {
                // landmarks ����
                int i = 0;
                foreach (Transform child in targetObject.transform)
                {
                    child.localPosition = new Vector3(landmarksList[i]*10, landmarksList[i + 1] * 10, landmarksList[i + 2] * 10);
                    accuracy[i / 4] = landmarksList[i + 3];
                    i += 4;
                }
                // landmarks ����
                landmarksList.Clear();
            }
            
            
            
        }
       
    }

    
    // landmarks �����͸� txt ���Ϸ� �����ϴ� �޼ҵ�
    void SaveLandmarksToFile(float[] landmarks, String filename)
    {
        // ���� ���
        string filePath = Application.dataPath + "/" + filename+".txt";

        // ���� ����
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            for (int i = 0; i < landmarks.Length; i++)
            {
                writer.WriteLine(landmarks[i]);
            }
        }
    }

    // ���� ������ �Լ�
    void ReceiveThread()
    {
        // ���� ���°� �����Ǵ� ���� �ݺ�
        while (isConnected)
        {
            try
            {
                // ���� ������ �б�
                byte[] buffer = new byte[4096];
                int nbytes = stream.Read(buffer, 0, buffer.Length);
                if (nbytes > 0)
                {
                    // ������ �����͸� float �迭�� ��ȯ�Ͽ� landmarks�� �߰�
                    string landmarksStr = System.Text.Encoding.Default.GetString(buffer);
                    string[] landmarkStrArr = landmarksStr.Split(',');
                    for (int i = 0; i < landmarkStrArr.Length - 1; i += 4)
                    {
                        float x = float.Parse(landmarkStrArr[i]);
                        float y = float.Parse(landmarkStrArr[i + 1]);
                        float z = float.Parse(landmarkStrArr[i + 2]);
                        float r = float.Parse(landmarkStrArr[i + 3]);
                        //mediapipe�� x, y, z ��ǥ��: x��, y��, z��
                        //Unity�� x, y, z ��ǥ��: x��, y��,z��
                        //r�� ��� ���� ����Ʈ ��Ȯ ���� (������ 1, �ƴϸ� 0)
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
    // ���� Thread �Լ�
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
                    // landmarks ����
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

    // ���� �Լ�
    void OnApplicationQuit()
    {
        // Thread ����
        receiveThread.Abort();

        // ���� ����
        stream.Close();
        client.Close();
    }


}
