import matplotlib.pyplot as plt
import json
import os, sys

os.chdir(os.path.dirname(os.path.abspath(__file__)))
os.chdir("..")
os.chdir("SaveData")

g = []
p = []
t = []
with open("Garden.json") as f:
    d = json.load(f)
    for i in range(len(d)):
        key = str(i)
        if key in d:
            value = d[key]
            g.append(tuple(value)[0])
            p.append(tuple(value)[1])
            t.append(i)
plt.plot(t, g, 'r-.', t, p, 'g--')
plt.show()