# ASP.NET MVC 4.5 → .NET 10 Core MVC Migration Analysis Report
## MvcMusicStore Application

**Analysis Date:** 2025-01-07  
**Source Framework:** ASP.NET MVC 4.5 on .NET Framework 4.5  
**Target Framework:** ASP.NET Core MVC on .NET 10  
**Architecture Constraint:** MVC-only (single project)

---

## EXECUTIVE SUMMARY

The MvcMusicStore application is a classic ASP.NET MVC 4.5 e-commerce music store built on .NET Framework 4.5. The migration to .NET 10 Core MVC is rated **Complex** due to the following primary drivers:

- Deep `System.Web` dependency across all layers
- WebMatrix SimpleMembership / OAuthWebSecurity authentication stack (no direct .NET Core equivalent)
- Entity Framework 5.0 (not EF Core) with `DropCreateDatabaseIfModelChanges` initializer
- `ChildActionOnly` actions (`Html.Action`, `Html.RenderAction`) used in layout
- `System.Web.Optimization` bundling/minification
- `DotNetOpenAuth` OAuth library (no .NET Core port)
- `FormCollection` usage in controllers
- `HttpContextBase` passed directly to model layer (ShoppingCart)
- `WebApiConfig` / `GlobalConfiguration` (legacy Web API)
- `web.config` Forms Authentication configuration

**Total Issues Found:** 52  
**Critical:** 12 | **High:** 18 | **Medium:** 14 | **Low:** 8  
**Estimated Effort:** 80–120 hours  
**Compatibility Score:** 22/100 (before migration)

---

## PART 1: STEP-BY-STEP MIGRATION PLAN

### Phase 1 – Pre-Migration Assessment (Week 1)
**Tasks:**
1. Inventory all NuGet packages and identify .NET 10 equivalents
2. Document all `System.Web` usages across the codebase
3. Identify all `ChildActionOnly` actions and plan ViewComponent replacements
4. Audit authentication stack (SimpleMembership → ASP.NET Core Identity)
5. Capture all route definitions and verify they can be preserved
6. Document all `web.config` settings that must migrate to `appsettings.json`
7. Baseline the existing database schema (MvcMusicStore.mdf)

**Deliverables:** Migration inventory spreadsheet, risk register

**Validation Checkpoint:** All blockers catalogued with severity and owner assigned

---

### Phase 2 – Environment Setup (Week 1–2)
**Tasks:**
1. Install .NET 10 SDK (`dotnet --version` → 10.x.x)
2. Install `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef`
3. Create new ASP.NET Core MVC project (single project, MVC-only):
   ```
   dotnet new mvc -n MvcMusicStore --framework net10.0
   ```
4. Create solution file:
   ```
   dotnet new sln -n MvcMusicStore
   dotnet sln MvcMusicStore.sln add MvcMusicStore/MvcMusicStore.csproj
   ```
