using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CloudLoginUnity;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public GameObject buyRatsMenu;


	public void ToggleBuyRatsMenu() {
		buyRatsMenu.SetActive(!buyRatsMenu.activeSelf);
	}
	private void Start() {
		email.text = PlayerPrefs.GetString("userEmail");
		//password.text = PlayerPrefs.GetString("password");

		if (email.text.Length > 0) {
			CloseAccountScreen();
		} else {
			ChangeAccount();
		}
		
		DataSaver.s.CallOnLogin(LoginComplete);
		DataSaver.s.CallOnLogin(CloseAccountScreen);

		PlayerLoadoutController.s.hiraganaKatakanaOnlyMode = PlayerPrefs.GetInt("hiraganaMode", 1) == 1;
		
		modeText.text = PlayerLoadoutController.s.hiraganaKatakanaOnlyMode ? 
			"Change Mode\nCurrent Mode: Hiragana/Katakana" : 
			"Change Mode\nCurrent Mode: Random Words";
	}

	private void OnDestroy() {
		DataSaver.s.RemoveFromLoginCall(LoginComplete);
		DataSaver.s.RemoveFromLoginCall(CloseAccountScreen);
	}

	public GameObject accountOverlay;
	public TMP_Text accountText;
	public void PlayMode(int mode) {
		SceneLoader.s.LoadPlayScene(mode);
	}


	public TMP_Text modeText;
	public void ChangeMode() {
		PlayerLoadoutController.s.hiraganaKatakanaOnlyMode = !PlayerLoadoutController.s.hiraganaKatakanaOnlyMode;

		PlayerPrefs.SetInt("hiraganaMode", PlayerLoadoutController.s.hiraganaKatakanaOnlyMode ? 1 : 0);

		modeText.text = PlayerLoadoutController.s.hiraganaKatakanaOnlyMode ? 
			"Change Mode\nCurrent Mode: Hiragana/Katakana" : 
			"Change Mode\nCurrent Mode: Random Words";
	}

	public void ChangeAccount() {
		accountOverlay.SetActive(true);
	}

	public Button loginButton;
	public TMP_Text loginText;
	public TMP_InputField email;
	public TMP_InputField password;
	public void TryToLogIn() {
		loginText.text = "Logging in";
		loginButton.interactable = false;
		DataSaver.s.SignUp(email.text, password.text);
	}

	public void CloseAccountScreen() {
		loginText.text = "Try to Log in";
		loginButton.interactable = true;
		accountOverlay.SetActive(false);
	}

	void LoginComplete() {
		loginText.text = "Try to Log i	n";
		loginButton.interactable = true;
		var username = CloudLoginUser.CurrentUser.GetUsername();
		accountText.text = $"Change Account\nCurrentAccount: {username}";
	}
}
