using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CoinGraphics : MonoBehaviour
{
    public SpriteAnimationHolder spritesHolder;

    public float delay = 0.3f;
    
    private float curDelay;
    private int curIndex;

    private SpriteRenderer _renderer;
    private Image _renderer2;
    // Start is called before the first frame update
    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer2 = GetComponent<Image>();
        PlayerLoadoutController.s.OnGraphicsLoadoutChanged.AddListener(RefreshGraphicsLoadout);
        RefreshGraphicsLoadout();
    }

    void RefreshGraphicsLoadout() {
        spritesHolder = PlayerLoadoutController.s.coinGraphics;
    }

    private void OnDestroy() {
        PlayerLoadoutController.s.OnGraphicsLoadoutChanged.RemoveListener(RefreshGraphicsLoadout);
    }

    // Update is called once per frame
    void Update() {
        if (curDelay > delay) {
            GoToNextSprite();

            curDelay = 0;
        }

        curDelay += Time.deltaTime;
    }


    [Button]
    public void GoToNextSprite() {
        curIndex += 1;
        var sprites = spritesHolder.sprites;
        curIndex %= sprites.Length;
        SetSprite(sprites[curIndex]);
    }

    [Button]
    void SetSpriteEditor() {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer2 = GetComponent<Image>();
        var sprites = spritesHolder.sprites;
        curIndex %= sprites.Length;
        SetSprite(sprites[curIndex]);
    }

    void SetSprite(Sprite toSet) {
        if(_renderer != null)
            _renderer.sprite = toSet;
        if (_renderer2 != null)
            _renderer2.sprite = toSet;
    }
}
