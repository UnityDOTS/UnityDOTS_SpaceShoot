using Unity.Entities;

namespace DOTS
{
    public struct EnemyComponent : IComponentData
    {
        public float ChangeDestinationCountdown; //调整目的点倒计时
        public Entity DestructionParticle; // 爆炸粒子
    }
}