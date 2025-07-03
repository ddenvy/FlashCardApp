# QuickMind - Инструкции по установке для Linux

QuickMind доступен для Linux в различных форматах пакетов. Выберите наиболее подходящий для вашего дистрибутива.

## 📦 Доступные форматы пакетов

### 1. DEB пакеты (Debian/Ubuntu)
- **quickmind_VERSION_amd64.deb** - для x64 систем
- **quickmind_VERSION_arm64.deb** - для ARM64 систем

### 2. RPM пакеты (Red Hat/Fedora/openSUSE)
- **quickmind-VERSION-1.*.x86_64.rpm** - для x64 систем
- **quickmind-VERSION-1.*.aarch64.rpm** - для ARM64 систем

### 3. AppImage (универсальный)
- **QuickMind-VERSION-x86_64.AppImage** - для x64 систем
- **QuickMind-VERSION-aarch64.AppImage** - для ARM64 систем

### 4. Flatpak
- **QuickMind-VERSION.flatpak** - универсальный пакет

### 5. Snap
- **quickmind_VERSION_amd64.snap** - для x64 систем

### 6. Исходники
- **quickmind-VERSION-src.tar.gz** - исходный код для сборки

## 🔧 Инструкции по установке

### Debian/Ubuntu (.deb)

```bash
# Скачайте соответствующий .deb файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind_VERSION_amd64.deb

# Установка
sudo dpkg -i quickmind_VERSION_amd64.deb

# Если есть проблемы с зависимостями
sudo apt-get install -f

# Запуск
quickmind
```

### Red Hat/Fedora/CentOS (.rpm)

```bash
# Скачайте соответствующий .rpm файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-1.*.x86_64.rpm

# Установка на Fedora/CentOS
sudo dnf install quickmind-VERSION-1.*.x86_64.rpm

# Установка на старых версиях
sudo rpm -i quickmind-VERSION-1.*.x86_64.rpm

# Запуск
quickmind
```

### openSUSE (.rpm)

```bash
# Скачайте соответствующий .rpm файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-1.*.x86_64.rpm

# Установка
sudo zypper install quickmind-VERSION-1.*.x86_64.rpm

# Запуск
quickmind
```

### AppImage (универсальный)

```bash
# Скачайте AppImage файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/QuickMind-VERSION-x86_64.AppImage

# Сделайте исполняемым
chmod +x QuickMind-VERSION-x86_64.AppImage

# Запуск
./QuickMind-VERSION-x86_64.AppImage

# Опционально: интеграция с системой
# Для этого можно переместить файл в ~/.local/bin или /usr/local/bin
```

### Flatpak

```bash
# Скачайте Flatpak файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/QuickMind-VERSION.flatpak

# Установка
flatpak install --user QuickMind-VERSION.flatpak

# Запуск
flatpak run com.quickmind.QuickMind

# Альтернативно через GUI
# Файл появится в списке приложений
```

### Snap

```bash
# Скачайте Snap файл
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind_VERSION_amd64.snap

# Установка
sudo snap install quickmind_VERSION_amd64.snap --dangerous

# Запуск
quickmind
```

### Arch Linux

```bash
# Скачайте исходный архив
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-src.tar.gz

# Распакуйте архив
tar -xzf quickmind-VERSION-src.tar.gz

# Найдите PKGBUILD файл в arch-package/
cd quickmind-VERSION/arch-package

# Сборка пакета
makepkg -si

# Запуск
quickmind
```

### Gentoo

```bash
# Скачайте исходный архив
wget https://github.com/ddenvy/QuickMind/releases/download/vVERSION/quickmind-VERSION-src.tar.gz

# Распакуйте архив
tar -xzf quickmind-VERSION-src.tar.gz

# Скопируйте ebuild в ваш локальный оверлей
sudo cp -r quickmind-VERSION/gentoo-package/app-misc/quickmind /usr/local/portage/app-misc/

# Создайте манифест
cd /usr/local/portage/app-misc/quickmind
sudo ebuild quickmind-VERSION.ebuild manifest

# Установка
sudo emerge quickmind

# Запуск
quickmind
```

