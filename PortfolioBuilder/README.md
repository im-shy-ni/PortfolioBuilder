
Portfolio Builder - ASP.NET Core MVC (skeleton with extras)
Framework: .NET 7
Database: SQLite (EF Core)
Authentication: ASP.NET Identity (email confirmation & password reset via Emails folder)
Design: Bootstrap 5 + modern theme, dark mode toggle

New features added:
- Edit/Delete for projects and skills
- Profile picture upload (saved to wwwroot/uploads)
- Modern card-based theme and dark/light toggle
- Email confirmation and password reset flows. Emails are written to the Emails folder (simple demo email sender).
- Demo user: username 'demo' password 'Demo@1234' (email already confirmed)

How to run:
1. Install .NET SDK 7 (or compatible).
2. Open this folder in Visual Studio or VS Code.
3. From terminal: dotnet restore
4. Run the app with: dotnet run
   The app will create the SQLite DB automatically and seed demo data.
5. Check Emails folder at the project root to view confirmation/reset links (if using new users).
6. Public portfolio view: /Portfolio/View/demo
