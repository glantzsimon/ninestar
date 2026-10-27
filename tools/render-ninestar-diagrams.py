"""Render precise Nine Star Ki diagrams over the approved gold artwork.

Development-only: requires Pillow, Inkscape and the DejaVu Serif font.
Run after exporting the gold element icons and blank centre medallion.
Labels come from the existing Dictionary resources. No website runtime changes.
"""
import base64
import html
import math
from pathlib import Path
import shutil
import subprocess
import tempfile
import xml.etree.ElementTree as ET

from PIL import Image


ROOT = Path(__file__).resolve().parent.parent
IMAGES = ROOT / 'webapp/WebApplication/Images/ninestar'
PATTERNS = {
    'heaven': (1, 1, 1),
    'soil': (0, 0, 0),
    'thunder': (0, 0, 1),
    'wind': (1, 1, 0),
    'water': (0, 1, 0),
    'fire': (1, 0, 1),
    'mountain': (1, 0, 0),
    'lake': (0, 1, 1),
}
ELEMENTS = ('Fire', 'Earth', 'Metal', 'Water', 'Tree')
PENTAGON = {'Fire': (220, 96), 'Earth': (350, 188),
            'Metal': (300, 346), 'Water': (140, 346), 'Tree': (90, 188)}
CROSS = {'Fire': (220, 96), 'Earth': (220, 244),
         'Metal': (350, 244), 'Water': (220, 378), 'Tree': (90, 244)}
EDGES = {
    'supportive': (('Fire', 'Earth'), ('Earth', 'Metal'), ('Metal', 'Water'),
                   ('Water', 'Tree'), ('Tree', 'Fire')),
    'challenging': (('Tree', 'Earth'), ('Earth', 'Water'), ('Water', 'Fire'),
                    ('Fire', 'Metal'), ('Metal', 'Tree')),
    'four': (('Tree', 'Fire'), ('Fire', 'Metal'), ('Metal', 'Water'),
             ('Water', 'Tree')),
}


def embedded(path):
    return 'data:image/png;base64,' + base64.b64encode(path.read_bytes()).decode('ascii')


def image_tag(path, x, y, width, height):
    return (f'<image x="{x}" y="{y}" width="{width}" height="{height}" '
            f'xlink:href="{embedded(path)}"/>')


def svg_page(size, contents):
    return (f'<svg xmlns="http://www.w3.org/2000/svg" '
            f'xmlns:xlink="http://www.w3.org/1999/xlink" width="{size}" '
            f'height="{size}" viewBox="0 0 {size} {size}">{contents}</svg>')


