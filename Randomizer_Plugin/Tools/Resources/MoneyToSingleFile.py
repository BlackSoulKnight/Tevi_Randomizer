import json
import os
import random

Path = os.path.dirname(os.path.realpath(__file__))+"/Money/"
totalAmount = 0
singleLocationCount = {}
singleFile = []
for file in os.listdir(Path):
    jsonFileRead = open(Path+file,'r')
    jsonFile = json.load(jsonFileRead)
    jsonFileRead.close()
    totalAmount += len(jsonFile)
    for location in jsonFile:
        # Adding a number to each block if its on the same Map location
        if "X:" in location["LocationName"]:
            continue
        position = location["LocationRegion"][0]["Area"]*100_000 + location["LocationRegion"][0]["X"] * 100 + location["LocationRegion"][0]["Y"]
        if position in singleLocationCount:
            singleLocationCount[position] += 1
        else:
            singleLocationCount[position] = 1
        location["LocationName"] += f" X: {location["LocationRegion"][0]["X"]} Y: {location["LocationRegion"][0]["Y"]} #{singleLocationCount[position]}"
    writer = open(Path+file,'w+')
    writer.write(json.dumps(jsonFile,indent=4))
    writer.close()
    singleFile += jsonFile


writer = open(os.path.dirname(os.path.realpath(__file__))+"/MoneyLocations.json",'w+')
writer.write(json.dumps(singleFile,indent=4))
writer.close()

print(len(singleFile))
print(totalAmount)