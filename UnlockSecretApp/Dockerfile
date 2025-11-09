# 1. Стадия сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект в контейнер
COPY . .

# Публикуем проект в Release
RUN dotnet publish UnlockSecretApp/UnlockSecretApp.csproj -c Release -o /app

# 2. Стадия runtime (чистое окружение для запуска)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копируем собранное приложение из стадии build
COPY --from=build /app .

# Порт, на котором будет работать приложение
EXPOSE 5000

# Команда запуска приложения
ENTRYPOINT ["dotnet", "UnlockSecretApp.dll"]
