using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public Transform spellShootArea;
    public GameObject spellPrefab;

    private void Start() {
        WordSystemController.s.OnCorrectMatch.AddListener(ShootSpellCorrect);
    }

    public void ShootSpellCorrect(WordWrapper wrapper) {
        var target = MonsterSpawner.s.GetMonsterWithWord(wrapper);
        ShootSpell(target, true);
    }
    
    public void ShootSpell(MonsterScript target, bool isCorrect, bool unaffectedBySlowdown = false) {
        var spell = Instantiate(spellPrefab, transform);
        spell.transform.position = spellShootArea.transform.position;
        spell.GetComponent<SpellScript>().SetUp(target, isCorrect);

        if (unaffectedBySlowdown)
            spell.GetComponent<SpellScript>().unaffectedByTime = true;
        
        target.SpellShot();
    }
}
