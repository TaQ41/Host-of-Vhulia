using System.Threading.Tasks;
using UnityEngine;

namespace LocaleSetupProcesses
{
    public abstract class LocaleSetupProcessPart : MonoBehaviour
    {
        public enum LoadProcessParts
        {
            None = 0,
            Player = 1,
            Missions = 2,
            All = Player | Missions
        };

        [SerializeField] protected LoadProcessParts m_loadProcessParts;
        public LoadProcessParts GetLoadProcessParts() => m_loadProcessParts;

        public abstract Task BeginSetup(System.Action callback);
    }
}
