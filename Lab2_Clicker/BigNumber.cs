using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Clicker
{
    public class BigNumber
    {
        // ===== ПОЛЯ =====
        private int[] number;              // массив блоков, младший — в конце
        private const int Base = 1000;     // основание системы счисления

        // ===== СВОЙСТВА =====
        public int ArrayLength
        {
            get { return number.Length; }
        }

        // ===== КОНСТРУКТОРЫ =====

        // Приватный конструктор из массива — используется внутри класса
        private BigNumber(int[] arr)
        {
            number = TrimLeadingZeros(arr);
        }

        // Конструктор из строки: "2147483647" → [2, 147, 483, 647]
        public BigNumber(string numberStr)
        {
            // Убираем ведущие нули у строки
            numberStr = numberStr.TrimStart('0');
            if (string.IsNullOrEmpty(numberStr))
                numberStr = "0";

            // Разбиваем строку справа налево на блоки по 3 цифры
            List<int> blocks = new List<int>();
            int pos = numberStr.Length;

            while (pos > 0)
            {
                int start = Math.Max(0, pos - 3);
                int length = pos - start;
                string blockStr = numberStr.Substring(start, length);
                blocks.Add(int.Parse(blockStr));
                pos = start;
            }

            // blocks идут от младших к старшим — разворачиваем
            blocks.Reverse();
            number = blocks.ToArray();
        }

        // Конструктор из int (для удобства)
        public BigNumber(int value)
        {
            number = ParseString(value.ToString()).number;
        }

        // Вспомогательный метод: превратить строку в массив (для конструктора из int)
        private static BigNumber ParseString(string s)
        {
            return new BigNumber(s);
        }

        // ===== ВСПОМОГАТЕЛЬНЫЙ МЕТОД =====
        private int[] TrimLeadingZeros(int[] arr)
        {
            // Ищем первый ненулевой блок
            int start = 0;
            while (start < arr.Length - 1 && arr[start] == 0)
                start++;

            // Если старт на нуле — значит массив из одного нуля
            if (start == 0 && arr.Length == 0)
                return new int[] { 0 };

            int[] result = new int[arr.Length - start];
            Array.Copy(arr, start, result, 0, result.Length);
            return result;
        }

        // ===== СЛОЖЕНИЕ =====
        private BigNumber Add(BigNumber bnum)
        {
            int maxLen = Math.Max(number.Length, bnum.number.Length) + 1;
            int[] result = new int[maxLen];
            int carry = 0;

            // Идём с младших блоков (с конца массивов)
            for (int i = 0; i < maxLen; i++)
            {
                int a = (i < number.Length) ? number[number.Length - 1 - i] : 0;
                int b = (i < bnum.number.Length) ? bnum.number[bnum.number.Length - 1 - i] : 0;

                int sum = a + b + carry;
                result[maxLen - 1 - i] = sum % Base;
                carry = sum / Base;
            }

            return new BigNumber(result);
        }

        // ===== ВЫЧИТАНИЕ =====
        private BigNumber Subtract(BigNumber bnum)
        {
            // Предполагаем, что this >= bnum
            int maxLen = number.Length;
            int[] result = new int[maxLen];
            int borrow = 0;

            for (int i = 0; i < maxLen; i++)
            {
                int a = number[number.Length - 1 - i];
                int b = (i < bnum.number.Length) ? bnum.number[bnum.number.Length - 1 - i] : 0;

                int diff = a - b - borrow;
                if (diff < 0)
                {
                    diff += Base;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result[maxLen - 1 - i] = diff;
            }

            return new BigNumber(result);
        }

        // ===== УМНОЖЕНИЕ на double =====
        private BigNumber Multiply(double multiplier)
        {
            // Работаем с дробной частью: умножаем, округляем вниз
            int maxLen = number.Length + 1;
            int[] result = new int[maxLen];
            double carry = 0;

            for (int i = 0; i < maxLen; i++)
            {
                double a = (i < number.Length) ? number[number.Length - 1 - i] : 0;
                double product = a * multiplier + carry;

                result[maxLen - 1 - i] = (int)(product % Base);
                carry = Math.Floor(product / Base);
            }

            return new BigNumber(result);
        }

        // ===== ДЕЛЕНИЕ на double =====
        private BigNumber Divide(double divisor)
        {
            int[] result = new int[number.Length];
            double remainder = 0;

            // Идём слева направо (от старших к младшим)
            for (int i = 0; i < number.Length; i++)
            {
                double current = remainder * Base + number[i];
                result[i] = (int)(current / divisor);
                remainder = current % divisor;
            }

            return new BigNumber(result);
        }

        // ===== СРАВНЕНИЕ =====
        private int CompareTo(BigNumber bnum)
        {
            // Сначала сравниваем по длине (у кого больше блоков — тот больше)
            if (number.Length > bnum.number.Length) return 1;
            if (number.Length < bnum.number.Length) return -1;

            // Длина одинаковая — сравниваем по блокам
            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] > bnum.number[i]) return 1;
                if (number[i] < bnum.number[i]) return -1;
            }

            return 0;
        }

        // ===== КЛОНИРОВАНИЕ =====
        public BigNumber Clone()
        {
            return new BigNumber((int[])number.Clone());
        }

        // ===== ToString =====
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < number.Length; i++)
            {
                if (i == 0)
                    sb.Append(number[i].ToString());       // старший блок — как есть
                else
                    sb.Append(number[i].ToString("D3"));   // остальные — с нулями до 3 знаков
            }

            return sb.ToString();
        }

        // ===== ОПЕРАТОРЫ =====
        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Subtract(b);
        public static BigNumber operator *(BigNumber a, double b) => a.Multiply(b);
        public static BigNumber operator /(BigNumber a, double b) => a.Divide(b);
        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
    }
}