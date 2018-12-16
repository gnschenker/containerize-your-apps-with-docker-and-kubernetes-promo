﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace app
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) {  }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine(">>> Starting Fraud Consumer >>>");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Map("/ws", SocketHandler.Map);
        }
    }
}
