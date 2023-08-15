using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;
//using RootMotion.FinalIK; // Full Body Biped IK 네임스페이스

public class Mapping : MonoBehaviour
{
    //public FullBodyBipedIK ik; // Full Body Biped IK 컴포넌트를 저장할 변수


    // 서버 주소와 포트번호
    private string SERVER_IP = "127.0.0.1";
    private int SERVER_PORT = 9999;
    // 서버와 연결되어 있는지 여부를 저장하는 변수
    private bool isConnected = false;

    // TcpClient 변수
    private TcpClient client;

    // NetworkStream 변수
    private NetworkStream stream;

    // Thread 변수
    private Thread receiveThread;

    // landmarks 데이터를 저장하는 배열
    private float[] landmarks;

    // landmarks를 적용할 GameObject
    public GameObject targetObject;

    // landmarks를 적용할 JointType
    //public ArticulationJointType[] jointTypes;
  

    //public GameObject[] Body;
    //List<string> lines;
    //int counter = 0;
    // landmarks 데이터를 저장하는 리스트
    //private List<float> landmarksList = new List<float>();
    private List<Vector3> landmarksList = new List<Vector3>(); // 33개 joint 저장
    private List<Vector3> landmarksList_before = new List<Vector3>(); // 33개 joint 저장
    private object landmarksLock = new object(); // 동기화 객체
    public float rotationSpeed = 180; // 회전 속도를 조절할 변수 // 180도/초로 회전 속도 설정
    public float temp = 1;

    // 캐릭터의 Animator 컴포넌트에 대한 참조
    public Animator animator;


    // 랜드마크와 본 인덱스의 매핑
    public int[] landmarkToBoneIndexMap = new int[]
    { // 0~ 10까지 얼굴에 있는 랜드마크는 넘기기 위해 999
        999, 999, 999, 999, 999,
         999, 999, 999, 999, 999,  999,
   (int)(int)HumanBodyBones.LeftUpperArm,  //  11. left_shoulder
    (int)HumanBodyBones.RightUpperArm, // 12. right_shoulder
     (int)HumanBodyBones.LeftLowerArm,// 13. left_elbow
    (int)HumanBodyBones.RightLowerArm, // 14. right_elbow
    (int)HumanBodyBones.LeftHand,// 15.left_wrist
   (int)HumanBodyBones.RightHand,   // 16. right_wrist

     999, // (int)HumanBodyBones.LeftLittleDistal,   // 17.left_pinky
    999, // (int)HumanBodyBones.RightLittleDistal,   // 18. right_pinky
     999, //(int)HumanBodyBones.LeftIndexDistal, //19. left_index
     999, //(int)HumanBodyBones.RightIndexDistal, // 20.  right_index
      999, // (int)HumanBodyBones.LeftThumbDistal, // 21. left_thumb
      999, // (int)HumanBodyBones.RightThumbDistal, // 22. right_thumb

     (int)HumanBodyBones.LeftUpperLeg, // 23. left_hip ////
  (int)HumanBodyBones.RightUpperLeg, // 24. right_hip ////
    (int)HumanBodyBones.LeftLowerLeg, // 25.  left_knee
    (int)HumanBodyBones.RightLowerLeg, //   26. right_knee
       (int)HumanBodyBones.LeftFoot, // 27. left_ankle
     (int)HumanBodyBones.RightFoot, // 28. right_ankle
         999, // 29.left_heel 뒷꿈치도 없음
         999, // 30. right_heel
      (int)HumanBodyBones.LeftToes, // 31.left_foot_index
       (int)HumanBodyBones.RightToes // 32. right_foot_index
        };



    public GameObject[] Body;
    public GameObject[] Body2;
    

    // Start is called before the first frame update
    void Start()
    {
        // TcpClient 객체 생성
        client = new TcpClient();

        // 서버와 연결
        client.Connect(SERVER_IP, SERVER_PORT);

        // NetworkStream 객체 생성
        stream = client.GetStream();

        // 연결 상태 변경
        isConnected = true;
        Debug.Log("연결됨");

        // Thread 생성 및 실행
        receiveThread = new Thread(new ThreadStart(ReceiveThread));
        receiveThread.Start();

        // landmarksList_before를 0,0,0인 Vector3로 초기화
        for (int i = 0; i < 33; i++)
        {
            landmarksList_before.Add(Vector3.zero);
        }
        // Full Body Biped IK 컴포넌트를 연결
        //ik = targetObject.GetComponent<FullBodyBipedIK>();


    }


