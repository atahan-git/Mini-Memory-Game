using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour {
    public static DataHolder s;

    private void Awake() {
        if (s == null)
            s = this;
    }
    
}