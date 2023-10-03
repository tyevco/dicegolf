# CineCam

Cinecam is a camera system based around vritual camera nodes that hold information about location, target and translation. It does not aim to be a clone or replacement of its inspiration, but rather to adapt its main workflow into a simple, indie-oriented package that follows the godot philosophy: Everything is a node, runs fast, and the provided features are simple but complete, and can be easily extended via gdscript.

## Features

- Easily manage a collection of virtual cameras with support for transitions
- Define cameras in terms of their location, with support for orbiting parameters (rotation, displacement, pivoting, etc.) and their target (Either custom or any Node3D).
- Procedurally animate cameras by manipulating how they respond to the movement of the location or their target: add easing, smoothing, bounce, and other effects.

## How to Use

1) Add CineCam to your *res://addons/* folder.
2) Add the camera_system scene to your parent scene and make it local. Make sure the Camera3D inside this scene is your only camera in the parent scene.
3) Add any number of virtual_camera.tscn to the "Virtual Cameras" node.
4) Set the parameters for each virtual camera. You can edit the position of the Location and Target nodes by making the Camera nodes local.

## TODO List

- Individual axis dampening
- Rotation dampening
- Fancier transitions (splines, dampened)
- AverageNodes follow type
- Target deadzones
- Change dampening without reloading game
- Demo cameras
