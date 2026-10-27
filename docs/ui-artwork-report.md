# UI artwork refresh report

## Reference audit

The original website maps `solar-nature.png` to the Constitution tab and `lunar-nature.png` to the Character tab, in both the personal-chart and compatibility views. The rename preserves this existing mapping: `constitution.png` and `character.png`. Updated both views, project content registrations and the explicit mobile artwork allowlist. `question-mark.jpg` is converted to `question-mark.png` with the owner's explicit approval to enable transparency.

Two active `Dictionary.LunarNature` callers remained: the personal-chart tab and `NineStarKiEnergy.CharacteEnergyLabel`. The old resource already displays Character. Both callers now use the existing `Dictionary.CharacterLabel`, preserving the visible wording. The old resource/accessor is retained for compatibility. No active solar-nature/lunar-nature file references remain. Solar/lunar astronomy terminology, compatibility API property names such as `solarElements` and `lunarElements`, and other unrelated folder names are intentionally unchanged.

The folder contains 58 images, plus `Thumbs.db` and `ui.lnk`, which are not images. The 1 × 30 `fade-out.png` is a functional alpha gradient, not decorative artwork, and remains byte-for-byte unchanged. All other images receive individual redesigns. Keep existing filenames and canvas sizes except the three approved renames.

## Design

The owner approved the first generated images and the antique gold direction. See `visual-design.md` for the lasting site-wide artwork standard. This implementation changes only UI folder artwork and the references required by the naming cleanup. No page layout, styling tokens, memberships, calculations or reading content changes.

The built-in image-generation tool creates each artwork separately. The shared brief is: front-on warm antique gold medallion, restrained thin rim, burnished champagne highlights, bronze recessed engraving, subtle handworked texture, strong symbol legible at navigation size, genuinely transparent surrounding space, no external shadows, watermark or decorative writing. Attachment/help graphics use a portrait gold tablet/plaque variant. Functional state colours remain meaningful.

[View the complete preview sheet](ui-artwork-preview.png).

## Image inventory and subject briefs

Dimensions are width × height. Format means actual encoding rather than filename suffix; `document.jpg` is and remains PNG. Numerals 9 and 81 identify the existing period icons.

