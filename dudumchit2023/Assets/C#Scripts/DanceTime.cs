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
        //4:���߱⿡�� 5:���� ��� ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 5�� ȭ������");
        SceneManager.LoadScene("5_DanceComplete");
    }
    public void onClickStepPre()
    {

        //4:���߱⿡�� 2�� :���ǰ���� �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� ���ư��ϴ� -> 3�� ȭ������");
        SceneManager.LoadScene("3_ChooseDance");
    }
}