5. Add required NuGet packages to new `.csproj`:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
   <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.0" />
   <PackageReference Include="Microsoft.AspNetCore.Authentication.Cookies" Version="10.0.0" />
   <PackageReference Include="LigerShark.WebOptimizer.Core" Version="3.0.x" />
   <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
   ```
6. Configure `appsettings.json` with connection strings from `web.config`

**Deliverables:** Compilable empty .NET 10 MVC project with solution file

---

### Phase 3 – Core Infrastructure Migration (Week 2–3)
**Tasks:**
1. Create `Program.cs` replacing `Global.asax.cs`, `RouteConfig.cs`, `FilterConfig.cs`, `BundleConfig.cs`, `WebApiConfig.cs`, `AuthConfig.cs`, `AppConfig.cs`
2. Configure middleware pipeline (routing, static files, authentication, session, error handling)
3. Migrate `web.config` → `appsettings.json`
4. Set up `_ViewImports.cshtml` with Tag Helper imports
5. Set up `_ViewStart.cshtml`
6. Configure `wwwroot/` folder structure

**Deliverables:** Working `Program.cs`, `appsettings.json`, `_ViewImports.cshtml`

---

### Phase 4 – Data Layer Migration (Week 3–4)
**Tasks:**
1. Migrate `MusicStoreEntities` (EF5 DbContext) → EF Core `DbContext`
2. Remove `DropCreateDatabaseIfModelChanges` initializer → use EF Core Migrations
3. Migrate `UsersContext` → merge into `ApplicationDbContext` with ASP.NET Core Identity
4. Scaffold EF Core from existing SQL Server database:
   ```
   dotnet ef dbcontext scaffold "Server=(localdb)\mssqllocaldb;Database=MvcMusicStore;Trusted_Connection=True;" \
     Microsoft.EntityFrameworkCore.SqlServer \
     --output-dir Models/Entities \
     --context-dir Data \
     --context MusicStoreDbContext \
     --data-annotations \
     --force
   ```
5. Create initial EF Core migration:
   ```
   dotnet ef migrations add InitialCreate --output-dir Data/Migrations
   dotnet ef database update
   ```
6. Migrate `SampleData` seeder → `IHostedService` or `DbContext.OnModelCreating` seed

**Deliverables:** EF Core DbContext, Migrations folder, seeded database

---

### Phase 5 – Controllers and Models Migration (Week 4–5)
**Tasks:**
1. Update all controllers: `System.Web.Mvc.Controller` → `Microsoft.AspNetCore.Mvc.Controller`
2. Replace `ActionResult` return types with `IActionResult`
3. Replace `FormCollection` → model binding with strongly-typed models
4. Replace `HttpNotFound()` → `NotFound()`
5. Replace `TryUpdateModel()` → model binding parameter
6. Replace `HttpContext` (System.Web) → `HttpContext` (ASP.NET Core) via `IHttpContextAccessor`
7. Replace `Session[key]` → `HttpContext.Session.GetString(key)` / `SetString(key, value)`
8. Replace `ChildActionOnly` + `Html.Action` → ViewComponents
9. Replace `Json(results)` → `Json(results)` (same syntax, different namespace)
10. Replace `[Bind(Include=...)]` → `[Bind]` attribute or ViewModel approach
11. Migrate `StoreManagerController.Dispose()` → DI-managed DbContext lifetime

**Deliverables:** All 6 controllers migrated and compiling

---

### Phase 6 – Views Migration (Week 5–6)
**Tasks:**
1. Add `_ViewImports.cshtml` with `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`
2. Replace `@Html.ActionLink(...)` → `<a asp-controller="..." asp-action="...">` Tag Helpers
3. Replace `@Html.BeginForm(...)` → `<form asp-controller="..." asp-action="...">` Tag Helpers
4. Replace `@Html.TextBoxFor(...)` → `<input asp-for="..." />`
5. Replace `@Html.ValidationMessageFor(...)` → `<span asp-validation-for="..." />`
6. Replace `@Styles.Render(...)` → `<link>` tags or WebOptimizer
7. Replace `@Scripts.Render(...)` → `<script>` tags or WebOptimizer
8. Replace `@Html.Partial(...)` → `<partial name="..." />`
9. Replace `@Html.Action(...)` → ViewComponent invocations `@await Component.InvokeAsync(...)`
10. Move static files: `Content/` → `wwwroot/css/`, `Scripts/` → `wwwroot/js/`, `Images/` → `wwwroot/images/`

**Deliverables:** All 25 views migrated

---

### Phase 7 – Authentication and Authorization Migration (Week 6–7)
**Tasks:**
1. Replace `WebSecurity` (SimpleMembership) → ASP.NET Core Identity `UserManager<IdentityUser>` + `SignInManager<IdentityUser>`
2. Replace `OAuthWebSecurity` → ASP.NET Core external authentication providers
3. Replace `Roles.RoleExists()` / `Roles.CreateRole()` → `RoleManager<IdentityRole>`
4. Replace `[InitializeSimpleMembership]` filter → Identity configured in `Program.cs`
5. Replace `MembershipCreateUserException` → `IdentityResult` error handling
6. Replace `FormsAuthentication` (web.config) → Cookie Authentication in `Program.cs`
7. Migrate `AccountController` to use Core Identity APIs
8. Remove `DotNetOpenAuth` packages → use `Microsoft.AspNetCore.Authentication.Google/Facebook/Twitter`
9. Migrate `UsersContext` / `UserProfile` → `IdentityDbContext<ApplicationUser>`

**Deliverables:** Working authentication with ASP.NET Core Identity

---

### Phase 8 – Advanced Features Migration (Week 7–8)
**Tasks:**
1. Replace `ChildActionOnly` `CartSummary` → `CartSummaryViewComponent`
2. Replace `ChildActionOnly` `GenreMenu` → `GenreMenuViewComponent`
3. Replace `ChildActionOnly` `ExternalLoginsList` → `ExternalLoginsListViewComponent`
4. Replace `ChildActionOnly` `RemoveExternalLogins` → `RemoveExternalLoginsViewComponent`
5. Replace `System.Web.Optimization` bundles → WebOptimizer or LibMan
6. Replace `WebApiConfig` / `GlobalConfiguration` → ASP.NET Core minimal API or controller-based API
7. Replace `HandleErrorAttribute` global filter → `UseExceptionHandler` middleware
8. Replace `ValidateAntiForgeryToken` → same attribute (compatible in Core)
9. Replace `TempData` usage → same API (compatible in Core with session provider)

**Deliverables:** All advanced features working

---

### Phase 9 – Testing and Validation (Week 8–9)
**Tasks:**
1. Create unit test project: `dotnet new xunit -n MvcMusicStore.Tests`
2. Add to solution: `dotnet sln add MvcMusicStore.Tests/MvcMusicStore.Tests.csproj`
3. Write controller unit tests using `Moq`
4. Write EF Core repository tests using `InMemory` provider
5. Write integration tests using `WebApplicationFactory<Program>`
6. Run all tests: `dotnet test`
7. Verify build: `dotnet build MvcMusicStore.sln`

**Deliverables:** Test project with >70% coverage on controllers

---

### Phase 10 – Deployment and Cutover (Week 9–10)
**Tasks:**
1. Configure `appsettings.Production.json` with production connection strings
2. Publish: `dotnet publish -c Release -o ./publish`
3. Deploy to IIS or Azure App Service
4. Run smoke tests against production
5. Monitor logs for errors
6. Execute rollback plan if critical issues found

**Deliverables:** Production deployment, monitoring dashboard

---

## PART 2: FOLDER-BY-FOLDER MAPPING

| Legacy Path | .NET 10 Core Path | Notes |
|---|---|---|
| `Global.asax` / `Global.asax.cs` | `Program.cs` | All startup logic consolidated |
| `App_Start/RouteConfig.cs` | `Program.cs` → `app.MapControllerRoute(...)` | |
| `App_Start/FilterConfig.cs` | `Program.cs` → `builder.Services.AddControllersWithViews(o => o.Filters.Add(...))` | |
| `App_Start/BundleConfig.cs` | `Program.cs` → WebOptimizer or LibMan | |
| `App_Start/AuthConfig.cs` | `Program.cs` → `builder.Services.AddAuthentication(...)` | |
| `App_Start/AppConfig.cs` | `Program.cs` → `app.Services.CreateScope()` seed | |
| `App_Start/WebApiConfig.cs` | `Program.cs` → `app.MapControllers()` | |
| `Controllers/*.cs` | `Controllers/*.cs` | Update namespaces and base class |
| `Models/*.cs` | `Models/*.cs` | Remove System.Web references |
| `Models/MusicStoreEntities.cs` | `Data/MusicStoreDbContext.cs` | EF Core DbContext |
| `Models/AccountModels.cs` | `Models/AccountViewModels.cs` + Identity | Split view models from Identity |
| `Models/SampleData.cs` | `Data/DbInitializer.cs` | EF Core seeder |
| `ViewModels/*.cs` | `ViewModels/*.cs` | No changes needed |
| `Views/*.cshtml` | `Views/*.cshtml` | Update HTML helpers → Tag Helpers |
| `Views/_ViewStart.cshtml` | `Views/_ViewStart.cshtml` | Same |
| `Views/Web.config` | `Views/_ViewImports.cshtml` | Tag Helper imports |
| `Filters/InitializeSimpleMembershipAttribute.cs` | **DELETE** | Replaced by Identity in Program.cs |
| `Content/Site.css` | `wwwroot/css/site.css` | |
| `Content/themes/` | `wwwroot/lib/jquery-ui/` | Via LibMan |
| `Scripts/jquery-*.js` | `wwwroot/lib/jquery/` | Via LibMan |
| `Scripts/jquery.validate*.js` | `wwwroot/lib/jquery-validation/` | Via LibMan |
| `Scripts/modernizr-*.js` | `wwwroot/lib/modernizr/` | Via LibMan |
| `Images/` | `wwwroot/images/` | |
| `App_Data/` | **DELETE** | Use SQL Server proper |
| `web.config` | `appsettings.json` | Connection strings, app settings |
| `Web.Debug.config` | `appsettings.Development.json` | |
| `Web.Release.config` | `appsettings.Production.json` | |
| `packages.config` | `*.csproj` PackageReference | SDK-style project |
| `MvcMusicStore.csproj` (old-style) | `MvcMusicStore.csproj` (SDK-style) | |
| `Properties/AssemblyInfo.cs` | **DELETE** | SDK-style auto-generates |

---

## PART 3: TRANSFORMATION RULEBOOK (30+ Rules)

### RULE-001: Project File Format
**Legacy:**
```xml
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="...">
  <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
  <Reference Include="System.Web.Mvc, Version=4.0.0.0...">
```
**.NET 10 Equivalent:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
  </ItemGroup>
</Project>
```
**Explanation:** Replace old-style MSBuild project with SDK-style. All System.Web references are removed. NuGet packages use PackageReference.

---

### RULE-002: Application Entry Point (Global.asax → Program.cs)
**Legacy (Global.asax.cs):**
```csharp
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        WebApiConfig.Register(GlobalConfiguration.Configuration);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        AuthConfig.RegisterAuth();
        AppConfig.Configure();
    }
}
```
**.NET 10 Equivalent (Program.cs):**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HandleErrorFilter>();
});
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicStoreEntities")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MusicStoreDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSession();
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("/css/bundle.css", "css/site.css");
    pipeline.AddJavaScriptBundle("/js/bundle.js", "js/jquery.js", "js/jquery.validate.js");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicStoreDbContext>();
    await DbInitializer.SeedAsync(db, scope.ServiceProvider);
}

app.Run();
```
**Explanation:** All startup configuration moves to `Program.cs`. No more `HttpApplication` subclass.

---

### RULE-003: Controller Base Class
**Legacy:**
```csharp
using System.Web.Mvc;
public class HomeController : Controller { }
```
**.NET 10 Equivalent:**
```csharp
using Microsoft.AspNetCore.Mvc;
public class HomeController : Controller { }
```
**Explanation:** Namespace changes from `System.Web.Mvc` to `Microsoft.AspNetCore.Mvc`. The `Controller` class API is largely compatible.

---

### RULE-004: Action Result Return Types
**Legacy:**
```csharp
public ActionResult Index() { return View(); }
public ActionResult Details(int id) { return HttpNotFound(); }
```
**.NET 10 Equivalent:**
```csharp
public IActionResult Index() { return View(); }
public IActionResult Details(int id) { return NotFound(); }
```
**Explanation:** `ActionResult` → `IActionResult`. `HttpNotFound()` → `NotFound()`.

---

### RULE-005: FormCollection → Model Binding
**Legacy:**
```csharp
[HttpPost]
public ActionResult AddressAndPayment(FormCollection values)
{
    var order = new Order();
    TryUpdateModel(order);
    string promoCode = values["PromoCode"];
}
```
**.NET 10 Equivalent:**
```csharp
[HttpPost]
public async Task<IActionResult> AddressAndPayment(Order order, string promoCode)
{
    if (!ModelState.IsValid) return View(order);
    // promoCode is bound from form field
}
```
**Explanation:** `FormCollection` is not available in ASP.NET Core. Use strongly-typed model binding or `IFormCollection`.

---

### RULE-006: HttpContext / Session Access
**Legacy:**
```csharp
// In ShoppingCart.cs
public static ShoppingCart GetCart(MusicStoreEntities db, HttpContextBase context)
{
    cart.ShoppingCartId = context.Session[CartSessionKey].ToString();
}
// In Controller
Session[ShoppingCart.CartSessionKey] = UserName;
```
**.NET 10 Equivalent:**
```csharp
// Inject IHttpContextAccessor
public static ShoppingCart GetCart(MusicStoreDbContext db, IHttpContextAccessor httpContextAccessor)
{
    var session = httpContextAccessor.HttpContext!.Session;
    cart.ShoppingCartId = session.GetString(CartSessionKey) ?? Guid.NewGuid().ToString();
}
// In Controller (HttpContext is available directly)
HttpContext.Session.SetString(ShoppingCart.CartSessionKey, UserName);
```
**Explanation:** `HttpContextBase` → `IHttpContextAccessor`. `Session[key]` → `Session.GetString(key)` / `Session.SetString(key, value)`. Session must be configured with `AddSession()` and `UseSession()`.

---

### RULE-007: ChildActionOnly → ViewComponent
**Legacy:**
```csharp
[ChildActionOnly]
public ActionResult CartSummary()
{
    var cart = ShoppingCart.GetCart(storeDB, this.HttpContext);
    ViewBag.CartCount = cart.GetCount();
    return PartialView("CartSummary");
}
// In View:
@Html.Action("CartSummary", "ShoppingCart")
```
**.NET 10 Equivalent:**
```csharp
// ViewComponents/CartSummaryViewComponent.cs
public class CartSummaryViewComponent : ViewComponent
{
    private readonly MusicStoreDbContext _db;
    public CartSummaryViewComponent(MusicStoreDbContext db) { _db = db; }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cart = ShoppingCart.GetCart(_db, HttpContext);
        ViewBag.CartCount = cart.GetCount();
        return View();
    }
}
// In View:
@await Component.InvokeAsync("CartSummary")
// View file: Views/Shared/Components/CartSummary/Default.cshtml
```
**Explanation:** `[ChildActionOnly]` actions are not supported in ASP.NET Core. Replace with ViewComponents.

---

### RULE-008: Html.Action → ViewComponent Invocation
**Legacy (in _Layout.cshtml):**
```csharp
@Html.Action("CartSummary", "ShoppingCart")
@Html.Action("GenreMenu", "Store")
```
**.NET 10 Equivalent:**
```csharp
@await Component.InvokeAsync("CartSummary")
@await Component.InvokeAsync("GenreMenu")
```
**Explanation:** `Html.Action` / `Html.RenderAction` are removed in ASP.NET Core. Use ViewComponents.

---

### RULE-009: Html.ActionLink → Tag Helper
**Legacy:**
```csharp
@Html.ActionLink("Home", "Index", "Home")
@Html.ActionLink("Store", "Index", "Store")
```
**.NET 10 Equivalent:**
```html
<a asp-controller="Home" asp-action="Index">Home</a>
<a asp-controller="Store" asp-action="Index">Store</a>
```
**Explanation:** HTML Helpers still work in ASP.NET Core but Tag Helpers are the preferred approach.

---

### RULE-010: Html.BeginForm → Tag Helper
**Legacy:**
```csharp
@using (Html.BeginForm("Login", "Account", FormMethod.Post, new { @class = "form-horizontal" }))
{
    @Html.AntiForgeryToken()
}
```
**.NET 10 Equivalent:**
```html
<form asp-controller="Account" asp-action="Login" method="post" class="form-horizontal">
    <!-- AntiForgeryToken is automatically included with Tag Helpers -->
</form>
```
**Explanation:** Tag Helper forms automatically include anti-forgery tokens when `asp-controller` / `asp-action` are used.

---

### RULE-011: Html.TextBoxFor → Tag Helper
**Legacy:**
```csharp
@Html.TextBoxFor(m => m.UserName, new { @class = "form-control" })
@Html.ValidationMessageFor(m => m.UserName)
```
**.NET 10 Equivalent:**
```html
<input asp-for="UserName" class="form-control" />
<span asp-validation-for="UserName" class="text-danger"></span>
```
**Explanation:** Input Tag Helpers generate proper `id`, `name`, `type`, and validation attributes automatically.

---

### RULE-012: Styles.Render / Scripts.Render → Tag Helpers or WebOptimizer
**Legacy:**
```csharp
@Styles.Render("~/Content/css")
@Scripts.Render("~/bundles/jquery")
@Scripts.Render("~/bundles/modernizr")
```
**.NET 10 Equivalent (with WebOptimizer):**
```html
<link rel="stylesheet" href="~/css/bundle.css" asp-append-version="true" />
<script src="~/js/bundle.js" asp-append-version="true"></script>
```
**Explanation:** `System.Web.Optimization` is not available in .NET Core. Use `LigerShark.WebOptimizer.Core` or LibMan for client-side library management.

---

### RULE-013: Html.Partial → Partial Tag Helper
**Legacy:**
```csharp
@Html.Partial("_LoginPartial")
```
**.NET 10 Equivalent:**
```html
<partial name="_LoginPartial" />
```
**Explanation:** `<partial>` Tag Helper is the preferred approach. `Html.Partial()` still works but is synchronous.

---

### RULE-014: Entity Framework 5 DbContext → EF Core DbContext
**Legacy:**
```csharp
using System.Data.Entity;
public class MusicStoreEntities : DbContext
{
    public DbSet<Album> Albums { get; set; }
}
```
**.NET 10 Equivalent:**
```csharp
using Microsoft.EntityFrameworkCore;
public class MusicStoreDbContext : DbContext
{
    public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options) : base(options) { }
    public DbSet<Album> Albums { get; set; } = null!;
}
```
**Explanation:** EF Core requires `DbContextOptions` constructor injection. No parameterless constructor. `DbSet` properties should be non-nullable.

---

### RULE-015: EF Database Initializer → EF Core Migrations
**Legacy:**
```csharp
public class SampleData : DropCreateDatabaseIfModelChanges<MusicStoreEntities>
{
    protected override void Seed(MusicStoreEntities context) { ... }
}
// In AppConfig.cs:
Database.SetInitializer(new SampleData());
```
**.NET 10 Equivalent:**
```csharp
// Data/DbInitializer.cs
public static class DbInitializer
{
    public static async Task SeedAsync(MusicStoreDbContext context, IServiceProvider services)
    {
        await context.Database.MigrateAsync(); // Apply pending migrations
        if (!context.Genres.Any())
        {
            context.Genres.AddRange(/* seed data */);
            await context.SaveChangesAsync();
        }
    }
}
// In Program.cs:
using var scope = app.Services.CreateScope();
await DbInitializer.SeedAsync(scope.ServiceProvider.GetRequiredService<MusicStoreDbContext>(),
                               scope.ServiceProvider);
