using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeathEffect : MonoBehaviour {

    public float verticalSpeed = 60;
    public float gravity = 10;
    public Vector2 xSpeed = new Vector2(20, 30);
    public float curXSpeed;
    public TMP_Text myText;
    
    public void SetUp(MonsterScript monsterScript) {
        myText.text = monsterScript.myWord.GetAnswerSide();

        transform.position = monsterScript.transform.position;

        curXSpeed = Random.Range(xSpeed.x, xSpeed.y);
        curXSpeed *= Random.value > 0.5f ? 1 : -1;
        MonsterSpawner.s.dyingMonsters.Add(gameObject);
    }


    private void Update() {
        transform.position += new Vector3(curXSpeed, verticalSpeed, 0) * Time.deltaTime;
        verticalSpeed -= gravity * Time.deltaTime;

        if (transform.position.y < -50) {
            MonsterSpawner.s.dyingMonsters.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
