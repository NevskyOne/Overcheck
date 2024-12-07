
using UnityEngine;
using UnityEngine.UI;

public class AntiCriminalTool : Tool
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _correct;
    [SerializeField] private Sprite _wrong;
    
    private void OnEnable()
    {
        NPCManager.OnGiveDocs += () =>
        {
            _image.sprite = NPCManager.CurrentNPC.IsCriminal ? _wrong : _correct;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
        };
        NPCManager.OnNPCCheck += () =>
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        };
    }
}
