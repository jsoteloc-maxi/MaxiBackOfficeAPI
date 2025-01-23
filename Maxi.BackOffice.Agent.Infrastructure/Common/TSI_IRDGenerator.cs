using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace Maxi.BackOffice.Agent.Infrastructure.Common
{

    [SupportedOSPlatform("windows")]
    public class TSI_IRDGenerator
    {
        //int vfIRDHeight;
        //int vfIRDWidth;
        //int vfClearBandH;

        StringFormat genericTypoFormat = new StringFormat(StringFormat.GenericTypographic); //evitar demasiado espacio en textos

        RectangleF vgRectFull;
        //RectangleF vgRectCheck;
        //RectangleF vgRectSide;
        //RectangleF vgRectBottom;

        //Font vgFontMicr;
        //Font vgFontCourier;
        //Font vgFontArial;
        //Font vgFontB4;

        //-----------------
        RectangleF F1_Rect;
        RectangleF F2_Rect;
        RectangleF F3_Rect;
        RectangleF F4_Rect;
        RectangleF F5_Rect;
        RectangleF F7_Rect;
        RectangleF F8_Rect;

        //RectangleF B1_RectFull;
        RectangleF B1_RectImage;
        //RectangleF B1_RectA;
        //RectangleF B1_RectB;
        //RectangleF B1_RectC;
        //RectangleF B2_Rect;
        //-----------------


        public string Text_MICR { get; set; } = "";
        public string Text_ReturnReasonCode { get; set; }
        public string Text_ReturnReasonName { get; set; }
        public string Text_ReturnReasonAbbreviation { get; set; }

        public string Text_F2_Secuence { get; set; }
        public string Text_F2_RoutingNum { get; set; } //de la institucion del cheque original ??
        public string Text_F2_InstBusinessDate { get; set; }

        public string Text_F3_InstRoutingNum { get; set; }
        public string Text_F3_Secuence { get; set; }
        public string Text_F3_InstBusinessDate { get; set; }

        public List<string> Text_F8 { get; set; } = new List<string>();


        Bitmap fImageF = null;
        Bitmap fImageR = null;

        public bool vgDebug = true;

        public Bitmap IRDImageF { get; set; }
        public Bitmap IRDImageR { get; set; }

        //public string TxtMICR { get; set; } = "";
        //public List<string> TxtB1 { get; private set; }
        //public List<string> TxtB2 { get; private set; }
        //public List<string> TxtB3 { get; private set; }
        //public List<string> TxtB4 { get; private set; }
        public List<string> TxtB5 { get; private set; }


        public TSI_IRDGenerator()
        {
            //TxtB1 = new List<string>();
            //TxtB2 = new List<string>();
            //TxtB3 = new List<string>();
            //TxtB4 = new List<string>();
            TxtB5 = new List<string>();

            //SetupSizes01();
        }


        public void SetCheckImages(Image imgF, Image imgR)
        {
            fImageF = null;
            if (imgF != null)
                fImageF = TiffUtil.ConvertToEditable(imgF);

            fImageR = null;
            if (imgR != null)
                fImageR = TiffUtil.ConvertToEditable(imgR);
        }

        public void LoadCheckImage(string cual, string filename)
        {
            var m = Image.FromFile(filename);

            if (cual.ToUpper() == "F")
                fImageF = TiffUtil.ConvertToEditable(m);
            else
                fImageR = TiffUtil.ConvertToEditable(m);
        }

        public int GenImagesIRD()
        {
            int r = 0;
            IRDImageF = null;
            IRDImageR = null;

            //if (fImageF != null)
                r = RenderImageIRD("F");

            //if (fImageR != null)
                r = RenderImageIRD("R");

            return r;
        }

        public void SaveImage(string acual, string location)
        {
            if(acual.ToUpper() == "F")
            {
                //IRDImageF.Save(location, ImageFormat.Tiff, );
                
                TiffUtil.SaveToTiff(IRDImageF, location);

                //TiffUtil.SaveToTiff(IRDImageF, sPath + "IRD T2 Front.tif");

                //TifUtil_03.SaveToTiff(g.IRDImageF, sPath + "IRD_Front T3.tif");
                //picBox3.Image = g.IRDImageF;
            }
            else
            {
                TiffUtil.SaveToTiff(IRDImageR, location);
                //IRDImageR.Save(location, ImageFormat.Tiff);
            }
        }

        public MemoryStream SaveImageToStream(string acual)
        {
            var m = new MemoryStream();

            if (acual.ToUpper() == "F")
                TiffUtil.SaveToTiff(IRDImageF, m);
            else
                TiffUtil.SaveToTiff(IRDImageR, m);

            m.Position = 0;
            return m;
        }

        
        //private void SetupSizes01()
        //{
        //    vfIRDWidth = 1677;  //797;
        //    vfIRDHeight = 600;  //308;
        //    vfClearBandH = 65;  //40

        //    CalcAreaRectangles();

        //    vgFontMicr = new Font("MICR Encoding", 17, FontStyle.Bold);
        //    vgFontCourier = new Font("Courier New", 9);
        //    vgFontArial = new Font("Arial", 8);
        //    vgFontB4 = new Font("Arial", 12, FontStyle.Bold);

        //    //Bloque superior
        //    //vfB1_FontSize = 10;
        //    //vfB1_LeftMargin = 10;

        //    ////Leyenda
        //    //vfB2_FontSize = 8;

        //    ////Leyenda Return Reason
        //    //vfB3_FontSize = 8;

        //    //Return Reason en cheque
        //    //vfB4_FontSize = 18;

        //    ////Text 90 grados
        //    //vfB5_FontSize = 12;
        //}


        //private void _Rect(Rectangle r, int x, int y, int w, int h)
        //{
        //    r.X = x;
        //    r.Y = y;
        //    r.Width = w;
        //    r.Height = h;
        //}


        //private int CreateImageIRD(string cual)
        //{
        //    //Crear bitmap area de dibujo

        //    RectangleF r;
        //    cual = cual.ToUpper();

        //    Bitmap original = fImageR;
        //    if (cual.ToUpper() == "F")
        //        original = fImageF;

        //    if (original == null)
        //        throw new Exception("Error, original check image not assigned");

        //    var bmp = new Bitmap(vfIRDWidth, vfIRDHeight, PixelFormat.Format32bppArgb);
        //    bmp.SetResolution(original.HorizontalResolution, original.VerticalResolution);
        //    using (Graphics canvas = Graphics.FromImage(bmp))
        //    {
        //        //canvas.PageUnit = GraphicsUnit.Pixel;
        //        //canvas.SmoothingMode = SmoothingMode.None;

        //        canvas.FillRectangle(Brushes.White, vgRectFull);

        //        if (cual == "R")
        //        {
        //            //cambio de lugar los rect
        //            vgRectCheck.X = vgRectSide.X;
        //            vgRectSide.X = vgRectCheck.Right + 1;
        //        }

        //        if (vgDebug)
        //        {
        //            canvas.FillRectangle(Brushes.Red, vgRectFull);
        //            canvas.FillRectangle(Brushes.LightGreen, vgRectSide);
        //            canvas.FillRectangle(Brushes.Yellow, vgRectBottom);
        //            canvas.FillRectangle(Brushes.LightGray, vgRectCheck);
        //        }

        //        //r = new Rectangle(vlLeftMargin, vlTopMargin, vlMaxCheckWidth, vlMaxCheckHeight);
        //        //MapDrawing(canvas,new RectangleF(0,0,original.Width,original.Height), r, false);
        //        //canvas.DrawImage(original, r);

        //        //Calcular el rectangulo escalado en base al ancho
        //        r = new RectangleF(0, 0, original.Width, original.Height);
        //        r = _StretchRect(r, vgRectCheck.Width, -1);

        //        //Si el margen resultante superior/inferior es menor a cierto tamaño
        //        //=> volvemos a escalar pero ahora en base a el alto
        //        if (r.Height > vgRectCheck.Height)
        //        {
        //            r = new RectangleF(0, 0, original.Width, original.Height);
        //            r = _StretchRect(r, -1, vgRectCheck.Height);
        //        }

        //        //Coloca rectangulo dentro del area de dibujo
        //        r.X = vgRectCheck.X + (vgRectCheck.Width - r.Width) / 2;
        //        r.Y = vgRectCheck.Y + (vgRectCheck.Height - r.Height) / 2;
        //        canvas.DrawImage(original, r.X, r.Y, r.Width, r.Height);

        //        //var imgRect = Rectangle.Truncate(vgRectCheck);
        //        //canvas.DrawImageUnscaled(original, imgRect.X, imgRect.Y, imgRect.Width, imgRect.Height);

        //        if (cual == "F")
        //        {
        //            PintaAreaMicr(canvas);
        //            PintaAreaLateral(canvas);
        //        }
        //        else
        //        {
        //            PintaAreaLateralAtras(canvas);
        //        }

        //    }


        //    if (cual.ToUpper() == "F")
        //        IRDImageF = bmp;
        //    else
        //        IRDImageR = bmp;

        //    return 0;
        //}


        //private void PintaAreaMicr(Graphics canvas)
        //{
        //    var font = vgFontMicr;
        //    var s = TxtMICR;
        //    var m = canvas.MeasureString(s, font);

        //    //ver como alinear el micr en el area de abajo

        //    var x = vgRectBottom.Width - m.Width - 20;
        //    //var y = vgRectBottom.Top + ((vgRectBottom.Height - (m.Height-4)) / 2); //centrado vertical
        //    var y = vgRectBottom.Bottom - m.Height - 12; //se alinea de abajo hacia arriba, poner un margen

        //    canvas.DrawString(s, font, Brushes.Black, x, y);
        //}


        private void PintaAreaLateral(Graphics canvas)
        {
            /*
            float x, y;
            Font font;
            
            //rect para texto horizontal
            var r1 = new RectangleF
            {
                X = vgRectSide.X,
                Y = vgRectSide.Y,
                Width = (vgRectSide.Width / 6f) * 5,
                Height = vgRectSide.Height,
            };
            
            //rect para texto vertical
            var r2 = new RectangleF
            {
                X = r1.Right,
                Y = r1.Y,
                Width = vgRectSide.Width - r1.Width,
                Height = r1.Height,
            };
            if (vgDebug) canvas.FillRectangle(Brushes.LightCoral, r2);


            //pos inicial
            x = r1.Left + 5; // (float)vfB1_LeftMargin;
            y = r1.Top + 5;


            //--------------------------
            //Bloque 1
            font = vgFontCourier;
            foreach(var s in TxtB1)
            {
                var m = canvas.MeasureString(s, font);
                x = (r1.Width - m.Width) / 2; //centrado
                if (vgDebug) canvas.DrawRectangle(new Pen(Color.Red, 1), x, y, m.Width, m.Height);
                canvas.DrawString(s, font, Brushes.Black, x, y);
                y += m.Height - 4;
            }


            //--------------------------
            //B2 Leyenda
            if (TxtB2.Count == 0)
            {
                TxtB2.Add("This is a LEGAL COPY of your");
                TxtB2.Add("check. You can use it the same");
                TxtB2.Add("way you would use the original");
                TxtB2.Add("check.");
            }

            y += 8;
            font = vgFontArial;

            foreach (var s in TxtB2)
            {
                var m = canvas.MeasureString(s, font);
                x = Math.Min(x, (r1.Width - m.Width) / 2);
            }
            x = (x > 1) ? x : 5;

            foreach (var s in TxtB2)
            {
                var m = canvas.MeasureString(s, font);
                if (vgDebug) canvas.DrawRectangle(new Pen(Color.Red, 1), x, y, m.Width, m.Height);
                canvas.DrawString(s, font, Brushes.Black, x, y);
                y += m.Height;
            }



            //---------------
            //B3 Return Reason
            y += 10;
            foreach (var s in TxtB3)
            {
                var m = canvas.MeasureString(s, font);
                x = (r1.Width - m.Width) / 2;
                if (vgDebug) canvas.DrawRectangle(new Pen(Color.Red, 1), x, y, m.Width, m.Height);
                canvas.DrawString(s, font, Brushes.Black, x, y);
                y += m.Height;
            }


            //---------------
            //B4  Return Reason en Cheque
            font = vgFontB4;
            x = vgRectCheck.Left + vgRectCheck.Width / 2;
            y = vgRectCheck.Top + vgRectCheck.Height / 2;
            var i = 0;
            foreach (var s in TxtB3)
            {
                var m = canvas.MeasureString(s, font);

                if (i == 0)
                {
                    x -= m.Width / 2 + m.Height / 2;
                    //y -= m.Width / m.Height;
                }

                //x = vgRectCheck.Left + (vgRectCheck.Width / 2);


                DrawRotatedTextAt(canvas, -20, s, x, y, font, Brushes.Black);
                //canvas.DrawString(s, font, Brushes.Black, x, y);
                y += m.Height - 3;
                x += m.Height / 2;
                i++;
            }


            //---------------
            //B5 Texto 90 grados
            x = r2.Left + 3;
            y = r2.Bottom;
            font = vgFontCourier;
            foreach (var s in TxtB5)
            {
                var m = canvas.MeasureString(s, font);

                //centrar
                y = r2.Bottom - (r2.Height - m.Width) / 2f;

                DrawRotatedTextAt(canvas, -90f, s, x, y, font, Brushes.Black);
                x += m.Height;
            }
            */
        }


        private void PintaAreaLateralAtras(Graphics canvas)
        {
            /*
            float x, y;
            Font font;
            string s;

            s = "Do not endorse or write below this line.";
            var m = canvas.MeasureString(s, vgFontArial);

            x = vgRectBottom.Right - m.Width;
            y = vgRectBottom.Top - m.Height;
            canvas.DrawString(s, vgFontArial, Brushes.Black, x, y);
            canvas.DrawLine(new Pen(Brushes.Black), 5, vgRectBottom.Top, vgRectBottom.Right - 5, vgRectBottom.Top);


            var r1 = vgRectFull;
            r1.Height = vgRectSide.Height;


            //B5 Texto 90 grados atras
            x = r1.Right;
            y = r1.Bottom;
            font = vgFontCourier;
            foreach (var txt in TxtB5)
            {
                s = txt.Replace("[", ">").Replace("]", "<");
                m = canvas.MeasureString(s, font);
                //centrar vertical
                y = (r1.Height - m.Width) / 2;
                x -= m.Height - 5;
                DrawRotatedTextAt(canvas, 90, s, x, y, font, Brushes.Black);
            }
            */
        }


        private RectangleF _StretchRect(RectangleF r, float width, float height)
        {
            var newRect = r; //RectangleF es struct
            //var newRect = new RectangleF(r.Location, r.Size);

            if (width > 0)
            {
                newRect.Width = width;

                if (height > 0)
                    newRect.Height = height;

                if (height == -1)
                    newRect.Height = (width * r.Height) / r.Width;
            }
            else if (height > 0)
            {
                newRect.Height = height;

                if (width > 0)
                    newRect.Width = width;

                if (width == -1)
                    newRect.Width = (height * r.Width) / r.Height;
            }

            return newRect;
        }



        // Map a drawing coordinate rectangle to
        // a graphics object rectangle.
        //private void MapDrawing(Graphics gr, RectangleF drawing_rect, RectangleF target_rect, bool stretch)
        //{
        //    if (target_rect.Width < 1 ||
        //        target_rect.Height < 1) return;

        //    gr.ResetTransform();

        //    // Center the drawing area at the origin.
        //    float drawing_cx = (drawing_rect.Left + drawing_rect.Right) / 2;
        //    float drawing_cy = (drawing_rect.Top + drawing_rect.Bottom) / 2;
        //    gr.TranslateTransform(-drawing_cx, -drawing_cy);

        //    // Scale.
        //    // Get scale factors for both directions.
        //    float scale_x = target_rect.Width / drawing_rect.Width;
        //    float scale_y = target_rect.Height / drawing_rect.Height;
        //    if (!stretch)
        //    {
        //        // To preserve the aspect ratio,
        //        // use the smaller scale factor.
        //        scale_x = Math.Min(scale_x, scale_y);
        //        scale_y = scale_x;
        //    }
        //    gr.ScaleTransform(scale_x, scale_y, MatrixOrder.Append);

        //    // Translate to center over the drawing area.
        //    float graphics_cx = (target_rect.Left + target_rect.Right) / 2;
        //    float graphics_cy = (target_rect.Top + target_rect.Bottom) / 2;
        //    gr.TranslateTransform(graphics_cx, graphics_cy, MatrixOrder.Append);
        //}


        private void DrawRotatedTextAt(Graphics gr, float angle, string txt, float x, float y, Font the_font, Brush the_brush)
        {
            // Save the graphics state.
            GraphicsState state = gr.Save();
            gr.ResetTransform();

            // Rotate.
            gr.RotateTransform(angle);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            gr.TranslateTransform(x, y, MatrixOrder.Append);

            // Draw the text at the origin.
            gr.DrawString(txt, the_font, the_brush, 0, 0, genericTypoFormat);

            // Restore the graphics state.
            gr.Restore(state);
        }



        private void CalcRegions(float argDPI)
        {
            //Calcula rectangulos de area segun la especificacion

            //int dpi = 200; //ver si el dpi se pasa como parametro
            float dpi = argDPI;

            float x, y, w, h; //auxiliares para calculos

            float pxFullWidth = 8.5f * dpi;
            float pxFullHeight = 3.667f * dpi;
            float pxTopMargin = 0.2f * dpi; //No Print Area
            float pxMargin = 0.250f * dpi;


            pxFullWidth = (float)Math.Ceiling(pxFullWidth);
            pxFullHeight = (float)Math.Ceiling(pxFullHeight);

            //Area total
            vgRectFull = new RectangleF(0, 0, pxFullWidth, pxFullHeight);

            //Region F1 Imagen
            w = 5.750f * dpi;
            h = 2.750f * dpi;
            x = pxFullWidth - (w + pxMargin);
            y = pxTopMargin;
            F1_Rect = new RectangleF(x, y, w, h);

            //Region F2 Texto Vertical
            w = 0.400f * dpi;
            h = F1_Rect.Height;
            x = F1_Rect.Left - w;
            y = F1_Rect.Top;
            F2_Rect = new RectangleF(x, y, w, h);

            //Region F3 Creation Institution
            w = 1.850f * dpi;
            h = 0.460f * dpi;
            x = F2_Rect.Left - w;
            y = F2_Rect.Top;
            F3_Rect = new RectangleF(x, y, w, h);

            //Region F4 Legend
            w = F3_Rect.Width;
            h = 0.575f * dpi;
            x = F3_Rect.Left;
            y = F3_Rect.Bottom;
            F4_Rect = new RectangleF(x, y, w, h);

            //Region F7 Return Reason
            w = F3_Rect.Width;
            h = 0.475f * dpi;
            x = F4_Rect.Left;
            y = F4_Rect.Bottom;
            F7_Rect = new RectangleF(x, y, w, h);

            //Region F8 Optional Data
            w = F3_Rect.Width;
            h = F2_Rect.Height - (F3_Rect.Height + F4_Rect.Height + F7_Rect.Height);
            x = F7_Rect.Left;
            y = F7_Rect.Bottom;
            F8_Rect = new RectangleF(x, y, w, h);

            //Region F5 MICR Clear Band
            w = pxFullWidth;
            h = 0.7175f * dpi;
            x = 0;
            y = F1_Rect.Bottom;
            F5_Rect = new RectangleF(x, y, w, h);


            //Region B1 Imagen
            w = F1_Rect.Width;
            h = F1_Rect.Height;
            x = vgRectFull.Right - F1_Rect.Right;
            y = F1_Rect.Y;
            B1_RectImage = new RectangleF(x, y, w, h);


        }


        private void RenderStrList(string anchorType, Graphics canvas, List<string> strList, RectangleF rect, Font font, int linespace = 0, int margin = 0)
        {
            float x, y, h;

            h = 0;
            foreach (var s in strList)
            {
                var m = canvas.MeasureString(s, font);
                h += (m.Height + linespace);
            }

            if (h > 0)
                h -= 2;

            y = rect.Top + ((rect.Height - h) / 2); //centrado vertical

            if (anchorType == "T")
                y = rect.Top + margin;

            if (anchorType == "B")
                y = rect.Bottom - (h + margin);
           
            if (y < rect.Top) y = rect.Top + 1;

            foreach (var s in strList)
            {
                var m = canvas.MeasureString(s, font);
                x = rect.Left + ((rect.Width - m.Width) / 2); //centrado

                if (vgDebug) canvas.DrawRectangle(new Pen(Color.Red, 1), x, y, m.Width, m.Height);

                canvas.DrawString(s, font, Brushes.Black, x, y);
                y += (m.Height + linespace);
            }
        }



        private int RenderImageIRD(string cual)
        {
            //Crear bitmap area de dibujo

            //RectangleF r;
            cual = cual.ToUpper();

            Bitmap original = fImageR;
            if (cual.ToUpper() == "F")
                original = fImageF;

            if (original == null)
                throw new Exception("Error, original check image not assigned");

            if (original.Height <= 0 || original.Width <= 0)
                throw new Exception("Error, original check image size invalid");

            float dpi = original.HorizontalResolution;
            CalcRegions(dpi);

            
            var bmp = new Bitmap((int)vgRectFull.Width, (int)vgRectFull.Height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            using (Graphics canvas = Graphics.FromImage(bmp))
            {
                canvas.FillRectangle(Brushes.White, vgRectFull);

                if (cual == "F")
                {
                    if (vgDebug)
                    {
                        canvas.FillRectangle(Brushes.Red, vgRectFull);
                        canvas.FillRectangle(Brushes.LightGray, F1_Rect);
                        canvas.FillRectangle(Brushes.Yellow, F2_Rect);
                        canvas.FillRectangle(Brushes.LightGreen, F3_Rect);
                        canvas.FillRectangle(Brushes.Fuchsia, F4_Rect);
                        canvas.FillRectangle(Brushes.LightBlue, F5_Rect);
                        canvas.FillRectangle(Brushes.LightYellow, F8_Rect);
                    }

                    RenderRegion_F1(canvas, original);
                    RenderRegion_F2(canvas);
                    RenderRegion_F3(canvas);
                    RenderRegion_F4(canvas);
                    RenderRegion_F5(canvas, dpi);
                    RenderRegion_F7(canvas);
                    RenderRegion_F8(canvas, dpi);
                }
                else
                {
                    if (vgDebug)
                    {
                        canvas.FillRectangle(Brushes.Red, vgRectFull);
                        canvas.FillRectangle(Brushes.LightGray, B1_RectImage);
                        canvas.DrawRectangle(new Pen(Brushes.Black), F1_Rect.X, F1_Rect.Y, F1_Rect.Width, F1_Rect.Height);
                    }

                    RenderRegion_B1(canvas, dpi, original);
                }

                //var p = new Pen(Brushes.Black) { DashStyle = DashStyle.Dash };
                //canvas.DrawLine(p, vgRectFull.Left, vgRectFull.Top + 1, vgRectFull.Right, vgRectFull.Top + 1);
                //canvas.DrawLine(p, F5_Rect.Left, F5_Rect.Bottom, F5_Rect.Right, F5_Rect.Bottom);
            }

            if (cual.ToUpper() == "F")
                IRDImageF = bmp;
            else
                IRDImageR = bmp;
            return 0;
        }

        private void RenderRegion_F1(Graphics canvas, Bitmap original)
        {
            //Area de imagen de cheque
            //Return Reason Overlay (ANSI X9 6.1.7.2)


            //---------------
            //Return Reason Overlay
            var s = Text_ReturnReasonAbbreviation.Trim().ToUpper();
            var fontOverlay = new Font("Arial", 18);
            float x = F1_Rect.Left;
            float y = F1_Rect.Top;

            float overlayHeight = canvas.MeasureString(s, fontOverlay).Height + 3;
            canvas.DrawString(s, fontOverlay, Brushes.Black, x, y);

            //overlayHeight = ?? o podemos definir un margen superior ya fijo


            //----------------
            RectangleF r;

            //Calcular escalado de imagen dentro de F1
            var ratioH = (F1_Rect.Height - overlayHeight) / original.Height;
            var ratioW = F1_Rect.Width / original.Width;
            var scale = Math.Min(ratioH, ratioW);
            r = new RectangleF(0, 0, original.Width * scale, original.Height * scale);

            //Dibujar la imagen en la esquina inferior derecha
            r.X = F1_Rect.X + (F1_Rect.Width - r.Width);
            r.Y = F1_Rect.Y + (F1_Rect.Height - r.Height);
            canvas.DrawImage(original, r.X, r.Y, r.Width, r.Height);





            //texto overlay anterior rotado, ya no se usa
            /*
            x = vgRectCheck.Left + (vgRectCheck.Width / 2);
            y = vgRectCheck.Top + (vgRectCheck.Height / 2);
            var i = 0;
            foreach (var s in TxtB3)
            {
                var m = canvas.MeasureString(s, font);

                if (i == 0)
                {
                    x -= (m.Width / 2) + (m.Height / 2);
                    //y -= m.Width / m.Height;
                }

                //x = vgRectCheck.Left + (vgRectCheck.Width / 2);


                DrawRotatedTextAt(canvas, -20, s, x, y, font, Brushes.Black);
                //canvas.DrawString(s, font, Brushes.Black, x, y);
                y += m.Height - 3;
                x += m.Height / 2;
                i++;
            }
            */
        }

        private void RenderRegion_F2(Graphics canvas)
        {
            //6.1.2
            //Required Original Check Truncation Institution: Region 2F

            float x, y, w;
            var rect = new RectangleF(F2_Rect.Location, F2_Rect.Size);

            //formatear textos

            Text_F2_Secuence = Text_F2_Secuence.Trim();
            Text_F2_RoutingNum = Text_F2_RoutingNum.Trim().PadRight(9, '0');
            //Text_F2_InstitutionBusinessDate ver si sera DateTime o texto


            var strlist = new List<string>(2);
            strlist.Add(Text_F2_Secuence);
            strlist.Add($"[{Text_F2_RoutingNum}] {Text_F2_InstBusinessDate}");

            var font = new Font("OCR-A", 11);

            w = 0;
            foreach (var s in strlist)
            {
                var m = canvas.MeasureString(s, font);
                w += m.Height;
            }

            x = rect.Left + ((rect.Width - w) / 2f) + 2;
            if (x < rect.Left) x = rect.Left + 1;

            foreach (var s in strlist)
            {
                var m = canvas.MeasureString(s, font);

                //centrar
                y = rect.Bottom - ((rect.Height - m.Width) / 2f);

                DrawRotatedTextAt(canvas, -90f, s, x, y, font, Brushes.Black);
                x += m.Height;
            }
        }

        private void RenderRegion_F3(Graphics canvas)
        {
            //6.1.3
            //Required Creation Institution Region: Region 3F

            //formatear textos
            Text_F3_Secuence = Text_F3_Secuence.Trim();
            Text_F3_InstRoutingNum = Text_F3_InstRoutingNum.Trim().PadRight(9, '0');
            //Text_F3_InstitutionBusinessDate ver si sera DateTime o texto

            var strlist = new List<string>(2);
            strlist.Add($"*{Text_F3_InstRoutingNum}*");
            strlist.Add(Text_F3_InstBusinessDate);
            strlist.Add(Text_F3_Secuence);

            var font = new Font("OCR-A", 10);
            RenderStrList("B", canvas, strlist, F3_Rect, font, -3);
        }

        private void RenderRegion_F4(Graphics canvas)
        {
            //6.1.4  Required Legend Region: Region 4F

            var strList = new List<string>
            {
                "This is a LEGAL COPY of your",
                "check. You can use it the same",
                "way you would use the original",
                "check.",
            };

            var font = new Font("Arial", 8);
            RenderStrList("B", canvas, strList, F4_Rect, font);
        }

        private void RenderRegion_F5(Graphics canvas, float dpi)
        {
            //6.1.5  Required MICR Region: Region 5F

            //Todo:  revisar documentacion para F5,  ver como acomodar micr
            // ver si aqui se hace la identificacion si es un micr de un ird previo o es cheque original

            var s = Text_MICR.Trim();

            var font = new Font("MICR Encoding", 18);
            //var font = new Font("MICR Encoding", 22);
            var m = canvas.MeasureString(s, font);

            var rect = new RectangleF(F5_Rect.Location, F5_Rect.Size);

            //float upperBoundary = rect.Bottom - (0.4800f * dpi);
            //float upperBoundary = rect.Bottom - (0.4406299f * dpi);
            //float upperBoundary = rect.Bottom - (0.40125989f * dpi);0.36188979
            // float upperBoundary = rect.Bottom - (0.36188979f * dpi);
            //float upperBoundary = rect.Bottom - (0.34188979f * dpi);
            float upperBoundary = rect.Bottom - (0.32188979f * dpi);
            //float upperBoundary = rect.Bottom - (0.4375f * dpi);
            float lowerBoundary = rect.Bottom - (0.1875f * dpi);

            if (vgDebug)
            {
                canvas.DrawLine(new Pen(Brushes.Red), vgRectFull.Left, upperBoundary, vgRectFull.Right, upperBoundary);
                canvas.DrawLine(new Pen(Brushes.Black), vgRectFull.Left, lowerBoundary, vgRectFull.Right, lowerBoundary);
            }

            var margin = vgRectFull.Right - F1_Rect.Right;
            var x = rect.Right - (m.Width + margin);

            if (vgDebug) canvas.DrawRectangle(new Pen(Color.Red, 1), x, upperBoundary, m.Width, m.Height);
            canvas.DrawString(s, font, Brushes.Black, x, upperBoundary);
        }


        private void RenderRegion_F7(Graphics canvas)
        {
            var strList = new List<string>();

            var rrCode = Text_ReturnReasonCode.Trim().ToUpper();
            var rrName = Text_ReturnReasonName.Trim().ToUpper();

            strList.Add($"RETURN REASON-{rrCode}");
            strList.Add(rrName);
            
            var font = new Font("OCR-A", 10);
            RenderStrList("B", canvas, strList, F7_Rect, font);
        }

        private void RenderRegion_F8(Graphics canvas, float dpi)
        {
            //6.1.8  Optional Data Region: Region 8F

            var margin = (int)(0.100f * dpi); //margen superior
            var font = new Font("Arial", 9);
            RenderStrList("T", canvas, Text_F8, F8_Rect, font, 0, margin);
        }


        private void RenderRegion_B1(Graphics canvas, float dpi, Bitmap original)
        {
            //6.2.1  Required Item Back Image Region: Region 1B

            var rect = new RectangleF(B1_RectImage.Location, B1_RectImage.Size);

            RectangleF r;

            //Calcular escalado de imagen dentro de B1
            var ratioH = rect.Height / original.Height;
            var ratioW = rect.Width / original.Width;
            var scale = Math.Min(ratioH, ratioW);
            r = new RectangleF(0, 0, original.Width * scale, original.Height * scale);

            //Dibujar la imagen en la esquina inferior izquierda
            r.X = rect.Left;
            r.Y = rect.Y + (rect.Height - r.Height);
            canvas.DrawImage(original, r.X, r.Y, r.Width, r.Height);


            //-----------------------
            //6.2.1.2  Text and Line Overlay in Region 1B

            var sFormat = genericTypoFormat;
            float x, y;
            string s;

            var wingFont = new Font("Wingdings", 8, System.Drawing.FontStyle.Bold);
            var overlayFont = new Font("Arial", 7, System.Drawing.FontStyle.Bold);
            var overlayBottomMargin = (0.0625f * dpi);

            s = "Do not endorse or write below this line.";
            var m = canvas.MeasureString(s, overlayFont, new PointF(0, 0), sFormat);
            var overlayWidth = m.Width - 4;
            var overlayHeight = m.Height;

            var arrowStr = new string((char)0xE2, 1);
            var arrowSize = canvas.MeasureString(arrowStr, wingFont, new PointF(0, 0), sFormat);

            y = F1_Rect.Bottom - (overlayHeight + overlayBottomMargin);

            //Arrow derecha
            x = F1_Rect.Right - (arrowSize.Width + 2);
            if (vgDebug) canvas.DrawRectangle(new Pen(Color.White, 1), x, y, arrowSize.Width, arrowSize.Height);
            canvas.DrawString(arrowStr, wingFont, Brushes.Black, x, y, sFormat);

            //Texto
            x -= overlayWidth;
            if (vgDebug) canvas.DrawRectangle(new Pen(Color.LightYellow, 1), x, y, overlayWidth, overlayHeight);
            canvas.DrawString(s, overlayFont, Brushes.Black, x, y, sFormat);

            //Arrow izq
            x -= arrowSize.Width + 1;
            if (vgDebug) canvas.DrawRectangle(new Pen(Color.White, 1), x, y, arrowSize.Width, arrowSize.Height);
            canvas.DrawString(arrowStr, wingFont, Brushes.Black, x, y, sFormat);

            //Linea inferior
            canvas.DrawLine(new Pen(Brushes.Black), F5_Rect.Left, F5_Rect.Top, F5_Rect.Right, F5_Rect.Top);


            //-------------------------
            //Textos verticales parte trasera

            //se usa F1_Rect para definir area para escribir en la parte trasera
            rect = new RectangleF(F1_Rect.Location, F1_Rect.Size);

            //B5 Texto 90 grados atras
            var ocrFont = new Font("OCR-A", 10);

            x = rect.Right;
            foreach (var txt in TxtB5)
            {
                s = txt.Replace("[", ">").Replace("]", "<");
                m = canvas.MeasureString(s, ocrFont, new PointF(0, 0), sFormat);

                //centrar vertical
                y = rect.Top + ((rect.Height - m.Width) / 2);
                DrawRotatedTextAt(canvas, 90, s, x, y, ocrFont, Brushes.Black);
                x -= m.Height;
            }

        }


    }
}
