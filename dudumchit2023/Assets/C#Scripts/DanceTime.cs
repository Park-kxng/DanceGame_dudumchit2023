using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceTime : MonoBehaviour
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
        //4:춤추기에서 5:게임 결과 으로 이동하는 함수
        Debug.Log("다음 단계로 넘어갑니다 -> 5번 화면으로");
        SceneManager.LoadScene("5_DanceComplete");
    }
    public void onClickStepPre()
    {

        //4:춤추기에서 2번 :음악고르기로 이동하는 함수
        Debug.Log("이전 단계로 돌아갑니다 -> 3번 화면으로");
        SceneManager.LoadScene("3_ChooseDance");
    }
}
