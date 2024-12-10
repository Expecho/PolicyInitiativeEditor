param userAssignedIdentityId string
param userAssignedIdentityName string

var acrPullRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')

resource myAcrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  scope: resourceGroup()
  name: guid(resourceGroup().id, userAssignedIdentityName, acrPullRoleDefinitionId)
  properties:{
    
    roleDefinitionId: acrPullRoleDefinitionId
    principalId: userAssignedIdentityId
    principalType: 'ServicePrincipal'
  }
}
