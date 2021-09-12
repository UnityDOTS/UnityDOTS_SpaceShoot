using Unity.Entities;

namespace DOTS
{
    public struct BulletComponent : IComponentData
    {
        public bool IsFromPlayer;
    }
}