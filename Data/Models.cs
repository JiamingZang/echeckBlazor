namespace echeckBlazor.Data
{
	#region data models
	public class Worker
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Password { get; set; }
		public string? Email { get; set; }
		public string? Sex { get; set; }

	}
	public enum EquipmentStatus
	{
		NORMAL = 0,
		BROKEN,
		SCRAPED,
	}

	public enum TaskStatus
	{
		WAITING = 0,
		RUNNING,
		FINISHED,
		FAILED,
	}

	public class Equipment
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? Location { get; set; }
		public EquipmentStatus Status { get; set; } = EquipmentStatus.NORMAL;
		public string? Img { get; set; }
	}

	public class InspectionTask
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public int WorkerId { get; set; }
		public TaskStatus Status { get; set; } = TaskStatus.WAITING;

	}

	public class InspectionTaskRecord
	{
		public int Id { get; set; }
		public string? Remark { get; set; }
		public TaskStatus Status { get; set; } = TaskStatus.WAITING;
		public string? Image { get; set; }
		public int TaskId { get; set; }
		public int EquipmentId { get; set; }
	}
	#endregion

	#region dto
	public struct RecordResult
	{
		public string? Remark { get; set; }
		public TaskStatus Status { get; set; }
	}

	public struct Image
	{
		public string? ImageString { get; set; }
	}
	#endregion
}