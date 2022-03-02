rd "%CD%\BuildReports" /S /Q

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /t:"Clean" /p:platform="any cpu" /p:configuration="Release"

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /p:platform="any cpu" /p:configuration="Release"

dotnet tool restore -v d

dotnet test ".\tests\FluentValidationTest.Tests\FluentValidationTest.Tests.csproj" --no-build --configuration Release --collect:"xplat code coverage" --results-directory ./BuildReports/UnitTests/FluentValidationTest/net6.0 /p:CollectCoverage=true /p:CoverletOutput=..\..\BuildReports\Coverage\FluentValidationTest\net6.0\ /p:CoverletOutputFormat=cobertura /p:Exclude=\"[xunit.*]*"

reportgenerator "-reports:BuildReports\Coverage\FluentValidationTest\net6.0\coverage.cobertura.xml" "-targetdir:BuildReports\Coverage\FluentValidationTest\net6.0" -reporttypes:HtmlInline_AzurePipelines;Cobertura; -assemblyfilters:-xunit*.* -verbosity:Verbose

start BuildReports\Coverage\FluentValidationTest\net6.0\index.htm