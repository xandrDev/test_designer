using UnityEngine;

[CreateAssetMenu(fileName = "ShapePrototypeData", menuName = "ScriprableObjects/ShapePrototypeData", order = 1)]
public class ShapePrototypeData : ScriptableObject
{
    [SerializeField]
    private string _id = "Object";
    [SerializeField]
    private string _name = "Name";
    [SerializeField]
    private Sprite _image = null;
    
    public string Id => _id;
    public string Name => _name;
    public Sprite Image => _image;
}
