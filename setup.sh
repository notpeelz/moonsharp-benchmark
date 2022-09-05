#!/usr/bin/env bash

set -e

rm -rf ./moonsharp-a ./moonsharp-b

git -C ./moonsharp fetch origin
git -C ./moonsharp worktree remove "$PWD/moonsharp-a" &> /dev/null
git -C ./moonsharp worktree remove "$PWD/moonsharp-b" &> /dev/null
git -C ./moonsharp worktree add "$PWD/moonsharp-a" add-clr-primitive-wrappers~1
git -C ./moonsharp worktree add "$PWD/moonsharp-b" add-clr-primitive-wrappers

rm -rf ./nuget-pkgs/cache

pack_args=(
  "-p:DebugType=pdbonly"
  "-p:DebugSymbols=true"
)
dotnet pack ./moonsharp-a/MoonSharp.Interpreter -o ./nuget-pkgs/src -p:PackageId=MoonSharpA -p:AssemblyName=MoonSharpA -c Release "${pack_args[@]}"
dotnet pack ./moonsharp-b/MoonSharp.Interpreter -o ./nuget-pkgs/src -p:PackageId=MoonSharpB -p:AssemblyName=MoonSharpB -c Release "${pack_args[@]}"
