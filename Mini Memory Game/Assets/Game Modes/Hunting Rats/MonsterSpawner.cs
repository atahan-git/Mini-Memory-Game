using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour {
    public static MonsterSpawner s;

    private void Awake() {
        s = this;
    }

    public Transform[] spawnLocations;

    public GameObject monsterPrefab;

    public float monsterDelay = 5f;
    public float currentDelay = 0;
    [Range(0.5f,1f)]
    public float delayShortening = 0.95f;

    public List<MonsterScript> allMonsters = new List<MonsterScript>();

    public GameObject deathEffect;

    public float globalMoveDelayMultiplier = 1f;

    public int remainingRats = 50;
    public TMP_Text remainingRatsText;


    public Transform boostHeight;
    [HideInInspector]
    public float boostHeightY;

    private void Start() {
        boostHeightY = boostHeight.transform.position.y;
    }

    void Update() {
        if (PauseController.s.isPlaying && remainingRats > 0) {
            if (allMonsters.Count == 0) {
                SpawnMonster(out  WordWrapper wrapper);
                currentDelay += wrapper.GetExtraTimeDelay();
            }

            if (currentDelay <= 0) {
                var success = SpawnMonster(out  WordWrapper wrapper);
                currentDelay = monsterDelay + wrapper.GetExtraTimeDelay();
                if (success) {
                    monsterDelay *= delayShortening;
                    monsterDelay = Mathf.Clamp(monsterDelay, 0.5f, 10f);
                    globalMoveDelayMultiplier *= delayShortening;
                    globalMoveDelayMultiplier = Mathf.Clamp(globalMoveDelayMultiplier, 0.6f, 1f);
                }
            }

            currentDelay -= Time.deltaTime;
        }

        if (PauseController.s.isPlaying && (remainingRats + allMonsters.Count <= 0)) {
            WinLoseFinishController.s.Win();
        }
    }

    public Queue<int> lastLocations = new Queue<int>();

    bool SpawnMonster(out WordWrapper wordWrapper) {
        var index = Random.Range(0, spawnLocations.Length);
        for (int i = 0; i < 20; i++) {
            if (lastLocations.Contains(index)) {
                index = Random.Range(0, spawnLocations.Length);
            } else {
                break;
            }
        }

        lastLocations.Enqueue(index);
        var location = spawnLocations[index];

        if (lastLocations.Count > 5) {
            lastLocations.Dequeue();
        }

        var monsterScript = Instantiate(monsterPrefab, transform).GetComponent<MonsterScript>();
        monsterScript.transform.position = new Vector3(location.transform.position.x, location.transform.position.y, 10);

        wordWrapper = WordProvider.s.GetWord();

        if (wordWrapper != null) {
            monsterScript.Setup(wordWrapper);
            allMonsters.Add(monsterScript);

            remainingRats -= 1;
            remainingRatsText.text = remainingRats.ToString();

            return true;
        }

        return false;
    }

    public MonsterScript GetMonsterWithWord(WordWrapper wordWrapper) {
        for (int i = 0; i < allMonsters.Count; i++) {
            if (allMonsters[i].myWord == wordWrapper)
                return allMonsters[i];
        }

        throw new NoNullAllowedException($"Can't find mosnter with wordwrapper {wordWrapper}");
    }
}
