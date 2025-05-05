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
    public Vector3[] spawnPositions = new Vector3[20]; // 20개의 생성 위치
    private List<int> availablePositions = new List<int>(); // 사용 가능한 위치 인덱스

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];
        // 캐릭터 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Unity.Mathematics.quaternion.identity);
        player.transform.SetParent(playerContainer, false);
        // playerAnim = player.GetComponent<Animator>();

        // 20개의 생성 위치 초기화 (예시 위치, 실제 게임에 맞게 조정 필요)
        for (int i = 0; i < 20; i++)
        {
            spawnPositions[i] = new Vector3(Random.Range(-10f, 10f), 6f, 0); // (X범위, Y범위, Z범위)
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

    // 스피드 증가 코루틴
    IEnumerator SpeedUpRoutine()
    {
        while (speed < 22f)
        {
            yield return new WaitForSeconds(speedUpInterval);
            speed += 0.5f;
        }
    }

    // 점수 증가 코루틴
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += speed / 4;
            scoreTxt.text = "점수: " + (int)score;
        }
    }

    // 코인 증가
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

    // 게임 오버
    public void GameOver(bool isOver)//로그를 남겨야 하는 이유: 에러가 안남, 아마 실행순서와 연관이 있는 듯
    {
        Debug.Log("[1] GameOver 호출됨"); // 1. 메서드 진입 확인

        try
        {
            Debug.Log("[2] UI 요소 확인 전");
            if (overTxt == null) Debug.LogError("overTxt null");
            if (topBar == null) Debug.LogError("topBar null");
            if (gameOverPanel == null) Debug.LogError("gameOverPanel null");

            Debug.Log("[3] DataManager 확인");
            if (DataManager.instance == null) Debug.LogError("DataManager null");
            if (game == null) Debug.LogError("game null");

            DataManager.instance.saveData();
            Debug.Log("[4] 저장 완료");

            Time.timeScale = 0;
            Debug.Log("[5] TimeScale 0 설정");

            StopCoroutine("SpeedUpRoutine");
            StopCoroutine("ScoreUpRoutine");
            Debug.Log("[6] 코루틴 정지");

            overTxt.text = isOver ? "게임오버" : "일시정지";
            Debug.Log("[7] 텍스트 설정 완료");

            topBar.SetActive(true);
            returnBtn.SetActive(!isOver);
            reStartBtn.SetActive(isOver);
            Debug.Log("[8] UI 활성화 완료");

            gameOverPanel.SetActive(true);
            Debug.Log("[9] 게임오버 패널 활성화");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"에러 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    // [1-3] 게임 오버 패널 버튼 액션들
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