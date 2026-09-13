#!/usr/bin/env bash
set -euo pipefail

APP_NAME="GLS Companion"
PACKAGE_NAME="gls-companion"
VERSION="${1:-1.0.0}"
ARCH="amd64"

SOURCE_DIR="${2:-$HOME/Downloads/GLSCompanion-Final}"
OUTPUT_DIR="${3:-$HOME/Downloads}"
PACKAGE_ROOT="$(mktemp -d /tmp/glscompanion-deb.XXXXXX)"
OUTPUT_FILE="${OUTPUT_DIR}/GLSCompanion_${VERSION}_${ARCH}.deb"

cleanup() {
    rm -rf "$PACKAGE_ROOT"
}
trap cleanup EXIT

APP_DIR="$PACKAGE_ROOT/opt/gls-companion"
BIN_DIR="$PACKAGE_ROOT/usr/bin"
DESKTOP_DIR="$PACKAGE_ROOT/usr/share/applications"
DEBIAN_DIR="$PACKAGE_ROOT/DEBIAN"

echo "=== GLS Companion .deb Builder ==="
echo "Version:    $VERSION"
echo "Source:     $SOURCE_DIR"
echo "Output:     $OUTPUT_FILE"
echo

if [[ ! -d "$SOURCE_DIR" ]]; then
    echo "ERROR: Source directory not found:"
    echo "  $SOURCE_DIR"
    exit 1
fi

if [[ ! -f "$SOURCE_DIR/GLS.Companion.CrossPlatform" ]]; then
    echo "ERROR: GLS.Companion.CrossPlatform executable not found in:"
    echo "  $SOURCE_DIR"
    exit 1
fi

mkdir -p "$APP_DIR" "$BIN_DIR" "$DESKTOP_DIR" "$DEBIAN_DIR" "$OUTPUT_DIR"

echo "Copying validated Linux release..."
cp -a "$SOURCE_DIR/." "$APP_DIR/"
chmod +x "$APP_DIR/GLS.Companion.CrossPlatform"

cat > "$BIN_DIR/gls-companion" <<'EOF'
#!/usr/bin/env bash
exec /opt/gls-companion/GLS.Companion.CrossPlatform "$@"
EOF
chmod 0755 "$BIN_DIR/gls-companion"

cat > "$DESKTOP_DIR/gls-companion.desktop" <<'EOF'
[Desktop Entry]
Type=Application
Name=GLS Companion
Comment=Great Lakes Syndicate Star Citizen cargo and salvage companion
Exec=/usr/bin/gls-companion
Icon=applications-games
Terminal=false
Categories=Game;Utility;
StartupNotify=true
EOF
chmod 0644 "$DESKTOP_DIR/gls-companion.desktop"

cat > "$DEBIAN_DIR/control" <<EOF
Package: $PACKAGE_NAME
Version: $VERSION
Section: games
Priority: optional
Architecture: $ARCH
Maintainer: Great Lakes Syndicate
Depends: libwebkit2gtk-4.1-0, libgtk-3-0, libx11-6, libxtst6, libxkbcommon0, libxrandr2, libxext6, libsm6, libice6, libxrender1
Description: Great Lakes Syndicate Companion
 GLS Companion provides desktop access to the GLS Cargo and Salvage tools
 with Full, Compact, Run, and Park modes plus global F9 visibility control.
 .
 Global F9 currently requires an X11/Xorg desktop session.
EOF
chmod 0644 "$DEBIAN_DIR/control"

cat > "$DEBIAN_DIR/postinst" <<'EOF'
#!/usr/bin/env bash
set -e

if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database /usr/share/applications >/dev/null 2>&1 || true
fi

exit 0
EOF
chmod 0755 "$DEBIAN_DIR/postinst"

cat > "$DEBIAN_DIR/postrm" <<'EOF'
#!/usr/bin/env bash
set -e

if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database /usr/share/applications >/dev/null 2>&1 || true
fi

exit 0
EOF
chmod 0755 "$DEBIAN_DIR/postrm"

echo "Building Debian package..."
dpkg-deb --build --root-owner-group "$PACKAGE_ROOT" "$OUTPUT_FILE"

echo
echo "SUCCESS"
echo "Created:"
echo "  $OUTPUT_FILE"
echo
echo "Install with:"
echo "  sudo apt install \"$OUTPUT_FILE\""
echo
echo "Launch with:"
echo "  gls-companion"
