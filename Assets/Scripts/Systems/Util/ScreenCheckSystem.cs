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
                if (translation.Value.z < GameManager.Instance.BoundaryBottomLeft.z || translation.Value.z > GameManager.Instance.BoundaryTopRight.z)
                {
                    EntityManager.DestroyEntity(e);
                }
            }).WithStructuralChanges().Run();

            Entities.ForEach((Entity e, in Translation translation, in AsteroidComponent asteroidComponent) =>
            {
                if (translation.Value.z < GameManager.Instance.BoundaryBottomLeft.z)
                {
                    EntityManager.DestroyEntity(e);
                }
            }).WithStructuralChanges().Run();

            Entities.ForEach((Entity e, in Translation translation, in EnemyComponent enemyComponent) =>
            {
                if (translation.Value.z < GameManager.Instance.BoundaryBottomLeft.z)
                {
                    EntityManager.DestroyEntity(e);
                }
            }).WithStructuralChanges().Run();
        }
    }
}