using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheMoon_Cloud : MonoBehaviour
{

    void Update()
    {
        Move();
        FlipX();
    }

    void Move() //�,���� ����
    {
        Vector3 movement = Vector3.down * Time.deltaTime * Meteor_Manager.instance.speed;
        transform.position += movement;

        if (transform.position.y < -8)
        {
            Destroy(gameObject);
        }
    }

    void FlipX()
    {
        // ���� localScale�� x ���� ������Ŵ
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}
