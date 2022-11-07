#!/usr/bin/env python
import socket
import time
from PIL import Image
import PIL
import io

def getRender(xpos = 0, ypos = 0, zpos = 0, xrot = 0, yrot = 0, zrot = 0, wrot = 1, fov = 60):
    s = socket.socket()
    while True:
        try:
            s.connect(('127.0.0.1', 12349))
            break
        except ConnectionRefusedError:
            print("Unity Connection Failed")
            time.sleep(1)
    
    s.send(b"startXpos:"+bytes(str(xpos), 'utf-8') + b" Ypos:"+bytes(str(ypos), 'utf-8') + b" Zpos:"+bytes(str(zpos), 'utf-8') + b" Xrot:"+bytes(str(xrot), 'utf-8') + b" Yrot:"+bytes(str(yrot), 'utf-8') + b" Zrot:"+bytes(str(zrot), 'utf-8') + b" Wrot:"+bytes(str(wrot), 'utf-8') + b" fov:"+bytes(str(fov), 'utf-8') + b"end")

    while True:
        fragments = []
        while True:
            packet = b''
            packet = s.recv(1000)
            if packet[-4:] == b'done':
                fragments.append(packet[:-4])
                break
            fragments.append(packet)
 
        final = b"".join(fragments)
        b = io.BytesIO(final)
        return Image.open(b)
