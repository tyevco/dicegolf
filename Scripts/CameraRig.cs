using System.Diagnostics;
using Godot;
using PleaseKissMyElbow.addons.components.extensions;
using PleaseKissMyElbow.addons.components.scripts;

namespace PleaseKissMyElbow;

public partial class CameraRig : Node3D
{
    [ExportCategory("Movement")]
    [ExportGroup("Mouse Settings")]
    [Export(PropertyHint.Range, "0,100,1")]
    public int HorizontalMouseSensitivity { get; set; } = 25;

    [Export(PropertyHint.Range, "0,100,1")]
    public int VerticalMouseSensitivity { get; set; } = 25;

    [ExportGroup("Speed")] [Export] public float WalkSpeed { get; set; } = 10f;
    [Export] public float RunSpeed { get; set; } = 15f;
    [Export] public float CrawlSpeed { get; set; } = 5f;

    [ExportCategory("Tracking Camera")]
    [Export]
    public float MinimumViewAngle { get; set; } = -60f;

    [Export] public float MaximumViewAngle { get; set; } = -15f;

    [ExportCategory("Follow Camera")]
    [Export]
    public Node3D TrackingTarget { get; set; }

    [ExportCategory("Component Settings")]
    [Export]
    public Camera3D Camera { get; set; }

    [Export] public Input.MouseModeEnum MouseMode { get; set; }

    [Export] public RayCast3D Ray { get; set; }

    private float _horzMouseSensitivity;
    private float _vertMouseSensitivity;
    private SelectableComponent _selectableComponent = null;

    [Export] public Node3D DebugObjectStart { get; set; }
    [Export] public Node3D DebugObjectEnd { get; set; }

    public override void _Ready()
    {
        Input.MouseMode = MouseMode;

        _horzMouseSensitivity = HorizontalMouseSensitivity / 100f;
        _vertMouseSensitivity = VerticalMouseSensitivity / 100f;
    }

    private string SelectableComponentPath = "components/SelectableComponent";

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _horzMouseSensitivity));
            Camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * _vertMouseSensitivity));
            var rotation = Camera.Rotation;
            rotation.X = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(MinimumViewAngle),
                Mathf.DegToRad(MaximumViewAngle));

            Camera.Rotation = rotation;
        }
        else if (@event.IsActionPressed("camera_tracking_cancel"))
        {
            TrackingTarget = null;
        }
        else if (@event.IsActionPressed("camera_tracking_follow"))
        {
            if (_selectableComponent != null)
            {
                TrackingTarget = _selectableComponent.GetParent().GetParent() as Node3D;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (TrackingTarget != null)
        {
            this.Position = TrackingTarget.Position;
        }
    }

    public override void _Process(double delta)
    {
        if (_selectableComponent != null)
        {
            _selectableComponent.SetSelected(false);
            _selectableComponent = null;
        }

        if (TrackingTarget == null)
        {
            // raycast and get target
            var obj = GetTargettedObject();
            if (obj is Node3D node &&
                node.TryGetNode<SelectableComponent>(SelectableComponentPath, out var selectableComponent))
            {
                GD.Print("Looking at " + selectableComponent);
                selectableComponent.SetSelected(true);
                _selectableComponent = selectableComponent;
            }
        }

        var deltaAsFloat = (float)delta;
        var speedForFrame = WalkSpeed;
        if (Input.IsActionPressed("modifier_run"))
        {
            speedForFrame = RunSpeed;
        }
        else if (Input.IsActionPressed("modifier_crawl"))
        {
            speedForFrame = CrawlSpeed;
        }

        // If no target is being tracked, allow free movement. Otherwise, adjust the rig offset.
        if (TrackingTarget == null)
        {
            var inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

            var direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();

            var position = Position;

            position.X += direction.X * (speedForFrame * deltaAsFloat);
            position.Z += direction.Z * (speedForFrame * deltaAsFloat);

            var vertical = Input.GetAxis("move_down", "move_up");
            position.Y += vertical * (speedForFrame * deltaAsFloat);

            Position = position;
        }
        else
        {
            // modify the offset.
        }
    }

    private GodotObject GetTargettedObject()
    {
        var screen = GetViewport().GetVisibleRect();
        var screenCenter = new Vector2(screen.Size.X / 2, screen.Size.Y / 2);
        var cameraProjectOrigin = Camera.ProjectRayOrigin(screenCenter);
        //var cameraTarget = Camera.ProjectPosition(screenCenter, 100f);
        var cameraTarget = Camera.GlobalPosition - Camera.GlobalTransform.Basis.Z * 100;
        Ray.GlobalPosition = Camera.GlobalPosition;
        Ray.TargetPosition = Ray.ToLocal(cameraTarget);

        var collider = Ray.GetCollider();

        if (DebugObjectStart != null)
            DebugObjectStart.Position = Ray.GlobalPosition;

        DebugDraw3D.DrawLine(Camera.GlobalPosition, cameraTarget, Color.FromHsv(0.25f, 0.86f, 0.82f));
        DebugDraw3D.DrawLine(Camera.GlobalPosition, GlobalPosition, Color.FromHsv(0.25f, 0.86f, 0.82f));
        
        if (collider != null)
        {
            DebugDraw3D.DrawLine(Ray.GlobalPosition, Ray.GetCollisionPoint(), Color.FromHsv(1f, 1f, 1f));
            GD.Print(Ray.GlobalPosition + " : " + Ray.GetCollisionPoint() + "!");

        }
        else
        {
            DebugDraw3D.DrawLine(Ray.GlobalPosition, Ray.TargetPosition, Color.FromHsv(0.74f, 0.82f, 0.87f));
            GD.Print(Ray.GlobalPosition + " : " + Ray.TargetPosition);

        }

        if (DebugObjectEnd != null)
        {
            if (collider != null)
                DebugObjectEnd.Position = Ray.GetCollisionPoint();
            else
                DebugObjectEnd.Position = Ray.TargetPosition;
        }

        return collider;
    }
}