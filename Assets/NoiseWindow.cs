using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoiseWindow : MonoBehaviour {
    public Rect windowRect = new Rect(20, 20, 380, 135);

    public float LineWidth = 1;

    public Color GridColor = Color.gray;

    public Color LineColor = Color.HSVToRGB(0, 242, 144);

    public float Scale = 130.2f;

    [SerializeField]
    private float _offsetCoordSys = 19.2f;
    [SerializeField]
    private float _offsetAbzysse = 61.05f;

    [SerializeField]
    public List<float> ValueList = new List<float>();

    public int Id = 0;

    void OnGUI() {
        windowRect = GUI.Window(Id, windowRect, DoMyWindow, "Noise");
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
                second = new Vector2(_offsetCoordSys + i/(float)ValueList.Capacity * drawWidth,
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