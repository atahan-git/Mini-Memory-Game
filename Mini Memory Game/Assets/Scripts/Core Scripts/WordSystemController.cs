using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WordSystemController : MonoBehaviour {
	public static WordSystemController s;

	private void Awake() {
		s = this;
	}

	public Color imperfectMatchColor = Color.yellow;
	public Color wrongColor = Color.red;
	public Color correctColor = Color.green;

	public WordSystemSoundController soundController;

	public UnityEvent OnTryingMatch;
	public UnityEvent<WordWrapper> OnCorrectMatch;
	public UnityEvent OnWrongMatch;

	private void Start() {
		MobileKeyboardCapturer.s.StartListening(TryMatchWord);
		MobileKeyboardCapturer.s.valueChangedCallback += OnValueChanged;
		_inputChecker = StandardInputChecker;
	}

	public void OnValueChanged(string word) {
		soundController.MakeTypingSound();
	}
	
	private InputChecker _inputChecker;

	public void CorrectMatch(WordWrapper match) {
		WordProvider.s.RemoveWord(match, true);
		OnCorrectMatch?.Invoke(match); 
		soundController.CorrectMatchSoundEffect();
	}

	public void WrongMatch() {
		OnWrongMatch?.Invoke();
		soundController.WrongMatchSoundEffect();
	}
	
	delegate void InputChecker(string inputChecker);

	public bool IsEmpty(string word) {
		return word.Replace(" ", "") == "";
	}

	WordWrapper GetMatchingWrapper(string word) {
		for (int i = 0; i < WordProvider.s.activeWords.Count; i++) {
			var wordWrapper =  WordProvider.s.activeWords[i];
			var answerSide = wordWrapper.GetAnswerSide();
			if (IsMatch(word, answerSide)) {
				return wordWrapper;
			}
		}

		return null;
	}

	private int lastMatchDistance;
	public bool IsMatch(string word1, string word2) {
		var maxDistance = Mathf.FloorToInt(word1.Length * (1f/7)); // so for every 7 letters we can make one mistake
		lastMatchDistance = StringDistance.LevenshteinDistance(word1, word2);
		return lastMatchDistance <= maxDistance;
	}

	public void TryMatchWord(string word) {
		if (_inputChecker != null) {
			_inputChecker(word);
		}

		if (word.Length > 0) {
			soundController.MakeEnterSound();
		}
	}

	public void StandardInputChecker(string word) {
		if (!IsEmpty(word)) {
			OnTryingMatch?.Invoke();
			
			/*Debug.Log("DEBUG MATCHING OPTIONS PRESENT");
			if (word == "y" || word == "c") {
				CorrectMatch();
				return;
			} else if (word == "n" || word == "w") {
				WrongMatch();
				return;
			}*/

			/*if (word == "fix") {
				FixWord();
				return;
			}*/

			var matchingWord = GetMatchingWrapper(word);
			if (matchingWord != null) {
				if (lastMatchDistance > 0) {
					MobileKeyboardCapturer.s.SetTextColor(imperfectMatchColor);
				} else {
					MobileKeyboardCapturer.s.SetTextColor(correctColor);
				}
				
				CorrectMatch(matchingWord);
			} else {
				MobileKeyboardCapturer.s.SetTextColor(wrongColor);
				WrongMatch();
			}

			MobileKeyboardCapturer.s.LockField(true);
			StartCoroutine(ClearField());
		}
	}


	IEnumerator ClearField() {
		yield return new WaitForSecondsRealtime(0.2f);
		MobileKeyboardCapturer.s.ClearField();
		MobileKeyboardCapturer.s.LockField(false);
		MobileKeyboardCapturer.s.SetTextColor(Color.black);
	}

	/*public WordFixOverlay wordFixOverlay;
	void FixWord() {
		//wordFixOverlay.Engage(activeWordPair);
	}*/

}