    // Update is called once per frame
    void Update()
    {
        // landmarksList_before와 landmarksList의 회전을 계산하여 적용
        for (int i = 0; i < landmarksList_before.Count; i++)
        {
            if (landmarkToBoneIndexMap[i] != 999)
            {
                

                // 다른 Joint도 유사한 방식으로 적용 가능

                //Vector3 fromDirection = landmarksList_before[i];
                // i에 해당하는 landmark와 본 인덱스 매핑
                int boneIndex = landmarkToBoneIndexMap[i];
                // 해당 본의 Transform을 가져와 현재 위치 가져옴
                Transform boneTransform = animator.GetBoneTransform((HumanBodyBones)boneIndex);

                Vector3 fromDirection = boneTransform.position; // 본의 월드 좌표
                Vector3 toDirection = landmarksList[i];

                // 벡터 간의 각도 계산
                float angle = Vector3.Angle(fromDirection, toDirection);

                
                // 각 랜드마크 위치 사이의 회전 값을 보간하여 구함
                Quaternion rotation = Quaternion.Slerp(Quaternion.LookRotation(fromDirection), Quaternion.LookRotation(toDirection), 1);

                if (boneTransform != null)
                {
                    // 각 루프에서 고정된 회전 각도만큼 회전 적용
                    boneTransform.localRotation = Quaternion.RotateTowards(boneTransform.localRotation, rotation, rotationSpeed * Time.deltaTime);
                    //boneTransform.localPosition = landmarksList[i];
                }
                if (i ==23 || i ==25)
                { // 현재 로컬 회전을 가져옵니다.
                    Vector3 currentRotation = boneTransform.localRotation.eulerAngles;

                    // x 축을 -180도로 회전합니다.
                    currentRotation.z = +180f;

                    // 변경된 회전값을 적용합니다.
                    boneTransform.localRotation = Quaternion.Euler(currentRotation);

                }

               
            

        }
            /*
        lock (this)
            {
                for ( i = 0; i < Body.Length && i < landmarksList.Count; i++)
                {
                    if (Body[i] != null)
                    {
                        //Body[i].transform.position = landmarksList[i];
                        Body[i].transform.localPosition = landmarksList[i];
                       

                    }
                    //if (Body2[i] != null)    {  Body2[i].transform.position = landmarksList[i]; }
                }
            }
            */
            
        }

        



        // 루트 본의 Transform을 가져옴
        Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
        Vector3 leftHip = landmarksList[23];
        Vector3 rightHip = landmarksList[24];
        // 두 골반의 중간 지점을 계산하여 hip의 위치로 사용
        Vector3 hipPosition = (leftHip + rightHip) / 2.0f;
        // 루트 본의 위치를 새 위치로 설정
        hipsTransform.position = hipPosition;
        //targetObject.transform.position = hipPosition;
        

        // landmarksList의 데이터를 landmarksList_before로 깊은 복사
        lock (landmarksLock)
        {
            landmarksList_before.Clear(); // landmarksList_before 초기화
            foreach (Vector3 landmark in landmarksList)
            {
                Vector3 copiedLandmark = new Vector3(landmark.x, landmark.y, landmark.z);
                landmarksList_before.Add(copiedLandmark);
            }
        }

       
    }




    // 수신 쓰레드 함수
    void ReceiveThread()
    {
        // 연결 상태가 유지되는 동안 반복
        while (isConnected)
        {
            try
            {
                // 수신 데이터 읽기
                byte[] buffer = new byte[4096];
                int nbytes = stream.Read(buffer, 0, buffer.Length);
                if (nbytes > 0)
                {
                    // 수신한 데이터를 float 배열로 변환하여 landmarks에 추가
                    string landmarksStr = System.Text.Encoding.Default.GetString(buffer);
                    string[] landmarkStrArr = landmarksStr.Split(',');
                    // landmarksList 접근을 lock으로 보호
                    lock (landmarksLock)
                    {
                        landmarksList.Clear(); // 초기화
                        for (int i = 0; i < landmarkStrArr.Length - 1; i += 3)
                        {
                            float x = float.Parse(landmarkStrArr[i]);
                            float y = float.Parse(landmarkStrArr[i + 1]);
                            float z = float.Parse(landmarkStrArr[i + 2]);
                            landmarksList.Add(new Vector3((float)(x * temp), (float)(-y * temp), (float)(z * temp)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                break;
            }
        }
    }
}
