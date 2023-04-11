# SMPL to FBX
![](Imgs/teaser.gif)  
**춤 실행 결과 .npy 파일을 본 코드를 통해 .fbx 파일로 변환한 결과는 두둠칫 드라이브 data/NPYtoFBX/outputs 에 올려두었습니다**
## 참고 링크
1. https://github.com/softcat477/SMPL-to-FBX
2. https://github.com/google-research/mint/issues/39#issuecomment-1173681795
**본 readme는 1번 GitHub readme에서 일부 수정하였습니다**
**2번을 참고하여 SmplObject.py 와 PathFilter.py 코드를 일부 수정하였습니다**
## Steps
1. Install [Python FBX](https://download.autodesk.com/us/fbx/20112/fbx_sdk_help/index.html?url=WS1a9193826455f5ff453265c9125faa23bbb5fe8.htm,topicNumber=d0e8312).
1. Download the [SMPL fbx model](https://smpl.is.tue.mpg.de) for unity. Keep the male model `SMPL_m_unityDoubleBlends_lbs_10_scale5_207_v1.0.0.fbx`  
**두둠칫 드라이브 data/fbx/ 에서 다운받을 수 있습니다**.
2. `pip install -r requirements.txt`
3. Here's the file structure:
    ```
    <root>
    |--Convert.py
    |--SmplObject.py
    |--FbxReadWriter.py
    |--PathFilter.py
    |--<fbx_path>/
    |  |--SMPL_m_unityDoubleBlends_lbs_10_scale5_207_v1.0.0.fbx
    |--<npy_path>/
    |  |--*.npy
    |--<output_path>/
    ```
4. `python3 Convert.py --input_pkl_base <pkl_path> --fbx_source_path <fbx_path>/SMPL_m_unityDoubleBlends_lbs_10_scale5_207_v1.0.0.fbx --output_base <output_path>` to start converting.
## npy 파일 구조
* 24개의 각 조인트마다 3X3 로드리게스 회전 행렬
* 글로벌 위치 좌표 (x,y,z)
* 6개의 패딩 값
* 24 X 9 + 3 + 6 = 225 차원의 넘파이 배열
![](Imgs/npy_structure.jpg)
* `Dict["smpl_poses"]` : A `(N, 72)` ndarray, where `N` is the frame number.
    * Joint order: 
        ```
        0:Pelves
        1:L hip
        2:R hip"
        3:Spine1"
        4:L_Knee"
        5:R_Knee"
        6:Spine2"
        7:L_Ankle"
        8:R_Ankle"
        9:Spine3"
        10:L_Foot"
        11:R_Foot"
        12:Neck"
        13:L_Collar"
        14:R_Collar"
        15:Head"
        16:L_Shoulder"
        17:R_Shoulder"
        18:L_Elbow"
        19:R_Elbow"
        20:L_Wrist"
        21:R_Wrist"
        22:L_Hand"
        23:R_Hand"
        ```
    * `72` is from `24joints*3`; `3` is the dimension for the [rotation vector](https://en.wikipedia.org/wiki/Axis%E2%80%93angle_representation) of each joint.
* `Dict["smpl_trans"]`: A `(N, 3)` ndarray. The translation of the Pelvis in each frame.
* Basically following the [AIST++](https://google.github.io/aistplusplus_dataset/factsfigures.html) naming rule. The code is designed specifically to convert the AIST++ dance motions into fbx files.
## Appendix
### I got a translation vector `[d0, d1, d2]` of a frame. How do I assign each dimension to the correct axis?
Follow this order:![](Imgs/global_axis.jpg)

### Bones
```python
bones=[(0,1), (1,4), (4,7), (7,10), # R leg
       (0,2), (2,5), (5,8), (8,11), # L leg
       (0,3), (3,6), (6,9), # Spine
       (9,12), (12,15), # Head
       (9,13), (13,16), (16,18), (18,20), (20,22), # R arm
       (9,14), (14,17), (17,19), (19,21), (21,23)] # L arm
```
