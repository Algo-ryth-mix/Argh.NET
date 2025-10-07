Write-Host "Testing Argh.NET options with boolean values..."
Write-Host ""

Write-Host "Test 1: test command with --parallel flag"
& ".\Argh.Sample\bin\Debug\net8.0\Argh.Sample.exe" test ./tests --parallel
Write-Host ""

Write-Host "Test 2: test command with -p alias"
& ".\Argh.Sample\bin\Debug\net8.0\Argh.Sample.exe" test ./tests -p
Write-Host ""

Write-Host "Test 3: build command with --verbose flag"
& ".\Argh.Sample\bin\Debug\net8.0\Argh.Sample.exe" build ./solution --verbose
Write-Host ""

Write-Host "Test 4: deploy command with --dry-run flag"
& ".\Argh.Sample\bin\Debug\net8.0\Argh.Sample.exe" deploy production --dry-run
Write-Host ""

Write-Host "All tests completed!"
