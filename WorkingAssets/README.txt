For the .blend files:

These were created in Blender 4.3.2

For exporting, use the export.py script to automatically export to the correct location within the project.

Texturing is done using CraftReaper's brush pack:
https://omarfaruqtawsif.gumroad.com/l/zcpfq

UCU Paint is also used heavily in the texturing workflow.
It can be downloaded from File->Preferences->Get Extensions (Search UCU Paint), or here:
https://github.com/ucupumar/ucupaint

UVToolkit is used for unwrapping, though is not strictly required

Pose props: a prop's prefab root is parented to the attached bone at identity, so its child FBX instance's local transform is the prop's offset in bone space, computed in Blender as rel = (armature.matrix_world @ pose_bone.matrix).inverted() @ prop.matrix_world with the pose action active. Convert rel to Unity as follows:
- Position: (x, y, z) -> (-x, y, z)
- Rotation: quaternion (w, x, y, z) -> (w, x, -y, -z), then right-multiply by the FBX root's axis correction (Unity xyzw 0, 0.7071068, 0.7071068, 0; i.e. 180 degrees about (0,1,1))
- Scale: leave unoverridden, since the exported FBX root already carries it