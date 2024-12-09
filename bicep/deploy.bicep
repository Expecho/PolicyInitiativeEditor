param location string = resourceGroup().location

param workloadProfileType string = 'consumption'
param workloadProfileMinimumCount int = 0
param workloadProfileMaximumCount int = 1
param workloadProfileName string = 'Consumption'

param cappName string = 'policyinitativebuilder'
param cappConsumptionCpu string = '0.5'
param cappConsumptionMemory string = '1'
param cappImageName string = 'expechocontainerregistry.azurecr.io/policyinitativebuilder:latest'
param cappImageServer string = 'expechocontainerregistry.azurecr.io'

param vnnetName string = 'vnet-policyinitativebuilder'
param subnetName string = 'subnet-policyinitativebuilder'

param appInsightsName string = 'policyinitativebuilder-insights'
param laWorkspaceName string = 'policyinitativebuilderlogwsexpecho'

resource vnet 'Microsoft.Network/virtualNetworks@2022-07-01' = {
  name: vnnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          delegations: [
            {
              name: ' MicrosoftAppEnvironments'
              properties: {
                serviceName: 'Microsoft.App/environments'
              }
            }
          ]
          addressPrefix: '10.0.0.0/27'
        }
      }
    ]
  }
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-08-02-preview' = {
  name: 'PolicyInitativeBuilder'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: laWorkspace.properties.customerId
        sharedKey: laWorkspace.listKeys().primarySharedKey
      }
    }
    vnetConfiguration: {
      infrastructureSubnetId: vnet.properties.subnets[0].id
    }
    workloadProfiles: [
      { 
        name: workloadProfileName
        workloadProfileType: workloadProfileType
        minimumCount: workloadProfileMinimumCount
        maximumCount: workloadProfileMaximumCount
      }
    ]
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  kind: 'web'
  location: location
  name: appInsightsName
  properties:{
    Application_Type: 'web'
    WorkspaceResourceId: laWorkspace.id
    IngestionMode: 'LogAnalytics'
  }
}

resource laWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' ={
  location: location
  name: laWorkspaceName
  properties:{
    retentionInDays: 90
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource containerAppInConsumptionWorkloadProfile 'Microsoft.App/containerApps@2024-08-02-preview' = {
  name: cappName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        allowInsecure: false
        external: true
        targetPort: 80
      }
      registries: [
        {
          identity: 'system'
          server: cappImageServer
        }
      ]
    }
    managedEnvironmentId: containerAppEnv.id
    template: {
      containers: [
        {
          image: cappImageName
          resources: {
            cpu: json('${cappConsumptionCpu}')
            memory: '${cappConsumptionMemory}Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
    workloadProfileName: workloadProfileName
  }
}
