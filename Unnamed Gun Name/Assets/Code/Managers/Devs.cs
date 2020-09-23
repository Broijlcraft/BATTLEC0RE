using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Devs {
    public static string[] devs = new string[] {"dev", "max", "kubi" };

    public static bool IsDev(string name) {
        bool isDev = false;
        for (int i = 0; i < devs.Length; i++) {
            name = name.ToLower();
            if (name.Contains(devs[i])) {
                isDev = true;
            }
        }
        return isDev;
    }
}