using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFlyToCoinStore : MonoBehaviour
{

    public float speed;
    public float acceleration = 10f;
    void Update()
    {
        var deltaTime = Time.deltaTime;
        var myTarget = CoinAmount.s;
        
        transform.position = Vector3.MoveTowards(transform.position, myTarget.transform.position, speed*deltaTime);
        speed += acceleration * deltaTime;

        if (Vector3.Distance(transform.position, myTarget.transform.position) < 0.1f) {
           ReachTarget();
        }
    }


    void ReachTarget() {
        DataSaver.s.GetCurrentSave().coinCount += 1;
        DataSaver.s.SaveActiveGame();
        CoinAmount.s.CoinAmountChanged();
        Destroy(gameObject);
    }
}
