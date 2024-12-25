# Policy Initiative Bicep Builder

This app allows you to visually select one or more policies and generates or updates a bicep file to deploy a policy initiative.

Accessible at https://policyinitiativebuilder.yellowbay-9c92f1d3.westeurope.azurecontainerapps.io/

![image](https://github.com/user-attachments/assets/4ed81f73-4b5d-445e-9127-ef6fe09a3de0)

![image](https://github.com/user-attachments/assets/e3d97066-d80a-40e6-aadd-147e9a306548)

## Tools & Resources

- [Radzen Blazor Components](https://blazor.radzen.com/docs/index.html)
- [Azure Policy Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=AzurePolicy.azurepolicyextension)

## Resource Graph Explorer Queries

### Get a list of all exemptions

```sql
policyresources
| where type == "microsoft.authorization/policyexemptions"
| project
  id,
  name,
  tenantId,
  resourceGroup,
  subscriptionId,
  policyAssignmentId = properties.policyAssignmentId,
  displayName = properties.displayName,
  description = properties.description,
  category = properties.exemptionCategory,
  properties
```

### Get a list of all policies

```sql
PolicyResources
| extend subscriptionId = tostring(properties.subscriptionId)
| join kind=leftouter (
    ResourceContainers 
    | where type=='microsoft.resources/subscriptions'
    | project subscriptionName=name, subscriptionId) on subscriptionId
| where type =~ 'Microsoft.PolicyInsights/PolicyStates'
| project 
    policyAssignmentName = properties.policyAssignmentName,
    policy = properties.policyDefinitionReferenceId, 
    state = properties.complianceState, 
    effect = properties.policyDefinitionAction,
    subscriptionId,
    subscriptionName,
    resourceGroup = properties.resourceGroup, 
    resourceType = properties.resourceType,
    resourceId = properties.resourceId,
    properties
| order by tostring(policyAssignmentName) desc, tostring(['state']) asc
```
