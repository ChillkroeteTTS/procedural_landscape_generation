# Beispielprogramme zum Verständniss von Rauschfunktionen
Dieses Unity-Projekt enthält verschiedene Beispielprogramme um die Grundlagen von Noise-Funktionen zu erklären. Die Szenen sind als Prototypen zu Erklärungszwecken zu verstehen. Es wurde kein Wert auf performante Programmierung gelegt.

In der Regel lassen sich alle Parameter der Rauschfunktion im Unity-Inspector anpassen. 
Bei den C#-Implementierungen (Diamond-Square; NoiseCPU) in der Komponente und bei den HLSL-Shaderimplementierungen im Material.

##[...]NoiseLandscape.unity
Im Unterordner Shader/ befindet sich der Shader OwnTerrain.shader welcher jeweils eine Noise-Implementierung ímportiert(Z.37-...). 
Zum einfachereren Umgang wurde dieser Shader merfach kopiert, so dass es für jede Noise-Variation einmal die implementierung (z.B. perlinNoise.cginc) sowie den zugehörigen Shader (PerlinNoise.shader) gibt.

Im Ordner Shader/resources befinden sich verschiedene Materialien die die Shader benutzen und mit sinnvollen Werten befüllen. Die verschiedenen Szenen nutzen verschiedene Materialien.