| Original | Final | Dimensions | Encoding | Subject / treatment |
| --- | --- | --- | --- | --- |
| `adult-child-relationship.png` | `adult-child-relationship.png` | 160 × 160 | PNG | Large and small hands touching, adult and child connection. |
| `alchemy.png` | `alchemy.png` | 160 × 160 | PNG | Balanced alchemical star within a thin orbit. |
| `audio.png` | `audio.png` | 216 × 300 | PNG | a tall elegant ancient gold tablet with a prominent speaker symbol and two clean sound-wave arcs, recognisably an audio attachment. Rounded narrow tablet, ratio 216 wide by 300 high. |
| `career.png` | `career.png` | 160 × 160 | PNG | Antique compass and rising path, vocation. |
| `daily-cycle.png` | `daily-cycle.png` | 160 × 160 | PNG | a simple sun and crescent moon in balanced circular day-night motion, a single elegant celestial emblem. |
| `daily-planner.png` | `daily-planner.png` | 160 × 160 | PNG | a small ancient calendar tablet with a prominent sun and crescent, four clear day cells; no modern digits or letters. |
| `daystar.png` | `daystar.png` | 160 × 160 | PNG | one radiant four-point guiding star, a small secondary star, clear and luminous engraved relief. |
| `default-avatar.png` | `default-avatar.png` | 108 × 108 | PNG | a neutral anonymous human bust silhouette, inclusive and simple, a small unlabelled celestial halo, clearly a profile placeholder. |
| `document.jpg` | `document.jpg` | 216 × 300 | PNG | a tall elegant antique gold scroll or sheet with a folded corner and three simple engraved horizontal lines, recognisably a document attachment. Ratio 216 wide by 300 high. |
| `eighty-one-cycle.png` | `eighty-one-cycle.png` | 160 × 160 | PNG | an elegant solar disc bearing the exact large numerals 81; few restrained orbit marks, legible at small sizes. |
| `eighty-one-planner.png` | `eighty-one-planner.png` | 160 × 160 | PNG | an ancient calendar tablet with the exact large numerals 81 in the centre and a thin orbit around it. |
| `emotional-energy.png` | `emotional-energy.png` | 160 × 160 | PNG | one graceful sculpted heart containing a small radiant star, emotional warmth without neon. |
| `epoch.png` | `epoch.png` | 160 × 160 | PNG | a timeless ancient tree with broad roots and three ascending boughs within concentric celestial arcs, long collective ages. |
| `facebook_sm.png` | `facebook_sm.png` | 64 × 64 | PNG | the recognisable lowercase Facebook f silhouette in dark sapphire-blue enamel, on antique gold, no embellishments that obscure the f; clearly a social provider sign-in icon. |
| `fade-out.png` | `fade-out.png` | 1 × 30 | PNG | Functional gradient retained unchanged. |
| `fail-red.png` | `fail-red.png` | 96 × 96 | PNG | a large clear dark-red engraved exclamation mark within a thin red ring on the gold face, unmistakable failure alert, preserve strong red status cue. |
| `fail.gif` | `fail.gif` | 111 × 111 | GIF | an antique broken seal with a prominent deep-red cross symbol, clean clear failure or error cue, no cartoon face. |
| `fail.png` | `fail.png` | 120 × 120 | PNG | an antique broken seal with a prominent deep-red cross symbol, clean clear failure or error cue, no cartoon face. |
| `finances.png` | `finances.png` | 160 × 160 | PNG | a hand supporting one antique gold coin with a simple sprouting leaf engraved on it, prosperity and stewardship, no currency lettering. |
| `five-elements.png` | `five-elements.png` | 160 × 160 | PNG | a precise five-point cycle of five simple elemental emblems: water droplet, tree leaf, flame, mountain earth, metal ring; balanced pentagonal arrangement, no text. |
| `fundamental-nature.png` | `fundamental-nature.png` | 160 × 160 | PNG | a serene seated person with a simple small halo of nine subtle dots, foundational inner being, calm bold engraved silhouette. |
| `generation.png` | `generation.png` | 160 × 160 | PNG | three graceful joined human silhouettes of different ages beneath a gentle celestial arc; shared generational connection, readable simple engraving. |
| `google_sm.png` | `google_sm.png` | 64 × 64 | PNG | the recognisable simple Google G silhouette in the familiar four Google colours, on antique gold, preserve recognisability; clearly a social provider sign-in icon. |
| `health.png` | `health.png` | 160 × 160 | PNG | a flourishing healing leaf and a small radiant heart, harmonious vitality, clean sculptural relief. |
| `hourly-cycle.png` | `hourly-cycle.png` | 160 × 160 | PNG | an antique sundial face with clear hands and twelve simple tick marks, no numerals or lettering. |
| `hourly-planner.png` | `hourly-planner.png` | 160 × 160 | PNG | an antique calendar tablet with a prominent small sundial clock, no numerals or lettering. |
| `in-depth-analysis.png` | `in-depth-analysis.png` | 160 × 160 | PNG | a beautiful simple magnifying lens over a small celestial star map, clear uncluttered symbol. |
| `inner-child.png` | `inner-child.png` | 160 × 160 | PNG | a small joyful child reaching for a single guiding star, tender and graceful, simplified human silhouette. |
| `intellectual-qualities.png` | `intellectual-qualities.png` | 160 × 160 | PNG | an elegant human profile containing a simple luminous six-point star and three connected thought nodes, intellectual clarity. |
| `interpersonal-qualities.png` | `interpersonal-qualities.png` | 160 × 160 | PNG | two human profiles facing each other with a small balanced star between them, understanding and communication. |
| `locked.png` | `locked.png` | 120 × 120 | PNG | a bold closed antique padlock with a clear keyhole, recognisable restricted access symbol, subtle dark bronze inset. |
| `lunar-nature.png` | `character.png` | 160 × 160 | PNG | Human profile with a crescent, inner character. |
| `mirror-energy.png` | `mirror-energy.png` | 160 × 160 | PNG | a polished ancient circular hand mirror with a small central reflection star and restrained gold rim, engraved relief. |
| `monthly-planner.png` | `monthly-planner.png` | 160 × 160 | PNG | an antique calendar tablet with a prominent crescent moon, a few clear day cells, no lettering or numerals. |
| `moon.png` | `moon.png` | 160 × 160 | PNG | one large beautiful sculpted crescent moon, gentle antique gold relief, clear recognisable silhouette. |
| `nine-year-cycle.png` | `nine-year-cycle.png` | 160 × 160 | PNG | a simple radiant solar disc bearing the exact large numeral 9, restrained orbit marks, highly legible. |
| `nine-year-planner.png` | `nine-year-planner.png` | 160 × 160 | PNG | an antique calendar tablet bearing the exact large numeral 9 at its centre and a thin orbit, no other writing. |
| `notfound.png` | `notfound.png` | 120 × 120 | PNG | a simple antique star compass with a clear broken path ending at a small question mark, information not found, no other text. |
| `notfoundold.png` | `notfoundold.png` | 120 × 120 | PNG | an antique scroll with a single large question mark engraved at its centre, missing information, no other text. |
| `overview.png` | `overview.png` | 160 × 160 | PNG | a single graceful eye of insight within a subtle circular celestial arc, tasteful ancient engraving, clear bold silhouette. |
| `predictions.png` | `predictions.png` | 160 × 160 | PNG | a beautiful simple antique armillary sphere with a few planetary orbits and a guiding star, future celestial cycles, uncluttered engraving. |
| `question-mark.jpg` | `question-mark.png` | 236 × 350 | PNG | one large sculpted dark-bronze question mark on an elongated antique gold oval plaque, clean simple help symbol, absolutely no other markings, ratio 236 wide by 350 high. |
| `relationships-strengths-challenges.png` | `relationships-strengths-challenges.png` | 160 × 160 | PNG | two hands joined beneath a balanced pair of opposing leaf and flame symbols, complementary strengths and challenges, strong simple relief. |
| `relationships.png` | `relationships.png` | 160 × 160 | PNG | two graceful human profiles turned toward one another, with a small heart or shared star between them, warm loving connection. |
| `selected.png` | `selected.png` | 72 × 72 | PNG | one bold dark-bronze check mark inside a simple thin gold circular ring, unmistakably selected, extremely clean at 72px, very little detail. |
| `social-expression.png` | `social-expression.png` | 160 × 160 | PNG | three simple human silhouettes beneath outward radiating sun rays, social expression and presence. |
| `solar-nature.png` | `constitution.png` | 160 × 160 | PNG | Seated human figure in a restrained solar halo, foundational energy. |
| `spirituality.png` | `spirituality.png` | 160 × 160 | PNG | a serene seated person above a simple lotus with one tiny guiding star overhead, inner spiritual light, elegant bold relief. |
| `stress-response.png` | `stress-response.png` | 160 × 160 | PNG | a pair of hands cradling a small flame within a protective circle, emotional steadiness under pressure, clear readable engraving. |
| `success.png` | `success.png` | 96 × 96 | PNG | a large vivid emerald-green engraved check mark within a green circular inset, clear success state, strong positive green cue. |
| `sun.png` | `sun.png` | 160 × 160 | PNG | one beautiful raised solar disc with a small number of bold tapered rays, no face, no writing. |
| `unauthorized.png` | `unauthorized.png` | 120 × 120 | PNG | an upright open palm stop symbol within a clear red circular inset, unmistakable access denied state, no letters. |
| `unselected.png` | `unselected.png` | 72 × 72 | PNG | an empty simple antique gold circular ring with a quiet gold inset, no mark inside it, clearly an unchecked state at 72px, no decorative elements in centre. |
| `upgrade.png` | `upgrade.png` | 96 × 96 | PNG | a bold upward arrow in rich sapphire-blue enamel on the gold face, clear upgrade state, preserve blue cue. |
| `video.png` | `video.png` | 216 × 300 | PNG | a tall elegant ancient gold tablet with a prominent play triangle within a circle, recognisably a video attachment. Rounded narrow tablet, ratio 216 wide by 300 high. |
| `warning.png` | `warning.png` | 120 × 120 | PNG | a prominent amber triangle containing a dark exclamation mark, restrained old gold finish, unmistakable warning icon. |
| `yearly-planner.png` | `yearly-planner.png` | 160 × 160 | PNG | an antique calendar tablet with a prominent sun emblem and a few clear day cells, no lettering or numerals. |
| `yinyang.png` | `yinyang.png` | 160 × 160 | PNG | a perfectly balanced traditional yin-yang symbol with two opposing droplets and their dots, bronze and gold relief, preserve exact familiar geometry. |

