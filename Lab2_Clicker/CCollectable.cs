using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2_Clicker
{
    public abstract class CCollectable
    {
        protected Point position;
        protected Ellipse sprite;

        public CCollectable(Point pos, double size)
        {
            position = pos;
            sprite = new Ellipse();
            sprite.Width = size;
            sprite.Height = size;
            sprite.Stroke = Brushes.White;
            sprite.StrokeThickness = 2;
            sprite.RenderTransform = new TranslateTransform(position.X, position.Y);
            sprite.HorizontalAlignment = HorizontalAlignment.Left;
            sprite.VerticalAlignment = VerticalAlignment.Top;

            sprite.RenderTransform = new TranslateTransform(position.X, position.Y);
        }

        public Ellipse GetSprite() { return sprite; }

        // Проверка клика по сфере
        public bool IsMouseOnObject(Point mousePosition)
        {
            return mousePosition.X >= position.X &&
                   mousePosition.X <= position.X + sprite.Width &&
                   mousePosition.Y >= position.Y &&
                   mousePosition.Y <= position.Y + sprite.Height;
        }

        // Абстрактный метод применения эффекта
        public abstract void ApplyEffect(Player player, MainWindow window);
    }
}