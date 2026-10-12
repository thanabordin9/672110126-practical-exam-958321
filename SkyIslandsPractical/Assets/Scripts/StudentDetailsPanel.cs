using System;
using UnityEngine;
using UnityEngine.UI;

namespace StudentGame
{
    public static class StudentDetailsPanel
    {
        static Font studentFont;
        public static Font StudentFont => studentFont == null ? studentFont = Font.CreateDynamicFontFromOSFont(new[] { "Tahoma", "Arial" }, 24) : studentFont;
        public static Text IdentityText(Transform parent, string name, string value, float x, float y, float width, float height, int size)
        {
            var text = UiKit.Text(parent, name, value, x, y, width, height, size, UiKit.Muted); text.font = StudentFont; text.supportRichText = false; return text;
        }
        static InputField Field(Transform parent, string name, string placeholder, string value, float x, float y, float width, bool numeric = false)
        {
            var background = UiKit.Panel(parent, name, x, y, width, 44, new Color(0.14f, 0.23f, 0.29f));
            var input = background.gameObject.AddComponent<InputField>(); input.targetGraphic = background;
            var label = IdentityText(background.transform, "Text", "", 12, 0, width - 24, 44, 20); label.color = Color.white;
            var hint = IdentityText(background.transform, "Placeholder", placeholder, 12, 0, width - 24, 44, 18);
            input.textComponent = label; input.placeholder = hint; input.characterLimit = numeric ? 16 : 64;
            input.characterValidation = numeric ? InputField.CharacterValidation.Integer : InputField.CharacterValidation.None;
            input.text = value; return input;
        }
        public static void Build(Transform canvas, Action saved)
        {
            var panel = UiKit.Panel(canvas, "Student Details", 1020, 130, 490, 440, UiKit.Ink);
            UiKit.Text(panel.transform, "Student Heading", "STUDENT DETAILS", 25, 12, 440, 38, 23, UiKit.Accent, true);
            UiKit.Text(panel.transform, "Name Label", "Full name / ชื่อ-นามสกุล", 25, 55, 440, 25, 17, Color.white).font = StudentFont;
            var name = Field(panel.transform, "Student Name Input", "Your full name", StudentProfile.DisplayName, 25, 82, 440);
            UiKit.Text(panel.transform, "English Label", "English name (for source namespace)", 25, 137, 440, 25, 17, Color.white);
            var english = Field(panel.transform, "English Name Input", "Firstname Lastname", StudentProfile.EnglishName, 25, 164, 440);
            UiKit.Text(panel.transform, "Code Label", "Student Code", 25, 219, 270, 25, 17, Color.white);
            UiKit.Text(panel.transform, "Section Label", "Section", 310, 219, 155, 25, 17, Color.white);
            var code = Field(panel.transform, "Student Code Input", "Student code", StudentProfile.StudentCode, 25, 246, 270, true);
            var section = Field(panel.transform, "Student Section Input", "Optional", StudentProfile.Section, 310, 246, 155);
            var status = UiKit.Text(panel.transform, "Student Save Status", "Enter your own details, then save.", 25, 384, 440, 42, 17, UiKit.Muted);
            UiKit.Button(panel.transform, "Save Student Details", 25, 310, 440, () =>
            {
                bool inputValid = StudentProfile.IsValid(new StudentProfile.Record { englishName = english.text.Trim(), studentCode = code.text.Trim() });
                bool valid = inputValid && StudentProfile.Save(name.text, english.text, code.text, section.text);
                status.text = valid ? "Saved on this computer." : inputValid ? "Cannot save. Check access to the identity folder." : "Use an English name and a numeric student code.";
                status.color = valid ? UiKit.Accent : new Color(1, 0.58f, 0.66f);
                if (valid) { name.text = StudentProfile.DisplayName; saved?.Invoke(); }
            }, true);
        }
    }
}
