using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // File 클래스를 사용하기 위해 네임스페이스 추가
using UnityEditor;


public class ChooseMusic : MonoBehaviour
{
    private string fbxFilePath = "sample1"; // Resources 폴더 내의 경로를 지정 (확장자 제외)
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

    public void CopyFBX()
    {
        // 로컬 저장소에 저장된 FBX파일을 유니티의 Assets/Resources 경로에 저장

        Debug.Log("fbx 파일 복사 및 실행");
        string sourcePath = @"C:/Users/parkg/Desktop/dudum_fbx/gBR_sBM_cAll_d04_mBR0_ch01_mJB5.npy.fbx"; // 복사할 로컬 파일 경로
        string destinationPath = "Assets/Resources/sample1.fbx"; // Assets 폴더 내의 목적지 경로

        // 1) 이미 Assets 폴더에 FBX파일이 있는 경우
        if (System.IO.File.Exists(destinationPath)) {

            Debug.Log("이미 유니티에 fbx파일이 존재함 : Assets/Resources/sample1.fbx");
        }
        // 2) Assets 폴더 X, 로컬 저장소에 O
        else if (System.IO.File.Exists(sourcePath))
        {
            // 로컬 저장소에서 fbx파일을 복사해오는 코드
            System.IO.File.Copy(sourcePath, destinationPath, true);
            UnityEditor.AssetDatabase.Refresh(); // 에디터 스크립트에서 사용
            Debug.Log("FBX파일을 유니티에 복사 완료");
        }
        // 3) Assets 폴더 X, 로컬 저장소에 X
        else
        {
            Debug.LogError("원하는 파일이 로컬 경로에 존재하지 않음");
        }
        
       
    }
}
