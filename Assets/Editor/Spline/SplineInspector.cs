using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CubicHermiteFiniteDifference))]
public class SplineInspector : Editor
{
    private CubicHermiteFiniteDifference spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI()
    {
        spline = target as CubicHermiteFiniteDifference;

        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = DisplayPoint(0);
        for (int i = 1; i + 3 <= spline.ControlPoints.ToArray().Length; i++)
        {
            Vector3 p1 = DisplayPoint(i);
            Vector3 p2 = DisplayPoint(i + 1);
            Vector3 p3 = DisplayPoint(i + 2);

            Handles.color = Color.grey;
            Handles.DrawDottedLine(p0, p1, 5f);
            Handles.DrawDottedLine(p2, p3, 5f);

            Vector3 lineStart = spline.GetPoint(0f, i);

            int lineSteps = spline.Resolution;

            for (int j = 1; j <= lineSteps; j++)
            {
                float pos = j / (float)lineSteps;

                Vector3 lineEnd = spline.GetPoint(pos, i);
                Vector3 lineTangent = spline.GetSecondDerivative(pos, i).normalized;

                Handles.color = Color.white;
                Handles.DrawLine(lineStart, lineEnd);

                Handles.color = Color.green;
                Handles.DrawLine(lineEnd, lineEnd + lineTangent);

                lineStart = lineEnd;
            }

            p0 = p1;
        }
    }

    private Vector3 DisplayPoint(int i)
    {
        Vector3 localPoint = spline.ControlPoints[i];
        Vector3 globalPoint = handleTransform.TransformPoint(localPoint);

        EditorGUI.BeginChangeCheck();

        globalPoint = Handles.DoPositionHandle(globalPoint, handleRotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.ControlPoints[i] = handleTransform.InverseTransformPoint(globalPoint);
        }

        return globalPoint;
    }
}