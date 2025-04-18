global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Net.Http;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Abstractions;
global using Microsoft.AspNetCore.Mvc.Formatters;
global using Microsoft.AspNetCore.Mvc.Infrastructure;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.EntityFrameworkCore.Query;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
//global using Microsoft.EntityFrameworkCore.Relational;
global using Microsoft.EntityFrameworkCore.SqlServer;

global using Microsoft.Extensions.Diagnostics.HealthChecks;
//global using Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;

global using LinqKit;

global using Microsoft.IdentityModel.Tokens;
global using Microsoft.IdentityModel.Logging;
global using Microsoft.IdentityModel.JsonWebTokens;
global using Microsoft.IdentityModel.Protocols;
global using Microsoft.IdentityModel.Protocols.OpenIdConnect;
global using System.IdentityModel.Tokens.Jwt;

global using Swashbuckle.AspNetCore.Annotations;
global using Swashbuckle.AspNetCore.Swagger;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Swashbuckle.AspNetCore.SwaggerUI;
global using NSwag;
global using NSwag.Generation.Processors;
global using NSwag.Generation.AspNetCore;
 
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Hosting;
global using Microsoft.OpenApi.Models;