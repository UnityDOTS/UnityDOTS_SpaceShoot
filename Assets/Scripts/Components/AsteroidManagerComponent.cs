using Unity.Entities;

namespace DOTS
{
    [GenerateAuthoringComponent]
    public struct AsteroidManagerComponent : IComponentData
    {
        public Entity AsteroidPrefab_01;
        public Entity AsteroidPrefab_02;
        public Entity AsteroidPrefab_03;

        public Entity DestructionParticle;
    }
}