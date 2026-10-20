using System;
using System.Collections.Generic;
using System.Text;
using Lab3_ClickOrLoose;

namespace Lab2_Clicker
{
    public class Player
    {
        private int lvl;
        private BigNumber gold;
        private BigNumber damage;
        private double damageModifier;

        private BigNumber upgradeCost;
        private double upgradeModifier;

        // НОВОЕ: Перезарядка
        private CCountdownTimer clickCooldown;
        private BigNumber cooldownUpgradeCost;

        public int Lvl { get { return lvl; } }
        public BigNumber Gold { get { return gold; } }
        public BigNumber Damage { get { return damage; } }
        public double DamageModifier { get { return damageModifier; } }
        public BigNumber UpgradeCost { get { return upgradeCost; } }
        public double UpgradeModifier { get { return upgradeModifier; } }

        public BigNumber CooldownUpgradeCost { get { return cooldownUpgradeCost; } }
        public bool IsReadyToClick { get { return clickCooldown.IsReady(); } }
        public double CurrentCooldown { get { return clickCooldown.GetTargetTime(); } }

        public Player()
        {
            lvl = 1;
            gold = new BigNumber(0);
            damage = new BigNumber(1); // Чтобы урон сразу рос (как мы чинили)
            damageModifier = 1.5;

            upgradeCost = new BigNumber(1);
            upgradeModifier = 1.5;

            // Перезарядка: старт 0.5 секунды, улучшение стоит 50 золота
            clickCooldown = new CCountdownTimer(0.25);
            cooldownUpgradeCost = new BigNumber(50);
        }

        public void Update(double delta)
        {
            clickCooldown.Update(delta); // Тикаем таймер перезарядки
        }

        public void AddGold(BigNumber amount)
        {
            gold = gold + amount;
        }

        public bool TryUpgradeDamage()
        {
            if (gold < upgradeCost) return false;
            gold = gold - upgradeCost;
            lvl++;
            damage = damage * damageModifier;
            upgradeCost = upgradeCost * (upgradeModifier * lvl);
            return true;
        }

        // НОВОЕ: Улучшение перезарядки
        public bool TryUpgradeCooldown()
        {
            if (gold < cooldownUpgradeCost) return false;
            gold = gold - cooldownUpgradeCost;

            // Уменьшаем перезарядку на 0.05 сек (но не меньше 0.1)
            clickCooldown.DecreaseTargetTime(0.05);

            // Следующее улучшение дороже в 2 раза
            cooldownUpgradeCost = cooldownUpgradeCost * 2.0;
            return true;
        }

        // Вызывать при клике
        public BigNumber DealDamage()
        {
            clickCooldown.Reset(); // Запускаем перезарядку
            return damage.Clone();
        }
    }
}