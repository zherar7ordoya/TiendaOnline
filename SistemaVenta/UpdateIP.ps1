$tenantId = "b62888b3-1b34-494e-afda-976feec150dc"
az login --tenant $tenantId

$resourceGroup = "ResourceGroup"
$serverName = "zherar7ordoya"
$ruleName = "ThisPC"
$ip = (Invoke-WebRequest -Uri "https://api.ipify.org").Content.Trim()

az sql server firewall-rule create `
  --resource-group $resourceGroup `
  --server $serverName `
  --name $ruleName `
  --start-ip-address $ip `
  --end-ip-address $ip