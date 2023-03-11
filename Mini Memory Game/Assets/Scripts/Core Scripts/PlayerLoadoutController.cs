using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class PlayerLoadoutController : MonoBehaviour {
	public static PlayerLoadoutController s;


	public SpriteAnimationHolder ratGraphics;
	public SpriteAnimationHolder hatGraphics;
	public SpriteAnimationHolder spellGraphics;
	public SpriteAnimationHolder foodGraphics;
	public SpriteAnimationHolder backgroundOutsideGraphics;
	public SpriteAnimationHolder coinGraphics;

	[HideInInspector]
	public UnityEvent OnGraphicsLoadoutChanged;

	private WordPack[] selectedWordPacks;

	private void Awake() {
		if (s == null) {
			s = this;
		}
	}

	private void Start() {
		if (s == this) {
			DataSaver.s.CallOnLoad(RefreshLoadout);
		}
	}

	private void OnDestroy() {
		DataSaver.s.RemoveFromLoadCall(RefreshLoadout);
	}
	
	
	public bool hiraganaKatakanaOnlyMode = false;


	public void RefreshLoadout() {
		var mySave = DataSaver.s.GetCurrentSave();

		if (mySave.loadoutWordPackNames == null || mySave.loadoutWordPackNames.Length < 3 || mySave.loadoutWordPackNames[0] == null) {
			SetDefaultLoadoutWordPacks(mySave);
		}

		selectedWordPacks = new WordPack[3];

		for (int i = 0; i < mySave.loadoutWordPackNames.Length; i++) {
			selectedWordPacks[i] = WordPackLoader.s.allWordPacks.Find(pack => pack.wordPackName == mySave.loadoutWordPackNames[i]);
		}
	}

	public static void SetDefaultLoadoutWordPacks(DataSaver.SaveFile mySave) {
		try {
			mySave.loadoutWordPackNames = new[] {
				WordPackLoader.s.allWordPacks[0].wordPackName,
				WordPackLoader.s.allWordPacks[1].wordPackName,
				WordPackLoader.s.allWordPacks[2].wordPackName,
			};
			
			DataSaver.s.SaveActiveGame();
		} catch {
			//WordPackLoader.s.LoadDefaultWordPacksFromResources();
		}
	}


	public WordPack GetRandomPack() {
		if (hiraganaKatakanaOnlyMode) {
			return GetHiraganaOrKatana(Random.value > 0.5f);
		} else {
			return GetMainWordPack();
			//return DataSaver.s.GetCurrentSave().mainWordPack;
		}
	}
	
	
	public List<WordPack> GetAllPacks() {
		if (hiraganaKatakanaOnlyMode) {
			var allActivePacks = new List<WordPack>();
			allActivePacks.Add(GetHiraganaOrKatana(true));
			allActivePacks.Add(GetHiraganaOrKatana(false));
			
			return allActivePacks;
		} else {
			return WordPackLoader.s.allWordPacks;
		}
	}
	
	/*public WordPack GetWordPackWithIndex(int index) {
		if (selectedWordPacks == null || selectedWordPacks.Length < 3) {
			RefreshLoadout();
		}
		return selectedWordPacks[index];
	}*/

	public WordPack GetMainWordPack() {
		return WordPackLoader.s.allWordPacks.Find(pack => pack.wordPackName == "JPDB");
	}

	public WordPack GetHiraganaOrKatana(bool isHiragana) {
		if (selectedWordPacks == null || selectedWordPacks.Length < 3) {
			RefreshLoadout();
		}

		if (isHiragana) {
			return WordPackLoader.s.allWordPacks.Find(pack => pack.wordPackName == "Hiragana");
		} else {
			return WordPackLoader.s.allWordPacks.Find(pack => pack.wordPackName == "Katakana");
		}
	}
}
