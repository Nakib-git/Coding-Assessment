# Getting Started Guide

> **ERP Users — ASP.NET Core 8 + Angular 18 + PostgreSQL**  
> This guide takes you from a fresh `git clone` to a fully running application.  
> **Backend** → Visual Studio 2022 &nbsp;|&nbsp; **Frontend** → VS Code

---

## Project Layout

```
erp-users/                          ← repository root
│
├── backend/                        ← Open in Visual Studio 2022
│   ├── ErpUsers.sln                  ← VS 2022 solution file  ◄  open this
│   └── src/
│       ├── ErpUsers.Domain/          ← Entities + Repository interfaces
│       ├── ErpUsers.Application/     ← DTOs + IUserService + UserService
│       ├── ErpUsers.Infrastructure/  ← EF Core DbContext + ADO.NET Repository
│       └── ErpUsers.API/             ← Controllers + Program.cs + appsettings
│           ├── appsettings.json              ← committed (no secrets)
│           └── appsettings.Development.json  ← YOU create this (gitignored!)
│
├── frontend/                       ← Open in VS Code
│   ├── .vscode/                      ← Pre-configured launch, tasks, settings
│   ├── src/app/
│   │   ├── core/models/              ← TypeScript interfaces
│   │   ├── core/services/            ← UserService (RxJS BehaviorSubject)
│   │   └── features/users/
│   │       ├── user-list/            ← Paginated table with search/filter
│   │       └── user-form/            ← Create & Edit form
│   ├── angular.json
│   └── package.json
│
├── database/
│   ├── init.sql                    ← Table DDL
│   ├── indexes.sql                 ← GIN trigram + composite indexes
│   └── seed.sql                    ← 10 sample rows (optional)
│
├── .gitignore
├── README.md
```

---

## Prerequisites

| Tool | Version | Download / Install |
|------|---------|-------------------|
| Visual Studio 2022 | **17.8+** | https://visualstudio.microsoft.com (include *ASP.NET and web development* workload) |
| .NET SDK | **8.0+** | https://dotnet.microsoft.com/download |
| VS Code | latest | https://code.visualstudio.com |
| Node.js | **18 LTS+** | https://nodejs.org |
| Angular CLI | **18** | `npm install -g @angular/cli` |
| EF Core CLI tools | latest | `dotnet tool install --global dotnet-ef` |
| PostgreSQL | **18** | https://www.postgresql.org/download |

Verify in a terminal:

```powershell
dotnet --version        # 8.x.x
node --version          # 18+
ng version              # Angular CLI 18
dotnet ef --version     # 8.x.x
psql --version          # 18
```

---

## Step 1 — Clone the Repository

```bash
git clone https://github.com/Nakib-git/Coding-Assessment.git
cd Coding-Assessment
```

---

## Step 2 — Create the PostgreSQL Database

Open **pgAdmin**, **DBeaver**, or a `psql` terminal and run:

```sql
CREATE DATABASE erp_users_db;
```

Then apply the schema:

```bash
# run from the repository root
psql -U postgres -d erp_users_db -f database/init.sql
psql -U postgres -d erp_users_db -f database/indexes.sql

# optional — inserts 10 sample users
psql -U postgres -d erp_users_db -f database/seed.sql
```

> **Alternative — EF Core migrations (recommended for team projects)**
> ```powershell
> cd backend
> dotnet ef migrations add InitialCreate `
>   --project src/ErpUsers.Infrastructure `
>   --startup-project src/ErpUsers.API
>
> dotnet ef database update `
>   --project src/ErpUsers.Infrastructure `
>   --startup-project src/ErpUsers.API
> ```
> This is equivalent to `init.sql` but lets EF Core own future schema changes via migrations.

---

## Step 3 — Configure the Backend Connection String

`appsettings.Development.json` is **gitignored** — you must create it manually.

```
backend/src/ErpUsers.API/appsettings.Development.json
```

Create the file with this content (replace the password):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=erp_users_db;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

> This file is gitignored on purpose — **never commit passwords** to source control.

---

## Step 4 — Open & Run the Backend in Visual Studio 2022

1. Open **Visual Studio 2022**
2. **File → Open → Project/Solution**
3. Navigate to `backend/` and open **`ErpUsers.sln`**
4. In Solution Explorer you will see the four projects nested under `src/`

**Restore NuGet packages** (Visual Studio does this automatically on open, or):
```
Tools → NuGet Package Manager → Restore NuGet Packages
```

**Run the API:**
- Set **ErpUsers.API** as the startup project (right-click → *Set as Startup Project*)
- Press **F5** (Debug) or **Ctrl+F5** (without debugger)
- The API starts at `http://localhost:5000`
- **Swagger UI** opens automatically at `http://localhost:5000/swagger`

> **CLI alternative** (if you prefer the terminal):
> ```powershell
> cd backend
> dotnet run --project src/ErpUsers.API
> ```

---

## Step 5 — Open & Run the Frontend in VS Code

1. Open **VS Code**
2. **File → Open Folder** → select the **`frontend/`** folder
3. VS Code will suggest installing the recommended extensions — click **Install All**

**Install npm dependencies** (only needed once after cloning):
```bash
npm install
```

**Start the dev server:**
```bash
npm start
```
Angular serves at **http://localhost:4200**
