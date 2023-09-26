using Godot;

namespace PleaseKissMyElbow;

public partial class CameraMovement : Node3D
{
    [Export] public Node3D TrackingNode { get; set; }

    [Export] public Vector3 Offset { get; set; }
    [Export] public float HorizontalMouseSensitivity { get; set; } = 0.25f;
    [Export] public float VerticalMouseSensitivity { get; set; } = 0.25f;

    [Export] public NodePath Occlusion { get; set; }
    [Export] public NodePath CameraPath { get; set; }
    private Node3D Camera { get; set; }
    private RayCast3D OcclusionRay { get; set; }

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Camera = GetNode(CameraPath) as Node3D;
        OcclusionRay = GetNode(Occlusion) as RayCast3D;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * HorizontalMouseSensitivity));
            Camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * VerticalMouseSensitivity));
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

        var collider = OcclusionRay.GetCollider() as Node3D;
        if (collider != null)
            GD.Print(collider.GetPath());

        OcclusionRay.Position = TrackingNode.Position;
        OcclusionRay.TargetPosition = Camera.GlobalPosition;


        // GD.Print(Position);
    }
}