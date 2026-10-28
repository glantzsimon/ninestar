# Visual design standard

## Owner-approved direction

The website should feel beautiful, ancient, magical and mystical, appropriate to astrology and inner exploration. Use meaningful symbols and artwork rather than generic modern clip art. The owner likes a gold background within each emblem. Prefer warm antique gold, softly burnished champagne highlights and dark bronze engraved recesses, with restrained celestial geometry and a handcrafted relief finish. Avoid excessive intricacy, clutter, neon, cartoon faces and illegible decorative writing. Preserve clarity at the actual small navigation size.

The owner has approved this artwork direction for `webapp/WebApplication/Images/ui` and all subfolders of `webapp/WebApplication/Images/ninestar`. This establishes a direction for future work, not permission to restyle other folders, page layout, colours or controls automatically. Existing website theme tokens, blue focus accents, Dictionary-backed text and UI helper conventions remain authoritative.

The approved visual reference is the Constitution medallion below. Match its warm gold relief, thin rim, readable central symbol and transparent outer canvas when expanding the family.

![Approved Constitution artwork](../webapp/WebApplication/Images/ui/constitution.png)

## Asset invariants

Keep each existing canvas width and height, actual file format and filename, except explicitly approved renames. All artwork has a genuinely transparent surrounding canvas; a gold disc or object may itself be opaque. Preserve clean alpha edges when resizing and never flatten onto the page colour. No shadows, glow or solid rectangles outside the artwork. No baked-in UI labels; use Dictionary text in the interface. Numerals identifying nine-year and 81-year periods are meaningful symbols already present in the source artwork.

Preserve functional meanings: success, warning, failure, lock and selection must remain recognisable, including their established green, red and blue cues. Provider marks must remain identifiable. Functional gradients and animation must retain their original dimensions and rendering purpose. Do not add elaborate ornamental frames to tiny controls where that would obscure their state.

## Approved naming cleanup

| Original file | Replacement | Meaning |
| --- | --- | --- |
| `solar-nature.png` | `constitution.png` | Constitution / main energy |
| `lunar-nature.png` | `character.png` | Character / inner nature |
| `question-mark.jpg` | `question-mark.png` | Help; PNG conversion explicitly approved to permit transparency |

The existing website mapping was solar to Constitution and lunar to Character. Preserve that semantic mapping. `document.jpg` is actually a PNG, despite its extension; retain its existing filename and actual PNG encoding. `fail.gif` is animated and must remain GIF. `Thumbs.db` and `ui.lnk` are not image artwork and are not part of the redesign.

Active nature-label callers use the existing `Dictionary.CharacterLabel` instead of the obsolete `Dictionary.LunarNature` reference; both currently display Character. Preserve the old resource for compatibility. Solar/lunar astronomical cycles, lunar phases and legacy API protocol property names have separate domain meanings and are not subject to a global search-and-replace.

## Mobile catalogue compatibility

New artwork IDs are `ui.constitution` and `ui.character`. Previously published `ui.solar-nature` and `ui.lunar-nature` remain aliases resolving to the same new files and revisions. Canonical path lookups return the new IDs. `ui.question-mark` keeps its ID and resolves to the new PNG. Clients must obtain URLs from `/api/mobile/v1/metadata/artwork`, rather than reconstructing filenames or caching obsolete literal paths indefinitely.

After any image change, regenerate and check `tools/update-mobile-artwork.py`; ship the images and catalogue together. The existing content revisions and ETag mechanism invalidate cached artwork. Deploy changed images into the application’s Images folder, which is the catalogue’s same-origin source. If the website also uses a separately configured CDN, upload its copies there as well; changing repository files alone does not update that CDN.

## Generation and export

Artwork uses the built-in image-generation tool. The shared prompt requests a front-facing antique gold medallion, burnished champagne highlights, bronze recessed engraving, subtle handworked texture, thin raised rim, a strong readable symbol and actual transparent surrounding space, with no watermark or external shadow. Each asset receives its own subject brief. Final export must restore its exact original canvas dimensions and actual format while preserving alpha. Record subject briefs and validation in `docs/ui-artwork-report.md`.

## Nine Star Ki domain artwork

The `Images/ninestar` family includes energies, elements, modalities, organs, seasons, trigrams, transformations and yin/yang. Preserve energy numerals, recognisable organ/element subjects and the exact top-to-bottom trigram line patterns. The centre has no assigned trigram; do not invent one. Preserve yin/yang polarity and neutral unspecified state. Use deep bronze against gold for readable symbolic contrast.

Relationship diagrams retain open transparent space between gold element emblems. Their connections and trigram bars are precise vector geometry, not guessed generated diagram marks. `tools/render-ninestar-diagrams.py` renders the checked patterns and directed relationships using the generated gold element artwork. Its existing element labels come from Dictionary.resx. Light and dark variants preserve their respective label/arrow contrast, original filenames and dimensions. The cycle-of-five and supportive transformation share the same directed five-element graph; the former dark image incorrectly used sibling pairs and is corrected by this refresh. See `docs/ninestar-artwork-report.md` for the detailed audit and validation.

Energy identifiers use large central serif numerals (at least 75% height as the design target), in their established colours: white Water/Heaven/Mountain, black Soil, bright green Thunder, deep green Wind, yellow Core Earth, red Lake and purple Fire. Composite the numeral fill at 90% opacity over a single continuous artwork background; do not generate a different imitation pattern inside the number. Preserve the approved visible background and transparent exterior. Core Earth uses minimal circular core/globe embossing.

The HTML magic square uses the existing Cormorant Infant serif font in bold lining numerals, enlarged at all four existing size presets. A thin gold embossed square plate sits behind the live grid; energy colours move to the numerals to match the medallions. Active/lunar-house scaling and direction overlays retain their existing semantics. The decorative plate contains no numbers or grid lines.
