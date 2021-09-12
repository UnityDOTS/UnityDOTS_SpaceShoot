using Unity.Entities;

namespace DOTS
{
    /// <summary>
    /// 行星组件
    /// </summary>
    public struct AsteroidComponent : IComponentData
    {
        public Entity DestructionParticle; // 爆炸粒子
    }
}