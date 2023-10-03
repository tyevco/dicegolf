extends Camera3D
class_name CameraBrain

@export_group("Virtual Cameras")
var vcams: Array[Node]
@export var current_cam: int : set = _set_cam

func _set_cam(cam: int):
	current_cam = cam % vcams.size()
	location_node = vcams[current_cam].get_child(0)
	target_node = vcams[current_cam].get_child(1)

var location_node: Node3D
var target_node: Node3D

var pedestal: float
var tilt: float
var track: float
var radius: float
var yaw: float
var pitch: float

func _ready():
	for cam in $"../Virtual Cameras".get_children():
		vcams.push_back(cam)

	location_node = vcams[current_cam].get_child(0)
	target_node = vcams[current_cam].get_child(1)
	self.position = location_node.position
	self.rotation = location_node.rotation

	pedestal = vcams[current_cam].get("pedestal")
	tilt = vcams[current_cam].get("tilt")
	track = vcams[current_cam].get("track")
	radius = vcams[current_cam].get("radius")
	yaw = vcams[current_cam].get("yaw")
	pitch = vcams[current_cam].get("pitch")

func _physics_process(delta):
	# TODO clean this mess
	pedestal = vcams[current_cam].get("pedestal")
	tilt = vcams[current_cam].get("tilt")
	track = vcams[current_cam].get("track")
	radius = vcams[current_cam].get("radius")
	yaw = vcams[current_cam].get("yaw")
	pitch = vcams[current_cam].get("pitch")
		
	var location = location_node.position
	var target = target_node.position
	
	self.position = location + Vector3(0, vcams[current_cam].get("pedestal"), 0) \
		+ get_orbit(radius, yaw, pitch)
	var track_focus = target + get_orbit(track,yaw + -PI/2, 0)
	self.look_at(track_focus + Vector3(0, tilt, 0))

func get_orbit(_radius: float, _yaw: float, _pitch: float) -> Vector3:
	var ray := Vector3.FORWARD
	ray = Quaternion(Vector3.UP, TAU - _yaw) * ray

	var _pitch_axis := ray.cross(Vector3.UP)
	ray = Quaternion(_pitch_axis, TAU - _pitch) * ray

	return ray * -_radius
