using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Devs {
    public static string[] devs = new string[] { "Dev", "dev" };

    public static bool IsDev(string name) {
        Debug.Log(name);
        bool isDev = false;
        for (int i = 0; i < devs.Length; i++) {
            if (name.Contains(devs[i])) {
            Debug.Log(devs[i]);
                isDev = true;
            }
        }
        return isDev;
    }
}