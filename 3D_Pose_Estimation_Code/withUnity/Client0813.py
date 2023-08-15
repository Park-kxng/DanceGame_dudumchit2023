import socket

def main():
    host = "127.0.0.1"  # Unity 서버의 IP 주소
    port = 8888        # Unity 서버의 포트 번호

    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client.connect((host, port))

    # 서버로부터 메시지 받기
    data = client.recv(1024)
    print("Received message from Unity:", data.decode('utf-8'))

    client.close()

if __name__ == "__main__":
    main()
