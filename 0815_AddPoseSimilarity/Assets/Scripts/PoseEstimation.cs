using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PoseEstimationResult
{
    public List<Vector2> landmarks;
}

public class PoseEstimation : MonoBehaviour
{
    public RawImage rawImage;
    private Texture2D texture;

    IEnumerator Start()
    {
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // ��ķ �ؽ��ĸ� RawImage�� ǥ��
        rawImage.texture = texture;
        var webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // ���� �������� texture�� ����
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            // texture�� JPEG�� ���ڵ��Ͽ� ����Ʈ �迭�� ��ȯ
            byte[] bytes = texture.EncodeToJPG();
            // Flask API�� POST ��û�� ������ ����� ����
            UnityWebRequest request = UnityWebRequest.Post("http://localhost:5000/pose_estimation", "POST");
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // ���� JSON ����� �Ľ��Ͽ� landmark�� RawImage�� ǥ��
                string json = request.downloadHandler.text;
                Debug.Log(json); // ���ŵ� JSON �����͸� ����� �α׿� ���
                var result = JsonUtility.FromJson<PoseEstimationResult>(json);
                DrawLandmarks(result.landmarks);
            }
        }
    }

    private List<Vector2> AdjustLandmarks(List<Vector2> landmarks, float scaleX, float scaleY)
    {
        var adjustedLandmarks = new List<Vector2>();
        for (int i = 0; i < landmarks.Count; i++)
        {
            var landmark = landmarks[i];
            landmark.x *= scaleX;
            landmark.y *= scaleY;
            adjustedLandmarks.Add(landmark);
        }
        return adjustedLandmarks;
    }

    private void DrawLandmarks(List<Vector2> landmarks)
    {
        // RawImage ũ�⿡ �°� landmark ��ġ�� ����
        float scaleX = (float)texture.width / rawImage.rectTransform.rect.width;
        float scaleY = (float)texture.height / rawImage.rectTransform.rect.height;
        var adjustedLandmarks = AdjustLandmarks(landmarks, scaleX, scaleY);

        // landmark�� RawImage�� ǥ��
        var canvas = rawImage.GetComponentInParent<Canvas>();
        foreach (var landmark in adjustedLandmarks)
        {
            var marker = new GameObject("Marker");
            marker.transform.SetParent(canvas.transform);
            var image = marker.AddComponent<Image>();
            image.color = Color.red;
            image.rectTransform.sizeDelta = new Vector2(10, 10);
            // RawImage ���� ��ġ ����
            var rawImageRect = rawImage.rectTransform.rect;
            var markerRect = marker.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0, 1);
            markerRect.anchorMax = new Vector2(0, 1);
            markerRect.pivot = new Vector2(0.5f, 0.5f);
            markerRect.anchoredPosition = new Vector2(landmark.x - rawImageRect.width / 2, -landmark.y + rawImageRect.height / 2);
        }
    }

}