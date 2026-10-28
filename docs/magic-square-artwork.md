# HTML magic square artwork

NineStarKi/_MagicSquare.cshtml remains the owner of the nine live numeric cells and direction overlays. Its markup, data-index values, number mapping, active/lunar classes, conditions and tooltips are unchanged.

Scoped styles in Content/less/sections/ninestar.less and matching compiled CSS use the already-loaded Cormorant Infant serif, bold lining numerals and larger sizes in default/small/medium/very-small displays. The square uses Images/ninestar/magic-square-plate.png, a 512px transparent decorative plate with a fine gold rim, quiet centre and restrained corner embossing. Grid rules remain CSS, not generated artwork. A 5% inset keeps the grid inside the thin plate rim. Active cells retain their existing scale, stacking and shadow and use the same gold substrate so neighbouring digits do not show through them.

Numeral colours match the approved energy icons: 1/6/8 white, 2 black, 3 bright green, 4 deep green, 5 yellow, 7 red, 9 purple. A fine bronze text edge gives contrast over gold. CSS image URLs retain the established relative Images path and work in a virtual directory. No global font, navigation layout, calculations or direction mapping changes. The plate is registered for publishing and the artwork catalogue is regenerated.

Validation checks scoped source/compiled CSS agreement, live-cell markup preservation, publish references, alpha and catalogue consistency. Desktop/mobile browser rendering and runtime MVC/IIS/VS2017 checks remain unverified here: the available browser runtime could not be installed.
