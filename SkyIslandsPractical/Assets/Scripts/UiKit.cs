using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace StudentGame
{
    public static class UiKit
    {
        public static readonly Color Ink = new Color(0.035f, 0.08f, 0.12f, 0.92f);
        public static readonly Color Accent = new Color(0.25f, 0.96f, 0.84f);
        public static readonly Color Muted = new Color(0.66f, 0.78f, 0.82f);
        static Font font;
        public static RectTransform Canvas()
        {
            var go = new GameObject("Game UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1600, 900); scaler.matchWidthOrHeight = 0.5f;
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var events = new GameObject("UI Event System", typeof(EventSystem));
                events.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
            }
            return go.GetComponent<RectTransform>();
        }
        public static RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); r.SetParent(parent, false);
            r.anchorMin = r.anchorMax = new Vector2(0, 1); r.pivot = new Vector2(0, 1); r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
        }
        public static Image Panel(Transform parent, string name, float x, float y, float w, float h, Color colour)
        {
            var image = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Image>(); image.color = colour; return image;
        }
        public static Text Text(Transform parent, string name, string value, float x, float y, float w, float h, int size, Color colour, bool bold = false)
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var text = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Text>(); text.font = font; text.text = value; text.fontSize = size; text.color = colour;
            text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal; text.alignment = TextAnchor.MiddleLeft; text.raycastTarget = false; return text;
        }
        public static Button Button(Transform parent, string label, float x, float y, float w, UnityAction action, bool primary = false)
        {
            var bg = Panel(parent, label, x, y, w, 62, primary ? Accent : new Color(0.12f, 0.21f, 0.26f, 0.96f));
            var button = bg.gameObject.AddComponent<Button>(); button.targetGraphic = bg;
            var colors = button.colors; colors.highlightedColor = new Color(0.74f, 1, 0.92f); colors.pressedColor = new Color(0.5f, 0.78f, 0.74f); button.colors = colors;
            Text(bg.transform, "Label", label, 22, 0, w - 35, 62, 23, primary ? Ink : Color.white, true);
            button.onClick.AddListener(action); return button;
        }
        public static Slider Slider(Transform parent, string title, float x, float y, float value, UnityAction<float> callback)
        {
            Text(parent, title, title, x, y, 420, 45, 23, Color.white, true);
            var root = Rect(parent, title + " Slider", x, y + 65, 500, 40);
            var slider = root.gameObject.AddComponent<Slider>(); slider.minValue = 0; slider.maxValue = 1;
            Panel(root, "Track", 0, 12, 500, 12, new Color(0.21f, 0.33f, 0.38f));
            var fillArea = Rect(root, "Fill Area", 0, 12, 500, 12);
            var fill = Panel(fillArea, "Fill", 0, 0, 500, 12, Accent); slider.fillRect = fill.rectTransform;
            var handleArea = Rect(root, "Handle Area", 0, 0, 500, 40);
            var handle = Panel(handleArea, "Handle", 0, 0, 24, 40, Color.white); slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
            slider.value = value;
            var percent = Text(parent, title + " Value", Mathf.RoundToInt(value * 100) + "%", x + 425, y, 80, 45, 23, Accent);
            slider.onValueChanged.AddListener(v => { callback(v); percent.text = Mathf.RoundToInt(v * 100) + "%"; }); return slider;
        }
    }
}
