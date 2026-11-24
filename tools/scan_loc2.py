from sts2_paths import workshop_root

p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
i = 42320049
print(repr(d[i-900:i+700]))
