FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:0cbece2e210d064199e7ab3bb9ac695c5ed8bfb35b31eee8d80e051abfbef22b AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:cec7519c080fff418bcce2e07c44141e4eba866f9801e2b35ef7567f487b207b
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]