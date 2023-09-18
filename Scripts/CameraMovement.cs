using Godot;

namespace PleaseKissMyElbow;

public partial class CameraMovement : Node3D
{
	[Export] public Node TrackingNode { get; set; }

	[Export] public Vector3 Offset { get; set; }
	[Export] public float HorizontalMouseSensitivity { get; set; } = 0.25f;
	[Export] public float VerticalMouseSensitivity { get; set; } = 0.25f;
	
	private Node3D Camera { get; set; }
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Camera = GetNode("Camera3D") as Node3D;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * HorizontalMouseSensitivity));
			Camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * VerticalMouseSensitivity));
			
//			GD.Print($"{mouseMotion.Relative}");
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
