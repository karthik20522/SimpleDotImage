using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDotImage;
using System.Net;
using System.IO;
using System.Windows.Media.Imaging;

namespace SimpleDotImageTest
{
    [TestClass]
    public class ImageProcessingTest
    {
        string _testImageUrl = "http://farm8.staticflickr.com/7153/6528656357_9389da3940_b.jpg";
        string _waterMarkUrl = "http://koivi.com/archives/php-gd-image-watermark/watermarks/Sample-trans.png";

        string _testFileName = "c:\\Temp\\test_original.jpg";
        string _waterMarkFileName = "c:\\Temp\\test_waterMark.png";

        [TestInitialize]
        public void Init()
        {
            var webClient = new WebClient();
            webClient.DownloadFile(_testImageUrl, _testFileName);
            webClient.DownloadFile(_waterMarkUrl, _waterMarkFileName);
        }

        [TestMethod]
        public void Resize_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _resizedImage = _imgProcessing.Process(  
                                            imagePath: _testFileName, 
                                            resize: 640   
                                    );

                ImageHelper.SaveStream(_resizedImage, "c:\\Temp\\test_resized.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_resized.jpg"));
            }
        }

        [TestMethod]
        public void WaterMark_Image_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _waterMarkedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                resize:1024,
                                                waterMarkPath: _waterMarkFileName,
                                                waterMarkOpacity: 0.5
                                        );

                ImageHelper.SaveStream(_waterMarkedImage, "c:\\Temp\\test_watermarked.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_watermarked.jpg"));
            }
        }

        [TestMethod]
        public void WaterMark_Text_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _waterMarkedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                waterMarkText: "karthik20522",
                                                waterMarkOpacity: 0.5
                                        );

                ImageHelper.SaveStream(_waterMarkedImage, "c:\\Temp\\test_watermarked.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_watermarked.jpg"));
            }
        }

        [TestMethod]
        public void WaterMark_Text_Position_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _waterMarkedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                waterMarkText: "karthik20522",
                                                waterMarkOpacity: 0.3,
                                                waterMarkPosition: WaterMarkPosition.BottomRight
                                        );

                ImageHelper.SaveStream(_waterMarkedImage, "c:\\Temp\\test_watermarked.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_watermarked.jpg"));
            }
        }

        [TestMethod]
        public void WaterMark_Text_And_Image_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _waterMarkedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                resize: 1024,
                                                waterMarkPath: _waterMarkFileName,
                                                waterMarkText: "karthik20522",
                                                waterMarkOpacity: 0.3
                                        );

                ImageHelper.SaveStream(_waterMarkedImage, "c:\\Temp\\test_watermarked.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_watermarked.jpg"));
            }
        }

        [TestMethod]
        public void Rotate_Image_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _rotatedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                rotate: Rotation.Rotate90
                                        );

                ImageHelper.SaveStream(_rotatedImage, "c:\\Temp\\test_rotated.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_rotated.jpg"));
            }
        }

        [TestMethod]
        public void Flip_Image_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _flipImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                flipHorizontal: true,
                                                flipVertical: false
                                        );

                ImageHelper.SaveStream(_flipImage, "c:\\Temp\\test_flip.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_flip.jpg"));
            }
        }

        [TestMethod]
        public void GrayScale_Image_Test()
        {
            using (var _imgProcessing = new ImageProcessing())
            {
                var _grayImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                colorFormat: ColorFormat.Gray //Gray, BlackAndWhite, RGB, BGR, CMYK 
                                        );

                ImageHelper.SaveStream(_grayImage, "c:\\Temp\\test_gray.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_gray.jpg"));
            }
        }
    }
}
