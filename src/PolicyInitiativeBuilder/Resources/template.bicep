﻿@description('The Id of the Azure Management Group where custom Azure Policies are defined.')
param customPolicyMGScope string

targetScope = 'managementGroup'

var mgScope = tenantResourceId('Microsoft.Management/managementGroups', customPolicyMGScope)

resource [name] 'Microsoft.Authorization/policySetDefinitions@2021-06-01' = {
  name: '[NAME]'
  properties: {
    description: ''
    policyType: 'Custom'
    policyDefinitions: [
    ]
  }
}