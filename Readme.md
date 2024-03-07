dotnet clean

dotnet restore

dotnet build

dotnet run

dotnet tool install --global dotnet-ef --version 3.*

dotnet ef migrations add InitialCreate

dotnet ef database update

dotnet tool install --global dotnet-ef

dotnet watch run --launch-profile https

     "DefaultConnection": "Server=ATTA\\SQLEXPRESS;Database=fernfers_babymedic;Integrated Security=True;TrustServerCertificate=True;"