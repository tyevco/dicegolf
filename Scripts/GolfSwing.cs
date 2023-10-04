using System;
using Godot;

namespace PleaseKissMyElbow;

public partial class GolfSwing : Node
{
    private bool _isSwinging = false;

    [Export]
    public Dice DiceTarget { get; set; }
    
    [Export]
    public CameraRig Camera { get; set; }

    [Export]
    public Camera3D TargetingCamera { get; set; }
    
    public float MaxPower { get; set; } = 15f;

    public float MinPower { get; set; } = 0.1f;

    [Export]
    public float PowerDelta { get; set; } = 0.1f;

    [Export]
    public float StartingPower { get; set; } = 1f;
    
    private float _currentPower { get; set; } = 1f;

    private bool _increasePower = true;
    
    [Signal]
    public delegate void SwingEventHandler(Vector3 direction, float power);

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("golf_swing"))
        {
            if (_isSwinging)
            {
                EmitSignal(SignalName.Swing, -Camera.GlobalTransform.Basis.Z, _currentPower * 3);
            }
            else
            {
                TargetingCamera.GlobalPosition = DiceTarget.GlobalPosition + (Camera.GlobalTransform.Basis.Z * 2);
            }
            _currentPower = StartingPower;
            _increasePower = true;
            _isSwinging = !_isSwinging;
        }
    }

    public override void _Process(double delta)
    {
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
            
            DebugDraw3D.DrawLine(DiceTarget.GlobalPosition,DiceTarget.GlobalPosition - (Camera.GlobalTransform.Basis.Z * _currentPower));
        }
    }
}