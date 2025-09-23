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

Work against repository `zawmyohein1/codex-hrms-spa` on branch `main`.

Create a pull request titled:
PR1 – Project skeleton + Departments & Occupations CRUD (EF Core Code First)

Safety rules:
- Do NOT run restore/build/test; only create/modify text source files.
- Respect .gitignore. Exclude bin/, obj/, .vs/, *.dll, *.exe, *.pdb, *.mdf, *.ldf, *.zip.

Scope:
- Create ASP.NET Core MVC project `HRMS.UI` under solution `hrms.spa`
- Add EntityModels: Department, Occupation
- Add AppDbContext with DbSets
- Add CRUD controllers + views for Departments and Occupations
- Use SPA-style partials + AJAX for CRUD
- Navbar with icons:
  - Home (bi-house-fill)
  - Employees (bi-people-fill → placeholder in PR1)
  - Setup (bi-gear-fill) → Departments (bi-diagram-3-fill), Occupations (bi-briefcase-fill)
- Action buttons: icons only (Add, Edit, Delete)
-----------------------------------------------------------------------------------------------

### ✅ PR2 – Employees CRUD (EF Core Code First, SPA-style)

Work against repository `zawmyohein1/codex-hrms-spa` on branch `main`.

Create a pull request titled:
PR2 – Employees CRUD (EF Core Code First, SPA-style)

Safety rules:
- Do NOT run restore/build/test; only create/modify text source files.
- Respect .gitignore. Exclude bin/, obj/, .vs/, *.dll, *.exe, *.pdb, *.mdf, *.ldf, *.zip.
- Scope is ONLY Employees module.

Scope:
- Add Employee entity (Id, EmpNo, EmpName, DepartmentId, OccupationId, JobTitle, Gender,
  HiredDate, ResignDate, DateOfBirth, Phone, Email, Address, PhotoUrl)
- Update AppDbContext with DbSet<Employee> + relationships
- Add EmployeesController with SPA-style CRUD
- Views/Employees: Index + partials (_List, _CreateEdit, _Details, _Delete)
- Support search (EmpNo, EmpName), filter (Department, Occupation), paging
- Photo upload (save under wwwroot/uploads, store relative path in DB)
- Show photo thumbnail in grid (fallback bi-person-circle), large photo in Details modal
- Bootstrap icons only for actions (Add, View, Edit, Delete)
- Razor-safe implementation notes:
  - Do not put C# inline in option attributes → use `asp-items` or @if blocks
  - Rename loop variable to `pageNo` to avoid Razor `@page` directive conflict
  - Don’t nest `@{}` blocks

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

