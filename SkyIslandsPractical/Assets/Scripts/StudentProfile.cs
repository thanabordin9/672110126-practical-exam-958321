using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SirikanyaKawilawan
{
    public static class StudentProfile
    {
        [Serializable] public sealed class Record
        {
            public string displayName = "", englishName = "", studentCode = "", section = "";
        }
        const string Prefix = "SkyIslands.Student.";
        public static string FilePath => Path.Combine(Application.persistentDataPath, "StudentIdentity.json");
        static Record Defaults => Resources.Load<StudentProfileAsset>("StudentProfile")?.data ?? new Record();
        static Record Current
        {
            get
            {
                try { if (File.Exists(FilePath)) { var saved = JsonUtility.FromJson<Record>(File.ReadAllText(FilePath)); if (IsValid(saved)) return saved; } }
                catch (IOException) { } catch (UnauthorizedAccessException) { } catch (ArgumentException) { }
                var defaults = Defaults;
                return new Record { displayName = PlayerPrefs.GetString(Prefix + "Name", defaults.displayName), englishName = PlayerPrefs.GetString(Prefix + "EnglishName", defaults.englishName), studentCode = PlayerPrefs.GetString(Prefix + "Code", defaults.studentCode), section = PlayerPrefs.GetString(Prefix + "Section", defaults.section) };
            }
        }
        public static string DisplayName => Current.displayName;
        public static string EnglishName => Current.englishName;
        public static string StudentCode => Current.studentCode;
        public static string Section => Current.section;
        public static string NamespaceName => NamespaceFor(EnglishName);
        public static string Summary => "Student Code: " + (string.IsNullOrWhiteSpace(StudentCode) ? "Not entered" : StudentCode) + "   Name: " + (string.IsNullOrWhiteSpace(DisplayName) ? "Not entered" : DisplayName) + (string.IsNullOrWhiteSpace(Section) ? "" : "   Section: " + Section);
        public static string NamespaceFor(string name)
        {
            var value = Regex.Replace(name ?? "", "[^A-Za-z0-9_]", "");
            if (!Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$")) return "";
            const string keywords = " abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while ";
            return keywords.Contains(" " + value + " ") ? "" : value;
        }
        public static bool IsValid(Record data) => data != null && NamespaceFor(data.englishName).Length > 0 && Regex.IsMatch(data.studentCode ?? "", "^[0-9]+$");
        public static bool Save(string name, string english, string code, string section)
        {
            var data = new Record { displayName = (name ?? "").Trim(), englishName = (english ?? "").Trim(), studentCode = (code ?? "").Trim(), section = (section ?? "").Trim() };
            if (!IsValid(data)) return false;
            if (data.displayName.Length == 0) data.displayName = data.englishName;
            try { Directory.CreateDirectory(Application.persistentDataPath); File.WriteAllText(FilePath, JsonUtility.ToJson(data, true)); }
            catch (IOException) { return false; } catch (UnauthorizedAccessException) { return false; }
            PlayerPrefs.SetString(Prefix + "Name", data.displayName); PlayerPrefs.SetString(Prefix + "EnglishName", data.englishName);
            PlayerPrefs.SetString(Prefix + "Code", data.studentCode); PlayerPrefs.SetString(Prefix + "Section", data.section); PlayerPrefs.Save();
            return true;
        }
    }
}
