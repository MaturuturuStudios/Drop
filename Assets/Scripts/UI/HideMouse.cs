using UnityEngine;

public class HideMouse : MonoBehaviour {
    public Texture2D cursorTexture;
    void Start() {
        Cursor.visible = false;
        if (!Application.isEditor) {
            CursorMode cursorMode = CursorMode.Auto;
            Vector2 hotSpot = Vector2.zero;
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
    }
}
