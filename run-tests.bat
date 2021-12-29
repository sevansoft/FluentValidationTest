rd "%CD%\BuildReports" /S /Q

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /t:"Clean" /p:platform="any cpu" /p:configuration="Release"

msbuild.exe "FluentValidationTest.sln" /nologo /nr:false /p:platform="any cpu" /p:configuration="Release"

dotnet tool update dotnet-reportgenerator-globaltool --tool-path tools --version 5.0.2 --configfile nuget-officialonly.config -v n

dotnet test ".\tests\FluentValidationTest.Tests\FluentValidationTest.Tests.csproj" --no-build --configuration Release --collect:"xplat code coverage" --results-directory ./BuildReports/UnitTests/FluentValidationTest/net5.0 /p:CollectCoverage=true /p:CoverletOutput=..\..\BuildReports\Coverage\FluentValidationTest\net5.0\ /p:CoverletOutputFormat=cobertura /p:Exclude=\"[xunit.*]*"

tools\reportgenerator "-reports:BuildReports\Coverage\FluentValidationTest\net5.0\coverage.cobertura.xml" "-targetdir:BuildReports\Coverage\FluentValidationTest\net5.0" -reporttypes:HtmlInline_AzurePipelines;Cobertura; -assemblyfilters:-xunit*.* -verbosity:Verbose

start BuildReports\Coverage\FluentValidationTest\net5.0\index.htm