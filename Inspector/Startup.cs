using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inspector
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            using (var db = new ModelContext())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFrameworkSqlite().AddDbContext<ModelContext>();
            services.AddSingleton<IHostedService, InspectorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseExceptionHandler();
            app.UseJsonException();
            app.UseMvc();
        }
    }
}
