using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseMusic : MonoBehaviour
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
        //2번:음악 고르기에서 3번: 춤 선택 으로 이동하는 함수
        Debug.Log("다음 단계로 넘어갑니다 -> 3번 화면으로");
        SceneManager.LoadScene("3_ChooseDance");
    }
    public void onClickStepPre()
    {

        //2번:음악 고르기 에서 1번 :메인 메뉴로 이동하는 함수
        Debug.Log("이전 단계로 돌아갑니다 -> 1번 화면으로");
        SceneManager.LoadScene("1_MainMenu");
    }
}
