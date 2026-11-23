"""Pose the guard's arms and animate the planted stance on the idle mesh."""
import numpy as np

from build_guard_breath import PERIOD, envelope, smooth
from build_guard_thrust import GUARD_POINTS, ORIGIN


def add_limb_motion(rig):
    tracks = rig["animations"]["idle_loop"]["bones"]
    times = [key["time"] for key in tracks["breath_neck"]["rotate"]]
    for shoulder, wrist, elbow, pivot, swing in [
        ("breath_near_shoulder", "breath_grip", "elbow", "arm", -3.2),
        ("breath_far_shoulder", "breath_open_glove", "far_elbow", "far_arm", 2.8),
    ]:
        arm = (np.subtract(GUARD_POINTS[elbow], GUARD_POINTS[pivot]) * [1, -1])
        for i, t in enumerate(times):
            angle = swing * envelope(t, 1.46)
            key = tracks[shoulder]["rotate"][i]
            key["value"] = round(key["value"] + angle, 6)
            c, s = np.cos(np.radians(angle)), np.sin(np.radians(angle))
            dx, dy = np.array([[c - 1, -s], [s, c - 1]]) @ arm
            key = tracks[wrist]["translate"][i]
            key.update(x=round(key["x"] + dx, 6), y=round(key["y"] + dy, 6))

    # Spine Y points up. Match the high point of the chest breath and pass
    # through the painted guard at the loop seam, so a thrust starts in register.
    phase = 2 * np.pi * np.asarray(times) / PERIOD
    lift = 2.6 * (np.sin(phase - .65) + np.sin(.65))
    for track in tracks.values():
        for key, dy in zip(track["translate"], lift, strict=True):
            key["y"] = round(key["y"] + dy, 6)

    pivots = {"hips": GUARD_POINTS["pelvis"],
              "rear_knee": GUARD_POINTS["rear_knee"], "lead_knee": GUARD_POINTS["lead_knee"]}
    for name in pivots:
        travel = lift if name == "hips" else lift * .43
        tracks["breath_" + name] = {
            "translate": [dict(time=t, x=0, y=round(dy, 6)) for t, dy in zip(times, travel)],
            "rotate": [dict(time=t, value=0) for t in times],
        }
    indices = {}
    for name, pivot in pivots.items():
        indices[name] = len(rig["bones"])
        rig["bones"].append(dict(name="breath_" + name, parent="root",
                                 x=float(pivot[0]-ORIGIN[0]), y=float(ORIGIN[1]-pivot[1])))

    # Redistribute only the stationary root influence of this attachment.
    mesh = rig["skins"][0]["attachments"]["guard_reference"]["breathing_guard"]
    old = iter(mesh["vertices"])
    vertices = []
    for u, v in zip(mesh["uvs"][::2], mesh["uvs"][1::2]):
        x, y = u * mesh["width"], v * mesh["height"]
        side = "rear" if x < ORIGIN[0] else "lead"
        hip_y, knee_y, ankle_y = [GUARD_POINTS[f"{side}_{key}"][1] for key in ("thigh","knee","ankle")]
        # Shorten the projected leg segments vertically. Keep the kneecap
        # and its bandage together, and fade the shin motion out at the boot.
        thigh = smooth((y - hip_y) / (knee_y - 5 - hip_y))
        shin = smooth((y - knee_y - 5) / (ankle_y - knee_y - 5))
        shares = {"hips": 1 - thigh, side + "_knee": thigh * (1 - shin)}
        influences = []
        for _ in range(next(old)):
            idx, lx, ly, w = [next(old) for _ in range(4)]
            if idx != 0:
                influences.append((idx,lx,ly,w))
                continue
            fixed = 1-sum(shares.values())
            if fixed > 1e-8:
                influences.append((0,lx,ly,w*fixed))
            for name, share in shares.items():
                if share > 1e-8:
                    px, py = pivots[name]
                    influences.append((indices[name],x-px,py-y,w*share))
        vertices.append(len(influences))
        for influence in influences:
            vertices.extend(influence)
    mesh["vertices"] = vertices
