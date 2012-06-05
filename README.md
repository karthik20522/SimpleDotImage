SimpleDotImage
==============

Simple Image Processing library using WIC (Windows Imaging Component). CSharp based Image Processing library to perform simple image processing operation like Image Resizing, Image Watermarking etc.

Example usage:

Resize:
		
                	var _resizedImage = _imgProcessing.Process(  
                                            imagePath: _testFileName, 
                                            resize: 1024   
                                    );

Watermark using Image:
	
		    var _waterMarkedImage = _imgProcessing.Process(
                	                    imagePath: _testFileName,
                                	    resize:1024,
	                                    waterMarkPath: _waterMarkFileName,
	                                    waterMarkOpacity: 0.9
	                    ); 	

Watermark using Text:

		var _waterMarkedImage = _imgProcessing.Process(
				imagePath: _testFileName,
				resize: 1024,
				waterMarkText: "karthik20522",
				waterMarkOpacity: 0.2
		);
* Review test cases for further implementation how-to's