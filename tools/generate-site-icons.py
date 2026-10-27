"""Regenerate only favicon and maskable icon from the current approved icon-512.png.
Requires Pillow. Run: python tools/generate-site-icons.py
Other icons, the manifest and the supplied artwork are left unchanged.
"""
from pathlib import Path
from PIL import Image

root = Path(__file__).resolve().parent.parent
icons = root / "webapp/WebApplication/Images/icons"
source = Image.open(icons / "icon-512.png").convert("RGBA")
canvas = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
artwork = source.copy()
artwork.thumbnail((379, 379), Image.Resampling.LANCZOS)
canvas.alpha_composite(artwork, ((512 - artwork.width) // 2, (512 - artwork.height) // 2))
canvas.save(icons / "icon-maskable-512.png", optimize=True)
source.resize((256, 256), Image.Resampling.LANCZOS).save(
    icons / "favicon.ico", format="ICO", sizes=[(16, 16), (32, 32), (48, 48)])
print("Regenerated only transparent favicon and maskable icon.")
