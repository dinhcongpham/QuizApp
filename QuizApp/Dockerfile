# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["QuizApp.API/QuizApp.API.csproj", "QuizApp.API/"]
COPY ["QuizApp.Core/QuizApp.Core.csproj", "QuizApp.Core/"]
COPY ["QuizApp.Shared/QuizApp.Shared.csproj", "QuizApp.Shared/"]
RUN dotnet restore "QuizApp.API/QuizApp.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/QuizApp.API"
RUN dotnet build "QuizApp.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "QuizApp.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "QuizApp.API.dll"] 