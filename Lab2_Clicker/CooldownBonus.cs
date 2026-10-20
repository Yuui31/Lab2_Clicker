using System.Windows;
using System.Windows.Media;

namespace Lab2_Clicker
{
    public class CooldownBonus : CCollectable
    {
        public CooldownBonus(Point pos) : base(pos, 40)
        {
            sprite.Fill = Brushes.Red;
        }

        public override void ApplyEffect(Player player, MainWindow window)
        {
            window.ActivateCooldownBuff();
        }
    }
}