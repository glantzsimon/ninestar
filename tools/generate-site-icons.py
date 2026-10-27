"""Regenerate website icons from the approved source. Requires Pillow.
Run: python tools/generate-site-icons.py
"""
from pathlib import Path
import json
from PIL import Image

root = Path(__file__).resolve().parent.parent
icons = root / "webapp/WebApplication/Images/icons"
source = Image.open(root / "tools/assets/ninestar-icon-source.png").convert("RGBA")
icons.mkdir(parents=True, exist_ok=True)

for name, size, ratio in [
    ("apple-touch-icon.png", 180, 0.9),
    ("icon-192.png", 192, 0.9),
    ("icon-512.png", 512, 0.9),
    ("icon-maskable-512.png", 512, 0.74),
]:
    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    artwork = source.copy()
    artwork.thumbnail((round(size * ratio), round(size * ratio)), Image.Resampling.LANCZOS)
    canvas.alpha_composite(artwork, ((size - artwork.width) // 2, (size - artwork.height) // 2))
    canvas.save(icons / name, optimize=True)

source.resize((256, 256), Image.Resampling.LANCZOS).save(
    icons / "favicon.ico", format="ICO", sizes=[(16, 16), (32, 32), (48, 48)])
manifest = {
    "name": "NineStar",
    "short_name": "NineStar",
    "icons": [
        {"src": name, "sizes": f"{size}x{size}", "type": "image/png", "purpose": purpose}
        for name, size, purpose in [
            ("icon-192.png", 192, "any"),
            ("icon-512.png", 512, "any"),
            ("icon-maskable-512.png", 512, "maskable"),
        ]
    ],
}
(icons / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n")
print("Generated ICO, four PNGs and manifest.")
