using System;
using UnityEngine;

public class InputManager {

    public static float[] GetKeyboardAxis() {
        float[] _axes = new float[] {
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        };
        return _axes;
    }

    public static bool[] GetKeyboardInput() {
        bool[] _inputs = new bool[] {
            Input.GetKey(KeyCode.Space)
        };
        return _inputs;
    }

}