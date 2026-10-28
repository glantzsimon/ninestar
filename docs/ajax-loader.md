# Submit and AJAX loading artwork

The submit/link/payment indicator remains #pageSpinner, controlled by the existing JavaScript. AJAX panels still use $.fn.displaySpinner / hideSpinner and .partialSpinner. Their 50px boxes, centring, overlay styles and show/hide behaviour are unchanged. The initial pageFadeInOverlay and its yin-yang animation are separate and unchanged.

Both master layouts render _AjaxLoadingImage inside #pageSpinner. The panel helper clones this picture into newly created panel spinners. The existing CSS ring remains a fallback when that template is absent.

The browser selects animated shared/ajax-spinner-gold-animated.webp through the picture source MIME type; browsers without WebP/picture support use the existing shared/ninestar-spinner.gif img source. Reduced-motion browsers select the still shared/ajax-spinner-gold.png first. No user-agent sniffing or remote media host is used; Url.Content preserves local virtual paths. The new WebP is 160px square, 60 frames / 1.8 seconds, looping anticlockwise; exterior and centre retain alpha. The PNG still is 256px square. Images are registered for publishing and mobile catalogue revisions are regenerated.

Only artwork presentation changes. Submit validation, payment handlers, AJAX lifecycle, request data and loading container ownership are unchanged. LESS and compiled CSS receive identical flat presentation rules. Windows/IIS visual verification remains a local check.
