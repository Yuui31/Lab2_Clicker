using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Clicker
{
    // Тот самый интерфейс из задания
    public interface ISaveList<T>
    {
        void SaveToJson(string path);
        void LoadFromJson(string path);
    }
}