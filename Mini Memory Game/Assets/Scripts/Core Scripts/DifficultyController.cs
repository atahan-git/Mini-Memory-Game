using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifficultyController : MonoBehaviour {
    public static DifficultyController s;

    public bool gameHasBegun = false;

    private void Awake() {
        s = this;
    }


    private void Start() {
        difficultySelectScreen.SetActive(true);
    }

    public Difficulty[] difficulties;
    
    public int currentDifficulty = 1;

    public UnityEvent OnDifficultySet = new UnityEvent();
    public Difficulty GetCurrentDifficulty() {
        return difficulties[currentDifficulty];
    }


    public GameObject difficultySelectScreen;
    public void SetDifficulty(int dif) {
        currentDifficulty = dif;
        gameHasBegun = true;
        difficultySelectScreen.SetActive(false);
        OnDifficultySet?.Invoke();
    }
}


[System.Serializable]
public class Difficulty {
    public int minRatCount = 2;
    public int maxRatCount = 7;

    public float startSpawnDelay = 3f;
    public float minSpawnDelay = 0.5f;
    [Range(0.5f,1f)]
    public float spawnDelayShortening = 0.98f;

    public int ratCount = 50;

    public float startMoveDelay = 1f;
    public float minMoveDelay = 0.6f;
    [Range(0.5f,1f)]
    public float moveDelayShortening = 0.98f;

    public float longWordDelayMultiplier = 1f;

    public int coinsOnVictory = 5;
}