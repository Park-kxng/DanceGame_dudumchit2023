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
        //3��:�� ���ÿ��� 4:���߱� ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 4�� ȭ������");
        SceneManager.LoadScene("4_DanceTime");
    }
    public void onClickStepPre()
    {

        //3��:�� ���ÿ��� 2�� :���ǰ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� ���ư��ϴ� -> 2�� ȭ������");
        SceneManager.LoadScene("2_ChooseMusic");
    }
}
