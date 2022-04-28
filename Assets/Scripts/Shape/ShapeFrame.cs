using UnityEngine;
using UnityEngine.UI;

public class ShapeFrame : MonoBehaviour
{
    private Image _image;
    private RectTransform _rectTransform;
    private Shape _shape;

    public int Padding = 1;

    private void Awake()
    {
        _shape = transform.parent.GetComponent<Shape>();
        _shape.OnHeightChanged += Shape_OnHeightChanged;
        _shape.OnWidthChanged += Shape_OnWidthChanged;

        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        Hide();
    }

    private void Shape_OnWidthChanged(float shapeWidth)
    {
        _rectTransform.sizeDelta = new Vector2(shapeWidth + Padding, _rectTransform.sizeDelta.y);
    }

    private void Shape_OnHeightChanged(float shapeHeight)
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, shapeHeight + Padding);
    }

    public void Show()
    {
        _image.enabled = true;
    }

    public void Hide()
    {
        _image.enabled = false;
    }
}
