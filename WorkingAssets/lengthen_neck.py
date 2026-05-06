import bpy
import bmesh

# Constants
bottomWeightlessZ = 0.653846
bottomWeightedZ   = 0.754098
topWeightedZ      = 0.805539
topWeightlessZ    = 0.87185

offsetY = -0.009950
offsetZ = -0.01714


def get_weight(z):
    # Fully weightless zones
    if z <= bottomWeightlessZ or z >= topWeightlessZ:
        return 0.0

    # Fully weighted zone
    if bottomWeightedZ <= z <= topWeightedZ:
        return 1.0

    # Interpolation zones
    if bottomWeightlessZ < z < bottomWeightedZ:
        # ramp up from 0 → 1
        return (z - bottomWeightlessZ) / (bottomWeightedZ - bottomWeightlessZ)

    if topWeightedZ < z < topWeightlessZ:
        # ramp down from 1 → 0
        return (topWeightlessZ - z) / (topWeightlessZ - topWeightedZ)

    return 0.0


# --- Process Mesh Objects ---
for obj in bpy.context.selected_objects:
    if obj.type != 'MESH':
        continue

    bpy.context.view_layer.objects.active = obj

    # Use bmesh for edit-safe vertex access
    mesh = obj.data
    bm = bmesh.new()
    bm.from_mesh(mesh)

    for v in bm.verts:
        z = v.co.z
        weight = get_weight(z)
        if (obj.name == 'Boobs'):
            weight = 1

        v.co.y += offsetY * weight
        v.co.z += offsetZ * weight

    bm.to_mesh(mesh)
    bm.free()

    mesh.update()

# --- Process Armatures (per-end weighting + reconnect) ---
for obj in bpy.context.selected_objects:
    if obj.type != 'ARMATURE':
        continue

    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode='EDIT')

    arm = obj.data
    bones = arm.edit_bones

    # Step 1: Record connections
    connection_data = []
    for bone in bones:
        connection_data.append({
            "bone": bone,
            "parent": bone.parent,
            "was_connected": bone.use_connect
        })

    # Step 2: Disconnect
    for data in connection_data:
        if data["was_connected"]:
            data["bone"].use_connect = False

    # Step 3: Apply per-end weighting (SKIP mirrored .R bones)
    for bone in bones:
        if bone.name.endswith(".R"):
            continue
        if "hair" in bone.name:
            continue

        # Head
        hz = bone.head.z
        hw = get_weight(hz)
        bone.head.y += offsetY * hw
        bone.head.z += offsetZ * hw

        # Tail
        tz = bone.tail.z
        tw = get_weight(tz)
        bone.tail.y += offsetY * tw
        bone.tail.z += offsetZ * tw

    # Step 4: Reconnect cleanly
    for data in connection_data:
        bone = data["bone"]
        parent = data["parent"]

        if data["was_connected"] and parent is not None:
            bone.head = parent.tail
            bone.use_connect = True

    bpy.ops.object.mode_set(mode='OBJECT')


print("Vertex and bone offset complete.")