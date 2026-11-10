using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Clicker
{
    public interface IEnemy
    {
        string Name { get; }
        BigNumber CurrentHitPoints { get; }
        BigNumber MaxHitPoints { get; }
        BigNumber GoldReward { get; }
        bool IsDead { get; }
        bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut);
    }
}