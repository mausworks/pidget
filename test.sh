#!/bin/bash

dotnet build

for f in test/*
do
   cd $f

   dotnet test --no-build

   cd ../..
done
