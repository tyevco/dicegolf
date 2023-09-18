using Godot;

namespace PleaseKissMyElbow;

public partial class CameraMovement : Camera3D
{
    [Export] public Node TrackingNode { get; set; }

    [Export] public Vector3 Offset { get; set; }

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