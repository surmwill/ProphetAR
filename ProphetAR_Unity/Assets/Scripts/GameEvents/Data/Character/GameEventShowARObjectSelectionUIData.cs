using System;
using System.Collections.Generic;

namespace ProphetAR
{
    public class GameEventShowARObjectSelectionUIData
    {
        public List<Action> OnOptionSelected { get; }

        public GameEventShowARObjectSelectionUIData(List<Action> onOptionSelected)
        {
            OnOptionSelected = onOptionSelected;
        }
    }
}