using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DieFaceRenderer))]
public class DieFaceRendererEditor : Editor
{

    void OnSceneGUI()
    {
        // Get the chosen GameObject
        DieFaceRenderer t = target as DieFaceRenderer;

        if (t == null || t.faces == null)
            return;

        var zTest = Handles.zTest;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        foreach (var face in t.faces)
        {
            if (face.facePlane == null) continue;

            using (new Handles.DrawingScope(face.facePlane.localToWorldMatrix))
            {
                foreach (var eye in face.dieEyes)
                {
                    Handles.DrawWireDisc(eye.planeOffset, Vector3.up, .07f);
                }
            }
        }

        Handles.zTest = zTest;
    }
}
