using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheMoon_Coin : MonoBehaviour
{

    void Update()
    {
        if (ToTheMoon_Manager.instance != null)
        {
            // Manager���� ��� �̵� �ӵ� ��������
            float moveAmount = ToTheMoon_Manager.instance.BackgroundMoveSpeed * Time.deltaTime;
            transform.position += Vector3.down * moveAmount;
        }

        CheckFallOffScreen();
    }

    void CheckFallOffScreen()
    {
        // ȭ�� �Ʒ��� ���������� Ȯ��
        float screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        if (transform.position.y < screenBottom - 1f) // 1f ���� ����
        {
            Destroy(gameObject);
        }
    }

}
