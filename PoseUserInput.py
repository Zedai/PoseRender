#!/usr/bin/env python
import socket
import time
import matplotlib.pyplot as plt
from PIL import Image
import io
import numpy as np
import sys
import time
import threading

stop = False

def receive(sock):
    while True:
        fragments = []
        while True:
             packet = s.recv(1000)
             print(packet)
             if packet[-4:] == b'done':
                 fragments.append(packet[:-4])
                 print("got done")
                 break
             if not packet: break
             fragments.append(packet)

             if packet[-5:] == b'close':
                 print("Unity Closed")
                 sys.exit()

        if stop:
            print("Exiting Receiver Thread")
            sock.close()
            print("Socket Closed")
            break
        pass
        #sock.recv(1000)

if __name__ == "__main__":
    s = socket.socket()
    # s.setblocking(0)
    while True:
        try:
            s.connect(('127.0.0.1', 12349))
            break
        except ConnectionRefusedError:
            print("Unity Connection Failed")
            time.sleep(1)
    receiver = threading.Thread(target=receive, args= (s, ))
    receiver.start()

    print("Connected to Unity")

    #    s.send(b"startXpos:12 Ypos:35 Zpos:25 Xrot:"+bytes(str(12), 'utf-8') + b" Yrot:2 Zrot:5 Wrot:1 fov:45end")
    while True:
        try:
            xpos = input("X Position:")
            ypos = input("Y Position:")
            zpos = input("Z Position:")
            xrot = input("X Rotation:")
            yrot = input("Y Rotation:")
            zrot = input("Z Rotation:")
            wrot = input("W Rotation:")
            fov = input("FOV:")

        
            print("startXpos:12 Ypos:35 Zpos:25 Xrot:"+str(i) + " Yrot:2 Zrot:5 Wrot:1 fov:45end")
            s.send(b"startXpos:12 Ypos:35 Zpos:25 Xrot:"+bytes(str(i), 'utf-8') + b" Yrot:2 Zrot:5 Wrot:1 fov:45end")
            time.sleep(1)
        except KeyboardInterrupt:
            print("Exiting Main Thread")
            stop = True
            #print("threads successfully closed")
            break

    stop = True
    s.close()
    print("Socket Closed")

    # time.sleep(1)

    fragments = []
    while True:
         packet = s.recv(1000)
         print(packet)
         if packet[-4:] == b'done':
             fragments.append(packet[:-4])
             print("got done")
             break
         if not packet: break
         fragments.append(packet)

         if packet[-5:] == b'close':
             print("Unity Closed")
             sys.exit()

    # s.send(b"startXpos:12 Ypos:35 Zpos:25 Xrot:12 Yrot:2 Zrot:5 Wrot:1 fov:45end")

    # final = b"".join(fragments)
    # b = io.BytesIO(final)
    # image = Image.open(b)
    # plt.imshow(np.asarray(image))


