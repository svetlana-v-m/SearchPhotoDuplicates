using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchPhotoDuplicatesMVVM;
using SearchPhotoDuplicatesMVVM.Models;
using SearchPhotoDuplicatesMVVM.ViewModels;

namespace SearchPhotoDuplicatesMVVM.Tests
{
    [TestClass]
    public class CommandsTests
    {
        [TestMethod]
        public void FindImagesCommandTest()
        {
            ObservableCollection<ImageModel> testCollection = new ObservableCollection<ImageModel>();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            testCollection=mainWindowViewModel.Get
        }
    }
}
