using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PoseEstimationClient : MonoBehaviour
{
    // ���� ȣ��Ʈ �̸��� ��Ʈ ��ȣ
    private string host = "localhost";
    private int port = 9999;

    // ��Ʈ��ũ ���ϰ� ��Ʈ��
    private TcpClient client;
    private NetworkStream stream;

    // ���帶ũ ��ǥ�� ���� �迭
    private float[] landmarks = new float[33];

    // MediaPipe Pose �� �ε�
    [DllImport("MediapipeUnityPlugin")]
    private static extern IntPtr CreateGraph();
    [DllImport("MediapipeUnityPlugin")]
    private static extern void DeleteGraph(IntPtr graph);
    [DllImport("MediapipeUnityPlugin")]
    private static extern void ProcessImage(IntPtr graph, IntPtr image, int width, int height, float[] output);

    private IntPtr graph;

    void Start()
    {
        // ��Ʈ��ũ ���� ����
        client = new TcpClient(host, port);
        stream = client.GetStream();

        // MediaPipe Pose �� �ε�
        graph = CreateGraph();
    }

    void Update()
    {
        // RawImage���� Texture2D�� �����ϰ�, JPEG �������� ���ڵ��Ͽ� ����
        Texture2D tex = GetComponent<RawImage>().texture as Texture2D;
        byte[] jpegBytes = tex.EncodeToJPG();
        byte[] lengthBytes = BitConverter.GetBytes(jpegBytes.Length);
        byte[] data = new byte[lengthBytes.Length + jpegBytes.Length];
        Array.Copy(lengthBytes, data, lengthBytes.Length);
        Array.Copy(jpegBytes, 0, data, lengthBytes.Length, jpegBytes.Length);
        stream.Write(data, 0, data.Length);

        // ���帶ũ ��ǥ�� �����κ��� ����
        int numFloats = landmarks.Length;
        byte[] buffer = new byte[numFloats * 4];
        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int n = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
            bytesRead += n;
        }
        Buffer.BlockCopy(buffer, 0, landmarks, 0, buffer.Length);

        // ���帶ũ ��ǥ�� MediaPipe Pose �𵨿� �Է��Ͽ� ���� ���� ��� ����
        float[] pose = new float[33];
        GCHandle pinnedArray = GCHandle.Alloc(pose, GCHandleType.Pinned);
        IntPtr posePtr = pinnedArray.AddrOfPinnedObject();
        ProcessImage(graph, tex.GetNativeTexturePtr(), tex.width, tex.height, pose);
        pinnedArray.Free();

        // ������ ���� ���� ��� ����
        byte[] poseBytes = new byte[pose.Length * 4];
        Buffer.BlockCopy(pose, 0, poseBytes, 0, poseBytes.Length);
        stream.Write(poseBytes, 0, poseBytes.Length);
    }

    void OnDestroy()
    {
        // ��Ʈ��ũ ���� ���� �� MediaPipe Pose �� ����
        stream.Close();
        client.Close();
        DeleteGraph(graph);
    }
}
