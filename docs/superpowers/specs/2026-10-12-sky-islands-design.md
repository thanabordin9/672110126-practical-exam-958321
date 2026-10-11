# Sky Islands practical game design

## Goal and scope
Create a separate Unity 6 project named SkyIslandsPractical covering the user's twelve practical-exam criteria. Keep existing TheForest and sites projects intact. Use the provided FBX character, animation and WAV resources. Target Windows desktop keyboard and mouse. Scores are not guaranteed; report actual verification and unresolved requirements.

## Scenes and navigation
Create real Unity scenes with visible UI scene labels: MainMenu, StageSelection, Options, Credits, Stage1, Stage2, Win and GameOver. MainMenu Start opens StageSelection; Options opens Options and OK returns; Exit closes the build. StageSelection chooses either stage and Back returns to MainMenu. Both stages provide Back to MainMenu. Win returns to MainMenu. GameOver supports Try Again for the failed stage and Back to MainMenu. Credits returns to MainMenu. Provide a Credits button on MainMenu so the credits scene is reachable. Persist the last played stage and final score separately from active run state.

## Architecture
Scene navigation controls transitions and resets run state. A shared run model owns score, remaining time and terminal state. Item components describe exactly one effect. Stage1 collision collection and Stage2 interaction collection use the same effect model. HUD subscribes to state updates. A persistent audio service owns separate music and effect sources and volume settings. Character controllers remain independent of score and time logic.

## Stage 1
Use a visible capsule, CharacterController, camera-relative movement, gravity and grounded jumping. Build floating islands and narrow/wide bridges using ProBuilder meshes. Use prefab instances for collectibles and obstacles. Place at least 24 positive score items, enough for more than 10000 points, with a route that can be completed within available time. Falling below the level ends the run. Distinct colours, geometry and labels distinguish all four item types.

## Stage 2
Use the official Starter Assets Third Person Controller with a supplied character imported as Humanoid and an appropriate Animator Avatar. Use ProBuilder to create at least 50 square metres of playable space; target a 12 by 12 metre arena with additional paths. A nearby item is highlighted and shows an E prompt. E collects the focused item once. Walking into an item alone does not collect it. Physical obstacles provide contact sounds with a cooldown.

## Rules shared by both stages
Start with 30 seconds and score zero. Time items add 3 or subtract 5 seconds. Score items add or subtract 500 points. Countdown appears at upper left and score is visible. End the run on time expiry, then win if score is strictly greater than 10000; otherwise lose. Exactly 10000 loses because the winning wording specifies greater than 10000. Reaching the threshold early does not stop the timer because the instructions also say the game ends when time expires. A fall always loses. Clamp time to zero; allow negative score. Resolve item effects and terminal state consistently; no collection after the run ends. Retry resets all items, time and score.

## Audio and presentation
Play a looping supplied music track in every scene without duplicate persistent services. Options has independent music and SFX sliders and saved settings. Beneficial and harmful items have distinct supplied sounds; Stage2 interaction also produces collection audio. Obstacle contact produces a separate sound without every-frame repetition. Use readable English UI text, warm island surfaces, cyan collectibles, amber time bonuses and visibly harmful red/purple objects.

## Identity and delivery
Student English name is not supplied. Use temporary namespace StudentGame and document a required namespace replacement before submission; do not claim identity compliance until the name is provided. Do not invent student identity, Git URL, video URLs, evidence or earlier commits. Deliver the Unity source project, instructions and a rubric evidence checklist. A public remote repository requires an actual available account and destination; local source preparation is independent of that step.

## Verification
Compile in installed Unity. Verify scene list and navigation, both character controllers, correct ProBuilder components, prefab references, all four item effects, boundary scores 10000 and 10500, fall and timeout outcomes, retry reset, interaction-only collection, audio in each scene and independent volume persistence. Test rules using automated tests where useful and play-test movement and UI. Report unavailable Starter Assets, rig failures, build failures or untested behaviour explicitly instead of silently substituting another controller.

## Alternatives considered
Recommended: a separate Unity project, preserving existing work and allowing consistent scene setup. Extending TheForest risks inheriting unrelated behaviour. A browser game cannot demonstrate required Unity components such as ProBuilder and Starter Assets.
