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
        public static string TabName { get; set; } = "◄Miracad►";
        public static string PanelName { get; set; } = "Согласование";
        public static string Button1Name { get; set; } = "MoveIDBtn";
        public static string Button1Text { get; set; } = "Согл.\nс КР";
        public static string Button2Name { get; set; } = "ReturnIDBtn";
        public static string Button2Text { get; set; } = "Согл.\nс ИНЖ";
        public Result OnStartup(UIControlledApplication application)
        {
            #region если нужно, то можно получить наименования панелей
            /*
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
            */
            #endregion
            
            string path = Assembly.GetExecutingAssembly().Location;

            //application.CreateRibbonTab(TabName);
            //string str = "";
            var ribbonPanels = application.GetRibbonPanels("◄Miracad►");

            RibbonPanel targetPanel = null;

            foreach (RibbonPanel ribbonPanel in ribbonPanels)
            {
                if (ribbonPanel.Name == "ПО")
                    targetPanel = ribbonPanel;
                
            }
            //TaskDialog.Show("123", str);
            //RibbonPanel panelMira = application.CreateRibbonPanel(TabNameMira, PanelName);
            PushButtonData Mira01BtnData = new PushButtonData(Button1Name, Button1Text, path, "MiraFix.Button01Command")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_01-32.png", UriKind.Absolute)),
                ToolTip = "Переносит McCm_HostUniqueId в McCm_HostUniqueId_KR" +
                "\nдля согласования размещения отверстий с проектом КР"
            };
            Mira01BtnData.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_01-32.png", UriKind.Absolute));
            PushButton Mira01Btn = targetPanel.AddItem(Mira01BtnData) as PushButton;
            //Mira01Btn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_01-32.png", UriKind.Absolute));

            PushButtonData Mira02BtnData = new PushButtonData(Button2Name, Button2Text, path, "MiraFix.Button02Command")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_02-32.png", UriKind.Absolute)),
                ToolTip = "Возвращает McCm_HostUniqueId из McCm_HostUniqueId_KR" +
                "\n  для согласования размещения отверстий с инженерным проектом"
            };
            Mira02BtnData.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_02-32.png", UriKind.Absolute));
            PushButton Mira02Btn = targetPanel.AddItem(Mira02BtnData) as PushButton;
            //Mira02Btn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_02-32.png", UriKind.Absolute));
            //string str = "";

            

            //SplitButtonData sBtnData = new SplitButtonData("splitButton1", "Split");
            //SplitButton sBtn = navigatonPanel.AddItem(sBtnData) as SplitButton;
            //sBtn.AddPushButton(Mira01BtnData);
            //sBtn.AddPushButton(Mira02BtnData);


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        void WriteToFile(string dir, string name, string txt)
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

    
