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
        // 5:���� ������� 1:����ȭ������ �̵��ϴ� �Լ�
        Debug.Log("���� �ܰ�� �Ѿ�ϴ� -> 5�� ȭ������");
        SceneManager.LoadScene("1_MainMenu");
    }
}
