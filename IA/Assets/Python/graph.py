import matplotlib.pyplot as plt
import json
import os, sys

os.chdir(os.path.dirname(os.path.abspath(__file__)))
os.chdir("..")
os.chdir("SaveData")

plt.rcParams.update(plt.rcParamsDefault)
plt.style.use('seaborn-v0_8-dark')

g = []
p = []
t = []
with open("Garden.json") as f:
    d = json.load(f)
    for i in range(len(d)):
        key = str(i)
        if key in d:
            value = d[key]
            g.append((tuple(value)[0] if tuple(value)[0] > 0 else None))
            p.append((tuple(value)[1] if tuple(value)[1] > 0 else None))
            t.append(i)
plt.plot(t, g, 'r-.', t, p, 'g--')
plt.xlabel('Time')
plt.ylabel('Population')
plt.title('Animal and Plant Population Over Time')
plt.legend(['Animal Population (G)', 'Plant Population (P)'])
plt.grid(True)
print(plt.style.available)
plt.show()
