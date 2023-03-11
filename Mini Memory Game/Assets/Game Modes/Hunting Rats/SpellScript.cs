using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScript : MonoBehaviour {


    public float speed;
    public float acceleration = 10f;

    private MonsterScript myTarget;

    public bool unaffectedByTime = false;
    public bool isCorrect;
    public void SetUp(MonsterScript target, bool _isCorrect) {
        myTarget = target;
        isCorrect = _isCorrect;
    }

    // Update is called once per frame
    void Update() {
        var deltaTime = Time.deltaTime;
        if (unaffectedByTime) {
            deltaTime = Time.unscaledDeltaTime;
        }

        if (myTarget == null) {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, myTarget.transform.position, speed*deltaTime);
        speed += acceleration * deltaTime;

        if (Vector3.Distance(transform.position, myTarget.transform.position) < 0.1f) {
            var deathEffect = Instantiate(MonsterSpawner.s.deathEffect).GetComponent<DeathEffect>();
            deathEffect.SetUp(myTarget);
            
            WordProvider.s.SpawnCoin(isCorrect, myTarget.transform.position);
            Destroy(myTarget.gameObject);
            Destroy(gameObject);
        }
    }
}
