import bpy

SCALE_FACTOR = 1.0935
CURSOR_LOCATION = (0.0, -0.01935, 0.8322)

obj = bpy.context.active_object

# --- Safety checks ---
if not obj:
    print("No active object.")
elif obj.type != 'MESH':
    print("Active object is not a mesh.")
elif bpy.context.mode != 'EDIT_MESH':
    print("Not in Edit Mode.")
else:
    import bmesh

    bm = bmesh.from_edit_mesh(obj.data)

    selected_verts = [v for v in bm.verts if v.select]

    if not selected_verts:
        print("No vertices selected.")
    else:
        # --- Set 3D cursor ---
        bpy.context.scene.cursor.location = CURSOR_LOCATION

        # --- Set pivot to 3D cursor ---
        bpy.context.scene.tool_settings.transform_pivot_point = 'CURSOR'

        # --- Ensure vertex select mode (optional safety) ---
        bpy.ops.mesh.select_mode(type="VERT")

        # --- Scale selection on XY only (Z unaffected) ---
        bpy.ops.transform.resize(
            value=(SCALE_FACTOR, SCALE_FACTOR, 1.0),
            orient_type='GLOBAL',
            constraint_axis=(True, True, False)
        )

        # Update mesh
        bmesh.update_edit_mesh(obj.data)

        print("Scaling applied.")