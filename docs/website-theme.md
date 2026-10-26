# Website theme colours and borders

The LESS palette in `webapp/WebApplication/Content/less/config/colours.less` is the source of truth. Default custom properties live under `:root`; `.dark-mode` overrides the theme without changing the light values.

## Standard outlines

`@dark-border-color` is `#b8b8b8`, matching the mobile app. In dark mode, `--border-color` and `--border-grey` resolve to it. `--table-border-color`, `--border-muted-color` and `--border-inverse-color` resolve through `--border-color` so panels, form controls, dropdowns, wells/expandable help, table wrappers, calendars and standard navigation outlines share the same neutral colour.

The role-specific light defaults preserve existing appearance: `--border-muted-color` is the previous `@ghost-white` (#eeeeee), and `--border-inverse-color` is the previous white navbar-button outline. Their dark overrides use the shared border colour. Standard hardcoded white/near-white borders in dark-only rules now use `var(--border-color)`. Component rules that also apply in light mode use the role-specific token where necessary to retain their original light value.

Keep all existing border/outline widths and styles. Blue focus, selected-state and validation/status colours have separate meanings and must not be replaced with neutral grey. Decorative avatar/pie-chart white image framing, white text and image/background colours are unchanged. Existing intentionally darker grid lines remain distinct.

## Compiled CSS

Use the input/output mappings in `webapp/WebApplication/compilerconfig.json`. The shared palette is embedded by imports in 41 checked-in CSS outputs; update every affected copy together. Do not change relative image/font URLs while synchronising generated rules. The personal calendar already consumes the shared border variable and needs no additional override.

Validation for this change compiled all 69 configured LESS inputs successfully. The 41 affected CSS outputs were parsed and checked: 2,792 non-dark declarations retain their resolved light-theme values; 27 changed border/outline declarations preserve widths/styles; blue accent declarations are unchanged. Existing generated URL rewriting was retained. Browser visual validation and the Windows application build were not executed here.

After deploying assets, visually check dark/light panels, calculator controls and focused inputs, tables, expanded help, tabs, mobile navbar, and selected calendar days. Blue focus/selection should remain recognisable and standard dark outlines should be #b8b8b8.
