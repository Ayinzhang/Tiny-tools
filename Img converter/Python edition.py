import os
import glob
import cv2
import numpy as np
    
directory = os.getcwd(); files = glob.glob(directory + "/*")
print("Choose Instruction:\n 0 - Grey\n 1 - Transparent\n 2 - Stroke")
cnt = int(input())
match cnt:
    case 0:
        print("Input if reverse: ")
        cnt = int(input())
        for file in files:
            if file.endswith(".jpg") or file.endswith(".png"):
                src = cv2.imread(file, -1)
                height, width, channel = src.shape
                for i in range(0, height):
                    for j in range(0, width):
                        src[i, j, 0] = src[i, j, 1] = src[i, j, 2] = ((np.uint16(src[i, j, 0]) + src[i, j, 1] + src[i, j, 2]) / 3) if cnt == 0 else (255 - (np.uint16(src[i, j, 0]) + src[i, j, 1] + src[i, j, 2]) / 3)
                cv2.imwrite(file.replace(".jpg", "_new.png").replace(".png", "_new.png"), src)
    case 1:
        print("Input transparent value: ")
        cnt = int(input())
        for file in files:
            if file.endswith(".jpg") or file.endswith(".png"):
                src = cv2.imread(file, -1)
                height, width, channel = src.shape
                dst = np.zeros((height, width, 4), np.uint8)
                for i in range(0, height):
                    for j in range(0, width):
                        dst[i, j] = (255, 255, 255, 0) if int(src[i, j, 0]) + int(src[i, j, 1]) + int(src[i, j, 2]) > cnt else src[i, j] if channel == 4 else (src[i, j, 0], src[i, j, 1], src[i, j, 2], 255)
                cv2.imwrite(file.replace(".jpg", "_new.png").replace(".png", "_new.png"), dst)
    case 2:
        print("Input stroke value: ")
        cnt = int(input())
        for file in files:
            if file.endswith(".jpg") or file.endswith(".png"):
                src = cv2.imread(file, -1)
                height, width, channel = src.shape
                if channel == 4:
                    for i in range(0, height):
                        for j in range(0, width):
                            if src[i, j, 3] == 0:
                                src[i, j, 0] = src[i, j, 1] = src[i, j, 2] = 255
                tmp = np.zeros((height, width), np.uint64)
                dst = np.zeros((height, width, 4), np.uint8)
                for t in range(0, cnt):
                    for i in range(0, height):
                        for j in range(0, width):
                            tmp[i, j] = np.uint64(765) - src[i, j, 0] - src[i, j, 1] - src[i, j, 2] + tmp[i - 1, j] if i > 0 else 0 + tmp[i, j - 1] if j > 0 else 0 - tmp[i - 1, j - 1] if i * j > 0 else 0
                    for i in range(0, height):
                        for j in range(0, width):
                            li = max(i - 1, 0); ri = min(i + 1, height - 1); lj = max(j - 1, 0); rj = min(j + 1, width - 1)
                            num = (tmp[ri, rj] - tmp[li - 1, rj] if li > 0 else 0 + tmp[ri, lj - 1] if lj > 0 else 0
                                         + tmp[li - 1, lj - 1] if li * lj > 0 else 0) / ((ri - li + 1) * (rj - lj + 1))
                            src[i, j, 0] = dst[i, j, 0] = src[i, j, 0] - num if src[i, j, 0] > num else 0
                            src[i, j, 1] = dst[i, j, 1] = src[i, j, 1] - num if src[i, j, 1] > num else 0
                            src[i, j, 2] = dst[i, j, 2] = src[i, j, 2] - num if src[i, j, 2] > num else 0
                    print("Loop " + str(t + 1) + " complete")
                    if t == cnt - 1:
                        for i in range(0, height):
                            for j in range(0, width):
                                if int(dst[i, j, 0]) + int(dst[i, j, 1]) + int(dst[i, j, 2]) < 100:
                                    dst[i, j, 3] = 255
                                elif channel == 4:
                                    dst[i, j, 3] = src[i, j, 3]
                        cv2.imwrite(file.replace(".jpg", "_new.jpg").replace(".png", "_new.png"), dst)
print("Instruction finished")
exit()