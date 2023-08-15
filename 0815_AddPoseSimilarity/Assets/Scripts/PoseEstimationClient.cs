using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PoseEstimationClient : MonoBehaviour
{
    // 서버 호스트 이름과 포트 번호
    private string host = "localhost";
    private int port = 9999;

    // 네트워크 소켓과 스트림
    private TcpClient client;
    private NetworkStream stream;

    // 랜드마크 좌표를 담을 배열
    private float[] landmarks = new float[33];

    // MediaPipe Pose 모델 로드
    [DllImport("MediapipeUnityPlugin")]
    private static extern IntPtr CreateGraph();
    [DllImport("MediapipeUnityPlugin")]
    private static extern void DeleteGraph(IntPtr graph);
    [DllImport("MediapipeUnityPlugin")]
    private static extern void ProcessImage(IntPtr graph, IntPtr image, int width, int height, float[] output);

    private IntPtr graph;

    void Start()
    {
        // 네트워크 소켓 연결
        client = new TcpClient(host, port);
        stream = client.GetStream();

        // MediaPipe Pose 모델 로드
        graph = CreateGraph();
    }

    void Update()
    {
        // RawImage에서 Texture2D를 생성하고, JPEG 형식으로 인코딩하여 전송
        Texture2D tex = GetComponent<RawImage>().texture as Texture2D;
        byte[] jpegBytes = tex.EncodeToJPG();
        byte[] lengthBytes = BitConverter.GetBytes(jpegBytes.Length);
        byte[] data = new byte[lengthBytes.Length + jpegBytes.Length];
        Array.Copy(lengthBytes, data, lengthBytes.Length);
        Array.Copy(jpegBytes, 0, data, lengthBytes.Length, jpegBytes.Length);
        stream.Write(data, 0, data.Length);

        // 랜드마크 좌표를 서버로부터 수신
        int numFloats = landmarks.Length;
        byte[] buffer = new byte[numFloats * 4];
        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int n = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
            bytesRead += n;
        }
        Buffer.BlockCopy(buffer, 0, landmarks, 0, buffer.Length);

        // 랜드마크 좌표를 MediaPipe Pose 모델에 입력하여 포즈 추정 결과 수신
        float[] pose = new float[33];
        GCHandle pinnedArray = GCHandle.Alloc(pose, GCHandleType.Pinned);
        IntPtr posePtr = pinnedArray.AddrOfPinnedObject();
        ProcessImage(graph, tex.GetNativeTexturePtr(), tex.width, tex.height, pose);
        pinnedArray.Free();

        // 서버로 포즈 추정 결과 전송
        byte[] poseBytes = new byte[pose.Length * 4];
        Buffer.BlockCopy(pose, 0, poseBytes, 0, poseBytes.Length);
        stream.Write(poseBytes, 0, poseBytes.Length);
    }

    void OnDestroy()
    {
        // 네트워크 소켓 종료 및 MediaPipe Pose 모델 삭제
        stream.Close();
        client.Close();
        DeleteGraph(graph);
    }
}
