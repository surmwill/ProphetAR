using System;

namespace ProphetAR
{
    public class ARObjectSelectionUIOptionData
    { 
        public Action OnSelect { get; }
        
        public Func<bool> IsEnabled { get; }
        
        public string Key { get; }
        
        public bool IsCancelOption { get; }

        public ARObjectSelectionUIOptionData(string key, Action onSelect, Func<bool> isEnabled, bool isCancelOption = false)
        {
            Key = key;
            OnSelect = onSelect;
            IsCancelOption = isCancelOption;
            IsEnabled = isEnabled;
        }
    }
}