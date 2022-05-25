# QRcode_ticket
<p align="center"> <img width = "80%" height = "80%" src="images/Screenshot_1.png"/>  </p>
This is a model we use to assiociate to ESP332cam.  Beside using for scanning QR code , it contains a winform which is written by  C# for user easily 
use with one button SCAN QR and one richtextbox to print result. In addition we also associate to some functions to get json from database as well as
update status for ticket before/after scanning QR.

INCLUDE:

In `ScanQRcodeWithPython` folder, `Program.cs` and `ScanQRcodeWithPython/bin/Debug/scanqrcode.py` will be 2 main parts for this project. 

HOW TO USE:

When you click `Scan QR` box, system will be run a thread to compile `scanqrcode.py` . One window live steaming appears for scanning.

LIMITED SYSTEM:

When we use C# with a device hardware we meet some difficulties about warning of system. Especially in thread of system, it make some delay time as well as
asynchoronous between device and software.

