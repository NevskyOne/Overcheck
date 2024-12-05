using UnityEngine;
using UnityEngine.UI;

public class FaceScannerTool : Tool
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _correct;
    [SerializeField] private Sprite _wrong;
    
    private void OnEnable()
    {
        NPCManager.NPCAtTable += () =>
        {
            _image.sprite = NPCManager.CurrentNPC.FaceChanged ? _wrong : _correct;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
        };
        NPCManager.OnNPCCheck += () =>
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        };
    }

    
}