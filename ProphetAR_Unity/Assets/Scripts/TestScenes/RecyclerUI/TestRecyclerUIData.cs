using Swill.Recycler;
using UnityEngine;

namespace ProphetAR
{
    public class TestRecyclerUIData : IRecyclerScrollRectData<string>
    { 
        public string Key { get; }
        
        public Color Color { get; }
        
        public bool IsLarge { get; }

        public TestRecyclerUIData(string key, Color color, bool isLarge)
        {
            Key = key;
            Color = color;
            IsLarge = isLarge;
        }
    }
}