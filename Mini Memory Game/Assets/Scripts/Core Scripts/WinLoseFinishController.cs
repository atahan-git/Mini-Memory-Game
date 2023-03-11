using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseFinishController : MonoBehaviour {

	public static WinLoseFinishController s;

	private void Awake() {
		s = this;
	}

	public bool isGameOver = false;

	public GameObject gameOverScreen;
	public TMP_Text percentProgress;
	public TMP_Text coinsEarned;

	private string startPercent;
	private int startCoins;

	private void Start() {
		gameOverScreen.SetActive(false);
		startPercent = XPSlider.s.GetProgressPercent();
		startCoins = DataSaver.s.GetCurrentSave().coinCount;
	}


	public void Win() {
		GameOver();
	}


	public void Lose() {
		GameOver();
	}

	void GameOver() {
		PauseController.s.HidePauseScreen();
		PauseController.s.isPlaying = false;
		isGameOver = true;
		
		gameOverScreen.SetActive(true);

		percentProgress.text = $"Percent Progress: {startPercent} -> {XPSlider.s.GetProgressPercent()}";
		coinsEarned.text = $"Coins earned: ${DataSaver.s.GetCurrentSave().coinCount - startCoins}";
	}


	public void BackToMenu() {
		SceneLoader.s.LoadMenuScene();
	}
}
