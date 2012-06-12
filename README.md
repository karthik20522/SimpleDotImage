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
						waterMarkText: "karthik20522",
						waterMarkOpacity: 0.2
		);
		
Watermark positioning:

		 var _waterMarkedImage = _imgProcessing.Process(
                                                imagePath: _testFileName,
                                                waterMarkText: "karthik20522",
                                                waterMarkOpacity: 0.3,
                                                waterMarkPosition: WaterMarkPosition.BottomRight 
                                                // TopLeft, TopRight, BottomRight, BottomLeft, Center
                                        );

Watermark using Both Text and Image:

            var _waterMarkedImage = _imgProcessing.Process(
            							imagePath: _testFileName,
        								resize: 1024,
        								waterMarkPath: _waterMarkFileName,
        								waterMarkText: "karthik20522",
        								waterMarkOpacity: 0.3
        						);

Rotate Image:

             var _rotatedImage = _imgProcessing.Process(
            								imagePath: _testFileName,
        									rotate: Rotation.Rotate90
        							);

Flip Image:

        var _flipImage = _imgProcessing.Process(
    							imagePath: _testFileName,
								flipHorizontal: true,
								flipVertical: false
						);

Convert To Gray Scale:

    var _grayImage = _imgProcessing.Process(
    							imagePath: _testFileName,
								colorFormat: ColorFormat.Gray //Gray, BlackAndWhite, RGB, BGR, CMYK
						);


* Review test cases for further implementation how-to's