using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransformationChanger : PropertyChanger
{
    private InputField _heightInputField;
    private InputField _widthInputField;

    public override event Action<PropertyChanger> OnPropertyChanged;

    private bool _isActive = true;
    private float _minHeight = 5f;
    private float _minWidth = 5f;

    private string _validateCharacters = "0123456789";

    public bool IsActive 
    { 
        get { return _isActive; }
        set
        {
            if (_isActive == value) return;

            _isActive = value;
            gameObject.SetActive(_isActive);
        }
    }

    private void Awake()
    {
        _heightInputField = GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "HeightInputField")?.gameObject.GetComponent<InputField>();
        _widthInputField = GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "WidthInputField")?.gameObject.GetComponent<InputField>(); ;

        _heightInputField.onEndEdit.AddListener(delegate 
        {
            float.TryParse(_heightInputField.text, out float value);
            if (value < _minHeight)
                _heightInputField.text = _minHeight.ToString();

            OnPropertyChanged?.Invoke(this);
        });
        _heightInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateChar(_validateCharacters, addedChar); };

        _widthInputField.onEndEdit.AddListener(delegate 
        {
            float.TryParse(_widthInputField.text, out float value);
            if (value < _minWidth)
                _widthInputField.text = _minWidth.ToString();

            OnPropertyChanged?.Invoke(this);
        });
        _widthInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateChar(_validateCharacters, addedChar); };
    }

    private char ValidateChar(string validCharacters, char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
            return addedChar;
        else
            return '\0';
    }

    public void SetHeight(float height)
    {
        _heightInputField.text = height.ToString();
    }

    public void SetWidth(float width)
    {
        _widthInputField.text = width.ToString();
    }

    public override void ChangeShapeProperty(List<Shape> shapes)
    {
        foreach(Shape shape in shapes)
        {
            shape.Width = float.Parse(_widthInputField.text);
            shape.Height = float.Parse(_heightInputField.text);
        }
    }

    public override void RefreshVisualState(List<Shape> shapes)
    {
        if(shapes.Count == 1)
        {
            SetHeight(shapes[0].Height);
            SetWidth(shapes[0].Width);
        }
        else
        {
            SetHeight(_minHeight);
            SetWidth(_minWidth);
        }
    }
}
