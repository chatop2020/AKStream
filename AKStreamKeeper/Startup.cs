using System;
using System.IO;
using System.Net;
using LibCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AKStreamKeeper
{
    public class IpAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            IPAddress ip = (IPAddress)value!;
            writer.WriteValue(ip.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            return IPAddress.Parse(token.Value<string>());
        }
    }

    public class IpEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value!;
            writer.WriteStartObject();
            writer.WritePropertyName("Address");
            serializer.Serialize(writer, ep.Address);
            writer.WritePropertyName("Port");
            writer.WriteValue(ep.Port);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"]!.ToObject<IPAddress>(serializer)!;
            int port = jo["Port"]!.Value<int>();
            return new IPEndPoint(address, port);
        }
    }


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //配置跨域处理，允许所有来源
            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });
#if(DEBUG)
            // 注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                // 添加文档信息
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AKStreamKeeper", Version = "v1" });
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "AKStreamKeeper.xml")))
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AKStreamKeeper.xml"), true);
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "LibCommon.xml")))
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "LibCommon.xml"), true);
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "LibZLMediaKitMediaServer.xml")))
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "LibZLMediaKitMediaServer.xml"), true);
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "LibSystemInfo.xml")))
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "LibSystemInfo.xml"), true);
            });
#endif
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddNewtonsoftJson(options =>
                {
                    //修改属性名称的序列化方式，首字母小写
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //修改时间的序列化方式
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter()
                        { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                    options.SerializerSettings.Converters.Add(new IpAddressConverter());
                    options.SerializerSettings.Converters.Add(new IpEndPointConverter());
                }
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

#if(DEBUG)
            // 启用Swagger中间件
            app.UseSwagger();
            // 配置SwaggerUI
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AKStreamKeeper"); }
            );
#endif
            app.UseRouting();
            app.UseCors("cors");
            app.UseMiddleware<ExceptionMiddleware>(); //ExceptionMiddleware 加入管道
            app.UseAuthorization();

            if (!string.IsNullOrEmpty(Common.AkStreamKeeperConfig.CutMergeFilePath))
            {
                try
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider =
                            new PhysicalFileProvider(Common.CutOrMergePath),
                        OnPrepareResponse = (c) =>
                        {
                            c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        },
                        RequestPath = new PathString("/CutMergeFile")
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    return;
                }
            }
            else
            {
                if (!Directory.Exists(GCommon.BaseStartPath + "/CutMergeFile"))
                {
                    Directory.CreateDirectory(GCommon.BaseStartPath + "/CutMergeFile");
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider =
                        new PhysicalFileProvider(GCommon.BaseStartPath + "/CutMergeFile"),
                    OnPrepareResponse = (c) => { c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); },
                    RequestPath = new PathString("/" + (GCommon.BaseStartPath + "/CutMergeFile").TrimStart('/'))
                });
            }

            if (Common.AkStreamKeeperConfig.CustomRecordPathList != null &&
                Common.AkStreamKeeperConfig.CustomRecordPathList.Count > 0)
            {
                foreach (var path in Common.AkStreamKeeperConfig.CustomRecordPathList)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider =
                            new PhysicalFileProvider(path),
                        OnPrepareResponse = (c) =>
                        {
                            c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        },
                        RequestPath = new PathString("/" + path.TrimStart('/'))
                    });
                }
            }

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}