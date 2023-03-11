using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {
    public static HealthController s;

    private void Awake() {
	    s = this;
    }

    public Sprite[] healthSprites;

    public Image _renderer;

    public int currentHealth;

    [HideInInspector]
    public float healthHeight;
    void Start() {
        currentHealth = healthSprites.Length-1;
        _renderer.sprite = healthSprites[currentHealth];
    }

    private void Update() {
        healthHeight = _renderer.transform.position.y;
    }

    public void GetDamaged() {
        currentHealth -= 1;
        
        if (currentHealth == 0) {
            WinLoseFinishController.s.Lose();
        }
        
        _renderer.sprite = healthSprites[currentHealth];
    }
}
