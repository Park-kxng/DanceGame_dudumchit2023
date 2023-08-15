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
        // 웹캠 텍스쳐를 RawImage에 표시
        rawImage.texture = texture;
        var webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // 현재 프레임을 texture에 복사
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            // texture를 JPEG로 인코딩하여 바이트 배열로 변환
            byte[] bytes = texture.EncodeToJPG();
            // Flask API에 POST 요청을 보내고 결과를 받음
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
                // 받은 JSON 결과를 파싱하여 landmark를 RawImage에 표시
                string json = request.downloadHandler.text;
                Debug.Log(json); // 수신된 JSON 데이터를 디버깅 로그에 출력
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
        // RawImage 크기에 맞게 landmark 위치를 조정
        float scaleX = (float)texture.width / rawImage.rectTransform.rect.width;
        float scaleY = (float)texture.height / rawImage.rectTransform.rect.height;
        var adjustedLandmarks = AdjustLandmarks(landmarks, scaleX, scaleY);

        // landmark를 RawImage에 표시
        var canvas = rawImage.GetComponentInParent<Canvas>();
        foreach (var landmark in adjustedLandmarks)
        {
            var marker = new GameObject("Marker");
            marker.transform.SetParent(canvas.transform);
            var image = marker.AddComponent<Image>();
            image.color = Color.red;
            image.rectTransform.sizeDelta = new Vector2(10, 10);
            // RawImage 상의 위치 조정
            var rawImageRect = rawImage.rectTransform.rect;
            var markerRect = marker.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0, 1);
            markerRect.anchorMax = new Vector2(0, 1);
            markerRect.pivot = new Vector2(0.5f, 0.5f);
            markerRect.anchoredPosition = new Vector2(landmark.x - rawImageRect.width / 2, -landmark.y + rawImageRect.height / 2);
        }
    }

}