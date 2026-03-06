using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SistemaDeVenta
{
    public partial class pagina_prueba : UserControl
    {
        const int ItemSize = 80;
        const int MarginSize = 10;
        const int MinFilas = 2;

        List<Border> items = new List<Border>();

        Border draggedItem;
        Border ghostItem;

        Point mouseOffset;

        public pagina_prueba()
        {
            InitializeComponent();
            Loaded += Pagina_prueba_Loaded;
        }

        private void Pagina_prueba_Loaded(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            items.Clear();

            for (int i = 0; i < 30; i++)
            {
                var item = CrearItem(i);
                items.Add(item);
                MainCanvas.Children.Add(item);
            }

            ReposicionarItems();
        }

        // =========================
        // ITEM
        // =========================
        Border CrearItem(int index)
        {
            Border item = new Border
            {
                Width = ItemSize,
                Height = ItemSize,
                CornerRadius = new CornerRadius(12),
                Background = new LinearGradientBrush(
                    Color.FromRgb(80, 140, 220),
                    Color.FromRgb(40, 90, 180),
                    45),
                Child = new TextBlock
                {
                    Text = (index + 1).ToString(),
                    FontSize = 22,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                RenderTransform = new TranslateTransform(),
                SnapsToDevicePixels = true
            };

            item.MouseLeftButtonDown += (s, e) => IniciarDrag(item, e);
            item.MouseRightButtonUp += (s, e) => MostrarMenu(item, e);

            return item;
        }

        // =========================
        // MENÚ CONTEXTUAL
        // =========================
        void MostrarMenu(Border item, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem abrir = new MenuItem { Header = "Abrir" };
            abrir.Click += (_, __) => MessageBox.Show("Abrir item");

            MenuItem renombrar = new MenuItem { Header = "Renombrar" };
            renombrar.Click += (_, __) => MessageBox.Show("Renombrar item");

            MenuItem eliminar = new MenuItem { Header = "Eliminar" };
            eliminar.Click += (_, __) =>
            {
                items.Remove(item);
                MainCanvas.Children.Remove(item);
                ReposicionarItems();
            };

            menu.Items.Add(abrir);
            menu.Items.Add(renombrar);
            menu.Items.Add(new Separator());
            menu.Items.Add(eliminar);

            item.ContextMenu = menu;
            menu.IsOpen = true;

            e.Handled = true;
        }

        // =========================
        // GHOST
        // =========================
        Border CrearGhost(Border original)
        {
            return new Border
            {
                Width = ItemSize,
                Height = ItemSize,
                CornerRadius = new CornerRadius(12),
                Background = original.Background,
                Opacity = 0.45,
                IsHitTestVisible = false,
                RenderTransform = new TranslateTransform()
            };
        }

        // =========================
        // DRAG START
        // =========================
        void IniciarDrag(Border item, MouseButtonEventArgs e)
        {
            draggedItem = item;
            mouseOffset = e.GetPosition(item);

            ghostItem = CrearGhost(item);
            MainCanvas.Children.Add(ghostItem);

            Canvas.SetLeft(ghostItem, Canvas.GetLeft(item));
            Canvas.SetTop(ghostItem, Canvas.GetTop(item));

            draggedItem.Opacity = 0.2;
            Panel.SetZIndex(ghostItem, 999);

            draggedItem.CaptureMouse();
        }

        // =========================
        // DRAG MOVE
        // =========================
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedItem == null || ghostItem == null) return;

            Point pos = e.GetPosition(MainCanvas);

            Canvas.SetLeft(ghostItem, pos.X - mouseOffset.X);
            Canvas.SetTop(ghostItem, pos.Y - mouseOffset.Y);
        }

        // =========================
        // DRAG END
        // =========================
        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedItem == null) return;

            draggedItem.ReleaseMouseCapture();

            Point pos = e.GetPosition(MainCanvas);

            items.Remove(draggedItem);

            int columnas = ObtenerColumnas();

            int col = (int)(pos.X / (ItemSize + MarginSize));
            int fila = (int)(pos.Y / (ItemSize + MarginSize));

            col = Math.Max(0, col);
            fila = Math.Max(0, fila);

            int nuevoIndex = fila * columnas + col;

            if (nuevoIndex < 0) nuevoIndex = 0;
            if (nuevoIndex > items.Count) nuevoIndex = items.Count;

            items.Insert(nuevoIndex, draggedItem);

            draggedItem.Opacity = 1;

            MainCanvas.Children.Remove(ghostItem);
            ghostItem = null;
            draggedItem = null;

            ReposicionarItems();
        }

        // =========================
        // LAYOUT
        // =========================
        int ObtenerColumnas()
        {
            int columnas = Math.Max(1,
                (int)(MainCanvas.ActualWidth / (ItemSize + MarginSize)));

            int filas = (int)Math.Ceiling(items.Count / (double)columnas);
            if (filas < MinFilas) filas = MinFilas;

            return columnas;
        }

        void ReposicionarItems()
        {
            int columnas = ObtenerColumnas();

            for (int i = 0; i < items.Count; i++)
            {
                int fila = i / columnas;
                int col = i % columnas;

                double x = col * (ItemSize + MarginSize);
                double y = fila * (ItemSize + MarginSize);

                Animar(items[i], x, y);
            }
        }

        // =========================
        // ANIMACIÓN
        // =========================
        void Animar(Border item, double x, double y)
        {
            if (double.IsNaN(Canvas.GetLeft(item)))
            {
                Canvas.SetLeft(item, x);
                Canvas.SetTop(item, y);
                return;
            }

            item.BeginAnimation(Canvas.LeftProperty,
                new DoubleAnimation(x, TimeSpan.FromMilliseconds(180))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                });

            item.BeginAnimation(Canvas.TopProperty,
                new DoubleAnimation(y, TimeSpan.FromMilliseconds(180))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                });
        }
    }
}
