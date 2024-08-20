import os
import json
import networkx as nx
import matplotlib.pyplot as plt

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
            file = open(self.path+".json")
            self.mapFile = json.load(file)
            file.close()
        except:
            return
        self.currFunc = self.selectRoom

    def loadFiles(self,string):
        print("Enter File Name")
        self.path = string
        try:
            file = open(self.path+".json")
            self.mapFile = json.load(file)
            file.close()
        except:
            return
        self.currFunc = self.selectRoom

    def save(self):
        file = open(self.path+".json","w+")
        file.write(json.dumps(self.mapFile))
        file.close()

    def createGraph(self):
        plt.clf()

        graph = nx.DiGraph()
        if os.path.exists(self.path+".png"):
            os.remove(self.path+".png")

        for room in self.mapFile:
            graph.add_node(f"{room['RoomX']},{room['RoomY']},{room['RoomSection']}",pos=(room['RoomX'],room['RoomY']))
        for room in self.mapFile:
            for con in room["Connection"]:
                if room['Map'] != con['Map']:
                    continue
                graph.add_edge(f"{room['RoomX']},{room['RoomY']},{room['RoomSection']}",f"{con['RoomX']},{con['RoomY']},{con['RoomSection']}")

        post=nx.get_node_attributes(graph,'pos')
        fig = plt.figure(figsize=(32,18))
        nx.spring_layout(graph,pos=post)    
        nx.draw(graph,post,node_size=400,node_shape='s',edgecolors='white',edge_color='white')
        fig.gca().invert_yaxis()
        fig.set_facecolor('black')
        fig.savefig(self.path+".png")
        fig.show()
        

    def selectRoom(self):
        print("You can save here all changes with \"save\"")
        print("Display the Map as a Graph with \"display\"")
        print("Which Room should be edited (x,y,section) <- section can be left empty")
        i = input()
        if i == "back":
            self.currFunc = self.loadFile
            self.path = ""
            return
        if i == "save":
            self.save()
            return
        if i == "display":
            self.createGraph()
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
    def printAllMaps(self):
        for i in range(2,30):
            self.loadFiles("map"+str(i))
            self.createGraph()


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
                self.save()
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
            self.save()
        except:
            pass

    def loop(self):
        while True:
            self.clear()
            print("With \"back\" you can go back to the previous Menu")
            self.currFunc()

a = prog()
#a.printAllMaps()
a.loop()