using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowdownPower : MonoBehaviour {
    public static SlowdownPower s;

    private void Awake() {
        s = this;
    }

    public float cooldownTimer = 60f;
    public float curCooldown;

    public bool canUse;

    public Slider cooldownSlider;
    public Button myButton;


    public bool abilityActive = false;

    float slowDownSpeed = 5f;
    float slowedDownTime = 0f;

    private void Start() {
        WordProvider.s.OnHintSpawned.AddListener(ResetCooldown);
        WordSystemController.s.OnCorrectMatch.AddListener(DisableAbilityOnCorrect);
        WordSystemController.s.OnTryingMatch.AddListener(DisableAfterCertainCount);
        WinLoseFinishController.s.OnGameFinished.AddListener(DisableAbility);
    }

    // Update is called once per frame
    void Update() {
        curCooldown += Time.deltaTime;
        
        canUse = curCooldown > cooldownTimer;

        curCooldown = Mathf.Clamp(curCooldown, 0, cooldownTimer);

        cooldownSlider.value = curCooldown / cooldownTimer;

        myButton.interactable = canUse;

        if (abilityActive) {
            TimeController.s.SetTimeScale(Mathf.Lerp(Time.timeScale, slowedDownTime, slowDownSpeed*Time.unscaledDeltaTime));
        } else {
            TimeController.s.SetTimeScale(Mathf.Lerp(Time.timeScale, 1f, slowDownSpeed*Time.unscaledDeltaTime));
        }

        justActivatedLock -= Time.unscaledDeltaTime;
    }


    public void ResetCooldown() {
        curCooldown = cooldownTimer;
    }

    public void ActivateAbility() {
        if (canUse) {
            abilityActive = true;
            curCooldown = 0;
            justActivatedLock = 0.1f;
            disableCount = 0;
        }
    }

    private float justActivatedLock;

    public void DisableAbilityOnCorrect(WordWrapper arg0) {
        if (justActivatedLock <= 0 && MonsterSpawner.s.GetAliveMonsterCount() == 0) {
            abilityActive = false;
        }
    }

    public void DisableAbility() {
        abilityActive = false;
    }

    public int disableCount = 0;
    public void DisableAfterCertainCount() {
        disableCount += 1;

        if (disableCount >= 3) {
            abilityActive = false;
        }
    }
}
