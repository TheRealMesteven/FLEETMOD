using System;
using HarmonyLib;
using UnityEngine.UI;

namespace FLEETMOD.Fixes
{
    [HarmonyPatch(typeof(PLGlobal), "SafeLabelSetText", new Type[]
    {
        typeof(Text),
        typeof(string)
    })]
    internal class SafeLabelSetText
    {
        public static bool Prefix(ref Text go, ref string text)
        {
            if (!MyVariables.isrunningmod) return true;
            if (go != null && go.text != text && text != null)
            {
                if (text.Contains("*"))
                {
                    return false;
                }
                go.text = text;
            }
            return false;
        }
    }
}
