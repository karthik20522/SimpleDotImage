SimpleDotImage
==============

Simple Image Processing library using WIC (Windows Imaging Component). CSharp based Image Processing library to perform simple image processing operation like Image Resizing, Image Watermarking etc.

Example usage:

Resize:
	using (var _imgProcessing = new ImageProcessing())
	{
                	var _resizedImage = _imgProcessing.Process(  
                                            imagePath: _testFileName, 
                                            resize: 1024   
                                    );

	                ImageHelper.SaveStream(_resizedImage, "c:\\Temp\\test_resized.jpg");
                	Assert.IsTrue(File.Exists("c:\\Temp\\test_resized.jpg"));
	}

Watermark:
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