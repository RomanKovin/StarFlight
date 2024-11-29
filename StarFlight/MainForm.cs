namespace StarFlight
{
    /// <summary>
    /// Пример проекта для лабораторной работы по анимации. 
    /// Имитация полёта среди звёзд.
    /// © Ковин Р.В.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            stars = new Point3D[starCount];
            generateStars();

            starImage = Properties.Resources.star; // берем изображение звезды, загруженное в ресурсы
            cabinImage = Properties.Resources.cabin; // берем изображение кабины, загруженное в ресурсы

            kosmosImage = Properties.Resources.kosmos2;

            planet1Image = Properties.Resources.planet1;
            planet2Image = Properties.Resources.planet2;
            planet3Image = Properties.Resources.planet3;
            planet4Image = Properties.Resources.planet4;

            dz = -step; // по умолчанию движение вперед

        }
        Point3D[] stars; //массив звезд
        Brush brush = new SolidBrush(Color.White); // для варианта отрисовки зведы в виде белого круга
        int starCount = 2000; // число звезд
        double maxSize = 1000; // максимальная по модулю координата XYZ звезды в 3D-пространстве
        double dist = 1000; // расстояние до "наблюдателя"
        double step = 10; // шаг движения

        // смещение звезды в 3D-пространстве
        double dx = 0;
        double dy = 0;
        double dz = 0;

        float maxStarSize = 8; // макс. размер звезды в пикселях
        Image starImage;
        Image cabinImage;
        Image planet1Image;
        Image planet2Image;
        Image planet3Image;
        Image planet4Image;
        Image kosmosImage;

        bool cabinVisible = false; // видимость кабины

        Random random = new Random();

        private void generateStars()
        { 
            // положение точек случайное по каждой оси XYZ (-maxSize, +maxSize)
            for (int i = 0; i < stars.Length; i++)
            {
                double x = random.NextDouble() * maxSize * 2 - maxSize;
                double y = random.NextDouble() * maxSize * 2 - maxSize;
                double z = random.NextDouble() * maxSize * 2 - maxSize;
                Point3D star = new Point3D(x, y, z);
                stars[i] = star;
            }
            // вместо некоторых звезд будем рисовать планету
            int planetIndex;
            planetIndex = random.Next(0, stars.Length);
            stars[planetIndex].ObjectType = ObjectType.Planet1;
            planetIndex = random.Next(0, stars.Length);
            stars[planetIndex].ObjectType = ObjectType.Planet2;
            planetIndex = random.Next(0, stars.Length);
            stars[planetIndex].ObjectType = ObjectType.Planet3;
            planetIndex = random.Next(0, stars.Length);
            stars[planetIndex].ObjectType = ObjectType.Planet4;

        }
        private void moveStars()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                Point3D star = stars[i];
                // сдвигаем звезду в 3D пространстве
                star.X += dx;
                star.Y += dy;
                star.Z += dz;
                // не допускаем выхода звезды за пределы допустимой области,
                // замыкая движение в "кольцо" и "рождая" звезду в новой точке
                if (star.X < -maxSize)
                {
                    star.X = maxSize;
                    star.Y = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Z = random.NextDouble() * maxSize * 2 - maxSize;
                }
                if (star.X > maxSize)
                {
                    star.X = -maxSize;
                    star.Y = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Z = random.NextDouble() * maxSize * 2 - maxSize;
                }
                if (star.Y < -maxSize)
                {
                    star.X = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Y = maxSize;
                    star.Z = random.NextDouble() * maxSize * 2 - maxSize;
                }
                if (star.Y > maxSize)
                {
                    star.X = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Y = -maxSize;
                    star.Z = random.NextDouble() * maxSize * 2 - maxSize;
                }
                if (star.Z < -maxSize)
                {
                    star.X = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Y = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Z = maxSize;
                }
                if (star.Z > maxSize)
                {
                    star.X = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Y = random.NextDouble() * maxSize * 2 - maxSize;
                    star.Z = -maxSize;
                }
                stars[i] = star;
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            DrawStars(e);
        }

        /// <summary>
        /// Отрисовка 3D-сцены со звёздами
        /// </summary>
        /// <param name="e"></param>
        private void DrawStars(PaintEventArgs e)
        {
            // рисуем фон 
            e.Graphics.DrawImage(kosmosImage, 0, 0);
            // важное замечание: рисовать звезды будем от самых дальних к самым ближним
            // для этого массив необходимо упорядочить
            List<Point3D> listStart = stars.ToList();   // преобразовываем массив в список
            listStart.Sort();                           // сортируем список, см. Point3D.CompaerTo()
            stars = listStart.ToArray();                // записываем отсортированный список обратно в массив


            for (int i = stars.Length - 1; i >= 0; i--) // от самых дальних к самым ближним
            {  
                Point3D star = stars[i];
                // перспективная проекция, формулы см. здесь http://stratum.ac.ru/education/textbooks/kgrafic/lection04.html
                // оси X и Y как в математике
                // оси Z направлена вглубь экрана
                float xScr = (float)(dist * star.X / (star.Z + dist)) + ClientSize.Width / 2;
                float yScr = (float)(-dist * star.Y / (star.Z + dist)) + ClientSize.Height / 2;

                if (!float.IsNormal(xScr) ||
                    !float.IsNormal(yScr) ||
                    xScr > ClientSize.Width ||
                    yScr > ClientSize.Height ||
                    xScr < 0 ||
                    yScr < 0)
                {
                    continue;
                }
                // чем ближе к наблюдаетелю звезда тем больше ее размер
                float starSize = (float)((-star.Z + maxSize) * maxStarSize / maxSize / 2) + 1;

                try
                {
                    //e.Graphics.FillEllipse(brush, xScr, yScr, starSize, starSize); // альтернативное отображение 
                    int zoom;
                    if (star.ObjectType == ObjectType.Star)
                    {

                        // так как исходный рисунок звезды это детальный PNG с полупрозрачными областями
                        // увеличиваем размер выводимого рисунка, чтобы звезда была в нужном размере
                        zoom = 2;
                        // звезда отображается растровым рисунком-спрайтом (см. star.png, VS отображет прозрачность криво)
                        e.Graphics.DrawImage(starImage,
                            xScr - starSize * zoom, // рисунок отцентрован относительно центра звезды
                            yScr - starSize * zoom,
                            starSize * zoom * 2,
                            starSize * zoom * 2);
                    }
                    else 
                    {
                        Image image;
                        switch (star.ObjectType) 
                        {

                            case ObjectType.Planet1:
                                image = planet1Image;
                                break;
                            case ObjectType.Planet2:
                                image = planet2Image;
                                break;
                            case ObjectType.Planet3:
                                image = planet3Image;
                                break;
                            case ObjectType.Planet4:
                                image = planet4Image;
                                break;
                            default:
                                image = starImage;
                                break;
                        }
                        // так как исходный рисунок планеты это детальный PNG с полупрозрачными областями
                        // увеличиваем размер выводимого рисунка, чтобы объект был в нужном размере
                        zoom = 4;
                        // объект отображается растровым рисунком-спрайтом
                        e.Graphics.DrawImage(image,
                            xScr - starSize * zoom, // рисунок отцентрован относительно центра объекта
                            yScr - starSize * zoom,
                            starSize * zoom * 2,
                            starSize * zoom * 2);
                    }
                }
                catch (Exception)
                {
                    // поглощаем любые ошибки, прежде всего из-за возможного переполнения 
                }
            }
            if (cabinVisible)
            {
                e.Graphics.DrawImage(cabinImage,
                    0, // рисунок кабины на весь экран
                    0,
                    ClientSize.Width,
                    ClientSize.Height);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // обновляем координаты звезд
            moveStars();
            // требуем отрисовки
            Invalidate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // реакция на нажатия клавиш
            switch (e.KeyCode)
            {
                case Keys.Space: // пробел
                    timer1.Enabled = !timer1.Enabled; // включаем/отключаем таймер
                    break;
                case Keys.Left: // стрелка влево
                    dx = step;
                    dy = 0;
                    dz = 0;
                    break;
                case Keys.Right: // стрелка вправо
                    dx = -step;
                    dy = 0;
                    dz = 0;
                    break;
                case Keys.Up: // стрелка вверх
                    dx = 0;
                    dy = 0;
                    dz = -step;
                    break;
                case Keys.Down: // стрелка вниз
                    dx = 0;
                    dy = 0;
                    dz = step;
                    break;
                case Keys.C: // С
                    cabinVisible = !cabinVisible;
                    Invalidate();
                    break;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // требуем отрисовки
            Invalidate();
        }
    }
}