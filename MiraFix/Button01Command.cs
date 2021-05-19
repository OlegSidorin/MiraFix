namespace MiraFix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.DB;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class Button01Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            #region Проверка есть ли новый общий параметр в проекте, и если нет то загружает его
            try
            {
                var el = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Element>().ToList().First();
                string parameter = el.LookupParameter("McCm_HostUniqueIdTemp").GUID.ToString();
            }
            catch (Exception )
            {
                CreateSharedParameter createSharedParameter = new CreateSharedParameter();
                createSharedParameter.CreateMiraSharedParameters(doc, commandData.Application.Application);
                //TaskDialog.Show("123", "создан параметр ");
            }
            #endregion

            #region Копирует данные параметра Mira в новый параметр в выделенном элементе
            try
            {
                using (Transaction t = new Transaction(doc, "on45"))
                {
                    t.Start();
                    Element myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());
                    //ElementType myElementType = doc.GetElement(myElement.GetTypeId()) as ElementType;

                    Guid.TryParse("000508ad-5ea5-607b-58dc-861fd8601099", out Guid g);
                    Guid.TryParse("fa6b0a0d-a453-4462-9f3f-12ccc822c304", out Guid gTemp);

                    string pValue = myElement.get_Parameter(g).AsString();
                    if (pValue != "")
                    {
                        myElement.get_Parameter(gTemp).Set(pValue);
                        myElement.get_Parameter(g).Set("");
                    }
                        
                    //SharedParameterElement sp = SharedParameterElement.Lookup(doc, g);
                    //SharedParameterElement spTemp = SharedParameterElement.Lookup(doc, gTemp);
                    //InternalDefinition def = sp.GetDefinition();

                    TaskDialog.Show("123", "did");
                    t.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("123", e.ToString());
            }
            #endregion

            return Result.Succeeded;
        }
    }
}
