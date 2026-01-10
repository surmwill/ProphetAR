using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProphetAR
{
    public class TestTestRecyclerUI : MonoBehaviour
    {
        [SerializeField]
        private TestRecyclerUI _testRecycler = null;

        public const int SizeSmallEntry = 300;
        public const int SizeLargeEntry = 500;
        
        private const int NumEntries = 50;

        private void Start()
        {
            _testRecycler.AppendEntries(
                Enumerable.Repeat(new object(), NumEntries).Select((_, index) => new TestRecyclerUIData(
                    Guid.NewGuid().ToString(), Random.ColorHSV(), (index & 1) == 1)));
        }
    }
}