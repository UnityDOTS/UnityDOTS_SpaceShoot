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
            Entities
                .WithStructuralChanges()
                .ForEach((Entity e, in Translation translation, in BulletComponent bulletComponent) =>
                {
                    if (translation.Value.z < GameManager.BoundaryBottomLeft.z || translation.Value.z > GameManager.BoundaryTopRight.z)
                    {
                        EntityManager.DestroyEntity(e);
                    }
                }).Run();

            Entities
                .WithStructuralChanges()
                .ForEach((Entity e, in Translation translation, in AsteroidComponent asteroidComponent) =>
                {
                    if (translation.Value.z < GameManager.BoundaryBottomLeft.z)
                    {
                        EntityManager.DestroyEntity(e);
                    }
                }).Run();

            Entities
                .WithStructuralChanges()
                .ForEach((Entity e, in Translation translation, in EnemyComponent enemyComponent) =>
                {
                    if (translation.Value.z < GameManager.BoundaryBottomLeft.z)
                    {
                        EntityManager.DestroyEntity(e);
                    }
                }).Run();
        }
    }
}