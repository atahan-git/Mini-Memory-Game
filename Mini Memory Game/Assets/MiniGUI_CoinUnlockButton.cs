using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_CoinUnlockButton : MonoBehaviour {

    public string unlockName = "unset";

    private bool isUnlocked = false;
    
    private void Start() {
        if (unlockName == "unset") {
            Debug.LogError("Unlock button name not set!");
            return;
        }
        
        DataSaver.s.CallOnLoad(CheckUnlock);
        
        GetComponentInChildren<TMP_Text>().text = $"Unlock: {unlockAmount}";
    }

    void CheckUnlock() {
        isUnlocked = DataSaver.s.GetCurrentSave().unlockedUpgrades.Contains(unlockName);

        if (isUnlocked) {
            transform.parent.GetComponent<Button>().interactable = true;
            gameObject.SetActive(false);
        } else {
            transform.parent.GetComponent<Button>().interactable = false;
            GetComponent<Button>().onClick.AddListener(Unlock);
        }
    }

    public int unlockAmount = 3000;
    void Update() {
        GetComponent<Button>().interactable = DataSaver.s.GetCurrentSave().coinCount > unlockAmount;
    }

    public void Unlock() {
        if (!isUnlocked) {
            transform.parent.GetComponent<Button>().interactable = true;
            gameObject.SetActive(false);
            if (DataSaver.s.GetCurrentSave().unlockedUpgrades.Contains(unlockName)) {
                Debug.LogError($"Tried to unlock an upgrade that already was unlocked {unlockName}");
                return;
            }

            DataSaver.s.GetCurrentSave().unlockedUpgrades.Add(unlockName);
            isUnlocked = true;
            DataSaver.s.SaveActiveGame();
            
            CoinAmount.s.CoinAmountChanged();
        }
    }
}
