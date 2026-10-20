using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Lab2_Clicker
{
    public partial class MainWindow : Window
    {
        private Player player;
        private Enemy currentEnemy;
        private CEnemyTemplateList templates;
        private Random rnd = new Random();

        // НОВОЕ: Таймеры
        private DispatcherTimer gameTimer;
        private double spawnTimer = 0;
        private double effectTimer = 0;
        private double damageMultiplier = 1.0; // Бафф урона
        private double cooldownMultiplier = 1.0; // Бафф КД

        public MainWindow()
        {
            InitializeComponent();

            player = new Player();
            templates = new CEnemyTemplateList();

            LoadTemplatesFromJson();
            SpawnNextEnemy();
            UpdateUI();

            // Игровой цикл (тик каждые 100 мс)
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        // Игровой цикл: обновляет кулдаун и спавнит сферы
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            player.Update(0.1); // Обновляем перезарядку

            // Спавн бонусных сфер каждые 3 секунды
            spawnTimer += 0.1;
            if (spawnTimer >= 3.0 && currentEnemy != null && !currentEnemy.IsDead)
            {
                SpawnBonusSphere();
                spawnTimer = 0;
            }

            // Таймер действия баффов (длительность 5 сек)
            if (effectTimer > 0)
            {
                effectTimer -= 0.1;
                if (effectTimer <= 0)
                {
                    damageMultiplier = 1.0;
                    cooldownMultiplier = 1.0;
                    UpdateUI();
                }
            }

            UpdateUI();
        }

        // Создаем сферу поверх монстра
        private void SpawnBonusSphere()
        {
            // Теперь они спавнятся по всей ширине экрана, а не только справа
            double x = rnd.Next(50, 1000); // Твой экран 1100 в ширину
            double y = rnd.Next(50, 500);
            Point pos = new Point(x, y);

            CCollectable sphere;
            int type = rnd.Next(0, 3);

            if (type == 0) sphere = new DamageBonus(pos);
            else if (type == 1) sphere = new CooldownBonus(pos);
            else sphere = new GoldBonus(pos);

            var sprite = sphere.GetSprite();

            // Исправленный клик по сфере:
            sprite.MouseLeftButtonDown += (s, ev) =>
            {
                sphere.ApplyEffect(player, this);
                MainGrid.Children.Remove(sprite); // Удаляем сферу
                ev.Handled = true; // Останавливаем клик, чтобы он не бил монстра
            };

            MainGrid.Children.Add(sprite);
        }

        // --- МЕТОДЫ БАФФОВ (вызываются из сфер) ---
        public void ActivateDamageBuff()
        {
            damageMultiplier = 2.0; // Урон х2
            effectTimer = 5.0; // На 5 секунд
            MessageBox.Show("Бафф урона! Урон x2 на 5 сек");
        }

        public void ActivateCooldownBuff()
        {
            cooldownMultiplier = 0.5; // Перезарядка быстрее
            effectTimer = 5.0;
            MessageBox.Show("Бафф скорости! КД снижен на 5 сек");
        }

        // --- ОСТАЛЬНАЯ ЛОГИКА (КЛИКИ ПО ВРАГУ И АПГРЕЙДЫ) ---

        private void EnemyIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy == null || currentEnemy.IsDead) return;

            // ПРОВЕРКА КУЛДАУНА!
            if (!player.IsReadyToClick) return;

            // Считаем урон с учетом баффа
            BigNumber baseDamage = player.DealDamage();
            BigNumber finalDamage = baseDamage * damageMultiplier; // Умножаем на бафф

            bool killed = currentEnemy.TakeDamage(finalDamage, out BigNumber reward);

            if (killed)
            {
                player.AddGold(reward);
                SpawnNextEnemy();
            }

            UpdateUI();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeDamage()) UpdateUI();
            else MessageBox.Show("Недостаточно золота!");
        }

        private void CooldownUpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeCooldown()) UpdateUI();
            else MessageBox.Show("Недостаточно золота!");
        }

        // --- UI И ЗАГРУЗКА ---
        private void UpdateUI()
        {
            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = (player.Damage * damageMultiplier).ToString() + (damageMultiplier > 1 ? " (БАФФ)" : "");
            UpgradeCostText.Text = $"Стоимость: {player.UpgradeCost}";

            CooldownCostText.Text = $"Стоимость: {player.CooldownUpgradeCost}";
            CooldownInfoText.Text = $"КД: {player.CurrentCooldown * cooldownMultiplier:F2}с";

            if (currentEnemy != null)
            {
                EnemyNameText.Text = currentEnemy.Name;
                EnemyHpText.Text = $"{currentEnemy.CurrentHitPoints} / {currentEnemy.MaxHitPoints}";
                EnemyRewardText.Text = currentEnemy.GoldReward.ToString();
            }
        }

        private void LoadTemplatesFromJson()
        {
            string path = "enemies.json";
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<System.Collections.Generic.List<CEnemyTemplate>>(json);
            if (loaded != null)
            {
                foreach (var t in loaded)
                    templates.AddEnemy(t.Name, t.IconName, t.BaseLife, t.LifeModifier, t.BaseGold, t.GoldModifier, t.SpawnChance);
                templates.NormalizeChances();
            }
        }

        private void SpawnNextEnemy()
        {
            if (templates.GetListOfEnemyNames().Count == 0) return;

            var template = templates.FindByChance(rnd.NextDouble()) ?? templates.GetEnemyByIndex(0);

            BigNumber hp = new BigNumber(template.BaseLife);
            for (int i = 0; i < player.Lvl - 1; i++) hp = hp * template.LifeModifier;

            BigNumber gold = new BigNumber(template.BaseGold);
            for (int i = 0; i < player.Lvl - 1; i++) gold = gold * template.GoldModifier;

            // Ищем путь к иконке (как было в старой версии)
            string iconPath = FindIconPath(template.IconName);

            currentEnemy = new Enemy(template.Name, hp, gold, iconPath);

            // Показываем иконку
            try
            {
                if (!string.IsNullOrEmpty(iconPath))
                    EnemyIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                else
                    EnemyIcon.Source = null;
            }
            catch
            {
                EnemyIcon.Source = null;
            }
        }

        // ЭТОТ МЕТОД ТОЖЕ НУЖНО ВЕРНУТЬ (добавь его рядом со SpawnNextEnemy)
        private string FindIconPath(string iconName)
        {
            if (string.IsNullOrEmpty(iconName)) return "";
            string iconsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons");
            if (Directory.Exists(iconsDir))
            {
                string fullPath = Path.Combine(iconsDir, iconName);
                if (File.Exists(fullPath)) return fullPath;
            }
            return "";
        }
    }
}