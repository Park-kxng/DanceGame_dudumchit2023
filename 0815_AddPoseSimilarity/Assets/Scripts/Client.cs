using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;

public class Client : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer;
    private int bufferLength;
    private bool isRunning = false;

    // List of sphere game objects
    public List<GameObject> spheres;

    private void Start()
    {
        // Set up the TCP client
        client = new TcpClient("127.0.0.1", 9999);
        stream = client.GetStream();
        buffer = new byte[1024];
        bufferLength = 0;

        // Set isRunning flag to true
        isRunning = true;
    }

    private void Update()
    {
        if (isRunning && stream.CanRead)
        {
            // Check if there is data available to read
            if (stream.DataAvailable)
            {
                // Read the data from the stream into the buffer
                bufferLength = stream.Read(buffer, 0, buffer.Length);

                // Convert the buffer into a string
                string data = Encoding.UTF8.GetString(buffer, 0, bufferLength);

                // Split the string into an array of floats
                string[] coords = data.Split(',');

                // Set the positions of the spheres based on the landmark positions
                for (int i = 0; i < spheres.Count; i++)
                {
                    float x = float.Parse(coords[i * 3]);
                    float y = float.Parse(coords[i * 3 + 1]);
                    float z = float.Parse(coords[i * 3 + 2]);
                    spheres[i].transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Close the connection when the script is destroyed
        stream.Close();
        client.Close();
    }
}
