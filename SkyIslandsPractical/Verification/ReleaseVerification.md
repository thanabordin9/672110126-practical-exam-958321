# Release verification

- Unity Editor: 6000.6.5f1, Windows.
- EditMode: 23 passed, 0 failed (editmode-final.xml).
- PlayMode after final HUD update: 6 passed, 0 failed (playmode-release.xml).
- Actual virtual keyboard input exercised Capsule and Starter Assets movement and Stage2 E collection. Tests allow device input in the hidden Editor only and restore Input Settings afterward.
- Scene UI navigation, independent volume settings, persistence, item single-use and retry reset verified.
- Ch02 texture mapping regression: failed with missing body/hair diffuse textures, then passed after explicit 1001/1002 mapping.
- Rendered screenshots inspected for MainMenu and both gameplay stages. Screenshot capture uses Camera/RenderTexture because batch-mode Editor has no Game View target.
- Fresh read-only review found no Critical/Important bug.

## Deferred minor findings

- Opening Stage2 directly in the Unity Editor (bypassing MainMenu/StageSelection) retains default LastStage=1 for results/retry. Open MainMenu for normal play.
- Fixed 1600x900 UI coordinates favour 16:9 windows; narrower aspect ratios may clip content. Use the default window or a 16:9 resolution.

## Decisions made

- Used PowerShell/Python bookkeeping instead of Bash-only skill scripts on Windows. Same verification evidence retained; risk is tool-script interoperability rather than game behaviour.
- Updated ProBuilder/Input System dependencies for the installed Unity Editor; older versions failed on removed EntityId APIs. Cinemachine resolves to the Editor's builtin 6.6.0 package. Exact Unity version is required for reproducible imports.

## Review limits

- Physical speaker output quality depends on the user's audio device; audio source, clip and volume wiring is implemented and tested at software level.
- Full manual Stage2 win playthrough has not been recorded; Humanoid movement, E collection and shared win/loss boundary rules are verified separately.
- Windows build succeeded with 0 errors (139,034,451 bytes). The compiled executable initialized its main scene and remained running for 20 seconds in a headless smoke test with no exception/error log; actual scene graphics were inspected through PlayMode renders. See build.txt and player-startup-final.log.
- Final appearance checked by the author through rendered screenshots, separately from the static reviewer.

## Submission data still needed

Student English namespace/name/code, public repository URL, recorded video evidence and real Git history screenshot. No earlier work history or video URLs are fabricated.
