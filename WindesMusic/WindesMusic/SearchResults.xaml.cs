using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Page
    {
        public SearchResults(string input)
        {
            InitializeComponent();

            Database db = new Database();
            stackResults.Children.Clear();
            List<Song> resultList = db.GetSearchResults(input);
            TextBlock tbMessage = new TextBlock();
            tbMessage.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
            tbMessage.FontSize = 20;

            if (resultList.Count > 0)
            {
                if (input.Trim() != "" && !input.Trim().Contains("_"))
                {
                    foreach (var item in resultList)
                    {
                        // here it shows the search results, shown like the screen design in FO

                    }
                }
                else
                {
                    tbMessage.Text = "Please type in search criteria"; 
                    stackResults.Children.Add(tbMessage);
                }
            }
            else
            {
                tbMessage.Text = "No results found";
                stackResults.Children.Add(tbMessage);
            }
        }
    }
}
