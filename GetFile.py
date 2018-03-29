import socket,io,Image
from array import array

def save_image(byts):
    image = Image.open(io.BytesIO(byts))
    image.save("nutt.jpg")

def recv_all(client):
    image_data = ''
    while True:
        data = client.recv(4096)
        if not data and image_data:
            save_image(image_data)
            image_data = ''

        elif data:
            print("Data received: "+data)
            image_data += data

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('127.0.0.1', 55555))
s.listen(5)
client, addr = s.accept()
recv_all(client)
