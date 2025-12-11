# 1. Base image for running the app (Lightweight Linux)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# CRITICAL: I install the font library required by QuestPDF for Linux
RUN apt-get update && apt-get install -y libfontconfig1

# 2. SDK image for building the app (Full .NET SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# I copy the project files first to leverage Docker caching
COPY ["TalentPlus.Web/TalentPlus.Web.csproj", "TalentPlus.Web/"]
COPY ["TalentPlus.Infraestructure/TalentPlus.Infraestructure.csproj", "TalentPlus.Infraestructure/"]
COPY ["TalentPlus.Application/TalentPlus.Application.csproj", "TalentPlus.Application/"]
COPY ["TalentPlus.Domain/TalentPlus.Domain.csproj", "TalentPlus.Domain/"]

# I restore dependencies
RUN dotnet restore "TalentPlus.Web/TalentPlus.Web.csproj"

# I copy the rest of the source code
COPY . .

# I build the project
WORKDIR "/src/TalentPlus.Web"
RUN dotnet build "TalentPlus.Web.csproj" -c Release -o /app/build

# 3. I publish the app to a folder
FROM build AS publish
RUN dotnet publish "TalentPlus.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final stage: I copy the published app to the base image and run it
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentPlus.Web.dll"]