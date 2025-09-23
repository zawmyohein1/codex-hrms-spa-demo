# HRMS.SPA Demo

This project is a **demo ASP.NET Core 8 MVC Single Page Application (SPA)** using **Entity Framework Core (Code First)**.  
It provides basic CRUD operations for **Departments**, **Occupations**, and **Employees**, with a modern Bootstrap-based UI.

---

## 📌 Features

- **Department Setup** – CRUD with SPA-style modals  
- **Occupation Setup** – CRUD with SPA-style modals  
- **Employees Setup** – Full CRUD with:
  - Search by EmpNo / EmpName
  - Filter by Department, Occupation
  - Paging (default 20, options 10/20/50)
  - Photo upload (saved under `wwwroot/uploads`)
  - Bootstrap Icons for Add / View / Edit / Delete

-----------------------------------------------------------------------------

## 🛠️ Development Flow

### ✅ PR1 – Project Skeleton + Departments/Occupations CRUD

Work against repository `zawmyohein1/codex-hrms-spa-demo` on branch `main`.

Create a pull request titled:
PR1 – Project skeleton + Departments & Occupations CRUD (EF Core Code First)

Safety rules:
- Do NOT run restore/build/test; only create/modify text source files.
- Respect .gitignore. Exclude bin/, obj/, .vs/, *.dll, *.exe, *.pdb, *.mdf, *.ldf, *.zip.
-  No external file hunts (ignore AGENTS.md).

Scope:
1) Create solution and project
   - Solution name: hrms.spa
   - ASP.NET Core 8 MVC project: HRMS.UI
   - Folders:
     Controllers/, Views/, Models/EntityModels/, Models/ViewModels/, DataAccess/, wwwroot/

2) EF Core setup
   - Add `appsettings.json` with placeholder SQL Server connection string (username/password).
   - Install EF Core packages: Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools
   - Create `AppDbContext` with DbSets: Departments, Occupations.
   - Entities:
     - Department { Id (int, identity, PK), Name (nvarchar(200), required, unique) }
     - Occupation { Id (int, identity, PK), Name (nvarchar(200), required, unique) }

3) UI
   - `_Layout.cshtml` with Bootstrap 5 + Bootstrap Icons.
   - Navbar with icons:
     - Home (bi-house-fill)
     - Employees (bi-people-fill) → placeholder only in PR1
     - Setup (bi-gear-fill) dropdown → Departments (bi-diagram-3-fill), Occupations (bi-briefcase-fill)
   - Add `_ViewImports.cshtml` and `_ViewStart.cshtml`.

4) CRUD
   - DepartmentsController + Views (Index + partials: _List, _CreateEdit, _Delete).
   - OccupationsController + Views (same as Departments).
   - SPA-style CRUD using AJAX partials.
   - Action buttons must use **icons only**:
     - Add (bi-plus-circle), Edit (bi-pencil-square), Delete (bi-trash).

-----------------------------------

## 📦 EF Core – Migration Commands

Run in **Package Manager Console** (PMC), with **Default Project = HRMS.UI**:

# Add initial migration (PR1)
Add-Migration InitialCreate -Context AppDbContext
Update-Database -Context AppDbContext

# Add Employees migration (PR2)
Add-Migration AddEmployees -Context AppDbContext
Update-Database -Context AppDbContext

---

### ✅ PR2 – Employees CRUD + Filter

Work against repository `zawmyohein1/codex-hrms-spa-demo` on branch `main`.

Create a pull request titled:
PR2 – Employees CRUD (EF Core Code First, SPA-style)

Safety rules:
- Do NOT run restore/build/test; only create/modify text source files.
- Respect .gitignore. Exclude bin/, obj/, .vs/, *.dll, *.exe, *.pdb, *.mdf, *.ldf, *.zip.
- Scope is ONLY Employees module. Do not touch unrelated features.

Scope:
1) Entity Model (Models/EntityModels/Employee.cs)
   - Employee {
       Id (int, identity, PK)
       EmpNo (nvarchar(50), required, unique)
       EmpName (nvarchar(200), required)
       DepartmentId (int, FK → Departments.Id, required)
       OccupationId (int, FK → Occupations.Id, required)
       JobTitle (nvarchar(100), nullable)
       Gender (nvarchar(10), nullable)
       HiredDate (date, required)
       ResignDate (date, nullable)
       DateOfBirth (date, nullable)
       Phone (nvarchar(50), nullable)
       Email (nvarchar(256), nullable, unique when present)
       Address (nvarchar(max), nullable)
       PhotoUrl (nvarchar(500), nullable)
     }
   - Navigation: Department, Occupation (single). Do NOT add ICollection in Department/Occupation.

2) DbContext update
   - Add DbSet<Employee> to AppDbContext.
   - Configure relationships:
     Employee → Department (restrict delete)
     Employee → Occupation (restrict delete)

3) EmployeesController (Controllers/EmployeesController.cs)
   - Index (page shell with search/filter controls, Add button, grid container)
   - List partial (grid with AJAX; supports search by EmpNo/EmpName; filter by Department, Occupation; paging 10/20/50 default 20)
   - Create GET partial + POST (AJAX JSON response; file upload for PhotoUrl, store filename only)
   - Edit GET partial + POST (same as Create)
   - Details GET partial (modal; show all fields + larger photo)
   - Delete GET confirm partial + POST (AJAX JSON response)

4) Views (Views/Employees/)
   - Index.cshtml
   - Partials: _List.cshtml, _CreateEdit.cshtml, _Details.cshtml, _Delete.cshtml
   - Bootstrap table: columns → Photo (thumbnail, default bi-person-circle if empty), EmpNo, EmpName, Department, Occupation, JobTitle, Gender, HiredDate, Status (Active if ResignDate null, else Resigned)
   - Action buttons (icons only with tooltips):
     Add (bi-plus-circle), View (bi-eye), Edit (bi-pencil-square), Delete (bi-trash)

5) Photo Upload
   - Accept file upload in Create/Edit forms.
   - Save uploaded file into `wwwroot/uploads`.
   - Store relative path in Employee.PhotoUrl.
   - Display thumbnail in grid and larger photo in Details modal.

6) UI/UX
   - Keep SPA-style: grid loads via AJAX, modals for create/edit/details/delete.
   - Use Bootstrap alert area for success/error after AJAX.
   - Use icons only for all actions (no text buttons).

Acceptance:
- Solution builds.
- Navbar → Employees opens Employees CRUD page.
- Employees grid supports search (EmpNo/EmpName), filter (Department, Occupation), paging.
- Create/Edit/Delete via modals with AJAX.
- Photo upload works (stores file path, displays thumbnail).
- All actions use Bootstrap icons only.

Razor-safe implementation notes:
Do not put C# inline in option attributes → use asp-items or @if blocks
Rename loop variable to pageNo to avoid Razor @page directive conflict
Don’t nest @{} blocks

