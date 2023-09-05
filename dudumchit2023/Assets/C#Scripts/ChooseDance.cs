using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // File Ŭ������ ����ϱ� ���� ���ӽ����̽� �߰�
using UnityEditor;

public class ChooseDance : MonoBehaviour
{
    private GameObject fbxModel; // FBX �� �ν��Ͻ�
    private Animator animator; // Animator ������Ʈ

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
        //3��:�� ���ÿ��� 4:���߱� ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 4�� ȭ������");
        SceneManager.LoadScene("4_DanceTime");
    }
    public void onClickStepPre()
    {

        //3��:�� ���ÿ��� 2�� :���ǰ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� ���ư��ϴ� -> 2�� ȭ������");
        SceneManager.LoadScene("2_ChooseMusic");
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
            fbxModel.transform.position = Vector3.zero; // ���ϴ� ��ġ�� �̵�

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
