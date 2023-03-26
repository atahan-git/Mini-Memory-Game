using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WordProvider : MonoBehaviour {
    public static WordProvider s;

    private void Awake() {
        s = this;
    }

    public List<WordWrapper> activeWords = new List<WordWrapper>();

    public List<MiniGUI_WordHint> allHints = new List<MiniGUI_WordHint>();
    public CustomVerticalLayoutGroup wordHintArea;
    public GameObject wordHintPrefab;


    public int maxHints = 3;
    public int maxActiveWords = 10;
    public int maxTotalActiveWordLength = 20;

    [HideInInspector]
    public UnityEvent OnHintSpawned;
    [HideInInspector]
    public UnityEvent OnProgressUpdated ;

    public GameObject coin;
    public GameObject noCoin;

    /// <summary>
    /// return null if there are too many active words.
    /// </summary>
    public WordWrapper GetWord() {
        if (activeWords.Count >= maxActiveWords)
            return null;

        /*var totalActiveWordLength = GetTotalActiveWordLength();

        if (totalActiveWordLength > maxTotalActiveWordLength)
            return null;*/
        
        var wordPack = PlayerLoadoutController.s.GetRandomPack();
        if (wordPack.wordPairs.Count <= 0) {
            JPDB_Bridge.s.FetchWordsFromJPDB();
            return null;
        }
        
        var userPackProgress = DataSaver.s.GetCurrentSave().GetProgress(wordPack);

        var banList = new List<int>();

        var canShowNewWord = allHints.Count < maxHints;

        var wordPair = Scheduler.GetNextWordPair(wordPack, userPackProgress, true, banList, canShowNewWord);
        var userWordPairProgress = userPackProgress.GetWordPairData(wordPair);
        
        var wordWrapper = new WordWrapper() {
            pack = wordPack,
            packProgress = userPackProgress,
            pair = wordPair,
            pairProgress = userWordPairProgress
        };
        

        if (userWordPairProgress.learningMeaningSideType == WordPairProgressType.newWord) {
            SpawnHint(wordWrapper);
            OnHintSpawned?.Invoke();
        }
        
        activeWords.Add(wordWrapper);

        CheckGetNewWords(wordPack);
        
        return wordWrapper;
    }

    private int GetTotalActiveWordLength() {
        var totalActiveWordLength = 0;
        for (int i = 0; i < activeWords.Count; i++) {
            // 心<pos=0em><sup>しん</sup><pos=1em>理<pos=1em><sup>り</sup><pos=2em>学<pos=2em><sup>がく</sup><pos=2em>
            var sanitized = activeWords[i].GetQuestionSide()
                .Replace("pos", "").Replace("sup", "")
                .Replace("<", "").Replace(">", "")
                .Replace("em", "").Replace("/", "");
            totalActiveWordLength += sanitized.Length;
        }

        return totalActiveWordLength;
    }

    void CheckGetNewWords(WordPack pack) {
        if (!PlayerLoadoutController.s.hiraganaKatakanaOnlyMode) {
            var stats = new WordPackStats(pack);

            if (stats.notSeenCount < JPDB_Bridge.newWordFetchCount) {
                JPDB_Bridge.s.FetchWordsFromJPDB();
            }
        }
    }


    public void RemoveWord(WordWrapper wordWrapper, bool isCorrect) {
        
        Scheduler.RegisterResult(wordWrapper.pack, wordWrapper.packProgress, wordWrapper.pair, true, isCorrect);

        activeWords.Remove(wordWrapper);
        
        OnProgressUpdated?.Invoke();

        if (isCorrect) {
            for (int i = 0; i < allHints.Count; i++) {
                if (allHints[i].myWord.GetAnswerSide() == wordWrapper.GetAnswerSide()) {
                    allHints[i].UseHint();
                    allHints.RemoveAt(i);
                    break;
                }
            }
            
        } else {
            OnHintSpawned?.Invoke();
            SpawnHint(wordWrapper);
        }
    }

    public void SpawnCoin(bool isCorrect, Vector3 location) {
        if (isCorrect) {
            Instantiate(coin, transform).transform.position = location;
            
        } else {
            Instantiate(noCoin, transform).transform.position = location;
        }
    }

    public void SpawnHint(WordWrapper wordWrapper) {
        var newHint = Instantiate(wordHintPrefab);
        wordHintArea.AddNewObject(newHint);
        allHints.Add(newHint.GetComponent<MiniGUI_WordHint>());
        newHint.GetComponent<MiniGUI_WordHint>().Setup(wordWrapper);
    }
}


public class WordWrapper {
    public WordPack pack;
    public DataSaver.UserWordPackProgress packProgress;
    public WordPair pair;
    public DataSaver.UserWordPairProgress pairProgress;
    private const bool isLearningMeaningSide = true;


    public string GetQuestionSide() {
        return isLearningMeaningSide ? pair.word : pair.meaning;
    }

    public string GetAnswerSide() {
        return !isLearningMeaningSide ? pair.word : pair.meaning;
    }

    public float GetExtraTimeDelay() {
        return GetAnswerSide().Length * 0.15f;
    }

    public float GetSpeedMultiplier() {
        return Mathf.Max(1, GetAnswerSide().Length * 0.15f);
    }
}
