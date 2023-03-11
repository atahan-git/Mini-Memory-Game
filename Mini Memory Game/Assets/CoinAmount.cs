using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinAmount : MonoBehaviour {
    public static CoinAmount s;

    private void Awake() {
        s = this;
    }

    public TMP_Text coinText;
    public CoinGraphics graphics;

    public float curDelay = 0.1f;
    public float minDelay = 0.05f;
    public float maxDelay = 0.3f;
    public float delayChangeSpeed = 0.05f;

    private void Start() {
        CoinAmountChanged();
    }

    void Update() {
        graphics.delay = curDelay;
        curDelay = Mathf.MoveTowards(curDelay, maxDelay, delayChangeSpeed * Time.deltaTime);
    }

    public void CoinAmountChanged() {
        coinText.text = DataSaver.s.GetCurrentSave().coinCount.ToString();
        curDelay = minDelay;
    }
}
