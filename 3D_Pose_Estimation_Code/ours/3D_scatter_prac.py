import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import numpy as np
from mpl_toolkits.mplot3d import Axes3D

fig = plt.figure()
ax  = fig.add_subplot(111, projection = '3d')

X = [1,2]
Y = [4,9]
Z = [5,1]
ax.plot(X, Y, Z, color = 'b') # 두 점 직전으로 잇기

ax.scatter(X, Y, Z, color = 'r') # 좌표 점 찍기

plt.show()
