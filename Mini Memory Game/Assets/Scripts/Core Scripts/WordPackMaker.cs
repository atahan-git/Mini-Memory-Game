using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

public class WordPackMaker : MonoBehaviour {

    
    public WordPack activeWordPack;

    /*[Button()]
    public void SaveWordPack() {
        WordPackLoader.SaveWordPack(activeWordPack);
    }
    
    
    [Button()]
    public void LoadWordPack() {
        activeWordPack = WordPackLoader.LoadWordPack(activeWordPack.wordPackName);
    }*/

    public TextAsset csv;

    [Button]
    public void LoadFromCSV() {
        var lines = csv.text.Split('\n');

        activeWordPack = new WordPack();
        for (int i = 0; i < lines.Length; i++) {
            var curLine = lines[i].Split(',');
            var pair = new WordPair();
            pair.meaning = curLine[0];
            pair.word = curLine[1];
            activeWordPack.wordPairs.Add(pair);
        }
    }
}
