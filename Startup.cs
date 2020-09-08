
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IPChecker
{
	public class Startup
	{
		private IServiceCollection _services;
		private AddressInfo _lastAddress;

		public void ConfigureServices(IServiceCollection services)
		{
			_services = services;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
		
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await WriteContext(context, GetAddr(context));
				});
				
				endpoints.MapGet("/update", async context =>
				{
					using (var db = new IPCheckerContext())
					{
						db.Database.ExecuteSqlInterpolated($"DELETE FROM Addresses");
						db.Addresses.Add(_lastAddress = GetAddr(context));
						db.SaveChanges();
					}

					await WriteContext(context, _lastAddress);
				});

				endpoints.MapGet("/get-last-address", async context =>
				{

					if (_lastAddress == null)
					{
						using (var db = new IPCheckerContext())
						{
							if((_lastAddress = db.Addresses.FirstOrDefault()) == null)
							{
								_lastAddress = new AddressInfo();
							}
						}
					}

					await WriteContext(context, _lastAddress);
				}
				);
			});
		}

		private AddressInfo GetAddr(HttpContext context)
		{
			var addr = context.Connection.RemoteIpAddress;
			return new AddressInfo { Time = DateTime.Now, Address6 = addr.MapToIPv6().ToString(), Address4 = addr.MapToIPv4().ToString() };
		}

		private async Task WriteContext(HttpContext context, AddressInfo addr)
		{
			context.Response.ContentType = "text/json";
			await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(addr));
		}
	}
}
