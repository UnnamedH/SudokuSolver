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

namespace SudokuFix
{
  public partial class MainWindow : Window
  {
    private static Color GrayC = Colors.Gray;
    private static Color DimGrayC = Colors.DimGray;
    private static Color LightGrayC = Colors.LightGray;
    private static Brush Gray = new SolidColorBrush(GrayC);
    private static Brush DimGray = new SolidColorBrush(DimGrayC);
    private static Brush LightGray = new SolidColorBrush(LightGrayC);
    private static Brush Transparent = new SolidColorBrush(Colors.Transparent);

    private static string Const;
    private static int a = 1;
    public int h = 0, p = 0;

    private static List<Label> Labels = new List<Label>();
    private static List<Label> DefLabels = new List<Label>();
    private static List<Label> MTLabels = new List<Label>();
    private static List<Label> CommonLabels = new List<Label>();

    private static Stack stack = new Stack();
    private static Queue<WF.MethodInvoker> queue = new Queue<WF.MethodInvoker>();

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

            thisLabel.MouseDown += LabelClick;
            thisLabel.MouseEnter += LabelOver;
            thisLabel.MouseLeave += LabelOut;

            if (thisLabel.Name is "LN1")
            {
              thisLabel.Background = Gray;
              Const = "1";
            }
          }
        }
      }

      PutBoard();
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

        lbl.Background = Gray;

        Const = lbl.Content.ToString();
      }

      if (lbl.Name.Length >= 6)
      {
        if (lbl.Content is "")
        {
          lbl.Content = Const;
        }

        if (Const is "X")
        {
          lbl.Content = "";
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

      if (lbl.Background == Gray)
      {
      }
      if (lbl.Background == LightGray)
      {
        lbl.Background = Transparent;
      }
    }

    private void Solve_Click(object sender, RoutedEventArgs e)
    {
      DefLabels.Clear();
      MTLabels.Clear();

      foreach (Label label in Labels)
      {
        //Console.WriteLine(label.Name);

        if (label.Content != "")
        {
          DefLabels.Add(label);
        }
        else if (label.Content == "")
        {
          MTLabels.Add(label);
        }
      }

      Console.WriteLine(DefLabels.Count);
      Console.WriteLine(MTLabels.Count);

      Calculate();
    }

    private void Calculate()
    {
      a = 1;
      h = 0;

      Label label = MTLabels[h];

      label.Content = a;

      Check(label);
    }

    private void Check(Label lb)
    {
      Label label = lb;
      goto Start;
    Start:
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

    Repeat:
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
            a++;
            label.Content = a;

            goto Start;
          }
          else if (a < 9)
          {
            a++;
            label.Content = a;

            goto Repeat;
          }
        }
      }

      if (h + 1 == MTLabels.Count)
      {
        Console.WriteLine("Done");
      }
      if (h + 1 < MTLabels.Count)
      {
        a = 1;
        h++;
        label = MTLabels[h];
        label.Content = a;

        goto Start;
      }
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

        //PutBoard();
      }
    }

    private void PutBoard()
    {
      string path = @"C:\Users\Hrant\Desktop\sudokuBoard.txt";
      string board = File.ReadAllText(path);

      for (int i = 0; i < 81; i++)
      {
        string num = board.Substring(i, 1);

        if (num != "0")
        {
          Labels[i].Content = num;
        }
      }
    }
  }
}