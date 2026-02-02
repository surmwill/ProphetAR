using System;

namespace ProphetAR
{
    public static class ARObjectSelectionUIOptionDataFactory
    {
        public static string NextRandomKey => Guid.NewGuid().ToString();
        
        public static ARObjectSelectionUIOptionData Cancel(Action onCancel, string name = "Cancel")
        {
            return Cancel(NextRandomKey, name, onCancel);
        }
        
        public static ARObjectSelectionUIOptionData Cancel(string customKey, string name, Action onCancel)
        {
            return new ARObjectSelectionUIOptionData(customKey, name, onCancel, null, true);
        }
        
        public static ARObjectSelectionUIOptionData Default(string name, Action onSelect)
        {
            return Default(NextRandomKey, name, onSelect);
        }
        
        public static ARObjectSelectionUIOptionData Default(string customKey, string name, Action onSelect, Func<bool> isEnabled = null)
        {
            return new ARObjectSelectionUIOptionData(customKey, name, onSelect, isEnabled);
        }
    }
}