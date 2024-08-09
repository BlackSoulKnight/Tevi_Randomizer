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
    currX = -1
    currY = -1
    currSection = -1

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
        print("Which Room should be edited (x,y,section) <- section can be left empty")
        i = input()
        if i == "back":
            self.currFunc = self.loadFile
            self.path = ""
            return
        if i == "save":
            self.save()
            return
        i = i.split(',')
        if len(i) == 2:
            i.append(0)
        try:
            for m in self.mapFile:
                if m["RoomX"] == int(i[0]) and m["RoomY"] == int(i[1]) and m["RoomSection"] == int(i[2]):
                    self.currX = int(i[0])
                    self.currY = int(i[1])
                    self.currSection = int(i[2])
                    self.room = m
                    self.currFunc = self.roomEdit
                    return
        except:
            return

    def roomEdit(self):
        print(f"Current Room X:{self.currX} Y:{self.currY} Section:{self.currSection}")
        print("Enter add to create a new Connection")
        print("Fast Deletion with \"delete x\"")
        for idx,con in enumerate(self.room["Connection"]):
            print(f"Enter {idx+1} to Edit Connection: {con}")
        i = input()
        if i == "back":
            self.currFunc = self.selectRoom
            self.currX = -1    
            self.currY = -1
            self.currSection = -1
            self.room = None
            return
        elif i == "add":
            self.currFunc = self.addConnection
            return
        elif "delete" in i:
            try:
                i = i.split(' ')
                i = int(i[1]) -1 
                del self.room["Connection"][i]
            except:
                pass
            return
        try:
            i = int(i)-1
            if i >= len(self.room["Connection"]):
                return
            self.con = i
            self.currFunc = self.editConnection
        except:
            return
            
    def editConnection(self):
        print("Do you want to \"delete\" Connection or \"edit\" the Method")
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