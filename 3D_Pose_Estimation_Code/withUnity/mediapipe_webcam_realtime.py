
# Python과 OpenCV를 이용하여 Mediapipe를 사용하여
# 웹캠으로 실시간 pose estimation을 수행하고,
# 화면에 skeleton을 표시하며, 실시간으로 numpy 결과를 출력
# 필요 패키지 import
import cv2
import mediapipe as mp
import numpy as np

# Mediapipe에서 사용할 Pose 모델과 랜드마크를 추출하기 위한 모듈을 초기화
mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

# 웹캠에서 영상을 가져오기 위해 VideoCapture 객체를 생성
cap = cv2.VideoCapture(0)

# 영상을 가져온 후, 해당 영상에서 랜드마크를 추출하기 위해
# Mediapipe Pose 모델을 적용
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
    while True:
        ret, frame = cap.read()
        if ret == False:
            break
        # pose 객체를 사용하여 랜드마크를 추출하고, 화면에 skeleton을 그리기
        # Convert BGR to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Make detection
        results = pose.process(image)

        # Draw landmarks
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)

        # 추출된 랜드마크를 이용하여 pose estimation 결과를 numpy array로 출력
        # Extract landmarks
        try:
            landmarks = results.pose_landmarks.landmark
            landmarks = np.array([[landmark.x, landmark.y, landmark.z, landmark.visibility] for landmark in landmarks])
            print(landmarks)
            # 여기 landmarks를 unity로 보내면 될 것 같음
        except:
            pass
        # Show image
        cv2.imshow('Pose Estimation', image)

        # Exit when escape is pressed
        if cv2.waitKey(1) == 27:
            break
        # Show image
        cv2.imshow('Pose Estimation', image)

        # Exit when escape is pressed
        if cv2.waitKey(1) == 27:
            break
        # 웹캠에서 실시간으로 영상이 가져와지면서 pose estimation 결과가 콘솔창에 출력되고, 화면에는 skeleton이 표시
        # ESC 키를 누르면 프로그램이 종료