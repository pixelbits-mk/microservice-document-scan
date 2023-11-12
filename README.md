# Build and push docker image to Azure Container Registry

az login --tenant df6b18d9-22de-4a09-841c-49b4f8fe9cfe
az acr login --name studioworks
az acr update -n studioworks --admin-enabled true
docker build -t microservice-document-scanner -f Dockerfile .
docker tag microservice-document-scanner studioworks.azurecr.io/microservice-document-scanner
docker push studioworks.azurecr.io/microservice-document-scanner

# Setup Build Pipeline
1. Assign Managed Identity to ACR (system assigned)
2. In ACR IAM, assign AcrPull role to Managed Identity
3. In DevOps Artifacts, create a new feed as a source for Nuget packages. Create a nuget.config file in sln root directory.
4. In Project settings, create a new Service Connection to your ACR.
   - Authentication Type = Managed Service Identity
   - Subscription ID = az account show --query id --output table
   - Tenant ID = az account show --query tenantId --output table
   - ACR Login Server = az acr show --name studioworks --query loginServer --output table
5. Update build.pipeline.yaml
- task: DotNetCoreCLI@2
  displayName: 'Restore NuGet Packages'
  inputs:
    command: 'restore'
    projects: 'AvScanner.Api/AvScanner.Api.csproj'
    feedsToUse: 'select'
    vstsFeed: 'kangmike'