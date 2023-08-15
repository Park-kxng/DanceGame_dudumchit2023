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
    public ArticulationJointType[] jointTypes;

    /*
    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    */

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

    // Update �Լ�
    void Update()
    {
        // landmarks�� ������ ��
        if (landmarks != null)
        {
            Debug.Log("landmarks ������ ��");
            // landmarks ����
            
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
            // landmarks ����
            for (int i = 0; i <= 32; i++)
            {
                int index = i;
               
                Body[i].transform.localPosition = new Vector3(landmarks[index * 4], landmarks[index * 4 + 1], landmarks[index * 4 + 2]);

            }
            */
            // landmarks �ʱ�ȭ
            landmarks = null;

        }
    }

    // ���� Thread �Լ�
    void ReceiveThread()
    {
        while (isConnected)
        {
            //Thread.Sleep(30);
            try
            {
                // landmarks ������ ����
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
