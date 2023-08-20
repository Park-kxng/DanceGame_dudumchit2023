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
    public void CopyFile()
    {
        Debug.Log("fbx 파일 복사 및 실행");
        string sourcePath = @"C:/Users/parkg/Desktop/dudum_fbx/gBR_sBM_cAll_d04_mBR0_ch01_mJB5.npy.fbx"; // 복사할 로컬 파일 경로
        string destinationPath = "Assets/Resources/sample1.fbx"; // Assets 폴더 내의 목적지 경로

        if (System.IO.File.Exists(sourcePath))
        {
            System.IO.File.Copy(sourcePath, destinationPath, true);
            UnityEditor.AssetDatabase.Refresh(); // 에디터 스크립트에서 사용
            Debug.Log("File copied to Assets folder.");
        }
        else
        {
            Debug.LogError("Source file does not exist.");
        }
        
        GameObject fbxModelPrefab = Resources.Load<GameObject>(fbxFilePath);

        if (fbxModelPrefab != null)
        {
            GameObject fbxModel = Instantiate(fbxModelPrefab);
            fbxModel.transform.position = Vector3.zero; // 원하는 위치로 이동
        }
        else
        {
            Debug.LogError("FBX model not found in Resources folder.");
        }
    }
}
