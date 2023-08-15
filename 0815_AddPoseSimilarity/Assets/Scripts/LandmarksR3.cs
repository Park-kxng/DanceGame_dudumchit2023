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
    private float[] landmarks;

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
    }

    /*
    void Update()
    {
        lock (this)
        {
            // landmarks�� ������ ��
            if (landmarksList.Count > 0)
            {
                // landmarks ����
                // �� ����Ʈ�� ���� ���帶ũ ����
                for (int i = 0; i < 33; i++)
                {
                    // ����Ʈ�� ���� ���帶ũ �ε��� ���
                    int landmarkIndex = i * 3;

                    // ����Ʈ�� ���� ���帶ũ ����
                    float x = landmarksList[landmarkIndex] * 10;
                    float y = landmarksList[landmarkIndex + 1] * 10;
                    float z = landmarksList[landmarkIndex + 2] * 10;

                    // ����Ʈ�� ���帶ũ ����
                    ApplyLandmarkToJoint(i, x, y, z);
                }

                // landmarks ����
                landmarksList.Clear();
            }
        }
    }

    // ����Ʈ�� ���帶ũ �����ϴ� �޼ҵ�
    void ApplyLandmarkToJoint(int jointIndex, float x, float y, float z)
    {
        // ����Ʈ�� ���� ���帶ũ ���� ���� �ۼ�
        // targetObject���� �ش� ����Ʈ�� ã�� ���帶ũ ���� �����ϴ� ���� �ڵ�
        // targetObject�� ĳ���ͳ� ĳ������ �θ� ��ü

        // JointType�� ���� ����Ʈ�� ã�� ���帶ũ�� ���� => ���۾����� ����� ��
        switch (jointIndex)
        {
            case 0:
                // JointType 0�� �ش��ϴ� ����Ʈ�� ���帶ũ ����
                // targetObject�� �ڽ� ��ü�� Head ��ü�� ��ġ�� ���帶ũ ������ ����
                targetObject.transform.Find("Head").localPosition = new Vector3(x, y, z);
                break;
            case 1:
                // JointType 1�� �ش��ϴ� ����Ʈ�� ���帶ũ ����
                // ����: targetObject�� �ڽ� ��ü�� Neck ��ü�� ��ġ�� ���帶ũ ������ ����
                targetObject.transform.Find("Neck").localPosition = new Vector3(x, y, z);
                break;
                // ������ ����Ʈ�� ���� ���� ���� �ۼ��ؾ���...�̷��� ������ ���۾� ��~
                // ...
        }
    }
    */
  
    void Update()
    {
        lock (this)
        {
            // landmarks�� ������ ��
            if (landmarksList.Count > 0)
            {
                // landmarks ���� 33��
                for (int i = 0; i < 33; i++)
                {
                    if (i>0 && i <11)
                    { // �ڸ� ������ ���� ��� �κ��� pass
                        continue;
                    }
                    Body[i].transform.localPosition = new Vector3(landmarksList[i] / 100, landmarksList[i + 1] / 100, landmarksList[i + 2] /100);
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
        string filePath = Application.dataPath + "/" + filename + ".txt";

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
                    for (int i = 0; i < landmarkStrArr.Length - 1; i += 3)
                    {
                        float x = float.Parse(landmarkStrArr[i]);
                        float y = float.Parse(landmarkStrArr[i + 1]);
                        float z = float.Parse(landmarkStrArr[i + 2]);
                        //mediapipe�� x, y, z ��ǥ��: x��, y��, z��
                        //Unity�� x, y, z ��ǥ��: x��, y��,z��
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
