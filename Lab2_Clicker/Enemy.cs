using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Clicker
{
    public class Enemy
    {
        // ===== ПОЛЯ =====
        private string name;
        private BigNumber maxHitPoints;
        private BigNumber currentHitPoints;
        private BigNumber goldReward;
        private bool isDead;
        private string iconPath;   // пока сохраним путь к картинке

        // ===== СВОЙСТВА (только get) =====
        public string Name { get { return name; } }
        public BigNumber MaxHitPoints { get { return maxHitPoints; } }
        public BigNumber CurrentHitPoints { get { return currentHitPoints; } }
        public BigNumber GoldReward { get { return goldReward; } }
        public bool IsDead { get { return isDead; } }
        public string IconPath { get { return iconPath; } }

        // ===== КОНСТРУКТОР =====
        public Enemy(string name, BigNumber maxHP, BigNumber reward, string iconPath)
        {
            this.name = name;
            this.maxHitPoints = maxHP;
            this.currentHitPoints = maxHP.Clone();   // текущее = максимум
            this.goldReward = reward;
            this.iconPath = iconPath;
            this.isDead = false;
        }

        // ===== ПУБЛИЧНЫЕ МЕТОДЫ =====

        // Урон по врагу. Возвращает true, если враг побеждён этой атакой.
        // В out-параметр goldRewardOut пишется награда при победе.
        public bool TakeDamage(BigNumber dmg, out BigNumber goldRewardOut)
        {
            goldRewardOut = null;

            if (isDead)
                return false;

            // Проверяем: урон больше или равен текущему HP?
            if (currentHitPoints < dmg)
            {
                // Враг умирает
                currentHitPoints = new BigNumber(0);
                Die();
                goldRewardOut = goldReward.Clone();
                return true;
            }

            // Иначе — просто отнимаем урон
            currentHitPoints = currentHitPoints - dmg;
            return false;
        }

        // ===== ПРИВАТНЫЕ МЕТОДЫ =====
        private void Die()
        {
            isDead = true;
        }
    }
}