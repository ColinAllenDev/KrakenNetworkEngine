using System;
using UnityEngine;

public class InputManager {

    public static bool[] GetKeyboardInput() {
        bool[] _inputs = new bool[] {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };
        return _inputs;
    }

}