using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseController : MonoBehaviour {

    public static PauseController s;
    
    public GameObject pauseScreen;

    [HideInInspector]
    public UnityEvent GamePausedCall;
    [HideInInspector]
    public UnityEvent GameResumeCall;
    
    private void Awake() {
        s = this;
    }

    private void Start() {
        Resume();
        pauseScreen.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
    }

    public bool isPlaying = true;

    public void Pause() {
        if(WinLoseFinishController.s.isGameOver)
            return;

        if (isPlaying) {
            isPlaying = false;
            pauseScreen.SetActive(true);
            GamePausedCall?.Invoke();
        }
    }

    public void Resume() {
        if(WinLoseFinishController.s.isGameOver)
            return;
        
        if (!isPlaying) {
            isPlaying = true;
            pauseScreen.SetActive(false);
            GameResumeCall?.Invoke();
            MobileKeyboardCapturer.s.ReFocus();
        }
    }

    public void HidePauseScreen() {
        pauseScreen.SetActive(false);
    }
}
