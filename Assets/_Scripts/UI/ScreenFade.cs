using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    
    IEnumerator Start()
    {
        Image img = GetComponent<Image>();
        Color color = img.color;
        while (color.a < 1f)
        {
            color.a += _fadeSpeed * Time.deltaTime;
            img.color = color;
            yield return null;
        }
    }

}
