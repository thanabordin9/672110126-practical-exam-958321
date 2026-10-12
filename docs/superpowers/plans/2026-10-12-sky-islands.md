# Sky Islands Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Goal:** Build a playable Unity game covering all twelve practical criteria.

**Architecture:** A shared run model handles the rules in both stages. Separate navigation, item, input, HUD and audio components connect real Unity scenes. An editor builder creates repeatable scenes and prefabs.

**Tech Stack:** Unity 6, C#, ProBuilder, official Starter Assets Third Person Controller, Unity UI, supplied FBX/WAV assets.

**Spec:** ../specs/2026-10-12-sky-islands-design.md

## Global Constraints
- Separate project: SkyIslandsPractical; preserve existing projects.
- Start at 30 seconds and score zero; effects +3/-5 seconds and +500/-500 points.
- Timeout wins only for score strictly greater than 10000; falling always loses.
- Stage2 uses interaction instead of collision collection and has at least 50 square metres.
- Temporary namespace StudentGame requires the user's English name before identity compliance.
- No fabricated Git history, video URLs, identity or test results.

## Review Focus
- Retry and repeated scene loads must not duplicate music or retain consumed items.
- Exact score 10000 must lose; 10500 must win at timeout.
- Harmful time items can end the run; further collection must be ignored.
- Stage2 contact alone must not collect an item; each E press collects at most one.
- Persisted muted music must remain muted after returning from Options.

## Files and responsibilities
All implementation paths below are relative to SkyIslandsPractical.
- Packages/manifest.json: pinned Unity dependencies.
- Assets/Scripts/RunState.cs: deterministic rules and terminal state.
- Assets/Scripts/RunController.cs: clock, HUD events and result transitions.
- Assets/Scripts/SceneNavigation.cs: scene routes and last stage.
- Assets/Scripts/ItemEffect.cs: type, effect and single-use guard.
- Assets/Scripts/CapsuleMotor.cs: capsule movement and fall detection.
- Assets/Scripts/PlayerInteraction.cs: Stage2 focus and E input.
- Assets/Scripts/AudioService.cs: music, effects and persisted volumes.
- Assets/Scripts/GameHud.cs: score, timer and interaction prompt.
- Assets/Editor/ProjectBuilder.cs: assets, rig setup, prefabs, ProBuilder meshes and scenes.
- Assets/Tests/EditMode/RunStateTests.cs: deterministic rule verification.
- Assets/Tests/PlayMode/GameplayTests.cs: lifecycle and collection verification.
- README.md and EvidenceChecklist.md: opening, controls, verification and rubric evidence.

### Task 1: Shared rules and project foundation
**Produces:** RunState.Score (int), Remaining (float), Outcome (RunOutcome); Tick(float), Apply(ItemKind), Fall(), Reset(). ItemKind has TimeBonus, TimePenalty, ScoreBonus, ScorePenalty. RunOutcome has Playing, Won, Lost.
- [ ] Create Unity project configuration and EditMode tests for start state, all four effects, score boundaries, fall, terminal guard and reset.
- [ ] Run Unity tests and confirm failure from the missing rule class.
- [ ] Implement RunState and enums; clamp time at zero and evaluate timeout after effects.
- [ ] Run tests and require all rule cases to pass; commit project foundation.

### Task 2: Navigation, UI and audio
**Consumes:** Task1 run model. **Produces:** SceneNavigation.Load(string), StartStage(int), Retry(), Finish(RunOutcome,int); AudioService.PlayEffect(AudioClip), SetMusicVolume(float), SetEffectsVolume(float).
- [ ] Add lifecycle checks for repeated navigation, retry state, one audio singleton and saved mute values.
- [ ] Implement routing across MainMenu, StageSelection, Options, Credits, Stage1, Stage2, Win and GameOver. Restore normal time scale and cursor mode during transitions.
- [ ] Implement persistent music/effect sources, saved independent sliders and scene music selection.
- [ ] Build visible UI labels, result score, retry/back buttons and upper-left countdown; verify navigation and mute persistence; commit.

### Task 3: Capsule gameplay and level assets
**Consumes:** RunState.Apply and navigation/audio interfaces. **Produces:** ItemEffect.TryCollect(GameObject) returning bool; RunController.ActiveState and EndFall(); CapsuleMotor movement.
- [ ] Add checks for one-time collision collection, penalty timeout and no collection after terminal state.
- [ ] Import selected supplied audio files; create four distinct item prefabs and an obstacle prefab.
- [ ] Implement capsule movement, gravity, grounded jump, camera and fall detection.
- [ ] Create Stage1 using actual ProBuilder meshes, narrow/wide bridges and at least 24 positive score items plus time bonuses and harmful items.
- [ ] Verify route feasibility and correct item/contact sounds in Play Mode; commit.

### Task 4: Humanoid interaction stage
**Consumes:** Shared items, rules and audio. **Produces:** PlayerInteraction focus selection and explicit E collection.
- [ ] Obtain official Starter Assets and configure Input System/Cinemachine dependencies for the installed Unity version. Report an unavailable package rather than substitute an unrelated controller.
- [ ] Import supplied character and animation FBX as Humanoid; validate Avatar and Animator before scene creation.
- [ ] Add PlayMode checks: overlap alone leaves item active, E consumes one focused item, repeat E cannot consume it again.
- [ ] Implement nearby focus highlighting and prompt; create Stage2 with 12 by 12 metre ProBuilder playable area and distributed items.
- [ ] Verify locomotion, camera, interaction and obstacle sound cooldown; commit.

### Task 5: Integration and delivery
**Consumes:** All game components and scene builder. **Produces:** Windows build when supported, Unity source project and truthful evidence checklist.
- [ ] Generate all scenes, save prefab references and register the build scene list.
- [ ] Run full Unity EditMode and PlayMode tests; resolve errors and re-run affected checks.
- [ ] Build Windows desktop if installed modules permit; play-test each scene, both outcomes, retry and audio sliders.
- [ ] Write README with exact Unity version, controls, opening steps and any remaining limitations. Map all twelve criteria to actual objects/scripts and suggested video evidence without invented URLs.
- [ ] Package source excluding Library/Temp/Logs; report verified outputs and unresolved requirements.

## Self-review
All specification sections map to Tasks1-5. Tests cover each Review Focus case. The required scene routes, exact item values, ProBuilder components, official humanoid controller, namespace limitation and real evidence are preserved. Execute natively in this chat; independent agents are optional only if the user chooses them.
