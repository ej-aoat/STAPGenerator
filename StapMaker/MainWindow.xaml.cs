using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Livet;
using System.Timers;
using System.Diagnostics;

namespace StapMaker
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DispatcherHelper.UIDispatcher = this.Dispatcher;

			this.DataContext = vm;
		}

		MainWindowViewModel vm = new MainWindowViewModel();

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			vm.Start();
		}

		private void OutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			OutputTextBox.ScrollToEnd();
		}
	}

	public class MainWindowViewModel : ViewModel
	{
		public MainWindowViewModel()
		{
			sw = new Stopwatch();
			sb = new StringBuilder();
			bg = new BackgroundWorker();
			bg.DoWork += bg_DoWork;

			GenText.CollectionChanged += GenText_CollectionChanged;
			sw.Start();
		}

		public void Start()
		{
			bg.RunWorkerAsync();
		}


		void GenText_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (var prop in e.NewItems)
				{
					sb.AppendLine(prop.ToString());	
				}

				if (sw.ElapsedMilliseconds > 50)
				{
					RaiseOutputText();
					sw.Restart();
				}
			}
		}

		void bg_DoWork(object sender, DoWorkEventArgs e)
		{
			int count = 0;
			string text = "";
			do
			{
				count++;
				text = string.Format("{0,5}:{1}", count, Atlib.RandomAlphameric.RandomAlpha(4).ToUpper());

				GenText.Add(text);
			} while (text != "STAP" && count < 100000);

			if (count >= 100000)
			{
				sb.AppendLine("STAP細胞なんて無かったよ・・・");
			}

			
			sb.AppendLine(count + "回実行。");
			RaiseOutputText();
		}

		Stopwatch sw;
		StringBuilder sb;
		BackgroundWorker bg;
		ObservableSynchronizedCollection<string> GenText = new ObservableSynchronizedCollection<string>();

		string _OutputText;
		public string OutputText
		{
			get
			{
				return _OutputText;
			}
		}

		void RaiseOutputText()
		{
			_OutputText = sb.ToString();
			RaisePropertyChanged("OutputText");
		}
	}
}
