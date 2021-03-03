#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src
COPY ["Ids/Ids.csproj", "Ids/"]
RUN dotnet restore "Ids/Ids.csproj"
COPY . .
WORKDIR "/src/Ids"
RUN dotnet build "Ids.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ids.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "Asia/shanghai" > /etc/timezone
#RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "Ids.dll"]