using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheMoon_Character : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField]
    float moveSpeed = 5f;
    //[SerializeField]
    //float jumpForce = 8f; // ������ ����� �� ���� ��
    [SerializeField]
    float gravityScale = 2f; // �߷� ũ��

    private float minX, maxX;
    private float characterHalfWidth;
    private int characterDirection = 1; // -1: ������, +1: ����
    private bool isGameOver = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = gravityScale; // �߷� ����
        CalculateScreenBounds();
    }

    void Update()
    {
        if (isGameOver) return;

        HandleInput();
        ClampPosition();
        CheckFallOffScreen();
    }

    void CheckFallOffScreen()
    {
        // ȭ�� �Ʒ��� ���������� Ȯ��
        float screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        if (transform.position.y < screenBottom - 1f) // 1f ���� ����
        {
            GameOver();
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        ToTheMoon_Manager.instance.GameOver(isOver: true);
    }

    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }

    void CalculateScreenBounds()
    {
        Collider2D collider = GetComponent<Collider2D>();
        characterHalfWidth = collider != null ? collider.bounds.extents.x : 0.5f;

        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        minX = Camera.main.transform.position.x - cameraHalfWidth + characterHalfWidth;
        maxX = Camera.main.transform.position.x + cameraHalfWidth - characterHalfWidth;
    }

    void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 inputPosition = Input.mousePosition;

            if (inputPosition.x < Screen.width / 2)
            {
                // ���� �̵�
                rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
                if (characterDirection != 1)
                {
                    characterDirection = 1;
                    FlipCharacter();
                }
            }
            else
            {
                // ������ �̵�
                rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
                if (characterDirection != -1)
                {
                    characterDirection = -1;
                    FlipCharacter();
                }
            }
        }
        else
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
    }

    void FlipCharacter()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * characterDirection;
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin") && rigid.velocity.y < 0)
        {
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            Destroy(other.gameObject);
            ToTheMoon_Manager.instance.GetCoin();
        }

        if (other.CompareTag("Obstacle") && rigid.velocity.y < 0)
        {
            float desiredJumpHeight = 4f; // ���ϴ� ���� ����
            float gravity = Mathf.Abs(Physics2D.gravity.y) * rigid.gravityScale;
            float initialVelocity = Mathf.Sqrt(2 * gravity * desiredJumpHeight);

            rigid.velocity = new Vector2(rigid.velocity.x, initialVelocity);

            AudioManager.instance.PlaySFX(AudioManager.SFXClip.FAIL);
        }
    }
}