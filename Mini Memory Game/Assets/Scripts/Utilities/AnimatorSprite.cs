using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorSprite : MonoBehaviour {

    public Sprite[] sprites;

    public float delay = 1;

    public bool doAnim = true;

    private float curDelay;
    private int curIndex;

    private SpriteRenderer _renderer;
    private Image _renderer2;
    // Start is called before the first frame update
    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer2 = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (curDelay > delay) {
            GoToNextSprite();

            curDelay = 0;
        }

        if(doAnim)
            curDelay += Time.deltaTime;
    }


    [Button]
    public void GoToNextSprite() {
        curIndex += 1;
        curIndex %= sprites.Length;
        if(_renderer != null)
            _renderer.sprite = sprites[curIndex];
        if (_renderer2 != null)
            _renderer2.sprite = sprites[curIndex];
    }
}
