using LiteDB;
namespace echeckBlazor.Data
{
	public class LDB
	{
		public readonly LiteDatabase db;

		public ILiteCollection<Worker> Workers
			=> db.GetCollection<Worker>("workers");
		public ILiteCollection<Equipment> Equipments
			=> db.GetCollection<Equipment>("equipments");
		public ILiteCollection<InspectionTask> InspectionTasks
			=> db.GetCollection<InspectionTask>("inspectionTasks");
		public ILiteCollection<InspectionTaskRecord> InspectionTaskRecords
			=> db.GetCollection<InspectionTaskRecord>("inspectionTaskRecords");

		public LDB()
		{
			db = new LiteDatabase("echeckBlazor.db");
		}
	}
}