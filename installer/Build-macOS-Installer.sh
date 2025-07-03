#!/bin/bash

# Build macOS Installer for QuickMind
VERSION="2.0.0"
CONFIGURATION="Release"
APP_NAME="QuickMind"
BUNDLE_ID="com.quickmind.app"

echo "🍎 Building QuickMind macOS Installer v$VERSION"

# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf ./publish
rm -rf ./dist/$APP_NAME-v$VERSION.dmg
rm -rf ./dist/$APP_NAME.app

# Build and publish for macOS
echo "🔨 Publishing application for macOS..."
dotnet publish -c $CONFIGURATION -r osx-x64 --self-contained true -p:PublishSingleFile=false -o "./publish/osx-x64"

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

# Create .app bundle structure
echo "📦 Creating .app bundle..."
APP_DIR="./dist/$APP_NAME.app"
mkdir -p "$APP_DIR/Contents/MacOS"
mkdir -p "$APP_DIR/Contents/Resources"

# Copy executable and dependencies
cp -r ./publish/osx-x64/* "$APP_DIR/Contents/MacOS/"

# Create Info.plist
echo "📝 Creating Info.plist..."
cat > "$APP_DIR/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>QuickMind</string>
    <key>CFBundleIconFile</key>
    <string>QuickMind.icns</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.education</string>
</dict>
</plist>
EOF

# Convert icon to .icns format (if iconutil is available)
if command -v iconutil &> /dev/null; then
    echo "🎨 Converting icon to .icns format..."
    # This requires the icon to be converted to .icns format
    # For now, we'll skip this step - users can manually add icon later
    echo "⚠️  Icon conversion skipped. Please manually add QuickMind.icns to Contents/Resources/"
else
    echo "⚠️  iconutil not found. Icon conversion skipped."
fi

# Make executable
chmod +x "$APP_DIR/Contents/MacOS/QuickMind"

# Create DMG
if command -v hdiutil &> /dev/null; then
    echo "💿 Creating DMG..."
    
    # Create temporary directory for DMG content
    DMG_DIR="./dist/dmg_temp"
    mkdir -p "$DMG_DIR"
    
    # Copy app to DMG directory
    cp -r "$APP_DIR" "$DMG_DIR/"
    
    # Create symlink to Applications
    ln -s /Applications "$DMG_DIR/Applications"
    
    # Create DMG
    hdiutil create -srcfolder "$DMG_DIR" -volname "$APP_NAME v$VERSION" -fs HFS+ -fsargs "-c c=64,a=16,e=16" -format UDBZ -size 200m "./dist/$APP_NAME-v$VERSION.dmg"
    
    # Clean up
    rm -rf "$DMG_DIR"
    
    if [ $? -eq 0 ]; then
        echo "✅ macOS installer created successfully: ./dist/$APP_NAME-v$VERSION.dmg"
        
        # Display file info
        ls -lh "./dist/$APP_NAME-v$VERSION.dmg"
    else
        echo "❌ DMG creation failed!"
        echo "📱 App bundle created at: $APP_DIR"
    fi
else
    echo "⚠️  hdiutil not found. DMG creation skipped."
    echo "📱 App bundle created at: $APP_DIR"
fi

echo "🎉 Build completed!"

# Instructions
echo ""
echo "📋 Next steps:"
echo "1. Test the .app bundle: open '$APP_DIR'"
echo "2. To sign the app (for distribution): codesign --force --deep --sign 'Developer ID Application: Your Name' '$APP_DIR'"
echo "3. To notarize (for macOS Gatekeeper): xcrun notarytool submit ..."
echo "4. For icon: Create QuickMind.icns and place in '$APP_DIR/Contents/Resources/'"