```
**Explanation:** EF Core uses explicit migrations. `DropCreateDatabaseIfModelChanges` does not exist in EF Core.

---

### RULE-016: EF6 Include (string) → EF Core Include (lambda)
**Legacy:**
```csharp
var genreModel = storeDB.Genres.Include("Albums").Single(g => g.Name == genre);
```
**.NET 10 Equivalent:**
```csharp
var genreModel = await _db.Genres.Include(g => g.Albums).SingleAsync(g => g.Name == genre);
```
**Explanation:** EF Core supports both string and lambda `Include`, but lambda is strongly typed and preferred. Also make async.

---

### RULE-017: Synchronous DB Calls → Async
**Legacy:**
```csharp
public ActionResult Index()
{
    var albums = db.Albums.Include(a => a.Genre).Include(a => a.Artist).OrderBy(a => a.Price);
    return View(albums.ToList());
}
```
**.NET 10 Equivalent:**
```csharp
public async Task<IActionResult> Index()
{
    var albums = await _db.Albums.Include(a => a.Genre).Include(a => a.Artist)
                          .OrderBy(a => a.Price).ToListAsync();
    return View(albums);
}
```
**Explanation:** ASP.NET Core is async-first. All EF Core database calls should be awaited.

---

### RULE-018: DbContext Lifetime (Field) → Constructor Injection
**Legacy:**
```csharp
public class StoreManagerController : Controller
{
    private MusicStoreEntities db = new MusicStoreEntities(); // Direct instantiation
    protected override void Dispose(bool disposing)
    {
        db.Dispose();
        base.Dispose(disposing);
    }
}
```
**.NET 10 Equivalent:**
```csharp
public class StoreManagerController : Controller
{
    private readonly MusicStoreDbContext _db;
    public StoreManagerController(MusicStoreDbContext db) { _db = db; }
    // No Dispose needed - DI manages lifetime
}
```
**Explanation:** DbContext should be injected via constructor DI. The DI container manages its lifetime (Scoped by default).

---

### RULE-019: EntityState.Modified → EF Core Update
**Legacy:**
```csharp
db.Entry(album).State = EntityState.Modified;
db.SaveChanges();
```
**.NET 10 Equivalent:**
```csharp
_db.Update(album);
await _db.SaveChangesAsync();
```
**Explanation:** `_db.Update(entity)` is the EF Core equivalent. Also make async.

---

### RULE-020: WebSecurity (SimpleMembership) → ASP.NET Core Identity
**Legacy:**
```csharp
WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe)
WebSecurity.CreateUserAndAccount(model.UserName, model.Password)
WebSecurity.Logout()
WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword)
```
**.NET 10 Equivalent:**
```csharp
// Inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>
await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false)
await _userManager.CreateAsync(new ApplicationUser { UserName = model.UserName }, model.Password)
await _signInManager.SignOutAsync()
await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword)
```
**Explanation:** `WebMatrix.WebData.WebSecurity` has no .NET Core equivalent. Replace with ASP.NET Core Identity's `UserManager` and `SignInManager`.

---

### RULE-021: OAuthWebSecurity → ASP.NET Core External Auth
**Legacy:**
```csharp
OAuthWebSecurity.RegisterGoogleClient();
OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false)
OAuthWebSecurity.VerifyAuthentication(callbackUrl)
```
**.NET 10 Equivalent:**
```csharp
// In Program.cs:
builder.Services.AddAuthentication()
    .AddGoogle(options => {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });
