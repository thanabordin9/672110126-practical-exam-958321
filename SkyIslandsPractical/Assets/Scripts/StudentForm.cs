using UnityEngine;
using UnityEngine.UI;

namespace SirikanyaKawilawan
{
    public sealed class StudentForm : MonoBehaviour
    {
        public InputField nameInput,englishInput,codeInput,sectionInput;
        public Text status,identity;
        void Start() => Refresh();
        public void Refresh()
        {
            nameInput.text=StudentProfile.DisplayName;englishInput.text=StudentProfile.EnglishName;
            codeInput.text=StudentProfile.StudentCode;sectionInput.text=StudentProfile.Section;
            if(identity!=null)identity.text=StudentProfile.Summary;
        }
        public void Save()
        {
            bool inputValid=StudentProfile.IsValid(new StudentProfile.Record{englishName=englishInput.text.Trim(),studentCode=codeInput.text.Trim()});
            bool valid=inputValid&&StudentProfile.Save(nameInput.text,englishInput.text,codeInput.text,sectionInput.text);
            status.text=valid?"Saved on this computer.":inputValid?"Cannot save. Check access to the identity folder.":"Use an English name and a numeric student code.";
            status.color=valid?UiKit.Accent:new Color(1,0.58f,0.66f);
            if(valid)Refresh();
        }
    }
}
