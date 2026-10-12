using UnityEngine;
namespace SirikanyaKawilawan
{
    [CreateAssetMenu(menuName = "Sky Islands/Student Profile")]
    public sealed class StudentProfileAsset : ScriptableObject
    {
        public StudentProfile.Record data = new StudentProfile.Record();
    }
}
