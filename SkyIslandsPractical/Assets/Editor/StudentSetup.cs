using System.IO;
using UnityEngine;
using UnityEditor;

namespace StudentGame.Editor
{
    public sealed class StudentSetup : EditorWindow
    {
        [SerializeField] StudentProfile.Record data = new StudentProfile.Record();
        [SerializeField] string status = "";
        [MenuItem("Sky Islands/Student Details and Namespace")]
        public static void Open()
        {
            var window = GetWindow<StudentSetup>("Student Details");
            var asset = EnsureAsset(); window.data = JsonUtility.FromJson<StudentProfile.Record>(JsonUtility.ToJson(asset.data));
            if (File.Exists(StudentProfile.FilePath))
            {
                var saved = JsonUtility.FromJson<StudentProfile.Record>(File.ReadAllText(StudentProfile.FilePath));
                if (StudentProfile.IsValid(saved)) window.data = saved;
            }
        }
        public static StudentProfileAsset EnsureAsset()
        {
            const string path = "Assets/Resources/StudentProfile.asset";
            var asset = AssetDatabase.LoadAssetAtPath<StudentProfileAsset>(path);
            if (asset == null) { Directory.CreateDirectory("Assets/Resources"); AssetDatabase.Refresh(); asset = CreateInstance<StudentProfileAsset>(); AssetDatabase.CreateAsset(asset, path); }
            return asset;
        }
        void OnGUI()
        {
            EditorGUILayout.LabelField("Student information for this project", EditorStyles.boldLabel);
            data.displayName = EditorGUILayout.TextField("Full name", data.displayName);
            data.englishName = EditorGUILayout.TextField("English name", data.englishName);
            data.studentCode = EditorGUILayout.TextField("Student code", data.studentCode);
            data.section = EditorGUILayout.TextField("Section", data.section);
            EditorGUILayout.LabelField("Namespace", StudentProfile.NamespaceFor(data.englishName));
            if (GUILayout.Button("Save details and apply namespace")) Apply();
            EditorGUILayout.HelpBox(status.Length == 0 ? "You can also import details saved in the game by reopening this window. Build again after Unity finishes compiling." : status, MessageType.Info);
        }
        void Apply()
        {
            data.displayName = (data.displayName ?? "").Trim(); data.englishName = (data.englishName ?? "").Trim();
            data.studentCode = (data.studentCode ?? "").Trim(); data.section = (data.section ?? "").Trim();
            if (!StudentProfile.IsValid(data)) { status = "Enter an English name and a numeric student code."; return; }
            if (string.IsNullOrWhiteSpace(data.displayName)) data.displayName = data.englishName;
            if (!StudentProfile.Save(data.displayName, data.englishName, data.studentCode, data.section)) { status = "Cannot save the student identity file. Check folder access and try again."; return; }
            var asset = EnsureAsset(); asset.data = data; EditorUtility.SetDirty(asset); AssetDatabase.SaveAssets();
            string current = typeof(RunState).Namespace, next = StudentProfile.NamespaceFor(data.englishName);
            if (current != next)
            {
                foreach (var root in new[] { "Assets/Scripts", "Assets/Editor", "Assets/Tests" })
                foreach (var path in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
                {
                    string source = File.ReadAllText(path);
                    source = source.Replace("namespace " + current, "namespace " + next).Replace("using " + current + ";", "using " + next + ";").Replace("using " + current + ".", "using " + next + ".").Replace("\"" + current + ".", "\"" + next + ".");
                    File.WriteAllText(path, source);
                }
            }
            status = "Saved. Source namespace: " + next + ". Wait for compilation, then build the game again.";
            AssetDatabase.Refresh();
        }
    }
}
