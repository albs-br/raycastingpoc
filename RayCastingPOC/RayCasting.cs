using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RayCastingPOC
{
    class RayCasting
    {


        const int SCREEN_WIDTH_IN_PIXELS = 16 * 8; // 128 pixels
        const int SCREEN_HEIGHT_IN_PIXELS = 12 * 8; // 96 pixels

        // map grid is 8 x 8 cells
        const int MAP_GRID_SIDE_IN_CELLS = 8;

        // each map cell is 4 x 4 pixels
        const int MAP_CELL_SIDE_IN_PIXELS = 4;
        //const int MAP_CELL_SIDE_IN_PIXELS = 16;
        
        const int MAP_COLS_IN_PIXELS = MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS;

        int[] mapGridInCells = {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 0, 1, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 1, 0, 1,
                1, 0, 0, 0, 0, 1, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 1, 1, 1, 1,
                1, 1, 0, 0, 0, 0, 0, 1,
                1, 1, 1, 1, 1, 1, 1, 1,
            };

        //int[] mapGridInCells = {
        //        1, 1, 1, 1, 1, 1, 1, 1,
        //        1, 0, 0, 0, 0, 0, 0, 1,
        //        1, 0, 0, 0, 0, 0, 0, 1,
        //        1, 0, 0, 0, 0, 0, 0, 1,
        //        1, 0, 0, 0, 0, 0, 0, 1,
        //        1, 0, 0, 0, 1, 1, 1, 1,
        //        1, 1, 0, 0, 0, 0, 0, 1,
        //        1, 1, 1, 1, 1, 1, 1, 1,
        //    };

        int[] mapGridInPixels;





        const double DEGREES_VIEW = 60.0;
        const int DISTANCE_TO_PROJECTION_PLANE = MAP_CELL_SIDE_IN_PIXELS;



        Pen blackPen = new Pen(Color.Black, 1);
        Brush whiteBrush = new SolidBrush(Color.White);
        Brush grayBrush = new SolidBrush(Color.Gray);
        Brush redBrush = new SolidBrush(Color.Red);

        public void Run()
        {
            int multiplier = MAP_CELL_SIDE_IN_PIXELS/4;

            MakeScreen(new Point(12 * multiplier, 24 * multiplier), "screenz.bmp");

            MakeScreen(new Point(4 * multiplier, 19 * multiplier), "screen0.bmp");

            MakeScreen(new Point(6 * multiplier, 19 * multiplier), "screen1.bmp");

            MakeScreen(new Point(8 * multiplier, 19 * multiplier), "screen2.bmp");

            MakeScreen(new Point(12 * multiplier, 19 * multiplier), "screen3.bmp");
            
            MakeScreen(new Point(16 * multiplier, 19 * multiplier), "screen4.bmp");
            
            MakeScreen(new Point(20 * multiplier, 19 * multiplier), "screen5.bmp");
            
            MakeScreen(new Point(24 * multiplier, 19 * multiplier), "screen6.bmp");
            
            MakeScreen(new Point(27 * multiplier, 19 * multiplier), "screen7.bmp");
        }


        private int ConvertXYMapToSeq(int x, int y)
        {
            return ConvertXYMapToSeq(new Point(x, y));
        }

        private int ConvertXYMapToSeq(Point point)
        {
            return (point.Y * (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS)) + point.X;
        }

        private void MakeScreen(Point playerPosition, string filename)
        {
            mapGridInPixels = CreateMapGridInPixels(mapGridInCells);

            MakeMapImage(mapGridInPixels, playerPosition, filename);

            var seqPlayer = ConvertXYMapToSeq(playerPosition);
            if (mapGridInPixels[seqPlayer] == 1)
            {
                throw new Exception(String.Format("Player ({0}, {1}) positioned inside wall.", playerPosition.X, playerPosition.Y));
            }

            // tg a = cat op/cat adj

            Bitmap bmp = new Bitmap(SCREEN_WIDTH_IN_PIXELS, SCREEN_HEIGHT_IN_PIXELS);
            //Bitmap bmp = new Bitmap(MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS, MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.FillRectangle(whiteBrush, 0, 0, SCREEN_WIDTH_IN_PIXELS, SCREEN_HEIGHT_IN_PIXELS);

                var angle = DEGREES_VIEW / SCREEN_WIDTH_IN_PIXELS;
                var currentAngle = 360 - (DEGREES_VIEW / 2); // initial angle

                for (int rayNumber = 0; rayNumber < SCREEN_WIDTH_IN_PIXELS; rayNumber++)
                //var rayNumber = 0;
                {
                    var radians = currentAngle * (Math.PI / 180);
                    var tangent = - Math.Tan(radians);
                    var cosine = Math.Cos(radians);

                    var distance = MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS;
                    for (int rayPoint = 1; rayPoint < MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS; rayPoint++)
                    {
                        int yDest = playerPosition.Y - rayPoint;

                        int catAdj = rayPoint; // playerPosition.Y - yDest;

                        var catOp = catAdj * tangent;

                        var xDest = playerPosition.X - catOp;

                        if (xDest < 0 || yDest < 0)
                        {
                            distance = rayPoint;
                            break;
                        }

                        //graphics.DrawLine(blackPen, playerPositionX, playerPositionY, xDest, yDest);
                        //graphics.FillRectangle(grayBrush, xDest, yDest, 1, 1);

                        // convert x and y to sequential of map
                        //var sequ = (yDest * (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS)) + xDest;
                        var sequ = ConvertXYMapToSeq((int)xDest, yDest);
                        if (mapGridInPixels[sequ] == 1)
                        {
                            //graphics.FillRectangle(redBrush, xDest, yDest, 1, 1);

                            distance = rayPoint;

                            // wrong: it makes the "fisheye" effect
                            // cos = adj / hip
                            // hip = adj / cos
                            //distance = (int)Math.Round(catAdj / cosine);
                            
                            break;
                        }
                    }


                    // convert distance to wall size (32 = 0.?; 1 = 1.0)
                    const double MIN_HEIGHT_WALL_FACTOR = 0.1;
                    const double MAX_HEIGHT_WALL_FACTOR = 1.0;
                    const double MIN_DISTANCE = 1;
                    const double MAX_DISTANCE = MAP_COLS_IN_PIXELS; //32

                    //TODO: make this function log, not linear
                    //for (int i = (int)MIN_DISTANCE; i <= MAX_DISTANCE; i++)
                    //{
                        //distance = i;
                        var factor = (MIN_HEIGHT_WALL_FACTOR - MAX_HEIGHT_WALL_FACTOR) / (MAX_DISTANCE - MIN_DISTANCE);

                    // distance     factor2
                    // 1            1
                    // 32           1/32
                    
                        var factor2 = 1 + (((MAX_DISTANCE / distance) / MAX_DISTANCE)/1);

                        var wallHeightFactor = ((distance - 1) * (factor)) + 1;

                    //Console.WriteLine(distance + " - " + wallHeightFactor);
                    //}

                    //var wallHeight = (int)(wallHeightFactor * SCREEN_HEIGHT_IN_PIXELS);

                    var wallHeight = Math.Min((int)(SCREEN_HEIGHT_IN_PIXELS / distance) * DISTANCE_TO_PROJECTION_PLANE, SCREEN_HEIGHT_IN_PIXELS);

                    var grayShade = 255 - (distance * (int)(256/MAX_DISTANCE));

                    graphics.FillRectangle(
                        //grayBrush, 
                        new SolidBrush(Color.FromArgb(255, grayShade, grayShade, grayShade)),
                        rayNumber,                                                                              // x
                        (SCREEN_HEIGHT_IN_PIXELS - wallHeight) / 2,                                             // y
                        1,                                                                                      // width
                        wallHeight                                                                              // height
                        );

                    //Console.WriteLine(distance + " - " + factor + " - " + wallHeight);
                    Console.WriteLine(String.Format("Distance: {0}; Factor: {1}; WallHeight: {2}", distance, factor, wallHeight));

                    currentAngle += angle;
                }
            }

            bmp.Save(filename, ImageFormat.Bmp);
            Console.WriteLine("---------- Saving to file: " + filename);
        }

        //private int FromLinearToXY(int linear, int lineSize)
        //{ 
        //    return linear \
        //}

        private int[] CreateMapGridInPixels(int[] mapGridInCells)
        {
            int[] output = new int[(int)Math.Pow(MAP_COLS_IN_PIXELS, 2)];

            var col = 0;
            var line = 0;
            foreach (var cell in mapGridInCells)
            {
                
                for (var n = 0; n < MAP_CELL_SIDE_IN_PIXELS; n++)
                {
                    //output[col + n + (line * 32 * blockSide) + (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS) * 0] = cell;
                    //output[col + n + (line * 32 * blockSide) + (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS) * 1] = cell;
                    //output[col + n + (line * 32 * blockSide) + (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS) * 2] = cell;
                    //output[col + n + (line * 32 * blockSide) + (MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_SIZE_IN_PIXELS) * 3] = cell;

                    for (int m = 0; m < MAP_CELL_SIDE_IN_PIXELS; m++)
                    {
                        output[col + n + (line * MAP_COLS_IN_PIXELS * MAP_CELL_SIDE_IN_PIXELS) + MAP_COLS_IN_PIXELS * m] = cell;
                    }
                }

                col += MAP_CELL_SIDE_IN_PIXELS;

                if (col == MAP_COLS_IN_PIXELS)
                {
                    line++;
                    col = 0;
                }
            }

            return output;
        }



        private void MakeMapImage(int[] mapGridInPixels, Point playerPosition, string filename)
        {
            Bitmap bmp = new Bitmap(MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS, MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS);

            // Draw line to screen.
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.FillRectangle(whiteBrush, 0, 0, MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS, MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS);

                var x = 0;
                var y = 0;
                foreach (var pixel in mapGridInPixels)
                {
                    if (pixel != 0)
                    {
                        graphics.FillRectangle(grayBrush, x, y, 1, 1);
                    }

                    x++;
                    if (x == MAP_GRID_SIDE_IN_CELLS * MAP_CELL_SIDE_IN_PIXELS)
                    {
                        y++;
                        x = 0;
                    }
                }

                graphics.FillRectangle(redBrush, playerPosition.X, playerPosition.Y, 1, 1);
            }

            bmp.Save("map_" + filename, ImageFormat.Bmp);
        }

    }
}
