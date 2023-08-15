using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseDance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
}