// In AccountController:
var info = await _signInManager.GetExternalLoginInfoAsync();
await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
```
**Explanation:** `DotNetOpenAuth` and `OAuthWebSecurity` have no .NET Core equivalents. Use `Microsoft.AspNetCore.Authentication.Google/Facebook/Twitter` packages.

---

### RULE-022: Roles Provider → RoleManager
**Legacy:**
```csharp
Roles.RoleExists(_role)
Roles.CreateRole(_role)
Roles.AddUserToRole(_username, _role)
```
**.NET 10 Equivalent:**
```csharp
// Inject RoleManager<IdentityRole>
await _roleManager.RoleExistsAsync(_role)
await _roleManager.CreateAsync(new IdentityRole(_role))
await _userManager.AddToRoleAsync(user, _role)
```
**Explanation:** `System.Web.Security.Roles` provider → ASP.NET Core Identity `RoleManager<IdentityRole>`.

---

### RULE-023: Forms Authentication (web.config) → Cookie Authentication (Program.cs)
**Legacy (web.config):**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="2880" />
</authentication>
```
**.NET 10 Equivalent (Program.cs):**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
        options.SlidingExpiration = true;
    });
```
**Explanation:** Forms Authentication is a `System.Web` concept. ASP.NET Core uses Cookie Authentication middleware.

---

### RULE-024: [InitializeSimpleMembership] Filter → Remove
**Legacy:**
```csharp
[Authorize]
[InitializeSimpleMembership]
public class AccountController : Controller { }
```
**.NET 10 Equivalent:**
```csharp
[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AccountController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm)
    {
        _userManager = um;
        _signInManager = sm;
    }
}
```
**Explanation:** `InitializeSimpleMembershipAttribute` is a workaround for lazy initialization of SimpleMembership. In ASP.NET Core, Identity is initialized at startup via DI.

---

### RULE-025: MembershipCreateUserException → IdentityResult
**Legacy:**
```csharp
try
{
    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
}
catch (MembershipCreateUserException e)
{
    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
}
```
**.NET 10 Equivalent:**
```csharp
var result = await _userManager.CreateAsync(new ApplicationUser { UserName = model.UserName }, model.Password);
if (!result.Succeeded)
{
    foreach (var error in result.Errors)
        ModelState.AddModelError(string.Empty, error.Description);
    return View(model);
}
```
**Explanation:** `IdentityResult` replaces exception-based error handling. Errors are returned as a collection.

---

### RULE-026: web.config appSettings → appsettings.json + IConfiguration
**Legacy:**
```xml
<appSettings>
  <add key="DefaultAdminUsername" value="Administrator" />
  <add key="DefaultAdminPassword" value="YouShouldChangeThisPassword" />
</appSettings>
// In code:
string _username = ConfigurationManager.AppSettings["DefaultAdminUsername"];
```
**.NET 10 Equivalent:**
```json
// appsettings.json
{
  "AdminSettings": {
    "DefaultAdminUsername": "Administrator",
    "DefaultAdminPassword": "YouShouldChangeThisPassword"
  }
}
```
```csharp
// In code (via IConfiguration injection):
string username = _configuration["AdminSettings:DefaultAdminUsername"]!;
// Or via IOptions<AdminSettings>:
string username = _adminSettings.Value.DefaultAdminUsername;
```
**Explanation:** `ConfigurationManager.AppSettings` → `IConfiguration` (injected). Use `IOptions<T>` pattern for strongly-typed settings.

---

### RULE-027: web.config connectionStrings → appsettings.json
**Legacy:**
```xml
<connectionStrings>
  <add name="MusicStoreEntities"
       connectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\MvcMusicStore.mdf;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
**.NET 10 Equivalent:**
```json
{
  "ConnectionStrings": {
    "MusicStoreEntities": "Server=(localdb)\\mssqllocaldb;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
```csharp
// In Program.cs:
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicStoreEntities")));
```
**Explanation:** Connection strings move to `appsettings.json`. `|DataDirectory|` is replaced with a proper SQL Server connection string.

---

### RULE-028: HandleErrorAttribute → Exception Handler Middleware
**Legacy:**
```csharp
// FilterConfig.cs
filters.Add(new HandleErrorAttribute());
```
**.NET 10 Equivalent:**
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
```
**Explanation:** `HandleErrorAttribute` is replaced by `UseExceptionHandler` middleware. The `Error.cshtml` view is still used.

---

### RULE-029: RouteConfig → MapControllerRoute
**Legacy:**
```csharp
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```
**.NET 10 Equivalent:**
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```
**Explanation:** `RouteTable.Routes.MapRoute` → `app.MapControllerRoute`. `UrlParameter.Optional` → `?` suffix in pattern.

---

### RULE-030: WebApiConfig → MapControllers
**Legacy:**
```csharp
config.Routes.MapHttpRoute(
    name: "DefaultApi",
    routeTemplate: "api/{controller}/{id}",
    defaults: new { id = RouteParameter.Optional }
);
```
**.NET 10 Equivalent:**
```csharp
// In Program.cs:
app.MapControllers(); // For API controllers with [ApiController] attribute
// Or use minimal APIs:
app.MapGet("/api/{controller}/{id?}", ...);
```
**Explanation:** `GlobalConfiguration.Configuration` and `HttpConfiguration` are `System.Web.Http` concepts. ASP.NET Core unifies MVC and Web API.

---

### RULE-031: [Bind(Include=...)] → ViewModel or [Bind]
**Legacy:**
```csharp
[Bind(Include = "FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email")]
public class Order { ... }
```
**.NET 10 Equivalent:**
```csharp
// Option 1: Keep [Bind] attribute (compatible)
[Bind("FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email")]
public class Order { ... }

