using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedInputFieldPlugin;
using TMPro;
using UnityEngine;

public class MobileKeyboardCapturer : MonoBehaviour {
    public static MobileKeyboardCapturer s;

    private void Awake() {
        s = this;
    }

    public AdvancedInputField inputField;

    private void Start() {
        Application.targetFrameRate = 60;
        
        //TouchScreenKeyboard.Open("yeet", TouchScreenKeyboardType.Default, false,false, false, false, "yeet");
        //TouchScreenKeyboard.Android.closeKeyboardOnOutsideTap = false;

        
        /*inputField.onDeselect.AddListener(OnDeselect);
        inputField.onSubmit.AddListener(OnEditEnd);
        inputField.onEndEdit.AddListener(OnDeselect);*/
        
        
        inputField.ManualSelect();
        inputField.ShouldBlockDeselect = true;
    }

    public void ReFocus() {
        inputField.ManualSelect();
    }
    

    /*public void OnDeselect(string word) {
        inputField.ActivateInputField();
    }*/

    public WordCapturedCallback valueChangedCallback;
    public void OnValueChanged() {
        /*if (inputField.Text.Contains("\n")) {
            OnEditEnd();
        }
        print(inputField.Text);*/
        
        valueChangedCallback?.Invoke(inputField.Text);
    }

    public void OnEditEnd() {
        if (isListening) {
            _callback?.Invoke(inputField.Text);
            /*isListening = false;
            _callback = null;*/
        }
        
        //inputField.Clear();
        //inputField.ActivateInputField();
    }

    public void ClearField() {
        inputField.Clear();
    }

    public void LockField(bool isLocked) {
        inputField.ReadOnly = isLocked;
    }

    public TMP_Text actualText;

    public void SetTextColor(Color color) {
        actualText.color = color;
        //inputField.colo
    }

    public delegate void WordCapturedCallback(string word);
    private WordCapturedCallback _callback;
    private bool isListening = false;
    public void StartListening(WordCapturedCallback callback) {
        inputField.Text = "";
        _callback = callback;
        isListening = true;
    }

    public void StopListening() {
        isListening = false;
        _callback = null;
    }
}

