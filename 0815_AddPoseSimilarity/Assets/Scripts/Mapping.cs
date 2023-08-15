using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System;
//using RootMotion.FinalIK; // Full Body Biped IK ���ӽ����̽�

public class Mapping : MonoBehaviour
{
    //public FullBodyBipedIK ik; // Full Body Biped IK ������Ʈ�� ������ ����


    // ���� �ּҿ� ��Ʈ��ȣ
    private string SERVER_IP = "127.0.0.1";
    private int SERVER_PORT = 9999;
    // ������ ����Ǿ� �ִ��� ���θ� �����ϴ� ����
    private bool isConnected = false;

    // TcpClient ����
    private TcpClient client;

    // NetworkStream ����
    private NetworkStream stream;

    // Thread ����
    private Thread receiveThread;

    // landmarks �����͸� �����ϴ� �迭
    private float[] landmarks;

    // landmarks�� ������ GameObject
    public GameObject targetObject;

    // landmarks�� ������ JointType
    //public ArticulationJointType[] jointTypes;
  

    //public GameObject[] Body;
    //List<string> lines;
    //int counter = 0;
    // landmarks �����͸� �����ϴ� ����Ʈ
    //private List<float> landmarksList = new List<float>();
    private List<Vector3> landmarksList = new List<Vector3>(); // 33�� joint ����
    private List<Vector3> landmarksList_before = new List<Vector3>(); // 33�� joint ����
    private object landmarksLock = new object(); // ����ȭ ��ü
    public float rotationSpeed = 180; // ȸ�� �ӵ��� ������ ���� // 180��/�ʷ� ȸ�� �ӵ� ����
    public float temp = 1;

    // ĳ������ Animator ������Ʈ�� ���� ����
    public Animator animator;


    // ���帶ũ�� �� �ε����� ����
    public int[] landmarkToBoneIndexMap = new int[]
    { // 0~ 10���� �󱼿� �ִ� ���帶ũ�� �ѱ�� ���� 999
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
         999, // 29.left_heel �޲�ġ�� ����
         999, // 30. right_heel
      (int)HumanBodyBones.LeftToes, // 31.left_foot_index
       (int)HumanBodyBones.RightToes // 32. right_foot_index
        };



    public GameObject[] Body;
    public GameObject[] Body2;
    

    // Start is called before the first frame update
    void Start()
    {
        // TcpClient ��ü ����
        client = new TcpClient();

        // ������ ����
        client.Connect(SERVER_IP, SERVER_PORT);

        // NetworkStream ��ü ����
        stream = client.GetStream();

        // ���� ���� ����
        isConnected = true;
        Debug.Log("�����");

        // Thread ���� �� ����
        receiveThread = new Thread(new ThreadStart(ReceiveThread));
        receiveThread.Start();

        // landmarksList_before�� 0,0,0�� Vector3�� �ʱ�ȭ
        for (int i = 0; i < 33; i++)
        {
            landmarksList_before.Add(Vector3.zero);
        }
        // Full Body Biped IK ������Ʈ�� ����
        //ik = targetObject.GetComponent<FullBodyBipedIK>();


    }


    // Update is called once per frame
    void Update()
    {
        // landmarksList_before�� landmarksList�� ȸ���� ����Ͽ� ����
        for (int i = 0; i < landmarksList_before.Count; i++)
        {
            if (landmarkToBoneIndexMap[i] != 999)
            {
                

                // �ٸ� Joint�� ������ ������� ���� ����

                //Vector3 fromDirection = landmarksList_before[i];
                // i�� �ش��ϴ� landmark�� �� �ε��� ����
                int boneIndex = landmarkToBoneIndexMap[i];
                // �ش� ���� Transform�� ������ ���� ��ġ ������
                Transform boneTransform = animator.GetBoneTransform((HumanBodyBones)boneIndex);

                Vector3 fromDirection = boneTransform.position; // ���� ���� ��ǥ
                Vector3 toDirection = landmarksList[i];

                // ���� ���� ���� ���
                float angle = Vector3.Angle(fromDirection, toDirection);

                
                // �� ���帶ũ ��ġ ������ ȸ�� ���� �����Ͽ� ����
                Quaternion rotation = Quaternion.Slerp(Quaternion.LookRotation(fromDirection), Quaternion.LookRotation(toDirection), 1);

                if (boneTransform != null)
                {
                    // �� �������� ������ ȸ�� ������ŭ ȸ�� ����
                    boneTransform.localRotation = Quaternion.RotateTowards(boneTransform.localRotation, rotation, rotationSpeed * Time.deltaTime);
                    //boneTransform.localPosition = landmarksList[i];
                }
                if (i ==23 || i ==25)
                { // ���� ���� ȸ���� �����ɴϴ�.
                    Vector3 currentRotation = boneTransform.localRotation.eulerAngles;

                    // x ���� -180���� ȸ���մϴ�.
                    currentRotation.z = +180f;

                    // ����� ȸ������ �����մϴ�.
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

        



        // ��Ʈ ���� Transform�� ������
        Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
        Vector3 leftHip = landmarksList[23];
        Vector3 rightHip = landmarksList[24];
        // �� ����� �߰� ������ ����Ͽ� hip�� ��ġ�� ���
        Vector3 hipPosition = (leftHip + rightHip) / 2.0f;
        // ��Ʈ ���� ��ġ�� �� ��ġ�� ����
        hipsTransform.position = hipPosition;
        //targetObject.transform.position = hipPosition;
        

        // landmarksList�� �����͸� landmarksList_before�� ���� ����
        lock (landmarksLock)
        {
            landmarksList_before.Clear(); // landmarksList_before �ʱ�ȭ
            foreach (Vector3 landmark in landmarksList)
            {
                Vector3 copiedLandmark = new Vector3(landmark.x, landmark.y, landmark.z);
                landmarksList_before.Add(copiedLandmark);
            }
        }

       
    }




    // ���� ������ �Լ�
    void ReceiveThread()
    {
        // ���� ���°� �����Ǵ� ���� �ݺ�
        while (isConnected)
        {
            try
            {
                // ���� ������ �б�
                byte[] buffer = new byte[4096];
                int nbytes = stream.Read(buffer, 0, buffer.Length);
                if (nbytes > 0)
                {
                    // ������ �����͸� float �迭�� ��ȯ�Ͽ� landmarks�� �߰�
                    string landmarksStr = System.Text.Encoding.Default.GetString(buffer);
                    string[] landmarkStrArr = landmarksStr.Split(',');
                    // landmarksList ������ lock���� ��ȣ
                    lock (landmarksLock)
                    {
                        landmarksList.Clear(); // �ʱ�ȭ
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
