using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    private const string _fileName = "shapes.bin";
    private string _filePath;
    private BinaryFormatter _formatter;

    private List<ShapeSaveData> _saveShapes;

    public SaveSystem()
    {
        _formatter = new BinaryFormatter();
        _filePath = Application.persistentDataPath + "/" + _fileName;
        _saveShapes = new List<ShapeSaveData>();
    }

    public void Save(List<Shape> shapes)
    {
        _saveShapes.Clear();

        FileStream stream = new FileStream(_filePath, FileMode.Create);

        foreach(var shape in shapes)
        {
            Color shapeColor = shape.GetColor();

            var saveData = new ShapeSaveData();
            saveData.Color = new float[4] { shapeColor.r, shapeColor.g, shapeColor.b, shapeColor.a };
            saveData.Id = shape.Id;
            saveData.Position = new float[2] { shape.Position.x, shape.Position.y };
            saveData.Height = shape.Height;
            saveData.Width = shape.Width;
            _saveShapes.Add(saveData);
        }

        _formatter.Serialize(stream, _saveShapes);
        stream.Close();
    }

    public List<ShapeSaveData> Load()
    {
        _saveShapes.Clear();

        if (File.Exists(_filePath))
        {
            FileStream stream = new FileStream(_filePath, FileMode.Open);
            _saveShapes = _formatter.Deserialize(stream) as List<ShapeSaveData>;
            stream.Close();
        }
        else
        {
            Debug.LogError("File " + _filePath + " not find");
        }

        return _saveShapes;
    }
}
