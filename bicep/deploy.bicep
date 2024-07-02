@allowed([ 'Free', 'Standard' ])
param sku string = 'Free'
param name string
param location string = resourceGroup().location

param repositoryToken string
param repositoryUrl string = 'https://github.com/Expecho/Policy-Initiative-Bicep-Builder'
param branch string = 'master'
param appLocation string = './PolicyInitiativeEditor/Client'

resource staticwebapp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: name 
  location: location
  sku: {
    name: sku
  }
  properties: {
    repositoryUrl: repositoryUrl
    stagingEnvironmentPolicy: 'Disabled'
    branch: branch
    buildProperties: {
      appLocation: appLocation
      appArtifactLocation: './PolicyInitiativeEditor/wwwroot'
      outputLocation:
    }
    repositoryToken: repositoryToken
  }
}
