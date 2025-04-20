using HarmonyLib;
using UnityEngine;
using YourProjectName; // Assurez-vous que ce using est présent pour accéder à YourProjectNamePlugin

namespace YourProjectName
{
    [HarmonyPatch(typeof(PlayerController))]
    static class ExamplePlayerControllerPatch
    {
        [HarmonyPrefix, HarmonyPatch(nameof(PlayerController.Start))]
        private static void Start_Prefix(PlayerController __instance)
        {
            // Code to execute for each PlayerController *before* Start() is called.
            YourProjectNamePlugin.ModLogger.LogInfo($"{__instance} Start Prefix"); // Accéder à ModLogger via YourProjectNamePlugin
        }

        [HarmonyPostfix, HarmonyPatch(nameof(PlayerController.Start))]
        private static void Start_Postfix(PlayerController __instance)
        {
            // Code to execute for each PlayerController *after* Start() is called.
            YourProjectNamePlugin.ModLogger.LogInfo($"{__instance} Start Postfix"); // Accéder à ModLogger via YourProjectNamePlugin
        }
    }
}
