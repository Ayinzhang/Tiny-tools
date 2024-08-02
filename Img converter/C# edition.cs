using System.Drawing;
using System.Drawing.Imaging;

string directory = Directory.GetCurrentDirectory(); string[] files = Directory.GetFiles(directory);
Console.WriteLine("Choose Instruction:\n 0 - Grey\n 1 - Transparent\n 2 - Stroke"); int cnt = int.Parse(Console.ReadLine());

switch (cnt)
{
    case 0: 
        Console.WriteLine("Input if reverse (0 for normal, 1 for reverse): ");
        int reverse = int.Parse(Console.ReadLine());
        foreach (string file in files)
            if (file.EndsWith(".jpg") || file.EndsWith(".png"))
            {
                Bitmap src = new Bitmap(file);
                for (int i = 0; i < src.Height; i++)
                    for (int j = 0; j < src.Width; j++)
                    {
                        Color pixelColor = src.GetPixel(j, i);
                        int grayValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        if (reverse == 1) grayValue = 255 - grayValue;
                        Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                        src.SetPixel(j, i, grayColor);
                    }
                src.Save(file.Replace(".jpg", "_new.png").Replace(".png", "_new.png"), ImageFormat.Png);
            }
        break;
    case 1: 
        Console.WriteLine("Input transparent value: ");
        int transparentValue = int.Parse(Console.ReadLine());
        foreach (string file in files)
            if (file.EndsWith(".jpg") || file.EndsWith(".png"))
            {
                Bitmap src = new Bitmap(file), dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                for (int i = 0; i < src.Height; i++)
                    for (int j = 0; j < src.Width; j++)
                    {
                        Color pixelColor = src.GetPixel(j, i);
                        int sumValue = pixelColor.R + pixelColor.G + pixelColor.B;
                        if (sumValue > transparentValue) dst.SetPixel(j, i, Color.FromArgb(0, 255, 255, 255));
                        else dst.SetPixel(j, i, Color.FromArgb(255, pixelColor.R, pixelColor.G, pixelColor.B));
                    }
                dst.Save(file.Replace(".jpg", "_new.png").Replace(".png", "_new.png"), ImageFormat.Png); 
            }
        break;
    case 2:
        Console.WriteLine("Input stroke value: ");
        int strokeValue = int.Parse(Console.ReadLine());
        foreach (string file in files)
            if (file.EndsWith(".jpg") || file.EndsWith(".png"))
            {
                Bitmap src = new Bitmap(file), dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                long[,] tmp = new long[src.Width, src.Height];

                for (int t = 0; t < strokeValue; t++)
                {
                    for (int i = 0; i < src.Height; i++)
                        for (int j = 0; j < src.Width; j++)
                        {
                            int pixelSum = src.GetPixel(j, i).R + src.GetPixel(j, i).G + src.GetPixel(j, i).B;
                            tmp[j, i] = 765L - pixelSum + (i > 0 ? tmp[j, i - 1] : 0)
                                        + (j > 0 ? tmp[j - 1, i] : 0) - (i > 0 && j > 0 ? tmp[j - 1, i - 1] : 0);
                        }

                    for (int i = 0; i < src.Height; i++)
                        for (int j = 0; j < src.Width; j++)
                        {
                            int li = Math.Max(i - 1, 0), ri = Math.Min(i + 1, src.Height - 1),
                                lj = Math.Max(j - 1, 0), rj = Math.Min(j + 1, src.Width - 1);
                            long areaSum = tmp[rj, ri] - (li > 0 ? tmp[lj, ri] : 0)
                                          - (lj > 0 ? tmp[rj, li] : 0) + (li > 0 && lj > 0 ? tmp[lj, li] : 0),
                                 numPixels = (ri - li + 1) * (rj - lj + 1), average = areaSum / numPixels;
                            Color pixelColor = src.GetPixel(j, i);
                            (int r, int g, int b) = (Math.Max(pixelColor.R - (int)average, 0), 
                                Math.Max(pixelColor.G - (int)average, 0), Math.Max(pixelColor.B - (int)average, 0));
                            dst.SetPixel(j, i, Color.FromArgb(pixelColor.A, r, g, b));
                        }
                    Console.WriteLine("Loop " + (t + 1) + " complete");

                    if (t == strokeValue - 1)
                    {
                        for (int i = 0; i < dst.Height; i++)
                            for (int j = 0; j < dst.Width; j++)
                            {
                                Color pixelColor = dst.GetPixel(j, i);
                                if (pixelColor.R + pixelColor.G + pixelColor.B < 100) dst.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0));
                                else if (src.GetPixel(j, i).A == 0) dst.SetPixel(j, i, Color.FromArgb(0, 255, 255, 255));
                            }
                        dst.Save(file.Replace(".jpg", "_new.png").Replace(".png", "_new.png"), ImageFormat.Png);
                    }
                }
            }
        break;
}

Console.WriteLine("Instruction finished");