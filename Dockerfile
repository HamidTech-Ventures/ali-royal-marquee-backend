FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["src/AliRoyalMarquee.API/AliRoyalMarquee.API.csproj", "src/AliRoyalMarquee.API/"]
COPY ["src/AliRoyalMarquee.Application/AliRoyalMarquee.Application.csproj", "src/AliRoyalMarquee.Application/"]
COPY ["src/AliRoyalMarquee.Domain/AliRoyalMarquee.Domain.csproj", "src/AliRoyalMarquee.Domain/"]
COPY ["src/AliRoyalMarquee.Infrastructure/AliRoyalMarquee.Infrastructure.csproj", "src/AliRoyalMarquee.Infrastructure/"]
COPY ["src/AliRoyalMarquee.Shared/AliRoyalMarquee.Shared.csproj", "src/AliRoyalMarquee.Shared/"]

# Restore dependencies
RUN dotnet restore "src/AliRoyalMarquee.API/AliRoyalMarquee.API.csproj"

# Copy everything else
COPY . .

# Build and Publish
WORKDIR "/src/src/AliRoyalMarquee.API"
RUN dotnet publish "AliRoyalMarquee.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AliRoyalMarquee.API.dll"]
