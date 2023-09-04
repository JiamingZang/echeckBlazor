using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using echeckBlazor.Data;
using MudBlazor.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<LDB>();

builder.Services.AddEndpointsApiExplorer();
//为了让swagger中显示枚举的属性名称为字符串
builder.Services.AddControllers().AddJsonOptions(options =>
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// minimal api的JSON序列化和反序列化配置
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#configure-json-serialization-options
builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.WriteIndented = true;
	options.SerializerOptions.IncludeFields = true;
	// 真正起作用的，将字符串反序列化为枚举类型
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "易巡检API", Description = "巡检端api", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});


app.MapGet("/hello", () => "Hello World!");

app.MapGet("/client/tasks/{id}", (int id) =>
{
	var ldb = app.Services.GetService<LDB>();
	var tasks = ldb?.InspectionTasks.Find(x => x.WorkerId == id).ToList<InspectionTask>();
	var result = tasks?.Select(t =>
	{
		return new
		{
			id = t.Id,
			name = t.Name,
			status = t.Status,
			worker = ldb?.Workers.FindById(t.WorkerId),
			records = ldb?.InspectionTaskRecords.Find(x => x.TaskId == t.Id).Select(r =>
			{
				return new
				{
					id = r.Id,
					equipment = ldb.Equipments.FindById(r.EquipmentId),
					status = r.Status,
					remark = r.Remark,
					image = r.Image
				};
			})
		};
	});
	return result;
});

app.MapPost("/client/submit/{id}", (int id, RecordResult result) =>
{
	var ldb = app.Services.GetService<LDB>();
	var record = ldb?.InspectionTaskRecords.FindById(id);
	record.Status = result.Status;
	record.Remark = result.Remark;
	ldb?.InspectionTaskRecords.Update(record);

	var equipment = ldb?.Equipments.Find(x => x.Id == record.EquipmentId).FirstOrDefault();
	if (result.Status == echeckBlazor.Data.TaskStatus.FAILED)
	{
		equipment.Status = EquipmentStatus.BROKEN;
	}
	else if (result.Status == echeckBlazor.Data.TaskStatus.FINISHED)
	{
		equipment.Status = EquipmentStatus.NORMAL;
	}
	ldb?.Equipments.Update(equipment);

	var task = ldb?.InspectionTasks.FindById(record.TaskId);
	bool TaskFinished = ldb?.InspectionTaskRecords.Find(x => x.TaskId == record.TaskId).ToList()
		.Where(x => x.Status != echeckBlazor.Data.TaskStatus.FINISHED & x.Status != echeckBlazor.Data.TaskStatus.FAILED).ToList().Count == 0;
	if (TaskFinished)
	{
		task.Status = echeckBlazor.Data.TaskStatus.FINISHED;
	}
	else
	{
		task.Status = echeckBlazor.Data.TaskStatus.RUNNING;
	};
	ldb?.InspectionTasks.Update(task);
});

app.MapPost("/client/submit/{id}/img", (int id, Image img) =>
{
	//img:base64string
	var ldb = app.Services.GetService<LDB>();
	Stream imageStream = new MemoryStream();
	byte[] imageArr = Convert.FromBase64String(img.ImageString);
	imageStream.Write(imageArr);
	imageStream.Position = 0;
	var record = ldb?.InspectionTaskRecords.FindById(id);
	record.Image = "record" + record.Id;
	ldb?.InspectionTaskRecords.Update(record);
	ldb?.ImageStorage.Upload("record" + record.Id, "record" + record.Id, imageStream);
});

app.MapPost("/client/login", (LoginRequest request) =>
{
	//img:base64string
	var ldb = app.Services.GetService<LDB>();
	var worker = ldb.Workers.Find(x => x.Name == request.workerName&&x.Password == request.password).FirstOrDefault();
	return worker;
});
app.Run();