### Сборка из исходников

```bash
# Требования: .NET 9.0 SDK
# Установите .NET SDK для вашего дистрибутива

# Скачайте исходный код
git clone https://github.com/ddenvy/QuickMind.git
cd QuickMind

# Восстановите зависимости
dotnet restore

# Сборка
dotnet build -c Release

# Запуск
dotnet run -c Release
```

## 📋 Системные требования

### Минимальные требования
- **CPU**: x64 или ARM64 процессор
- **RAM**: 512 MB
- **Диск**: 50 MB свободного места
- **ОС**: Linux kernel 3.17+ (glibc 2.17+)

### Поддерживаемые дистрибутивы

#### Debian-based
- Ubuntu 18.04 LTS и выше
- Debian 9 (Stretch) и выше
- Linux Mint 19 и выше
- Pop!_OS 18.04 и выше

#### Red Hat-based
- Fedora 29 и выше
- CentOS 8 и выше
- RHEL 8 и выше
- Rocky Linux 8 и выше
- AlmaLinux 8 и выше

#### SUSE-based
- openSUSE Leap 15.0 и выше
- openSUSE Tumbleweed
- SLES 15 и выше

#### Arch-based
- Arch Linux (rolling release)
- Manjaro
- EndeavourOS

#### Gentoo
- Gentoo Linux (current)

### Зависимости
QuickMind автоматически включает необходимые зависимости, но для некоторых дистрибутивов могут потребоваться дополнительные пакеты:

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

## 🐛 Решение проблем

### Приложение не запускается

1. **Проверьте зависимости**:
   ```bash
   ldd /usr/bin/QuickMind  # для установленных пакетов
   ldd ./QuickMind         # для AppImage после монтирования
   ```

2. **Проверьте права доступа**:
   ```bash
   chmod +x /path/to/QuickMind
   ```

3. **Запустите из терминала для просмотра ошибок**:
   ```bash
   /usr/bin/QuickMind  # или путь к вашему исполняемому файлу
   ```

### Проблемы с дисплеем

1. **Для X11**:
   ```bash
   export DISPLAY=:0
   quickmind
   ```

2. **Для Wayland**:
   ```bash
   export GDK_BACKEND=wayland
   quickmind
   ```

### Проблемы с разрешениями (Flatpak/Snap)

1. **Flatpak - доступ к домашней директории**:
   ```bash
   flatpak override --user --filesystem=home com.quickmind.QuickMind
   ```

2. **Snap - подключение интерфейсов**:
   ```bash
   sudo snap connect quickmind:home :home
   ```

## 🔄 Обновление

### DEB/RPM пакеты
Скачайте и установите новую версию аналогично первой установке.

### AppImage
Просто скачайте новый AppImage файл и замените старый.

### Flatpak
```bash
flatpak update com.quickmind.QuickMind
```

### Snap
```bash
sudo snap refresh quickmind
```

## 🗑️ Удаление

### DEB пакеты
```bash
sudo apt-get remove quickmind
```

### RPM пакеты
```bash
sudo dnf remove quickmind  # Fedora/CentOS
sudo zypper remove quickmind  # openSUSE
```

### AppImage
Просто удалите файл AppImage.

### Flatpak
```bash
flatpak uninstall com.quickmind.QuickMind
```

### Snap
```bash
sudo snap remove quickmind
```

## 📞 Поддержка

Если у вас возникли проблемы с установкой или использованием QuickMind на Linux:

1. Проверьте [Issues на GitHub](https://github.com/ddenvy/QuickMind/issues)
2. Создайте новый issue с описанием проблемы
3. Укажите вашу версию дистрибутива и архитектуру процессора

---

**Совет**: Для большинства пользователей рекомендуется использовать DEB/RPM пакеты для лучшей интеграции с системой, или AppImage для максимальной совместимости. 