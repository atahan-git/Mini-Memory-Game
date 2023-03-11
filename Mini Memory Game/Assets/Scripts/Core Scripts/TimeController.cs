using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeController : MonoBehaviour {

	public static TimeController s;

	public Volume volume;

	public Color slowDownColor = new Color(148, 255, 255);

	private void Awake() {
		s = this;
	}

	public void SetTimeScale(float timeScale) {
		Time.timeScale = timeScale;

		var profile = volume.profile;
		
		if (profile.TryGet(typeof(ChromaticAberration), out ChromaticAberration chromatic)) {
			var intensity = chromatic.intensity;
			intensity.value = timeScale.Remap(0, 1f, 0.2f, 0f);
		}
		
		if (profile.TryGet(typeof(Vignette), out Vignette vignette)) {
			var intensity = vignette.intensity;
			intensity.value = timeScale.Remap(0, 1f, 0.2f, 0f);
		}
		
		if (profile.TryGet(typeof(ColorAdjustments), out ColorAdjustments colorAdjustments)) {
			var colorFilter = colorAdjustments.colorFilter;
			colorFilter.Interp(slowDownColor, Color.white, timeScale);
		}
	}
}
