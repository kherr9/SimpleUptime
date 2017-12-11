properties {
    $configuration = 'Release'
}

task default -depends Compile

task Complete -depends Clean, Compile, UnitTests, IntegrationTests 

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