using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2_Clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            BigNumber a = new BigNumber("3712117");
            BigNumber b = new BigNumber("15512900");
            BigNumber sum = a + b;

            BigNumber c = new BigNumber("7135654");
            BigNumber mul = c * 23;

            MessageBox.Show($"Сумма: {sum}\n(ожидалось 19225017)\n\n" +
                            $"Умножение: {mul}\n(ожидалось 164120042)");
        }
        private void TestPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            Player p = new Player();
            p.AddGold(new BigNumber("100"));

            string info = $"До: lvl={p.Lvl}, gold={p.Gold}, damage={p.Damage}, cost={p.UpgradeCost}\n";
            bool ok = p.TryUpgrade();
            info += $"Апгрейд: {ok}\n";
            info += $"После: lvl={p.Lvl}, gold={p.Gold}, damage={p.Damage}, cost={p.UpgradeCost}";

            MessageBox.Show(info);
        }
    }
}