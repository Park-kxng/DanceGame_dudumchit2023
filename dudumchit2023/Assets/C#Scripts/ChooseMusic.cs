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
}
