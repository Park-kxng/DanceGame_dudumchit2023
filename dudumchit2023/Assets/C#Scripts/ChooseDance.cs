using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // File 클래스를 사용하기 위해 네임스페이스 추가
using UnityEditor;

public class ChooseDance : MonoBehaviour
{
    private GameObject fbxModel; // FBX 모델 인스턴스
    private Animator animator; // Animator 컴포넌트

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
        //3번:춤 선택에서 4:춤추기 으로 이동하는 함수
        Debug.Log("다음 단계로 넘어갑니다 -> 4번 화면으로");
        SceneManager.LoadScene("4_DanceTime");
    }
    public void onClickStepPre()
    {

        //3번:춤 선택에서 2번 :음악고르기로 이동하는 함수
        Debug.Log("이전 단계로 돌아갑니다 -> 2번 화면으로");
        SceneManager.LoadScene("2_ChooseMusic");
    }

    public void playFBX()
    {
        string fbxFilePath = "sample1"; // Resources 내의 FBX 파일 이름 (확장자 제외)

        // Resources에서 FBX 모델을 로드
        GameObject fbxModelPrefab = Resources.Load<GameObject>(fbxFilePath);

        if (fbxModelPrefab != null)
        {
            // 기존의 FBX 모델을 삭제 (중복 생성 방지)
            if (fbxModel != null)
            {
                Destroy(fbxModel);
            }

            // FBX 모델 인스턴스 생성
            fbxModel = Instantiate(fbxModelPrefab);
            fbxModel.transform.position = Vector3.zero; // 원하는 위치로 이동

            // FBX 모델에 Animator 컴포넌트 추가
            animator = fbxModel.AddComponent<Animator>();

            // 애니메이션 클립을 가져올 수 있는지 확인
           
            AnimationClip animationClip = Resources.Load<AnimationClip>("sample1");

            if (animationClip != null)
            {
                // Animator 컴포넌트에 애니메이션 클립 할당
                animator.runtimeAnimatorController = RuntimeAnimatorController.Instantiate(Resources.Load<RuntimeAnimatorController>("sample1_animation_controller"));

                // 애니메이션 재생
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
