

namespace Lofi.Game
{
    public struct EnemyTemplate 
    {
        public string Name;
        public string Sprite;
        public int Health;
        public int Damage;
        public string Habitat;
        public IEnemyBehavior Behavior;

        public EnemyTemplate(string name, string sprite, int health, int damage, string habitat = "ALL", IEnemyBehavior behavior = null)
        {
            Name = name;
            Sprite = sprite;
            Health = health;
            Damage = damage;
            Habitat = habitat;
            Behavior = behavior;
        }
    }
}