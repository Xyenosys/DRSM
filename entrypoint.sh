#!/bin/bash

# Start ASP.NET Core application in the background
dotnet /app/DRSM.dll &

# Start Nginx in the foreground
nginx