# Nine Star Ki artwork refresh

## Scope and visual standard

Redesigned all 60 PNG files in `webapp/WebApplication/Images/ninestar`, including every subfolder: 9 energies, 10 elements/cycle images, 3 modalities, 10 organs, 9 seasons, 9 trigrams, 6 transformation diagrams and 4 yin/yang images. Every filename, original pixel canvas, PNG encoding, catalogue ID and theme-variant name is preserved. No images are added to or removed from this folder.

The owner-approved UI medallion is the reference: warm antique gold, burnished champagne highlights, dark bronze engraved recesses, restrained craftsmanship and a clear symbolic centre, with genuinely transparent surrounding space. See [visual-design.md](visual-design.md). [Complete preview sheet](ninestar-artwork-preview.png).

The built-in image-generation tool creates the sculpted illustration, using the approved Constitution medallion as its explicit style reference. Organ silhouettes and polarity icons also use their old artwork as subject references. The existing approved blank gold medallion supplies the neutral trigram background. Export restores exact original dimensions without flattening alpha. Precise symbols and relationships are composed as vector geometry with `tools/render-ninestar-diagrams.py`, rather than trusting generated text, bars or arrows.

## Source audit and correction

The old `elements/cycle-five-dark.png` was byte-identical to `transformations/transformation-same-dark.png`: it showed five sibling pairs. The light cycle-of-five file and its owning knowledge-base article, `Globalisation/Elements/en-gb/five-elements.htm`, instead show Earth between Fire and Metal in the supportive cycle. The refresh corrects the dark cycle-of-five image to the same directed cycle as the light image. The old light cycle-of-four graphic also omitted its Fire label; both redrawn variants now contain all five existing Dictionary element labels. Both cycle-of-five files deliberately share their final content with the matching supportive-transformation file.

`elements/earth.png` and `modalities/stable.png` were also identical. They continue to share the same redesigned grounded mountain/earth image. Other subjects retain their distinctions. All energy numbers remain present: Water 1, Soil 2, Thunder 3, Wind 4, Core Earth 5, Heaven 6, Lake 7, Mountain 8 and Fire 9. Organ symbols remain anatomical illustrations, not generic decorative heart/leaf substitutes. Yin and yang keep their opposite lobe polarities; unspecified stays neutral.

## Exact geometry and labels

The renderer reads Fire, Earth, Metal, Water and Tree from their existing `Dictionary.resx` entries. It introduces no new user-facing wording or runtime resource lookup. It preserves the existing English graphic labels and draws them in an italic serif face, deep bronze in light variants and bright champagne in dark variants. The five-element positions are Fire at top, Earth upper right, Metal lower right, Water lower left and Tree upper left.

- Supportive / cycle of five: Fire → Earth → Metal → Water → Tree → Fire.
- Challenging: Tree → Earth, Earth → Water, Water → Fire, Fire → Metal, Metal → Tree; five directed star edges without junction nodes.
- Sibling: reciprocal arrows inside each same-element pair; no arrows between different elements.
- Cycle of four: Tree → Fire → Metal → Water → Tree; Earth remains unconnected in the centre.
- The small generic cycle icon has four clockwise arrows.

Trigrams are specified top to bottom. Solid bars are unbroken; broken bars have exactly two equal halves and a clear central gap. Centre intentionally has no trigram and remains blank.

| Trigram | Top | Middle | Bottom |
| --- | --- | --- | --- |
| Heaven | Solid | Solid | Solid |
| Soil | Broken | Broken | Broken |
| Thunder | Broken | Broken | Solid |
| Wind | Solid | Solid | Broken |
| Water | Broken | Solid | Broken |
| Fire | Solid | Broken | Solid |
| Mountain | Solid | Broken | Broken |
| Lake | Broken | Solid | Solid |
| Core Earth | None | None | None |

The eight line patterns, small cycle symbol and ten relationship/cycle PNGs are rendered with exact SVG geometry over the gold artwork. The centre is not assigned a fabricated trigram. No website page layout, theme tokens, calculations, memberships, access gates or purchasing behaviour changes.

## File inventory

All paths below are relative to Images/ninestar. The artwork ID and variant remain the same as before; clients must resolve the returned catalogue URL rather than infer a filename from an ID. Shared images may return a canonical path belonging to an equivalent asset.

