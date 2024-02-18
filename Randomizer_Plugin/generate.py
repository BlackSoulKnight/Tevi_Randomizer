import random
import os

#example Randomizer

class t:
    def __init__(self):
        self.output=""
        self.placeditem = []
        self.originalitems = []

    output = ""
    placeditem = []
    originalitems = []

    def randomitem(self):
        created = False

        while(not created):
            item = [0,0]
            val = random.randint(1,378)
            if val < 9:
                if val != 7:
                    shopfix = random.randint(0,34)
                    if shopfix>29:
                        shopfix+=10
                    item =[val,shopfix]
                else:
                    item =[val,random.randint(0,4)]

            elif val >21 and val < 33 and (not val == 29 and not val==28 and not val== 27 and not val == 31):
                item = [val,random.randint(4,6)]
            elif val == 35:
                item = [val,random.randint(4,6)]
            elif val == 41 or val == 42:
                item = [val,random.randint(4,6)]
            elif val == 47 or val == 48:
                item = [val,random.randint(4,6)]
            elif val >= 52 and val <=54:
                item = [val,random.randint(4,6)]
            elif val == 57 or val == 60 or val == 63:
                item = [val,random.randint(4,6)]

            elif (val >21 and val < 67):
                item = [val,1]

            elif val > 119 and val <378:
                item = [val,1]
            else:
                continue
            if not(item in self.placeditem):
                self.placeditem.append(item)
                created = True
                
        return item 

    def addToOutput(self,itemId1,slotId1,itemId2,slotId2):
        self.output+= str(itemId1)+","+str(slotId1)+"   :   "+str(itemId2)+","+str(slotId2)+";\n"

    def createstuff(self):
        self.addToOutput(22,1,22,4)
        self.addToOutput(23,1,23,4)
        self.addToOutput(24,1,24,4)
        for i in range(377,0,-1):
            #Upgradeable item shuffle
            if i >21 and i < 33 and (not i == 29 and not i==28 and not i== 27 and not i == 31):
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue
            if i == 35:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue
            if i == 41 or i == 42:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])          
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue
            if i == 47 or i == 48:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue
            if i >= 52 and i <=54:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue
            if i == 57 or i == 60 or i == 63:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,2,item[0],item[1])
                item = self.randomitem()
                self.addToOutput(i,3,item[0],item[1])
                continue

            #well i was lazy
            if i > 21 and i < 67:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
            if i >119 and  i < 378:
                item = self.randomitem()
                self.addToOutput(i,1,item[0],item[1])
            if i < 9 and i != 7:
                for x in range(0,35):
                    item = self.randomitem()
                    self.addToOutput(i,x,item[0],item[1])
            if i == 7:
                for x in range(30,35):
                    item = self.randomitem()
                    self.addToOutput(i,x,item[0],item[1])
        self.output = self.output[:-2]

random.seed()

meh = t()
meh.createstuff()
if not os.path.exists("./data"):
    os.makedirs("./data")
file = open("data/file.dat",'w+')
file.write(meh.output)
file.close()