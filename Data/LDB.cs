using System.Data.Common;
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

		public ILiteStorage<string> ImageStorage
			=> db.FileStorage;

		public LDB()
		{
			db = new LiteDatabase("echeckBlazor.db");
		}
		public string GetImageSrcString(string Img)
		{
			var imageFromLocal = db.FileStorage.Find(x => x.Filename.Equals(Img));
			if (imageFromLocal.Any())
			{
				Stream imageStream = new MemoryStream();
				var res = db.FileStorage.Download(imageFromLocal.First().Id, imageStream);
				imageStream.Position = 0;
				byte[] imageArr = new byte[imageStream.Length];
				imageStream.Read(imageArr, 0, imageArr.Length);
				string base64str = Convert.ToBase64String(imageArr);
				return "data:image/png;base64," + base64str;
				
			}
			else
			{
				return Img;
			}
		}

	}
}