## API and deployment

Canonical catalogue IDs are `ui.constitution` and `ui.character`. Old IDs `ui.solar-nature` and `ui.lunar-nature` remain aliases with the same URL and revision as their replacements. `ui.question-mark` remains the same ID but now supplies PNG content. The catalogue generator emits aliases after the canonical entries, and path lookup chooses the canonical first entry. Clients should resolve catalogue URLs instead of constructing filenames. Updated registry regression coverage checks canonical IDs, old aliases and replacement URLs.

Deploy the artwork and regenerated catalogue together. Views use `MediaService.BaseImagesPath`; if this points at a separate CDN, the images must be uploaded there. No deployment, media-host upload, Windows build or xUnit execution is performed by this task.

## Validation

Passed source/export checks: all 58 original images retain their canvas dimensions and actual formats except the approved question-mark JPEG-to-PNG conversion; all 57 redesigned originals and seven additions have transparent surrounding pixels. The functional gradient and both non-image files are unchanged. The animated failure GIF retains transparency, its 111 × 111 canvas, looping and its 18-second total duration; the new engraved alert gently pulses in place instead of using the previous cartoon animation.

The catalogue regeneration check passes: 438 file registrations, 433 logical IDs, 440 variant registrations including two aliases, and 431 distinct contents. Canonical and legacy IDs resolve to identical replacement artwork. Project XML/content registrations are valid and unique; changed C# parses successfully and whitespace checks pass. Added Windows regression coverage for canonical/legacy mappings and all seven new navigation IDs. The complete final sheet was visually reviewed on both dark and light surfaces at actual output sizes, including symbols, status cues and absence of unintended lettering.

