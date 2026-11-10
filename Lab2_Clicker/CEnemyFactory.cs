using System;
using System.Reflection;

namespace Lab2_Clicker
{
    public static class CEnemyFactory
    {
        // Создает врага по имени класса
        public static IEnemy CreateEnemy(string typeName, string name, BigNumber hp, BigNumber gold, string icon)
        {
            // Ищем класс в текущей сборке (Lab2_Clicker)
            string fullTypeName = "Lab2_Clicker." + typeName;
            Type type = Type.GetType(fullTypeName);

            if (type == null)
            {
                // Если не нашли — создаем обычного
                return new CNormalEnemy(name, hp, gold, icon);
            }

            // Рефлексия: создаем экземпляр
            return (IEnemy)Activator.CreateInstance(type, name, hp, gold, icon);
        }
    }
}