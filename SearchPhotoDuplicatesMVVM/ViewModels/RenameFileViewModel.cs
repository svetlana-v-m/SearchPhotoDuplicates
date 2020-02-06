using SearchPhotoDuplicatesMVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SearchPhotoDuplicatesMVVM.ViewModels
{
    class RenameFileViewModel
    {
        public string Name { get; set; }
        public static ImageModel Image { get; set; }

        public RenameFileViewModel(ImageModel image)
        {
            Image = image;
            Name = Image.Name;
        }

        private RelayCommand newNameCommand;
        public RelayCommand NewNameCommand
        {
            get
            {
                return newNameCommand ??
                (newNameCommand = new RelayCommand(obj =>
                {
                    bool containsInvalidSymbols = Name.Intersect(Path.GetInvalidFileNameChars()).Any();
                    StringBuilder st = new StringBuilder();
                    st.Append(Image.Directory);
                    st.Append(@"\");
                    st.Append(Name);

                    if (!File.Exists(Image.FullPath))
                    {
                        MessageBox.Show("Файл" + Image.FullPath + "не существует.");
                    }
                    if (File.Exists(st.ToString()))
                    {
                        MessageBox.Show("Файл с таким именем уже существует. Введите другое имя.");
                    }
                    if (!File.Exists(st.ToString()))
                    {
                        try
                        {
                            MainWindowViewModel.NewFileName = Name;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }
                    MainWindowViewModel.NewFileFullName = st.ToString();
                }, (obj) => !string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name)));
            }
        }
    }
}
