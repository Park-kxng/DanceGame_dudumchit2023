using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceComplete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickHome()
    {
        // 5:게임 결과에서 1:메인화면으로 이동하는 함수
        Debug.Log("다음 단계로 넘어갑니다 -> 5번 화면으로");
        SceneManager.LoadScene("1_MainMenu");
    }
}
