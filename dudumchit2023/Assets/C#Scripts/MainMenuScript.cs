using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickModeBasic()
    {
        Debug.Log("모드 : 베이직");
        SceneManager.LoadScene("2_ChooseMusic");

    }

    public void OnClickModeCustomize()
    {
        Debug.Log("모드 : 커흐텀");
        SceneManager.LoadScene("2_ChooseMusic");

    }

    public void OnClickQuit()
    {
        Debug.Log("게임을 종료합니다");

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
