using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;

namespace Lab2_Clicker
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }
        public void AddEnemy(string name, string iconName, int baseLife, 
                            double lifeModifier, int baseGold, 
                            double goldModifier, double spawnChance)
        {
            CEnemyTemplate newEnemy = new CEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance);
            enemies.Add(newEnemy);
        }
        public CEnemyTemplate GetEnemyByName(string name)
        {
            foreach (CEnemyTemplate enemy in enemies)
            {
                if (enemy.Name == name)
                {
                    return enemy;
                }
            }
            return null;
        }
        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
            {
                return enemies[id];
            }
            return null;
        }
        public void DeleteEnemyByName(string name)
        {
            CEnemyTemplate found = GetEnemyByName(name);
            if (found != null)
            {
                enemies.Remove(found);
            }
        }
        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
            {
                enemies.RemoveAt(id);
            }
        }
        public List<string> GetListOfEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (CEnemyTemplate enemy in enemies)
            {
                names.Add(enemy.Name);
            }
            return names;
        }
        public void UpdateEnemy(string oldName, string newName, string iconName,
                       int baseLife, double lifeModifier, int baseGold,
                       double goldModifier, double spawnChance)
        {
            // Находим старого противника
            CEnemyTemplate found = GetEnemyByName(oldName);
            if (found == null)
                return;

            // Обновляем все его свойства
            found.Name = newName;
            found.IconName = iconName;
            found.BaseLife = baseLife;
            found.LifeModifier = lifeModifier;
            found.BaseGold = baseGold;
            found.GoldModifier = goldModifier;
            found.SpawnChance = spawnChance;
        }
        public void SaveToJson(string path)
        {
            // список enemies в JSONстроку
            string json = JsonSerializer.Serialize(enemies);

            // записываем строку в файл по указанному пути
            File.WriteAllText(path, json);
        }
        public void LoadFromJson(string path)
        {
            if (!File.Exists(path))
                return;

            // читаем содержимое файла
            string json = File.ReadAllText(path);

            // JSONстроку обратно в список
            // Если файл гавно —  null, тогда + пустой список
            enemies = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json)
                      ?? new List<CEnemyTemplate>();
        }
        // Нормализация шансов: сумма должна быть равна 1
        public void NormalizeChances()
        {
            if (enemies == null || enemies.Count == 0)
                return;

            double sum = 0;
            for (int i = 0; i < enemies.Count; i++)
                sum += enemies[i].SpawnChance;

            if (sum == 0)
                return;   // защита от деления на ноль

            for (int i = 0; i < enemies.Count; i++)
                enemies[i].SpawnChance /= sum;
        }

        // Выбор случайного шаблона по нормализованным шансам
        public CEnemyTemplate FindByChance(double chance)
        {
            double sum = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                sum += enemies[i].SpawnChance;
                if (sum >= chance)
                    return enemies[i];
            }
            return null;
        }
    }
}