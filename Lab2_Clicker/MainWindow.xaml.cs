using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lab2_Clicker
{
    public partial class MainWindow : Window
    {
        // ===== ИГРОВЫЕ ОБЪЕКТЫ =====
        private Player player;
        private Enemy currentEnemy;
        private CEnemyTemplateList templates;
        private Random rnd = new Random();

        // ===== СТАТИСТИКА =====
        private int killedCount = 0;
        private BigNumber totalDamage = new BigNumber(0);
        private BigNumber totalGold = new BigNumber(0);

        public MainWindow()
        {
            InitializeComponent();

            player = new Player();
            templates = new CEnemyTemplateList();

            LoadTemplatesFromJson();
            SpawnNextEnemy();
            UpdateUI();
        }

        // ===== ЗАГРУЗКА ШАБЛОНОВ ИЗ JSON =====
        private void LoadTemplatesFromJson()
        {
            string path = "enemies.json";

            if (!File.Exists(path))
            {
                MessageBox.Show("Файл enemies.json не найден! Положи его рядом с .exe");
                return;
            }

            string json = File.ReadAllText(path);
            List<CEnemyTemplate> loaded = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json)
                                          ?? new List<CEnemyTemplate>();

            // Закидываем в наш список
            foreach (var t in loaded)
            {
                templates.AddEnemy(t.Name, t.IconName, t.BaseLife,
                                   t.LifeModifier, t.BaseGold,
                                   t.GoldModifier, t.SpawnChance);
            }

            // Нормализуем шансы
            templates.NormalizeChances();
        }

        // ===== СОЗДАНИЕ НОВОГО ВРАГА =====
        private void SpawnNextEnemy()
        {
            if (templates.GetListOfEnemyNames().Count == 0)
            {
                MessageBox.Show("Нет шаблонов противников!");
                return;
            }

            // Выбираем шаблон по шансам
            double chance = rnd.NextDouble();
            CEnemyTemplate template = templates.FindByChance(chance);

            if (template == null)
                template = templates.GetEnemyByIndex(0);

            // Считаем характеристики с модификаторами
            // HP = BaseLife * (LifeModifier ^ player.Lvl)
            BigNumber hp = new BigNumber(template.BaseLife);
            for (int i = 0; i < player.Lvl - 1; i++)
                hp = hp * template.LifeModifier;

            // Gold = BaseGold * (GoldModifier ^ player.Lvl)
            BigNumber gold = new BigNumber(template.BaseGold);
            for (int i = 0; i < player.Lvl - 1; i++)
                gold = gold * template.GoldModifier;

            // Ищем иконку
            string iconPath = FindIconPath(template.IconName);

            currentEnemy = new Enemy(template.Name, hp, gold, iconPath);

            // Обновляем иконку
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

        // Поиск иконки в папке Icons (если она есть)
        private string FindIconPath(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
                return "";

            string iconsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons");
            if (Directory.Exists(iconsDir))
            {
                string fullPath = Path.Combine(iconsDir, iconName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return "";
        }

        // ===== КЛИК ПО ВРАГУ =====
        private void EnemyIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy == null || currentEnemy.IsDead)
                return;

            BigNumber damage = player.DealDamage();
            totalDamage = totalDamage + damage;

            bool killed = currentEnemy.TakeDamage(damage, out BigNumber reward);

            if (killed)
            {
                player.AddGold(reward);
                totalGold = totalGold + reward;
                killedCount++;

                // Небольшая задержка для читаемости
                System.Threading.Tasks.Task.Delay(150).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        SpawnNextEnemy();
                        UpdateUI();
                    });
                });
            }

            UpdateUI();
        }

        // ===== КНОПКА АПГРЕЙДА =====
        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            bool upgraded = player.TryUpgrade();
            if (upgraded)
                UpdateUI();
            else
                MessageBox.Show("Недостаточно золота!");
        }

        // ===== ОБНОВЛЕНИЕ ИНТЕРФЕЙСА =====
        private void UpdateUI()
        {
            // Игрок
            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();
            UpgradeCostText.Text = $"Стоимость: {player.UpgradeCost}";

            // Враг
            if (currentEnemy != null)
            {
                EnemyNameText.Text = currentEnemy.Name;
                EnemyHpText.Text = $"{currentEnemy.CurrentHitPoints} / {currentEnemy.MaxHitPoints}";
                EnemyRewardText.Text = currentEnemy.GoldReward.ToString();
            }

            // Статистика
            KilledText.Text = killedCount.ToString();
            TotalDamageText.Text = totalDamage.ToString();
            TotalGoldText.Text = totalGold.ToString();
        }
    }
}