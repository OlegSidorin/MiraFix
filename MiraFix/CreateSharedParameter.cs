namespace MiraFix
{
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;

    class CreateSharedParameter
    {
        public void CreateMiraSharedParameters(Document doc, Application app)
        {
            string path = Assembly.GetExecutingAssembly().Location;

            Guid guid; 
            Guid.TryParse("fa6b0a0d-a453-4462-9f3f-12ccc822c304", out guid);

            CategorySet catSet = app.Create.NewCategorySet();
            Categories categories = doc.Settings.Categories;

            foreach (Category c in categories)
            {
                if (c.AllowsBoundParameters)
                {
                    try
                    {
                        catSet.Insert(doc.Settings.Categories.get_Item(c.Name));
                    }
                    catch (Exception e)
                    {
                        //TaskDialog.Show("Warning", e.ToString());
                    }
                }
            }
            string originalFile = app.SharedParametersFilename;
            string tempFile = Path.GetDirectoryName(path) + "\\res\\MIRAFIX_FOP.txt";

            try
            {
                app.SharedParametersFilename = tempFile;

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Add Shared Parameters");
                    DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();
                    DefinitionGroup sharedParameterGroup = sharedParameterFile.Groups.get_Item("GROUP");
                    Definition sharedParameterDefinition = sharedParameterGroup.Definitions.get_Item("McCm_HostUniqueId_KR");
                    ExternalDefinition externalDefinition =
                        sharedParameterGroup.Definitions.get_Item("McCm_HostUniqueId_KR") as ExternalDefinition;
                    guid = externalDefinition.GUID;
                    InstanceBinding newIB = app.Create.NewInstanceBinding(catSet);
                    doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.INVALID);
                            
                    SharedParameterElement sp = SharedParameterElement.Lookup(doc, guid);
                    InternalDefinition def = sp.GetDefinition();
                    def.SetAllowVaryBetweenGroups(doc, true);
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                TaskDialog.Show("Warning", e.ToString());
            }
            finally
            {
                app.SharedParametersFilename = originalFile;
            }
        }

    }
}