// Option 2: Use a dedicated ViewModel (preferred)
public class OrderViewModel
{
    public string FirstName { get; set; } = string.Empty;
    // Only include bindable properties
}
```
**Explanation:** `[Bind(Include=...)]` syntax changes slightly. The preferred approach is a dedicated ViewModel.

---

### RULE-032: _ViewStart.cshtml (no change needed)
**Legacy:**
```csharp
@{ Layout = "~/Views/Shared/_Layout.cshtml"; }
```
**.NET 10 Equivalent:**
```csharp
@{ Layout = "~/Views/Shared/_Layout.cshtml"; }
```
**Explanation:** `_ViewStart.cshtml` syntax is identical. No changes needed.

---

### RULE-033: Add _ViewImports.cshtml (new file required)
**Legacy:** No `_ViewImports.cshtml` (namespaces in `Views/Web.config`)

**.NET 10 Equivalent (Views/_ViewImports.cshtml):**
```csharp
@using MvcMusicStore
@using MvcMusicStore.Models
@using MvcMusicStore.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
**Explanation:** `Views/Web.config` namespace declarations → `_ViewImports.cshtml`. Tag Helpers must be explicitly imported.

---

### RULE-034: Session Configuration
**Legacy:** Session is available by default in ASP.NET MVC (System.Web)

**.NET 10 Equivalent:**
```csharp
// Program.cs - Services:
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Program.cs - Middleware (order matters):
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Must be after UseRouting, before MapControllerRoute
app.MapControllerRoute(...);
```
**Explanation:** Session is opt-in in ASP.NET Core. Must configure `AddSession()` and `UseSession()`.

---

### RULE-035: Static Files Configuration
**Legacy:** Static files served from project root (Content/, Scripts/, Images/)

**.NET 10 Equivalent:**
```csharp
// Program.cs:
app.UseStaticFiles(); // Serves from wwwroot/ by default

// Move files:
// Content/Site.css → wwwroot/css/site.css
// Scripts/jquery-*.js → wwwroot/lib/jquery/
// Images/ → wwwroot/images/
```
**Explanation:** Only files in `wwwroot/` are served as static content in ASP.NET Core.

---

## PART 4: DATA STRATEGY WITH EF CORE SCAFFOLD COMMANDS

### Approach A: Database-First (Scaffold from Existing DB)

**Step 1: Install EF Core tools**
```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

**Step 2: Add required packages to .csproj**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

**Step 3: Scaffold DbContext and entities from existing database**
```bash
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models/Entities \
  --context-dir Data \
  --context MusicStoreDbContext \
  --data-annotations \
  --no-onconfiguring \
  --force \
  --project MvcMusicStore.csproj
```

**Parameter Explanations:**
- `--output-dir Models/Entities` — Where entity classes are generated
- `--context-dir Data` — Where the DbContext class is generated
- `--context MusicStoreDbContext` — Name of the generated DbContext class
- `--data-annotations` — Use data annotation attributes instead of Fluent API
- `--no-onconfiguring` — Don't generate `OnConfiguring` (connection string in appsettings.json instead)
- `--force` — Overwrite existing files
- `--project` — Target project file

**Step 4: Configure DbContext in Program.cs**
```csharp
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicStoreEntities")));
```

---

### Approach B: Code-First (Migrations from Existing Models)

**Step 1: Create initial migration from existing model classes**
```bash
dotnet ef migrations add InitialCreate \
  --output-dir Data/Migrations \
  --project MvcMusicStore.csproj \
  --startup-project MvcMusicStore.csproj
```

**Step 2: Apply migration to database**
```bash
dotnet ef database update \
  --project MvcMusicStore.csproj \
  --startup-project MvcMusicStore.csproj
```

**Step 3: Future schema changes**
```bash
# Add a new migration after model changes:
dotnet ef migrations add AddAlbumDescription \
  --output-dir Data/Migrations

# Apply to database:
dotnet ef database update

# Rollback to previous migration:
dotnet ef database update InitialCreate

# Remove last unapplied migration:
dotnet ef migrations remove
```

---

### DbContext Configuration (EF Core)

```csharp
// Data/MusicStoreDbContext.cs
public class MusicStoreDbContext : DbContext
{
    public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options) : base(options) { }

    public DbSet<Album> Albums { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure relationships, indexes, etc.
        modelBuilder.Entity<Album>()
            .Property(a => a.Price)
            .HasColumnType("decimal(18,2)");
    }
}
```

---

### Identity DbContext (Combined)

```csharp
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Album> Albums { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
}
```

---

## PART 5: AUTHENTICATION MIGRATION GUIDANCE

### A) Forms Authentication → Cookie Authentication

**Legacy (web.config):**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="2880" />
</authentication>
```

**.NET 10 (Program.cs):**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/LogOff";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Middleware order:
app.UseAuthentication();
app.UseAuthorization();
```

**Login Implementation:**
```csharp
// Legacy:
WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe)

// .NET 10:
var result = await _signInManager.PasswordSignInAsync(
    model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
if (result.Succeeded) return RedirectToLocal(returnUrl);
```

**Logout Implementation:**
```csharp
// Legacy:
WebSecurity.Logout();

// .NET 10:
await _signInManager.SignOutAsync();
```

**Claims-Based Identity Access:**
```csharp
// Legacy:
User.Identity.Name

// .NET 10 (same API, but backed by claims):
User.Identity!.Name
// Or get specific claim:
User.FindFirstValue(ClaimTypes.NameIdentifier) // User ID
User.FindFirstValue(ClaimTypes.Name)           // Username
```

---

### B) ASP.NET Identity (SimpleMembership) → ASP.NET Core Identity

**ApplicationUser Model:**
```csharp
// Models/ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    // Add custom properties here if needed
    // UserProfile.UserName is already in IdentityUser.UserName
}
```

**Identity Configuration (Program.cs):**
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

**AccountController Migration:**
```csharp
[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            await MigrateShoppingCartAsync(model.UserName);
            return RedirectToLocal(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
        return View(model);
    }
}
```

**Database Schema Compatibility:**
The ASP.NET Core Identity schema differs from SimpleMembership:
- `UserProfile` table → `AspNetUsers` table
- `webpages_Membership` → `AspNetUsers` (password hash column)
- `webpages_Roles` → `AspNetRoles`
- `webpages_UsersInRoles` → `AspNetUserRoles`

**Migration Script (SQL):**
```sql
-- Migrate existing users from SimpleMembership to ASP.NET Core Identity
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
                         EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                         PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
SELECT 
    NEWID(),
    up.UserName,
    UPPER(up.UserName),
    up.UserName + '@placeholder.com',
    UPPER(up.UserName + '@placeholder.com'),
    1,
    wm.Password, -- Note: Hash format differs, users may need to reset passwords
    NEWID(),
    NEWID(),
    0, 0, 1, 0
