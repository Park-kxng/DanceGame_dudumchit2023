import mediapipe as mp
import cv2

mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

# 웹캠으로부터 영상을 읽어올 수 있는 객체 생성
cap = cv2.VideoCapture(0)

# pose 추정을 위한 객체 생성
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
    while cap.isOpened():
        # 영상 읽어오기
        ret, frame = cap.read()

        if not ret:
            print("비디오를 가져올 수 없습니다. 종료 중...")
            break

        # 이미지 반전(미러링)
        image = cv2.flip(frame, 1)

        # 이미지를 RGB 포맷으로 변환
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

        # pose 추정 수행
        results = pose.process(image)

        # 추정 결과가 있을 경우
        if results.pose_landmarks:
            # 결과를 파일에 저장
            with open('AnimationFile.txt', 'w') as f:
                f.write(str(results.pose_landmarks))

            # 추정 결과를 이미지에 그리기
            mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)

        # 화면에 이미지 출력
        cv2.imshow('Pose Estimation', image)

        # q를 누르면 종료
        if cv2.waitKey(10) & 0xFF == ord('q'):
            break

cap.release()
cv2.destroyAllWindows()
