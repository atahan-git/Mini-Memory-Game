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

    float spawnDelay = 5f;
    float currentDelay = 0;
    float spawnDelayShortening = 0.98f;
    private float moveDelayShortening = 0.98f;

    public List<MonsterScript> allMonsters = new List<MonsterScript>();
    public List<GameObject> dyingMonsters = new List<GameObject>();

    public GameObject deathEffect;

    public float moveDelay = 1f;

    public int remainingRats = 50;
    public TMP_Text remainingRatsText;


    public Transform boostHeight;
    [HideInInspector] public float boostHeightY => boostHeight.transform.position.y;

    private void Start() {
        //boostHeightY = boostHeight.transform.position.y;
        
        DifficultyController.s.OnDifficultySet.AddListener(SetDifficulty);
    }

    private Difficulty difficulty;
    void SetDifficulty() {
        difficulty = DifficultyController.s.GetCurrentDifficulty();
        spawnDelay = difficulty.startSpawnDelay;
        spawnDelayShortening = difficulty.spawnDelayShortening;
        moveDelay = difficulty.startMoveDelay;
        moveDelayShortening = difficulty.moveDelayShortening;
        remainingRats = difficulty.ratCount;
    }

    private float miniDelay = 0;
    void Update() {
        if (DifficultyController.s.gameHasBegun) {
            if (PauseController.s.isPlaying && remainingRats > 0 && allMonsters.Count < difficulty.maxRatCount) {
                if (allMonsters.Count < difficulty.minRatCount && miniDelay <= 0) {
                    SpawnMonster(out WordWrapper wrapper);
                    currentDelay += wrapper.GetExtraTimeDelay() * difficulty.longWordDelayMultiplier;
                    miniDelay = 0.1f;
                }

                if (currentDelay <= 0) {
                    var success = SpawnMonster(out WordWrapper wrapper);
                    currentDelay = spawnDelay + wrapper.GetExtraTimeDelay() * difficulty.longWordDelayMultiplier;
                    if (success) {
                        spawnDelay *= spawnDelayShortening;
                        spawnDelay = Mathf.Clamp(spawnDelay, difficulty.minSpawnDelay, 10f);
                        moveDelay *= moveDelayShortening;
                        moveDelay = Mathf.Clamp(moveDelay, difficulty.minMoveDelay, 3);
                    }
                }

                miniDelay -= Time.deltaTime;
                currentDelay -= Time.deltaTime;
            }

            if (PauseController.s.isPlaying && (remainingRats + allMonsters.Count + dyingMonsters.Count <= 0) && !winBegun) {
                winBegun = true;
                WinLoseFinishController.s.Win();
            }
        }
    }

    private bool winBegun = false;

    public Queue<int> lastLocations = new Queue<int>();

    bool SpawnMonster(out WordWrapper wordWrapper) {
        wordWrapper = WordProvider.s.GetWord();

        if (wordWrapper != null) {
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

    public int GetAliveMonsterCount() {
        return allMonsters.Count;
    }
}
