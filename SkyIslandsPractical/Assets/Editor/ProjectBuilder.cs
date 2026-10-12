using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine.ProBuilder;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using StarterAssets;
using Object = UnityEngine.Object;

namespace SirikanyaKawilawan.Editor
{
    public static class ProjectBuilder
    {
        static readonly string[] Scenes = { "MainMenu", "StageSelection", "Options", "Credits", "Stage1", "Stage2", "Win", "GameOver" };
        static Material grass, rock, wood, cyan, gold, red, purple, cloud, dark, characterBody, characterHair;
        static GameObject[] itemPrefabs;
        static GameObject obstaclePrefab;
        static int meshId;
        static string currentScene;
        const string Model = "Assets/ExamResources/Character Models/Ch02_nonPBR.fbx";
        const string Animations = "Assets/StarterAssets/ThirdPersonController/Character/Animations/";
        static AudioClip Audio(string path) => AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ExamResources/Sounds/" + path);

        [MenuItem("Sky Islands/Generate Game Scenes")]
        public static void Generate()
        {
            foreach (var folder in new[] { "Assets/Scenes", "Assets/Prefabs", "Assets/Art", "Assets/Art/Meshes", "Assets/Art/Textures", "Assets/Art/CharacterMaterials" }) Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
            ConfigureModels(); CreateMaterials(); CreatePrefabs();
            foreach (var scene in Scenes)
            {
                currentScene = scene;
                meshId = 0;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                SetEnvironment();
                var audio = new GameObject("Music and SFX Service").AddComponent<AudioService>(); audio.musicClip = Audio("BGM/Tiny_Blocks.wav");
                if (scene == "Stage1" || scene == "Stage2") BuildStage(scene == "Stage2" ? 2 : 1);
                else { BuildBackdrop(); new GameObject("Scene UI").AddComponent<SceneScreen>(); }
                EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/" + scene + ".unity");
            }
            EditorBuildSettings.scenes = Scenes.Select(s => new EditorBuildSettingsScene("Assets/Scenes/" + s + ".unity", true)).ToArray();
            PlayerSettings.companyName = "StudentGame"; PlayerSettings.productName = "Sky Islands";
            PlayerSettings.defaultScreenWidth = 1600; PlayerSettings.defaultScreenHeight = 900; PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = true;
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            File.WriteAllText("Verification/generation.txt", "Generated eight scenes with ProBuilder geometry, item prefabs and Starter Assets humanoid.\n");
            Debug.Log("SKY ISLANDS: all scenes generated successfully.");
        }

