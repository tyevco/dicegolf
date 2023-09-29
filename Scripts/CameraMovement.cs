using Godot;

namespace PleaseKissMyElbow;

public partial class CameraMovement : Node3D
{
    [Export] public Node3D TrackingNode { get; set; }

    [Export] public Vector3 Offset { get; set; }
    [Export(PropertyHint.Range, "0,100,1")] public int HorizontalMouseSensitivity { get; set; } = 25;
    [Export(PropertyHint.Range, "0,100,1")] public int VerticalMouseSensitivity { get; set; } = 25;

    [Export] public NodePath Occlusion { get; set; }
    [Export] public NodePath CameraPath { get; set; }
    private Node3D Camera { get; set; }
    private RayCast3D OcclusionRay { get; set; }

    private float _horzMouseSensitivity;
    private float _vertMouseSensitivity;
    
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        
        Camera = GetNode(CameraPath) as Node3D;
        OcclusionRay = GetNode(Occlusion) as RayCast3D;

        _horzMouseSensitivity = HorizontalMouseSensitivity / 100f;
        _vertMouseSensitivity = VerticalMouseSensitivity / 100f;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _horzMouseSensitivity));
            Camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * _vertMouseSensitivity));
            var rotation = Camera.Rotation;
            rotation.X = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-60), Mathf.DegToRad(-15));
            // GD.Print($"{Mathf.RadToDeg(rotation.X)}:{Mathf.RadToDeg(Camera.Rotation.X)}");
            Camera.Rotation = rotation;
        }

        //GD.Print($"{Camera.Position}; {Camera.Rotation} | {Position}; {Rotation}");
    }

    public override void _Process(double delta)
    {
        var cameraMount = TrackingNode.GetNode("cameraMount");
        if (this.GetParent() != cameraMount)
        {
            this.GetParent().RemoveChild(this);
            cameraMount.AddChild(this);
        }

        // GD.Print(Position);
    }
}