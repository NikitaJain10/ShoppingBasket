# Build and test stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet test --configuration Release --no-restore
RUN dotnet publish src/ShoppingBasket.App -c Release -o /app --no-restore

# Small runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ShoppingBasket.App.dll"]
