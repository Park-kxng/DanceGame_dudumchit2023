import socket

print("hello")
server_ip = '127.0.0.1'  # 유니티 서버의 IP 주소
server_port = 8888  # 유니티 서버의 포트 번호

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect((server_ip, server_port))
print("Connected to Unity server.")

while True:
    data = client.recv(1024)
    if not data:
        break
    received_message = data.decode('utf-8')
    print("Received:", received_message)

client.close()