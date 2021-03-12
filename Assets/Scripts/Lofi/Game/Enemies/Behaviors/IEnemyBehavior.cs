

namespace Lofi.Game
{
    public interface IEnemyBehavior
    {
        public void DecideNextMove(Enemy self);
        public void AddAbilites(Enemy self);
    }
}