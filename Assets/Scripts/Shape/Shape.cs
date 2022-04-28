using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Shape : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler,IInitializePotentialDragHandler, IPointerUpHandler
{
    private Image _image;
    private RectTransform _rectTransform;
    private Vector3[] _shapeCorners = new Vector3[4];
    private ShapeFrame _frame;

    public Vector2 Position { get { return _rectTransform.position; } }
    public float LeftMargin { get; private set; }
    public float TopMargin { get; private set; }
    public float RightMargin { get; private set; }
    public float BottomMargin { get; private set; }
    public string Id { get; private set; }
    public string Name { get; private set; }
    public Color GetColor() => _image.color;

    public event Action<Shape> OnDragStarted;
    public event Action<Shape> OnDragEnded;
    public event Action<Shape> OnMouseDown;
    public event Action<Shape> OnMouseUp;
    public event Action<Vector2> OnMouseDrag;
    public event Action<float> OnHeightChanged;
    public event Action<float> OnWidthChanged;

    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            if (_isSelected == value) return;

            _isSelected = value;

            if (_isSelected)
                _frame.Show();
            else
                _frame.Hide();
        }
    }

    private void Awake()
    {
        _image = GetComponentsInChildren<Image>()[0];
        _rectTransform = GetComponent<RectTransform>();
        _frame = transform.GetComponentInChildren<ShapeFrame>();
    }

    public void SetMargins(float left, float top, float right, float bottom)
    {
        LeftMargin = left;
        TopMargin = top;
        RightMargin = right;
        BottomMargin = bottom;
    }

    public Vector3[] GetCorners()
    {
        _rectTransform.GetWorldCorners(_shapeCorners);
        return _shapeCorners;
    }

    public float Height
    {
        get { return _rectTransform.rect.height; }
        set 
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, value);
            OnHeightChanged?.Invoke(value);
        }
    }

    public float Width
    {
        get { return _rectTransform.rect.width; }
        set 
        {
            _rectTransform.sizeDelta = new Vector2(value, _rectTransform.sizeDelta.y);
            OnWidthChanged?.Invoke(value);
        }
    }

    public void UpdateData(ShapePrototypeData shapeData)
    {
        Id = shapeData.Id;
        Name = shapeData.Name;

        if (shapeData.Image)
        {
            _image.sprite = shapeData.Image;
            _image.color = Color.white;
        }
        else
        {
            _image.sprite = null;
            _image.color = Color.clear;
        }
    }

    public void UpdateData(ShapeSaveData shapeData)
    {
        Id = shapeData.Id;
        _image.color = new Color(shapeData.Color[0], shapeData.Color[1], shapeData.Color[2], shapeData.Color[3]);
        _rectTransform.position = new Vector2(shapeData.Position[0], shapeData.Position[1]);
        Height = shapeData.Height;
        Width = shapeData.Width;
    }

    public void ChangeColor(Color color)
    {
        _image.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseDown?.Invoke(this);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        OnDragStarted?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnMouseDrag?.Invoke(eventData.delta);

        if(!IsSelected)
            Move(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnded?.Invoke(this);
    }

    public void Move(Vector2 delta)
    {
        _rectTransform.anchoredPosition += delta;
    }

    public void MoveToPoint(float x, float y)
    {
        _rectTransform.position = new Vector2(x, y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseUp?.Invoke(this);
    }
}
