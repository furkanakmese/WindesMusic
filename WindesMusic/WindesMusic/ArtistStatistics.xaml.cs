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
using System.Windows.Shapes;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for ArtistStatistics.xaml
    /// </summary>
    public partial class ArtistStatistics : Window
    {
        Database db = new Database();
        private int _ArtistName;
        List<Song> artistSongs;


        
        public ArtistStatistics()
        {
            InitializeComponent();
        }
    }
}
