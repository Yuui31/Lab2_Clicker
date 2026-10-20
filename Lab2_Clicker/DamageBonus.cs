using System.Windows;
using System.Windows.Media;

namespace Lab2_Clicker
{
    public class DamageBonus : CCollectable
    {
        public DamageBonus(Point pos) : base(pos, 40)
        {
            sprite.Fill = Brushes.LimeGreen;
        }

        public override void ApplyEffect(Player player, MainWindow window)
        {
            // Вызываем метод окна, который даст бафф урона на время
            window.ActivateDamageBuff(); 
        }
    }
}