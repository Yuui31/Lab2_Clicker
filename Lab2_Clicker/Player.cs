using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Clicker
{
    public class Player
    {
        // ===== ПОЛЯ =====
        private int lvl;
        private BigNumber gold;
        private BigNumber damage;
        private double damageModifier;
        private BigNumber upgradeCost;
        private double upgradeModifier;

        // ===== СВОЙСТВА (get публичный, set приватный) =====
        public int Lvl { get { return lvl; } }
        public BigNumber Gold { get { return gold; } }
        public BigNumber Damage { get { return damage; } }
        public double DamageModifier { get { return damageModifier; } }
        public BigNumber UpgradeCost { get { return upgradeCost; } }
        public double UpgradeModifier { get { return upgradeModifier; } }

        // ===== КОНСТРУКТОР =====
        public Player()
        {
            lvl = 1;
            gold = new BigNumber(0);
            damage = new BigNumber(1);
            damageModifier = 1.2;
            upgradeCost = new BigNumber(10);
            upgradeModifier = 1.5;
        }

        // ===== ПУБЛИЧНЫЕ МЕТОДЫ =====

        // Добавить золото игроку (после победы над врагом)
        public void AddGold(BigNumber amount)
        {
            gold = gold + amount;
        }

        // Попытка улучшения: если хватает золота — улучшаем, иначе false
        public bool TryUpgrade()
        {
            if (gold < upgradeCost)
                return false;

            // Списываем золото
            gold = gold - upgradeCost;

            // Увеличиваем уровень
            lvl++;

            // Пересчитываем характеристики
            RecalculateStats();

            return true;
        }

        // Урон за один клик
        public BigNumber DealDamage()
        {
            return damage.Clone();
        }

        // ===== ПРИВАТНЫЕ МЕТОДЫ =====

        private void RecalculateStats()
        {
            // Урон растёт на damageModifier
            damage = damage * damageModifier;

            // Стоимость следующего апгрейда
            upgradeCost = CalculateNextUpgradeCost();
        }

        private BigNumber CalculateNextUpgradeCost()
        {
            // upgradeCost = upgradeCost * (upgradeModifier * lvl)
            double multiplier = upgradeModifier * lvl;
            return upgradeCost * multiplier;
        }

        private bool TrySpendGold(BigNumber amount)
        {
            if (gold < amount)
                return false;
            gold = gold - amount;
            return true;
        }
    }
}