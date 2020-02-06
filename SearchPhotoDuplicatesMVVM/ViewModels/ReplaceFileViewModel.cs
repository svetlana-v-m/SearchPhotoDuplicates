using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using SearchPhotoDuplicatesMVVM.Models;

namespace SearchPhotoDuplicatesMVVM.ViewModels
{
    class ReplaceFileViewModel:INotifyPropertyChanged
    {
        private string directory;
        public string Directory
        {
            get { return directory; }
            set { directory = value; OnPropertyChanged("Directory"); }
        }

        private string selectedPath;

        private RelayCommand newDirectoryCommand;
        public RelayCommand NewDirectoryCommand
        {
            get
            {
                return newDirectoryCommand ??
                    (newDirectoryCommand = new RelayCommand(obj =>
                      {
                          if (System.IO.Directory.Exists(Directory)) MainWindowViewModel.NewDirectory = Directory;
                          else
                          {
                              MainWindowViewModel.NewDirectory = "noDirectory";
                              System.Windows.MessageBox.Show("Путь введен некорректно.");
                          }
                      }));
            }
        }

        private RelayCommand choseDirectoryCommand;
        public RelayCommand ChoseDirectoryCommand
        {
            get
            {
                return choseDirectoryCommand ??
                (choseDirectoryCommand = new RelayCommand(obj =>
                {
                    ChooseDirectory();
                }/*, (obj) => !string.IsNullOrEmpty(Directory) && !string.IsNullOrWhiteSpace(Directory)*/));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void ChooseDirectory()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            if (dlg != null) { selectedPath = dlg.SelectedPath; Directory = selectedPath; }
        }
    }
}
