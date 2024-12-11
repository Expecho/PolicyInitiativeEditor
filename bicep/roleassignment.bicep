param userAssignedIdentityId string
param userAssignedIdentityName string

var acrPullRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
var keyVaultSecretUserRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  scope: resourceGroup()
  name: guid(resourceGroup().id, userAssignedIdentityName, acrPullRoleDefinitionId)
  properties:{
    
    roleDefinitionId: acrPullRoleDefinitionId
    principalId: userAssignedIdentityId
    principalType: 'ServicePrincipal'
  }
}

resource keyVaultSecretUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: resourceGroup()
  name: guid(resourceGroup().id, userAssignedIdentityName, keyVaultSecretUserRoleDefinitionId)
  properties: {
    roleDefinitionId: keyVaultSecretUserRoleDefinitionId
    principalId: userAssignedIdentityId
    principalType: 'ServicePrincipal'
  }
}
