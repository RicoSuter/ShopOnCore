FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR .
COPY ShopOnCore.Web/ShopOnCore.Web.csproj ShopOnCore.Web/
RUN dotnet restore ShopOnCore.Web/ShopOnCore.Web.csproj
COPY . .
WORKDIR /ShopOnCore.Web
RUN dotnet build ShopOnCore.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ShopOnCore.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShopOnCore.Web.dll"]
