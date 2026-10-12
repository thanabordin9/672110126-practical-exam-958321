# Final submission verification

Student: Sirikanya Kawilawan / สิริกัญญา กาวิลาวรรณ / 672110126 / Section 001.

- Unity 6000.6.5f1 Windows.
- EditMode: 34 passed, 0 failed (canvas-editmode-final.xml), including eight saved Canvas and persistent-action checks.
- Regular PlayMode: 10 passed, 0 failed (canvas-playmode.xml); the explicit recording test is intentionally skipped in the regular suite.
- Explicit recording: 1 passed, 0 failed (canvas-recorded-evidence.xml). Actual Input System keyboard movement, real collisions, E interaction and natural timeout. No score/time injection or teleportation.
- UI is saved as Canvas prefab instances in all eight scenes. Runtime reuses them and updates values. All buttons have persistent On Click handlers; sliders have saved change handlers.
- Packaged combined Latin/Thai OFL font replaces the temporary operating-system font object. License and derivative notice included.
- Additive Options isolates menu input and keeps the menu canvas/draft alive. Stage Back and direct Stage2 retry verified.
- Actual 15,000-point Stage1 win at natural timeout, fall loss, retry reset and Stage2 E collection shown in recorded evidence.
- Windows build succeeded with zero errors; see build.txt and canvas-windows-build.log.
- Read-only reviewer found no remaining Critical/Important code blocker after the Section001 default was corrected.

## Limits

The fixed UI is designed for a 16:9 window. Video is an automated engine run, not manual student play. It does not show E collection of every item type in Stage2; source prefabs/rule tests supplement the recording. Public repository uses the connected account thanabordin9. Genuine work history begins 12 October 2026 Bangkok time, so daily commit/push coverage for earlier exam dates is not claimed.
