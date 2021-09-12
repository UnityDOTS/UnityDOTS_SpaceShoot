using Unity.Entities;
using Unity.Transforms;

namespace DOTS
{
    /// <summary>
    /// 屏幕边界位置交换
    /// </summary>
    public class ScreenCheckSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, in Translation translation, in BulletComponent bulletComponent) =>
            {
                if (translation.Value.z < -4f || translation.Value.z > 14f)
                {
                    EntityManager.DestroyEntity(e);
                    // EntityManager.AddComponentData(e, new DestroyComponent());
                }
            }).WithStructuralChanges().Run();

            Entities.ForEach((Entity e, in Translation translation, in AsteroidComponent asteroidComponent) =>
            {
                if (translation.Value.z < -6f)
                {
                    EntityManager.DestroyEntity(e);
                    // EntityManager.AddComponentData(e, new DestroyComponent());
                }
            }).WithStructuralChanges().Run();

            Entities.ForEach((Entity e, in Translation translation, in EnemyComponent enemyComponent) =>
            {
                if (translation.Value.z < -6f)
                {
                    EntityManager.DestroyEntity(e);
                    // EntityManager.AddComponentData(e, new DestroyComponent());
                }
            }).WithStructuralChanges().Run();
        }
    }
}