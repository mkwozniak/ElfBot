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

namespace ElfBot.Components
{
	public partial class BuffOptionsPanel
	{
		private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

		public BuffOptionsPanel()
		{
			InitializeComponent();
		}
	}
}
