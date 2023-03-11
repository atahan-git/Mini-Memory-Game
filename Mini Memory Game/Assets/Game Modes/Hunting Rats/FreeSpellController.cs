using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpellController : MonoBehaviour
{
    public static FreeSpellController s;

    private void Awake() {
        s = this;
    }

    public Sprite[] manaSprites;

    public Image _renderer;

    public int currentMana;
    public int maxMana;

    public float regenDelay = 2f;
    private float curDelay;

    public SpellController spellController;
    void Start() {
        maxMana = manaSprites.Length-1;
        currentMana = maxMana;
        _renderer.sprite = manaSprites[currentMana];
    }

    private void Update() {
        if (curDelay > regenDelay) {
            curDelay = 0;

            currentMana += 1;
        }

        curDelay += Time.deltaTime;

        if (currentMana >= maxMana) {
            currentMana = maxMana;
            curDelay = 0;
        }
        
        
        _renderer.sprite = manaSprites[currentMana];
    }


    public void UseSpell(MonsterScript target) {
        if (currentMana >= 2) {
            if (MonsterSpawner.s.allMonsters.Contains(target)) {
                WordProvider.s.RemoveWord(target.myWord, false);
                
                currentMana -= 2;
                _renderer.sprite = manaSprites[currentMana];

                spellController.ShootSpell(target,false, true);
                SlowdownPower.s.ActivateAbility();
            }
        }
    }
}
