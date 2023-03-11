﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour {
	public static string versionTextValue;


	private void Awake() {
		versionTextValue = GetVersionNumber();
	}

	// Start is called before the first frame update
    void Start()
    {
		UpdateVersionText();
    }

	public TMP_Text version;
	public TextAsset versionText;
	void UpdateVersionText () {
		try {
			versionTextValue = GetVersionNumber();
			version.text = versionTextValue;
		} catch {
			Invoke("UpdateVersionText", 2f);
		}
	}

	string GetVersionNumber () {
		try {
			string content = versionText.text;

			if (content != null) {
				return content;
			} else {
				return " ";
			}
		} catch (System.Exception e) {
			Debug.LogError("Can't Get Version Number ");
		}
		return " ";
	}
}
