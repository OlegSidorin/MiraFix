namespace MiraFix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using adWin = Autodesk.Windows;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Main : IExternalApplication
    {
        public static string TabName { get; set; } = "КСП-ТИМ-2";
        public static string PanelName { get; set; } = "Посчитать";
        public static string PanelTransferring { get; set; } = "Посчитать";
        public static string ButtonName { get; set; } = "SumBtn";
        public static string ButtonText { get; set; } = "Сменить\nномер";
        public Result OnStartup(UIControlledApplication application)
        {
            string st = "";
            adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
            foreach (adWin.RibbonTab tab in ribbon.Tabs)
            {
                foreach (adWin.RibbonPanel panel in tab.Panels)
                {
                    foreach (adWin.RibbonItem ribbonItem in panel.Source.Items)
                    {
                        st += tab.Id + " : " + panel.Source.Id + " : " + ribbonItem.Id + Environment.NewLine;
                    }

                }
                st += tab.Id + Environment.NewLine;
            }
            TaskDialog.Show("123", st);
            string workingDir = @"C:\Users\" + Environment.UserName.ToString() + @"\Documents\TEST\";
            if (!Directory.Exists(workingDir)) 
                Directory.CreateDirectory(workingDir);
            WriteToFile(workingDir, "ribbons", st);
            List<RibbonPanel> panels = application.GetRibbonPanels();
            string str = "";

            application.CreateRibbonTab(TabName);
            RibbonPanel panelVS = application.CreateRibbonPanel(TabName, PanelName);
            var mypanels = application.GetRibbonPanels(); //.Where(xxx => xxx.Name == "MiraPanel").First();

            string path = Assembly.GetExecutingAssembly().Location;
            string st1 = "";
            foreach (var p in mypanels)
            {
                st1 += p.Name + p.Title + Environment.NewLine;
            }
            TaskDialog.Show("123", st1);


            PushButtonData SumBtnData = new PushButtonData(ButtonName, ButtonText, path, "KSP_VolumesSum.VSum")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mirafix-32.png", UriKind.Absolute)),
                ToolTip = "Суммирует объемы элементов модели, если они есть"
            };
            PushButton SumBtn = panelVS.AddItem(SumBtnData) as PushButton;
            SumBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mirafix-32.png", UriKind.Absolute));


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public void WriteToFile(string dir, string name, string txt)
        {
            string fileName = dir + "\\" + name + ".txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                writer.Write(txt);
            }
        }

    }
}

    
