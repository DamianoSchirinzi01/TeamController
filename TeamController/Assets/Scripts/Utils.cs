using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float lerpValue(float _from, float _to, float _duration, bool _incrementValue)
    {
        float newLerp = Mathf.Lerp(_from, _to, _duration);
        if (_incrementValue)
        {
            if (newLerp >= _to - 0.05f)
            {
                newLerp = _to;
            }
        }
        else
        {
            if (newLerp <= _to + 0.05f)
            {
                newLerp = _to;
            }
        }

        return newLerp;
    }
}
