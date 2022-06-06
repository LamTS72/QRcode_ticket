import cv2
import numpy as np
import pyzbar.pyzbar as pyzbar
import urllib.request
import time
import keyboard
from datetime import datetime


#!/usr/bin/python

# -*- coding: utf8 -*-
 
#cap = cv2.VideoCapture(0) #that is for camera in laptop
font = cv2.FONT_HERSHEY_SIMPLEX
 
url='http://172.16.2.72/'
cv2.namedWindow("live streaming", cv2.WINDOW_AUTOSIZE)
 
prev=""
curr=""
listcode = []
while True:
    #print("Waiting to scan:......")
    now = datetime.now()
    img_resp=urllib.request.urlopen(url+'cam-hi.jpg')
    imgnp=np.array(bytearray(img_resp.read()),dtype=np.uint8)
    frame=cv2.imdecode(imgnp,-1)
    #_, frame = cap.read()
    
    decodedObjects = pyzbar.decode(frame)
    for obj in decodedObjects:
        curr=obj.data
        if curr not in listcode:
            scannedTime = now
            print(obj.data.decode("utf-8"),flush = True)
            listcode.append(curr)
        else:
            difference = now - scannedTime
            if(difference.total_seconds() >= 5):
                print(obj.data.decode("utf-8"),flush = True)
        cv2.putText(frame, str(obj.data.decode("utf-8")), (50, 50), font, 1,(255, 0, 0), 3)
        

 
    cv2.imshow("live streaming", frame)
 
    cv2.waitKey(1)
    #if key == 27:
        #break
    if keyboard.is_pressed('q'):
        break


cv2.destroyAllWindows()
