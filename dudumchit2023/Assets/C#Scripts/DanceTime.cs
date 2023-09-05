using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // File Ŭ������ ����ϱ� ���� ���ӽ����̽� �߰�
using UnityEditor;

public class DanceTime : MonoBehaviour
{
    private GameObject fbxModel; // FBX �� �ν��Ͻ�
    private Animator animator; // Animator ������Ʈ
    public Vector3 newPosition = new Vector3(500f, 854f, 613f);  // ���ϴ� ��ġ�� ���� (X, Y, Z)
    public Vector3 newSize = new Vector3(200f, 200f, 200f); // ���ϴ� ������
    // Start is called before the first frame update
    void Start()
    {
        playFBX();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickStepNext()
    {
        //4:���߱⿡�� 5:���� ��� ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 5�� ȭ������");
        SceneManager.LoadScene("5_DanceComplete");
    }
    public void onClickStepPre()
    {

        //4:���߱⿡�� 2�� :���ǰ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� ���ư��ϴ� -> 3�� ȭ������");
        SceneManager.LoadScene("3_ChooseDance");
    }

    public void playFBX()
    {
        string fbxFilePath = "sample1"; // Resources ���� FBX ���� �̸� (Ȯ���� ����)

        // Resources���� FBX ���� �ε�
        GameObject fbxModelPrefab = Resources.Load<GameObject>(fbxFilePath);

        if (fbxModelPrefab != null)
        {
            // ������ FBX ���� ���� (�ߺ� ���� ����)
            if (fbxModel != null)
            {
                Destroy(fbxModel);
            }

            // FBX �� �ν��Ͻ� ����
            fbxModel = Instantiate(fbxModelPrefab);

            // FBX �� ��ġ �� ������ ���� 
            fbxModel.transform.position = newPosition;
            fbxModel.transform.localScale = newSize;

            // FBX �𵨿� Animator ������Ʈ �߰�
            animator = fbxModel.AddComponent<Animator>();

            // �ִϸ��̼� Ŭ���� ������ �� �ִ��� Ȯ��

            AnimationClip animationClip = Resources.Load<AnimationClip>("sample1");

            if (animationClip != null)
            {
                // Animator ������Ʈ�� �ִϸ��̼� Ŭ�� �Ҵ�
                animator.runtimeAnimatorController = RuntimeAnimatorController.Instantiate(Resources.Load<RuntimeAnimatorController>("sample1_animation_controller"));

                // �ִϸ��̼� ���
                animator.Play(animationClip.name);
            }
            else
            {
                Debug.LogError("Animation clip not found in Resources folder.");
            }
        }
        else
        {
            Debug.LogError("FBX model not found in Resources folder.");
        }
    }
}
