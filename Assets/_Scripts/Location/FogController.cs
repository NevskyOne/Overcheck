using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class FogController : MonoBehaviour
{
    private static readonly int DensityMultiplier = Shader.PropertyToID("_DensityMultiplier");
    [SerializeField] private Material[] _fogMaterials;
    [SerializeField] private float _maxDensity = 0.01f;
    
    private void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        foreach (var fog in _fogMaterials)
        {
            StartCoroutine(FogTransition(fog, _maxDensity));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        foreach (var fog in _fogMaterials)
        {
            StartCoroutine(FogTransition(fog, 0));
        }
    }
    
    private IEnumerator FogTransition(Material fog, float toDensity)
    {
        if (toDensity == 0)
        {
            while (fog.GetFloat(DensityMultiplier) > toDensity)
            {
                fog.SetFloat(DensityMultiplier, fog.GetFloat(DensityMultiplier) - 0.001f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (fog.GetFloat(DensityMultiplier) < toDensity) 
            {
                fog.SetFloat(DensityMultiplier, fog.GetFloat(DensityMultiplier) + 0.001f);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
