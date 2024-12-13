
using UnityEngine;

public class NPCRandomizer : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Mesh[] _models;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Material[] _acesMaterials;
    
    public void Randomize(Transform npc)
    {
        var meshRenderer = npc.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();
        var randomModel = Random.Range(0, 10);
        meshRenderer.sharedMesh = _models[randomModel];

        var randomMat = Random.Range(0, _materials.Length);
        meshRenderer.SetMaterials(new()
        {
            _materials[randomMat],
            _materials[randomMat],
            _acesMaterials[Random.Range(0, _acesMaterials.Length)]
        });
        NPCManager.CurrentNPC.Photo = RandomParamSt.Photos[(randomModel) * 10 + (int)Mathf.Ceil((randomMat + 1) / 3f) - 1];

        int randomHat = Random.Range(0, 4);
        if (randomHat < 3)
            meshRenderer.SetBlendShapeWeight(randomHat, 100);
        int randomCount = Random.Range(0, 5);
        if (randomCount > 0)
        {
            for (var i = 0; i < randomCount; i++)
                meshRenderer.SetBlendShapeWeight(Random.Range(3, 7), 100);
        }
    }
}