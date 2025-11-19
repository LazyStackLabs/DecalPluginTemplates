#!/bin/bash
# Build script for DragonMoonNavRecorder
# This script attempts to build the plugin using available tools

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="$PROJECT_DIR/bin/Release"

echo "Building DragonMoonNavRecorder..."
echo "Project directory: $PROJECT_DIR"

# Check for dotnet CLI
if command -v dotnet &> /dev/null; then
    echo "Using dotnet CLI..."
    cd "$PROJECT_DIR"
    dotnet restore
    dotnet build -c Release -f net462
    echo "Build complete! Output: $OUTPUT_DIR/DragonMoonNavRecorder.dll"
    exit 0
fi

# Check for MSBuild
if command -v msbuild &> /dev/null; then
    echo "Using MSBuild..."
    cd "$PROJECT_DIR"
    msbuild DragonMoonNavRecorder.csproj /p:Configuration=Release /p:TargetFramework=net462
    echo "Build complete! Output: $OUTPUT_DIR/DragonMoonNavRecorder.dll"
    exit 0
fi

# Check for Mono mcs
if command -v mcs &> /dev/null; then
    echo "Using Mono mcs compiler..."
    cd "$PROJECT_DIR"
    mkdir -p "$OUTPUT_DIR"
    
    # Compile with mcs (Mono C# compiler)
    mcs -target:library \
        -out:"$OUTPUT_DIR/DragonMoonNavRecorder.dll" \
        -define:TRACE,VVS_REFERENCED \
        -reference:System.dll,System.Core.dll,System.Drawing.dll,System.Xml.dll,System.Windows.Forms.dll \
        Globals.cs PluginCore.cs NavRecorder.cs Properties/AssemblyInfo.cs Util.cs \
        VirindiViews/*.cs \
        -resource:mainView.xml,DragonMoonNavRecorder.mainView.xml \
        -optimize
    
    echo "Build complete! Output: $OUTPUT_DIR/DragonMoonNavRecorder.dll"
    exit 0
fi

echo "ERROR: No suitable build tool found!"
echo "Please install one of:"
echo "  - .NET SDK (dotnet)"
echo "  - MSBuild"
echo "  - Mono (mcs)"
exit 1
