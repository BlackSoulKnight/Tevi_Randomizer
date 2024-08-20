import json
import os
import random
import sys

## Reading main Data files

    

Path = os.path.dirname(os.path.realpath(__file__))


from enum import Enum



def convertAreaJsonToTxt():
    s = open("./Maps/map0.json",'r')
    d = open("../../resource/NewArea.txt",'w+')
    c = open("../../resource/NewConnection.txt",'w+')
    s = json.load(s)
    for v in s:
        print(v)
        name = f"{v['Map']},{v['RoomX']},{v['RoomY']},{v['RoomSection']}"
        d.write(name+"\n")
        for ve in v["Connection"]:
            c.write(f"{name}:{ve['Map']},{ve['RoomX']},{ve['RoomY']},{ve['RoomSection']}:{ve['Method']}\n")
    #for k,v in s.items():
    #    for a in v:
    #        d.write(f"{a['Name']}\n")
    #        for cs in a["Connections"]:
    #            c.write(f"{a['Name']}:{cs['Exit']}:{cs['Method']}\n")
    d.close()
    


def convertLocationJsonToTxt():
    s = open("Item Location/map0.json",'r')
    d = open("../../resource/NewLocation.txt",'w+')
    s = json.load(s)
    for k in s:
        d.write(f"{k['ItemName'].split(' ')[0]}:{k['Location']}:{k['SlotID']}:{k['Method']}\n")
    d.close()

convertAreaJsonToTxt()
convertLocationJsonToTxt()

