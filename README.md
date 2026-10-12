# Sky Islands Practical Exam 958321

สิริกัญญา กาวิลาวรรณ / Sirikanya Kawilawan / 672110126 / Section 001

Unity 3D เกม Time Attack สองด่านตาม Practical Exam 1/2569

- [คำตอบแบบฟอร์ม 9 หน้า](Submission/169-PracticalExam-672110126-Answered.pdf)
- [คลิปหลักฐานพร้อมเสียง](https://github.com/thanabordin9/672110126-practical-exam-958321/releases/download/submission-2026-10-12/ExamEvidence.mp4)
- [คำอธิบาย 12 ข้อและตำแหน่งในคลิป](Submission/Evidence.md)
- [ดาวน์โหลดเกม Windows](https://github.com/thanabordin9/672110126-practical-exam-958321/releases/tag/submission-2026-10-12)
- [ประวัติ Git พร้อมวันเวลา](Submission/GitHistory.tsv)

เปิดโฟลเดอร์ SkyIslandsPractical ด้วย Unity 6000.6.5f1 แล้วเปิด Assets/Scenes/MainMenu.unity กด Play

WASD เดิน, Shift วิ่ง, Space กระโดด, Esc กลับเมนู ด่าน 1 เก็บด้วยการชน ด่าน 2 เข้าใกล้แล้วกด E

source ที่พัฒนาเองใช้ namespace SirikanyaKawilawan รักษา namespace StarterAssets ของ Unity ไว้

คลิปใช้การทดสอบคีย์บอร์ดจำลองใน Unity engine และเวลานับถอยหลังจริง ไม่ใช่การเล่นด้วยมือ ประวัติ commit เป็นของจริง เริ่มทำวันที่ 12 ตุลาคม 2026 จึงไม่สามารถรับรองการ commit/push ครบทุกวันย้อนหลังในช่วงสอบได้

UI เป็น Canvas ที่บันทึกในทั้ง 8 ฉาก แก้ได้ใน Hierarchy ก่อน Play และมี Prefab ใน Assets/Prefabs/UI ปุ่มมี On Click ที่ตั้งค่าใน Inspector ได้ ดูคำแนะนำเพิ่มเติมใน SkyIslandsPractical/README.md
