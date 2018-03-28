#!/bin/bash
# This script will start up the virtual machine development environment
# You need to make sure VT-x or AMD-V is enabled in your BIOS settings
# You can set the amount of RAM in megabytes here:
RAM=2048

qemu-system-x86_64 -drive media=disk,file=HoloLensDev.qcow2,format=qcow2,index=0 -m $RAM -cpu host --enable-kvm -usbdevice tablet -boot d
