using System.Collections.Generic;
using UnityEngine;

public class NoiseWindow : MonoBehaviour {
    public Rect windowRect = new Rect(50, 40, 250, 100);

    public float LineWidth = 1;

    public Color GridColor = Color.gray;

    public Color LineColor = Color.red;

    public float Scale = 6;

    [SerializeField]
    private float _offset = 6;

    [SerializeField]
    public List<float> ValueList = new List<float>();

    public int ExpectedValues = 1;

    void OnGUI() {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Noise");
    }

    void DoMyWindow(int windowID) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        float drawWidth = windowRect.width - 2*_offset,
              drawHeight = windowRect.height - 2*_offset;

        //Draw Coordinate system
        GUI.Label(new Rect(_offset, _offset - _offset /2, drawWidth, 30), ""+(1 / Scale));
        Drawing.DrawLine(new Vector2(_offset, _offset), new Vector2(_offset, windowRect.height- _offset), GridColor, LineWidth, true);
        Drawing.DrawLine(new Vector2(_offset, windowRect.height - _offset), new Vector2(windowRect.width-_offset, windowRect.height - _offset), GridColor, LineWidth, true);

        Vector2 lastSecond = Vector2.zero;

        for (int i = 0; i < ValueList.Count; i++) {
            Vector2 first, 
                second = new Vector2(_offset + i/(float)ExpectedValues * drawWidth,
                                    windowRect.height - _offset - ValueList[i] * drawHeight * Scale);
            if (i == 0) 
                first = new Vector2(_offset, windowRect.height - _offset);
            else
                first = lastSecond;
            
            Drawing.DrawLine(first, second, LineColor, LineWidth, true);
            lastSecond = second;
        }


    }
}