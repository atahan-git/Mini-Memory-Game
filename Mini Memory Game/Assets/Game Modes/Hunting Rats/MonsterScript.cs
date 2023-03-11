using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterScript : MonoBehaviour {

	public TMP_Text text;

	public WordWrapper myWord;

	public float moveDelay = 1;
	private float curDelay;
	public float moveAmount = 1;

	public bool doMove = true;

	private RatGraphics ratGraphics;

	public float wordLengthSpeedMultiplier;

	public void Setup(WordWrapper word) {
		myWord = word;
		wordLengthSpeedMultiplier = word.GetSpeedMultiplier();
	}

	private void Start() {
		ratGraphics = GetComponentInChildren<RatGraphics>();
		ratGraphics.isDead = false;
		ratGraphics.doAnim = false;
	}


	void Update() {
		if (PauseController.s.isPlaying) {
			var distFromSpeed = transform.position.y - MonsterSpawner.s.boostHeightY;

			var distBoost = 1f;
			if (distFromSpeed > 0) {
				distBoost = distFromSpeed.Remap(0, 2, 0.7f, 0.1f);
				distBoost = Mathf.Clamp(distBoost, 0.1f, 1);
			}

			if (curDelay > moveDelay * MonsterSpawner.s.globalMoveDelayMultiplier * distBoost * wordLengthSpeedMultiplier) {
				Move();

				curDelay = 0;
			}

			if (doMove)
				curDelay += Time.deltaTime;


			text.text = myWord.GetQuestionSide();
		}
	}


	void Move() {
		transform.position += Vector3.down*moveAmount;
		
		ratGraphics.GoToNextSprite();

		if (transform.position.y < HealthController.s.healthHeight) {
			HealthController.s.GetDamaged();
			WordProvider.s.RemoveWord(myWord, false);
			WordProvider.s.SpawnCoin(false, transform.position);
			MonsterSpawner.s.allMonsters.Remove(this);
			Destroy(gameObject);
		}
	}

	public void SpellShot() {
		MonsterSpawner.s.allMonsters.Remove(this);
	}

	private void OnMouseDown() {
		FreeSpellController.s.UseSpell(this);
	}
}
