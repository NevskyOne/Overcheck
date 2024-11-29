using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    [SerializeField][Range(0,1)] private float _maxValue;
        
    private void OnEnable() => StartCoroutine(StartFade());

    private IEnumerator StartFade()
    {
        Image img = GetComponent<Image>();
        Color color = img.color;
        color.a = 0f;
        img.color = color;
        while (color.a < _maxValue)
        {
            color.a += _fadeSpeed * Time.deltaTime;
            img.color = color;
            yield return null;
        }
    }
    public IEnumerator EndFade()
    {
        Image img = GetComponent<Image>();
        Color color = img.color;
        color.a = _maxValue;
        img.color = color;
        while (color.a > 0f)
        {
            color.a -= _fadeSpeed * Time.deltaTime;
            img.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
