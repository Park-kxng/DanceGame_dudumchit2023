import pickle
import socket
import cv2
import mediapipe as mp
import numpy as np

# 생성된 춤 미디어파이프 결과 npy 파일 불러옴
generated_dance = np.load('./dance_mediapipe.npy')
print("생성된 춤 shape: ", generated_dance.shape)
rows,columns = generated_dance.shape
frames = int(rows/33)
flag_frame = 0

# 포즈 비교 함수 - (x,y)는 유클리드, z는 맨하탄 거리 비교
def pose_similarity_2d_depth(dance, user_motion, alpha=0.5, beta=0.5):
    i = flag_frame
    x1, y1, d1 = dance[33*i:33*(i+1), 0], dance[33*i:33*(i+1), 1], dance[33*i:33*(i+1), 2]
    x2, y2, d2 = user_motion[:, 0], user_motion[:, 1], user_motion[:, 2]
    location_dist = np.sqrt((x1-x2)**2 + (y1-y2)**2)
    depth_dist = np.abs(d1-d2)
    similarity = np.exp(-alpha * location_dist - beta * depth_dist)
    return similarity

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

            # 생성된 춤과 사용자의 춤 비교하여 정확도 계산
            print("flag:", flag_frame)
            user_motion = np.array([[landmark.x, landmark.y, landmark.z] for landmark in landmarks])
            print("user_motion: ",user_motion.shape)
            posesimilarity = pose_similarity_2d_depth(generated_dance, user_motion)
            print("posesimilarity: ", posesimilarity)
            # 정확도가 낮은 조인트 파란색으로 변경하기 위함
            ####### 우선 정확도 0.5 기준으로 해둠. 나중에 변경
            posesimilarity_result = np.where(posesimilarity < 0.5, 0, 1) # 정확도가 0.5보다 작으면 0, 크면 1
            print("posesimilarity: ", posesimilarity_result)

            landmarks_str = ""
            i=0
            for landmark in landmarks:
                landmarks_str += str(landmark.x) + "," + str(landmark.y) + "," + str(landmark.z) +"," + str(posesimilarity_result[i]) +"," ;
                i+=1

            client_socket.send(landmarks_str.encode())
            print(landmarks_str)
            flag_frame += 1
            #print(landmarks_str.encode())
            #landmarks_str = ""
            #print(landmarks_str)

            # 생성된 춤 종료 후 처리 필요

        except:
            print("pass")
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
