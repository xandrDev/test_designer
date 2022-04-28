using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkArea : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Transform _selectionAreaTransform = null;
    [SerializeField]
    private ShapeFactory _shapeFactory = null;
    [SerializeField]
    private List<PropertyChanger> _propertyChangers = null;

    private SaveSystem _saveSystem;
    private RectTransform _rectTransform;
    private ShapeCollection _shapeCollection;
    private Vector2 startMousePosition;
    private Vector2 lowerLeftCorner;
    private Vector2 upperRightCorner;
    private Vector3[] _areaCorners = new Vector3[4];
    private Vector3[] _shapeCorners = new Vector3[4];

    private void Awake()
    {
        _saveSystem = new SaveSystem();

        _shapeCollection = new ShapeCollection();
        _selectionAreaTransform.gameObject.SetActive(false);

        _rectTransform = GetComponent<RectTransform>();
        
        foreach(var propertyChanger in _propertyChangers)
            propertyChanger.OnPropertyChanged += PropertyChanger_OnPropertyChanged;
    }

    private void PropertyChanger_OnPropertyChanged(PropertyChanger changer)
    {
        changer.ChangeShapeProperty(_shapeCollection.GetSelectedShapes());
    }

    public void AddShape(Shape shape)
    {
        if (_shapeCollection.AddShape(shape))
            AddShapeHandlers(shape);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startMousePosition = eventData.position;
        UnselectAllShapes();
    }

    public void Clear()
    {
        for (int i = _shapeCollection.Count - 1; i >= 0; i--)
            RemoveShape(_shapeCollection.GetShape(i));
    }

    public void RemoveSelectedShapes()
    {
        for (int i = _shapeCollection.SelectedCount - 1; i >= 0; i--)
            RemoveShape(_shapeCollection.GetSelectedShape(i));
    }

    private void RemoveShape(Shape shape)
    {
        if (shape == null) return;

        RemoveShapeHandlers(shape);
        Destroy(shape.gameObject);
        _shapeCollection.Remove(shape);
    }

    private void RemoveShapeHandlers(Shape shape)
    {
        shape.OnMouseDown += Shape_OnMouseDown;
        shape.OnMouseDrag -= Shape_OnMouseDrag;
    }

    private void AddShapeHandlers(Shape shape)
    {
        shape.OnMouseDown += Shape_OnMouseDown;
        shape.OnMouseDrag += Shape_OnMouseDrag;
    }

    private void Shape_OnMouseDown(Shape shape)
    {
        if (!shape.IsSelected)
        {
            UnselectAllShapes();
            SelectShape(shape);
            RefreshPropertyChangers();
        }
    }

    public void UnselectAllShapes()
    {
        for (int i = _shapeCollection.SelectedCount - 1; i >= 0; i--)
        {
            Shape shape = _shapeCollection.GetSelectedShape(i);
            shape.IsSelected = false;
            _shapeCollection.RemoveSelectedShape(shape);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var currentMousePosition = eventData.position;

        lowerLeftCorner = new Vector2(Mathf.Min(startMousePosition.x, currentMousePosition.x), Mathf.Min(startMousePosition.y, currentMousePosition.y));
        upperRightCorner = new Vector2(Mathf.Max(startMousePosition.x, currentMousePosition.x), Mathf.Max(startMousePosition.y, currentMousePosition.y));

        _selectionAreaTransform.position = lowerLeftCorner;
        _selectionAreaTransform.localScale = upperRightCorner - lowerLeftCorner;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _selectionAreaTransform.gameObject.SetActive(false);

        for(int i = 0; i < _shapeCollection.Count; i++)
        {
            Shape shape = _shapeCollection.GetShape(i);
            if (shape.Position.x > lowerLeftCorner.x && shape.Position.y > lowerLeftCorner.y &&
                shape.Position.x < upperRightCorner.x && shape.Position.y < upperRightCorner.y)
                SelectShape(shape);
        }

        RefreshPropertyChangers();
    }

    private void RefreshPropertyChangers()
    {
        foreach (var propertyChanger in _propertyChangers)
            propertyChanger.RefreshVisualState(_shapeCollection.GetSelectedShapes());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _selectionAreaTransform.gameObject.SetActive(true);
    }

    private void Shape_OnMouseDrag(Vector2 delta)
    {
        MoveSelectedShapes(delta);
    }

    private void SelectShape(Shape shape)
    {
        shape.IsSelected = true;
        _shapeCollection.AddSelectedShape(shape);
    }

    public void MoveSelectedShapes(Vector2 delta)
    {
        CorrectDelta(ref delta);

        for (int i = 0; i < _shapeCollection.SelectedCount; i++)
            _shapeCollection.GetSelectedShape(i).Move(delta);
    }

    private Vector2 CorrectDelta(ref Vector2 delta)
    {
        for (int i = 0; i < _shapeCollection.SelectedCount; i++)
        {
            Shape shape = _shapeCollection.GetSelectedShape(i);

            if (delta.x < 0)
            {
                if (delta.x + shape.LeftMargin < 0)
                    delta.x = -1f * shape.LeftMargin;
            }
            else
            {
                if (shape.RightMargin - delta.x < 0)
                    delta.x = shape.RightMargin;
            }

            if (delta.y < 0)
            {
                if (delta.y + shape.BottomMargin < 0)
                    delta.y = -1f * shape.BottomMargin;
            }
            else
            {
                if (shape.TopMargin - delta.y < 0)
                    delta.y = shape.TopMargin;
            }
        }

        return delta;
    }

    private void Update()
    {
        _rectTransform.GetWorldCorners(_areaCorners);

        UpdateShapesMargins();
    }

    private void UpdateShapesMargins()
    {
        for (int i = 0; i < _shapeCollection.Count; i++)
        {
            Shape shape = _shapeCollection.GetShape(i);

            _shapeCorners = shape.GetCorners();
            float leftMargin = _shapeCorners[0].x < _areaCorners[0].x ? 0f : Mathf.Abs(_areaCorners[0].x - _shapeCorners[0].x);
            float topMargin = _shapeCorners[1].y > _areaCorners[1].y ? 0f : _areaCorners[1].y - _shapeCorners[1].y;
            float rightMargin = _shapeCorners[3].x > _areaCorners[3].x ? 0f : _areaCorners[3].x - _shapeCorners[3].x;
            float bottomMargin = _shapeCorners[0].y < _areaCorners[0].y ? 0f : Mathf.Abs(_areaCorners[0].y - _shapeCorners[0].y);
            shape.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
        }
    }

    public void SaveShapes()
    {
        _saveSystem.Save(_shapeCollection.GetShapes());
    }

    public void LoadShapes()
    {
        Clear();

        var loadShapes = _saveSystem.Load();

        foreach (var shape in loadShapes)
            AddShape(_shapeFactory.CreateLoadedShape(shape));
    }

    public bool CheckShapeInWorkArea(Shape shape)
    {
        _shapeCorners = shape.GetCorners();
        if (_shapeCorners[0].x < _areaCorners[0].x)
            return false;
        if (_shapeCorners[3].x > _areaCorners[3].x)
            return false;
        if (_shapeCorners[0].y < _areaCorners[0].y)
            return false;
        if (_shapeCorners[1].y > _areaCorners[1].y)
            return false;

        return true;
    }
}
