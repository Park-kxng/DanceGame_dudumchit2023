// Server.cs 0815
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Text;
//
using System.Diagnostics;


public class Server : MonoBehaviour
{
    private const int port = 8888;
    private TcpListener server;

    private void Start()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Debug.Log("Server started. Waiting for clients...");

        // Start the server handling thread
        Thread serverThread = new Thread(ServerThreadFunction);
        serverThread.Start();

        // ������ �źε� 
        /*
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = "C:/Users/SAMSUNG/AppData/Local/Programs/Python/Python39";
            // ������ ���ø����̼� �Ǵ� ����
            psi.StartInfo.Arguments = "C:/Users/SAMSUNG/Desktop/3D_Pose_Estimation/3D_Pose_Estimation_Code/withUnity/Client0813.py";
            // ���� ���۽� ����� �μ�
            psi.StartInfo.CreateNoWindow = true;
            // ��â �ȶ����
            psi.StartInfo.UseShellExecute = false;
            // ���μ����� �����Ҷ� �ü�� ���� �������
            psi.Start();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
        */
    }

    private void Update()
    {

    }

    private void ServerThreadFunction()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Debug.Log("Client connected.");

            // Start the Python client handling thread
            Thread pythonClientThread = new Thread(() => PythonClientThreadFunction(client));
            pythonClientThread.Start();
        }
    }

    private void PythonClientThreadFunction(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes("Hello from Unity server!");
        stream.Write(data, 0, data.Length);

        // Execute the Python client script
        string pythonPath = "python"; // ���̽� ���� ���
        string pythonScriptPath = "C:/Users/SAMSUNG/Desktop/3D_Pose_Estimation/3D_Pose_Estimation_Code/withUnity/Client0813.py"; // ���̽� Ŭ���̾�Ʈ ��ũ��Ʈ ���

        ProcessStartInfo psi = new ProcessStartInfo(pythonPath, pythonScriptPath);
        psi.RedirectStandardOutput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = psi;
        process.Start();

        while (!process.StandardOutput.EndOfStream)
        {
            string output = process.StandardOutput.ReadLine();
            Debug.Log("Python Output: " + output);
        }

        process.WaitForExit();

        client.Close();
    }
}