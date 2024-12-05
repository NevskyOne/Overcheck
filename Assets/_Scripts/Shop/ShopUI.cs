using UnityEngine;
using UnityEngine.UI;


public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform _scroll;
    [SerializeField] private Image _imageLeft, _imageRight;

    
    public void ScrollLeft()
    {
        if(_imageLeft.color == Color.grey) return;
        _scroll.localPosition = new Vector3(_scroll.localPosition.x + 720, 0, 0);
        _imageLeft.color = _scroll.localPosition.x == 0 ? Color.grey : Color.white;
        
        _imageRight.color = Color.white;
    }
    public void ScrollRight()
    {
        if(_imageRight.color == Color.grey) return;
        _scroll.localPosition = new Vector3(_scroll.localPosition.x - 720, 0, 0);
        _imageRight.color = _scroll.localPosition.x == -2880 ? Color.grey : Color.white;
        
        _imageLeft.color = Color.white;
    }
}
