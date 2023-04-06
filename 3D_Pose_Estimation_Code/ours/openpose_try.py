import cv2
import pyopenpose as op

# OpenPose 모델을 로드합니다.
opWrapper = op.WrapperPython()
params = dict(model_folder='path/to/model/folder')
opWrapper.configure(params)
opWrapper.start()

# 웹캠으로부터 프레임을 읽어옵니다.
cap = cv2.VideoCapture(0)

while True:
    # 프레임을 읽어옵니다.
    ret, frame = cap.read()

    # OpenPose로 포즈 추정을 수행합니다.
    datum = op.Datum()
    datum.cvInputData = frame
    opWrapper.emplaceAndPop([datum])

    # 추정된 포즈를 그립니다.
    op.drawKeypoints(datum.cvOutputData, datum.poseKeypoints, outImage=frame)

    # 결과를 출력합니다.
    cv2.imshow('OpenPose Webcam Demo', frame)

    # ESC 키를 누르면 종료합니다.
    if cv2.waitKey(1) == 27:
        break

# 사용한 자원을 해제합니다.
cap.release()
cv2.destroyAllWindows()
