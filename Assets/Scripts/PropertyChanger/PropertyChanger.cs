using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyChanger : MonoBehaviour
{
    public abstract event Action<PropertyChanger> OnPropertyChanged;
    public abstract void ChangeShapeProperty(List<Shape> shapes);
    public abstract void RefreshVisualState(List<Shape> shapes);
}
