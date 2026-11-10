using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace Lab2_Clicker
{
    // Базовый класс, чтобы не дублировать код
    public abstract class CBaseEnemy : IEnemy
    {
        protected string name;
        protected BigNumber maxHitPoints;
        protected BigNumber currentHitPoints;
        protected BigNumber goldReward;
        protected bool isDead;
        protected string iconPath;

        public string Name => name;
        public BigNumber MaxHitPoints => maxHitPoints;
        public BigNumber CurrentHitPoints => currentHitPoints;
        public BigNumber GoldReward => goldReward;
        public bool IsDead => isDead;
        public string IconPath => iconPath;

        public abstract bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut);
    }

    // 1. Обычный
    public class CNormalEnemy : CBaseEnemy
    {
        public CNormalEnemy(string name, BigNumber hp, BigNumber gold, string icon) 
        { this.name = name; maxHitPoints = hp; currentHitPoints = hp.Clone(); goldReward = gold; iconPath = icon; }

        public override bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut)
        {
            goldRewardOut = null;
            if (isDead) return false;
            currentHitPoints = currentHitPoints - dmg;
            if (currentHitPoints < new BigNumber(0)) currentHitPoints = new BigNumber(0);
            if (currentHitPoints.ToString() == "0") { isDead = true; goldRewardOut = goldReward.Clone(); return true; }
            return false;
        }
    }

    // 2. Бронированный (урон уменьшается вдвое)
    public class CArmoredEnemy : CBaseEnemy
    {
        public CArmoredEnemy(string name, BigNumber hp, BigNumber gold, string icon) 
        { this.name = name; maxHitPoints = hp; currentHitPoints = hp.Clone(); goldReward = gold; iconPath = icon; }

        public override bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut)
        {
            goldRewardOut = null;
            if (isDead) return false;

            // Урон делится на 2 (защита)
            BigNumber reducedDmg = dmg / 2.0;

            // Если урон округлился до 0, делаем минимум 1, чтобы врага можно было убить
            if (reducedDmg.ToString() == "0") reducedDmg = new BigNumber(1);

            currentHitPoints = currentHitPoints - reducedDmg;

            if (currentHitPoints < new BigNumber(0)) currentHitPoints = new BigNumber(0);
            if (currentHitPoints.ToString() == "0") { isDead = true; goldRewardOut = goldReward.Clone(); return true; }
            return false;
        }
    }

    // 3. Лечащийся (лечится на 10% каждый раз, когда получает урон)
    public class CHealingEnemy : CBaseEnemy
    {
        public CHealingEnemy(string name, BigNumber hp, BigNumber gold, string icon) 
        { this.name = name; maxHitPoints = hp; currentHitPoints = hp.Clone(); goldReward = gold; iconPath = icon; }

        public override bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut)
        {
            goldRewardOut = null;
            if (isDead) return false;

            currentHitPoints = currentHitPoints - dmg;

            if (currentHitPoints.ToString() == "0")
            {
                isDead = true;
                goldRewardOut = goldReward.Clone();
                return true;
            }

            // Лечится на 10% от ПОЛУЧЕННОГО урона (а не от максимума)
            BigNumber heal = dmg / 10.0;
            currentHitPoints = currentHitPoints + heal;

            if (currentHitPoints > maxHitPoints) currentHitPoints = maxHitPoints.Clone();

            return false;
        }
    }
}