param (
	[Parameter(Mandatory=$true)] [string] $subscriptionId,
	[Parameter(Mandatory=$true)] [string] $location,
	[Parameter(Mandatory=$true)] [string] $resourceGroupName,
	[Parameter(Mandatory=$true)] [string] $storageAccountName,
	[Parameter(Mandatory=$true)] [string] $storageContainerName,
	[Parameter(Mandatory=$true)] [string] $keyVaultName,
	[Parameter(Mandatory=$true)] [string] $adminGroupObjectId,
	[Parameter(Mandatory=$true)] [string[]] $servicePrincipalObjectIds
)

Select-AzSubscription $subscriptionId

New-AzResourceGroup -ResourceGroupName $resourceGroupName `
					-Location $location

New-AzStorageAccount -ResourceGroupName $resourceGroupName `
					 -Location $location `
					 -Name $storageAccountName `
					 -SkuName Standard_LRS `
					 -AccessTier Hot `
					 -Kind StorageV2

$storageContext = New-AzStorageContext -StorageAccountName $storageAccountName `
									   -UseConnectedAccount

New-AzStorageContainer -Name $storageContainerName `
					   -Permission Off `
					   -Context $storageContext

New-AzKeyVault -Name $keyVaultName `
			   -ResourceGroupName $resourceGroupName `
			   -Location $location `
			   -Sku Standard

Set-AzKeyVaultAccessPolicy -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -PermissionsToSecrets get, list, set -ObjectId $adminGroupObjectId

$servicePrincipalObjectIds | foreach-object {	
	Set-AzKeyVaultAccessPolicy -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -PermissionsToSecrets get, list -ObjectId $_
}