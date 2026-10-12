# Sky Islands

เกม Unity 3D สองด่านสำหรับเกณฑ์ Practical Exam 958321

## เล่นเกม
เปิด `SkyIslands.exe` ในโฟลเดอร์ Windows build แล้วกด **Start Adventure** เลือกด่าน 1 หรือ 2

- WASD: เดิน, Shift: วิ่ง, Space: กระโดด, Esc: กลับเมนู
- Stage 1: เดินชนไอเท็มเพื่อเก็บ
- Stage 2: เข้าใกล้ไอเท็มที่แสดงข้อความแล้วกด E เพื่อเก็บ
- ปุ่ม Back ในด่านกลับ StageSelection; Options เปิดแบบ Additive และรักษาข้อมูลที่ยังไม่ได้บันทึกในเมนู
- เปิด Options เพื่อปรับ Background Music และ Sound Effects แยกกัน ค่าเสียงบันทึกไว้เมื่อเปลี่ยนฉากหรือปิดเกม

เริ่มด้วยเวลา 30 วินาทีหลังนับเตรียมตัว 3 วินาที คะแนนเริ่มที่ศูนย์ ไอเท็มสีฟ้าเพิ่ม 500 คะแนน สีแดงลด 500 คะแนน สีทองเพิ่มเวลา 3 วินาที สีม่วงลดเวลา 5 วินาที ชนะเมื่อหมดเวลาและคะแนน **มากกว่า 10,000** ตกฉากจะแพ้เสมอ คะแนนเท่ากับ 10,000 ยังแพ้ตามเงื่อนไขชนะที่ระบุว่ามากกว่า 10,000 คะแนน

## เปิดแก้ไขโปรเจกต์
ใช้ Unity **6000.6.5f1** บน Windows ใน Unity Hub เลือก Add project from disk แล้วเลือกโฟลเดอร์ที่มี Assets, Packages และ ProjectSettings เปิด `Assets/Scenes/MainMenu.unity` และกด Play ครั้งแรกต้องรอ Package Manager นำเข้าแพ็กเกจ

เมนู `Sky Islands > Generate Game Scenes` ใช้สร้างฉากและ Prefab ใหม่ตามแบบที่อยู่ใน ProjectBuilder **จะเขียนทับฉากและ Prefab ที่สร้างไว้** จึงควรสำรองงานที่แก้เองก่อนใช้ เมนู `Sky Islands > Build Windows Game` สร้าง Windows build ใน Builds/Windows

Unity เวอร์ชันนี้จัดหา Cinemachine 6.6.0, Unity UI 2.6.0 และ Test Framework 1.8.0 จากแพ็กเกจที่มากับ Editor ส่วน ProBuilder 6.1.3 และ Input System 1.20.1 ติดตั้งผ่าน registry รายการที่ resolve จริงอยู่ใน Packages/packages-lock.json

## โครงสร้าง
- Assets/Scenes: MainMenu, StageSelection, Options, Credits, Stage1, Stage2, Win, GameOver
- Assets/Prefabs: TimePlus3, TimeMinus5, ScorePlus500, ScoreMinus500, RockObstacle
- Assets/Scripts: namespace SirikanyaKawilawan; กติกาเกม UI ระบบเสียง การชน และ Interaction
- Assets/Editor/ProjectBuilder.cs: สร้างฉากจาก ProBuilder และตั้งค่าตัวละคร
- Assets/StarterAssets: คอนโทรลเลอร์และ locomotion ของ Unity Technologies พร้อมที่มาและใบอนุญาต
- Assets/ExamResources: ตัวละคร Ch02, animation และเสียงที่ให้มา
- Assets/Tests: EditMode ตรวจ rule/scene/Avatar และ PlayMode ตรวจ movement, E, retry, UI/audio
- Verification: ผลการทดสอบและภาพจาก Play Mode เมื่อทดสอบพร้อม graphics

ตัวละครด่าน 2 ใช้ ThirdPersonController, StarterAssetsInputs, PlayerInput, CharacterController, Animator แบบ Humanoid และ Cinemachine Follow กล้องตามตำแหน่งแบบมุมคงที่ การวางพื้นและสะพานใช้ ProBuilderMesh จริง สามารถเปิดแก้ด้วย ProBuilder ได้ พื้นหลักด่าน 2 มีขนาด 12 × 12 เมตร (144 ตารางเมตร)

## ก่อนส่งงาน
ข้อมูลนักศึกษา: สิริกัญญา กาวิลาวรรณ / Sirikanya Kawilawan / 672110126 / Section 001 ข้อมูลเริ่มต้นบันทึกใน Assets/Resources/StudentProfile.asset และ source ที่พัฒนาเองอยู่ใน namespace SirikanyaKawilawan

GitHub: https://github.com/thanabordin9/672110126-practical-exam-958321

คำตอบและคลิปอยู่ในโฟลเดอร์ Submission ที่ root ของ repository ประวัติ Git ใช้วันเวลาจริง เริ่มวันที่ 12 ตุลาคม 2026 จึงไม่สามารถรับรองเงื่อนไข commit/push ย้อนหลังครบทุกวันตลอดช่วงสอบได้

## แก้ UI ใน Unity Editor
ทุกฉากมี Game UI เป็น Canvas พร้อม CanvasScaler และ GraphicRaycaster ตั้งแต่ก่อนกด Play ขยาย Game UI ใน Hierarchy เพื่อเลือกปุ่ม Text Slider หรือ InputField แล้วแก้ RectTransform สีและข้อความใน Inspector

Prefab อยู่ใน Assets/Prefabs/UI มี 8 ไฟล์ตามชื่อฉาก ปุ่มมี On Click ที่บันทึกจริง เรียก SceneButtonAction.Invoke; เปลี่ยน Command, Target Scene และ Stage ในคอมโพเนนต์ของปุ่มเพื่อแก้เส้นทาง ปุ่ม Save ใช้ StudentForm.Save และ Slider ใช้ VolumeControlBinding.Apply

GameHud เก็บ reference ของเวลา คะแนน แถบความคืบหน้าและข้อความในฉาก ระบบอัปเดตเฉพาะค่าระหว่างเล่น ไม่สร้าง Canvas ทับงานที่แก้ไว้ เมนู Sky Islands > Create Editable Canvas UI ใช้เตรียม UI ใหม่เมื่อฉากยังไม่มี Canvas ควรบันทึกและสำรองฉากก่อนใช้เครื่องมือสร้างฉาก

## แหล่งทรัพยากร
ไฟล์ตัวละคร/animation/เสียง: PracticalExamResources.zip ที่ผู้ใช้ให้มา

Starter Assets จากตัวอย่างทางการของ Unity Technologies:
https://github.com/Unity-Technologies/com.unity.multiplayer.samples.bitesize/tree/v1.2.1/Basic/ClientDriven/Assets/StarterAssets

รักษา license.txt ของ Starter Assets ไว้ การตั้งค่าการนำเข้า locomotion ปิด animation events เสียงเท้าเพื่อให้เสียงทั้งหมดที่เกมใช้ผ่าน AudioService และตัวปรับ SFX
