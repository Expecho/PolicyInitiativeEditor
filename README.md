# Policy Initiative Bicep Builder

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