FROM UserProfile up
JOIN webpages_Membership wm ON up.UserId = wm.UserId;
```

---

## PART 6: BLOCKERS & MITIGATIONS

### BLOCKER-001: System.Web Namespace Dependency (CRITICAL)
**Description:** The entire application depends on `System.Web` which does not exist in .NET Core. This affects all controllers, models, filters, and configuration files.
**Impact:** HIGH — Blocks compilation entirely
**Files Affected:** All `.cs` files, `web.config`, `Global.asax`
**Mitigation:**
- Replace `using System.Web.Mvc` → `using Microsoft.AspNetCore.Mvc`
- Replace `using System.Web` → `using Microsoft.AspNetCore.Http`
- Replace `using System.Web.Routing` → routing is built into ASP.NET Core
- Replace `using System.Web.Optimization` → `using WebOptimizer`
- Remove all `System.Web.*` assembly references from `.csproj`

---

### BLOCKER-002: WebMatrix.WebData / SimpleMembership (CRITICAL)
**Description:** `WebSecurity`, `WebMatrix.WebData`, `InitializeSimpleMembershipAttribute` have no .NET Core equivalent.
**Impact:** HIGH — AccountController, AppConfig, Filters all fail to compile
**Files Affected:** `AccountController.cs`, `AppConfig.cs`, `Filters/InitializeSimpleMembershipAttribute.cs`
**Mitigation:**
- Replace `WebSecurity.Login()` → `SignInManager.PasswordSignInAsync()`
- Replace `WebSecurity.CreateUserAndAccount()` → `UserManager.CreateAsync()`
- Replace `WebSecurity.Logout()` → `SignInManager.SignOutAsync()`
- Delete `InitializeSimpleMembershipAttribute.cs` entirely
- Configure ASP.NET Core Identity in `Program.cs`

---

### BLOCKER-003: OAuthWebSecurity / DotNetOpenAuth (CRITICAL)
**Description:** `OAuthWebSecurity`, `DotNetOpenAuth.AspNet`, `Microsoft.Web.WebPages.OAuth` have no .NET Core equivalents.
**Impact:** HIGH — AccountController external login methods fail to compile
**Files Affected:** `AccountController.cs`, `AuthConfig.cs`, `packages.config`
**Mitigation:**
- Remove all `DotNetOpenAuth.*` packages
- Remove `Microsoft.Web.WebPages.OAuth` package
- Replace with `Microsoft.AspNetCore.Authentication.Google`, `.Facebook`, `.Twitter` packages
- Rewrite `ExternalLogin`, `ExternalLoginCallback`, `ExternalLoginConfirmation` actions using Core Identity external auth APIs

---

### BLOCKER-004: ChildActionOnly / Html.Action (CRITICAL)
**Description:** `[ChildActionOnly]` attribute and `Html.Action()` / `Html.RenderAction()` are not supported in ASP.NET Core MVC.
**Impact:** HIGH — `CartSummary`, `GenreMenu`, `ExternalLoginsList`, `RemoveExternalLogins` actions fail; layout breaks
**Files Affected:** `ShoppingCartController.cs`, `StoreController.cs`, `AccountController.cs`, `Views/Shared/_Layout.cshtml`
**Mitigation:**
- Create `ViewComponents/CartSummaryViewComponent.cs`
- Create `ViewComponents/GenreMenuViewComponent.cs`
- Create `ViewComponents/ExternalLoginsListViewComponent.cs`
- Create `ViewComponents/RemoveExternalLoginsViewComponent.cs`
- Replace `@Html.Action(...)` → `@await Component.InvokeAsync(...)` in views
- Create view files in `Views/Shared/Components/{ComponentName}/Default.cshtml`

---

### BLOCKER-005: Entity Framework 5 → EF Core (CRITICAL)
**Description:** `EntityFramework 5.0.0` targets .NET Framework only. EF Core is the .NET 10 equivalent.
**Impact:** HIGH — All data access code fails to compile
**Files Affected:** `MusicStoreEntities.cs`, `AccountModels.cs`, `SampleData.cs`, all controllers
**Mitigation:**
- Replace `using System.Data.Entity` → `using Microsoft.EntityFrameworkCore`
- Replace `DbContext` constructor → add `DbContextOptions` constructor
- Replace `DropCreateDatabaseIfModelChanges` → EF Core Migrations
- Replace `Database.SetInitializer()` → `context.Database.MigrateAsync()`
- Replace `db.Entry(x).State = EntityState.Modified` → `db.Update(x)`
- Replace `.Include("Albums")` string includes → `.Include(g => g.Albums)` lambda includes
- Make all DB calls async

---

### BLOCKER-006: HttpContextBase in Model Layer (HIGH)
**Description:** `ShoppingCart.GetCart()` accepts `HttpContextBase` directly, creating a tight coupling to `System.Web`.
**Impact:** HIGH — ShoppingCart model fails to compile; session access pattern must change
**Files Affected:** `Models/ShoppingCart.cs`, `ShoppingCartController.cs`, `CheckoutController.cs`, `AccountController.cs`
**Mitigation:**
```csharp
// Replace:
public static ShoppingCart GetCart(MusicStoreEntities db, HttpContextBase context)
// With:
public static ShoppingCart GetCart(MusicStoreDbContext db, IHttpContextAccessor httpContextAccessor)
// Or pass session directly:
public static ShoppingCart GetCart(MusicStoreDbContext db, ISession session, ClaimsPrincipal user)
```

---

### BLOCKER-007: System.Web.Optimization Bundling (HIGH)
**Description:** `BundleConfig.cs` uses `System.Web.Optimization` (`ScriptBundle`, `StyleBundle`, `BundleTable`) which does not exist in .NET Core.
**Impact:** HIGH — `BundleConfig.cs` fails to compile; `@Styles.Render()` / `@Scripts.Render()` in views fail
**Files Affected:** `App_Start/BundleConfig.cs`, `Views/Shared/_Layout.cshtml`
**Mitigation:**
- Delete `BundleConfig.cs`
- Add `LigerShark.WebOptimizer.Core` NuGet package
- Configure bundles in `Program.cs`
- Replace `@Styles.Render(...)` → `<link>` tags with `asp-append-version="true"`
- Replace `@Scripts.Render(...)` → `<script>` tags with `asp-append-version="true"`

---

### BLOCKER-008: Global.asax Application Events (HIGH)
**Description:** `Global.asax` / `MvcApplication : HttpApplication` lifecycle events (`Application_Start`, `Application_Error`, `Application_BeginRequest`) do not exist in ASP.NET Core.
**Impact:** HIGH — Application startup fails
**Files Affected:** `Global.asax`, `Global.asax.cs`
**Mitigation:**
- Delete `Global.asax` and `Global.asax.cs`
- Move `Application_Start` logic → `Program.cs`
- Move `Application_Error` → `UseExceptionHandler` middleware
- Move `Application_BeginRequest` → custom middleware

---

### BLOCKER-009: web.config System.Web Configuration (HIGH)
**Description:** `<system.web>`, `<system.webServer>`, `<authentication mode="Forms">`, `<compilation>`, `<httpRuntime>` sections are IIS/System.Web specific.
**Impact:** HIGH — Configuration system is completely different
**Files Affected:** `Web.config`, `Web.Debug.config`, `Web.Release.config`
**Mitigation:**
- Migrate `<connectionStrings>` → `appsettings.json`
- Migrate `<appSettings>` → `appsettings.json`
- Migrate `<authentication mode="Forms">` → Cookie Authentication in `Program.cs`
- Delete `<system.web>` section entirely
- Delete `<system.webServer>` section (IIS-specific, handled by hosting)
- Create `appsettings.Development.json` and `appsettings.Production.json`

---

### BLOCKER-010: FormCollection in CheckoutController (HIGH)
**Description:** `CheckoutController.AddressAndPayment(FormCollection values)` uses `FormCollection` which is not available in ASP.NET Core.
**Impact:** HIGH — Checkout action fails to compile
**Files Affected:** `Controllers/CheckoutController.cs`
**Mitigation:**
```csharp
// Replace:
public ActionResult AddressAndPayment(FormCollection values)
{
    var order = new Order();
    TryUpdateModel(order);
    string promoCode = values["PromoCode"];
}
// With:
public async Task<IActionResult> AddressAndPayment(Order order, string promoCode)
{
    if (!ModelState.IsValid) return View(order);
    // promoCode is bound from form field named "PromoCode"
}
```

---

### BLOCKER-011: TryUpdateModel (HIGH)
**Description:** `TryUpdateModel(order)` in `CheckoutController` is available in ASP.NET Core but the pattern is discouraged. Combined with `FormCollection`, it must be replaced.
**Impact:** MEDIUM — Works differently in Core; combined with FormCollection blocker
**Files Affected:** `Controllers/CheckoutController.cs`
**Mitigation:** Use model binding parameter directly in action method signature (see BLOCKER-010).

---

### BLOCKER-012: WebApiConfig / GlobalConfiguration (HIGH)
**Description:** `WebApiConfig.Register(GlobalConfiguration.Configuration)` uses `System.Web.Http.GlobalConfiguration` which does not exist in .NET Core.
**Impact:** HIGH — Startup fails to compile
**Files Affected:** `App_Start/WebApiConfig.cs`, `Global.asax.cs`
**Mitigation:**
- Delete `WebApiConfig.cs`
- In .NET Core, MVC and Web API are unified. Use `app.MapControllers()` for API endpoints.
- If API routes are needed, use `[Route("api/[controller]")]` attribute routing.

---

### BLOCKER-013: Session State (MEDIUM)
**Description:** Session is available by default in ASP.NET MVC (System.Web). In ASP.NET Core, it must be explicitly configured.
**Impact:** MEDIUM — Shopping cart session breaks at runtime if not configured
**Files Affected:** `Models/ShoppingCart.cs`, `Controllers/ShoppingCartController.cs`, `Controllers/AccountController.cs`
**Mitigation:**
```csharp
// Program.cs:
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
app.UseSession(); // After UseRouting, before MapControllerRoute
```

---

### BLOCKER-014: Synchronous I/O Operations (MEDIUM)
**Description:** All controller actions and EF calls are synchronous. ASP.NET Core strongly recommends async I/O.
**Impact:** MEDIUM — Performance degradation; potential deadlocks under load
**Files Affected:** All 6 controllers, `ShoppingCart.cs`
**Mitigation:**
- Change all `ActionResult` → `async Task<IActionResult>`
- Change all `.ToList()` → `await .ToListAsync()`
- Change all `.Single()` → `await .SingleAsync()`
- Change all `.Find()` → `await .FindAsync()`
- Change all `.SaveChanges()` → `await .SaveChangesAsync()`
- Change all `.Any()` → `await .AnyAsync()`

---

### BLOCKER-015: MvcMusicStore.csproj Old-Style Format (MEDIUM)
**Description:** The `.csproj` file uses the old MSBuild format with explicit file listings, `<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>`, and `packages.config`.
**Impact:** MEDIUM — Cannot be used with .NET 10 SDK
**Files Affected:** `MvcMusicStore.csproj`, `packages.config`
**Mitigation:**
- Create new SDK-style `.csproj` with `<TargetFramework>net10.0</TargetFramework>`
- Replace `packages.config` with `<PackageReference>` elements in `.csproj`
- Remove explicit file listings (SDK-style auto-includes)

---

### BLOCKER-016: App_Data / LocalDB MDF Files (MEDIUM)
**Description:** `App_Data/` folder with `.mdf` files and `|DataDirectory|` connection string are IIS/System.Web patterns.
**Impact:** MEDIUM — Database connection fails in .NET Core
**Files Affected:** `Web.config` (connection strings), `App_Data/` folder
**Mitigation:**
- Replace `|DataDirectory|` with proper SQL Server connection string
- Use SQL Server LocalDB: `Server=(localdb)\mssqllocaldb;Database=MvcMusicStore;Trusted_Connection=True;`
- Or migrate to SQL Server Express / full SQL Server
- Remove `App_Data/` folder from project

---

### BLOCKER-017: Newtonsoft.Json Version (LOW)
**Description:** `Newtonsoft.Json 4.5.6` is very old. .NET 10 uses `System.Text.Json` by default.
**Impact:** LOW — Compatibility issue; upgrade required
**Files Affected:** `packages.config`
**Mitigation:**
- Upgrade to `Newtonsoft.Json 13.0.3` if needed for compatibility
- Or migrate to `System.Text.Json` (built-in to .NET 10)
- Add `Microsoft.AspNetCore.Mvc.NewtonsoftJson` if JSON.NET behavior is required

---

### BLOCKER-018: jQuery 1.7.1 / jQuery UI 1.8.20 (LOW)
**Description:** Very old jQuery versions with known security vulnerabilities.
**Impact:** LOW — Security risk; compatibility with modern browsers
**Files Affected:** `Scripts/` folder, `BundleConfig.cs`
**Mitigation:**
- Upgrade to jQuery 3.7.x via LibMan: `libman install jquery --provider cdnjs`
- Upgrade to jQuery UI 1.13.x
- Update `_Layout.cshtml` script references

---

### BLOCKER-019: Knockout.js 2.1.0 (LOW)
**Description:** Knockout.js 2.1.0 is outdated. Consider upgrading or replacing.
**Impact:** LOW — Outdated library
**Files Affected:** `Scripts/knockout-2.1.0.js`
**Mitigation:** Upgrade to Knockout.js 3.5.x or replace with modern framework (Vue.js, Alpine.js).

---

### BLOCKER-020: Missing TempData Provider Configuration (LOW)
**Description:** `TempData` in ASP.NET Core requires explicit provider configuration (Cookie or Session).
**Impact:** LOW — TempData may not work without configuration
**Files Affected:** Any controller using `TempData`
**Mitigation:**
```csharp
// Program.cs (Cookie-based TempData - default in Core):
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider(); // If using session-based TempData
// Or use default cookie-based TempData (no extra config needed)
```

---

## PART 7: UNIT TESTING FRAMEWORK IMPLEMENTATION

### Test Project Setup

```bash
# Create test project
dotnet new xunit -n MvcMusicStore.Tests --framework net10.0

