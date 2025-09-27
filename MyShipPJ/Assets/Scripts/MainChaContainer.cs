using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEditor;
using UnityEngine;

public class MainChaContainer : MonoBehaviour
{
    public List<GameObject> characterPrefabs;
    public GameObject curCharacterObj;
    readonly string path = "Prefabs/Characters/";

    void Start()
    {
        // 캐릭터 프리팹 List에 프리팹 추가
        foreach (Character cha in DataManager.instance.characterSotred)
            characterPrefabs.Add(Resources.Load<GameObject>(path + cha.name));

        ChangeCharacter();

        // blink 애니메이션 코루틴 시작
        StartCoroutine(BlinkAnimation());
    }

    public void ChangeCharacter()
    {
        if (gameObject.transform.childCount > 0)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }

        GameObject curCharacterObj = Instantiate(characterPrefabs[PlayerPrefs.GetInt("CurCharacter", 0)], transform.position, Quaternion.identity);
        curCharacterObj.transform.SetParent(gameObject.transform, false);
        curCharacterObj.SetActive(true);

        // 캐릭터 변경 후 blink 애니메이션 다시 시작
        StopCoroutine(BlinkAnimation());
        StartCoroutine(BlinkAnimation());
    }

    // 5초마다 blink 애니메이션 실행
    IEnumerator BlinkAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5초 대기

            // 현재 활성화된 캐릭터의 Animator 찾기
            if (gameObject.transform.childCount > 0)
            {
                Transform currentCharacter = gameObject.transform.GetChild(0);
                Animator animator = currentCharacter.GetComponent<Animator>();

                if (animator != null)
                {
                    animator.SetTrigger("Blink"); // blink 트리거 발동
                }
            }
        }
    }
}