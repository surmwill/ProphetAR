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

        private const int NumEntries = 50;

        private void Start()
        {
            _testRecycler.AppendEntries(
                Enumerable.Repeat(new object(), NumEntries).Select(_ => new TestRecyclerUIData(Guid.NewGuid().ToString(), Random.ColorHSV())));
        }
    }
}