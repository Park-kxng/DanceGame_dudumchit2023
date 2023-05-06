from flask import Flask, request, jsonify
import mediapipe as mp
import cv2
import numpy as np
import json

app = Flask(__name__)

mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

@app.route('/pose_estimation', methods=['POST'])
def pose_estimation():
    # 받은 이미지 데이터를 Numpy 배열로 변환
    data = np.frombuffer(request.data, np.uint8)
    img = cv2.imdecode(data, cv2.IMREAD_COLOR)
    # 이미지를 RGB로 변환
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    # Mediapipe로 Pose Estimation 수행
    with mp_pose.Pose(min_detection_confidence=0.7, min_tracking_confidence=0.7) as pose:
        results = pose.process(img_rgb)
        if results.pose_landmarks:
            landmarks = []
            for lm in results.pose_landmarks.landmark:
                landmarks.append({"x": lm.x, "y": lm.y})
            # Pose Estimation 결과를 JSON으로 변환하여 Unity에 전송
            result = {"landmarks": landmarks}
            return jsonify(result)
        else:
            return "no landmarks detected"

if __name__ == '__main__':
    app.run(host='127.0.0.1', port=5000)  # 서버를 5000번 포트에서 실행
