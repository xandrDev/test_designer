using System.Collections.Generic;
using UnityEngine;

public class ShapeCollection
{
    private List<Shape> _shapes;
    private List<Shape> _selectedShapes;

    public List<Shape> GetShapes() => _shapes;
    public List<Shape> GetSelectedShapes() => _selectedShapes;

    public int Count { get { return _shapes.Count; } }
    public int SelectedCount { get { return _selectedShapes.Count; } }

    public ShapeCollection()
    {
        _shapes = new List<Shape>();
        _selectedShapes = new List<Shape>();
    }

    public Shape GetShape(int index)
    {
        if(_shapes.Count >= index)
            return _shapes[index];
        return null;
    }

    public bool AddShape(Shape shape)
    {
        if (!_shapes.Contains(shape))
        {
            _shapes.Add(shape);
            return true;
        }

        return false;
    }

    public Shape GetSelectedShape(int index)
    {
        if (_selectedShapes.Count >= index)
            return _selectedShapes[index];
        return null;
    }

    public void AddSelectedShape(Shape shape)
    {
        _selectedShapes.Add(shape);
    }

    public void RemoveSelectedShape(Shape shape)
    {
        if (_selectedShapes.Contains(shape))
            _selectedShapes.Remove(shape);
    }

    public void Remove(Shape shape)
    {
        if (_shapes.Contains(shape))
            _shapes.Remove(shape);

        if (_selectedShapes.Contains(shape))
            _selectedShapes.Remove(shape);
    }
}