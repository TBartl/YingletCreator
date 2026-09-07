# Used for idle animations in Yinglet-Stances
# Takes an anim with poses at 0 and 45 

import bpy

reverse = False

BONE_OFFSETS = {
    "root": 1,
    "chest_ik": 3,
    "shoulder.L": 4,
    "shoulder.R": 4,
    "face_ik": 5,
    
    "hand_ik.L": 9,
    "f_ring.01.R": 12,
    "f_middle.01.R": 12,
    "f_index.01.R": 12,
    "thumb.01.R": 12,
    "thumb.02.R": 12,
    
    # previously 12, 15
    "hand_ik.L": 10,
    "f_ring.01.L": 13,
    "f_middle.01.L": 13,
    "f_index.01.L": 13,
    "thumb.01.L": 13,
    "thumb.02.L": 13,
}

def ensure_mirrored():
    obj = bpy.context.object
    action = obj.animation_data.action

    for layer in action.layers:
        for strip in layer.strips:
            for channelbag in strip.channelbags:
                for fcurve in channelbag.fcurves:

                    # Remove existing Cycles modifiers
                    for modifier in list(fcurve.modifiers):
                        if modifier.type == 'CYCLES':
                            fcurve.modifiers.remove(modifier)

                    # Add mirrored cycle
                    modifier = fcurve.modifiers.new('CYCLES')
                    modifier.mode_before = 'MIRROR'
                    modifier.mode_after = 'MIRROR'

def offset_bones():
    armature = bpy.context.object

    if armature is None or armature.type != 'ARMATURE':
        raise RuntimeError("Select an armature object.")

    if not armature.animation_data or not armature.animation_data.action:
        raise RuntimeError("Armature has no active action.")

    action = armature.animation_data.action

    for layer in action.layers:
        for strip in layer.strips:
            for channelbag in strip.channelbags:
                for fcurve in channelbag.fcurves:

                    if not fcurve.data_path.startswith('pose.bones["'):
                        continue

                    try:
                        bone_name = fcurve.data_path.split('"')[1]
                    except IndexError:
                        continue

                    if bone_name not in BONE_OFFSETS:
                        continue

                    offset = BONE_OFFSETS[bone_name]
                    
                    if reverse:
                        offset = -offset

                    for keyframe in fcurve.keyframe_points:
                        keyframe.co.x += offset
                        keyframe.handle_left.x += offset
                        keyframe.handle_right.x += offset

                    fcurve.keyframe_points.sort()

ensure_mirrored()
offset_bones()