## 1. Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [MySQL Server 8.x](https://dev.mysql.com/downloads/mysql/) (or MySQL Workbench) running locally
- [Node.js 20 LTS](https://nodejs.org/) (includes npm)
- [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
- VS Code (recommended extensions: C# Dev Kit, Angular Language Service)

## Demo Credentials

The app seeds a set of demo users into `database` on first load. Use the below to log in:

| Role     | Email                          | Password      |
|----------|---------------------------------|---------------|
| admin | admin@example.com           | Admin@123   |
| user   | user@example.com             | User@1234   |

## 2. Get the project

Unzip the Bus Booking application anywhere, and open that folder in VS Code.

## 3. Configure the backend

Edit `appsettings.json` and fill in:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=busbooking;user=root;password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Secret": "REPLACE_WITH_A_LONG_RANDOM_STRING_AT_LEAST_32_CHARACTERS"
  }
}
```

`appsettings.json` is git-ignored on purpose - real secrets never get committed.

## 4. MySQL setup

Apply the migration and update the database inside the backend folder run the following command

```sql
dotnet ef database update --project BusBooking.Infrastructure --startup-project BusBooking.API
```

## 5. Restore & add the EF Core tooling

```powershell
cd backend
dotnet restore BusBooking.sln
dotnet tool install --global dotnet-ef   # skip if already installed
```

## 6. Build

```powershell
dotnet build BusBooking.sln
```

## 7. Run the backend

```powershell
cd backend\BusBooking.API
dotnet run
```

On first run, `DbSeeder` populates demo data (see `README.md` for credentials). The API listens on
`https://localhost:7080` / `http://localhost:5050` (see `Properties/launchSettings.json`) and
opens Swagger automatically at `/swagger`.

## 8. Configure & run the frontend

```powershell
cd frontend\bus-booking-ui
npm install
```

Check `src/environments/environment.ts` - `apiBaseUrl` should point at your running backend
(`http://localhost:5050/api` by default, matching step 7).

```powershell
npm start
```

This opens the app at `http://localhost:4200`.

## 9. Use the application

1. Go to `http://localhost:4200` - search Delhi -> Dehradun (works without logging in).
2. Click a bus - you'll be sent to login/register first.
3. Enter passenger details and confirm the booking - seat availability decreases.
4. Go to "My Bookings" - edit or cancel the booking; cancelling restores the seats.
5. Log out, log back in as `admin@example.com`, open "Admin" - view buses and their bookings.
