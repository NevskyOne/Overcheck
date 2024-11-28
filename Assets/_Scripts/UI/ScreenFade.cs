using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;

    private void OnEnable() => StartCoroutine(StartFade());

    private IEnumerator StartFade()
    {
        Image img = GetComponent<Image>();
        Color color = img.color;
        color.a = 0f;
        img.color = color;
        while (color.a < 1f)
        {
            color.a += _fadeSpeed * Time.deltaTime;
            img.color = color;
            yield return null;
        }
    }

}
