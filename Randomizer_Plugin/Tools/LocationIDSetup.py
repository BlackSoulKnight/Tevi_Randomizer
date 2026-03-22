import json
import os
import random


## Reading main Data files

    

Path = os.path.dirname(os.path.realpath(__file__))


def teleporterHub():
    s = open(Path+"/Area.json",'r')
    s = json.load(s)
    a = 0
    s["TeleporterHub"] = [{"Name":f"TeleportHub","Connections":[]}]
    for k in s["Teleporter"]:
        data = {"Exit":k["Name"],"Method":f"Teleporter {a}"}
        s["TeleporterHub"][0]["Connections"].append(data)
        a+=1
    d = open(Path+"Test.json",'w+')
    d.write(json.dumps(s))
    d.close


def addLocationId():
    maxID = 0
    s = open(Path+"/../resource/JsonFiles/Location.json",'r')
    locations = json.load(s)
    s.close()
    s = open(Path+"/../resource/JsonFiles/MoneyLocations.json",'r')
    moneylocation = json.load(s)
    s.close() 
    s = open(Path+"/../resource/JsonFiles/UpgradeResourceLocation.json",'r')
    resourcelocation = json.load(s)
    s.close()
    combined = locations+resourcelocation+moneylocation
    noIds = [] 
    for v in combined:
        if "EVENT_" in v["Itemname"]:
            v["LocationID"] = 0
            continue
        if not"LocationID" in v:
            noIds.append(v)
        elif v["LocationID"] > maxID:
            maxID = v["LocationID"]
    for v in noIds:
        maxID +=1
        v["LocationID"] = maxID
    s = open(Path+"/../resource/JsonFiles/Location.json",'w+')
    s.write(json.dumps(locations,indent=4))
    s.close()
    s = open(Path+"/../resource/JsonFiles/MoneyLocations.json",'w+')
    s.write(json.dumps(moneylocation,indent=4))
    s.close()
    s = open(Path+"/../resource/JsonFiles/UpgradeResourceLocation.json",'w+')
    s.write(json.dumps(resourcelocation,indent=4))
    s.close()
    


        
addLocationId()

