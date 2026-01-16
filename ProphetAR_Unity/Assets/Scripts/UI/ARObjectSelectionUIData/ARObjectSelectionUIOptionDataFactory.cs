using System;

namespace ProphetAR
{
    public static class ARObjectSelectionUIOptionDataFactory
    {
        public static ARObjectSelectionUIOptionData Cancel(string customKey, Action onCancel)
        {
            return new ARObjectSelectionUIOptionData(customKey, onCancel, null, true);
        }
        
        public static ARObjectSelectionUIOptionData Cancel(Action onCancel)
        {
            return new ARObjectSelectionUIOptionData(Guid.NewGuid().ToString(), onCancel, null, true);
        }
        
        public static ARObjectSelectionUIOptionData Default(string customKey, Action onSelect, Func<bool> isEnabled = null)
        {
            return new ARObjectSelectionUIOptionData(customKey, onSelect, isEnabled, false);
        }
        
        public static ARObjectSelectionUIOptionData Default(Action onSelect, Func<bool> isEnabled = null)
        {
            return new ARObjectSelectionUIOptionData(Guid.NewGuid().ToString(), onSelect, isEnabled, false);
        }
    }
}