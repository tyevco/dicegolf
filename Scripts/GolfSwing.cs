using System;
using Godot;

namespace PleaseKissMyElbow;

public partial class GolfSwing : Node
{
    private bool _isSwinging = false;

    [Export] public Dice TargetDice { get; set; }

    [Export] public CameraRig Camera { get; set; }

    [Export] public Camera3D TargetingCamera { get; set; }

    [Export] public RayCast3D TargetRay { get; set; }

    [Export] public GeometryInstance3D TargetLocator { get; set; }

    [Export] public Node3D RayRig { get; set; }
    
    public float MaxPower { get; set; } = 15f;

    public float MinPower { get; set; } = 0.1f;

    [Export] public float PowerDelta { get; set; } = 0.1f;

    [Export] public float StartingPower { get; set; } = 1f;

    [Export] public float TargetRotationSpeed { get; set; } = 2.5f;
    private float _currentPower { get; set; } = 1f;

    private bool _increasePower = true;

    private Vector3 TargetLocation { get; set; } = new Vector3(0, 0, 1);

    [Signal]
    public delegate void SwingEventHandler(Vector3 direction, Vector3 impactPoint, float power);

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("golf_swing"))
        {
            if (_isSwinging)
            {
                EmitSignal(SignalName.Swing, -Camera.GlobalTransform.Basis.Z, TargetRay.GetCollisionPoint(), _currentPower * 3);
            }

            _currentPower = StartingPower;
            _increasePower = true;
            _isSwinging = !_isSwinging;
        }
    }

    public override void _Process(double delta)
    {
        var inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

        if (inputDirection.LengthSquared() > 0)
        {
            RayRig.RotateY(Mathf.DegToRad(-inputDirection.X * TargetRotationSpeed));
            RayRig.RotateX(Mathf.DegToRad(-inputDirection.Y * TargetRotationSpeed));
        }
        
        TargetingCamera.GlobalPosition = TargetDice.GlobalPosition + (Camera.GlobalTransform.Basis.Z * 2);
        TargetingCamera.LookAt(TargetDice.GlobalPosition);

        TargetRay.TargetPosition = TargetRay.ToLocal(TargetDice.Position);

        var collider = TargetRay.GetCollider();
        if (collider != null)
        {
            TargetLocator.Position = TargetLocator.ToLocal(TargetRay.GetCollisionPoint());
        }
        
        if (_isSwinging)
        {
            if (_increasePower)
            {
                _currentPower = Mathf.Min(_currentPower + PowerDelta, MaxPower);
                if (Math.Abs(_currentPower - MaxPower) < PowerDelta / 10)
                    _increasePower = false;
            }
            else
            {
                _currentPower = Mathf.Max(_currentPower - PowerDelta, MinPower);
                if (Math.Abs(_currentPower - MinPower) < PowerDelta / 10)
                    _increasePower = true;
            }

            DebugDraw3D.DrawLine(TargetDice.GlobalPosition,
                TargetDice.GlobalPosition - (Camera.GlobalTransform.Basis.Z * _currentPower));
        }
    }
}