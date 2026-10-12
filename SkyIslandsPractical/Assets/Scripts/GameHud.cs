using UnityEngine;
using UnityEngine.UI;

namespace StudentGame
{
    public sealed class GameHud : MonoBehaviour
    {
        public RunController run;
        public PlayerInteraction interaction;
        Text timer, score, ready, prompt, feedback;
        Image progress, readyPanel, promptPanel, feedbackPanel;
        float feedbackUntil;
        void Start()
        {
            var canvas = UiKit.Canvas();
            var timePanel = UiKit.Panel(canvas, "Countdown Card", 28, 26, 360, 145, UiKit.Ink);
            UiKit.Text(timePanel.transform, "Scene Label", run.stage == 1 ? "STAGE 1 / CAPSULE PLAYER" : "STAGE 2 / HUMANOID PLAYER", 22, 15, 325, 30, 18, UiKit.Muted, true);
            timer = UiKit.Text(timePanel.transform, "Time Remaining", "30.0", 22, 43, 310, 78, 60, Color.white, true);
            UiKit.Text(timePanel.transform, "Unit", "SECONDS LEFT", 22, 118, 300, 22, 13, UiKit.Accent);
            var scorePanel = UiKit.Panel(canvas, "Score Card", 1200, 26, 372, 145, UiKit.Ink);
            UiKit.Text(scorePanel.transform, "Score Label", "YOUR SCORE", 22, 15, 320, 30, 18, UiKit.Muted, true);
            score = UiKit.Text(scorePanel.transform, "Score", "0", 22, 44, 320, 70, 52, Color.white, true);
            UiKit.Text(scorePanel.transform, "Goal", "WIN AT TIMEOUT: MORE THAN 10,000", 22, 115, 330, 25, 14, UiKit.Accent);
            UiKit.Panel(canvas, "Progress Track", 1200, 177, 372, 6, new Color(0.1f, 0.24f, 0.28f));
            progress = UiKit.Panel(canvas, "Score Progress", 1200, 177, 1, 6, UiKit.Accent);
            UiKit.Button(canvas, "Back to Main Menu", 680, 28, 250, () => SceneNavigation.Load("MainMenu"));
            var controls = UiKit.Panel(canvas, "Controls", 28, 802, 720, 70, UiKit.Ink);
            UiKit.Text(controls.transform, "Controls Text", run.stage == 1 ? "WASD  MOVE    SHIFT  SPRINT    SPACE  JUMP    ESC  MENU" : "WASD  MOVE    SHIFT  SPRINT    SPACE  JUMP    E  COLLECT", 18, 0, 690, 70, 18, Color.white);
            var legend = UiKit.Panel(canvas, "Item Legend", 1010, 750, 562, 122, UiKit.Ink);
            UiKit.Text(legend.transform, "Good", "CYAN  +500 pts     GOLD  +3 seconds", 18, 12, 525, 42, 19, UiKit.Accent);
            UiKit.Text(legend.transform, "Bad", "RED  -500 pts       PURPLE  -5 seconds", 18, 57, 525, 42, 19, new Color(1, 0.58f, 0.66f));
            readyPanel = UiKit.Panel(canvas, "Countdown Background", 460, 325, 680, 190, UiKit.Ink);
            ready = UiKit.Text(canvas, "Ready Countdown", "", 460, 325, 680, 190, 64, Color.white, true); ready.alignment = TextAnchor.MiddleCenter;
            promptPanel = UiKit.Panel(canvas, "Interaction Background", 520, 660, 560, 62, UiKit.Ink);
            prompt = UiKit.Text(canvas, "Interaction Prompt", "", 520, 660, 560, 62, 28, UiKit.Accent, true); prompt.alignment = TextAnchor.MiddleCenter;
            feedbackPanel = UiKit.Panel(canvas, "Feedback Background", 560, 190, 480, 65, UiKit.Ink);
            feedback = UiKit.Text(canvas, "Pickup Feedback", "", 560, 190, 480, 65, 32, UiKit.Accent, true); feedback.alignment = TextAnchor.MiddleCenter;
            promptPanel.gameObject.SetActive(false); feedbackPanel.gameObject.SetActive(false);
            run.ItemCollected += ShowFeedback;
        }
        void ShowFeedback(string value) { feedback.text = value; feedbackUntil = Time.time + 1.0f; }
        void Update()
        {
            if (timer == null || run == null) return;
            timer.text = run.ActiveState.Remaining.ToString("0.0"); timer.color = run.ActiveState.Remaining < 8 ? new Color(1, 0.42f, 0.4f) : Color.white;
            score.text = run.ActiveState.Score.ToString("N0"); progress.rectTransform.sizeDelta = new Vector2(372 * Mathf.Clamp01(run.ActiveState.Score / 10500f), 6);
            ready.text = run.StartDelay > 0 ? "GET READY\n" + Mathf.CeilToInt(run.StartDelay) : "";
            if (run.ActiveState.Outcome != RunOutcome.Playing) ready.text = run.ActiveState.Outcome == RunOutcome.Won ? "TIME UP / YOU WIN" : "RUN ENDED";
            readyPanel.gameObject.SetActive(ready.text.Length > 0);
            prompt.text = interaction != null && interaction.Focused != null && interaction.Focused.gameObject.activeSelf ? "[ E ]  COLLECT " + interaction.Focused.Label : "";
            if (Time.time > feedbackUntil) feedback.text = "";
            promptPanel.gameObject.SetActive(prompt.text.Length > 0); feedbackPanel.gameObject.SetActive(feedback.text.Length > 0);
        }
        void OnDestroy() { if (run != null) run.ItemCollected -= ShowFeedback; }
    }
}
