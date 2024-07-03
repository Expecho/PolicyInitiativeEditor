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
