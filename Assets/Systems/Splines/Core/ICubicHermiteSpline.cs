using UnityEngine;

public interface ICubicHermiteSpline
{
    Vector3 GetPoint(float t, int k);
    Vector3 GetFirstDerivative(float t, int k);
    Vector3 GetSecondDerivative(float t, int k);
}