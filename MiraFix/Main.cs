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
        //public static string TabName { get; set; } = "КСП-ТИМ-Mira2";
        public static string TabNameMira { get; set; } = "◄Miracad►";
        public static string PanelName { get; set; } = "ChangeId";
        //public static string PanelTransferring { get; set; } = "CustomCtrl_%◄Miracad►%Панель навигации";
        public static string Button1Name { get; set; } = "MoveIDBtn";
        public static string Button1Text { get; set; } = "Очистить\nID";
        public static string Button2Name { get; set; } = "ReturnIDBtn";
        public static string Button2Text { get; set; } = "Вернуть\nID";
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

            #region другой способ, дает только созданиые плагинами, но Miracad почему-то нет
            /*
            List<RibbonPanel> panels = application.GetRibbonPanels();
            var mypanels = application.GetRibbonPanels(); //.Where(xxx => xxx.Name == "MiraPanel").First();
            string str = "";
            foreach (var p in mypanels)
            {
                str += p.Name + p.Title + Environment.NewLine;
            }
            TaskDialog.Show("123", str);
            */
            #endregion
            
            //application.CreateRibbonTab(TabName);
            RibbonPanel panelMira = application.CreateRibbonPanel(TabNameMira, PanelName);
            PushButtonData Mira01BtnData = new PushButtonData(Button1Name, Button1Text, path, "MiraFix.Button01Command")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_01-32.png", UriKind.Absolute)),
                ToolTip = "Удаляет ID"
            };
            PushButton Mira01Btn = panelMira.AddItem(Mira01BtnData) as PushButton;
            Mira01Btn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_01-32.png", UriKind.Absolute));
            
            PushButtonData Mira02BtnData = new PushButtonData(Button2Name, Button2Text, path, "MiraFix.Button02Command")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_02-32.png", UriKind.Absolute)),
                ToolTip = "Возвращает ID"
            };
            PushButton Mira02Btn = panelMira.AddItem(Mira02BtnData) as PushButton;
            Mira02Btn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(path) + "\\res\\mira_button_02-32.png", UriKind.Absolute));

            //PlaceButtonOnMiraRibbon();


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
        
        void PlaceButtonOnMiraRibbon()
        {
            /*

            try
            {
                String SystemTabId = "◄Miracad►";
                String SystemPanelId = "CustomCtrl_%◄Miracad►%Панель навигации";

                adWin.RibbonControl adWinRibbon = adWin.ComponentManager.Ribbon;

                adWin.RibbonTab adWinSysTab = null;
                adWin.RibbonPanel adWinSysPanel = null;

                adWin.RibbonTab adWinApiTab = null;
                adWin.RibbonPanel adWinApiPanel = null;
                adWin.RibbonItem adWinApiItem = null;

                foreach (adWin.RibbonTab ribbonTab in adWinRibbon.Tabs)
                {
                    // Look for the specified system tab

                    if (ribbonTab.Id == SystemTabId)
                    {
                        adWinSysTab = ribbonTab;

                        foreach (adWin.RibbonPanel ribbonPanel in ribbonTab.Panels)
                        {
                            // Look for the specified panel 
                            // within the system tab

                            if (ribbonPanel.Source.Id == SystemPanelId)
                            {
                                adWinSysPanel = ribbonPanel;
                            }
                        }
                    }
                    else
                    {
                        // Look for our API tab

                        if (ribbonTab.Id == Main.TabName)
                        {
                            adWinApiTab = ribbonTab;

                            foreach (adWin.RibbonPanel ribbonPanel in ribbonTab.Panels)
                            {
                                if (ribbonPanel.Source.Id == "CustomCtrl_%" + Main.TabName + "%" + Main.PanelName)
                                {
                                    foreach (adWin.RibbonItem ribbonItem in ribbonPanel.Source.Items)
                                    {
                                        if (ribbonItem.Id == "CustomCtrl_%CustomCtrl_%" + Main.TabName + "%" + Main.PanelName + "%" + Main.Button1Name)
                                        {
                                            adWinApiItem = ribbonItem;
                                        }
                                    }
                                }

                                if (ribbonPanel.Source.Id == "CustomCtrl_%" + Main.TabName + "%" + Main.PanelName)
                                {
                                    adWinApiPanel = ribbonPanel;
                                }
                            }
                        }
                    }
                }


                if (adWinSysTab != null
                  && adWinSysPanel != null
                  && adWinApiTab != null
                  && adWinApiPanel != null
                   && adWinApiItem != null)
                {
                    adWinSysTab.Panels.Add(adWinApiPanel);
                    adWinApiTab.IsVisible = false;
                    //adWinApiPanel.Source.Items.Add(adWinApiItem);
                    //adWinApiTab.Panels.Remove(adWinApiPanel);
                }


            }

            #region catch and finally
            catch (Exception ex)
            {
                TaskDialog.Show("me", ex.Message + Environment.NewLine + ex.InnerException);
            }
            finally
            {
            }
            #endregion
            */
        }

    }
}

    
