﻿using Godot;

namespace PleaseKissMyElbow.addons.components.scripts;

[Component]
public partial class SelectableComponent : Node
{
    [Export] public MeshInstance3D OutlineMesh { get; set; }

    private bool _selected;
    
    public void SetSelected(bool value)
    {
        _selected = value;
        OutlineMesh.Visible = value;
    }

    public bool IsSelected()
    {
        return _selected;
    }
}