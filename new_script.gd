@tool
extends EditorScenePostImport

func _post_import(scene):
	var path := get_source_file()
	print("Running post import script on file '{file}'".format({'file': path}))
	
	var file_name := path.get_file()

	path = path.substr(0, path.rfind('/'))
	
	# Have a subfolder to store all the individual meshes and materials for each imported glTF scene
	# Prefix it with _import to not clutter the directories as much
	var new_subfolder := path + "/_import/" + file_name + "/"
	
	# if the subfolder for resources already exists, clear it
	recursively_delete_dir_absolute(new_subfolder)
	
	# Now create the empty subfolder
	DirAccess.make_dir_recursive_absolute(new_subfolder)
	
	var mesh_count : int = 0
	
	# Get valid mesh objects
	for object in get_all_children_recursive(scene):
		if !(object is MeshInstance3D):
			continue
		
		# Reference to the mesh
		var mesh : MeshInstance3D = object 
		
		# First iterate all materials of the mesh and save them to disk
		for mat in mesh.mesh.get_surface_count():
			
			# Path where to store the extracted material
			var surface_path = new_subfolder + str(mesh_count) + "_" + str(mat) + "_" + str(mesh.name) + ".res"
			
			var material := mesh.mesh.surface_get_material(mat)
			
			# Store the material to disk as a resource file
			if ResourceSaver.save(material, surface_path, ResourceSaver.FLAG_CHANGE_PATH) == OK:
				
				# Now using a resource loader load the same material and overwrite the old one
				var new_material = ResourceLoader.load(surface_path) as Material
				# This prevents the imported scene from storing the material as a sub resource
				mesh.mesh.surface_set_material(mat, new_material)
				print("Saved Material and overwrote resource: " + surface_path)
		
		# Now save the mesh of our mesh object to disk
		var mesh_path = new_subfolder + str(mesh_count) + "_" + str(mesh.name) + ".res"

		if ResourceSaver.save(mesh.mesh, mesh_path, ResourceSaver.FLAG_CHANGE_PATH) == OK:
			
			# Now using a resource loader load the same mesh again and overwrite our scene mesh with it
			var new_mesh = ResourceLoader.load(mesh_path)
			# This prevents the imported scene from storing the mesh as a sub resource
			mesh.mesh = new_mesh
			print("Saved Mesh and overwrote resource: " + mesh_path)
			mesh_count += 1

	return scene
		
		
func get_all_children_recursive(node: Node, include_self: bool = false) -> Array:
	var nodes : Array = []

	if include_self:
		nodes.append(node)

	for N in node.get_children():
		if N.get_child_count() > 0:
			nodes.append(N)
			nodes.append_array(get_all_children_recursive(N))
		else:
			nodes.append(N)

	return nodes
	
func recursively_delete_dir_absolute(folder: String) -> bool:
	# Delete folder if it exists
	if DirAccess.dir_exists_absolute(folder):
		# Open folder
		var access := DirAccess.open(folder)
		# Delete all files within the folder
		access.list_dir_begin() 
		var file_name = access.get_next()
		while file_name != "":
			if not access.current_is_dir():
				access.remove(file_name)
			else:
				# If it contains more folders, first delete that folder recursively
				recursively_delete_dir_absolute(folder + "/" + file_name)
				
			file_name = access.get_next()
		# Delete the now empty folder
		if DirAccess.remove_absolute(folder) != OK:
			return false
		return true
	return false