A Windows VS2017 build, xUnit execution and visual verification on the running website have not been performed. No deployment or CDN upload was performed. These remain release checks.

## Additional navigation icons

Seven new 160 × 160 transparent PNGs use the same approved gold family. Reuse existing subjects where appropriate instead of creating duplicates. These assets are registered in the project and public catalogue; no new website navigation or reading is implemented.

| Requested use | Filename in Images/ui | API artwork ID | Treatment |
| --- | --- | --- | --- |
| Knowledge Base | `knowledge-base.png` | `ui.knowledge-base` | New: open book and guiding star |
| Articles | `articles.png` | `ui.articles` | New: parchment pages and quill |
| AI Astrologer | `ai-astrologer.png` | `ui.ai-astrologer` | New: wise face and connected constellation |
| Personal Chart | `personal-chart.png` | `ui.personal-chart` | New: personal celestial wheel |
| Predictions | `predictions.png` | `ui.predictions` | Reuse redesigned celestial instrument |
| Compatibility | `relationships.png` | `ui.relationships` | Reuse redesigned relationship symbol |
| Origins | `origins.png` | `ui.origins` | New: rooted seed/tree and guiding star |
| Cycles of Light and Dark | `daily-cycle.png` | `ui.daily-cycle` | Reuse redesigned sun/moon cycle |
| Yin and Yang | `yinyang.png` | `ui.yinyang` | Reuse redesigned yin-yang |
| Nine Core Energies | `nine-core-energies.png` | `ui.nine-core-energies` | New: nine-part solar arrangement |
| Nine Character Energies | `nine-character-energies.png` | `ui.nine-character-energies` | New: nine-part celestial arrangement with a crescent centre |
| Yearly Predictions | `sun.png` | `ui.sun` | Reuse redesigned sun |
| Monthly Predictions | `moon.png` | `ui.moon` | Reuse redesigned moon |
| Relationships | `relationships.png` | `ui.relationships` | Reuse redesigned relationship symbol |

The final scope is 57 redesigned images, seven new images and the unchanged functional gradient: 65 image files in UI. `Thumbs.db` and `ui.lnk` remain unchanged. No extra duplicate icons are added for synonymous navigation uses.
