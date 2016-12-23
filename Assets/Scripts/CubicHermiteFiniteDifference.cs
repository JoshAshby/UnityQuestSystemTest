using UnityEngine;
using System.Collections.Generic;

public class CubicHermiteFiniteDifference : MonoBehaviour, ICubicHermiteSpline
{
    [SerializeField]
    [Range(1, 50)]
    public int Resolution = 10;

    [SerializeField]
    public List<Vector3> ControlPoints;

    public void Reset()
    {
        ControlPoints = new List<Vector3> {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }

    public Vector3 GetPoint(float t, int k)
    {
        t = Mathf.Clamp01(t);

        Vector3 pkm1 = ControlPoints[k - 1];
        Vector3 pk   = ControlPoints[k];
        Vector3 pkp1 = ControlPoints[k + 1];
        Vector3 pkp2 = ControlPoints[k + 2];

        Vector3 m0 = (((pkp1 - pk) + (pk - pkm1)) / 2);
        Vector3 m1 = (((pkp2 - pkp1) + (pkp1 - pk)) / 2);

        float h00 = (2 * Mathf.Pow(t, 3f)) - (3 * Mathf.Pow(t, 2f)) + 1;
        float h10 = Mathf.Pow(t, 3f) - (2 * Mathf.Pow(t, 2f)) + t;
        float h01 = ((-2) * Mathf.Pow(t, 3f)) + (3 * Mathf.Pow(t, 2f));
        float h11 = Mathf.Pow(t, 3f) - Mathf.Pow(t, 2f);

        return (h00 * pk) + (h10 * m0) + (h01 * pkp1) + (h11 * m1);
    }

    public Vector3 GetFirstDerivative(float t, int k)
    {
        t = Mathf.Clamp01(t);

        Vector3 pkm1 = ControlPoints[k - 1];
        Vector3 pk   = ControlPoints[k];
        Vector3 pkp1 = ControlPoints[k + 1];
        Vector3 pkp2 = ControlPoints[k + 2];

        Vector3 m0 = (((pkp1 - pk) + (pk - pkm1)) / 2);
        Vector3 m1 = (((pkp2 - pkp1) + (pkp1 - pk)) / 2);

        float h00 = (6 * Mathf.Pow(t, 2f)) - (6 * t);
        float h10 = (3 * Mathf.Pow(t, 2f)) - (2 * t) + 1;
        float h01 = (-(6) * Mathf.Pow(t, 2f)) + (6 * t);
        float h11 = (2 * Mathf.Pow(t, 2f)) - (2 * t);

        return (h00 * pk) + (h10 * m0) + (h01 * pkp1) + (h11 * m1);
    }

    public Vector3 GetSecondDerivative(float t, int k)
    {
        t = Mathf.Clamp01(t);

        Vector3 pkm1 = ControlPoints[k - 1];
        Vector3 pk   = ControlPoints[k];
        Vector3 pkp1 = ControlPoints[k + 1];
        Vector3 pkp2 = ControlPoints[k + 2];

        Vector3 m0 = (((pkp1 - pk) + (pk - pkm1)) / 2);
        Vector3 m1 = (((pkp2 - pkp1) + (pkp1 - pk)) / 2);

        float h00 = (12 * t) - 6;
        float h10 = (6 * t) - 2;
        float h01 = ((-12) * t) + 6;
        float h11 = (4 * t) - 2;

        return (h00 * pk) + (h10 * m0) + (h01 * pkp1) + (h11 * m1);
    }
}