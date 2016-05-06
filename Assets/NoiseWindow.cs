using System.Collections.Generic;
using UnityEngine;

public class NoiseWindow : MonoBehaviour {
    public Rect windowRect = new Rect(50, 40, 250, 100);

    public float LineWidth = 1;

    public Color GridColor = Color.gray;

    public Color LineColor = Color.red;

    public float Scale = 6;

    [SerializeField]
    private float _offsetCoordSys = 6;
    [SerializeField]
    private float _offsetAbzysse = 12;

    [SerializeField]
    public List<float> ValueList = new List<float>();

    public int ExpectedValues = 1;

    void OnGUI() {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Noise");
    }

    void DoMyWindow(int windowID) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        float drawWidth = windowRect.width - 2* _offsetCoordSys,
              drawHeight = windowRect.height - 2* _offsetAbzysse;

        //Draw Coordinate system
        GUI.Label(new Rect(_offsetCoordSys, _offsetCoordSys - _offsetCoordSys / 2, drawWidth, 30), ""+(1 / Scale));
        GUI.Label(new Rect(_offsetCoordSys, windowRect.height - _offsetCoordSys - _offsetCoordSys / 2, drawWidth, 30), "" + (-1 / Scale));
        Drawing.DrawLine(new Vector2(_offsetCoordSys, _offsetCoordSys), new Vector2(_offsetCoordSys, windowRect.height- _offsetCoordSys), GridColor, LineWidth, true);
        Drawing.DrawLine(new Vector2(_offsetCoordSys, windowRect.height - _offsetAbzysse), new Vector2(windowRect.width- _offsetCoordSys, windowRect.height - _offsetAbzysse), GridColor, LineWidth, true);

        Vector2 lastSecond = Vector2.zero;

        for (int i = 0; i < ValueList.Count; i++) {
            Vector2 first, 
                second = new Vector2(_offsetCoordSys + i/(float)ExpectedValues * drawWidth,
                                    windowRect.height - _offsetAbzysse - ValueList[i] * drawHeight * Scale);
            if (i == 0) 
                first = second;
            else
                first = lastSecond;
            
            Drawing.DrawLine(first, second, LineColor, LineWidth, true);
            lastSecond = second;
        }


    }
}