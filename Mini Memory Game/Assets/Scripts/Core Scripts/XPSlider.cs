using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPSlider : MonoBehaviour {
    public static XPSlider s;

    private void Awake() {
        s = this;
    }

    public Slider seenSlider;
    public Slider masteredSlider;
    public Slider needReviewSlider;

    public TMP_Text overallProgress;

    private void Start() {
        ProgressUpdated();
        WordProvider.s.OnProgressUpdated.AddListener(ProgressUpdated);
    }

    public void ProgressUpdated() {
        overallProgress.text = GetProgressPercent();
    }

    public float GetProgressValue() {
        var activePacks = PlayerLoadoutController.s.GetAllPacks();

        var totalWords = 0;
        var totalLearned = 0;
        var totalMastered = 0;
        var totalNeedReview = 0;

        for (int i = 0; i < activePacks.Count; i++) {
            var stats = new WordPackStats(activePacks[i]);
            totalWords += stats.totalWordCount;
            totalLearned += stats.learnedWordCount;
            totalMastered += stats.masteredWordCount;
            totalNeedReview += stats.needPracticeCount;
        }


        seenSlider.value = totalLearned / (float)totalWords;
        masteredSlider.value = totalMastered / (float)totalWords;
        needReviewSlider.value = totalNeedReview / (float)totalWords;

        var progressValue = (totalLearned / (float)totalWords )+ (totalMastered / (float)totalWords);
        progressValue /= 2f;

        return progressValue;
    }

    public string GetProgressPercent() {
        return $"{GetProgressValue() * 100:F1}%";
    }
}
