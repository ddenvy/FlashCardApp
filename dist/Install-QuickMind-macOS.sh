#!/bin/bash

# QuickMind macOS Installation Script
echo "🍎 Installing QuickMind for macOS..."

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "❌ This script must be run on macOS"
    exit 1
fi

# Set variables
APP_NAME="QuickMind.app"
INSTALL_DIR="/Applications"
CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "📂 Current directory: $CURRENT_DIR"
echo "📱 Installing $APP_NAME to $INSTALL_DIR"

# Check if app bundle exists
if [ ! -d "$CURRENT_DIR/$APP_NAME" ]; then
    echo "❌ $APP_NAME not found in current directory"
    echo "Please make sure $APP_NAME is in the same folder as this script"
    exit 1
fi

# Ask for permission to install
echo ""
read -p "🤔 Install QuickMind to Applications folder? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Installation cancelled"
    exit 1
fi

# Remove existing installation if present
if [ -d "$INSTALL_DIR/$APP_NAME" ]; then
    echo "🗑️  Removing existing QuickMind installation..."
    sudo rm -rf "$INSTALL_DIR/$APP_NAME"
fi

# Copy app to Applications
echo "📦 Copying QuickMind to Applications..."
sudo cp -R "$CURRENT_DIR/$APP_NAME" "$INSTALL_DIR/"

# Set executable permissions
echo "🔧 Setting permissions..."
sudo chmod +x "$INSTALL_DIR/$APP_NAME/Contents/MacOS/QuickMind"
sudo chmod -R 755 "$INSTALL_DIR/$APP_NAME"

# Fix attributes for Gatekeeper
echo "🛡️  Setting macOS attributes..."
sudo xattr -cr "$INSTALL_DIR/$APP_NAME"

# Verify installation
if [ -d "$INSTALL_DIR/$APP_NAME" ]; then
    echo "✅ QuickMind installed successfully!"
    echo ""
    echo "🚀 You can now:"
    echo "   1. Find QuickMind in Applications folder"
    echo "   2. Add to Dock by dragging from Applications"
    echo "   3. Launch by double-clicking or from Spotlight"
    echo ""
    echo "📝 Note: On first launch, you may need to:"
    echo "   - Right-click → Open (if Gatekeeper shows warning)"
    echo "   - Go to System Preferences → Security & Privacy to allow"
    echo ""
    
    # Ask to launch now
    read -p "🤔 Launch QuickMind now? (Y/n): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Nn]$ ]]; then
        echo "🚀 Launching QuickMind..."
        open "$INSTALL_DIR/$APP_NAME"
    fi
    
    echo "🎉 Installation complete!"
else
    echo "❌ Installation failed"
    exit 1
fi