# Website calculator trial and remembered inputs

Public pages, the homepage and knowledge-base access remain unchanged. A visitor can obtain one successful anonymous calculator result in the same browser, shared across Personal Chart, Predictions, Compatibility, Numerology, Biorhythms and I Ching. Further website calculator submissions redirect to the existing Account/Login page, with the owning calculator's GET page as the return URL. Free accounts can continue under their existing memberships and paywalls. No purchasing, membership grants, schema or mobile/plugin authentication changes are included.

## Trial enforcement

`CalculatorTrialAttribute` guards the six main calculator POSTs and the existing Numerology Link POST. Existing result-page links that post to these actions use the same check. GET pages remain public. Browsing a result's existing predictions/forecast/date-navigation endpoints does not consume another trial; those endpoints and mobile/plugin APIs retain their current access contracts. This is a website sign-up prompt, not a new security boundary for every calculation service or API.

The marker is recorded only for a successful, processed result: profile, compatibility, numerology, predictions, populated biorhythms, or a generated I Ching hexagram. Invalid ModelState, unprocessed models and action exceptions do not use the allowance. Authenticated users bypass the trial entirely; existing authorisation, age, complementary-credit and membership checks still run in the original actions.

`CalculatorTrialHelper` stores a boolean in the existing session plus an HttpOnly, SameSite=Lax, host-only cookie scoped to the application path, secure on HTTPS and expiring after one year. Any existing marker-cookie value means used; editing its value does not restore a trial. No birth details or results are stored in this cookie. Clearing both cookies/session or using another browser can reset an anonymous allowance; this is an intentional lightweight browser limit, not per-person tracking. MVC's ordinary session locking serialises submissions within a session; there is no new database/device/IP tracking.

Calculator GET responses now use no-store rather than their previous long output cache, so the trial notice is evaluated for each browser. Guarded submissions are also no-store. `_CalculatorTrialNotice` and the login explanation use the new Dictionary resources `CalculatorTrialNotice` and `CalculatorSignInRequired`, with generated accessors kept in sync. The existing login/register flow receives a local calculator return URL.

## Remembered form details

`Scripts/k9/calculatorMemory.js` is included through the existing k9 wildcard bundle and initialised after the standard control/data-set initialisation. Each marked form has an explicit `data-calculator-page` key; values are stored in localStorage under `NineStarKi.CalculatorInputs.v1:<page>`. Keys are personal-chart, predictions, compatibility, numerology, biorhythms and i-ching. I Ching has no entered fields to remember; its generated reading is not persisted or replayed.

Only an explicit allowlist of person fields and calculator/date/time/options is saved. Compatibility stores the two people separately. Passwords, tokens, uploads, arbitrary hidden fields, entitlements and result HTML are excluded. MVC's hidden checkbox fallback cannot overwrite its actual checkbox. Existing result-page cross-calculator POST forms explicitly opt in their allowlisted hidden person fields; a separate ISO date attribute ensures their birth date can be restored in either desktop or mobile controls without changing the original submitted value or storing anti-forgery tokens.

Values are saved on input/change, date-picker changes and submission, including a submission that is redirected to sign-in. Restoration runs only on GET forms, so it cannot overwrite POST validation messages or a newly calculated result. Random desktop date-editor IDs are resolved through the logical hidden ISO date field and its current linkid. Existing checkboxes, selects, radios, birth-time and advanced-option visibility are restored. Restoration does not submit a calculation or fire preference-writing change handlers. It does not replace either person's account birth details.

This is same-browser form memory, not account synchronisation or saved result history. It survives normal browser restarts and the sign-in/registration round trip, until localStorage/site data is cleared. On shared browser profiles these entered details are shared too. Disabled/full/malformed storage is ignored; calculations and sign-in continue, but input memory is unavailable. After returning from sign-in, users explicitly click Calculate again; there is no automatic replay or unexpected complementary-credit use.

## Validation and release

- Offline JavaScript checks: `node tools/tests/test_calculator_memory.js`. These cover page isolation, both Compatibility people, radios/checkboxes, ISO date-picker restoration, result-link forms, field allowlists, preference-handler suppression, POST preservation and disabled/malformed storage.
- xUnit: `CalculatorTrialTests` covers first/second submissions, cookie/session markers, changed cookie values, authenticated bypass, invalid/unprocessed results and exceptions. Run in the supported Windows/VS2017 environment.
- Build the website and Globalisation projects. On Integration, use a fresh anonymous browser profile to obtain one result, then submit another calculator and verify login/registration plus return to the intended calculator with the entered details. Verify an invalid first submission keeps the trial, logged-in/free users retain existing access, date/time/select controls restore correctly and all public pages/knowledge-base access remain as before. Check that JavaScript-disabled or storage-disabled browsers can still calculate/sign in, with input memory unavailable.

Source/syntax checks and the offline JavaScript harness do not substitute for Windows MVC/Razor build and browser/IIS validation. No deployment, application startup or schema execution is performed while preparing these changes.