def render(svg, target, size):
    executable = shutil.which('inkscape')
    if not executable:
        raise RuntimeError('Install Inkscape to regenerate the checked-in PNG diagrams.')
    with tempfile.TemporaryDirectory(prefix='ninestar-artwork-') as directory:
        source = Path(directory) / 'diagram.svg'
        output = Path(directory) / 'diagram.png'
        source.write_text(svg, encoding='utf-8')
        subprocess.run([executable, str(source), '--export-area-page',
                        '--export-type=png', '--export-filename=' + str(output),
                        '--export-width=' + str(size), '--export-height=' + str(size)],
                       check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        with Image.open(output) as image:
            image.convert('RGBA').save(target, format='PNG', optimize=True)


def trigram_svg(pattern):
    # The centre medallion has no assigned trigram and remains unmarked.
    background = image_tag(IMAGES / 'trigrams/trigram-coreearth-square.png', 0, 0, 160, 160)
    bars = []
    for row, solid in enumerate(pattern):
        y = 50 + row * 24
        segments = ((34, 92),) if solid else ((34, 37), (89, 37))
        for x, width in segments:
            bars.append(f'<rect x="{x}" y="{y}" width="{width}" height="12" '
                        'rx="0.7" fill="url(#bronze)" stroke="#3b240f" stroke-width="0.8"/>')
            bars.append(f'<path d="M{x + 1},{y + 11}h{width - 2}" '
                        'stroke="#e6bc68" stroke-width="0.7" opacity="0.8"/>')
    gradient = ('<defs><linearGradient id="bronze" x2="0" y2="1">'
                '<stop stop-color="#3b240f"/><stop offset="1" stop-color="#81521f"/>'
                '</linearGradient></defs>')
    return svg_page(160, gradient + background + ''.join(bars))


def trimmed(start, end, radius=55):
    dx, dy = end[0] - start[0], end[1] - start[1]
    length = math.hypot(dx, dy)
    ux, uy = dx / length, dy / length
    return ((start[0] + radius * ux, start[1] + radius * uy),
            (end[0] - radius * ux, end[1] - radius * uy))


def cycle_svg():
    background = image_tag(IMAGES / 'trigrams/trigram-coreearth-square.png', 0, 0, 160, 160)
    parts = [background, '<defs><marker id="arrow" markerWidth="10" markerHeight="10" '
             'refX="10" refY="5" orient="auto" markerUnits="userSpaceOnUse">'
             '<path d="M0,0L10,5L0,10L2,5Z" fill="#563316"/></marker></defs>']
    for quadrant in range(4):
        angles = [math.radians(-90 + quadrant * 90 + offset) for offset in (8, 82)]
        start, end = [(80 + 42 * math.cos(angle), 80 + 42 * math.sin(angle)) for angle in angles]
        parts.append(f'<path d="M{start[0]},{start[1]}A42,42 0 0 1 {end[0]},{end[1]}" '
                     'fill="none" stroke="#563316" stroke-width="3.5" '
                     'stroke-linecap="round" marker-end="url(#arrow)"/>')
    return svg_page(160, ''.join(parts))


def diagram_svg(kind, dark, labels):
    positions = CROSS if kind == 'four' else PENTAGON
    colour = '#f4dfaa' if dark else '#4b3015'
    marker = (f'<defs><marker id="arrow" markerWidth="11" markerHeight="11" '
              'refX="11" refY="5.5" orient="auto" markerUnits="userSpaceOnUse">'
              f'<path d="M0,0L11,5.5L0,11L3,5.5Z" fill="{colour}"/>'
              '</marker></defs>')
    parts = [marker]
    for element in ELEMENTS:
        x, y = positions[element]
        path = IMAGES / 'elements' / (element.lower() + '.png')
        if kind == 'same':
            parts.append(image_tag(path, x - 47, y - 22, 44, 44))
            parts.append(image_tag(path, x + 3, y - 22, 44, 44))
            for direction in (1, -1):
                start_x, end_x = x - 29 * direction, x + 29 * direction
                curve_y = y - 50 * direction
                edge_y = y - 31 * direction
                parts.append(f'<path d="M{start_x},{edge_y}Q{x},{curve_y} {end_x},{edge_y}" '
                             f'fill="none" stroke="{colour}" stroke-width="3.5" '
                             'stroke-linecap="round" marker-end="url(#arrow)"/>')
        else:
            parts.append(image_tag(path, x - 48, y - 48, 96, 96))
        label_y = (31 if element == 'Fire' else
                   (180 if element in ('Tree', 'Earth', 'Metal') else 317)) if kind == 'four' else (
                       31 if element == 'Fire' else 123 if element in ('Tree', 'Earth') else 420)
        parts.append(f'<text x="{x}" y="{label_y}" text-anchor="middle" '
                     f'font-family="DejaVu Serif" font-style="italic" font-size="24" '
                     f'fill="{colour}">{html.escape(labels[element])}</text>')
    for first, second in EDGES.get(kind, ()):
        start, end = trimmed(positions[first], positions[second])
        if kind == 'challenging':
            path = f'M{start[0]},{start[1]}L{end[0]},{end[1]}'
        else:
            mx = (start[0] + end[0]) / 2
            my = (start[1] + end[1]) / 2
            ox, oy = mx - 220, my - 244
            length = math.hypot(ox, oy)
            control = (mx + 16 * ox / length, my + 16 * oy / length)
            path = f'M{start[0]},{start[1]}Q{control[0]},{control[1]} {end[0]},{end[1]}'
        parts.append(f'<path d="{path}" fill="none" stroke="{colour}" '
                     'stroke-width="3.5" stroke-linecap="round" marker-end="url(#arrow)"/>')
    return svg_page(440, ''.join(parts))


def main():
    resources = ET.parse(ROOT / 'webapp/Globalisation/Dictionary.resx').getroot()
    labels = {key: next(item.findtext('value') for item in resources.findall('data')
                        if item.attrib['name'] == key) for key in ELEMENTS}
    render(cycle_svg(), IMAGES / 'elements/cycle.png', 160)
    for name, pattern in PATTERNS.items():
        render(trigram_svg(pattern), IMAGES / f'trigrams/trigram-{name}-square.png', 160)
    for dark in (False, True):
        suffix = '-dark' if dark else ''
        for kind in ('supportive', 'challenging', 'same', 'four'):
            target = (IMAGES / f'elements/cycle-four{suffix}.png' if kind == 'four' else
                      IMAGES / f'transformations/transformation-{kind}{suffix}.png')
            render(diagram_svg(kind, dark, labels), target, 440)
        # Both transports illustrate the same five-element supportive cycle.
        shutil.copyfile(IMAGES / f'transformations/transformation-supportive{suffix}.png',
                        IMAGES / f'elements/cycle-five{suffix}.png')
    print('Rendered eight exact trigrams, one cycle icon and ten relationship/cycle images.')


if __name__ == '__main__':
    main()
