using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RealEstateSolution.WpfClient.Utils
{
    public class CaptchaGenerator
    {
        private static readonly Random Random = new();
        private static readonly string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static (BitmapSource Image, string Code) Generate(int width = 120, int height = 40)
        {
            var code = GenerateCode(5);
            var drawingVisual = new DrawingVisual();

            using (var context = drawingVisual.RenderOpen())
            {
                // 绘制背景
                context.DrawRectangle(
                    new SolidColorBrush(Colors.White),
                    null,
                    new Rect(0, 0, width, height));

                // 添加干扰线
                for (int i = 0; i < 5; i++)
                {
                    var pen = new Pen(GetRandomBrush(), 1);
                    context.DrawLine(
                        pen,
                        new Point(Random.Next(width), Random.Next(height)),
                        new Point(Random.Next(width), Random.Next(height)));
                }

                // 添加干扰点
                for (int i = 0; i < 100; i++)
                {
                    var brush = GetRandomBrush();
                    context.DrawRectangle(
                        brush,
                        null,
                        new Rect(Random.Next(width), Random.Next(height), 2, 2));
                }

                // 绘制验证码文字
                var formattedText = new FormattedText(
                    code,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    height * 0.6,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip);

                // 对每个字符进行变形和旋转
                var charWidth = width / code.Length;
                for (int i = 0; i < code.Length; i++)
                {
                    var transform = new TransformGroup();
                    transform.Children.Add(new RotateTransform(Random.Next(-30, 30)));
                    transform.Children.Add(new TranslateTransform(
                        i * charWidth + (charWidth - formattedText.Width / code.Length) / 2,
                        (height - formattedText.Height) / 2 + Random.Next(-5, 5)));

                    context.PushTransform(transform);
                    context.DrawText(
                        new FormattedText(
                            code[i].ToString(),
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            height * 0.6,
                            GetRandomBrush(),
                            VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip),
                        new Point(0, 0));
                    context.Pop();
                }
            }

            var bitmap = new RenderTargetBitmap(
                width, height,
                96, 96,
                PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            return (bitmap, code);
        }

        private static string GenerateCode(int length)
        {
            var code = string.Empty;
            for (int i = 0; i < length; i++)
            {
                code += Characters[Random.Next(Characters.Length)];
            }
            return code;
        }

        private static Brush GetRandomBrush()
        {
            return new SolidColorBrush(Color.FromRgb(
                (byte)Random.Next(0, 255),
                (byte)Random.Next(0, 255),
                (byte)Random.Next(0, 255)));
        }
    }
} 