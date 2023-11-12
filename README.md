# Build and push docker image to Azure Container Registry

az login --tenant df6b18d9-22de-4a09-841c-49b4f8fe9cfe
az acr login --name studioworks
az acr update -n studioworks --admin-enabled true
docker build -t microservice-document-scanner -f Dockerfile .
docker tag microservice-document-scanner studioworks.azurecr.io/microservice-document-scanner
docker push studioworks.azurecr.io/microservice-document-scanner

# Setup Build Pipeline
1. In DevOps Artifacts, create a new feed as a source for Nuget packages. Create a nuget.config file in sln root directory.
2. In Project settings, create a new Service Connection to your ACR.
   - Authentication Type = Service Principal
   - Login with your Azure credentials when prompted by browser
   - Make sure to check the box for "Grant access permissions to all pipelines"
5. Update build.pipeline.yaml
- task: DotNetCoreCLI@2
  displayName: 'Restore NuGet Packages'
  inputs:
    command: 'restore'
    projects: 'AvScanner.Api/AvScanner.Api.csproj'
    feedsToUse: 'select'
    vstsFeed: 'kangmike'
  
6. Get the Service Principal's Object ID:

You can find the Object ID of your Service Principal using the Azure CLI or Azure Portal. Look for App Registration for your app service.
1a3ca2c8-33c1-49f1-b59a-ddd1022da5d9

7. Setup a Nuget Service Connection in DevOps
https://pkgs.dev.azure.com/kangmike/AvScanner/_packaging/kangmike/nuget/v3/index.json
Create a PAT with Packaging Read and Packaging Write permissions in Azure DevOps

