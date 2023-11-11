az login --tenant df6b18d9-22de-4a09-841c-49b4f8fe9cfe
az acr login --name studioworks
az acr update -n studioworks --admin-enabled true
docker build -t microservice-document-scanner -f Dockerfile .
docker tag microservice-document-scanner studioworks.azurecr.io/microservice-document-scanner
docker push studioworks.azurecr.io/microservice-document-scanner
