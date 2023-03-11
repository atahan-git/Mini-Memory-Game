using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class WordPackLoader : MonoBehaviour {
    public static WordPackLoader s;
    const string wordPackFolderName = "WordPacks";
    
    public List<WordPack> allWordPacks = new List<WordPack>();
    public bool wordPacksLoaded = false;

    public List<TextAsset> allWordPackJsons = new List<TextAsset>();
    
    public TMP_Text currentWords;

    public static string GetWordPackPath () {
        return Path.Combine(Application.persistentDataPath, wordPackFolderName);
    }


    /*public static string[] GetAllWordPackPaths() {
        Directory.CreateDirectory(GetWordPackPath());
        return Directory.GetFiles(GetWordPackPath(), "*.json");
    }*/


    private void Awake() {
        if (s == null) {
            s = this;
            LoadWordPacks();
        }
    }

    private void Start() {
        DataSaver.s.CallOnLoad(OnLoad);
    }

    private void OnDestroy() {
        DataSaver.s.RemoveFromLoadCall(OnLoad);
        /*if (wordPacksLoaded) {
            SaveAllWordPacksToDisk();
        }*/
    }

    void OnLoad() {
        //LoadDefaultWordPacksFromResources(); // for updating the word packs
        /*if (!DataSaver.s.GetCurrentSave().isDefaultWordPacksLoaded) {
            LoadDefaultWordPacksFromResources();
            DataSaver.s.GetCurrentSave().isDefaultWordPacksLoaded = true;
            DataSaver.s.SaveActiveGame();
        } else {
            LoadWordPacks();
        }*/

        LoadWordPacks();
        wordPacksLoaded = true;
    }


    /*void LoadWordPacks() {
        var allPaths = GetAllWordPackPaths();
        allWordPacks = new List<WordPack>();

        Debug.Log($"Loading {allPaths.Length} word packs from {GetWordPackPath()}");

        for (int i = 0; i < allPaths.Length; i++) {
            var wordPack = DataSaver.ReadFile<WordPack>(allPaths[i]);
            
            Debug.Log( $"{wordPack.wordPairs.Count} words found in word pack {wordPack.wordPackName}" );

            FixWordPairIds(wordPack);
            allWordPacks.Add(wordPack);
        }

        loadedWordPacksText.text = allWordPacks.Count.ToString();
    }*/
    
    void LoadWordPacks() {
        allWordPacks = new List<WordPack>();

        for (int i = 0; i < allWordPackJsons.Count; i++) {
            var wordPack = DataSaver.ParseJson<WordPack>(allWordPackJsons[i].text);
            
            //Debug.Log( $"{wordPack.wordPairs.Count} words found in word pack {wordPack.wordPackName}" );

            FixWordPairIds(wordPack);
            allWordPacks.Add(wordPack);
        }

        UpdateWordCount();
        //loadedWordPacksText.text = allWordPacks.Count.ToString();
    }

    void UpdateWordCount() {
        var pack = PlayerLoadoutController.s.GetMainWordPack();

        currentWords.text = pack.wordPairs.Count.ToString();
    }

    void FixWordPairIds(WordPack wordPack) {
        int n = 0;
        //bool changeWasMade = false;
        for (int i = 0; i < wordPack.wordPairs.Count; i++) {
            var curPair = wordPack.wordPairs[i];

            if (curPair.id < 0) {
                curPair.id = n;
                //changeWasMade = true;
            } else {
                n = Mathf.Max(n, curPair.id);
            }
            n++;
        }
        
        /*if(changeWasMade)
            SaveWordPack(wordPack);*/
    }

    /*public void LoadDefaultWordPacksFromResources() {
        
        print("--- START LOADING WORDPACKS FROM RESOURCES");
        var wordPacks = Resources.LoadAll<TextAsset>(wordPackFolderName);
        
        Directory.CreateDirectory(GetWordPackPath());

        for (int i = 0; i < wordPacks.Length; i++) {
            var path = Path.Combine(GetWordPackPath(), wordPacks[i].name + ".json");
            File.WriteAllText(path, wordPacks[i].text);
            Debug.Log($"Word Pack -{wordPacks[i].name}- written to {path}");
        }

        print($"--- END LOADING WORD PACKS FROM RESOURCES total: {wordPacks.Length}");

        LoadWordPacks();
    }*/

    /*public static WordPack LoadWordPack(string wordPackName) {
        var path = Path.Combine(GetWordPackPath(), wordPackName + ".json");
        Debug.Log($"Load Word Pack:\"{wordPackName}\" from \"{path}\"");
        var wordPack = DataSaver.ReadFile<WordPack>(path);

        return wordPack;
    }

    public static void SaveWordPack(WordPack wordPack) {
        var path = Path.Combine(GetWordPackPath(), wordPack.wordPackName + ".json");
        Debug.Log($"Saving Word Pack:\"{wordPack.wordPackName}\" to \"{path}\"");
        DataSaver.WriteFile(path, wordPack);
    }*/

    /*[Button]
    public void SaveAllWordPacksToDisk() {
        int n = 0;
        foreach (var wordPack in allWordPacks) {
            if (n % 3 == 0) {
                wordPack.wordPackAffinity = "arcane";
            } else if(n % 3 == 1){
                wordPack.wordPackAffinity = "fire";
            } else {
                wordPack.wordPackAffinity = "ice";
            }

            n++;
            SaveWordPack(wordPack);
        }
    }*/
    
    [Button]
    public static void SaveMainWordPackToFile() {
        var wordPack = PlayerLoadoutController.s.GetMainWordPack();
        var path = Path.Combine(GetWordPackPath(), wordPack.wordPackName + ".json");
        Debug.Log($"Saving Word Pack:\"{wordPack.wordPackName}\" to \"{path}\"");
        DataSaver.WriteFile(path, wordPack);
    }
}
