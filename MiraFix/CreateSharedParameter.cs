namespace MiraFix
{
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System;
    using System.IO;
    using System.Reflection;

    class CreateSharedParameter
    {
        public void CreateMiraSharedParameters(Document doc, Application app)
        {
            string path = Assembly.GetExecutingAssembly().Location;

            Guid guid; 
            Guid.TryParse("fa6b0a0d-a453-4462-9f3f-12ccc822c304", out guid);

            CategorySet categorySet = app.Create.NewCategorySet();
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Levels));
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel));
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls));
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexPipeCurves));
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves));

            string originalFile = app.SharedParametersFilename;
            string tempFile = Path.GetDirectoryName(path) + "\\res\\MIRAFIX_FOP.txt";

            try
            {
                app.SharedParametersFilename = tempFile;

                DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();

                foreach (DefinitionGroup dg in sharedParameterFile.Groups)
                {
                    if (dg.Name == "GROUP")
                    {
                        ExternalDefinition externalDefinition = dg.Definitions.get_Item("McCm_HostUniqueIdTemp") as ExternalDefinition;
                        guid = externalDefinition.GUID;
                        using (Transaction t = new Transaction(doc))
                        {
                            t.Start("Add Shared Parameters");
                            InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);
                            doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.INVALID);
                            SharedParameterElement sp = SharedParameterElement.Lookup(doc, guid);
                            InternalDefinition def = sp.GetDefinition();
                            def.SetAllowVaryBetweenGroups(doc, true);
                            t.Commit();
                        }
                    }
                }
            }
            catch { }
            finally
            {
                app.SharedParametersFilename = originalFile;
            }
        }
    }
}