# Add to solution
dotnet sln MvcMusicStore.sln add MvcMusicStore.Tests/MvcMusicStore.Tests.csproj

# Add references
dotnet add MvcMusicStore.Tests reference MvcMusicStore

# Add test packages
dotnet add MvcMusicStore.Tests package Moq --version 4.20.x
dotnet add MvcMusicStore.Tests package Microsoft.EntityFrameworkCore.InMemory --version 10.0.0
dotnet add MvcMusicStore.Tests package Microsoft.AspNetCore.Mvc.Testing --version 10.0.0
dotnet add MvcMusicStore.Tests package FluentAssertions --version 6.x
```

### Test Project Structure
```
MvcMusicStore.Tests/
├── Controllers/
│   ├── HomeControllerTests.cs
│   ├── StoreControllerTests.cs
│   ├── ShoppingCartControllerTests.cs
│   ├── StoreManagerControllerTests.cs
│   ├── CheckoutControllerTests.cs
│   └── AccountControllerTests.cs
├── Models/
│   └── ShoppingCartTests.cs
├── Integration/
│   └── MvcMusicStoreIntegrationTests.cs
├── Helpers/
│   └── TestDbContextFactory.cs
└── MvcMusicStore.Tests.csproj
```

### Example: HomeController Test
```csharp
public class HomeControllerTests
{
    private readonly MusicStoreDbContext _db;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new MusicStoreDbContext(options);
        SeedTestData(_db);
        _controller = new HomeController(_db);
    }

    [Fact]
    public async Task Index_ReturnsViewWithTopAlbums()
    {
        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Album>>(viewResult.Model);
        Assert.True(model.Count() <= 6);
    }

    private static void SeedTestData(MusicStoreDbContext db)
    {
        var genre = new Genre { GenreId = 1, Name = "Rock" };
        var artist = new Artist { ArtistId = 1, Name = "Test Artist" };
        db.Genres.Add(genre);
        db.Artists.Add(artist);
        db.Albums.AddRange(Enumerable.Range(1, 10).Select(i => new Album
        {
            AlbumId = i, Title = $"Album {i}", Price = 9.99m,
            GenreId = 1, ArtistId = 1
        }));
        db.SaveChanges();
    }
}
```

### Example: StoreManagerController Test with Moq
```csharp
public class StoreManagerControllerTests
{
    private readonly Mock<MusicStoreDbContext> _mockDb;
    private readonly StoreManagerController _controller;

    [Fact]
    public async Task Create_ValidAlbum_RedirectsToIndex()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var db = new MusicStoreDbContext(options);
        var controller = new StoreManagerController(db);
        var album = new Album { Title = "Test", Price = 9.99m, GenreId = 1, ArtistId = 1 };

        // Act
        var result = await controller.Create(album);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var db = new MusicStoreDbContext(options);
        var controller = new StoreManagerController(db);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
```

### Example: Integration Test
```csharp
public class MvcMusicStoreIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MvcMusicStoreIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_HomeIndex_ReturnsSuccessAndCorrectContentType()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }

    [Fact]
    public async Task Get_StoreIndex_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Store");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_StoreManager_RequiresAuthentication()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var response = await client.GetAsync("/StoreManager");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location!.ToString());
    }
}
```

### Build Verification (Post-Migration)
After completing the migration, the team must run:
```bash
# Restore packages
dotnet restore MvcMusicStore.sln

