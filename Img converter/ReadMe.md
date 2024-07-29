1. Move all JPG and PNG files to the folder to be converted

2. Open the exe file, where 0 represents grayscale inversion, 1 represents transparent background, and 2 represents bold line draft.
  If it is a grayscale inversion operation, you need to input whether it is inverted (0 is No, 1 is Yes)
  If it is a transparent background operation, you need to input the transparency value (integer range 0-765, any RGB values in the image that are greater than the transparency value will be set as transparent)
  If it is a bold line draft operation, a bold value (positive integer, determines the number of iterations, recommended within 5) needs to be entered

3. Wait for completion, the new image will end with '_new' and be saved next to the original image

  

  PS: Transparent background operation will expand the image to four channel PNG format to ensure transparency