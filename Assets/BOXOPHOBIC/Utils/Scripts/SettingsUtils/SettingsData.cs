// Cristian Pop - https://boxophobic.com/

using Boxophobic.StyledGUI;
using UnityEngine;

namespace Boxophobic.Utils
{
    [CreateAssetMenu(fileName = "Data", menuName = "BOXOPHOBIC/Settings Data")]
    public class SettingsData : StyledScriptableObject
    {
        [StyledBanner(0.65f, 0.65f, 0.65f, "Settings Data")]
        public bool styledBanner;

        [Space]
        public string data = "";
    }
}