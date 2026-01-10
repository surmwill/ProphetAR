using Swill.Recycler;
using UnityEngine;

namespace ProphetAR
{
    public class TestRecyclerUIData : IRecyclerScrollRectData<string>
    { 
        public string Key { get; }
        
        public Color Color { get; }

        public TestRecyclerUIData(string key, Color color)
        {
            Key = key;
            Color = color;
        }
    }
}