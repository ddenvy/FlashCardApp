# QuickMind - Linux Installation Guide

QuickMind is available for Linux in various package formats. Choose the most suitable one for your distribution.

## 📦 Available Package Formats

### 1. DEB packages (Debian/Ubuntu)
- **quickmind_VERSION_amd64.deb** - for x64 systems
- **quickmind_VERSION_arm64.deb** - for ARM64 systems

### 2. RPM packages (Red Hat/Fedora/openSUSE)
- **quickmind-VERSION-1.*.x86_64.rpm** - for x64 systems
- **quickmind-VERSION-1.*.aarch64.rpm** - for ARM64 systems

### 3. AppImage (universal)
- **QuickMind-VERSION-x86_64.AppImage** - for x64 systems
- **QuickMind-VERSION-aarch64.AppImage** - for ARM64 systems

### 4. Flatpak
- **QuickMind-VERSION.flatpak** - universal package

### 5. Snap
- **quickmind_VERSION_amd64.snap** - for x64 systems

### 6. Source packages
- **quickmind-VERSION-src.tar.gz** - source code for building

## 🔧 Installation Instructions

### Debian/Ubuntu (.deb)

```bash
# Download the appropriate .deb file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind_VERSION_amd64.deb

# Installation
sudo dpkg -i quickmind_VERSION_amd64.deb

# If there are dependency issues
sudo apt-get install -f

# Launch
quickmind
```

### Red Hat/Fedora/CentOS (.rpm)

```bash
# Download the appropriate .rpm file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-1.*.x86_64.rpm

# Installation on Fedora/CentOS
sudo dnf install quickmind-VERSION-1.*.x86_64.rpm

# Installation on older versions
sudo rpm -i quickmind-VERSION-1.*.x86_64.rpm

# Launch
quickmind
```

### openSUSE (.rpm)

```bash
# Download the appropriate .rpm file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-1.*.x86_64.rpm

# Installation
sudo zypper install quickmind-VERSION-1.*.x86_64.rpm

# Launch
quickmind
```

### AppImage (universal)

```bash
# Download AppImage file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/QuickMind-VERSION-x86_64.AppImage

# Make executable
chmod +x QuickMind-VERSION-x86_64.AppImage

# Launch
./QuickMind-VERSION-x86_64.AppImage

# Optional: system integration
# You can move the file to ~/.local/bin or /usr/local/bin
```

### Flatpak

```bash
# Download Flatpak file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/QuickMind-VERSION.flatpak

# Installation
flatpak install --user QuickMind-VERSION.flatpak

# Launch
flatpak run com.quickmind.QuickMind

# Alternatively through GUI
# The app will appear in your application list
```

### Snap

```bash
# Download Snap file
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind_VERSION_amd64.snap

# Installation
sudo snap install quickmind_VERSION_amd64.snap --dangerous

# Launch
quickmind
```

### Arch Linux

```bash
# Download source archive
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-src.tar.gz

# Extract archive
tar -xzf quickmind-VERSION-src.tar.gz

# Find PKGBUILD file in arch-package/
cd quickmind-VERSION/arch-package

# Build package
makepkg -si

# Launch
quickmind
```

### Gentoo

```bash
# Download source archive
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-src.tar.gz

# Extract archive
tar -xzf quickmind-VERSION-src.tar.gz

# Copy ebuild to your local overlay
sudo cp -r quickmind-VERSION/gentoo-package/app-misc/quickmind /usr/local/portage/app-misc/

# Create manifest
cd /usr/local/portage/app-misc/quickmind
sudo ebuild quickmind-VERSION.ebuild manifest

# Installation
sudo emerge quickmind

# Launch
quickmind
```

### Building from Source

```bash
# Requirements: .NET 9.0 SDK
# Install .NET SDK for your distribution

# Download source code
git clone https://github.com/ddenvy/QuickMind.git
cd QuickMind

# Restore dependencies
dotnet restore

# Build
dotnet build -c Release

# Launch
dotnet run -c Release
```

## 📋 System Requirements

### Minimum Requirements
- **CPU**: x64 or ARM64 processor
- **RAM**: 512 MB
- **Disk**: 50 MB free space
- **OS**: Linux kernel 3.17+ (glibc 2.17+)

### Supported Distributions

#### Debian-based
- Ubuntu 18.04 LTS and later
- Debian 9 (Stretch) and later
- Linux Mint 19 and later
- Pop!_OS 18.04 and later

#### Red Hat-based
- Fedora 29 and later
- CentOS 8 and later
- RHEL 8 and later
- Rocky Linux 8 and later
- AlmaLinux 8 and later

#### SUSE-based
- openSUSE Leap 15.0 and later
- openSUSE Tumbleweed
- SLES 15 and later

#### Arch-based
- Arch Linux (rolling release)
- Manjaro
- EndeavourOS

#### Gentoo
- Gentoo Linux (current)

### Dependencies
QuickMind automatically includes necessary dependencies, but some distributions may require additional packages:

```bash
# Debian/Ubuntu
sudo apt-get install libc6 libgcc-s1 libssl3

# Fedora/CentOS
sudo dnf install glibc libgcc openssl-libs

# openSUSE
sudo zypper install glibc libgcc_s1 libopenssl3

# Arch Linux
sudo pacman -S glibc gcc-libs openssl
```

## 🐛 Troubleshooting

### Application won't start

1. **Check dependencies**:
   ```bash
   ldd /usr/bin/QuickMind  # for installed packages
   ldd ./QuickMind         # for AppImage after mounting
   ```

2. **Check permissions**:
   ```bash
   chmod +x /path/to/QuickMind
   ```

3. **Run from terminal to see errors**:
   ```bash
   /usr/bin/QuickMind  # or path to your executable
   ```

### Display issues

1. **For X11**:
   ```bash
   export DISPLAY=:0
   quickmind
   ```

2. **For Wayland**:
   ```bash
   export GDK_BACKEND=wayland
   quickmind
   ```

### Permission issues (Flatpak/Snap)

1. **Flatpak - home directory access**:
   ```bash
   flatpak override --user --filesystem=home com.quickmind.QuickMind
   ```

2. **Snap - interface connections**:
   ```bash
   sudo snap connect quickmind:home :home
   ```

## 🔄 Updating

### DEB/RPM packages
Download and install the new version similar to the first installation.

### AppImage
Simply download the new AppImage file and replace the old one.

### Flatpak
```bash
flatpak update com.quickmind.QuickMind
```

### Snap
```bash
sudo snap refresh quickmind
```

## 🗑️ Uninstalling

### DEB packages
```bash
sudo apt-get remove quickmind
```

### RPM packages
```bash
sudo dnf remove quickmind  # Fedora/CentOS
sudo zypper remove quickmind  # openSUSE
```

### AppImage
Simply delete the AppImage file.

### Flatpak
```bash
flatpak uninstall com.quickmind.QuickMind
```

### Snap
```bash
sudo snap remove quickmind
```

## 📞 Support

If you encounter issues with installing or using QuickMind on Linux:

1. Check [GitHub Issues](https://github.com/ddenvy/QuickMind/issues)
2. Create a new issue with problem description
3. Include your distribution version and processor architecture

---

**Tip**: For most users, we recommend using DEB/RPM packages for better system integration, or AppImage for maximum compatibility. 