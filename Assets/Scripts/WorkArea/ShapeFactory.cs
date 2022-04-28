using System.Collections.Generic;
using UnityEngine;

public class ShapeFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject _shapePrafab = null;
    [SerializeField]
    private Transform _shapeProtorypesContainer = null;
    [SerializeField]
    private WorkArea _workArea = null;
    [SerializeField]
    private List<ShapePrototypeData> _shapePrototypes = new List<ShapePrototypeData>();

    private Dictionary<string, int> _shapeIdToIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        for (int i = 0; i < _shapePrototypes.Count; i++)
        {
            CreateShapePrototype(i);
            _shapeIdToIndexMap.Add(_shapePrototypes[i].Id, i);
        }
    }

    private void Shape_OnDragStarted(Shape shape)
    {
        shape.transform.SetParent(_workArea.transform);
        CreateShapePrototype(_shapeIdToIndexMap[shape.Id]);
        shape.OnDragStarted -= Shape_OnDragStarted;
        shape.OnDragEnded += Shape_OnDragEnded;
    }

    private void Shape_OnDragEnded(Shape shape)
    {
        shape.OnDragEnded -= Shape_OnDragEnded;

        if (!_workArea.CheckShapeInWorkArea(shape))
            Destroy(shape.gameObject);
        else
            _workArea.AddShape(shape);
    }

    private void CreateShapePrototype(int index)
    {
        Shape shape = InstantiateShape(_shapeProtorypesContainer, _shapePrototypes[index]);
        shape.transform.SetSiblingIndex(index);
        shape.OnDragStarted += Shape_OnDragStarted;
    }

    public Shape CreateLoadedShape(ShapeSaveData shapeSaveData)
    {
        Shape shape = InstantiateShape(_workArea.transform, _shapePrototypes[_shapeIdToIndexMap[shapeSaveData.Id]]);
        shape.UpdateData(shapeSaveData);
        return shape;
    }

    private Shape InstantiateShape(Transform parent, ShapePrototypeData data)
    {
        Shape shape = Instantiate(_shapePrafab, parent).GetComponent<Shape>();
        shape.UpdateData(data);
        return shape;
    }
}
