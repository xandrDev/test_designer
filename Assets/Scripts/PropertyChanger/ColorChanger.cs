using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ColorChanger : PropertyChanger, IPointerClickHandler
{
    private RectTransform _rectTransform;
    private Texture2D _colorTexture;

    private Color _currentColor;

    public override event Action<PropertyChanger> OnPropertyChanged;

    //public event Action<Color> OnColorChanged;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _colorTexture = GetComponent<Image>().mainTexture as Texture2D;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, null, out delta);

        delta += new Vector2(_rectTransform.rect.width * 0.5f, _rectTransform.rect.height * 0.5f);

        float x = Mathf.Clamp(delta.x / _rectTransform.rect.width, 0f, 1f);
        float y = Mathf.Clamp(delta.y / _rectTransform.rect.height, 0f, 1f);

        int texX = Mathf.RoundToInt(x * _colorTexture.width);
        int texY = Mathf.RoundToInt(y * _colorTexture.height);

        _currentColor = _colorTexture.GetPixel(texX, texY);

        OnPropertyChanged?.Invoke(this);
    }

    public override void ChangeShapeProperty(List<Shape> shapes)
    {
        foreach(var shape in shapes)
            shape.ChangeColor(_currentColor);
    }

    public override void RefreshVisualState(List<Shape> shapes)
    {
        
    }
}
