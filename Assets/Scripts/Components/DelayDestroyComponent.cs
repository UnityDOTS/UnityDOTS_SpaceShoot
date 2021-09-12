using Unity.Entities;

namespace DOTS
{
    [GenerateAuthoringComponent]
    public struct DelayDestroyComponent : IComponentData
    {
        public float delay;
    }
}