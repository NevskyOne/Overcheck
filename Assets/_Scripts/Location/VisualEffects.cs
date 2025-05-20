
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisualEffects : MonoBehaviour
{
    [SerializeField] private VolumeProfile _profile;
    [SerializeField] private Vector4 _mainGamma;
    [SerializeField] private Vector4 _redGamma;

    public void ChangeChromatic(float value)
    {
        StopCoroutine("ChangeChromaticRoutine");
        StartCoroutine(ChangeChromaticRoutine(value));
    }
    
    private IEnumerator ChangeChromaticRoutine(float value)
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

    public void Starve(float motionI, float lensI,float grainI,float paniniI)
    {
        _profile.TryGet<MotionBlur>(out var motion);
        motion.intensity.value = motionI;
        _profile.TryGet<LensDistortion>(out var lens);
        lens.intensity.value = lensI;
        _profile.TryGet<FilmGrain>(out var grain);
        grain.intensity.value = grainI;
        _profile.TryGet<PaniniProjection>(out var panini);
        panini.distance.value = paniniI;
    }

    private void OnDisable()
    {
        Starve(0f, 0, 0, 0);
    }
}
