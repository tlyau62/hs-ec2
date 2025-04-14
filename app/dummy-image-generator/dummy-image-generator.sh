#!/bin/bash

if ! command -v convert &> /dev/null; then
  echo "Error: ImageMagick is not installed. Install it with:"
  echo "sudo apt-get install imagemagick  # Debian/Ubuntu"
  echo "or"
  echo "sudo dnf install ImageMagick      # Fedora"
  exit 1
fi

mkdir ./imgs

for i in {1..10000}; do
  # Create a 64KB image with random noise, save as PNG
  convert -size 103x103 xc: +noise random "imgs/${i}.png"
  echo "Generated: ${i}.png"
done

echo "Done!"