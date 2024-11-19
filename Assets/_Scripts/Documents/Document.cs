using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public abstract class Document : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _photoImage;
    
    protected string _name;
    protected Sprite _photo;
    protected bool _origin = true;
    protected int _paramCount = 2;
    protected Random _rnd = new Random();

    public virtual void Initialize(string name, Sprite photo, int planet)
    {
        _name = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
        _photo = photo;

        _nameText.text = _name;
        _photoImage.sprite = _photo;
    }

    public virtual void Randomize(int maxRandomCount)
    {
        _origin = false;
    }
}