| File | Pixels | Artwork ID | Variant | Subject / treatment |
| --- | --- | --- | --- | --- |
| `elements/cycle-five-dark.png` | 440 × 440 | `ninestar.elements.cycle-five` | dark | Exact supportive five-element cycle; shared content per theme. |
| `elements/cycle-five.png` | 440 × 440 | `ninestar.elements.cycle-five` | default | Exact supportive five-element cycle; shared content per theme. |
| `elements/cycle-four-dark.png` | 440 × 440 | `ninestar.elements.cycle-four` | dark | Exact four-stage cycle with unconnected Earth at centre. |
| `elements/cycle-four.png` | 440 × 440 | `ninestar.elements.cycle-four` | default | Exact four-stage cycle with unconnected Earth at centre. |
| `elements/cycle.png` | 160 × 160 | `ninestar.elements.cycle` | default | Four precise clockwise vector arrows on the gold medallion. |
| `elements/earth.png` | 160 × 160 | `ninestar.elements.earth` | default | A sturdy mountain and gently layered earth, pure earth element and grounded stability. No numbers. |
| `elements/fire.png` | 160 × 160 | `ninestar.elements.fire` | default | One strong sculpted flame, pure fire element. No numbers. |
| `elements/metal.png` | 160 × 160 | `ninestar.elements.metal` | default | An elegant polished metal disc with concentric turned-metal rings, unmistakable metal element, no numbers. |
| `elements/tree.png` | 160 × 160 | `ninestar.elements.tree` | default | One beautiful rooted tree with a spreading leafy canopy, pure tree element. No numbers. |
| `elements/water.png` | 160 × 160 | `ninestar.elements.water` | default | A bold sculpted water droplet above concentric ripples, pure water element. No numbers. |
| `energies/coreearth.png` | 160 × 160 | `ninestar.energies.coreearth` | default | An Earth globe with recognisable continents symbolising the centre, with the single large unmistakable Arabic numeral 5 in dark bronze at lower centre. |
| `energies/fire.png` | 160 × 160 | `ninestar.energies.fire` | default | A bold upward dancing flame, with the single large unmistakable Arabic numeral 9 in dark bronze at lower centre. |
| `energies/heaven.png` | 160 × 160 | `ninestar.energies.heaven` | default | A luminous open sky with two restrained sculpted clouds, with the single large unmistakable Arabic numeral 6 in dark bronze at lower centre. |
| `energies/lake.png` | 160 × 160 | `ninestar.energies.lake` | default | A still lake with a low distant mountain reflected in it, with the single large unmistakable Arabic numeral 7 in dark bronze at lower centre. |
| `energies/mountain.png` | 160 × 160 | `ninestar.energies.mountain` | default | One strong majestic mountain peak, with the single large unmistakable Arabic numeral 8 in dark bronze at lower centre. |
| `energies/soil.png` | 160 × 160 | `ninestar.energies.soil` | default | Layered fertile earth and gently furrowed soil, with the single large unmistakable Arabic numeral 2 in dark bronze at lower centre. |
| `energies/thunder.png` | 160 × 160 | `ninestar.energies.thunder` | default | A bold lightning bolt emerging from a small storm cloud, with the single large unmistakable Arabic numeral 3 in dark bronze at lower centre. |
| `energies/water.png` | 160 × 160 | `ninestar.energies.water` | default | A sculpted water droplet above flowing ripples, with the single large unmistakable Arabic numeral 1 in dark bronze at lower centre. |
| `energies/wind.png` | 160 × 160 | `ninestar.energies.wind` | default | Gracefully flowing wind ribbons through a simple leafy branch, with the single large unmistakable Arabic numeral 4 in dark bronze at lower centre. |
| `modalities/dynamic.png` | 160 × 160 | `ninestar.modalities.dynamic` | default | A bold energetic burst of sculpted sparks and rays, dynamic outward movement. No numbers. |
| `modalities/reflective.png` | 160 × 160 | `ninestar.modalities.reflective` | default | A still lake reflecting a mountain beneath a quiet crescent, inward reflective calm. No numbers. |
| `modalities/stable.png` | 160 × 160 | `ninestar.modalities.stable` | default | Shared with the redesigned Earth element: grounded mountain/earth. |
| `organs/gallbladder.png` | 160 × 160 | `ninestar.organs.gallbladder` | default | A recognisable anatomical gallbladder: pear-shaped small sac with its slender branching bile ducts; preserve the source silhouette, no labels. |
| `organs/heart.png` | 160 × 160 | `ninestar.organs.heart` | default | A recognisable anatomical human heart with its main upper vessel shapes, not a Valentine heart; preserve the source silhouette, no labels. |
| `organs/kidneys.png` | 160 × 160 | `ninestar.organs.kidneys` | default | Two recognisable bean-shaped kidneys arranged as a pair, anatomically styled simple relief; preserve source silhouette, no labels. |
| `organs/largeintestine.png` | 160 × 160 | `ninestar.organs.largeintestine` | default | A recognisable large intestine, a broad colon outline framing an open centre with short terminal stem; preserve source silhouette, no labels. |
| `organs/liver.png` | 160 × 160 | `ninestar.organs.liver` | default | A recognisable anatomical liver with one large and one small lobe; preserve the source silhouette, no labels. |
| `organs/lungs.png` | 160 × 160 | `ninestar.organs.lungs` | default | Two recognisable lungs with central trachea and simple bronchial branches; preserve the source silhouette, no labels. |
| `organs/smallintestine.png` | 160 × 160 | `ninestar.organs.smallintestine` | default | Recognisable gently coiled small intestine loops, distinct from the enclosing large colon; preserve the source silhouette, no labels. |
| `organs/spleenpancreas.png` | 160 × 160 | `ninestar.organs.spleenpancreas` | default | Recognisable pancreas and spleen together, elongated lobulated pancreas beside a small curved oval spleen; preserve the source silhouettes, no labels. |
| `organs/stomach.png` | 160 × 160 | `ninestar.organs.stomach` | default | A recognisable curved anatomical stomach sac with its entry and exit tubes; preserve the source silhouette, no labels. |
| `organs/urinarybladder.png` | 160 × 160 | `ninestar.organs.urinarybladder` | default | A recognisable simple anatomical urinary bladder, rounded sac with narrow lower outlet; preserve the source silhouette, no labels. |
| `seasons/autumntowinter.png` | 160 × 160 | `ninestar.seasons.autumntowinter` | default | A mountain landscape with a few late autumn leaves and a trace of snow, the quiet transition from autumn to winter. No letters or numbers. |
| `seasons/centre.png` | 160 × 160 | `ninestar.seasons.centre` | default | Graceful balanced cloud forms opening around a quiet light, the neutral seasonal centre. No letters or numbers. |
| `seasons/earlyautumn.png` | 160 × 160 | `ninestar.seasons.earlyautumn` | default | A ripe apple on a leafy branch, early autumn fruitfulness. No letters or numbers. |
| `seasons/earlyspring.png` | 160 × 160 | `ninestar.seasons.earlyspring` | default | Two graceful daffodils with fresh leaves, early spring awakening. No letters or numbers. |
| `seasons/lateautumn.png` | 160 × 160 | `ninestar.seasons.lateautumn` | default | A ripe pumpkin with two autumn leaves, late autumn harvest. No letters or numbers. |
| `seasons/latespring.png` | 160 × 160 | `ninestar.seasons.latespring` | default | A flourishing leafy tree above fresh grass, abundant late spring growth. No letters or numbers. |
| `seasons/summer.png` | 160 × 160 | `ninestar.seasons.summer` | default | A radiant sun and a single sunflower, warm summer abundance. No letters or numbers. |
| `seasons/winter.png` | 160 × 160 | `ninestar.seasons.winter` | default | A quiet snow-covered landscape with one bare tree and a low sun, winter stillness. No letters or numbers. |
| `seasons/wintertospring.png` | 160 × 160 | `ninestar.seasons.wintertospring` | default | A woodland branch with first opening buds above thawing ground, winter changing into spring. No letters or numbers. |
| `transformations/transformation-challenging-dark.png` | 440 × 440 | `ninestar.transformations.transformation-challenging` | dark | Exact directed challenging star. |
| `transformations/transformation-challenging.png` | 440 × 440 | `ninestar.transformations.transformation-challenging` | default | Exact directed challenging star. |
| `transformations/transformation-same-dark.png` | 440 × 440 | `ninestar.transformations.transformation-same` | dark | Exact reciprocal same-element pairs. |
| `transformations/transformation-same.png` | 440 × 440 | `ninestar.transformations.transformation-same` | default | Exact reciprocal same-element pairs. |
| `transformations/transformation-supportive-dark.png` | 440 × 440 | `ninestar.transformations.transformation-supportive` | dark | Exact supportive five-element cycle; shared content per theme. |
| `transformations/transformation-supportive.png` | 440 × 440 | `ninestar.transformations.transformation-supportive` | default | Exact supportive five-element cycle; shared content per theme. |
| `trigrams/trigram-coreearth-square.png` | 160 × 160 | `ninestar.trigrams.trigram-coreearth-square` | default | Blank gold centre, no assigned trigram. |
| `trigrams/trigram-fire-square.png` | 160 × 160 | `ninestar.trigrams.trigram-fire-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-heaven-square.png` | 160 × 160 | `ninestar.trigrams.trigram-heaven-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-lake-square.png` | 160 × 160 | `ninestar.trigrams.trigram-lake-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-mountain-square.png` | 160 × 160 | `ninestar.trigrams.trigram-mountain-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-soil-square.png` | 160 × 160 | `ninestar.trigrams.trigram-soil-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-thunder-square.png` | 160 × 160 | `ninestar.trigrams.trigram-thunder-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-water-square.png` | 160 × 160 | `ninestar.trigrams.trigram-water-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `trigrams/trigram-wind-square.png` | 160 × 160 | `ninestar.trigrams.trigram-wind-square` | default | Exact vector bars on the blank gold medallion; pattern above. |
| `yinyang/unspecified.png` | 160 × 160 | `ninestar.yinyang.unspecified` | default | A neutral yin-yang emblem: both lobes the SAME middle-gold tone separated by a subtle engraved S curve, two hollow engraved circular dots, neither lobe darkened to imply a polarity. No numbers or labels. |
| `yinyang/yang.png` | 160 × 160 | `ninestar.yinyang.yang` | default | A precise traditional yin-yang emblem oriented vertically with BRIGHT GOLD upper lobe and DARK BRONZE lower lobe; a dark bronze dot in the upper gold lobe and a bright gold dot in the lower dark lobe. Match the source orientation. No numbers or labels. |
| `yinyang/yin.png` | 160 × 160 | `ninestar.yinyang.yin` | default | A precise traditional yin-yang emblem oriented vertically with DARK BRONZE upper lobe and BRIGHT GOLD lower lobe; a bright gold dot in the upper dark lobe and a dark bronze dot in the lower bright lobe. Match the source orientation. No numbers or labels. |
| `yinyang/yinyang-large.png` | 820 × 819 | `ninestar.yinyang.yinyang-large` | default | A large precise traditional balanced yin-yang emblem with DARK BRONZE upper-left lobe and BRIGHT GOLD lower-right lobe, each containing one opposite-tone dot, gracefully rounded S boundary, no numbers or labels. |

## Catalogue host isolation

The mobile catalogue previously took its base from `MediaService.BaseImagesPath`. That shared website media setting could point Integration at a Production host or CDN. `MobileArtworkController` now builds its image base from the actual request origin and application path plus Images, independently of scheduled website media selection. Integration returns Integration URLs; Production returns its own host; local virtual directories and ports are retained. The website's existing CDN behaviour and embedded reading HTML media selection remain separate.

Deploy image files into this installation's Images folder. If website views also use a separate configured CDN, upload their copies there too. Keep mobile catalogue caches separated by environment and refresh after deployment. The request-derived base contributes to the existing version/ETag, so changing an old Production-based Integration catalogue returns new content rather than an incorrect 304. No client-side host rewriting is required.

Added controller regression cases cover Integration, Production, root applications, null/root application paths, local virtual directories, non-default ports and HTTP/HTTPS. Existing public max-age=300, ETag/304, unavailable JSON and identity-free catalogue behaviour are retained.

## Regeneration and validation

To regenerate exact diagrams after a source-artwork or Dictionary change, install Pillow, Inkscape and DejaVu Serif, then run:

```sh
python tools/render-ninestar-diagrams.py
python tools/update-mobile-artwork.py
python tools/update-mobile-artwork.py --check
```

These are offline development tools, not website startup dependencies. The final application serves compiled metadata and checked-in PNGs without directory scans, generation or external health checks in the catalogue action.

Passed validation: all 60 original paths, dimensions, PNG formats and transparent corners are preserved; every image changed. The rasterised trigram bars match all eight canonical top-to-bottom patterns, and the centre remains blank. Directed SVG paths were checked against the four-stage, supportive and challenging relationships, including arrow counts and sibling-only reciprocal pairs. Rendering is byte-for-byte repeatable. Both cycle-of-five variants match their supportive diagrams and differ from sibling diagrams.

Catalogue regeneration passes with the same 433 logical IDs, 440 variants and 438 files; 430 distinct contents reflect deliberate sharing. All metadata IDs, paths, MIME types, dimensions and variant names match the previous catalogue. Final sheets were reviewed at output sizes; energy numerals, anatomical silhouettes and yin/yang polarity remain recognisable. C# syntax, Python syntax and whitespace checks pass. Added Windows regression coverage protects the cycle-of-five themes and request-origin image hosting.

The Windows VS2017 build, xUnit execution, actual IIS/static-image responses, running-site visual checks and deployment have not been performed here. No site restart, migration or CDN upload was run.
