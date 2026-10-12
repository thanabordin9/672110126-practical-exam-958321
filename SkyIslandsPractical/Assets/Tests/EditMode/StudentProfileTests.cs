using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace StudentGame.Tests
{
    public class StudentProfileTests
    {
        Type type;
        string[] keys={"SkyIslands.Student.Name","SkyIslands.Student.EnglishName","SkyIslands.Student.Code","SkyIslands.Student.Section"};
        string[] old;
        bool[] existed;
        string file,backup;
        [SetUp] public void Setup()
        {
            type=Type.GetType("StudentGame.StudentProfile, StudentGame");
            Assert.That(type,Is.Not.Null,"Student name/code storage has not been implemented.");
            old=keys.Select(k=>PlayerPrefs.GetString(k)).ToArray();existed=keys.Select(PlayerPrefs.HasKey).ToArray();
            file=(string)type.GetProperty("FilePath").GetValue(null);backup=File.Exists(file)?File.ReadAllText(file):null;
        }
        bool Save(string name,string english,string code,string section)=>(bool)type.GetMethod("Save").Invoke(null,new object[]{name,english,code,section});
        string Value(string property)=>(string)type.GetProperty(property).GetValue(null);
        [Test] public void SavedStudentDetailsPersistAndProvideEnglishNamespace()
        {
            Assert.That(Save("Example Student","Example Student","650510123","1"),Is.True);
            Assert.That(Value("DisplayName"),Is.EqualTo("Example Student"));Assert.That(Value("StudentCode"),Is.EqualTo("650510123"));
            Assert.That(Value("NamespaceName"),Is.EqualTo("ExampleStudent"));Assert.That(Value("Section"),Is.EqualTo("1"));
            Assert.That(PlayerPrefs.GetString(keys[2]),Is.EqualTo("650510123"));Assert.That(File.Exists(file),Is.True);
        }
        [Test] public void InvalidDetailsDoNotOverwriteSavedIdentity()
        {
            Save("Example Student","Example Student","650510123","1");
            Assert.That(Save("","","64xxxxxxx",""),Is.False);
            Assert.That(Value("StudentCode"),Is.EqualTo("650510123"));
            Assert.That(Save("Class","class","650510123",""),Is.False);
            Assert.That(Value("NamespaceName"),Is.EqualTo("ExampleStudent"));
        }
        [Test] public void SharedIdentityFileOverridesStalePlayerPreferences()
        {
            Save("Corrected Student","Corrected Student","650510999","2");
            PlayerPrefs.SetString(keys[0],"Old Student"); PlayerPrefs.SetString(keys[2],"650510111");
            Assert.That(Value("DisplayName"),Is.EqualTo("Corrected Student"));
            Assert.That(Value("StudentCode"),Is.EqualTo("650510999"));
        }
        [TearDown] public void Restore()
        {
            if(old==null)return;
            for(int i=0;i<keys.Length;i++){if(existed[i])PlayerPrefs.SetString(keys[i],old[i]);else PlayerPrefs.DeleteKey(keys[i]);}
            PlayerPrefs.Save();if(backup!=null)File.WriteAllText(file,backup);else if(File.Exists(file))File.Delete(file);
        }
    }
}
