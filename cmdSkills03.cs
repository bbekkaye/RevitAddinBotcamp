using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;

namespace RevitAddinBotcamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSkills03 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 2. Create an instance of the Building class

            Building building1 = new Building("Big Office Building", "10 Main Street",
                10, 150_000);
            Building building2 = new Building("Fancy Hotel", "15 Main Street",
                15, 200_000);

            List<Building> buildings = new List<Building> { building1, building2 };

            buildings.Add(new Building("Hospital", "20 Main Street", 20, 350_000));

            buildings.Add(new Building("Giant Store", "30 Main Street", 5, 40_000));


            // 4. Create an instance of the Neighborhood class
            Neighborhood downtown = new Neighborhood("Downtown", "New York", "NY", buildings);

            TaskDialog.Show("Test", $"There are {downtown.GetBuildingCount()} buildings " +
                                    $"in the {downtown.Name} neighborhood.");

            // 5. Work with rooms.
            FilteredElementCollector roomCollector = new FilteredElementCollector(doc);
            roomCollector.OfCategory(BuiltInCategory.OST_Rooms);

            Room curRoom = roomCollector.First() as Room;

            // Get the room name
            string roomName = curRoom.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();

            // 7a. check room name
            if (roomName.Contains("Office"))
            {
                TaskDialog.Show("Test", "Found the room!");
            }

            // 7.b get room point
            Location roomLocation = curRoom.Location;
            LocationPoint roomLocPt = curRoom.Location as LocationPoint;
            XYZ roomPoint = roomLocPt.Point;


            using Transaction t = new Transaction(doc);
            t.Start("Insert Families into Rooms");

            // 8. insert families
            FamilySymbol familySymbol = GetFamilySymbolByName(doc, "Desk", "Large");
            familySymbol.Activate();

            foreach (Room curRoom2 in roomCollector)
            {
                LocationPoint loc = curRoom2.Location as LocationPoint;

                FamilyInstance curFamInstance = doc.Create.NewFamilyInstance(loc.Point, familySymbol, curRoom2, StructuralType.NonStructural);
            }

            t.Commit();


            return Result.Succeeded;
        }

        private FamilySymbol GetFamilySymbolByName(Document doc, string familyName, string familySymbolName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol));

            foreach (FamilySymbol familySymbol in collector)
            {
                if (familySymbol.Family.Name == familyName && familySymbol.Name == familySymbolName)
                {
                    return familySymbol;
                }
            }

            return null;

        }
      
    }

    
}
