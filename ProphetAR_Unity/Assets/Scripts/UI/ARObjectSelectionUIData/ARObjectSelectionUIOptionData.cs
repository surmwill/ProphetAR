using System;

namespace ProphetAR
{
    public class ARObjectSelectionUIOptionData
    { 
        public string Key { get; }
        
        public string Name { get; }
        
        public Action OnSelect { get; }
        
        public Func<bool> IsEnabled { get; }
        
        public bool IsCancelOption { get; }

        public ARObjectSelectionUIOptionData(string key, string name, Action onSelect, Func<bool> isEnabled, bool isCancelOption = false)
        {
            Key = key;
            Name = name;
            OnSelect = onSelect;
            IsCancelOption = isCancelOption;
            IsEnabled = isEnabled;
        }
    }
}