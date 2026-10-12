# Practical Exam evidence checklist

รายการนี้อธิบายตำแหน่งระบบและสิ่งที่ควรบันทึกวิดีโอ ไม่ใช่การรับรองคะแนนจากผู้ตรวจ

| เกณฑ์ | คะแนนเต็ม | ระบบที่เตรียม | สิ่งที่ควรแสดงในวิดีโอ |
|---|---:|---|---|
| Scene Management | 10 | 8 scenes และ SceneNavigation | MainMenu → StageSelection → ด่านทั้งสอง; Options, Credits, Win, GameOver, retry และ Back |
| Stage1 Capsule | 5 | CapsuleMotor + CharacterController | WASD, วิ่ง, กระโดด และการตกฉาก |
| Stage1 Level Design | 5 | ProBuilder islands และสะพานกว้าง/แคบ | Scene view แสดง ProBuilderMesh และทางเดินที่ตกได้ |
| Stage1 Items/Obstacles | 5 | Prefab ไอเท็ม 4 ชนิดและ RockObstacle | ชนไอเท็มแล้วเห็น +3s, -5s, +500, -500 และชนสิ่งกีดขวาง |
| Stage1 Win/Lose | 10 | RunState + RunController | HUD 30 วินาที; หมดเวลาชนะเมื่อ >10000; คะแนนไม่พอและตกฉากแพ้ |
| Stage2 Humanoid | 5 | Ch02 Humanoid + official Starter Assets | Avatar Valid/Human, Animator, PlayerInput, เดิน/วิ่ง/กระโดด |
| Stage2 Level Design | 5 | ProBuilder arena 12×12 เมตร | ขนาด 144 ตารางเมตรและ Prefab กระจายตามฉาก |
| Stage2 Interaction | 10 | PlayerInteraction + ItemEffect | เดินชนไม่เก็บ; ข้อความ E; กด E รับผลไอเท็มครบ 4 ชนิด |
| Volume Control | 5 | AudioService และ Options sliders | แยก music/SFX; ทดลองค่า 0/สูง; เปลี่ยนฉากแล้วค่ายังอยู่ |
| BGM ทุกฉาก | 5 | persistent music + scene service | เล่นเพลงในทุกฉาก รวมหน้า Win/GameOver/Options/Credits |
| Item SFX | 5 | pickupClip ผ่าน AudioService | ฟังเสียงเก็บไอเท็มทั้งสองด่าน |
| Obstacle SFX | 5 | PlayerContact + Obstacle | ชน RockObstacle แล้วมีเสียงและไม่เล่นซ้ำทุกเฟรม |

## ข้อมูลที่ผู้ใช้ต้องกรอกด้วยหลักฐานจริง
- ชื่อภาษาอังกฤษสำหรับ namespace และชื่อ/รหัสนักศึกษาในเอกสารส่งงาน
- public Git repository URL
- วิดีโอจริงและคำอธิบายหลักฐานของแต่ละเกณฑ์
- Screenshot ของ Git history ที่มีวันเวลา commit จริง

## ขอบเขตการตรวจ
EditMode ตรวจค่า effect, boundary 10000/10500, fall, timeout, terminal guard, reset, build scenes, saved mesh, ProBuilder, Prefab, พื้นที่และ Avatar

PlayMode ตรวจ one-time collection, explicit interaction, audio singleton/volume persistence, เส้นทาง UI, retry, การเดินด้วย keyboard ของ Capsule/Starter Assets และกด E จริงผ่าน Input System

ภาพทดสอบและ XML จะอยู่ใน Verification หลังรันทดสอบ ไม่ใช้วิดีโอสมมติหรือ URL ที่ยังไม่ได้สร้าง
