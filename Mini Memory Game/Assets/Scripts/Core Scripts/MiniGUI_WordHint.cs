using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGUI_WordHint : MonoBehaviour {
   public TMP_Text displayText;

   public WordWrapper myWord;
	
   public void Setup(WordWrapper word) {
      myWord = word;
   }

   private void Update() {
      displayText.text = myWord.GetAnswerSide();
   }

   public void UseHint() {
      GetComponentInParent<CustomVerticalLayoutGroup>().RemoveObject(gameObject);
   }
}
