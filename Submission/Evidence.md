# คำตอบและหลักฐาน Practical Exam 958321

สิริกัญญา กาวิลาวรรณ | Sirikanya Kawilawan | 672110126 | Section 001

[คลิปหลักฐานเกมจริง](https://github.com/thanabordin9/672110126-practical-exam-958321/releases/download/submission-2026-10-12/ExamEvidence.mp4)

คลิปบันทึกภาพที่ Unity PlayMode เรนเดอร์จริงและเสียงที่ AudioListener ผสมจริง ใช้คีย์บอร์ดจำลองผ่าน Input System เพื่อทดสอบ ไม่เพิ่มคะแนน ไม่ข้ามเวลานับถอยหลัง และไม่ย้ายตำแหน่งผู้เล่นแบบ teleport ปุ่มเมนูถูกเรียกโดยเครื่องมือทดสอบในฉากจริง กติกาการเก็บและแพ้ชนะเป็นโค้ดเดียวกับ Windows build

## ตำแหน่งหลักฐานในคลิป

- 00:00 — Q1 Q10 MainMenu and student identity
- 00:02 — Q1 Q9 additive Options and volume control
- 00:08 — Q1 Credits identity
- 00:10 — Q1 StageSelection
- 00:12 — Q2 Q3 Stage1 capsule and sky route
- 00:23 — Q4 Q5 Q11 collected score and time bonuses; wait for natural timeout
- 01:01 — Q1 Q5 Win after natural timeout
- 01:07 — Q4 Q12 Stage1 four item effects and obstacle collision
- 01:16 — Q1 Q5 falling causes GameOver
- 01:19 — Q1 Q5 Retry resets timer and score
- 01:24 — Q6 Q7 Stage2 humanoid arena
- 01:29 — Q8 collision alone does not collect; E interaction
- 01:33 — Q6 Q8 Q11 Q12 movement interaction jump and obstacles
- 01:58 — Q5 Stage2 natural timeout below target
- 02:01 — Q1 return MainMenu

## คำตอบ 12 ข้อ

### Q1

สร้าง 8 ฉาก MainMenu, StageSelection, Options, Credits, Stage1, Stage2, Win, GameOver

Options เปิดแบบ Additive; OK ปิดฉากซ้อนและกลับเมนูเดิม ด่านกลับ StageSelection

GameOver มี Retry ด่านเดิม; Win และ GameOver กลับ MainMenu ได้ มีชื่อฉากบน UI

### Q2

Capsule ใช้ CharacterController และ CapsuleMotor รับ WASD, Shift และ Space

เคลื่อนที่ตามทิศกล้อง มีแรงโน้มถ่วง ตรวจพื้นและการกระโดด กล้อง Cinemachine ติดตาม

RunController เริ่มเวลา 30 วินาทีหลังเตรียมตัว 3 วินาที HUD แสดงเวลาและคะแนน

### Q3

สร้างพื้นเกาะลอย 7 เกาะและสะพานด้วย ProBuilderMesh ที่บันทึกในฉาก

มีสะพานแคบและกว้าง ไม่มีรั้วกันตก เก็บคะแนนจาก Prefab +500 จำนวน 32 ชิ้น

วางไอเท็มเวลา คะแนนติดลบ และ RockObstacle ตามเส้นทาง คะแนนบวกมีเพียงพอให้ชนะ

### Q4

แยกชนิดไอเท็มด้วย ItemKind และคอมโพเนนต์ ItemEffect ใน Prefab ทั้ง 4 ชนิด

TimePlus3 เพิ่มเวลา 3 วินาที; TimeMinus5 ลดเวลา 5 วินาที

ScorePlus500 เพิ่ม 500 คะแนน; ScoreMinus500 ลด 500 คะแนน

Stage1 เก็บด้วย Trigger เมื่อวัตถุมี tag Player เก็บแล้วปิดไอเท็มและไม่คิดซ้ำ

RockObstacle มี Collider และ Obstacle ใช้ PlayerContact ตรวจชนและเล่นเสียง

ItemEffect เรียก RunState.Apply และ AudioService.PlayEffect พร้อมข้อความบน HUD

### Q5

RunState เริ่มเวลา 30 วินาที คะแนน 0; Tick นับถอยหลังด้วย deltaTime ของเกม

ชนะเมื่อหมดเวลาและคะแนนมากกว่า 10,000 เท่านั้น เก็บถึงเป้าแล้วยังเล่นต่อจนหมดเวลา

คะแนนเท่ากับ 10,000 ยังไม่ผ่าน เพราะโจทย์ชนะระบุว่ามากกว่า 10,000

หมดเวลาและคะแนนไม่ผ่านเป้า หรือพิกัดผู้เล่นตกต่ำกว่าฉาก จะเข้า GameOver

กด Esc กลับ MainMenu เพื่อยกเลิกรอบ กด Back กลับ StageSelection

Retry สร้างด่านเดิมใหม่ คืนเวลาและคะแนน ทดสอบ boundary, timeout และ fall

### Q6

ใช้ Ch02_nonPBR จากทรัพยากรข้อสอบ ตั้ง ModelImporter เป็น Humanoid และ Avatar valid

ใช้ Starter Assets ThirdPersonController, StarterAssetsInputs, PlayerInput และ Animator

CharacterController ตรวจพื้น กล้อง Cinemachine ติดตาม; PlayerInteraction เลือกไอเท็มใกล้ตัว

### Q7

พื้นหลัก Stage2 สร้างด้วย ProBuilder ขนาด 12 x 12 เมตร รวม 144 ตารางเมตร

มีสะพานและเกาะโบนัส วาง Prefab +500 จำนวน 25 ชิ้นในลานและ 4 ชิ้นบนเกาะโบนัส

วางไอเท็มเวลา คะแนนติดลบ และสิ่งกีดขวาง คะแนนบวกมากกว่า 10,000

### Q8

Stage2 ใช้ PlayerInteraction ตรวจไอเท็มในระยะ 2.25 เมตร พร้อมตรวจทิศและสิ่งบัง

แสดงข้อความ E COLLECT เมื่อเลือกไอเท็ม ต้องกด E จึงเรียก TryCollect แบบ explicit

ตั้ง interactionOnly=true ทำให้เดินชนอย่างเดียวไม่เก็บ ทดสอบว่าคะแนนยังเป็นศูนย์

ใช้ Prefab TimePlus3, TimeMinus5, ScorePlus500 และ ScoreMinus500 เหมือนด่าน 1

เก็บแล้วปรับเวลา/คะแนนตามชนิด เล่นเสียงและปิดไอเท็มเพื่อป้องกันการเก็บซ้ำ

ใช้กติกาเวลา คะแนน การชนะและแพ้ชุดเดียวกับ Stage1

### Q9

Options มี Slider แยก Background Music และ Sound Effects

AudioService ปรับ AudioSource.volume และบันทึก PlayerPrefs คงค่าข้ามฉากและเปิดเกมใหม่

### Q10

AudioService เป็น singleton แบบ DontDestroyOnLoad เล่น Tiny_Blocks.wav วนทุกฉาก

ฉากใหม่ไม่สร้างเพลงซ้ำ จึงต่อเนื่องทั้งเมนู ด่าน Options Credits Win และ GameOver

### Q11

ItemEffect เรียก PlayEffect ของ AudioService เมื่อเก็บสำเร็จ

ใช้ Coin Pickup 4, Buff_Power Up 1 และ Debuff Downgrade 1 ตามชนิดไอเท็ม

### Q12

RockObstacle มี Collider และ Obstacle; PlayerContact ตรวจการชนแล้วเรียก PlayEffect

ใช้ Bounce_Jump 3.wav และมีการเว้นช่วงเพื่อไม่ให้เล่นซ้ำทุกเฟรมเมื่อสัมผัสค้าง

## ผลตรวจและข้อจำกัด

- EditMode 34 ผ่าน; PlayMode ปกติ 10 ผ่าน; งานอัดคลิป Explicit 1 ผ่านแยกต่างหาก
- ดู XML ใน SkyIslandsPractical/Verification/canvas-editmode-final.xml, canvas-playmode.xml และ canvas-recorded-evidence.xml
- การเดินและการกด E ในคลิปเกิดจาก Input System จริงใน engine แต่คลิปไม่ใช่การเล่นด้วยมือของนักศึกษา
- ขนาดพื้นที่และการตั้งค่า ProBuilder/Avatar ตรวจจาก source ฉากและ EditMode ไม่ควรตีความคลิปเพียงอย่างเดียวว่าพิสูจน์ค่าเหล่านั้น
- คลิปเน้นผลการเล่น ไม่ได้แสดงการเก็บไอเท็มทั้ง 4 ชนิดด้วย E ใน Stage2 ทุกชนิด; Prefab และโค้ดกำหนดผลครบและมีผลทดสอบประกอบ
- ประวัติ Git จริงเริ่มวันที่ 12 ตุลาคม 2026 ตามเวลาไทย จึงไม่มีหลักฐาน commit/push ย้อนหลังครบวันที่ 8–11 ตุลาคม และยังไม่รับรองเงื่อนไขทุกวันตลอดช่วงสอบ
- GitHub repository อยู่ในบัญชีที่เชื่อมไว้ thanabordin9 โดยระบุผู้ส่งงาน Sirikanya Kawilawan และรหัส 672110126 ในโปรเจกต์และเอกสาร

## การแก้ Canvas ใน Editor

ทุกฉากมี Canvas และ Prefab UI ก่อน Play ปุ่มมี persistent On Click ที่แก้ใน Inspector ได้ ภาพคลิปนี้บันทึกหลังเปลี่ยนเป็น Canvas ที่บันทึกในฉากแล้ว ฟอนต์ UI รองรับอังกฤษและไทย พร้อม OFL license และประกาศการดัดแปลงใน Assets/Resources
