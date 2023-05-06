import socket
import struct
import cv2
import numpy as np

# 서버 호스트 이름과 포트 번호
HOST = 'localhost'
PORT = 9999

# 서버 소켓 생성
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind((HOST, PORT))
server_socket.listen()

print('Waiting for client connection...')

# 클라이언트 연결 대기
client_socket, addr = server_socket.accept()
print('Connected by', addr)

# Unity에서 보낸 웹캠 영상의 크기
width, height = 640, 480

# 비디오 캡처 객체 생성
cap = cv2.VideoCapture(0)

# 비디오 캡처 시작
while True:
    # Unity에서 보낸 웹캠 영상 수신
    data = client_socket.recv(width * height * 3)
    frame = np.frombuffer(data, dtype=np.uint8).reshape(height, width, 3)

    # 프레임 회전 (MediaPipe Pose 모델은 세로 방향의 이미지를 기준으로 동작하므로, 세로 방향으로 회전)
    frame = cv2.rotate(frame, cv2.ROTATE_90_COUNTERCLOCKWISE)

    # 프레임을 JPEG 형식으로 인코딩하여 전송
    encode_param = [int(cv2.IMWRITE_JPEG_QUALITY), 90]
    result, encoded_frame = cv2.imencode('.jpg', frame, encode_param)
    data = struct.pack('!I', len(encoded_frame)) + encoded_frame.tobytes()
    client_socket.sendall(data)

    # 클라이언트로부터 포즈 추정 결과 수신
    data = client_socket.recv(1024)
    landmarks = []
    num_floats = len(data) // 4
    for i in range(num_floats):
        landmarks.append(struct.unpack('!f', data[i*4:(i+1)*4])[0])

    # Unity 클라이언트로 포즈 추정 결과 전송
    if len(landmarks) > 0:
        pose_result = struct.pack('!' + 'f'*len(landmarks), *landmarks)
        client_socket.sendall(pose_result)

    # 프레임 출력
    cv2.imshow('frame', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# 종료
cap.release()
cv2.destroyAllWindows()
client_socket.close()
server_socket.close()
