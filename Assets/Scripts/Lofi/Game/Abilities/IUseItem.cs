
using UnityEngine;

namespace Lofi.Game
{
    public interface IUseItem
    {
        public bool UseItemWithDirection(Vector3 direction);

        public string GetName();
    }
}