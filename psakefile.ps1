properties {
    $configuration = 'Release'
    $artifactDir = '.\artifacts'
}

task default -depends Compile

task Complete -depends Clean, Compile, UnitTests, IntegrationTests, Pack, Publish

task Clean {
    exec {
        dotnet clean .\src\SimpleUptime.sln -c $configuration
    }
}

task Compile {
    exec {
        dotnet build .\src\SimpleUptime.sln -c $configuration /m --no-restore
    }
}

Task UnitTests {
    exec {
        dotnet test .\src\SimpleUptime.UnitTests\SimpleUptime.UnitTests.csproj -c $configuration --no-build --no-restore
    }
}

Task IntegrationTests {
    exec {
        dotnet test .\src\SimpleUptime.IntegrationTests\SimpleUptime.IntegrationTests.csproj -c $configuration --no-build --no-restore
    }
}

Task Pack {
    $source = ".\src\SimpleUptime.ResourceGroup\bin\$configuration"
    $destination = "$artifactDir\SimpleUptime.ResourceGroup"
    
    "copy $source to $destination"

    Remove-Item $destination -Force -Recurse -ErrorAction Ignore
    Copy-Item -Path $source -Recurse -Destination $destination -Force -Container

    "copy azure function"
    exec {
        dotnet publish .\src\SimpleUptime.FuncApp -o C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp --no-restore
    }

    ZipAzureFunction C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp.zip
}

Task Authenticate {
    $profile = (Resolve-Path $artifactDir).ToString() + "\AzureRmProfile.json"

    if(Test-Path $profile) {
        Select-AzureRmProfile -Path $profile
    } else {
        Login-AzureRmAccount
        Save-AzureRmProfile -Path $profile
    }
}

Task Publish-Function -depends Authenticate {
    # https://markheath.net/post/deploy-azure-functions-kudu-powershell
    # https://dscottraynsford.wordpress.com/2017/07/12/publish-an-azure-rm-web-app-using-a-service-principal-in-powershell/
    $resourceGroupName = "simpleuptime-uat-rg"
    $functionAppName = "simpleuptime-uat-func"
    $creds = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroupName -ResourceType Microsoft.Web/sites/config `
                -ResourceName $functionAppName/publishingcredentials -Action list -ApiVersion 2015-08-01 -Force
    
    $username = $creds.Properties.PublishingUserName
    $password = $creds.Properties.PublishingPassword

    DeployAzureFunction $username $password $functionAppName "C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp.zip"
}

Task Publish-ResourceGroup -depends Authenticate {
    $resourceGroupLocation = "northcentralus"
    $resourceGroupName = "simpleuptime-uat-rg"
    $templateFile = "DocumentDB.json"
    $templateParametersFile = "DocumentDB.parameters.json"

    .\artifacts\SimpleUptime.ResourceGroup\Deploy-AzureResourceGroup.ps1 `
        -ResourceGroupLocation $resourceGroupLocation `
        -ResourceGroupName $resourceGroupName `
        -TemplateFile $templateFile `
        -TemplateParametersFile $templateParametersFile
}

Task Publish -depends Publish-ResourceGroup, Publish-Function

Function ZipAzureFunction(
    [Parameter(Mandatory = $true)]
    [String]$functionPath,
    [Parameter(Mandatory = $true)]
    [String]$outputPath
)
{
  $excluded = @(".vscode", ".gitignore", "appsettings.json", "secrets")
  $include = Get-ChildItem $functionPath -Exclude $excluded
  Compress-Archive -Path $include -Update -DestinationPath $outputPath
}

Function DeployAzureFunction(
    [Parameter(Mandatory = $true)]
    [String]$username,
    [Parameter(Mandatory = $true)]
    [String]$password,
    [Parameter(Mandatory = $true)]
    [String]$functionAppName,
    [Parameter(Mandatory = $true)]
    [String]$zipFilePath    
)
{
  $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $username,$password)))
  $apiUrl = "https://$functionAppName.scm.azurewebsites.net/api/zip/site/wwwroot"
  Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method PUT -InFile $zipFilePath -ContentType "multipart/form-data"
}