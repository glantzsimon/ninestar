# Submit and AJAX loading artwork

The submit/link/payment indicator remains #pageSpinner, controlled by the existing JavaScript. AJAX panels still use $.fn.displaySpinner / hideSpinner and .partialSpinner. Their 50px boxes, centring, overlay styles and show/hide behaviour are unchanged. The initial pageFadeInOverlay and its yin-yang animation are separate and unchanged.

Both master layouts render _AjaxLoadingImage inside #pageSpinner. The panel helper clones this picture into newly created panel spinners. Panel loaders without that template use the existing GIF as a background fallback. The obsolete CSS border ring and its rotation keyframes are removed; loader containers do not rotate.

The browser selects animated shared/ajax-spinner-gold-animated.webp through the picture source MIME type; browsers without WebP/picture support use the existing shared/ninestar-spinner.gif img source. Reduced-motion browsers select the still shared/ajax-spinner-gold.png first. No user-agent sniffing or remote media host is used; Url.Content preserves local virtual paths. The new WebP is 160px square, 60 frames / 1.8 seconds, looping anticlockwise; exterior and centre retain alpha. The PNG still is 256px square. Images are registered for publishing and mobile catalogue revisions are regenerated.

Only artwork presentation changes. Submit validation, payment handlers, AJAX lifecycle, request data and loading container ownership are unchanged. LESS and compiled CSS receive identical flat presentation rules. Windows/IIS visual verification remains a local check.

The gold animation rotates about the measured geometric centre of its ring, with a fixed canvas/pivot for every frame. Rotation is performed only inside the WebP, never again on its HTML container. After updating local DEBUG assets, hard-refresh the page to discard cached spinner CSS.

The gold loader now completes two anticlockwise turns per 1.8-second loop. Its angular speed varies smoothly, peaking after the first turn and returning to its starting speed at the second; the fixed geometric pivot is retained. Small alpha-blended gold sparks emit and fade close to the rim within the existing 160px canvas. The GIF fallback and reduced-motion still are unchanged; only the WebP frames animate.
