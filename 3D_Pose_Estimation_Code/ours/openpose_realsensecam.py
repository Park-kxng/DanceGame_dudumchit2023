import cv2
from pathlib import Path

import argparse
import numpy as np
import pyrealsense2 as rs

# MPII에서 각 파트 번호, 선으로 연결될 POSE_PAIRS
BODY_PARTS = {"Head": 0, "Neck": 1, "RShoulder": 2, "RElbow": 3, "RWrist": 4,
              "LShoulder": 5, "LElbow": 6, "LWrist": 7, "RHip": 8, "RKnee": 9,
              "RAnkle": 10, "LHip": 11, "LKnee": 12, "LAnkle": 13, "Chest": 14,
              "Background": 15}

POSE_PAIRS = [["Head", "Neck"], ["Neck", "RShoulder"], ["RShoulder", "RElbow"],
              ["RElbow", "RWrist"], ["Neck", "LShoulder"], ["LShoulder", "LElbow"],
              ["LElbow", "LWrist"], ["Neck", "Chest"], ["Chest", "RHip"], ["RHip", "RKnee"],
              ["RKnee", "RAnkle"], ["Chest", "LHip"], ["LHip", "LKnee"], ["LKnee", "LAnkle"]]

# 각 파일 path
BASE_DIR = Path(__file__).resolve().parent
protoFile = str(BASE_DIR) + "/openpose_model/mpi/pose_deploy_linevec_faster_4_stages.prototxt"
weightsFile = str(BASE_DIR) + "/openpose_model/mpi/pose_iter_160000.caffemodel"

# 위의 path에 있는 network 모델 불러오기
net = cv2.dnn.readNetFromCaffe(protoFile, weightsFile)

# 쿠다 사용 안하면 밑에 이미지 크기를 줄이는게 나을 것이다
# net.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA) #벡엔드로 쿠다를 사용하여 속도향상을 꾀한다
# net.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA) # 쿠다 디바이스에 계산 요청

# realsense
# read arguments
parser = argparse.ArgumentParser()
rs_group = parser.add_argument_group("RealSense")
rs_group.add_argument("--resolution", default=[640, 480], type=int, nargs=2, metavar=('width', 'height'),
                      help="Resolution of the realsense stream.")
rs_group.add_argument("--fps", default=30, type=int,
                      help="Framerate of the realsense stream.")

args = parser.parse_args()

# create realsense pipeline
pipeline = rs.pipeline()

width, height = args.resolution


# 	config () API 참고 링크 : https://intelrealsense.github.io/librealsense/doxygen/classrs2_1_1config.html
# 	void 	enable_stream (rs2_stream stream_type, int width, int height, rs2_format format=RS2_FORMAT_ANY, int framerate=0)
#  framerate = FPS 초당 프레임수를 나타냄. FPS 6으로 설정되어 있길래 30으로 바꾸었으나 여전히 느림
config = rs.config()
config.enable_stream(rs.stream.depth, width, height, rs.format.z16, 30)
config.enable_stream(rs.stream.color, width, height, rs.format.bgr8, 30)

profile = pipeline.start(config)

# align class 참고 링크: https://intelrealsense.github.io/librealsense/python_docs/_generated/pyrealsense2.align.html#pyrealsense2.align
# 깊이(depth) 이미지와 다른 이미지를 정렬해야 한다

# 아래 align_to에 다른 스트림 타입을 대입하여 정렬을 하는 과정
align_to = rs.stream.color
align = rs.align(align_to)


try:
    while True:
        frames = pipeline.wait_for_frames()
        aligned_frames = align.process(frames)
        depth_frame = aligned_frames.get_depth_frame()
        color_frame = aligned_frames.get_color_frame()
        depth_info = depth_frame.as_depth_frame()

        if not color_frame:
            break
        image = np.asanyarray(color_frame.get_data())

        # Flip the image horizontally for a later selfie-view display, and convert
        # the BGR image to RGB.
        #image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)

        # 여러 개의 영상을 추론할 때, 아래 함수로 여러 개의 블롭 객체를 받아 사용
        # 참고 링크: https://deep-learning-study.tistory.com/299
        # cv2.dnn.blobFromImage(image, scalefactor=None, size=None, mean=None, swapRB=None, crop=None, ddepth=None) -> retval
        # • image: 입력 영상
        # • scalefactor: 입력 영상 픽셀 값에 곱할 값. 기본값은 1.
        #  => 딥러닝 학습을 진행할 때 입력 영상을 0~255 픽셀값을 이용했는지, 0~1로 정규화해서 이용했는지에 맞게 지정
        #      0~1로 정규화하여 학습을 진행했으면 1/255를 입력
        # • size: 출력 영상의 크기. 기본값은 (0, 0).
        # • mean: 입력 영상 각 채널에서 뺄 평균 값. 기본값은 (0, 0, 0, 0).
        # • swapRB: R과 B 채널을 서로 바꿀 것인지를 결정하는 플래그. 기본값은 False.
        # • crop: 크롭(crop) 수행 여부. 기본값은 False.
        # • ddepth: 출력 블롭의 깊이. CV_32F 또는 CV_8U. 기본값은 CV_32F.
        # • retval: 영상으로부터 구한 블롭 객체. numpy.ndarray. shape=(N,C,H,W). dtype=numpy.float32.
        inpBlob = cv2.dnn.blobFromImage(image, 1.0 / 255, (width, height), (0, 0, 0), swapRB=False)

        imgb = cv2.dnn.imagesFromBlob(inpBlob)
        # inpBlob: 신경망을 통과할 이미지
        # cv2.imshow("motion",(imgb[0]*255.0).astype(np.uint8))

        # network에 넣어주기
        net.setInput(inpBlob)

        # 결과 받아오기
        output = net.forward()

        # 키포인트 검출시 이미지에 그려줌
        points = []
        for i in range(0, 15):
            # 해당 신체부위 신뢰도 얻음.
            probMap = output[0, i, :, :]

            # global 최대값 찾기
            minVal, prob, minLoc, point = cv2.minMaxLoc(probMap)

            # 원래 이미지에 맞게 점 위치 변경
            x = (width * point[0]) / output.shape[3]
            y = (height * point[1]) / output.shape[2]

            # 키포인트 검출한 결과가 0.1보다 크면(검출한곳이 위 BODY_PARTS랑 맞는 부위면) points에 추가, 검출했는데 부위가 없으면 None으로
            if prob > 0.1:
                cv2.circle(image, (int(x), int(y)), 3, (0, 255, 255), thickness=-1,
                           lineType=cv2.FILLED)  # circle(그릴곳, 원의 중심, 반지름, 색)
                cv2.putText(image, "{}".format(i), (int(x), int(y)), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255),
                            1,
                            lineType=cv2.LINE_AA)
                points.append((int(x), int(y)))
            else:
                points.append(None)

        # 각 POSE_PAIRS별로 선 그어줌 (머리 - 목, 목 - 왼쪽어깨, ...)
        for pair in POSE_PAIRS:
            partA = pair[0]  # Head
            partA = BODY_PARTS[partA]  # 0
            partB = pair[1]  # Neck
            partB = BODY_PARTS[partB]  # 1

            # partA와 partB 사이에 선을 그어줌 (cv2.line)
            if points[partA] and points[partB]:
                cv2.line(image, points[partA], points[partB], (0, 255, 0), 2)

        cv2.imshow("Output-Keypoints", image)


        #image = np.asanyarray(color_frame.get_data())
        #image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)


        if cv2.waitKey(5) & 0xFF == 27:
            break
finally:
    pipeline.stop()
    cv2.destroyAllWindows()  # 모든 윈도우 창 닫음