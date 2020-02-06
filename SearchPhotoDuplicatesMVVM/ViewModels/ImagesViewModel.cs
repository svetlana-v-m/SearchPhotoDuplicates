using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using SearchPhotoDuplicatesMVVM.Models;

namespace SearchPhotoDuplicatesMVVM.ViewModels
{
    public class ImagesViewModel
    {
        public static string CalculateCollectionSize(ObservableCollection<ImageModel> collection)
        {
            long size = 0;
            
            foreach (ImageModel image in collection)
            {
                long copiesSize = 0;
                if (image.Copies.Count>0)
                {
                    copiesSize = image.LongCopiesSize;
                }
                size = size + image.Size + copiesSize;
            }

            return ImageModel.Length_convert(size);
        }

        public static string CalculateCopiesTotalSize(ObservableCollection<ImageModel> collection)
        {
            long copiessize = 0;
            foreach(var im in collection)
            {
                if (im.Copies.Count > 0)
                    foreach (var copy in im.Copies) copiessize = copiessize + copy.Size;
             }
            return ImageModel.Length_convert(copiessize);
        }

        public static string CountNumberOfCopies(ObservableCollection<ImageModel> collection)
        {
            long numberOfCopies = 0;
            foreach (var im in collection)
            {
                if (im.Copies.Count > 0)
                    foreach (var copy in im.Copies) numberOfCopies++;
            }
            return numberOfCopies.ToString();
        }

        public static string CountPictures(ObservableCollection<ImageModel> collection)
        {
            long n = 0;
            n = collection.Count();
            foreach (ImageModel image in collection)
            {
                n = n + image.Copies.Count();
            }
            return n.ToString();
        }

        public static BitmapImage SomeImageSource(string fullPath)
        {
            BitmapImage someImage = new BitmapImage();
            try
                {
                    GC.Collect();
                    someImage.BeginInit();
                    someImage.CacheOption = BitmapCacheOption.OnLoad;
                    someImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    someImage.UriSource = new Uri(fullPath);
                    someImage.EndInit();
                }
                catch
                {
                    MessageBox.Show("Неозможно отобразить на экране. Возможно, файл поврежден или имеет неверный формат.");
                    return null;
                }
            return someImage;
        }

        public static void GetImagesViews(string[] ext, string root, BackgroundWorker bgWorker, DoWorkEventArgs e)
        {
            List<string> directories = new List<string>(GetDirectoriesList(root));//коллекция для списка поддиректорий типа string
            List<string> files = new List<string>();//коллекция для списка файлов типа string
            List<ImageModel> images = new List<ImageModel>();//коллекция для списка файлов типа ImageModel
            
            try
            {
                //поиск картинок в root
                foreach (string ex in ext)
                {
                    foreach (string findedimage in Directory.EnumerateFiles(root, ex, SearchOption.TopDirectoryOnly))
                    {
                        FileInfo fileinfo = null;
                        try
                        {
                           fileinfo = new FileInfo(findedimage);
                           ImageModel img = new ImageModel(fileinfo.Name, fileinfo.DirectoryName, fileinfo.FullName, fileinfo.LastWriteTime, fileinfo.Length);

                            if (images.Count == 0)
                            {
                                images.Add(img);
                                bgWorker.ReportProgress(-1, img);
                                if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                continue;
                            }
                            else
                            {
                                bool flag = false;
                                for (int j = 0; j < images.Count; j++)
                                {
                                    if (img.Equals(images[j]))
                                    {
                                        images[j].Copies.Add(img);
                                        bgWorker.ReportProgress(j, images[j]);
                                        if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                        flag = true; break;
                                    }
                                }
                                if (flag == false)
                                {
                                    images.Add(img);
                                    bgWorker.ReportProgress(-1, img);
                                    if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                }
                            }
                        }
                        catch (Exception ex1) { MessageBox.Show(ex1.Message); }
                    }
                }
                //поиск картинок в поддиректориях root
                 foreach (string dir in directories)
                {
                    if (File.GetAttributes(dir).HasFlag(FileAttributes.Hidden)) continue;
                    else
                    {
                        foreach (string pattern in ext)
                        {
                            List<string> list = new List<string>(GetFilesList(dir, pattern));
                            if (list.Count > 0)
                            {
                                try
                                {
                                    foreach (string f in list)
                                    {
                                        FileInfo file = new FileInfo(f);
                                        ImageModel img = new ImageModel(file.Name, file.DirectoryName, file.FullName, file.LastWriteTimeUtc, file.Length);

                                        if (images.Count == 0)
                                        {
                                            images.Add(img);
                                            bgWorker.ReportProgress(-1, img); 
                                            if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                            continue;
                                        }
                                        else
                                        {
                                            bool flag = false;
                                            for (int j = 0; j < images.Count; j++)
                                            {
                                                if (img.Equals(images[j]))
                                                {
                                                    images[j].Copies.Add(img);
                                                    bgWorker.ReportProgress(j, images[j]);
                                                    flag = true;
                                                    if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                                    break;
                                                }
                                            }
                                            if (!flag)
                                            {
                                                images.Add(img);
                                                bgWorker.ReportProgress(-1, img);
                                                if (bgWorker.CancellationPending) { e.Cancel = true; return; }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex) { MessageBox.Show(ex.Message); }
                            }
                            else continue;
                        }
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }

            images.Clear();
            directories.Clear();
            files.Clear();

        }

        public static bool Equals(ObservableCollection<ImageModel> one,ObservableCollection<ImageModel> other)
        {
            List<bool> compareList = new List<bool>();
            foreach (ImageModel im in one)
            {
                foreach(ImageModel im1 in other)
                {
                    if (im == im1) compareList.Add(true);
                    else compareList.Add(false);
                }
            }
            if (compareList.Contains(false)) return false;
            else return true;
        }

        private static List<string> GetDirectoriesList(string root)
        {
            List<string> result = Directory.GetDirectories(root).ToList();
            return result;
        }

        private static string[] GetFilesList(string dir,string ext)
        {
            string[] result = Directory.GetFiles(dir, ext, SearchOption.AllDirectories);
            return result;
        }
    }
}
