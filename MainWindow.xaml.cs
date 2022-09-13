using Microsoft.Win32;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using WF = System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SudokuFix
{
    public partial class MainWindow : Window
    {
        private static Color GrayC = Colors.Gray;
        private static Color DimGrayC = Colors.DimGray;
        private static Color LightGrayC = Colors.LightGray;
        private static Color DarkGrayC = Colors.DarkGray;
        private static Color BlackC = Colors.Black;
        private static Color WhiteC = Colors.White;

        private static Brush Gray = new SolidColorBrush(GrayC);
        private static Brush DimGray = new SolidColorBrush(DimGrayC);
        private static Brush LightGray = new SolidColorBrush(LightGrayC);
        private static Brush DarkGray = new SolidColorBrush(DarkGrayC);
        private static Brush Transparent = new SolidColorBrush(Colors.Transparent);
        private static Brush Black = new SolidColorBrush(BlackC);
        private static Brush White = new SolidColorBrush(WhiteC);

        private static Brush BackCyan = new SolidColorBrush(Color.FromRgb(38, 204, 250));
        private static Brush DCyan = new SolidColorBrush(Color.FromRgb(97, 195, 222));
        private static Brush LCyan = new SolidColorBrush(Color.FromRgb(140, 230, 255));

        private static string Const;
        private static int a = 1;
        private static int h = 0, p = 0;
        private static int sleep = 4;

        private static List<Label> Labels = new List<Label>();
        private static List<Label> DefLabels = new List<Label>();
        private static List<Label> MTLabels = new List<Label>();
        private static List<Label> CommonLabels = new List<Label>();

        private static Stack stack = new Stack();
        private static Queue<WF.MethodInvoker> queue = new Queue<WF.MethodInvoker>();
        private static Stopwatch timer = new Stopwatch();

        private static bool solved = false;
        private static bool fast = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            foreach (var objs in grid1.Children)
            {
                if (objs.GetType() == typeof(Rectangle))
                {
                    Rectangle thisRect = objs as Rectangle;

                    Console.WriteLine(thisRect.Name);
                }
                if (objs.GetType() == typeof(Button))
                {
                    Button thisBtn = objs as Button;

                    Console.WriteLine(thisBtn.Name);
                }

                if (objs.GetType() == typeof(Grid))
                {
                    Grid thisGrid = objs as Grid;
                    Console.WriteLine(thisGrid.Name);

                    foreach (Label child in thisGrid.Children)
                    {
                        Console.WriteLine(child.Name);

                        if (child.Name.Length >= 6)
                        {
                            child.Background = Transparent;
                            child.Foreground = Black;

                            child.MouseDown += LabelClick;
                            child.MouseEnter += LabelOver;
                            child.MouseLeave += LabelOut;

                            if (child.Content.ToString().Length != 1)
                            {
                                child.Content = "";
                            }

                            Labels.Add(child);
                        }
                    }
                }
                if (objs.GetType() == typeof(Label))
                {
                    Label thisLabel = objs as Label;
                    Console.WriteLine(thisLabel.Name);

                    if (thisLabel.Name.Length == 3)
                    {
                        thisLabel.Background = Transparent;
                        thisLabel.Foreground = Black;

                        thisLabel.MouseDown += LabelClick;
                        thisLabel.MouseEnter += LabelOver;
                        thisLabel.MouseLeave += LabelOut;

                        if (thisLabel.Name is "LN1")
                        {
                            thisLabel.Background = White;
                            Const = "1";
                        }
                    }
                }
            }

            PutBoard("", false);
        }

        private void LabelClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Clicked");

            Label lbl = sender as Label;

            if (lbl.Name.Length is 3)
            {
                foreach (var label in grid1.Children.OfType<Label>())
                {
                    if (label.Name.Length is 3)
                    {
                        label.Background = Transparent;
                        Console.WriteLine(label.Name);
                    }
                }

                lbl.Background = White;

                Const = lbl.Content.ToString();
            }

            if (lbl.Name.Length >= 6)
            {
                if (lbl.Content is "")
                {
                    lbl.Content = Const;
                    lbl.FontWeight = FontWeights.Bold;
                    lbl.Foreground = DimGray;
                }

                if (Const is "X")
                {
                    lbl.Content = "";
                    lbl.FontWeight = FontWeights.Regular;
                    lbl.Foreground = Black;
                }
            }

            Console.WriteLine(Const);
        }

        private void LabelOver(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Enter");

            Label lbl = sender as Label;

            if (lbl.Background == Transparent)
            {
                Console.WriteLine("o");
                lbl.Background = LightGray;
            }
        }

        private void LabelOut(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Leave");

            Label lbl = sender as Label;

            if (lbl.Background == White)
            {
            }
            if (lbl.Background == LightGray)
            {
                lbl.Background = Transparent;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn == BenchmarkBtn)
            {
                btn.Background = new SolidColorBrush(Colors.Lime);
                NormalBtn.Background = new SolidColorBrush(Colors.Red);
                fast = true;
            }
            else if (btn == NormalBtn)
            {
                btn.Background = new SolidColorBrush(Colors.Lime);
                BenchmarkBtn.Background = new SolidColorBrush(Colors.Red);
                fast = false;
            }
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            DefLabels.Clear();
            MTLabels.Clear();

            if (!solved)
            {
                foreach (Label label in Labels)
                {
                    if (label.Content != "")
                    {
                        DefLabels.Add(label);
                        MTLabels.Remove(label);
                        label.FontWeight = FontWeights.Bold;
                        label.Foreground = DimGray;
                    }
                    else if (label.Content == "")
                    {
                        MTLabels.Add(label);
                        DefLabels.Remove(label);
                        label.FontWeight = FontWeights.Regular;
                        label.Foreground = Black;
                    }
                }
            }
            else if (solved)
            {
                foreach (Label label in Labels)
                {
                    if (label.FontWeight == FontWeights.Bold)
                    {
                        DefLabels.Add(label);
                    }
                    if (DefLabels.Contains(label))
                    {
                        label.Content = DefLabels[DefLabels.IndexOf(label)].Content;
                    }
                    else if (!DefLabels.Contains(label))
                    {
                        MTLabels.Add(label);
                        label.Content = "";
                    }
                }

                solved = false;
            }

            if (fast)
            {
                sleep = 0;
            }
            else if (!fast)
            {
                sleep = 20;
            }

            Console.WriteLine(DefLabels.Count);
            Console.WriteLine(MTLabels.Count);

            Calculate();
        }

        private async void Calculate()
        {
            a = 1;
            h = 0;

            Label label = MTLabels[h];

            label.Content = a;

            if (fast) timer.Restart();

            await Check(label);
        }

        private async Task Check(Label label)
        {
            do
            {
            LastDo:
                CommonLabels.Clear();
                var p1 = VisualTreeHelper.GetParent(label) as UIElement;

                int labelID = Convert.ToInt32(label.Name.Substring(5));
                int labelY = GetLabelY(labelID);
                int labelX = labelID % 9;
                string labelParent = ((Grid)p1).Name;

                foreach (Label lbl in Labels)
                {
                    if (lbl != label)
                    {
                        var p2 = VisualTreeHelper.GetParent(lbl) as UIElement;
                        int lblID = Convert.ToInt32(lbl.Name.Substring(5));
                        int lblY = GetLabelY(lblID);
                        int lblX = lblID % 9;
                        string lblParent = ((Grid)p2).Name;

                        if (lblY == labelY || lblX == labelX || lblParent == labelParent)
                        {
                            CommonLabels.Add(lbl);
                        }
                    }
                }



            Restart:
                foreach (Label lbl in CommonLabels)
                {
                    if (lbl.Content.ToString() == label.Content.ToString())
                    {
                        if (a >= 9)
                        {
                            label.Content = "";
                            h--;
                            label = MTLabels[h];
                            a = (int)label.Content;

                            if (a >= 9)
                            {
                                label.Content = "";
                                h--;
                                label = MTLabels[h];
                                a = (int)label.Content;

                                if (a >= 9)
                                {
                                    label.Content = "";
                                    h--;
                                    label = MTLabels[h];
                                    a = (int)label.Content;
                                    a++;
                                    label.Content = a;
                                }
                                else if (a < 9)
                                {
                                    a++;
                                    label.Content = a;
                                }
                            }
                            else if (a < 9)
                            {
                                a++;
                                label.Content = a;
                            }

                            goto GetOut;
                        }
                        else if (a < 9)
                        {
                            a++;
                            label.Content = a;

                            goto Restart;
                        }
                    }
                }


                if (h + 1 < MTLabels.Count)
                {
                    a = 1;
                    h++;
                    label = MTLabels[h];

                    label.Content = a;
                    await Task.Delay(sleep);

                    Console.WriteLine(label.Name, a);
                }

                if (h + 1 == MTLabels.Count)
                {
                    a = 1;

                    label.Content = a;
                    Console.WriteLine(label.Name, a);

                    h++;
                    goto LastDo;
                }

            GetOut:
                ;
            } while (h + 1 < MTLabels.Count);

            Console.WriteLine("Done");

            if (fast)
            {
                timer.Stop();
                Console.WriteLine("It took: {0:hh\\:mm\\:ss} to find the solution", timer.Elapsed);
            }
            solved = true;
        }

        private int GetLabelY(int labelID)
        {
            if (labelID <= 9) return 1;
            else if (labelID <= 18) return 2;
            else if (labelID <= 27) return 3;
            else if (labelID <= 36) return 4;
            else if (labelID <= 45) return 5;
            else if (labelID <= 54) return 6;
            else if (labelID <= 63) return 7;
            else if (labelID <= 72) return 8;
            else if (labelID <= 81) return 9;
            else return 0;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            WF.FolderBrowserDialog fbd = new WF.FolderBrowserDialog();
            fbd.Description = "Custom Description";

            if (fbd.ShowDialog() == WF.DialogResult.OK)
            {
                string selectedPath = fbd.SelectedPath;
                selectedPath += @"\sudokuBoard.txt";

                foreach (Label label in Labels)
                {
                    if (label.Content == "")
                    {
                        result += "0";
                    }
                    else
                    {
                        result += label.Content;
                    }
                }

                File.WriteAllText(selectedPath, result);
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            string input;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                input = File.ReadAllText(openFileDialog.FileName);

                PutBoard(input, true);
            }
            return;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            for (int i = 0; i < 81; i++)
            {
                Labels[i].Content = "";
                Labels[i].FontWeight = FontWeights.Regular;
                Labels[i].Background = Transparent;
                Labels[i].Foreground = Black;
            }
        }

        private void ClearSolved_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 81; i++)
            {
                if (Labels[i].FontWeight == FontWeights.Bold)
                {
                    continue;
                }
                else if (Labels[i].FontWeight == FontWeights.Regular)
                {
                    Labels[i].Content = "";
                }
            }
        }

        private void PutBoard(string input, bool import)
        {
            if (import)
            {
                for (int i = 0; i < 81; i++)
                {
                    string num = input.Substring(i, 1);

                    if (num != "0")
                    {
                        Labels[i].Content = num;
                        Labels[i].FontWeight = FontWeights.Bold;
                        Labels[i].Foreground = DimGray;
                    }
                }
            }
            if (!import)
            {
                string board = "000009310060182970100003200076090035010000090920030480003900004021734050049600000";

                for (int i = 0; i < 81; i++)
                {
                    string num = board.Substring(i, 1);

                    if (num != "0")
                    {
                        Labels[i].Content = num;
                        Labels[i].FontWeight = FontWeights.Bold;
                        Labels[i].Foreground = DimGray;
                    }
                }
            }
        }
    }
}