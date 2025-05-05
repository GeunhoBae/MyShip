using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToTheMoon_Manager : MonoBehaviour
{
    public static ToTheMoon_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    Game game;
    public float score = 0;
    public int coin = 0;

    public float speed = 6f;
    public int speedUpInterval = 5;

    public Transform playerContainer;
    // public Animator playerAnim;

    public GameObject coinPrefab;
    public Vector3[] spawnPositions = new Vector3[20]; // 20���� ���� ��ġ
    private List<int> availablePositions = new List<int>(); // ��� ������ ��ġ �ε���

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];
        // ĳ���� ����
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Unity.Mathematics.quaternion.identity);
        player.transform.SetParent(playerContainer, false);
        // playerAnim = player.GetComponent<Animator>();

        // 20���� ���� ��ġ �ʱ�ȭ (���� ��ġ, ���� ���ӿ� �°� ���� �ʿ�)
        for (int i = 0; i < 20; i++)
        {
            spawnPositions[i] = new Vector3(Random.Range(-10f, 10f), 6f, 0); // (X����, Y����, Z����)
        }

        Time.timeScale = 0;

        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");
    }

    public void GameStart(GameObject startBtn)
    {
        Time.timeScale = 1;
        startBtn.SetActive(false);
    }

    // ���ǵ� ���� �ڷ�ƾ
    IEnumerator SpeedUpRoutine()
    {
        while (speed < 22f)
        {
            yield return new WaitForSeconds(speedUpInterval);
            speed += 0.5f;
        }
    }

    // ���� ���� �ڷ�ƾ
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += speed / 4;
            scoreTxt.text = "����: " + (int)score;
        }
    }

    // ���� ����
    public void GetCoin()
    {
        int coinValue;

        if (speed > 18)
            coinValue = 5;
        else if (speed > 12)
            coinValue = 3;
        else
            coinValue = 2;

        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
    }

    // ���� ����
    public void GameOver(bool isOver)//�α׸� ���ܾ� �ϴ� ����: ������ �ȳ�, �Ƹ� ��������� ������ �ִ� ��
    {
        Debug.Log("[1] GameOver ȣ���"); // 1. �޼��� ���� Ȯ��

        try
        {
            Debug.Log("[2] UI ��� Ȯ�� ��");
            if (overTxt == null) Debug.LogError("overTxt null");
            if (topBar == null) Debug.LogError("topBar null");
            if (gameOverPanel == null) Debug.LogError("gameOverPanel null");

            Debug.Log("[3] DataManager Ȯ��");
            if (DataManager.instance == null) Debug.LogError("DataManager null");
            if (game == null) Debug.LogError("game null");

            DataManager.instance.saveData();
            Debug.Log("[4] ���� �Ϸ�");

            Time.timeScale = 0;
            Debug.Log("[5] TimeScale 0 ����");

            StopCoroutine("SpeedUpRoutine");
            StopCoroutine("ScoreUpRoutine");
            Debug.Log("[6] �ڷ�ƾ ����");

            overTxt.text = isOver ? "���ӿ���" : "�Ͻ�����";
            Debug.Log("[7] �ؽ�Ʈ ���� �Ϸ�");

            topBar.SetActive(true);
            returnBtn.SetActive(!isOver);
            reStartBtn.SetActive(isOver);
            Debug.Log("[8] UI Ȱ��ȭ �Ϸ�");

            gameOverPanel.SetActive(true);
            Debug.Log("[9] ���ӿ��� �г� Ȱ��ȭ");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���� �߻�: {e.Message}\n{e.StackTrace}");
        }
    }

    // [1-3] ���� ���� �г� ��ư �׼ǵ�
    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnAction()
    {
        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");

        gameOverPanel.SetActive(false);
        Time.timeScale = 1;

        topBar.SetActive(false);
    }

    public void ExitAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
}