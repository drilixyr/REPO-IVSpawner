using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using REPOLib;
using REPOLib.Commands;

namespace YourProjectName
{
    // Plugin entry point
    [BepInPlugin("YourName.YourProjectName", "YourProjectName", "1.0")]
    [BepInDependency(MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class YourProjectNamePlugin : BaseUnityPlugin
    {
        internal static YourProjectNamePlugin Instance { get; private set; } = null!;
        internal static ManualLogSource ModLogger => Instance.logger!;
        private ManualLogSource? logger => base.Logger;

        // Hotkey for toggling UI
        private ConfigEntry<KeyboardShortcut>? toggleUIKey;
        private bool isUIVisible = false;

        // Window size and scroll position
        private Rect windowRect = new Rect(10, 10, 420, 600);
        private Vector2 scrollPosition = Vector2.zero;

        // Search query for filtering items
        private string searchQuery = "";

        // List of Valuable
        private List<string> valuableItems = new List<string>
        {
                        "Valuable Diamond", "Valuable Emerald Bracelet", "Valuable Goblet", "Valuable Ocarina", "Valuable Pocket Watch", "Valuable Uranium Mug", "Valuable Arctic Bonsai", "Valuable Arctic HDD", "Valuable Chomp Book", "Valuable Crown", "Valuable Doll", "Valuable Frog", "Valuable Gem Box", "Valuable Globe", "Valuable Love Potion", "Valuable Money", "Valuable Music Box", "Valuable Toy Monkey", "Valuable Uranium Plate", "Valuable Vase Small", "Valuable Arctic 3D Printer", "Valuable Arctic Laptop", "Valuable Arctic Propane Tank", "Valuable Arctic Sample", "Valuable Arctic Sample Six Pack", "Valuable Bottle", "Valuable Clown", "Valuable Computer", "Valuable Fan", "Valuable Gramophone", "Valuable Marble Table", "Valuable Radio", "Valuable Ship in a bottle", "Valuable Trophy", "Valuable Vase", "Valuable Wizard Goblin Head", "Valuable Wizard Power Crystal", "Valuable Wizard Time Glass", "Valuable Arctic Barrel", "Valuable Arctic Big Sample", "Valuable Creature Leg", "Valuable Arctic Flamethrower", "Valuable Arctic Guitar", "Valuable Arctic Sample Cooler", "Valuable Diamond Display", "Valuable Ice Saw", "Valuable Scream Doll", "Valuable Television", "Valuable Vase Big", "Valuable Wizard Cube of Knowledge", "Valuable Wizard Master Potion", "Valuable Animal Crate", "Valuable Arctic Ice Block", "Valuable Dinosaur", "Valuable Piano", "Valuable Wizard Griffin Statue", "Valuable Arctic Science Station", "Valuable Harp", "Valuable Painting", "Valuable Wizard Dumgolfs Staff", "Valuable Wizard Sword", "Valuable Arctic Server Rack", "Valuable Golden Statue", "Valuable Grandfather Clock", "Valuable Wizard Broom",
        };

        // List of Item
        private List<string> normalItems = new List<string>
        {
            "Item Cart Medium", "Item Cart Small", "Item Drone Battery", "Item Drone Feather", "Item Drone Indestructible", "Item Drone Torque", "Item Drone Zero Gravity", "Item Extraction Tracker", "Item Grenade Duct Taped", "Item Grenade Explosive", "Item Grenade Human", "Item Grenade Shockwave", "Item Grenade Stun", "Item Gun Handgun", "Item Gun Shotgun", "Item Gun Tranq", "Item Health Pack Large", "Item Health Pack Medium", "Item Health Pack Small", "Item Melee Baseball Bat", "Item Melee Frying Pan", "Item Melee Inflatable Hammer", "Item Melee Sledge Hammer", "Item Melee Sword", "Item Mine Explosive", "Item Mine Shockwave", "Item Mine Stun", "Item Orb Zero Gravity", "Item Power Crystal", "Rubber Duck", "Item Upgrade Map Player Count", "Item Upgrade Player Energy", "Item Upgrade Player Extra Jump", "Item Upgrade Player Grab Range", "Item Upgrade Player Grab Strength", "Item Upgrade Player Health", "Item Upgrade Player Sprint Speed", "Item Upgrade Player Tumble Launch", "Item Valuable Tracker",
        };

        // Tracks which tab is active (Valuables or Items)
        private bool isValuablesTabActive = false;

        // Called when the mod is loaded
        private void Awake()
        {
            Instance = this;
            isValuablesTabActive = true;

            gameObject.transform.parent = null;
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            toggleUIKey = Config.Bind("Hotkeys", "ToggleUI", new KeyboardShortcut(KeyCode.F1), "Toggle item spawn menu.");
            ModLogger.LogInfo("YourProjectName mod loaded.");
        }

        // Checks for hotkey press to toggle UI
        private void Update()
        {
            if (toggleUIKey?.Value.IsDown() == true)
                isUIVisible = !isUIVisible;
        }

        // Renders the UI
        private void OnGUI()
        {
            if (!isUIVisible) return;
            windowRect = GUI.Window(0, windowRect, DrawWindow, "YourProjectName Menu");
        }

        // Draws the UI content
        private void DrawWindow(int windowID)
        {
            GUIStyle centeredButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                hover = { textColor = Color.yellow }
            };

            GUILayout.BeginVertical();
            GUILayout.Space(10);

            // Tab buttons to switch between "Valuables" and "Items"
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Valuables", centeredButtonStyle, GUILayout.Width(190), GUILayout.Height(30)))
                isValuablesTabActive = true;
            GUILayout.Space(10);
            if (GUILayout.Button("Items", centeredButtonStyle, GUILayout.Width(190), GUILayout.Height(30)))
                isValuablesTabActive = false;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Search bar for filtering items
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchQuery = GUILayout.TextField(searchQuery, GUILayout.Width(320));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Scrollable list of items based on the active tab
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(480));

            List<string> itemsToDisplay = isValuablesTabActive ? valuableItems : normalItems;

            foreach (var item in itemsToDisplay)
            {
                if (!string.IsNullOrEmpty(searchQuery) && !item.ToLower().Contains(searchQuery.ToLower()))
                    continue;

                if (GUILayout.Button(item, centeredButtonStyle, GUILayout.Width(370)))
                {
                    TrySpawnItem(item);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        // Attempts to spawn the item
        private void TrySpawnItem(string itemName)
        {
            GameObject? prefab = FindItemPrefab(itemName);

            if (prefab == null)
            {
                ModLogger.LogError("Item prefab not found: " + itemName);
                return;
            }

            GameObject player = GameObject.FindObjectOfType<PlayerController>()?.gameObject!;
            if (player == null)
            {
                ModLogger.LogError("No player found.");
                return;
            }

            Vector3 spawnPosition = player.transform.position + player.transform.forward * 2f;
            GameObject spawnedItem = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedItem.name = itemName + "_Clone";

            ModLogger.LogInfo($"Spawned item: {spawnedItem.name}");
        }

        // Finds the prefab of an item by name
        private GameObject? FindItemPrefab(string itemName)
        {
            var allPrefabs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in allPrefabs)
            {
                if (go.name == itemName)
                    return go;
            }
            return null;
        }
    }
}
