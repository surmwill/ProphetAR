using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace ProphetAR
{
    public static class CustomMenu
    {
        private const string CustomMenuRootDir = "ProphetAR/";

        private const string MenuItemPrintGameEvents = CustomMenuRootDir + "Print Game Events";
        
        [MenuItem(MenuItemPrintGameEvents)]
        public static void TogglePrintGameEvents()
        {
            int curr = PlayerPrefs.GetInt(GameEventProcessor.DebugPrintGameEventsPlayerPrefKey);
            PlayerPrefs.SetInt(GameEventProcessor.DebugPrintGameEventsPlayerPrefKey, curr != 0 ? 0 : 1);
            PlayerPrefs.Save();
        }

        [MenuItem(MenuItemPrintGameEvents, true)]
        public static bool TogglePrintGameEventsValidate()
        {
            Menu.SetChecked(MenuItemPrintGameEvents, PlayerPrefs.GetInt(GameEventProcessor.DebugPrintGameEventsPlayerPrefKey) != 0);
            return true;
        }
    }
}

#endif