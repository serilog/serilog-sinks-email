#!/bin/bash

set -e 

export PATH=$HOME/.dotnetcli:$PATH

dotnet --info
dotnet --list-sdks
dotnet restore

echo "🤖 Attempting to build..."
for path in src/**/*.csproj; do
    dotnet build -f netstandard2.0 -c Release ${path}
done

echo "🤖 Running tests..."
for path in test/*.Tests/*.csproj; do
    dotnet test -f net6.0  -c Release ${path}
done

