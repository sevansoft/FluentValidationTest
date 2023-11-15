rd "%CD%\BuildReports" /S /Q

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /t:"Clean" /p:platform="any cpu" /p:configuration="Release"

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /p:platform="any cpu" /p:configuration="Release"

dotnet tool restore -v d

dotnet test ".\tests\FluentValidationTest.Tests\FluentValidationTest.Tests.csproj" --no-build --configuration Release --collect:"xplat code coverage" --results-directory ./BuildReports/UnitTests/FluentValidationTest/net8.0

reportgenerator "-reports:BuildReports\UnitTests\**\coverage.cobertura.xml" "-targetdir:BuildReports\Coverage\FluentValidationTest\net8.0" -reporttypes:HtmlInline_AzurePipelines;Cobertura; -assemblyfilters:-xunit*.* -verbosity:Verbose

start BuildReports\Coverage\FluentValidationTest\net8.0\index.htm