# HTML magic square artwork

NineStarKi/_MagicSquare.cshtml remains the owner of the nine live numeric cells and direction overlays. Its markup, data-index values, number mapping, active/lunar classes, conditions and tooltips are unchanged.

Scoped styles in Content/less/sections/ninestar.less and matching compiled CSS use the already-loaded Cormorant Infant serif, bold lining numerals and larger sizes in default/small/medium/very-small displays. The square uses Images/ninestar/magic-square-plate.png, a 512px transparent decorative plate with a fine gold rim, quiet centre and restrained corner embossing. Grid rules remain CSS, not generated artwork. A 5% inset keeps the grid inside the thin plate rim. Active cells retain their stacking and shadow, with restrained enlargement and use the same gold substrate so neighbouring digits do not show through them.

Numeral colours match the approved energy icons: 1/6/8 white, 2 black, 3 bright green, 4 deep green, 5 yellow, 7 red, 9 purple. A fine bronze text edge gives contrast over gold. CSS image URLs retain the established relative Images path and work in a virtual directory. No global font, navigation layout, calculations or direction mapping changes. The plate is registered for publishing and the artwork catalogue is regenerated.

Validation checks scoped source/compiled CSS agreement, live-cell markup preservation, publish references, alpha and catalogue consistency. Desktop/mobile browser rendering and runtime MVC/IIS/VS2017 checks remain unverified here: the available browser runtime could not be installed.

Numerals are wrapped in a dedicated span so the top row can move down 2px and the bottom row up 2px without moving the cell grid. A subtle text shadow adds relief. Main/lunar selected tiles scale to 1.08/1.04 and their numeral multipliers reduce to 1.4/1.2, keeping selected edge tiles inside the plate rim. Two-pixel bronze/champagne CSS pseudo-elements create recessed internal seams; they do not intercept pointer events or alter the direction overlay.
All magic squares on the My Chart screen use slightly smaller numerals while preserving the large, medium, small and very-small size hierarchy. Base sizes are 2.35rem, 1.8rem, 1.5rem and 1.125rem respectively; selected cells use 1.25 times their size. These rules are scoped by the My Chart page class and do not change prediction squares.
