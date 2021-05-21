namespace MiraFix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.DB;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class Button02Command : IExternalCommand
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
            catch (Exception)
            {
                CreateSharedParameter createSharedParameter = new CreateSharedParameter();
                createSharedParameter.CreateMiraSharedParameters(doc, commandData.Application.Application);
            }
            #endregion

            #region Копирует данные параметра McCm_HostUniqueIdTemp в McCm_HostUniqueId
            try
            {
                using (Transaction t = new Transaction(doc, "on55"))
                {
                    t.Start();
                    Guid.TryParse("000508ad-5ea5-607b-58dc-861fd8601099", out Guid g);
                    Guid.TryParse("fa6b0a0d-a453-4462-9f3f-12ccc822c304", out Guid gTemp);
                    try
                    {
                        //Element myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());
                        IList<Element> gmodels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel)
                            .WhereElementIsNotElementType().ToElements();
                        //ElementType myElementType = doc.GetElement(myElement.GetTypeId()) as ElementType;
                        foreach (var gmodel in gmodels)
                        {
                            string pValue = gmodel.get_Parameter(g).AsString();
                            string pTempValue = gmodel.get_Parameter(gTemp).AsString();
                            if (pTempValue != "")
                            {
                                gmodel.get_Parameter(g).Set(pTempValue);
                                gmodel.get_Parameter(gTemp).Set("");
                                gmodel.GetParameters("Комментарии")[0].Set("Согласование с инженерным проектом");
                            }
                        }

                        //SharedParameterElement sp = SharedParameterElement.Lookup(doc, g);
                        //SharedParameterElement spTemp = SharedParameterElement.Lookup(doc, gTemp);
                        //InternalDefinition def = sp.GetDefinition();
                    }
                    catch (Exception e)
                    {
                        TaskDialog.Show("Warning", e.ToString());
                        return Result.Succeeded;
                    }

                    t.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Warning", e.ToString());
            }
            #endregion

            return Result.Succeeded;
        }
    }
}
