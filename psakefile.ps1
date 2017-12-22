properties {
    $configuration = 'Release'
    $artifactDir = '.\artifacts'
}

task default -depends Compile

task Complete -depends Clean, Compile, UnitTests, IntegrationTests, Pack, Publish

Task Publish -depends Publish-ResourceGroup, Publish-Function, Publish-SpaHost, Publish-WebApp

task Clean {
    exec {
        dotnet clean .\src\SimpleUptime.sln -c $configuration
    }

    Remove-Item .\src\SimpleUptime.WebApp\dist -Force -Recurse -ErrorAction Ignore

    Remove-Item "$artifactDir\SimpleUptime.*" -Force -Recurse -ErrorAction Ignore
}

task Compile {
    exec {
        dotnet build .\src\SimpleUptime.sln -c $configuration /m --no-restore
    }

    exec {
        npm run build --prefix .\src\SimpleUptime.WebApp
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

    "copy azure function - app"
    exec {
        dotnet publish .\src\SimpleUptime.FuncApp -o C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp --no-restore
    }

    ZipAzureFunction C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp C:\git\SimpleUptime\artifacts\SimpleUptime.FuncApp.zip

    "copy azure function - spa host"
    exec {
        dotnet publish .\src\SimpleUptime.SpaHost -o C:\git\SimpleUptime\artifacts\SimpleUptime.SpaHost --no-restore
    }
    
    ZipAzureFunction C:\git\SimpleUptime\artifacts\SimpleUptime.SpaHost C:\git\SimpleUptime\artifacts\SimpleUptime.SpaHost.zip

    $source = ".\src\SimpleUptime.WebApp\dist"
    $destination = "$artifactDir\SimpleUptime.WebApp"
    "copy $source to $destination"
    
    Remove-Item $destination -Force -Recurse -ErrorAction Ignore
    Copy-Item -Path $source -Recurse -Destination $destination -Force -Container
}

Task Authenticate {
    $profile = (Resolve-Path $artifactDir).ToString() + "\AzureRmProfile.json"

    if (Test-Path $profile) {
        Select-AzureRmProfile -Path $profile
    }
    else {
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

Task Publish-SpaHost -depends Authenticate {
    # https://markheath.net/post/deploy-azure-functions-kudu-powershell
    # https://dscottraynsford.wordpress.com/2017/07/12/publish-an-azure-rm-web-app-using-a-service-principal-in-powershell/
    $resourceGroupName = "simpleuptime-uat-rg"
    $functionAppName = "simpleuptime-uat-spahost"
    $creds = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroupName -ResourceType Microsoft.Web/sites/config `
        -ResourceName $functionAppName/publishingcredentials -Action list -ApiVersion 2015-08-01 -Force
    
    $username = $creds.Properties.PublishingUserName
    $password = $creds.Properties.PublishingPassword

    DeployAzureFunction $username $password $functionAppName "C:\git\SimpleUptime\artifacts\SimpleUptime.SpaHost.zip"
}

Task Publish-WebApp -depends Authenticate {
    $resourceGroupName = "simpleuptime-uat-rg"
    $storageAccountName = "simpleuptimeuatdata001"
    $containerName = "www"

    $storageAccount = Get-AzureRmStorageAccount -ResourceGroupName $resourceGroupName -Name $storageAccountName

    $ctx = $storageAccount.Context

    # create container
    $container = Get-AzureStorageContainer -Name $containerName -Context $ctx -ErrorAction Ignore
    if ($container) {
        "Container $containerName already exists"
    }
    else {
        $container = New-AzureStorageContainer -Name $containerName -Context $ctx -Permission blob
    }
    
    $dir = (Resolve-Path ".\artifacts\SimpleUptime.WebApp").ToString()

    $files = Get-ChildItem $dir -Recurse -File
    foreach ($x in $files) {
        $targetPath = ($x.fullname.Substring($dir.Length + 1)).Replace("\", "/")

        $contentType = [System.Web.MimeMapping]::GetMimeMapping($targetPath)
        $blobProperties = @{"ContentType" = $contentType};

        "Uploading $("\" + $x.fullname.Substring($dir.Length + 1)) to $($container.CloudBlobContainer.Uri.AbsoluteUri + "/" + $targetPath)"
        Set-AzureStorageBlobContent -File $x.fullname -Container $container.Name -Blob $targetPath -Context $ctx -Properties $blobProperties -Force:$Force | Out-Null
    }
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
    
    # set custom dns
    $webAppName = "simpleuptime-uat-spahost"
    $fqdn = "app-uat.simpleuptime.io"

    Set-AzureRmWebApp `
        -Name $webAppName `
        -ResourceGroupName $resourceGroupName `
        -HostNames @($fqdn, "$webappname.azurewebsites.net")
}

Function ZipAzureFunction(
    [Parameter(Mandatory = $true)]
    [String]$functionPath,
    [Parameter(Mandatory = $true)]
    [String]$outputPath
) {
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
) {
    $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $username, $password)))
    $apiUrl = "https://$functionAppName.scm.azurewebsites.net/api/zip/site/wwwroot"
    Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization = ("Basic {0}" -f $base64AuthInfo)} -Method PUT -InFile $zipFilePath -ContentType "multipart/form-data"
}