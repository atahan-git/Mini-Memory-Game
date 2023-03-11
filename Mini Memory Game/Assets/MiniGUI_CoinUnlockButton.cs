using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_CoinUnlockButton : MonoBehaviour {


    public int unlockAmount = 3000;
    void Update() {
        GetComponent<Button>().interactable = DataSaver.s.GetCurrentSave().coinCount > unlockAmount;
    }
}
