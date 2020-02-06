using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SearchPhotoDuplicatesMVVM.Models;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace SearchPhotoDuplicatesMVVM.ViewModels
{
    class StartWindowViewModel:INotifyPropertyChanged
    {
        private string chosenDirectory;
        public string ChosenDirectory
        {
            get { return chosenDirectory; }
            set { chosenDirectory = value; OnPropertyChanged("ChosenDirectory"); }
        }

        private RelayCommand choseDirectoryCommand;
        public RelayCommand ChoseDirectoryCommand
        {
            get
            {
                return choseDirectoryCommand ??
                (choseDirectoryCommand = new RelayCommand(obj =>
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.ShowDialog();
                    if (dlg != null) ChosenDirectory = dlg.SelectedPath;
                    //else System.Windows.MessageBox.Show("Введите корректный путь");
                }));
            }
        }

        private RelayCommand nextCommand;
        public RelayCommand NextCommand
        {
            get
            {
                return nextCommand ??
                (nextCommand = new RelayCommand(obj =>
                {
                    if (String.IsNullOrEmpty(ChosenDirectory) || String.IsNullOrWhiteSpace(ChosenDirectory)) System.Windows.MessageBox.Show("Введите путь.");
                    else
                    {
                        if (Directory.Exists(ChosenDirectory))
                        {
                            MainWindowViewModel.Dir = ChosenDirectory;
                        }
                        else System.Windows.MessageBox.Show("Директории " + ChosenDirectory + " не существует.");
                    }
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
