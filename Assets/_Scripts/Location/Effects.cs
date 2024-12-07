
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Effects : MonoBehaviour
{
    [SerializeField] private VolumeProfile _profile;
    [SerializeField] private Vector4 _mainGamma;
    [SerializeField] private Vector4 _redGamma;
    
    public IEnumerator ChangeChromatic(float value)
    {
        if (!_profile.TryGet<ChromaticAberration>(out var chromatic)) yield break;
        while (Mathf.Abs(chromatic.intensity.value - value) > 0.04f)
        {
            var chValue = chromatic.intensity.value;
            chromatic.intensity.Override(chValue < value ? chValue + 0.05f : chValue - 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    public IEnumerator ChangeGamma(bool red = false)
    {
        if (!_profile.TryGet<LiftGammaGain>(out var gamma)) yield break;
        var targetGamma = red ? _redGamma : _mainGamma;
        while (gamma.gamma.value != targetGamma)
        {
            var value = gamma.gamma.value;
            gamma.gamma.Override(Vector4.Lerp(value, targetGamma,0.05f));
            yield return new WaitForSeconds(0.05f);
        }
    }
}