# Build the solution (verify no compilation errors)
dotnet build MvcMusicStore.sln --configuration Release

# Run all tests
dotnet test MvcMusicStore.sln --configuration Release --logger "console;verbosity=detailed"

# Publish (verify publish succeeds)
dotnet publish MvcMusicStore/MvcMusicStore.csproj -c Release -o ./publish
```

**Expected successful output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Test run for MvcMusicStore.Tests.dll (.NETCoreApp,Version=v10.0)
Microsoft (R) Test Execution Command Line Tool Version 10.x
...
Passed! - Failed: 0, Passed: XX, Skipped: 0, Total: XX
```

---

## PART 8: COMPLETE ISSUES LIST

| ID | Severity | File | Line | Issue |
|---|---|---|---|---|
| issue-1 | critical | Global.asax.cs | 1 | System.Web.HttpApplication dependency |
| issue-2 | critical | Global.asax.cs | 7 | System.Web.Http.GlobalConfiguration |
| issue-3 | critical | Controllers/AccountController.cs | 1 | WebMatrix.WebData.WebSecurity |
| issue-4 | critical | Controllers/AccountController.cs | 5 | DotNetOpenAuth.AspNet / OAuthWebSecurity |
| issue-5 | critical | Controllers/AccountController.cs | 6 | Microsoft.Web.WebPages.OAuth |
| issue-6 | critical | Filters/InitializeSimpleMembershipAttribute.cs | 1 | WebMatrix.WebData.WebSecurity |
| issue-7 | critical | App_Start/BundleConfig.cs | 1 | System.Web.Optimization |
| issue-8 | critical | Models/MusicStoreEntities.cs | 1 | System.Data.Entity (EF5) |
| issue-9 | critical | Models/AccountModels.cs | 1 | System.Data.Entity (EF5) |
| issue-10 | critical | Models/SampleData.cs | 1 | DropCreateDatabaseIfModelChanges |
| issue-11 | critical | App_Start/WebApiConfig.cs | 1 | System.Web.Http.GlobalConfiguration |
| issue-12 | critical | Web.config | 8 | Forms Authentication (system.web) |
| issue-13 | high | Controllers/CheckoutController.cs | 22 | FormCollection parameter |
| issue-14 | high | Controllers/CheckoutController.cs | 27 | TryUpdateModel() |
| issue-15 | high | Controllers/ShoppingCartController.cs | 57 | [ChildActionOnly] CartSummary |
| issue-16 | high | Controllers/StoreController.cs | 42 | [ChildActionOnly] GenreMenu |
| issue-17 | high | Controllers/AccountController.cs | 230 | [ChildActionOnly] ExternalLoginsList |
| issue-18 | high | Controllers/AccountController.cs | 237 | [ChildActionOnly] RemoveExternalLogins |
| issue-19 | high | Views/Shared/_Layout.cshtml | 22 | @Html.Action("CartSummary") |
| issue-20 | high | Views/Shared/_Layout.cshtml | 23 | @Html.Action("GenreMenu") |
| issue-21 | high | Views/Shared/_Layout.cshtml | 7 | @Styles.Render() |
| issue-22 | high | Views/Shared/_Layout.cshtml | 8 | @Scripts.Render() |
| issue-23 | high | Views/Shared/_Layout.cshtml | 30 | @Scripts.Render() |
| issue-24 | high | Models/ShoppingCart.cs | 14 | HttpContextBase dependency |
| issue-25 | high | Controllers/StoreManagerController.cs | 1 | Synchronous DB calls |
| issue-26 | high | Controllers/HomeController.cs | 1 | Synchronous DB calls |
| issue-27 | high | Controllers/StoreController.cs | 1 | Synchronous DB calls |
| issue-28 | high | Controllers/CheckoutController.cs | 1 | Synchronous DB calls |
| issue-29 | high | Controllers/ShoppingCartController.cs | 1 | Synchronous DB calls |
| issue-30 | high | MvcMusicStore.csproj | 1 | Old-style MSBuild project format |
| issue-31 | medium | App_Start/AppConfig.cs | 1 | ConfigurationManager.AppSettings |
| issue-32 | medium | App_Start/AppConfig.cs | 8 | System.Web.Security.Roles |
| issue-33 | medium | Controllers/StoreManagerController.cs | 60 | db.Entry().State = EntityState.Modified |
| issue-34 | medium | Controllers/StoreController.cs | 28 | .Include("Albums") string include |
| issue-35 | medium | Controllers/StoreManagerController.cs | 1 | DbContext field instantiation (not DI) |
| issue-36 | medium | Controllers/CheckoutController.cs | 1 | DbContext field instantiation (not DI) |
| issue-37 | medium | Controllers/HomeController.cs | 1 | DbContext field instantiation (not DI) |
| issue-38 | medium | Controllers/ShoppingCartController.cs | 1 | DbContext field instantiation (not DI) |
| issue-39 | medium | Controllers/StoreController.cs | 1 | DbContext field instantiation (not DI) |
| issue-40 | medium | Web.config | 1 | LocalDB |DataDirectory| connection string |
| issue-41 | medium | Models/Order.cs | 4 | System.Web.Mvc.Bind attribute |
| issue-42 | medium | Views/_ViewStart.cshtml | 1 | Missing _ViewImports.cshtml |
| issue-43 | medium | packages.config | 1 | packages.config format (not PackageReference) |
| issue-44 | medium | App_Start/AuthConfig.cs | 1 | OAuthWebSecurity.Register* methods |
| issue-45 | low | packages.config | 3 | Newtonsoft.Json 4.5.6 (very old) |
| issue-46 | low | Scripts/ | 1 | jQuery 1.7.1 (outdated, security risk) |
| issue-47 | low | Scripts/ | 1 | jQuery UI 1.8.20 (outdated) |
| issue-48 | low | Scripts/ | 1 | Knockout.js 2.1.0 (outdated) |
| issue-49 | low | Scripts/ | 1 | Modernizr 2.5.3 (outdated) |
| issue-50 | low | Views/Shared/_Layout.cshtml | 15 | @Html.ActionLink() (prefer Tag Helpers) |
| issue-51 | low | Properties/AssemblyInfo.cs | 1 | AssemblyInfo.cs (not needed in SDK-style) |
| issue-52 | low | .nuget/ | 1 | Legacy NuGet.targets (not needed in SDK-style) |

---

## APPENDIX: RECOMMENDED PACKAGES FOR .NET 10

| Legacy Package | .NET 10 Replacement | NuGet Package |
|---|---|---|
| EntityFramework 5.0.0 | EF Core 10 | `Microsoft.EntityFrameworkCore.SqlServer` |
| Microsoft.AspNet.Mvc 4.0 | Built-in | (included in SDK) |
| Microsoft.AspNet.Web.Optimization 1.0 | WebOptimizer | `LigerShark.WebOptimizer.Core` |
| Microsoft.AspNet.WebApi 4.0 | Built-in | (included in SDK) |
| WebMatrix.WebData 2.0 | ASP.NET Core Identity | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| DotNetOpenAuth.AspNet 4.0 | Core Auth | `Microsoft.AspNetCore.Authentication.Google` etc. |
| Microsoft.Web.WebPages.OAuth 2.0 | Core Auth | `Microsoft.AspNetCore.Authentication.OAuth` |
| Newtonsoft.Json 4.5.6 | System.Text.Json (built-in) or `Newtonsoft.Json 13.0.3` | |
| jQuery 1.7.1 | jQuery 3.7.x | Via LibMan |
| jQuery UI 1.8.20 | jQuery UI 1.13.x | Via LibMan |
| Modernizr 2.5.3 | Modernizr 3.x | Via LibMan |
| knockoutjs 2.1.0 | Knockout 3.5.x | Via LibMan |
