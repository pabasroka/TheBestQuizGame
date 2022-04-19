using System.Windows.Media;

namespace QuizFinalVersion.Models
{
    public class ListBoxItem
    {
        public string Name { get; set; }
        public SolidColorBrush Brush { get; set; }

        public ListBoxItem(string name, SolidColorBrush brush)
        {
            Name = name;
            Brush = brush;
        }
    }
}