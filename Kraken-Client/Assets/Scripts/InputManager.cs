using System;
using UnityEngine;

public class InputManager {

<<<<<<< HEAD
    public static bool[] GetKeyboardInput() {
        bool[] _inputs = new bool[] {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
=======
    public static float[] GetKeyboardAxis() {
        float[] _axes = new float[] {
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        };
        return _axes;
    }

    public static bool[] GetKeyboardInput() {
        bool[] _inputs = new bool[] {
>>>>>>> develop
            Input.GetKey(KeyCode.Space)
        };
        return _inputs;
    }

}