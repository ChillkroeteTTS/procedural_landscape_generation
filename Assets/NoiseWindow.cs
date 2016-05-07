using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoiseWindow : MonoBehaviour {
    public Rect windowRect = new Rect(20, 20, 380, 135);

    public float LineWidth = 1;

    public Color GridColor = Color.gray;

    public Color AuxColor = Color.magenta;

    public Color LineColor = Color.HSVToRGB(0, 242, 144);

    public float Scale = 22.3f;

    [SerializeField]
    private float _offsetCoordSys = 19.2f;
    [SerializeField]
    private float _offsetAbzysse = 67.9f;

    [SerializeField]
    public List<float> ValueList = new List<float>();

    [SerializeField]
    public List<float> AuxList = new List<float>();

    public int Id = 0;

    void OnGUI() {
        windowRect = GUI.Window(Id, windowRect, DoMyWindow, Id != 0 ? "Noise created by "+(Id-1)+". recursion step" : "Resulting Noise");
    }

    void DoMyWindow(int windowID) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        float drawWidth = windowRect.width - 2* _offsetCoordSys,
              drawHeight = windowRect.height - 2* _offsetCoordSys;

        //Draw Coordinate system
        GUI.Label(new Rect(_offsetCoordSys+drawWidth+_offsetCoordSys/3, windowRect.height-_offsetAbzysse-30/3, drawWidth, 30), "x");
        GUI.Label(new Rect(_offsetCoordSys, _offsetCoordSys - _offsetCoordSys / 2, drawWidth, 30), ""+(1 / Scale));
        GUI.Label(new Rect(_offsetCoordSys, windowRect.height - _offsetCoordSys - _offsetCoordSys / 2, drawWidth, 30), "" + (-1 / Scale));
        Drawing.DrawLine(new Vector2(_offsetCoordSys, _offsetCoordSys), new Vector2(_offsetCoordSys, windowRect.height- _offsetCoordSys), GridColor, LineWidth, true);
        Drawing.DrawLine(new Vector2(_offsetCoordSys, windowRect.height - _offsetAbzysse), new Vector2(windowRect.width- _offsetCoordSys, windowRect.height - _offsetAbzysse), GridColor, LineWidth, true);

        Vector2 lastSecond = Vector2.zero;

        for (int i = 0; i < ValueList.Count; i++) {
            Vector2 first, 
                second = new Vector2(_offsetCoordSys + i/(float)ValueList.Capacity * drawWidth,
                                    windowRect.height - _offsetAbzysse - ValueList[i] * drawHeight / 2 * Scale);
            if (i == 0) 
                first = second;
            else
                first = lastSecond;
            
            Drawing.DrawLine(first, second, LineColor, LineWidth, true);
            lastSecond = second;
        }

        // Draw aux values
        for (int i = 1; i <= AuxList.Count; i++) {
            float x = _offsetCoordSys+i*(drawWidth/(AuxList.Count-1)),
                lastX = _offsetCoordSys + (i-1) * (drawWidth / (AuxList.Count - 1));
            //Draw Grid mark
            Drawing.DrawLine(new Vector2(x, _offsetCoordSys*3), new Vector2(x, windowRect.height-_offsetCoordSys* 3), GridColor, LineWidth-0.2f, true);

            //Draw AuxValue
            Drawing.DrawLine(new Vector2(lastX, windowRect.height - _offsetAbzysse), new Vector2(lastX, windowRect.height - _offsetAbzysse - AuxList[i-1] * drawHeight / 2f * Scale), AuxColor, LineWidth - 0.2f, true);
        }

    }
}