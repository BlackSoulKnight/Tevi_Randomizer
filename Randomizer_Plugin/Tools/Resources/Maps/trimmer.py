import os
import json

class prog:
    currFunc = ()
    def __init__(self) -> None:
        self.currFunc = self.loadFile

    def clear(self):
        os.system('cls') or None

    mapFile = []
    room = []
    con = None
    path = ""
    def loadFile(self):
        print("Enter File Name")
        self.path = input()
        try:
            file = open(self.path)
            self.mapFile = json.load(file)
            file.close()
        except:
            return
        self.currFunc = self.selectRoom

    def save(self):
        file = open(self.path,"w+")
        file.write(json.dumps(self.mapFile))
        file.close()

    def selectRoom(self):
        print("You can save here all changes with \"save\"")
        print("Which Room should be edited (x,y,section)")
        i = input()
        if i == "back":
            self.currFunc = self.loadFile
            self.path = ""
            return
        if i == "save":
            self.save()
            return
        i = i.split(',')
        try:
            for m in self.mapFile:
                if m["RoomX"] == int(i[0]) and m["RoomY"] == int(i[1]) and m["RoomSection"] == int(i[2]):
                    self.room = m
                    self.currFunc = self.roomEdit
                    return
        except:
            return

    def roomEdit(self):
        print("Enter add to create a new Connection")
        for idx,con in enumerate(self.room["Connection"]):
            print(f"Enter {idx} to Edit Connection: {con}")
        i = input()
        if i == "back":
            self.currFunc = self.selectRoom
            self.room = None
            return
        elif i == "add":
            self.currFunc = self.addConnection
            return
        try:
            i = int(i)
            if i >= len(self.room["Connection"]):
                return
            self.con = i
            self.currFunc = self.editConnection
        except:
            return
            
    def editConnection(self):
        print("Do you want to \"add\" or \"delete\" Connection or \"edit\" the Method")
        i = input()
        if i == "delete":
            del self.room["Connection"][self.con]
            self.currFunc = self.roomEdit
        elif i == "back":
            self.currFunc = self.roomEdit
            self.con = -1


    def addConnection(self):
        print("To add a Connection enter in the following stlye (map,x,y,section)")
        i = input()
        if i == "back":
            self.currFunc = self.roomEdit
            return
        try:
            i = i.split(',')
            newCon = {"Map":int(i[0]),"RoomX":int(i[1]),"RoomY":int(i[2]),"RoomSection":int(i[3]),"Method":[]}
            self.room["Connection"].append(newCon)
            self.currFunc = self.roomEdit
        except:
            pass

    def loop(self):
        while True:
            self.clear()
            print("With \"back\" you can go back to the previous Menu")
            self.currFunc()

a = prog()
a.loop()