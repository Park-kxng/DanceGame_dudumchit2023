import argparse

import cv2
import numpy as np
import pyrealsense2 as rs


def main():
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

    config = rs.config()
    config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 6)
    config.enable_stream(rs.stream.color, width, height, rs.format.bgr8, 30)

    profile = pipeline.start(config)

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

            # (400, 120) 좌표의 depth 값 출력
            x, y = 400, 120
            print(depth_info.get_distance(x, y))

            image = np.asanyarray(color_frame.get_data())
            image = cv2.circle(image, (x, y), 2, (255, 0, 0), -1)

            # Flip the image horizontally for a later selfie-view display, and convert
            # the BGR image to RGB.
            image = cv2.flip(image, 1)

            cv2.imshow('depth value test', image)

            if cv2.waitKey(5) & 0xFF == 27:
                break
    finally:
        pipeline.stop()


if __name__ == "__main__":
    main()
