namespace VampireSurvivor.Core
{
    /// <summary>
    /// Interface for objects that can be pooled and reused
    /// </summary>
    public interface IPoolable
    {
        void OnSpawnFromPool();
        void OnReturnToPool();
    }
}
