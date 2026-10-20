using System.Windows;
using System.Windows.Media;

namespace Lab2_Clicker
{
    public class GoldBonus : CCollectable
    {
        public GoldBonus(Point pos) : base(pos, 40)
        {
            sprite.Fill = Brushes.Gold;
        }

        public override void ApplyEffect(Player player, MainWindow window)
        {
            player.AddGold(new BigNumber(50)); // Мгновенно даем золото
        }
    }
}