        static void ConfigureModels()
        {
            var paths = Directory.GetFiles("Assets/ExamResources", "*.fbx", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Animations, "*.fbx")).Select(p => p.Replace('\\', '/'));
            foreach (var path in paths)
            {
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel; importer.sourceAvatar = null;
                importer.importAnimation = true;
                var clips = importer.defaultClipAnimations;
                foreach (var clip in clips)
                {
                    bool jump = path.Contains("Jump--Jump"); clip.loopTime = !jump;
                    clip.lockRootPositionXZ = true; clip.lockRootRotation = true;
                    clip.events = Array.Empty<AnimationEvent>();
                }
                if (clips.Length > 0) importer.clipAnimations = clips;
                importer.SaveAndReimport();
            }
            var modelImporter = (ModelImporter)AssetImporter.GetAtPath(Model);
            modelImporter.ExtractTextures("Assets/Art/Textures"); modelImporter.SaveAndReimport();
            AssetDatabase.Refresh();
            foreach (var path in Directory.GetFiles("Assets/Art/Textures", "*Normal.png"))
            {
                var textureImporter = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
                if (textureImporter.textureType != TextureImporterType.NormalMap) { textureImporter.textureType = TextureImporterType.NormalMap; textureImporter.SaveAndReimport(); }
            }
            var avatar = AssetDatabase.LoadAllAssetsAtPath(Model).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman) throw new InvalidOperationException("Provided Ch02 character did not import as a valid Humanoid.");
        }

        static Material Mat(string name, Color color, float glow = 0)
        {
            string path = "Assets/Art/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) { material = new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(material, path); }
            material.color = color; material.SetFloat("_Glossiness", 0.22f);
            if (glow > 0) { material.EnableKeyword("_EMISSION"); material.SetColor("_EmissionColor", color * glow); }
            return material;
        }
        static void CreateMaterials()
        {
            grass = Mat("Island Jade", new Color(0.20f, 0.52f, 0.42f)); rock = Mat("Rock", new Color(0.26f, 0.35f, 0.38f));
            wood = Mat("Bridge Sandstone", new Color(0.73f, 0.59f, 0.38f)); cyan = Mat("Score Cyan", new Color(0.15f, 0.94f, 0.83f), 0.45f);
            gold = Mat("Time Gold", new Color(1, 0.72f, 0.18f), 0.35f); red = Mat("Score Red", new Color(0.9f, 0.21f, 0.25f), 0.2f);
            purple = Mat("Time Purple", new Color(0.66f, 0.3f, 0.89f), 0.25f); cloud = Mat("Cloud", new Color(0.90f, 0.96f, 0.98f)); dark = Mat("Obstacle", new Color(0.19f, 0.26f, 0.32f));
            characterBody = CharacterMaterial("Ch02 Body", "1001", false); characterHair = CharacterMaterial("Ch02 Hair", "1002", true);
        }
        static Material CharacterMaterial(string name, string tile, bool cutout)
        {
            var material = Mat(name, Color.white);
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Textures/Ch02_" + tile + "_Diffuse.png");
            material.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Textures/Ch02_" + tile + "_Normal.png")); material.EnableKeyword("_NORMALMAP");
            if (material.mainTexture == null) throw new InvalidOperationException("Missing extracted character diffuse texture " + tile);
            if (cutout) { material.SetFloat("_Mode", 1); material.SetFloat("_Cutoff", 0.35f); material.EnableKeyword("_ALPHATEST_ON"); material.SetOverrideTag("RenderType", "TransparentCutout"); material.renderQueue = 2450; }
            return material;
        }
        static GameObject Primitive(string name, PrimitiveType type, Vector3 pos, Vector3 size, Material mat, bool collision = false)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.position = pos; go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collision) Object.DestroyImmediate(go.GetComponent<Collider>()); return go;
        }
        static void CreatePrefabs()
        {
            itemPrefabs = new GameObject[4];
            var kinds = new[] { ItemKind.TimeBonus, ItemKind.TimePenalty, ItemKind.ScoreBonus, ItemKind.ScorePenalty };
            var materials = new[] { gold, purple, cyan, red };
            var names = new[] { "TimePlus3", "TimeMinus5", "ScorePlus500", "ScoreMinus500" };
            for (int i = 0; i < 4; i++)
            {
                var go = new GameObject(names[i]); var effect = go.AddComponent<ItemEffect>(); effect.kind = kinds[i];
                var collider = go.AddComponent<SphereCollider>(); collider.radius = 0.65f; collider.isTrigger = true;
                var rigidbody = go.AddComponent<Rigidbody>(); rigidbody.isKinematic = true; rigidbody.useGravity = false;
                var visual = Primitive("Collectible Visual", i < 2 ? PrimitiveType.Sphere : PrimitiveType.Cube, Vector3.zero, Vector3.one * 0.5f, materials[i]);
                visual.transform.SetParent(go.transform, false); if (i >= 2) visual.transform.localRotation = Quaternion.Euler(30, 0, 30);
                effect.visual = visual.transform;
                effect.pickupClip = Audio(i == 0 ? "SFX/Buff_Power Up 1.wav" : i == 2 ? "SFX/Coin Pickup 4.wav" : "SFX/Debuff Downgrade 1.wav");
                var label = new GameObject("Item Label").AddComponent<TextMesh>(); label.transform.SetParent(go.transform, false); label.transform.localPosition = new Vector3(0, 0.58f, 0);
                label.text = effect.Label; label.fontSize = 64; label.characterSize = 0.045f; label.anchor = TextAnchor.MiddleCenter; label.color = Color.white;
                label.gameObject.AddComponent<Billboard>();
                itemPrefabs[i] = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/" + names[i] + ".prefab"); Object.DestroyImmediate(go);
            }
            var obstacle = Primitive("RockObstacle", PrimitiveType.Cube, Vector3.zero, new Vector3(1.1f, 1.6f, 1.1f), dark, true);
            obstacle.AddComponent<Obstacle>(); obstaclePrefab = PrefabUtility.SaveAsPrefabAsset(obstacle, "Assets/Prefabs/RockObstacle.prefab"); Object.DestroyImmediate(obstacle);
        }
        static GameObject Box(string name, Vector3 center, Vector3 size, Material mat)
        {
            var pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size); pb.name = name; pb.transform.position = center; pb.GetComponent<Renderer>().sharedMaterial = mat;
            pb.ToMesh(); pb.Refresh();
            string path = "Assets/Art/Meshes/" + currentScene + "_" + meshId++ + ".asset";
            if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null) AssetDatabase.DeleteAsset(path);
            var mesh = pb.GetComponent<MeshFilter>().sharedMesh;
            AssetDatabase.CreateAsset(mesh, path);
            if (pb.GetComponent<Collider>() == null) pb.gameObject.AddComponent<BoxCollider>();
            pb.gameObject.layer = 6; return pb.gameObject;
        }
        static void SetEnvironment()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = new Color(0.66f, 0.77f, 0.85f);
            RenderSettings.fog = true; RenderSettings.fogColor = new Color(0.62f, 0.81f, 0.90f); RenderSettings.fogMode = FogMode.Linear; RenderSettings.fogStartDistance = 75; RenderSettings.fogEndDistance = 160;
            var sky = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Sky.mat");
            if (sky == null) { sky = new Material(Shader.Find("Skybox/Procedural")); AssetDatabase.CreateAsset(sky, "Assets/Art/Sky.mat"); }
            sky.SetColor("_SkyTint", new Color(0.53f, 0.78f, 0.88f)); sky.SetColor("_GroundColor", new Color(0.56f, 0.76f, 0.85f)); sky.SetFloat("_AtmosphereThickness", 0.55f); RenderSettings.skybox = sky;
            var sun = new GameObject("Sun").AddComponent<Light>(); sun.type = LightType.Directional; sun.intensity = 1.3f; sun.color = new Color(1, 0.92f, 0.80f); sun.shadows = LightShadows.Soft; sun.transform.rotation = Quaternion.Euler(40, -35, 0); RenderSettings.sun = sun;
            for (int i = 0; i < 14; i++)
            {
                float x = (i % 2 == 0 ? -1 : 1) * (13 + i % 3 * 5);
                for (int j = 0; j < 3; j++) Primitive("Cloud", PrimitiveType.Sphere, new Vector3(x + j * 2.4f, -5 - i % 3, i * 7), new Vector3(6, 2.8f, 4.8f), cloud);
            }
        }
        static Camera CameraAt(Vector3 pos, Vector3 target)
        {
            var go = new GameObject("Main Camera"); go.tag = "MainCamera"; var camera = go.AddComponent<Camera>(); go.AddComponent<AudioListener>();
            camera.transform.position = pos; camera.transform.LookAt(target); camera.fieldOfView = 52; camera.farClipPlane = 200; return camera;
        }
        static void BuildBackdrop()
        {
            Box("Menu Island", new Vector3(5, -0.35f, 10), new Vector3(10, 0.7f, 10), grass);
            Box("Menu Island Rock", new Vector3(5, -1.15f, 10), new Vector3(8.8f, 1.2f, 8.8f), rock);
            Box("Menu Bridge", new Vector3(5, -0.1f, 20), new Vector3(2.4f, 0.35f, 10), wood);
            Box("Far Island", new Vector3(5, -0.35f, 30), new Vector3(8, 0.7f, 8), grass);
            for (int i = 0; i < 9; i++) Primitive("Glowing Collectible", PrimitiveType.Cube, new Vector3(5, 1.0f, 7 + i * 2.5f), Vector3.one * 0.6f, i % 3 == 0 ? gold : cyan);
            for (int i = 0; i < 4; i++) Tree(new Vector3(i % 2 == 0 ? 1.8f : 8.2f, 0, 7 + i * 1.8f));
            CameraAt(new Vector3(-12, 10, -6), new Vector3(5, 0.7f, 14));
        }
        static void Tree(Vector3 pos)
        {
            Primitive("Tree Trunk", PrimitiveType.Cylinder, pos + Vector3.up * 0.7f, new Vector3(0.35f, 0.7f, 0.35f), wood);
            Primitive("Tree Crown", PrimitiveType.Sphere, pos + Vector3.up * 2.1f, new Vector3(1.6f, 2.2f, 1.6f), grass);
        }
        static void BuildStage(int stage)
        {
            var run = new GameObject("Time Attack Rules").AddComponent<RunController>(); run.stage = stage; run.winClip = Audio("SFX/Win sound 2.wav"); run.loseClip = Audio("SFX/Game Over 2.wav");
            if (stage == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    Box("Island " + (i + 1), new Vector3(0, -0.35f, i * 12), new Vector3(i % 2 == 0 ? 8 : 6, 0.7f, 8), grass);
                    Box("Island Rock " + (i + 1), new Vector3(0, -1.1f, i * 12), new Vector3(i % 2 == 0 ? 7.3f : 5.3f, 1.1f, 7.3f), rock);
                    if (i < 6) Box("Narrow Bridge " + i, new Vector3(0, -0.17f, i * 12 + 6), new Vector3(i % 2 == 0 ? 2.2f : 3.8f, 0.35f, 4.05f), wood);
                    Tree(new Vector3(i % 2 == 0 ? -2.8f : 2.1f, 0, i * 12 + 2.3f));
                    if (i > 0) PlaceItem(0, new Vector3(0, 1, i * 12 - 1.2f), run, stage);
                }
                for (int i = 0; i < 32; i++) PlaceItem(2, new Vector3(0, 1, 0.8f + i * 2.2f), run, stage);
                for (int i = 1; i < 6; i++) { PlaceItem(1, new Vector3(2, 1, i * 12 + 1), run, stage); PlaceItem(3, new Vector3(-2, 1, i * 12), run, stage); PlaceObstacle(new Vector3(i % 2 == 0 ? 1.4f : -1.4f, 0.8f, i * 12 + 2.7f)); }
            }
            else
            {
                Box("Arena 12 x 12 metres", new Vector3(0, -0.35f, 0), new Vector3(12, 0.7f, 12), grass);
                Box("Arena Rock Base", new Vector3(0, -1.1f, 0), new Vector3(11.3f, 1.1f, 11.3f), rock);
                Box("Explorer Bridge", new Vector3(0, -0.17f, 8), new Vector3(3, 0.35f, 4.05f), wood);
                Box("Bonus Island", new Vector3(0, -0.35f, 13), new Vector3(8, 0.7f, 6), grass);
                for (int row = 0; row < 5; row++) for (int col = 0; col < 5; col++) PlaceItem(2, new Vector3(-4 + col * 2, 1, -4 + row * 2), run, stage);
                for (int i = 0; i < 4; i++) PlaceItem(0, new Vector3(-5.1f + i * 3.4f, 1, 5.2f), run, stage);
                PlaceItem(1, new Vector3(-2, 1, 12), run, stage); PlaceItem(3, new Vector3(2, 1, 12), run, stage);
                for (int i = 0; i < 4; i++) PlaceItem(2, new Vector3(-2.5f + i * 1.7f, 1, 14), run, stage);
                PlaceObstacle(new Vector3(-1, 0.8f, 1)); PlaceObstacle(new Vector3(3, 0.8f, -1));
                Tree(new Vector3(-5.2f, 0, 2.3f)); Tree(new Vector3(5.2f, 0, -3));
            }
            var camera = CameraAt(new Vector3(0, 8, -10), new Vector3(0, 1, 0));
            var player = new GameObject("Player"); player.tag = "Player"; player.layer = 7;
            player.transform.position = stage == 1 ? new Vector3(0, 0.1f, -2.3f) : new Vector3(-4, 0.1f, -4.8f);
            var controller = player.AddComponent<CharacterController>(); controller.height = 1.8f; controller.radius = 0.32f; controller.center = new Vector3(0, 0.9f, 0); controller.stepOffset = 0.3f;
            run.player = player.transform;
            var target = new GameObject("Camera Target").transform; target.SetParent(player.transform, false); target.localPosition = new Vector3(0, 1.4f, 0);
            var contact = player.AddComponent<PlayerContact>(); contact.obstacleClip = Audio("SFX/Bounce_Jump 3.wav");
            PlayerInteraction interaction = null;
            if (stage == 1)
            {
                var visual = Primitive("Capsule Body", PrimitiveType.Capsule, Vector3.zero, new Vector3(0.65f, 0.9f, 0.65f), cyan);
                visual.transform.SetParent(player.transform, false); visual.transform.localPosition = Vector3.up * 0.9f;
                var face = Primitive("Forward Marker", PrimitiveType.Cube, Vector3.zero, new Vector3(0.25f, 0.16f, 0.08f), dark); face.transform.SetParent(player.transform, false); face.transform.localPosition = new Vector3(0, 1.45f, 0.32f);
                var motor = player.AddComponent<CapsuleMotor>(); motor.run = run; motor.cameraTransform = camera.transform;
            }
            else
            {
                var model = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Model)); model.transform.SetParent(player.transform, false);
                foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>()) renderer.sharedMaterials = renderer.sharedMaterials.Select(m => m != null && m.name.ToLowerInvariant().Contains("hair") ? characterHair : characterBody).ToArray();
                var importedAnimator = model.GetComponent<Animator>(); if (importedAnimator != null) Object.DestroyImmediate(importedAnimator);
                var animator = player.AddComponent<Animator>(); animator.avatar = AssetDatabase.LoadAllAssetsAtPath(Model).OfType<Avatar>().First();
                animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Animations + "StarterAssetsThirdPerson.controller"); animator.applyRootMotion = false;
                var input = player.AddComponent<StarterAssetsInputs>(); input.cursorLocked = false; input.cursorInputForLook = false;
                var playerInput = player.AddComponent<PlayerInput>(); playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/StarterAssets/InputSystem/StarterAssets.inputactions");
                playerInput.defaultActionMap = "Player"; playerInput.defaultControlScheme = "KeyboardMouse"; playerInput.notificationBehavior = PlayerNotifications.SendMessages;
                var motor = player.AddComponent<ThirdPersonController>(); motor.MoveSpeed = 5; motor.SprintSpeed = 8.5f; motor.GroundLayers = 1 << 6; motor.CinemachineCameraTarget = target.gameObject; motor.LockCameraPosition = true;
                motor.FootstepAudioClips = Array.Empty<AudioClip>();
                var gate = player.AddComponent<StarterRunGate>(); gate.run = run; gate.motor = motor;
                interaction = player.AddComponent<PlayerInteraction>(); interaction.run = run;
            }
            camera.gameObject.AddComponent<CinemachineBrain>();
            var cine = new GameObject("Player Follow Camera").AddComponent<CinemachineCamera>(); cine.Follow = target; cine.LookAt = target; cine.Lens.FieldOfView = 52;
            var follow = cine.gameObject.AddComponent<CinemachineFollow>(); follow.FollowOffset = new Vector3(0, 7, -9.5f); follow.TrackerSettings.BindingMode = BindingMode.WorldSpace;
            cine.gameObject.AddComponent<CinemachineHardLookAt>();
            var hud = new GameObject("HUD").AddComponent<GameHud>(); hud.run = run; hud.interaction = interaction;
        }
        static void PlaceItem(int type, Vector3 position, RunController run, int stage)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(itemPrefabs[type]); go.transform.position = position;
            var item = go.GetComponent<ItemEffect>(); item.run = run; item.interactionOnly = stage == 2;
        }
        static void PlaceObstacle(Vector3 position) { var go = (GameObject)PrefabUtility.InstantiatePrefab(obstaclePrefab); go.transform.position = position; }

        [MenuItem("Sky Islands/Build Windows Game")]
        public static void BuildWindows()
        {
            StudentSetup.EnsureAsset(); AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Builds/Windows");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(), locationPathName = "Builds/Windows/SkyIslands.exe", target = BuildTarget.StandaloneWindows64, options = BuildOptions.None });
            File.WriteAllText("Verification/build.txt", report.summary.result + " / errors: " + report.summary.totalErrors + " / size: " + report.summary.totalSize);
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Windows build failed: " + report.summary.result);
        }
    }
}
