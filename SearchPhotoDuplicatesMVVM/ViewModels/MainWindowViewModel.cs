using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SearchPhotoDuplicatesMVVM.Models;
using SearchPhotoDuplicatesMVVM.Views;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace SearchPhotoDuplicatesMVVM.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        #region Properties

        public static ObservableCollection<ImageModel> ImagesCollection { get; set; }

        #region Selections
        private ImageModel selectedImage;
        public ImageModel SelectedImage
        {
            get
            { return selectedImage; }
            set
            {
                selectedImage = value;
                if (value == null) IsCheckAllCopiesEnabled = false;
                else IsCheckAllCopiesEnabled = true; OnPropertyChanged("SelectedImage");
            }
        }
        private ImageModel selectedCopy;
        public ImageModel SelectedCopy
        {
            get { return selectedCopy; }
            set
            {
                selectedCopy = value; OnPropertyChanged("SelectedCopy");
            }
        }
        private string selectedFilter;
        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                if (selectedFilter == value) return;
                selectedFilter = value; OnPropertyChanged("SelectedFilter");
            }
        }
        private BitmapImage imageSource;
        public BitmapImage ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; OnPropertyChanged("ImageSource"); }
        }
        private BitmapImage imageCopySource;
        public BitmapImage ImageCopySource
        {
            get { return imageCopySource; }
            set { imageCopySource = value; OnPropertyChanged("ImageCopySource"); }
        }
        
        #endregion

        #region Search
        public static string Dir { get; set; }
        private string searchProgress;
        public string SearchProgress
        {
            get { return searchProgress; }
            set { searchProgress = value; OnPropertyChanged("SearchProgress"); }
        }
        private string searchButtonContent;
        public string SearchButtonContent
        {
            get { return searchButtonContent; }
            set { searchButtonContent = value; OnPropertyChanged("SearchButtonContent"); }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged("SearchText"); }
        }
        private List<ImageModel> SearchExcludedItems { get; set; }
        private List<ImageModel> SearchResultItems { get; set; }
        #endregion

        #region Collection Items
        public static string NewFileName { get; set; }
        public static string NewFileFullName { get; set; }
        public static string NewDirectory { get; set; }
        public ObservableCollection<ImageModel> CheckedImages { get; set; }
        private ObservableCollection<ImageModel> CheckedCopies { get; set; }
        private List<ImageModel> FilteredItems { get; set; }
        #endregion

        #region ForUI
        private string dirForUI;
        public string DirForUI
        {
            get { return dirForUI; }
            set { dirForUI = value; OnPropertyChanged("DirForUI"); }
        }
        private string _collectionsize;
        public string CollectionSize
        {
            get { return _collectionsize; }
            set { _collectionsize = value; OnPropertyChanged("CollectionSize"); }
        }
        private string _copiesTotalSize;
        public string CopiesTotalSize
        {
            get { return _copiesTotalSize; }
            set { _copiesTotalSize = value; OnPropertyChanged("CopiesTotalSize"); }
        }
        private string _numberOfImages;
        public string NumberOfImages
        {
            get { return _numberOfImages; }
            set { _numberOfImages = value; if (_numberOfImages != null) LabelVisibility = "Visible"; OnPropertyChanged("NumberOfImages"); }
        }
        private string _numberOfCopies;
        public string NumberOfCopies
        {
            get { return _numberOfCopies; }
            set { _numberOfCopies = value; if (_numberOfCopies != null) Label1Visibility = "Visible"; OnPropertyChanged("NumberOfCopies"); }
        }
        private string labelVisibility;
        public string LabelVisibility
        {
            get { return labelVisibility; }
            set { labelVisibility = value; OnPropertyChanged("LabelVisibility"); }
        }
        private string label1Visibility;
        public string Label1Visibility
        {
            get { return label1Visibility; }
            set { label1Visibility = value;OnPropertyChanged("Label1Visibility"); }
        }
        private string label2Visibility;
        public string Label2Visibility
        {
            get { return label2Visibility; }
            set { label2Visibility = value; OnPropertyChanged("Label2Visibility"); }
        }
        private string label3Visibility;
        public string Label3Visibility
        {
            get { return label3Visibility; }
            set { label3Visibility = value; OnPropertyChanged("Label3Visibility"); }
        }
        private string numberOfCheckedImages;
        public string NumberOfCheckedImages
        {
            get { return numberOfCheckedImages; }
            set { numberOfCheckedImages = value;
                if (value == "0") { Label2Visibility = "Hidden"; }
                else { Label2Visibility = "Visible";} OnPropertyChanged("NumberOfCheckedImages"); }
        }
        private string sizeOfCheckedImages;
        public string SizeOfCheckedImages
        {
            get { return sizeOfCheckedImages; }
            set { sizeOfCheckedImages = value;OnPropertyChanged("SizeOfCheckedImages"); }
        }
        private string numberOfCheckedCopies;
        public string NumberOfCheckedCopies
        {
            get { return numberOfCheckedCopies; }
            set
            {
                numberOfCheckedCopies = value;
                if (value == "0") Label3Visibility = "Hidden";
                else Label3Visibility = "Visible"; OnPropertyChanged("NumberOfCheckedCopies");
            }
        }
        private string sizeOfCheckedCopies;
        public string SizeOfCheckedCopies
        {
            get { return sizeOfCheckedCopies; }
            set { sizeOfCheckedCopies = value; OnPropertyChanged("SizeOfCheckedCopies"); }
        }
        private bool isFilterEnabled;
        public bool IsFilterEnabled
        {
            get { return isFilterEnabled; }
            set { isFilterEnabled = value;OnPropertyChanged("IsFilterEnabled"); }
        }
        private bool isCheckAllEnabled;
        public bool IsCheckAllEnabled
        {
            get { return isCheckAllEnabled; }
            set { isCheckAllEnabled = value; OnPropertyChanged("IsCheckAllEnabled"); }
        }
        private bool isCheckAllCopiesEnabled;
        public bool IsCheckAllCopiesEnabled
        {
            get { return isCheckAllCopiesEnabled; }
            set { isCheckAllCopiesEnabled = value; OnPropertyChanged("IsCheckAllCopiesEnabled"); }
        }
        #endregion
        #endregion

        #region Variables

        static readonly string[] FExt = { "*.jpeg", "*.tiff", "*.tif", "*.bmp", "*.gif", "*.eps", "*.png", "*.pict", "*.pcx", "*.ico", "*.cdr", "*.ai", "*.raw", "*.svg", "*.jpg" };

        public List<string> Filters { get; } = new List<string>() {
        "Фильтр", "Показать все изображения", "Показать изображения с копиями"};

        BackgroundWorker Bgworker;
  
        int n;
        #endregion

        #region Commands

        #region Filling in ImagesCollection

        //заполнить dataGrid , найти копии изображений
        private RelayCommand findImagesCommand;
        public RelayCommand FindImagesCommand
        {
            get
            {
                return findImagesCommand ??
                (findImagesCommand = new RelayCommand(obj =>
                {
                    if (!Bgworker.IsBusy)
                    {
                        StartWindow ImgSearch = new StartWindow();
                        bool? dialogResult = ImgSearch.ShowDialog();
                        switch (dialogResult)
                        {
                            case true:
                                {
                                    DirForUI = "В папке "+Dir;
                                    NewCollection();
                                    Bgworker.RunWorkerAsync();
                                    break;
                                }
                            case false:
                                { break; }
                            default:
                                { break; }
                        }
                    }
                    else
                    {
                        SearchProgress = "Остановка поиска...";
                        Bgworker.CancelAsync();
                    }
                        
                }));
            }
        }

        //найти изображения и их копии заново в той же директории
        private RelayCommand refreshImagesListCommand;
        public RelayCommand RefreshImagesListCommand
        {
            get
            {
                return refreshImagesListCommand ??
                (refreshImagesListCommand = new RelayCommand(obj =>
                {
                    NewCollection();
                    Bgworker.RunWorkerAsync();
                }, (obj) => !Bgworker.IsBusy&&ImagesCollection.Count>0));
            }
        }
        #endregion

        #region Actions on ImagesCollection

        #region Context menu on DataGrids rows

        //открыть файл
        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ??
                    (openFileCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        OpenFile(image);
                    },obj=>ImagesCollection.Count>0||SelectedImage.Copies.Count>0));
            }
        }

        //открыть папку с файлом
        private RelayCommand openFolderCommand;
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return openFolderCommand ??
                    (openFolderCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        OpenFolder(image);
                    },obj=>ImagesCollection.Count>0 || SelectedImage.Copies.Count > 0));
            }
        }

        //удалить строку из dataGrid
        private RelayCommand removeFromListCommand;
        public RelayCommand RemoveFromListCommand
        {
            get
            {
                return removeFromListCommand ??
                (removeFromListCommand = new RelayCommand(obj =>
                {
                    string imagesToRemove = obj as string;
                    if (imagesToRemove == "selectedImages") RemoveFromList();
                    else if (imagesToRemove == "selectedCopies") RemoveFromCopiesList(SelectedImage);
                }, (obj) => ImagesCollection.Count > 0&&!Bgworker.IsBusy));
            }
        }

        //удалить файл с диска
        private RelayCommand removeFromDiskCommand;
        public RelayCommand RemoveFromDiskCommand
        {
            get
            {
                return removeFromDiskCommand ??
                    (removeFromDiskCommand = new RelayCommand(obj =>
                    {
                        string imageToRemove = obj as string;
                        if (imageToRemove == "selectedImages") RemoveImagesFromDisk();
                        else if (imageToRemove == "selectedCopies") RemoveCopiesFromDisk();
                    },obj=> ImagesCollection.Count > 0 && !Bgworker.IsBusy));
            }
        }

        //переместить файл
        private RelayCommand replaceCommand;
        public RelayCommand ReplaceCommand
        {
            get
            {
                return replaceCommand ??
                    (replaceCommand = new RelayCommand(obj =>
                    {
                        string imagesToReplace = obj as string;
                        ReplaceFiles(imagesToReplace);
                    }, obj=>ImagesCollection.Count > 0 && !Bgworker.IsBusy));
            }
        }

        //переименовать файл
        private RelayCommand renameFileCommand;
        public RelayCommand RenameFileCommand
        {
            get
            {
                return renameFileCommand ??
                (renameFileCommand = new RelayCommand(obj =>
                {
                    ImageModel imageToRename = obj as ImageModel;
                    RenameFiles(imageToRename);
                }, (obj) => ImagesCollection.Count > 0 && !Bgworker.IsBusy||SelectedImage != null || SelectedCopy != null));
            }
        }

        //поменять местами копию с оригиналом
        private RelayCommand thisIsOriginCommand;
        public RelayCommand ThisIsOriginCommand
        {
            get
            {
                return thisIsOriginCommand ??
                (thisIsOriginCommand = new RelayCommand(obj =>
                {
                    ThisIsOriginalImage();
                }));
            }
        }

        #endregion

        #region Search in ImagesCollection

        //поиск по введенному в textBox тексту
        private RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return searchCommand ??
                (searchCommand = new RelayCommand(obj =>
                {
                    if (SelectedFilter != Filters[2])
                    {
                        foreach (ImageModel im in SearchExcludedItems) if (!ImagesCollection.Contains(im)) ImagesCollection.Add(im);
                        CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                        NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                        SearchResultItems.Clear();
                        SearchExcludedItems.Clear();
                        SearchResultItems = ImagesCollection.Where(i => i.FullPath.Contains(SearchText)).ToList();
                        SearchExcludedItems = ImagesCollection.Where(i => !i.FullPath.Contains(SearchText)).ToList();
                        ImagesCollection.Clear();
                        foreach (ImageModel im in SearchResultItems) ImagesCollection.Add(im);
                        CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                        NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                    }
                    else
                    {
                        foreach (ImageModel im in SearchExcludedItems) if (!ImagesCollection.Contains(im) && im.Copies.Count > 0) ImagesCollection.Add(im);
                        CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                        NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                        SearchResultItems.Clear();
                        SearchExcludedItems.Clear();
                        SearchResultItems = ImagesCollection.Where(i => i.FullPath.Contains(SearchText)).ToList();
                        SearchExcludedItems = ImagesCollection.Where(i => !i.FullPath.Contains(SearchText)).ToList();
                        ImagesCollection.Clear();
                        foreach (ImageModel im in SearchResultItems) ImagesCollection.Add(im);
                        CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                        NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                    }
                },(obj)=>ImagesCollection.Count>0));
            }
        }

        //textBox в фокусе
        private RelayCommand searchTextBoxGotFocusCommand;
        public RelayCommand SearchTextBoxGotFocusCommand
        {
            get
            {
                return searchTextBoxGotFocusCommand ??
                (searchTextBoxGotFocusCommand = new RelayCommand(obj =>
                {
                    if (SearchText.Equals("Поиск..."))
                        SearchText = "";
                }));
            }
        }

        //textBox в фокусе
        private RelayCommand searchTextBoxLostFocusCommand;
        public RelayCommand SearchTextBoxLostFocusCommand
        {
            get
            {
                return searchTextBoxLostFocusCommand ??
                (searchTextBoxLostFocusCommand = new RelayCommand(obj =>
                {
                    if (string.IsNullOrWhiteSpace(SearchText))
                    {
                        SearchText = "Поиск...";
                        foreach (ImageModel im in SearchExcludedItems) ImagesCollection.Add(im);
                    }

                }));
            }
        }

        #endregion

        #region Other actions

        //применить фильтр для основного dataGrid
        private RelayCommand filterCommand;
        public RelayCommand FilterCommand
        {
            get
            {
                return filterCommand ??
                (filterCommand = new RelayCommand(obj =>
                {
                    if (selectedFilter == Filters[1])
                    {
                        if (FilteredItems != null)
                        {
                            foreach (ImageModel im in FilteredItems) ImagesCollection.Insert(0, im);
                            FilteredItems.Clear();
                            SelectedFilter = Filters[0];
                        }
                    }
                    else if (selectedFilter == Filters[2])
                    {
                        FilteredItems = ImagesCollection.Where(i => i.NumberOfCopies == "0").ToList();
                        foreach (var im in FilteredItems) ImagesCollection.Remove(im);
                    }
                }, (obj) => ImagesCollection.Count > 0 && !Bgworker.IsBusy));
            }
        }

        //изменение выбранной строки в основном dataGrid
        private RelayCommand selectionChangedCommand;
        public RelayCommand SelectionChangedCommand
        {
            get
            {
                return selectionChangedCommand ??
                (selectionChangedCommand = new RelayCommand(obj =>
                {
                    if (SelectedImage != null)
                    {
                        ImageSource = ImagesViewModel.SomeImageSource(SelectedImage.FullPath);
                        ImageCopySource = null;
                    }
                    List<ImageModel> list = ImagesCollection.Where(i => i.ImageChecked).ToList();
                    CheckedImages.Clear();
                    foreach (ImageModel im in list) CheckedImages.Add(im);
                }, (obj)=>ImagesCollection.Count>0));
            }
        }

        //изменение выбранной строки в dataGrid копий
        private RelayCommand selectionCopyChangedCommand;
        public RelayCommand SelectionCopyChangedCommand
        {
            get
            {
                return selectionCopyChangedCommand ??
                (selectionCopyChangedCommand = new RelayCommand(obj =>
                {
                    ImageCopySource = null;
                    if (SelectedCopy != null)
                    {
                        ImageCopySource = ImagesViewModel.SomeImageSource(SelectedImage.FullPath);
                    }
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    List<ImageModel> list = ImagesCollection[n].Copies.Where(i => i.ImageChecked).ToList();
                    CheckedCopies.Clear();
                    foreach (ImageModel im in list) CheckedCopies.Add(im);
                    
                },(obj)=>ImagesCollection.Count>0));
            }
        }

        //удалить все найденные копии с диска
        private RelayCommand removeAllCopiesFromDiskCommand;
        public RelayCommand RemoveAllCopiesFromDiskCommand
        {
            get
            {
                return removeAllCopiesFromDiskCommand ??
                (removeAllCopiesFromDiskCommand = new RelayCommand(obj =>
                {
                    if (obj != null) DeleteAllCopiesOfSelectedImage();
                    else DeleteAllCopies();
                }, (obj) => ImagesCollection.Count>0&&!Bgworker.IsBusy));
            }
        }

        //очистить список копий для выбранного файла в основном dataGrid
        private RelayCommand removeAllCopiesFromListCommand;
        public RelayCommand RemoveAllCopiesFromListCommand
        {
            get
            {
                return removeAllCopiesFromListCommand ??
                (removeAllCopiesFromListCommand = new RelayCommand(obj =>
                {
                    if (selectedImage != null)
                    {
                        n = ImagesCollection.IndexOf(selectedImage);
                        ImagesCollection[n].Copies.Clear();
                        NumberOfImages = ImagesViewModel.CountPictures(ImagesCollection);
                        CollectionSize = ImagesViewModel.CalculateCollectionSize(ImagesCollection);
                        //CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                        //NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                    }
                },(obj)=> ImagesCollection.Count > 0 && !Bgworker.IsBusy));
            }
        }

        private RelayCommand checkedImageCommand;
        public RelayCommand CheckedImageCommand
        {
            get
            {
                return checkedImageCommand ??
                    (checkedImageCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        AddToChecked(image);
                     }, (obj) => ImagesCollection.Count > 0));
            }
        }

        private RelayCommand uncheckedImageCommand;
        public RelayCommand UncheckedImageCommand
        {
            get
            {
                return uncheckedImageCommand ??
                    (uncheckedImageCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        RemoveFromChecked(image);
                    }, (obj) => ImagesCollection.Count > 0));
            }
        }

        private RelayCommand checkAllImagesCommand;
        public RelayCommand CheckAllImagesCommand
        {
            get
            {
                return checkAllImagesCommand ??
                    (checkAllImagesCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        CheckAll(image);
                    }, (obj) => ImagesCollection.Count > 0));
            }
        }

        private RelayCommand uncheckAllImagesCommand;
        public RelayCommand UncheckAllImagesCommand
        {
            get
            {
                return uncheckAllImagesCommand ??
                    (uncheckAllImagesCommand = new RelayCommand(obj =>
                    {
                        string image = obj as string;
                        UncheckAll(image);
                    }, (obj) => ImagesCollection.Count > 0));
            }
        }
        #endregion

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            SearchButtonContent = "Найти изображения";
            SearchText = "Поиск...";
            IsCheckAllEnabled = false;
            InitializeCollections();
            InitializeBackgroundWorker();
            InitializeUI();
        }

        private void InitializeUI()
        {
            LabelVisibility = "Hidden";
            Label1Visibility = "Hidden";
            Label2Visibility = "Hidden";
            Label3Visibility = "Hidden";
            IsFilterEnabled = false;
        }

        #region Collection Methods
        private void InitializeCollections()
        {
            ImagesCollection = new ObservableCollection<ImageModel>();
            SearchResultItems = new List<ImageModel>();
            SearchExcludedItems = new List<ImageModel>();
            ImagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
            CheckedImages = new ObservableCollection<ImageModel>();
            CheckedImages.CollectionChanged += CheckedImages_CollectionChanged;
            CheckedCopies = new ObservableCollection<ImageModel>();
            CheckedCopies.CollectionChanged += CheckedCopies_CollectionChanged;
        }

        private void CheckedImages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            long size = 0;
            foreach (ImageModel im in CheckedImages) size = size + im.Size;
            SizeOfCheckedImages = ImageModel.Length_convert(size);
            NumberOfCheckedImages = CheckedImages.Count.ToString();

        }

        private void CheckedCopies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            long size = 0;
            foreach (ImageModel im in CheckedCopies) size = size + im.Size;
            SizeOfCheckedCopies = ImageModel.Length_convert(size);
            NumberOfCheckedCopies = CheckedCopies.Count.ToString();

        }

        private void ImagesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionSize=ImagesViewModel.CalculateCollectionSize(ImagesCollection);
            NumberOfImages = ImagesViewModel.CountPictures(ImagesCollection);
        }

        private void NewCollection()
        {
            GC.Collect();
            ImageSource = null;
            ImageCopySource = null;
            SearchProgress = "Поиск изображений...";
            SearchText = "Поиск...";
            SelectedFilter = Filters[0];
            if (ImagesCollection.Count > 0)
            {
                ImagesCollection.Clear();
                CopiesTotalSize = null;
                NumberOfCopies = null;
            };
            if(CheckedImages.Count>0)
            {
                CheckedImages.Clear();
                NumberOfCheckedImages = null;
                SizeOfCheckedImages = null;
            }
        }

        private void OpenFile(string image)
        {
            if(image=="selectedImages")
            {
                if(CheckedImages.Count>0)
                {
                    foreach(ImageModel im in CheckedImages) Process.Start(im.FullPath);
                }
                else Process.Start(SelectedImage.FullPath);
            }
            else if(image=="selectedCopies")
            {
                if(CheckedCopies.Count>0)
                {
                    foreach(ImageModel im in CheckedCopies) Process.Start(im.FullPath);
                }
                else Process.Start(SelectedCopy.FullPath);
            }
        }

        private void OpenFolder(string image)
        {
            if(image=="selectedImages")
            {
                if(CheckedImages.Count>0)
                {
                    foreach(ImageModel im in CheckedImages)
                    {
                        ProcessStartInfo startInfo = null;
                        try
                        {
                            startInfo = new ProcessStartInfo("Explorer")
                            {
                                UseShellExecute = false,
                                Arguments = @"/select," + im.FullPath
                            };
                            Process.Start(startInfo);
                        }
                        catch (Exception) { }
                        finally
                        {
                            if (startInfo != null) startInfo = null;
                        }
                    }
                }
                else
                {
                    ProcessStartInfo startInfo = null;
                    try
                    {
                        startInfo = new ProcessStartInfo("Explorer")
                        {
                            UseShellExecute = false,
                            Arguments = @"/select," + SelectedImage.FullPath
                        };
                        Process.Start(startInfo);
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (startInfo != null) startInfo = null;
                    }
                }
            }
            else if(image=="selectedCopies")
            {
                if(CheckedCopies.Count>0)
                {
                    foreach(ImageModel im in CheckedCopies)
                    {
                        ProcessStartInfo startInfo = null;
                        try
                        {
                            startInfo = new ProcessStartInfo("Explorer")
                            {
                                UseShellExecute = false,
                                Arguments = @"/select," + im.FullPath
                            };
                            Process.Start(startInfo);
                        }
                        catch (Exception) { }
                        finally
                        {
                            if (startInfo != null) startInfo = null;
                        }
                    }
                }
                else
                {
                    ProcessStartInfo startInfo = null;
                    try
                    {
                        startInfo = new ProcessStartInfo("Explorer")
                        {
                            UseShellExecute = false,
                            Arguments = @"/select," + SelectedCopy.FullPath
                        };
                        Process.Start(startInfo);
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (startInfo != null) startInfo = null;
                    }
                }
            }
        }

        private void RemoveFromList()
        {
            if(CheckedImages.Count>0)
            {
                List<ImageModel> list = new List<ImageModel>(CheckedImages);
                foreach (ImageModel im in list)
                {
                    if (im.Copies.Count > 0) RemoveFromCopiesList(im);
                    ImagesCollection.Remove(im);
                }
            }
            else
            {
                if (SelectedImage.Copies.Count > 0) RemoveFromCopiesList(SelectedImage);
                ImagesCollection.Remove(SelectedImage);
            }
            SelectedFilter = Filters[0];
            ImageSource = null;
            CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
            NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
        }

        private void RemoveFromCopiesList(ImageModel image)
        {
            int n = ImagesCollection.IndexOf(image);
            if (CheckedCopies.Count>0)
            {
                List<ImageModel> list = new List<ImageModel>(CheckedCopies);
                foreach (ImageModel im in list)
                {
                    ImagesCollection.Add(im);
                    ImagesCollection[n].Copies.Remove(im);
                }
            }
            else
            {
                ImagesCollection.Add(SelectedCopy);
                ImagesCollection[n].Copies.Remove(SelectedCopy);
            }
            
            SelectedFilter = Filters[0];
            ImageSource = null;
            CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
            NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
        }

        private void RemoveImagesFromDisk()
        {
            try
            {
                if (CheckedImages.Count > 0)
                {
                    string header = "Запрос на удаление файлов.";
                    string message = "Удалить " + CheckedImages.Count + " выделенных файла с диска?";
                    var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (ImageModel im in CheckedImages)
                        {
                            File.Delete(im.FullPath);
                            int i = ImagesCollection.IndexOf(im);
                            if (ImagesCollection[i].Copies.Count > 0) RemoveFromCopiesList(im);
                            ImagesCollection.Remove(im);
                        }
                    }
                }
                else
                {
                    string header = "Запрос на удаление файла.";
                    string message = "Удалить файл " + SelectedImage.FullPath + " с диска?";
                    var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(SelectedImage.FullPath);
                        RemoveFromList();
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        private void RemoveCopiesFromDisk()
        {
            try
            {
                if (CheckedCopies.Count > 0)
                {
                    string header = "Запрос на удаление файлов.";
                    string message = "Удалить " + CheckedCopies.Count + " выделенных файла с диска?";
                    var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int n = ImagesCollection.IndexOf(SelectedImage);
                        foreach (ImageModel c in CheckedCopies)
                        {
                            File.Delete(c.FullPath);
                            ImagesCollection[n].Copies.Remove(c);
                            CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
                            NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
                        }
                    }
                }
                else
                {
                    string header = "Запрос на удаление файла.";
                    string message = "Удалить файл " + SelectedCopy.FullPath + " с диска?";
                    var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(SelectedCopy.FullPath);
                        int n = ImagesCollection.IndexOf(SelectedImage);
                        ImagesCollection[n].Copies.Remove(SelectedCopy);                     
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        private void ReplaceSelectedImages()
        {
            try
            {
                if(CheckedImages.Count>0)
                {
                    foreach (ImageModel im in CheckedImages)
                    {
                        int n = ImagesCollection.IndexOf(im);
                        StringBuilder st = new StringBuilder();
                        st.Append(NewDirectory);
                        st.Append(@"\");
                        st.Append(im.Name);
                        if (!File.Exists(st.ToString()))
                        {
                            File.Move(ImagesCollection[n].FullPath, st.ToString());
                            ImagesCollection[n].FullPath = st.ToString();
                            ImagesCollection[n].Directory = NewDirectory;
                        }
                        else
                        {
                            MessageBox.Show("Невозможно переместить файл " + im.FullPath + " в директорию " + NewDirectory + ", так как файл с именем " + im.Name + " уже существует в этой директории.");
                            continue;
                        }
                    }
                   //ImageSource = ImagesViewModel.SomeImageSource(st.ToString());
                }
                else
                {
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    StringBuilder st = new StringBuilder();
                    st.Append(NewDirectory);
                    st.Append(@"\");
                    st.Append(SelectedImage.Name);
                    if (!File.Exists(st.ToString()))
                    {
                        File.Move(ImagesCollection[n].FullPath, st.ToString());
                        ImagesCollection[n].FullPath = st.ToString();
                        ImagesCollection[n].Directory = NewDirectory;
                    }
                    else
                    {
                        MessageBox.Show("Невозможно переместить файл " + SelectedImage.FullPath + " в директорию " + NewDirectory + ", так как файл с именем " + SelectedImage.Name + " уже существует в этой директории.");
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ReplaceSelectedCopies()
        {
            try
            {
                int m = ImagesCollection.IndexOf(SelectedImage);
                if(CheckedCopies.Count>0)
                {
                    foreach (ImageModel c in CheckedCopies)
                    {
                        int n = ImagesCollection[m].Copies.IndexOf(c);
                        StringBuilder st = new StringBuilder();
                        st.Append(NewDirectory);
                        st.Append(@"\");
                        st.Append(c.Name);
                        if (!File.Exists(st.ToString()))
                        {
                            File.Move(ImagesCollection[m].Copies[n].FullPath, st.ToString());
                            ImagesCollection[m].Copies[n].FullPath = st.ToString();
                            ImagesCollection[m].Copies[n].Directory = NewDirectory;
                        }
                        else
                        {
                            MessageBox.Show("Невозможно переместить файл " + c.FullPath + " в директорию " + NewDirectory + ", так как файл с именем " + c.Name + " уже существует в этой директории.");
                            continue;
                        }
                    }
                }
                else
                {
                    int n = ImagesCollection[m].Copies.IndexOf(SelectedCopy);
                    StringBuilder st = new StringBuilder();
                    st.Append(NewDirectory);
                    st.Append(@"\");
                    st.Append(SelectedCopy.Name);
                    if (!File.Exists(st.ToString()))
                    {
                        File.Move(ImagesCollection[m].Copies[n].FullPath, st.ToString());
                        ImagesCollection[m].Copies[n].FullPath = st.ToString();
                        ImagesCollection[m].Copies[n].Directory = NewDirectory;
                    }
                    else
                    {
                        MessageBox.Show("Невозможно переместить файл " + SelectedCopy.FullPath + " в директорию " + NewDirectory + ", так как файл с именем " + SelectedCopy.Name + " уже существует в этой директории.");
                    }
                }
                
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ReplaceFiles(string imagesToReplace)
        {
            ReplaceFileWindow rFile = new ReplaceFileWindow();
            bool? dialogResult = rFile.ShowDialog();

            switch (dialogResult)
            {
                case true:
                    {
                        if (NewDirectory == "noDirectory") ReplaceFiles(imagesToReplace);
                        else
                        {
                            if (imagesToReplace == "selectedImages") ReplaceSelectedImages();
                            else if (imagesToReplace == "selectedCopies") ReplaceSelectedCopies();
                        }
                        break;
                    }
                case false:
                    { break; }
                default:
                    { break; }
            }
        }

        private void RenameImage()
        {
            try
            {
               if (NewFileName.Intersect(Path.GetInvalidFileNameChars()).Any()) { MessageBox.Show(@"Имя файла содержит один из недопустимых символов: \ / : * ? < > | + """); RenameFiles(SelectedImage); } 
                else
                {
                    File.Move(SelectedImage.FullPath, NewFileFullName);
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    ImageModel newImg = new ImageModel(SelectedImage) { Name = NewFileName, FullPath = NewFileFullName };
                    ImagesCollection.Remove(SelectedImage);
                    ImagesCollection.Insert(n, newImg);
                    SelectedImage = ImagesCollection[n];
                }
            }
            catch(Exception e) { MessageBox.Show(e.Message); }
        }

        private void RenameCopyImage()
        {
            try
            {
                if (NewFileName.Intersect(Path.GetInvalidFileNameChars()).Any()) { MessageBox.Show(@"Имя файла содержит один из недопустимых символов: \ / : * ? < > | + """); RenameFiles(SelectedCopy); }
                else
                {
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    int m = ImagesCollection[n].Copies.IndexOf(SelectedCopy);

                    File.Move(SelectedCopy.FullPath, NewFileFullName);
                    
                    ImageModel image = new ImageModel(SelectedCopy) { Name = NewFileName, FullPath = NewFileFullName };
                    ImagesCollection[n].Copies.Remove(SelectedCopy);
                    ImagesCollection[n].Copies.Insert(m, image);
                    SelectedCopy = ImagesCollection[n].Copies[m];
                }
            }
            catch(Exception e) { MessageBox.Show(e.Message); }
        }

        private void RenameFiles(ImageModel imageToRename)
        {
            RenameFileWindow rFile = new RenameFileWindow(imageToRename);
            bool? dialogResult = rFile.ShowDialog();
            switch (dialogResult)
            {
                case true:
                    {
                        if (imageToRename.FullPath.Equals(SelectedImage.FullPath)) RenameImage();
                        else if (imageToRename.FullPath.Equals(SelectedCopy.FullPath)) RenameCopyImage();
                        break;
                    }
                case false:
                    { break; }
                default:
                    { break; }
            }
        }

        private void ThisIsOriginalImage()
        {
            ObservableCollection<ImageModel> copies = new ObservableCollection<ImageModel>();
            foreach (ImageModel im in selectedImage.Copies)
            {
                if (!im.FullPath.Equals(selectedCopy.FullPath)) copies.Add(im);
            }
            copies.Add(new ImageModel(selectedImage.Name, selectedImage.Directory, selectedImage.FullPath, selectedImage.Date_Time, selectedImage.Size));
            ImageModel image = new ImageModel(selectedCopy, copies);
            int i = ImagesCollection.IndexOf(selectedImage);
            ImagesCollection.Remove(selectedImage);
            ImagesCollection.Insert(i, image);
        }

        private void DeleteAllCopiesOfSelectedImage()
        {
            n = ImagesCollection.IndexOf(selectedImage);
            foreach (ImageModel im in ImagesCollection[n].Copies)
            {
                try
                {
                    string header = "Запрос на удаление файла.";
                    string message = "Удалить файл " + im.FullPath + " с диска?";
                    var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) File.Delete(im.FullPath);
                }
                catch (Exception e) { MessageBox.Show(e.Message); }
            }
            ImagesCollection[n].Copies.Clear();
            CollectionSize = ImagesViewModel.CalculateCollectionSize(ImagesCollection);
            NumberOfImages = ImagesViewModel.CountPictures(ImagesCollection);
        }

        private void DeleteAllCopies()
        {
            string header = "Запрос на удаление файлов.";
            string message = "Удалить все найденные копии с диска?";
            var result = MessageBox.Show(message, header, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (ImageModel im in ImagesCollection)
                {
                    List<ImageModel> copies = new List<ImageModel>(im.Copies);
                    foreach(ImageModel c in copies)
                    {
                        try
                        {
                            File.Delete(c.FullPath);
                            im.Copies.Remove(c);
                        }
                        catch (Exception e) { MessageBox.Show(e.Message); }
                    }
                }
            }
        }

        private void AddToChecked(string image)
        {
            if(image=="selectedImage")
            {
                if(SelectedImage!=null)
                {
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    ImagesCollection[n].ImageChecked = true;
                }
                List<ImageModel> list = ImagesCollection.Where(i => i.ImageChecked).ToList();
                CheckedImages.Clear();
                foreach (ImageModel im in list) CheckedImages.Add(im);
            }
            else if(image=="selectedCopy")
            {
                int n = ImagesCollection.IndexOf(SelectedImage);
                if (SelectedCopy!=null)
                {
                    int m = ImagesCollection[n].Copies.IndexOf(SelectedCopy);
                    ImagesCollection[n].Copies[m].ImageChecked = true;
                }
                List<ImageModel> list = ImagesCollection[n].Copies.Where(i => i.ImageChecked).ToList();
                CheckedCopies.Clear();
                foreach (ImageModel im in list) CheckedCopies.Add(im);
            }
        }

        private void RemoveFromChecked(string image)
        {
            if (image == "selectedImage")
            {
                if(SelectedImage!=null)
                {
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    ImagesCollection[n].ImageChecked = false;
                }
                List<ImageModel> list = ImagesCollection.Where(i => i.ImageChecked).ToList();
                CheckedImages.Clear();
                foreach (ImageModel im in list) CheckedImages.Add(im);
            }
            else if (image == "selectedCopy")
            {
                int n = ImagesCollection.IndexOf(SelectedImage);
                if(SelectedCopy!=null)
                {
                    int m = ImagesCollection[n].Copies.IndexOf(SelectedCopy);
                    ImagesCollection[n].Copies[m].ImageChecked = false;
                }
                List<ImageModel> list = ImagesCollection[n].Copies.Where(i => i.ImageChecked).ToList();
                CheckedCopies.Clear();
                foreach (ImageModel im in list) CheckedCopies.Add(im);
            }
        }

        private void CheckAll(string image)
        {
            if(image=="selectedImages")
            {
                foreach (ImageModel im in ImagesCollection)
                {
                    im.ImageChecked = true;
                    if (!CheckedImages.Contains(im)) CheckedImages.Add(im);
                }
            }
            else if(image=="selectedCopies")
            {
                if(SelectedImage!=null)
                {
                    int n = ImagesCollection.IndexOf(SelectedImage);
                    foreach(ImageModel im in ImagesCollection[n].Copies)
                    {
                        im.ImageChecked = true;
                        if (!CheckedCopies.Contains(im)) CheckedCopies.Add(im);
                    }
                }
            }
        }

        private void UncheckAll(string image)
        {
            if(image=="selectedImages")
            {
                CheckedImages.Clear();
                foreach (ImageModel im in ImagesCollection)
                {
                    im.ImageChecked = false;
                }
            }
            else if(image=="selectedCopies")
            {
                CheckedCopies.Clear();
                int n = ImagesCollection.IndexOf(SelectedImage);
                foreach(ImageModel im in ImagesCollection[n].Copies)
                {
                    im.ImageChecked = false;
                }
            }
        }
        #endregion

        #region INotifyoropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region BackgroundWorker

        public void InitializeBackgroundWorker()
        {
            Bgworker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            Bgworker.DoWork += new DoWorkEventHandler(Bgworker_DoWork);
            Bgworker.ProgressChanged += new ProgressChangedEventHandler(Bgworker_ProgressChanged);
            Bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bgworker_RunWorkerCompleted);
        }

        private void Bgworker_DoWork(object sender,DoWorkEventArgs e)
        {
            SearchButtonContent = "Остановить поиск";
            ImagesViewModel.GetImagesViews(FExt, Dir, Bgworker, e);
            e.Result = null;
            if (Bgworker.CancellationPending)
            { e.Cancel = true; e.Result = null; return; }
        }

        private void Bgworker_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                ImagesCollection.Add((ImageModel)e.UserState);
            }
            else
            {
                ImagesCollection.RemoveAt(e.ProgressPercentage);
                ImagesCollection.Insert(e.ProgressPercentage, (ImageModel)e.UserState);
            }
        }

        private void Bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsFilterEnabled = true;
            IsCheckAllEnabled = true;
            SearchButtonContent = "Новый поиск";
            SearchProgress = null;
            CopiesTotalSize = ImagesViewModel.CalculateCopiesTotalSize(ImagesCollection);
            NumberOfCopies = ImagesViewModel.CountNumberOfCopies(ImagesCollection);
        }

        #endregion

    }
}