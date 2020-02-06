using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchPhotoDuplicatesMVVM.Models;
using SearchPhotoDuplicatesMVVM.ViewModels;

namespace UnitTestProject
{
    [TestClass]
    public class ImagesViewModelTests
    {
        static ImageModel image1 = new ImageModel(@"image1.jpg", @"C:\TestFolder", @"C:\TestFolder\image1.jpg", new DateTime(2015, 05, 01,12,00,30), 200000);
        static ImageModel image2 = new ImageModel(@"image2.jpg", @"C:\TestFolder", @"C:\TestFolder\image2.jpg", new DateTime(2015, 05, 02, 11, 10, 00), 200400);
        static ImageModel image3 = new ImageModel(@"image3.jpg", @"C:\TestFolder", @"C:\TestFolder\image3.jpg", new DateTime(2013, 04, 01, 17, 50, 10), 201400);
        static ImageModel image4 = new ImageModel(@"image4.jpg", @"C:\TestFolder", @"C:\TestFolder\image4.jpg", new DateTime(2017, 03, 02, 18, 02, 00), 230000);
        static ImageModel image5 = new ImageModel(@"image5.jpg", @"C:\TestFolder", @"C:\TestFolder\image5.jpg", new DateTime(2013, 01, 02, 13, 40, 50), 205670);
        static ObservableCollection<ImageModel> Image1Copies = new ObservableCollection<ImageModel>
        {
            new ImageModel(@"copyImage1.jpg", @"C:\TestFolder\Copies", @"C:\TestFolder\Copies\copyImage1.jpg", new DateTime(2015, 05, 01, 12, 00, 30), 200000),
            new ImageModel(@"copyImage1(1).jpg", @"C:\TestFolder\Copies\Copies", @"C:\TestFolder\Copies\Copies\copyImage1.jpg", new DateTime(2015, 05, 01, 12, 00, 30), 200000)
        };
        static ImageModel copyImage2 = new ImageModel(@"copyImage2.jpg", @"C:\TestFolder\Copies", @"C:\TestFolder\Copies\copyImage2.jpg", new DateTime(2015, 05, 02, 11, 10, 00), 200400);
        static ImageModel copyImage3 = new ImageModel(@"copyImage3.jpg", @"C:\TestFolder\Copies", @"C:\TestFolder\Copies\copyImage3.jpg", new DateTime(2013, 04, 01, 17, 50, 10), 201400);
        static ImageModel copyImage4 = new ImageModel(@"copyImage4.jpg", @"C:\TestFolder\Copies", @"C:\TestFolder\Copies\copyImage4.jpg", new DateTime(2017, 03, 02, 18, 02, 00), 230000);


        ObservableCollection<ImageModel> testCollection = new ObservableCollection<ImageModel>
        {
            new ImageModel(image1,Image1Copies),
            new ImageModel(image2,copyImage2),
            new ImageModel(image3,copyImage3),
            new ImageModel(image4,copyImage4),
            new ImageModel(image5)
        };
        
        [TestMethod]
        public void CalculateCollectionSizeTest()
        {
            string collectionSize = ImagesViewModel.CalculateCollectionSize(testCollection);
            Assert.AreEqual("1,9734 МБ", collectionSize);
        }

        [TestMethod]
        public void CalculateCopiesTotalSizeTest()
        {
            string collectionCopiesSize = ImagesViewModel.CalculateCopiesTotalSize(testCollection);
            Assert.AreEqual("Объем копий: 1007,6172 КБ", collectionCopiesSize);
        }

        [TestMethod]
        public void CountNumberOfCopiesTest()
        {
            string collectionCopies = ImagesViewModel.CountNumberOfCopies(testCollection);
            Assert.AreEqual("Всего копий: 5", collectionCopies);
        }

        [TestMethod]
        public void CountPicturesTest()
        {
            string collection = ImagesViewModel.CountPictures(testCollection);
            Assert.AreEqual("10", collection);
        }
    }
}
