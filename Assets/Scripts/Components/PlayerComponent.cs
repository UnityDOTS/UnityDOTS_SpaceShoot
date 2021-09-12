using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerComponent : IComponentData
    {
        public int Score;
        public int Life;
        public bool IsTouch;
        public float3 Destination;
        public Entity DestructionParticle; // 爆炸粒子
    }
}