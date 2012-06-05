using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDotImage;
using System.Net;
using System.IO;

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
                                            resize: 1024   
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
                                                waterMarkOpacity: 0.9
                                        );

                ImageHelper.SaveStream(_waterMarkedImage, "c:\\Temp\\test_watermarked.jpg");
                Assert.IsTrue(File.Exists("c:\\Temp\\test_watermarked.jpg"));
            }
        }
    }
}
