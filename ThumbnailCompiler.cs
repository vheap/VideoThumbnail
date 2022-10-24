     public void RenderFramesThumbnail(FileInfo[] files, string path)
        {
            int iMarginX = 10;
            int iMarginY = 10;
            int iWidth = 430;
            int iHeight = 210;
            int nPerRow = 4;
            int nImages = files.Count();

            double nRows = (double)nImages / 4;
            nRows = Math.Ceiling(nRows);

            int sizeX = iWidth * nPerRow;
            sizeX += nPerRow * iMarginX;
            sizeX -= iMarginX; //remove excess right side border
            int sizeY = iHeight * (int)nRows;
            sizeY += (int)nRows * iMarginY;
            sizeY -= iMarginY; //remove excess bottom border
            int cRow = 0; //current pos
            int cItems = 0; //current items per row

            Bitmap bitmap = new Bitmap(sizeX, sizeY);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.Clear(Color.Black);
                foreach (FileInfo file in files)
                {
                    if (!VerifyFrameIntegrity(file.FullName))
                    {
                        continue;
                    }
                    using (var img = Image.FromFile(file.FullName))
                    {
                        string ts = SecondsToTimestamp(file.Name);
                        int _iMarginX = iMarginX;
                        int _iMarginY = iMarginY;
                        using (var thumb = img.GetThumbnailImage(iWidth, iHeight, () => false, IntPtr.Zero))
                        {
                            if (cItems == 0)//First image in the row.
                            {
                                _iMarginX = 0;
                            }
                            if (cRow == 0)//First image in the row.
                            {
                                _iMarginY = 0;
                            }
                            g.DrawImage(thumb, (iWidth * cItems) + (_iMarginX * cItems), (iHeight * cRow) + (_iMarginY * cRow));
                            GraphicsPath p = new GraphicsPath();
                            p.AddString(
                                ts,
                                FontFamily.GenericSansSerif,
                                (int)FontStyle.Bold,
                                g.DpiY * 10 / 72,
                                new Point((iWidth * cItems) + (_iMarginX * cItems), (iHeight * cRow) + (_iMarginY * cRow)),
                                new StringFormat());
                            Pen drawingPen = new Pen(Brushes.Black);
                            drawingPen.Width = 4;
                            g.DrawPath(drawingPen, p);
                            g.FillPath(Brushes.White, p);
                            cItems++;
                            if (cItems == nPerRow)
                            {
                                cItems = 0;
                                cRow++;
                            }
                        }
                    }
                }
            }
            SaveJpeg(path, bitmap, 90);
     }
