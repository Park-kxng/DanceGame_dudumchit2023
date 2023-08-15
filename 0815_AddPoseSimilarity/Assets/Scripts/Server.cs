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

        // 엑세스 거부됨 
        /*
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = "C:/Users/SAMSUNG/AppData/Local/Programs/Python/Python39";
            // 시작할 어플리케이션 또는 문서
            psi.StartInfo.Arguments = "C:/Users/SAMSUNG/Desktop/3D_Pose_Estimation/3D_Pose_Estimation_Code/withUnity/Client0813.py";
            // 애플 시작시 사용할 인수
            psi.StartInfo.CreateNoWindow = true;
            // 새창 안띄울지
            psi.StartInfo.UseShellExecute = false;
            // 프로세스를 시작할때 운영체제 셸을 사용할지
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
        string pythonPath = "python"; // 파이썬 실행 경로
        string pythonScriptPath = "C:/Users/SAMSUNG/Desktop/3D_Pose_Estimation/3D_Pose_Estimation_Code/withUnity/Client0813.py"; // 파이썬 클라이언트 스크립트 경로

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