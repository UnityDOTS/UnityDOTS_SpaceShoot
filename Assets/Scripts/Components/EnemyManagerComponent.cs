using Unity.Entities;

namespace DOTS
{
    [GenerateAuthoringComponent]
    public struct EnemyManagerComponent : IComponentData
    {
        public Entity EnmeyPrefab_01;
        
        public Entity DestructionParticle;
    }
}