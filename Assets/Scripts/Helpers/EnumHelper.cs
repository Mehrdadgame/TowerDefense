using UnityEngine;

namespace Helpers
{
    // EnumHelper class to provide static access to TowerType enum
    // This is used in multiple components to avoid circular dependencies
    public enum TowerType
    {
        Basic,
        Heavy,
        Fast
    }
    public enum EnemyType
    {
        Basic,
        Fast,
        Heavy
    }

}

