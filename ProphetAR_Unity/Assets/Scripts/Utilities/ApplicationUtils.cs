using UnityEngine;

namespace ProphetAR
{
    public static class ApplicationUtils
    {
        public static bool IsEditMode => Application.isEditor && !Application.isPlaying;
    }
}