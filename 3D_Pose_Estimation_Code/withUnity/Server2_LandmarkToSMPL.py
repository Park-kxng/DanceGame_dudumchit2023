import pickle
import socket
import cv2
import mediapipe as mp
import numpy as np

# 서버 주소와 포트번호
HOST = '127.0.0.1'
PORT = 9999

# 소켓 생성
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 서버 주소와 포트번호 할당
server_socket.bind((HOST, PORT))

# 연결 대기
server_socket.listen()

# Mediapipe에서 사용할 Pose 모델과 랜드마크를 추출하기 위한 모듈을 초기화
mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

# 웹캠에서 영상을 가져오기 위해 VideoCapture 객체를 생성
cap = cv2.VideoCapture(0)
# Pose 모델 초기화
pose = mp_pose.Pose(min_detection_confidence=0.7, min_tracking_confidence=0.7)

while True:
    # 클라이언트로부터 연결 요청 수락
    client_socket, addr = server_socket.accept()
    print(f"Connected by {addr}")

    while True:
        # 비디오 프레임 가져오기
        ret, frame = cap.read()
        if ret == False:
            print("비디오 프레임 가져오지 못함")
            break


        # Convert BGR to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Make detection
        results = pose.process(image)

        # Draw landmarks
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)

        # Extract landmarks
        try:
            landmarks = results.pose_landmarks.landmark
            #landmarks = np.array([[landmark.x, landmark.y, landmark.z, landmark.visibility] for landmark in landmarks])
            #####print(landmarks)
            print("=======================")
            # landmarks 데이터 클라이언트에게 전송
            #data = landmarks.tobytes()
            ####data = pickle.dumps(landmarks)
            #client_socket.send(data)
            #print(landmarks)
            #print(data)

            #########
            # SMPL 변환
            smpl_landmarks = np.array([[landmark.x, landmark.y, landmark.z] for landmark in landmarks])

            landmarks_str = ""
            for landmark in landmarks:
                landmarks_str += str(landmark.x) + "," + str(landmark.y) + "," + str(landmark.z) +",";

            client_socket.send(landmarks_str.encode())
            print(landmarks_str)
            print(landmarks_str.encode())
            landmarks_str = ""
            print(landmarks_str)
        except:
            pass

        # Show image
        cv2.imshow('Pose Estimation', image)
        # Show image
        #resized_image = cv2.resize(image, (int(image.shape[1] / 2), int(image.shape[0])))
        #cv2.imshow('Pose Estimation', resized_image)
        # Exit when escape is pressed
        if cv2.waitKey(1) == 27:
            break

    # 클라이언트와의 연결 종료
    client_socket.close()

# 소켓 종료
server_socket.close()
