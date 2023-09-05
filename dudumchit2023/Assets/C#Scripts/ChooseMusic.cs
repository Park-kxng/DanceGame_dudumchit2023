using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // File Ŭ������ ����ϱ� ���� ���ӽ����̽� �߰�
using UnityEditor;


public class ChooseMusic : MonoBehaviour
{
    private string fbxFilePath = "sample1"; // Resources ���� ���� ��θ� ���� (Ȯ���� ����)
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
        //2��:���� ���⿡�� 3��: �� ���� ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 3�� ȭ������");
        SceneManager.LoadScene("3_ChooseDance");
    }
    public void onClickStepPre()
    {

        //2��:���� ���� ���� 1�� :���� �޴��� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� ���ư��ϴ� -> 1�� ȭ������");
        SceneManager.LoadScene("1_MainMenu");
    }

    public void CopyFBX()
    {
        // ���� ����ҿ� ����� FBX������ ����Ƽ�� Assets/Resources ��ο� ����

        Debug.Log("fbx ���� ���� �� ����");
        string sourcePath = @"C:/Users/parkg/Desktop/dudum_fbx/gBR_sBM_cAll_d04_mBR0_ch01_mJB5.npy.fbx"; // ������ ���� ���� ���
        string destinationPath = "Assets/Resources/sample1.fbx"; // Assets ���� ���� ������ ���

        // 1) �̹� Assets ������ FBX������ �ִ� ���
        if (System.IO.File.Exists(destinationPath)) {

            Debug.Log("�̹� ����Ƽ�� fbx������ ������ : Assets/Resources/sample1.fbx");
        }
        // 2) Assets ���� X, ���� ����ҿ� O
        else if (System.IO.File.Exists(sourcePath))
        {
            // ���� ����ҿ��� fbx������ �����ؿ��� �ڵ�
            System.IO.File.Copy(sourcePath, destinationPath, true);
            UnityEditor.AssetDatabase.Refresh(); // ������ ��ũ��Ʈ���� ���
            Debug.Log("FBX������ ����Ƽ�� ���� �Ϸ�");
        }
        // 3) Assets ���� X, ���� ����ҿ� X
        else
        {
            Debug.LogError("���ϴ� ������ ���� ��ο� �������� ����");
        }
        
       
    }
}
