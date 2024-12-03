
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Effects : MonoBehaviour
{
    [SerializeField] private VolumeProfile _profile;

    public IEnumerator ChangeChromatic(float value)
    {
        if (!_profile.TryGet<ChromaticAberration>(out var chromatic)) yield break;
        while (Mathf.Approximately(chromatic.intensity.value, value))
        {
            var chValue = chromatic.intensity.value;
            
            
            chromatic.intensity.Override(chValue < value ? chValue + 0.05f : chValue - 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
