import time
import picamera
import datetime
import json

def TakePhoto(camera):
    """Ottaa kuvan ja tallentaa sen juuressa olevaan kansioon Photos."""
    tempDate = str(datetime.datetime.now(datetime.timezone.utc))
    date = tempDate.replace(" ", "_")
    date = date.replace(":", "-")
    date = date.replace(".", "-")
    photoName = "photo_" + date + ".jpg"
    savePath = "./Photos/"
    camera.capture(savePath + photoName)
    print(f'Photo "{photoName}" taken and saved to "{savePath}"!\n')

def GetPhotoCaptureState():
    """Lukee tiedostosta PhotoCaptureState.json booleanin IsCapturing."""
    isCapturing = ""
    try:
        f = open("PhotoCaptureState.json")
        state = json.load(f)
        isCapturing = state["IsCapturing"]
        f.close()
    except:
        print("PhotoCapture.py > GetPhotoCaptureState() went into exception.")
    if isCapturing == "true" or isCapturing == True:
        return True
    else:
        return False

def GetTimeBetweenPhotoCapture():
    """Lukee tiedostosta appsettings.json arvon Settings:TimeBetweenPhotoCapture"""
    try:
        f = open("appsettings.json", encoding="UTF-8-SIG")
        appsettings = json.load(f)
        TimeBetweenPhotoCapture = appsettings["Settings"]["TimeBetweenPhotoCapture"]
        f.close()
        return int(TimeBetweenPhotoCapture)
    except:
        print('Could not load TimeBetweenPhotoCapture from appsettings.json! Used the default of "3" instead.')
    return 3

"""Ohjelman main loop. Tarkistaa, onko isCapturing True. Jos on, ottaa kuvan."""
with picamera.PiCamera() as camera:
    TimeBetweenPhotoCapture = GetTimeBetweenPhotoCapture()
    while True:
        time.sleep(TimeBetweenPhotoCapture)
        try:
            isCapturing = GetPhotoCaptureState()
            if isCapturing == True:
                TakePhoto(camera)
            else:
                print("No photo taken.\n")
        except:
            print("PhotoCapture.py > main loop went into exception")

