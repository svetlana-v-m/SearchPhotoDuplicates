using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SearchPhotoDuplicatesMVVM.Models;
using SearchPhotoDuplicatesMVVM.ViewModels;
using System.Drawing;
using System.Windows;

namespace SearchPhotoDuplicatesMVVM.Models
{
    public class ImageModel:INotifyPropertyChanged
    {
        #region Properties and variables
        private string _name;
        private string _directory;
        private string _fullPath;
  
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public string Directory
        {
            get { return _directory; }
            set { _directory = value; OnPropertyChanged("Directory"); }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; OnPropertyChanged("FullPath"); }
        }

        public long Size { get; }

        public string Length { get; }

        public DateTime Date_Time { get; }

        public string MD5 { get; }

        public ObservableCollection<ImageModel> Copies { get; set; }

        private string numberOfCopies;
        public string NumberOfCopies
        {
            get { return numberOfCopies; }
            set { numberOfCopies = value;OnPropertyChanged("NumberOfCopies"); }
        }

        private long longCopiesSize;
        public long LongCopiesSize
        {
            get { return longCopiesSize; }
            set { longCopiesSize = value;OnPropertyChanged("LongCopiesSize"); }
        }

        private string stringCopiesSize;
        public string StringCopiesSize
        {
            get { return stringCopiesSize; }
            set { stringCopiesSize = value; OnPropertyChanged("StringCopiesSize"); }
        }

        private bool imageChecked;
        public bool ImageChecked
        {
            get { return imageChecked; }
            set { imageChecked = value; OnPropertyChanged("ImageChecked"); }
        }
        #endregion

        #region Constructors
        public ImageModel()
        {
            Copies.CollectionChanged += Copies_CollectionChanged;
        }

        public ImageModel(ImageModel img)
        {
            _name = img.Name;
            _directory = img.Directory;
            _fullPath = img.FullPath;
            Date_Time = img.Date_Time;
            Size = img.Size;
            Length = Length_convert(img.Size);
            MD5 = img.MD5;
            Copies = img.Copies;
            NumberOfCopies = img.NumberOfCopies;
            LongCopiesSize = img.LongCopiesSize;
            StringCopiesSize = img.StringCopiesSize;
            Copies.CollectionChanged += Copies_CollectionChanged;
        }

        public ImageModel(string name, string directory, string fullPath, DateTime date_time, long size)
        {
            _name = name;
            _directory = directory;
            _fullPath = fullPath;
            Date_Time = date_time;
            Size = size;
            Length = Length_convert(size);
            MD5 = ComputeMD5Checksum(_fullPath);
            Copies = new ObservableCollection<ImageModel>();
            NumberOfCopies =  "0";
            LongCopiesSize = CalculateCopiesSize(Copies);
            StringCopiesSize = Length_convert(LongCopiesSize);
            Copies.CollectionChanged += Copies_CollectionChanged;
            ImageChecked = false;
        }

        public ImageModel(ObservableCollection<ImageModel> copies)
        {
            Copies = copies;
            Copies.CollectionChanged += Copies_CollectionChanged;
            NumberOfCopies =Copies.Count.ToString();
            LongCopiesSize = CalculateCopiesSize(copies);
            StringCopiesSize = Length_convert(LongCopiesSize);
        }

        public ImageModel(ImageModel img,ImageModel copy)
        {
            _name = img.Name;
            _directory = img.Directory;
            _fullPath = img.FullPath;
            Date_Time = img.Date_Time;
            Size = img.Size;
            Length = Length_convert(img.Size);
            MD5 = "abracadabra";
            Copies = img.Copies;
            Copies.Add(new ImageModel(copy));
            NumberOfCopies = img.NumberOfCopies;
            LongCopiesSize = img.LongCopiesSize;
            StringCopiesSize = img.StringCopiesSize;
            Copies.CollectionChanged += Copies_CollectionChanged;
            ImageChecked = false;
        }

        public ImageModel(ImageModel img, ObservableCollection<ImageModel> copies)
        {
            _name = img.Name;
            _directory = img.Directory;
            _fullPath = img.FullPath;
            Date_Time = img.Date_Time;
            Size = img.Size;
            Length = img.Length;
            MD5 = img.MD5;
            Copies = copies;
            NumberOfCopies = Copies.Count.ToString();
            LongCopiesSize = CalculateCopiesSize(copies);
            StringCopiesSize = Length_convert(LongCopiesSize);
            Copies.CollectionChanged += Copies_CollectionChanged;
            ImageChecked = false;
        }


        #endregion

        #region Methods

        public bool Equals(ImageModel other)
        {
            if (this.MD5 == other.MD5) return true;
            else return false;
        }

        private static long CalculateCopiesSize(ObservableCollection<ImageModel> collection)
        {
            long size = 0;
            foreach (ImageModel im in collection) size = size + im.Size;
            return size;
        }

        private static string ComputeMD5Checksum(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = System.IO.File.OpenRead(path))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    byte[] checkSum = md5.ComputeHash(fileData);
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    return result;
                }
            }
            else
            { return "CouldNotBeenCalculated"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public static string Length_convert(long length)
        {
            double res = 0;
            if (length < 1073741824 && length > 1048576)
            {
                res = length / (1024.0 * 1024.0);
                string FullSizeContent = res.ToString("0.0000");
                return FullSizeContent + " МБ";
            }
            else if (length < 1048576 && length > 1024)
            {
                res = length / 1024.0;
                string FullSizeContent = res.ToString("0.0000");
                return FullSizeContent + " КБ";
            }
            else
            {
                res = length / 1024.0 / 1024.0 / 1024.0;
                string FullSizeContent = res.ToString("0.0000");
                return FullSizeContent + " ГБ";
            }
        }

        public static ImageModel AddRenamedFile(ImageModel image,string name)
        {
            StringBuilder st = new StringBuilder();
            st.Append(image.Directory);
            st.Append(@"\");
            st.Append(name);
            ImageModel img = new ImageModel(image) { Name=name,FullPath=st.ToString()};
            return img;
        }

        private void Copies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LongCopiesSize = CalculateCopiesSize(Copies);
            StringCopiesSize = Length_convert(LongCopiesSize);
            NumberOfCopies = Copies.Count.ToString();
        }

        #endregion
    }
}
